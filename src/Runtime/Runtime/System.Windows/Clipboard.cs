
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
using System.Threading.Tasks;
using DotNetForHtml5.Core;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Provides static methods that facilitate transferring data to and from the system
    /// clipboard. In Silverlight 5, this access is limited to Unicode text strings.
    /// </summary>
    public static class Clipboard
    {
        /// <summary>
        /// Sets Unicode text data to store on the clipboard, for later access with <see cref="GetText"/>.
        /// </summary>
        /// <param name="text">
        /// A string that contains the Unicode text data to store on the clipboard.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// text is null.
        /// </exception>
        public static void SetText(string text)
        {
            _ = SetTextAsync(text);
        }

        /// <summary>
        /// Sets Unicode text data to store on the clipboard, for later access with <see cref="GetTextAsync"/>.
        /// </summary>
        /// <param name="text">
        /// A string that contains the Unicode text data to store on the clipboard.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// text is null.
        /// </exception>
        public static Task SetTextAsync(string text)
        {
            if (text is null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            if (OpenSilver.Interop.IsRunningInTheSimulator)
            {
                INTERNAL_Simulator.ClipboardHandler.SetText(text);
                return Task.CompletedTask;
            }
            else
            {
                var tcs = new TaskCompletionSource<object>();

                OpenSilver.Interop.ExecuteJavaScript("navigator.clipboard.writeText($0).then($1());",
                    text,
                    new Action(() =>
                    {
                        try
                        {
                            tcs.SetResult(null);
                        }
                        catch (Exception ex)
                        {
                            tcs.SetException(ex);
                        }
                    }));

                return tcs.Task;
            }
        }

        /// <summary>
        /// Queries the clipboard for the presence of data in the UnicodeText format.
        /// Not implemented in the browser. Use <see cref="GetTextAsync"/> instead.
        /// </summary>
        /// <returns>
        /// Always returns <see cref="string.Empty"/> in the browser.
        /// </returns>
        [Obsolete("Use GetTextAsync() instead.")]
        [OpenSilver.NotImplemented]
        public static string GetText()
        {
            if (OpenSilver.Interop.IsRunningInTheSimulator)
            {
                return INTERNAL_Simulator.ClipboardHandler.GetText();
            }

            return string.Empty;
        }

        /// <summary>
        /// Retrieves Unicode text data from the system clipboard, if Unicode text data exists.
        /// </summary>
        /// <returns>
        /// If Unicode text data is present on the system clipboard, returns a string that
        /// contains the Unicode text data. Otherwise, returns an empty string.
        /// </returns>
        public static Task<string> GetTextAsync()
        {
            if (OpenSilver.Interop.IsRunningInTheSimulator)
            {
                return Task.FromResult<string>(INTERNAL_Simulator.ClipboardHandler.GetText());
            }
            else
            {
                var tcs = new TaskCompletionSource<string>();

                OpenSilver.Interop.ExecuteJavaScript("navigator.clipboard.readText().then(text => $0(text));",
                    new Action<string>(content =>
                    {
                        try
                        {
                            tcs.SetResult(content);
                        }
                        catch (Exception ex)
                        {
                            tcs.SetException(ex);
                        }
                    }));

                return tcs.Task;
            }
        }

        /// <summary>
        /// Queries the clipboard for the presence of data in the UnicodeText format.
        /// Not implemented in the browser. Use <see cref="ContainsTextAsync"/> instead.
        /// </summary>
        /// <returns>
        /// Always return false in the browser.
        /// </returns>
        [Obsolete("Use ContainsTextAsync() instead.")]
        [OpenSilver.NotImplemented]
        public static bool ContainsText()
        {
            if (OpenSilver.Interop.IsRunningInTheSimulator)
            {
                return INTERNAL_Simulator.ClipboardHandler.ContainsText();
            }

            return false;
        }

        /// <summary>
        /// Queries the clipboard for the presence of data in the UnicodeText format.
        /// </summary>
        /// <returns>
        /// true if the system clipboard contains Unicode text data; otherwise, false.
        /// </returns>
        public static Task<bool> ContainsTextAsync()
        {
            if (OpenSilver.Interop.IsRunningInTheSimulator)
            {
                return Task.FromResult<bool>(INTERNAL_Simulator.ClipboardHandler.ContainsText());
            }
            else
            {
                var tcs = new TaskCompletionSource<bool>();

                OpenSilver.Interop.ExecuteJavaScript("navigator.clipboard.readText().then(text => $0(!!text))",
                    new Action<bool>(b =>
                    {
                        try
                        {
                            tcs.SetResult(b);
                        }
                        catch (Exception ex)
                        {
                            tcs.SetException(ex);
                        }
                    }));

                return tcs.Task;
            }
        }
    }
}
