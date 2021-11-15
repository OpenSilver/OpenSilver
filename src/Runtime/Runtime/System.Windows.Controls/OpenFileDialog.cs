

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


using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

#if MIGRATION
using System.Windows;
using System.Windows.Controls;
#else
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    public sealed partial class OpenFileDialog
    {
        private object _windowFocusCallback;
        private readonly object _inputElement;

        public OpenFileDialog()
        {
            // Creates <input> element but does not add to DOM
            _inputElement = OpenSilver.Interop.ExecuteJavaScript(@"
                (function() {
                    var inputElement = document.createElement(""input"");
                    inputElement.type = ""file"";
                    return inputElement;
                })()");
        }

        private void AddFileChangeCallback(TaskCompletionSource<bool?> showDialogTaskCompletionSource)
        {
            // For each file selected in the dialog, this will be called back
            Action<string, object> onFileChanged = (filename, base64Content) =>
            {
                byte[] bytes = null;
                if (base64Content != null)
                {
                    // Removing base64 data type prefix e.g. 'data:image/png;base64,<data>'
                    string data = base64Content.ToString().Split(',')[1];
                    bytes = Convert.FromBase64String(data);
                }
                Files.Add(new MemoryFileInfo(filename, bytes));
            };

            Action onFinishedReading = () =>
            {
                try
                {
                    // Setting result of Task returned by ShowDialog()
                    showDialogTaskCompletionSource.SetResult(true);
                }
                catch (Exception ex)
                {
                    showDialogTaskCompletionSource.SetException(ex);
                }
            };

            // Listen to the "change" property of the "input" element, and call the callback:
            OpenSilver.Interop.ExecuteJavaScript(@"
                $0.addEventListener(""change"", function(e) {
                    var isRunningInTheSimulator = $2;
                    if (isRunningInTheSimulator) {
                        alert(""The file open dialog is not supported in the Simulator. Please test in the browser instead."");
                    }

                    window.isOpenFileDialogOpen = false;

                    // Removing window focus handler to detect cancels, otherwise we could have multiple handlers
                    // after calling multiple ShowDialog()
                    window.removeEventListener('focus', $3);

                    if(!e) {
                      e = window.event;
                    }
                    var input = e.target;
                    var callback = $1;
                    var reader = new FileReader();

                    // Reading each file sequentially, some results were null when running concurrently
                    function readNext(i) {
                        var file = input.files[i];
                        reader.onload = function() {
                            callback(file.name, reader.result);

                            if (input.files.length > i + 1) {
                                readNext(i + 1);
                            } else {
                                // Triggers finished callback
                                $4();
                            }
                        };

                        // For performance improvements, readAsArrayBuffer could be used and Uint8Array sent to C#,
                        // this has been optimized in .NET 6. However, this would require changes to the C# callback method,
                        // the array cannot be received as object (must be byte[]).
                        reader.readAsDataURL(file);
                    }
                    readNext(0);
                });", _inputElement, onFileChanged, OpenSilver.Interop.IsRunningInTheSimulator,
                    _windowFocusCallback, onFinishedReading);
        }

        private void SetFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return;
            }

            // Process the filter list to convert the syntax from XAML to HTML5:
            // Example of syntax in Silverlight: Image Files (*.bmp, *.jpg)|*.bmp;*.jpg|All Files (*.*)|*.*
            // Example of syntax in HTML5: .gif, .jpg, .png, .doc
            string[] splitted = filter.Split('|');
            List<string> itemsKept = new List<string>();
            if (splitted.Length == 1)
            {
                itemsKept.Add(splitted[0]);
            }
            else
            {
                for (int i = 1; i < splitted.Length; i += 2)
                {
                    itemsKept.Add(splitted[i]);
                }
            }
            string filtersInHtml5 = string.Join(",", itemsKept).Replace("*", "").Replace(";", ",");

            // Apply the filter:
            if (!string.IsNullOrWhiteSpace(filtersInHtml5))
            {
                OpenSilver.Interop.ExecuteJavaScript(@"$0.accept = $1", _inputElement, filtersInHtml5);
            }
            else
            {
                OpenSilver.Interop.ExecuteJavaScript(@"$0.accept = """"", _inputElement);
            }
        }

        private bool _multiselect = false;
        public bool Multiselect
        {
            get { return _multiselect; }
            set
            {
                _multiselect = value;

                if (_multiselect)
                {
                    OpenSilver.Interop.ExecuteJavaScript(@"$0.multiple = 'multiple';", _inputElement);
                }
            }
        }

        private string _filter;
        public string Filter
        {
            get
            {
                return _filter;
            }
            set
            {
                _filter = value;
                SetFilter(_filter);
            }
        }

        [OpenSilver.NotImplemented]
        // No option to separate html5 'accept' file types into groups, they all appear together as 'Custom',
        // with an additional option for 'All Files' (Chrome).
        // In Firefox they appear all together, and then one option for each file type.
        public int FilterIndex
        {
            get;
            set;
        }

        public MemoryFileInfo File
        {
            get
            {
                return Files.FirstOrDefault();
            }
        }

        public IEnumerable<MemoryFileInfo> Files
        {
            get;
            private set;
        } = new List<MemoryFileInfo>();

        /// <summary>
        /// Opens the default browser file dialog. This returns a Task, differently from Silverlight.
        /// This is because it is not possible to wait for the dialog to conclude, since the process is single-threaded.
        /// </summary>
        /// <returns>A Task that will have the result of the file dialog. True for files selected, false for cancel/exit.</returns>
        public Task<bool?> ShowDialog()
        {
            return ShowDialog(null);
        }

        /// <summary>
        /// Opens the default browser file dialog. This returns a Task, differently from Silverlight.
        /// This is because it is not possible to wait for the dialog to conclude, since the process is single-threaded.
        /// </summary>
        /// <param name="owner"></param>
        /// <returns>A Task that will have the result of the file dialog. True for files selected, false for cancel/exit.</returns>
        public Task<bool?> ShowDialog(Window owner)
        {
            ClearFiles();

            // This wraps a task to return to the caller and have a result asynchronously
            TaskCompletionSource<bool?> showDialogTaskCompletionSource = new TaskCompletionSource<bool?>();

            AddCancelCallback(showDialogTaskCompletionSource);

            AddFileChangeCallback(showDialogTaskCompletionSource);

            // Triggers 'click' on <input>, even though it is not on the DOM
            OpenSilver.Interop.ExecuteJavaScript(@"
                window.isOpenFileDialogOpen = true;
                $0.dispatchEvent(new MouseEvent(""click""));", _inputElement);

            return showDialogTaskCompletionSource.Task;
        }

        private void ClearFiles()
        {
            ((IList<MemoryFileInfo>)Files).Clear();
        }

        private void AddCancelCallback(TaskCompletionSource<bool?> showDialogTaskCompletionSource)
        {
            Action<object> onDialogCancel = (result) =>
            {
                try
                {
                    // Setting result of Task returned by ShowDialog()
                    showDialogTaskCompletionSource.SetResult(false);
                }
                catch (Exception ex)
                {
                    showDialogTaskCompletionSource.SetException(ex);
                }
            };

            _windowFocusCallback = OpenSilver.Interop.ExecuteJavaScript(@"
                (function() {
                    var isChrome = !!window.chrome;

                    var windowFocusCallbackForFileDialogCancel = function(e) {
                        if (isChrome) {
                            // If on Chrome, verifies flag after timeout because the window 'focus' is called before
                            // the 'change' event, timeout should be enough to make sure 'change' hasn't been triggered
                            setTimeout(function() {
                                if (window.isOpenFileDialogOpen) {
                                    window.isOpenFileDialogOpen = false;

                                    // Removing window focus handler to detect cancels, otherwise we could have multiple handlers
                                    // after calling multiple ShowDialog()
                                    window.removeEventListener('focus', windowFocusCallbackForFileDialogCancel);

                                    // Calls cancel callback
                                    $0();
                                }
                            }, 1000);
                        } else {
                            if (window.isOpenFileDialogOpen) {
                                window.isOpenFileDialogOpen = false;

                                // Removing window focus handler to detect cancels, otherwise we could have multiple handlers
                                // after calling multiple ShowDialog()
                                window.removeEventListener('focus', windowFocusCallbackForFileDialogCancel);

                                // Calls cancel callback
                                $0();
                            }
                        }
                    }

                    window.addEventListener('focus', windowFocusCallbackForFileDialogCancel);

                    return windowFocusCallbackForFileDialogCancel;
                })()
            ", onDialogCancel);
        }
    }
}
