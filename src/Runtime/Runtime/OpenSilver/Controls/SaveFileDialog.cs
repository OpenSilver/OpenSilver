
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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Diagnostics;
using System.Threading;
using CSHTML5.Internal;

namespace OpenSilver.Controls;

/// <summary>
/// Provides a dialog box that enables the user to specify options for saving a file.
/// </summary>
public sealed class SaveFileDialog
{
    private static bool? _isFileSystemApiAvailable;

    private object _handle;
    private string _filter = string.Empty;
    private int _filterIndex = 1;
    private List<(string Description, string[] Patterns)> _filterEntries;

    private static bool IsFileSystemApiAvailable
    {
        get
        {
            _isFileSystemApiAvailable ??= Interop.ExecuteJavaScriptBoolean("typeof window.showSaveFilePicker === 'function'");
            return _isFileSystemApiAvailable.GetValueOrDefault();
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SaveFileDialog"/> class.
    /// </summary>
    public SaveFileDialog() { }

    /// <summary>
    /// Gets or sets a filter string that specifies the files types and descriptions to display in the <see cref="SaveFileDialog"/>.
    /// </summary>
    /// <returns>
    /// A filter string that specifies the file types and descriptions to display in the <see cref="SaveFileDialog"/>.
    /// The default is <see cref="string.Empty"/>.
    /// </returns>
    public string Filter
    {
        get => _filter;
        set
        {
            _filter = value;

            if (string.IsNullOrEmpty(value))
            {
                _filterEntries = null;
                return;
            }

            _filterEntries = new();

            // Example of a filter: "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
            string[] filterTokens = value.Split('|');
            string description = string.Empty;
            for (int i = 0; i < filterTokens.Length; i++)
            {
                // Even indices are for file descriptions
                if (i % 2 == 0)
                {
                    description = filterTokens[i].Trim();
                }
                else // Odd indices are for file extensions
                {
                    string[] filterPatterns = filterTokens[i].Split(';').Select(p => p.Trim()).ToArray();
                    _filterEntries.Add((description, filterPatterns));
                    description = string.Empty;
                }
            }
        }
    }

    /// <summary>
    /// Gets or sets the index of the selected item in the Save as type drop-down list.
    /// </summary>
    /// <returns>
    /// The index of the selected item in the Save as type filter drop-down list. The default is 1.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The filter index is less than 1.
    /// </exception>
    public int FilterIndex
    {
        get => _filterIndex;
        set
        {
            if (value < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            _filterIndex = value;
        }
    }

    /// <summary>
    /// Gets the file name for the selected file associated with the <see cref="SaveFileDialog"/>.
    /// </summary>
    /// <returns>
    /// The file name for the selected file associated with the <see cref="SaveFileDialog"/>.
    /// The default is <see cref="string.Empty"/>.
    /// </returns>
    public string SafeFileName => Path.GetFileName(GetFilename());

    /// <summary>
    /// Gets or sets the default file name extension applied to files that are saved with the <see cref="SaveFileDialog"/>.
    /// </summary>
    /// <returns>
    /// The default file name extension applied to files that are saved with the <see cref="SaveFileDialog"/>, which can 
    /// optionally include the dot character (.).
    /// </returns>
    public string DefaultExt { get; set; }

    /// <summary>
    /// Gets or sets the file name used if a file name is not specified by the user.
    /// </summary>
    /// <returns>
    /// The file name used if a file name is not specified by the user.
    /// </returns>
    public string DefaultFileName { get; set; }

    /// <summary>
    /// Opens the file specified by the <see cref="SafeFileName"/> property.
    /// </summary>
    /// <returns>
    /// A write-only stream for the file specified by the <see cref="SafeFileName"/> property.
    /// </returns>
    public Task<Stream> OpenFileAsync()
    {
        var fileSaver = new FileSaver();
        return fileSaver.OpenFileAsync(_handle, SafeFileName);
    }

    /// <summary>
    /// Displays a <see cref="SaveFileDialog"/> that is modal to the Web browser or main window.
    /// </summary>
    /// <returns>
    /// true if the user clicked Save; false if the user clicked Cancel or closed the dialog box.
    /// </returns>
    public async Task<bool?> ShowDialogAsync()
    {
        // In Silverlight, when DefaultFileName is set, a security warning MessageBox is shown before the dialog.
        if (!string.IsNullOrEmpty(DefaultFileName))
        {
            var messageBoxResult = MessageBox.Show(
                $"Do you want to save '{DefaultFileName}' ?",
                "File Download - Security Warning",
                MessageBoxButton.OKCancel);

            if (messageBoxResult == MessageBoxResult.Cancel)
            {
                return false;
            }
        }

        if (!IsFileSystemApiAvailable)
        {
            return true;
        }

        var tcs = new TaskCompletionSource<object>();

        var onSuccess = JavaScriptCallback.Create(() => tcs.SetResult(null));
        var onError = JavaScriptCallback.Create((string error) =>
        {
            // Errors thrown when dialogs are canceled are suppressed
            if (!error.StartsWith("AbortError"))
            {
                tcs.SetException(new InvalidOperationException(error));
            }
        });

        bool dialogResult;

        try
        {
            _handle = Interop.ExecuteJavaScript(
                $$"""
                try {
                    (async () => {
                        const opts = {
                            startIn: 'downloads',
                            suggestedName: $2,
                            types: {{TranslateFilterToTypes()}}
                        };
                        return await window.showSaveFilePicker(opts);
                    })().then(handle => { $0(); return handle; }, error => { $1(error.toString()) });
                } catch (error) {
                    console.error(error);
                    throw error;
                }
                """,
                onSuccess,
                onError,
                SafeFileName);

            await tcs.Task;

            dialogResult = true;
        }
        catch (OperationCanceledException)
        {
            // Cancel button was hit on file dialog
            dialogResult = false;
        }
        catch
        {
            // Falling back to not using File System API as it might not be supported on this browser
            _isFileSystemApiAvailable = false;
            dialogResult = true;
        }

        onSuccess.Dispose();
        onError.Dispose();

        return dialogResult;
    }

    private string GetFilename()
    {
        if (_filterEntries.Count < FilterIndex)
        {
            throw new InvalidOperationException("FilterIndex is out of bounds of Filter");
        }

        // https://learn.microsoft.com/en-us/previous-versions/windows/silverlight/dotnet-windows-silverlight/dd459587(v=vs.95)
        // The default file name extension is applied when the selected filter does not specify an extension
        // (or the Filter property is not set) and the user does not specify an extension.
        if ((string.IsNullOrEmpty(Filter) || _filterEntries[FilterIndex - 1].Patterns.Contains("*.*")) &&
            !string.IsNullOrEmpty(DefaultFileName) &&
            !Path.HasExtension(DefaultFileName))
        {
            return Path.ChangeExtension(DefaultFileName, DefaultExt);
        }

        // In Firefox, if there is no default filename but there is a default extension, it is used.
        if (!IsFileSystemApiAvailable && (string.IsNullOrEmpty(DefaultFileName) || !Path.HasExtension(DefaultFileName)))
        {
            string filename = !string.IsNullOrEmpty(DefaultFileName) ? DefaultFileName : "download";

            if (!string.IsNullOrEmpty(Filter))
            {
                string firstFilterPattern = _filterEntries[FilterIndex - 1].Patterns.FirstOrDefault();
                string firstFilterExtension = null;
                if (!string.IsNullOrEmpty(firstFilterPattern))
                {
                    firstFilterExtension = Path.GetExtension(firstFilterPattern);
                }
                if (string.IsNullOrEmpty(firstFilterExtension))
                {
                    throw new InvalidOperationException("Unable to parse Filter extension pattern");
                }
                return Path.ChangeExtension(filename, firstFilterExtension);
            }

            if (!string.IsNullOrEmpty(DefaultExt))
            {
                return Path.ChangeExtension(filename, DefaultExt);
            }
        }
        return DefaultFileName;
    }

    /// <summary>
    /// Translates the legacy Filter member of Silverlight SaveFileDialog into the types option member of showSaveFilePicker().
    /// </summary>
    /// <returns>
    /// A string that defines the types option of showSaveFilePicker() to apply filters in the dialog.
    /// </returns>
    private string TranslateFilterToTypes()
    {
        // Example of a types option for showSaveFilePicker:
        // const opts = {
        //     types: [
        //         {
        //             description: "Text file",
        //             accept: { "text/plain": [".txt"] },
        //         },
        //     ],
        // };

        if (_filterEntries != null)
        {
            List<string> types = new();

            foreach ((string description, string[] patterns) in _filterEntries)
            {
                var acceptTypes = new Dictionary<string, List<string>>();
                foreach (string filterPattern in patterns)
                {
                    // Using .* on "accept" throws ("Extension '.*' contains invalid characters").
                    if (!string.IsNullOrEmpty(filterPattern) && !Path.GetExtension(filterPattern.Trim()).Contains("*"))
                    {
                        string mimeType = MimeMapping.GetMimeMapping(filterPattern.Trim());
                        if (!acceptTypes.TryGetValue(mimeType, out List<string> mimeTypes))
                        {
                            mimeTypes = new();
                            acceptTypes[mimeType] = mimeTypes;
                        }

                        mimeTypes.Add($"'{Path.GetExtension(filterPattern.Trim())}'");
                    }
                }

                types.Add($"{{ description: '{description}', accept: {{ {string.Join(",", acceptTypes.Select(kv => $"'{kv.Key}' : [{string.Join(",", kv.Value)}]"))} }} }}");
            }

            if (FilterIndex > 1)
            {
                if (_filterEntries.Count < FilterIndex)
                {
                    throw new InvalidOperationException("FilterIndex is out of bounds of Filter");
                }

                var type = types[FilterIndex - 1];
                types.RemoveAt(FilterIndex - 1);
                types.Insert(0, type);
            }

            return $"[{string.Join(",", types)}]";
        }
        return "[]";
    }

    private sealed class FileSaver
    {
        // This is the size in which writes don't seem to block for too long
        private const int BufferSize = 131072;

        private IDisposable _writableStream;
        private bool _isClosed = false;
        private string _filename;

        ~FileSaver() => Close();

        /// <summary>
        /// OpenFileAsync
        /// </summary>
        /// <param name="handle">
        /// JS Promise object that fulfills to a FileSystemWritableFileStream if File System API is available.
        /// </param>
        /// <param name="fallbackFilename">
        /// Filename to save in case downloads are managed by the browser.
        /// </param>
        public async Task<Stream> OpenFileAsync(object handle, string fallbackFilename)
        {
            if (IsFileSystemApiAvailable)
            {
                var tcs = new TaskCompletionSource<object>();

                var onSuccess = JavaScriptCallback.Create(() => tcs.SetResult(null));
                var onError = JavaScriptCallback.Create((string error) => tcs.SetException(new InvalidOperationException(error)));

                try
                {
                    _writableStream = Interop.ExecuteJavaScript(
                        """
                        try {
                            $0.then(async handle => await handle.createWritable())
                            .then(stream => { $1(); return stream; }, error => { console.error(error); $2(error.toString()) });
                        } catch (error) {
                            console.error(error);
                            throw error;
                        }
                        """,
                        handle,
                        onSuccess,
                        onError);

                    await tcs.Task;

                    onSuccess.Dispose();
                    onError.Dispose();
                }
                catch
                {
                    onSuccess.Dispose();
                    onError.Dispose();
                    throw;
                }
            }
            else
            {
                _writableStream = Interop.ExecuteJavaScript("[];");
                _filename = fallbackFilename;
            }

            return new WritableStream(this);
        }

        private async Task Write(byte[] bytes, int offset, int length)
        {
            if (bytes is null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            if ((uint)length > bytes.Length - offset)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            if (_writableStream is null)
            {
                throw new InvalidOperationException("Unable to write to an unopened file.");
            }

            int bytesToWrite = BufferSize;
            int i = offset;
            do
            {
                if (_isClosed)
                {
                    break;
                }

                if (i + bytesToWrite > length)
                {
                    bytesToWrite = length - i;
                }

                string base64 = Convert.ToBase64String(bytes, i, bytesToWrite);

                if (IsFileSystemApiAvailable)
                {
                    var tcs = new TaskCompletionSource<object>();

                    var onSuccess = JavaScriptCallback.Create(() => tcs.SetResult(null));
                    var onError = JavaScriptCallback.Create((string error) => tcs.SetException(new InvalidOperationException(error)));

                    try
                    {
                        Interop.ExecuteJavaScriptVoid(
                            """
                            try {
                                $1.then(async writableStream => {
                                    const binaryString = atob($0);
                                    let uint8 = new Uint8Array(binaryString.length);
                                    for (let i = 0; i < binaryString.length; i++) uint8[i] = binaryString.charCodeAt(i);
                                    await writableStream.write(uint8);
                                }).then(() => $2(), error => { console.error(error); $3(error.toString()) });
                            } catch (error) {
                                console.error(error);
                                throw error;
                            }
                            """,
                            base64,
                            _writableStream,
                            onSuccess,
                            onError);

                        await tcs.Task;

                        onSuccess.Dispose();
                        onError.Dispose();
                    }
                    catch
                    {
                        onSuccess.Dispose();
                        onError.Dispose();
                        throw;
                    }
                }
                else
                {
                    Interop.ExecuteJavaScriptVoid(
                        """
                        try {
                            const binaryString = atob($0);
                            for (let i in binaryString) {
                                $1.push(binaryString[i]);
                            }
                        } catch (error) {
                            console.error(error);
                            throw error;
                        }
                        """, base64, _writableStream);
                }

                // This loop could be long running for large files, so the file is written in
                // chunks and Delay is called to give control back to UI so it does not freeze.
                await Task.Delay(1);

                i += bytesToWrite;
            } while (i < bytes.Length - 1);
        }

        private async void Close()
        {
            if (_writableStream != null && !_isClosed)
            {
                if (IsFileSystemApiAvailable)
                {
                    var tcs = new TaskCompletionSource<object>();

                    var onSuccess = JavaScriptCallback.Create(() => tcs.SetResult(null));
                    var onError = JavaScriptCallback.Create((string error) =>
                    {
                        /* Errors are suppressed because it could just be that downloads were canceled */
                    });

                    try
                    {
                        Interop.ExecuteJavaScriptVoid(
                            """
                            try {
                                $0.then(async writableStream => await writableStream.close())
                                .then(() => $1(), error => { $2(error.toString()) });
                            } catch (error) {
                                console.error(error);
                                throw error;
                            }
                            """,
                            _writableStream,
                            onSuccess,
                            onError);

                        await tcs.Task;

                        onSuccess.Dispose();
                        onError.Dispose();
                    }
                    catch
                    {
                        onSuccess.Dispose();
                        onError.Dispose();
                        throw;
                    }
                }
                else
                {
                    Interop.ExecuteJavaScriptVoid(
                        """
                        try {
                            let uint8Array = new Uint8Array($0.length);
                            for (let i = 0; i < $0.length; i++) uint8Array[i] = $0[i].charCodeAt(0);
                            // Must pass TypedArray as regular Array to Blob constructor, that's why it's wrapped in []
                            saveAs(new Blob([uint8Array]), $1);
                        } catch (error) {
                            console.error(error);
                            throw error;
                        }
                        """, _writableStream, _filename);
                }

                _writableStream.Dispose();
                _writableStream = null;
                _isClosed = true;
            }
        }

        private sealed class WritableStream : Stream
        {
            private readonly FileSaver _fileSaver;

            public WritableStream(FileSaver fileSaver)
            {
                Debug.Assert(fileSaver is not null);
                _fileSaver = fileSaver;
            }

            public override bool CanRead => false;

            public override bool CanSeek => false;

            public override bool CanWrite => true;

            public override long Length => throw new NotSupportedException();

            public override long Position
            {
                get => throw new NotSupportedException();
                set => throw new NotSupportedException();
            }

            public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
                => _fileSaver.Write(buffer, offset, count);

            public override Task FlushAsync(CancellationToken cancellationToken) => Task.CompletedTask;

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                _fileSaver.Close();
            }

            public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

            public override void Flush() => throw new NotSupportedException();

            public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();

            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

            public override void SetLength(long value) => throw new NotSupportedException();
        }
    }
}
