using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OpenSilver.IO;
using System.Web;
using System.Windows;

namespace OpenSilver.Controls
{
    // If possible, use <a> with 'Content-Disposition: attachment' server response header instead.
    // This will make downloads from the server be entirely managed by the browser (progress, folder, etc). Example:
    // OpenSilver.Interop.ExecuteJavaScript($@"const downloadElement = document.createElement('a');
    //    downloadElement.download = '{filename}';
    //    downloadElement.href = '{filepath}';
    //    downloadElement.target = '_blank';
    //    downloadElement.click();");
    public sealed class SaveFileDialog
    {
        private object _handle;

        private static bool? _isFileSystemApiAvailable;
        internal static bool IsFileSystemApiAvailable
        {
            get
            {
                if (_isFileSystemApiAvailable == null)
                {
                    _isFileSystemApiAvailable = !string.IsNullOrEmpty(
                        Interop.ExecuteJavaScript("window.showSaveFilePicker").ToString());
                }
                return _isFileSystemApiAvailable.GetValueOrDefault();
            }
            set => _isFileSystemApiAvailable = value;
        }

        private string _filter;
        public string Filter
        {
            get => _filter;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    FilterEntries = new List<Tuple<string, string[]>>();

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
                            FilterEntries.Add(new Tuple<string, string[]>(description, filterPatterns));
                            description = string.Empty;
                        }
                    }
                }
                else
                {
                    FilterEntries = null;
                }
                _filter = value;
            }
        }

        private IList<Tuple<string, string[]>> FilterEntries { get; set; }

        public int FilterIndex
        {
            get;
            set;
        } = 1;

        /// <summary>
        /// Gets the file name for the selected file associated with the SaveFileDialog.
        /// </summary>
        public string SafeFileName => Path.GetFileName(GetFilename());

        // Summary:
        //     Gets or sets the default file name extension applied to files that are saved
        //     with the System.Windows.Controls.SaveFileDialog.
        //
        // Returns:
        //     The default file name extension applied to files that are saved with the System.Windows.Controls.SaveFileDialog,
        //     which can optionally include the dot character (.).
        public string DefaultExt { get; set; }

        //
        // Summary:
        //     Gets or sets the file name used if a file name is not specified by the user.
        //
        // Returns:
        //     The file name used if a file name is not specified by the user.System.String.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     Occurs if the specified file name is null or contains invalid characters such
        //     as quotes ("), less than (<), greater than (>), pipe (|), backspace (\b), null
        //     (\0), tab (\t), colon (:), asterisk(*), question mark (?), and slashes (\\, /).
        public string DefaultFileName { get; set; }

        //
        // Summary:
        //     Opens the file specified by the System.Windows.Controls.SaveFileDialog.SafeFileName
        //     property.
        //
        // Returns:
        //     A read-write stream for the file specified by the System.Windows.Controls.SaveFileDialog.SafeFileName
        //     property.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     No file was selected in the dialog box.
        public async Task<Stream> OpenFile(long? size = null)
        {
            var fileSaver = new FileSaver();
            await fileSaver.OpenFile(_handle, SafeFileName, size);

            MemoryFileStream memoryFileStream = new MemoryFileStream(bytes => fileSaver.Write(bytes),
                () => fileSaver.Close());
            return memoryFileStream;
        }

        private string GetFilename()
        {
            if (FilterEntries.Count <= FilterIndex - 1)
            {
                throw new InvalidOperationException("FilterIndex is out of bounds of Filter");
            }

            // https://learn.microsoft.com/en-us/previous-versions/windows/silverlight/dotnet-windows-silverlight/dd459587(v=vs.95)
            // The default file name extension is applied when the selected filter does not specify an extension
            // (or the Filter property is not set) and the user does not specify an extension.
            if ((string.IsNullOrEmpty(Filter) || FilterEntries[FilterIndex - 1].Item2.Contains("*.*")) &&
                    !string.IsNullOrEmpty(DefaultFileName) && !Path.HasExtension(DefaultFileName))
            {
                return Path.ChangeExtension(DefaultFileName, DefaultExt);
            }

            // In Firefox, if there is no default filename but there is a default extension, it is used.
            if (!IsFileSystemApiAvailable && (string.IsNullOrEmpty(DefaultFileName) || !Path.HasExtension(DefaultFileName)))
            {
                string filename = !string.IsNullOrEmpty(DefaultFileName) ? DefaultFileName : "download";

                if (!string.IsNullOrEmpty(Filter))
                {
                    string firstFilterPattern = FilterEntries[FilterIndex - 1].Item2.FirstOrDefault();
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

        //
        // Summary:
        //     Displays a System.Windows.Controls.SaveFileDialog that is modal to the Web browser
        //     or main window.
        //
        // Returns:
        //     true if the user clicked Save; false if the user clicked Cancel or closed the
        //     dialog box.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     Silverlight was unable to display the dialog box due to an improperly formatted
        //     filter, an invalid filter index or other reasons.
        //
        //   T:System.Security.SecurityException:
        //     Active Scripting in Internet Explorer is disabled.-or-The call to the System.Windows.Controls.OpenFileDialog.ShowDialog
        //     method was not made from user-initiated code or too much time passed between
        //     user-initiation and the display of the dialog.
        public async Task<bool?> ShowDialog()
        {
            // In Silverlight, when DefaultFileName is set, a security warning MessageBox is shown before the dialog.
            if (!string.IsNullOrEmpty(DefaultFileName))
            {
                var messageBoxResult = MessageBox.Show($"Do you want to save {DefaultFileName}?", "File Download - Security Warning",
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

            TaskCompletionSource<IDisposable> taskCompletionSource = new TaskCompletionSource<IDisposable>();

            _handle = Interop.ExecuteJavaScript($@"
                try {{
                    (async () => {{
                        const opts = {{
                            startIn: 'downloads',
                            suggestedName: $2,
                            types: {TranslateFilterToTypes()}
                        }};
                        return await window.showSaveFilePicker(opts);
                    }})().then(handle => {{ $0(); return handle; }}, error => {{ $1(error.toString()) }});
                }} catch (error) {{
                    console.error(error);
                    throw error;
                }}",
                () => taskCompletionSource.SetResult(null),
                (Action<string>)(error =>
                {
                    // Errors thrown when dialogs are canceled are suppressed
                    if (!error.StartsWith("AbortError"))
                    {
                        taskCompletionSource.SetException(new Exception(error));
                    }
                }),
                SafeFileName);

            try
            {
                await taskCompletionSource.Task;

                return true;
            }
            catch (OperationCanceledException)
            {
                // Cancel button was hit on file dialog
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                // Falling back to not using File System API as it might not be supported on this browser
                IsFileSystemApiAvailable = false;
                return true;
            }
        }

        /// <summary>
        /// Translates the legacy Filter member of Silverlight SaveFileDialog into the types option member of showSaveFilePicker().
        /// </summary>
        /// <returns>A string that defines the types option of showSaveFilePicker() to apply filters in the dialog.</returns>
        private string TranslateFilterToTypes()
        {
            // Example of a types option for showSaveFilePicker:
            // const opts = {
            //      types: [
            //          {
            //              description: "Text file",
            //              accept: { "text/plain": [".txt"] },
            //          },
            //      ],
            //  };

            IList<string> types = new List<string>();

            if (FilterEntries != null)
            {
                foreach (var entry in FilterEntries)
                {
                    var acceptTypes = new Dictionary<string, IList<string>>();
                    foreach (string filterPattern in entry.Item2)
                    {
                        // Using .* on "accept" throws ("Extension '.*' contains invalid characters").
                        if (!string.IsNullOrEmpty(filterPattern) && !Path.GetExtension(filterPattern.Trim()).Contains("*"))
                        {
                            string mimeType = MimeMapping.GetMimeMapping(filterPattern.Trim());
                            if (!acceptTypes.ContainsKey(mimeType))
                            {
                                acceptTypes[mimeType] = new List<string>();
                            }

                            acceptTypes[mimeType].Add($"'{Path.GetExtension(filterPattern.Trim())}'");
                        }
                    }

                    types.Add($"{{ description: '{entry.Item1}', accept: {{ {string.Join(",", acceptTypes.Select(kv => $"'{kv.Key}' : [{string.Join(",", kv.Value)}]"))} }} }}");
                }

                if (FilterIndex > 1)
                {
                    if (FilterEntries.Count <= FilterIndex - 1)
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
    }

    public class FileSaver
    {
        // This is the size in which writes don't seem to block for too long
        private const int BufferSize = 131072;

        private IDisposable _writableStream;
        private bool _isClosed = false;
        private string _filename;

        ~FileSaver()
        {
            Close();
        }

        /// <summary>
        /// </summary>
        /// <param name="handle">JS Promise object that fulfills to a FileSystemWritableFileStream if File System API is available.</param>
        /// <param name="fallbackFilename">Filename to save in case downloads are managed by the browser.</param>
        /// <param name="fallbackSize">File size to show progress in case downloads are managed by the browser.</param>
        /// <returns></returns>
        internal async Task OpenFile(object handle, string fallbackFilename, long? fallbackSize)
        {
            if (SaveFileDialog.IsFileSystemApiAvailable)
            {
                TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();

                _writableStream = Interop.ExecuteJavaScript(@"
                    try {
                        $0.then(async handle => await handle.createWritable())
                        .then(stream => { $1(); return stream; }, error => { console.error(error); $2(error.toString()) });
                    } catch (error) {
                        console.error(error);
                        throw error;
                    }",
                    handle,
                    () => taskCompletionSource.SetResult(null),
                    (Action<string>)(error => taskCompletionSource.SetException(new Exception(error))));

                await taskCompletionSource.Task;
            }
            else
            {
                _writableStream = Interop.ExecuteJavaScript(@"[];");
                _filename = fallbackFilename;
            }
        }

        internal async Task Write(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            if (_writableStream == null)
            {
                throw new InvalidOperationException("Unable to write to an unopened file.");
            }

            int bytesToWrite = BufferSize;
            int i = 0;
            do
            {
                if (_isClosed)
                {
                    break;
                }

                if (i + bytesToWrite > bytes.Length)
                {
                    bytesToWrite = bytes.Length - i;
                }

                TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();

                string base64 = Convert.ToBase64String(bytes.Skip(i).Take(bytesToWrite).ToArray());

                if (SaveFileDialog.IsFileSystemApiAvailable)
                {
                    Interop.ExecuteJavaScriptVoid(@"
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
                        }",
                        base64,
                        _writableStream,
                        () => taskCompletionSource.SetResult(null),
                        (Action<string>)(error => taskCompletionSource.SetException(new Exception(error))));

                    await taskCompletionSource.Task;
                }
                else
                {
                    Interop.ExecuteJavaScriptVoid(@"
                        try {
                            const binaryString = atob($0);
                            for (let i in binaryString) {
                                $1.push(binaryString[i]);
                            }
                        } catch (error) {
                            console.error(error);
                            throw error;
                        }", base64, _writableStream);
                }

                // This loop could be long running for large files, so the file is written in
                // chunks and Delay is called to give control back to UI so it does not freeze.
                await Task.Delay(1);

                i += bytesToWrite;
            } while (i < bytes.Length - 1);
        }

        internal async void Close()
        {
            if (_writableStream != null && !_isClosed)
            {
                if (SaveFileDialog.IsFileSystemApiAvailable)
                {
                    TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();

                    Interop.ExecuteJavaScriptVoid(@"
                        try {
                            $0.then(async writableStream => await writableStream.close())
                            .then(() => $1(), error => { $2(error.toString()) });
                        } catch (error) {
                            console.error(error);
                            throw error;
                        }",
                        _writableStream,
                        () => taskCompletionSource.SetResult(null),
                        (Action<string>)(error =>
                        {
                            /* Errors are suppressed because it could just be that downloads were canceled */
                        }));

                    await taskCompletionSource.Task;
                }
                else
                {
                    Interop.ExecuteJavaScriptVoid(@"
                        try {
                            let uint8Array = new Uint8Array($0.length);
                            for (let i = 0; i < $0.length; i++) uint8Array[i] = $0[i].charCodeAt(0);

                            // Must pass TypedArray as regular Array to Blob constructor, that's why it's wrapped in []
                            saveAs(new Blob([uint8Array]), $1);
                        } catch (error) {
                            console.error(error);
                            throw error;
                        }", _writableStream, _filename);
                }

                _writableStream.Dispose();
                _writableStream = null;
                _isClosed = true;
            }
        }
    }
}
