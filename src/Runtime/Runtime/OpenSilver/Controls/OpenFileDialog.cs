
/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CSHTML5.Internal;
using OpenSilver.Internal;
using OpenSilver.IO;

namespace OpenSilver.Controls;

/// <summary>
/// Provides a dialog box that enables the user to select one or more files.
/// </summary>
public sealed class OpenFileDialog
{
    private static readonly ReferenceIDGenerator _idGenerator = new();
    private readonly int _id;
    private readonly OpenFileDialogCallbacks _callbacks;
    private TaskCompletionSource<bool?> _tcs;
    private MemoryFileInfo[] _files;
    private bool _multiselect = false;
    private string _filter = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenFileDialog"/> class.
    /// </summary>
    public OpenFileDialog()
    {
        _id = _idGenerator.NewId();
        _callbacks = new OpenFileDialogCallbacks();
        Interop.ExecuteJavaScriptVoidAsync(
            $"document.openFileDialog.createDialog('{_id.ToInvariantString()}', {Interop.GetVariableStringForJS(_callbacks.Change)}, {Interop.GetVariableStringForJS(_callbacks.ChangeComplete)}, {Interop.GetVariableStringForJS(_callbacks.Cancel)})");
    }

    ~OpenFileDialog()
    {
        _callbacks.Dispose();
        Interop.ExecuteJavaScriptVoidAsync(
            $"document.openFileDialog.deleteDialog('{_id.ToInvariantString()}')");
    }

    /// <summary>
    /// Gets or sets a value that indicates whether the <see cref="OpenFileDialog"/>
    /// allows users to select multiple files.
    /// </summary>
    /// <returns>
    /// true if multiple selections are allowed; otherwise, false. The default is false.
    /// </returns>
    public bool Multiselect
    {
        get => _multiselect;
        set
        {
            if (_multiselect == value) return;

            _multiselect = value;
            Interop.ExecuteJavaScriptVoidAsync(
                $"document.openFileDialog.setMultiple('{_id.ToInvariantString()}', {(value ? "true" : "false")})");
        }
    }

    /// <summary>
    /// Gets or sets a filter string that specifies the file types and descriptions to
    /// display in the <see cref="OpenFileDialog"/>.
    /// </summary>
    /// <returns>
    /// A filter string that specifies the file types and descriptions to display in
    /// the <see cref="OpenFileDialog"/>. The default is <see cref="string.Empty"/>.
    /// </returns>
    public string Filter
    {
        get => _filter;
        set
        {
            if (_filter == value) return;

            _filter = value;

            string accept = string.Empty;

            if (!string.IsNullOrEmpty(value))
            {
                // Process the filter list to convert the syntax from XAML to HTML5:
                // Example of syntax in Silverlight: Image Files (*.bmp, *.jpg)|*.bmp;*.jpg|All Files (*.*)|*.*
                // Example of syntax in HTML5: .gif, .jpg, .png, .doc
                string[] split = value.Split('|');

                if (split.Length <= 2)
                {
                    if (split[split.Length - 1] != "*.*")
                    {
                        accept = split[split.Length - 1].Replace("*", "").Replace(';', ',');
                    }
                }
                else
                {
                    var builder = new StringBuilder();

                    int i = 1;
                    while (i < split.Length)
                    {
                        if (split[i] == "*.*")
                        {
                            i += 2;
                            continue;
                        }

                        builder.Append(split[i].Replace("*", "").Replace(';', ','));
                        i += 2;
                        break;
                    }

                    for (; i < split.Length; i += 2)
                    {
                        if (split[i] != "*.*")
                        {
                            builder.Append(',');
                            builder.Append(split[i].Replace("*", "").Replace(';', ','));
                        }
                    }

                    accept = builder.ToString();
                }
            }

            Interop.ExecuteJavaScriptVoidAsync(
                $"document.openFileDialog.setAccept('{_id.ToInvariantString()}', '{accept}')");
        }
    }

    /// <summary>
    /// Gets a <see cref="MemoryFileInfo"/> object for the selected file. If multiple files are
    /// selected, returns the first selected file.
    /// </summary>
    /// <returns>
    /// The selected file. If multiple files are selected, returns the first selected
    /// file.
    /// </returns>
    public MemoryFileInfo File => _files?.FirstOrDefault();

    /// <summary>
    /// Gets a collection of <see cref="MemoryFileInfo"/> objects for the selected files.
    /// </summary>
    public IEnumerable<MemoryFileInfo> Files => _files;

    /// <summary>
    /// Gets or sets the index of the selected item in the <see cref="OpenFileDialog"/>
    /// filter drop-down list.
    /// </summary>
    /// <returns>
    /// The index of the selected item in the <see cref="OpenFileDialog"/>
    /// filter drop-down list. The default is 1.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The filter index is less than 1.
    /// </exception>
    [NotImplemented]
    public int FilterIndex { get; set; }

    /// <summary>
    /// Gets or sets the directory displayed when the dialog starts.
    /// </summary>
    /// <returns>
    /// The directory displayed when the dialog starts. The default is an empty string.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// The directory specified is not a valid file path.
    /// </exception>
    [NotImplemented]
    public string InitialDirectory { get; set; }

    /// <summary>
    /// Opens the default browser file dialog. This returns a Task, differently from Silverlight.
    /// This is because it is not possible to wait for the dialog to conclude, since the process is single-threaded.
    /// </summary>
    /// <returns>
    /// A Task that will have the result of the file dialog. True for files selected, false for cancel/exit.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// The browser was unable to open the dialog.
    /// </exception>
    public Task<bool?> ShowDialogAsync() => ShowDialogAsync(null);

    /// <summary>
    /// Opens the default browser file dialog. This returns a Task, differently from Silverlight.
    /// This is because it is not possible to wait for the dialog to conclude, since the process is single-threaded.
    /// </summary>
    /// <param name="owner"></param>
    /// <returns>
    /// A Task that will have the result of the file dialog. True for files selected, false for cancel/exit.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// The browser was unable to open the dialog.
    /// </exception>
    public Task<bool?> ShowDialogAsync(Window owner)
    {
        string message = Interop.ExecuteJavaScriptString($"document.openFileDialog.showDialog('{_id.ToInvariantString()}')");

        if (!string.IsNullOrEmpty(message))
        {
            throw new InvalidOperationException(message);
        }

        // Before opening the file dialog, we add a strong reference to the callbacks to prevent this object from being
        // garbage collected while dialog is used. The reference is later cleared after receiving the 'change' or 'cancel'
        // event to avoid memory leak.
        _callbacks.Owner = this;
        _callbacks.ResetFiles();

        _tcs = new TaskCompletionSource<bool?>();
        return _tcs.Task;
    }

    private sealed class OpenFileDialogCallbacks : IDisposable
    {
        private readonly List<MemoryFileInfo> _files;

        public OpenFileDialogCallbacks()
        {
            _files = new();
            Change = JavaScriptCallback.Create(OnChanged);
            ChangeComplete = JavaScriptCallback.Create(OnChangeCompleted);
            Cancel = JavaScriptCallback.Create(OnCancelled);
        }

        public OpenFileDialog Owner { get; set; }

        public JavaScriptCallback Change { get; }

        public JavaScriptCallback ChangeComplete { get; }

        public JavaScriptCallback Cancel { get; }

        public void ResetFiles() => _files.Clear();

        private void OnChanged(string fileName, string b64Data)
        {
            byte[] bytes = b64Data is null ? [] : Convert.FromBase64String(b64Data);
            _files.Add(new MemoryFileInfo(fileName, bytes));
        }

        private void OnChangeCompleted()
        {
            (OpenFileDialog owner, Owner) = (Owner, null);
            MemoryFileInfo[] files = _files.ToArray();
            _files.Clear();

            owner._files = files;
            owner._tcs.SetResult(true);
        }

        private void OnCancelled()
        {
            (OpenFileDialog owner, Owner) = (Owner, null);
            _files.Clear();

            owner._tcs.SetResult(false);
        }

        public void Dispose()
        {
            Change.Dispose();
            ChangeComplete.Dispose();
            Cancel.Dispose();
        }
    }
}
