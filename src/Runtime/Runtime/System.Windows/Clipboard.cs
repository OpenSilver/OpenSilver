
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
using OpenSilver.Internal;

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
        private static readonly IAsyncClipboard _impl;

        static Clipboard()
        {
            _impl = ClipboardProvider.GetClipboard();
        }

        /// <summary>
        /// Sets Unicode text data to store on the clipboard, for later access with <see cref="GetText"/>.
        /// </summary>
        /// <param name="text">
        /// A string that contains the Unicode text data to store on the clipboard.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// text is null.
        /// </exception>
        [Obsolete("Use SetTextAsync(string) instead.")]
        public static void SetText(string text)
        {
            if (text is null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            _impl.SetText(text);
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

            return _impl.SetTextAsync(text);
        }

        /// <summary>
        /// Queries the clipboard for the presence of data in the UnicodeText format.
        /// Not implemented in the browser. Use <see cref="GetTextAsync"/> instead.
        /// </summary>
        /// <returns>
        /// Always returns <see cref="string.Empty"/> in the browser.
        /// </returns>
        [Obsolete("Use GetTextAsync() instead.")]
        public static string GetText() => _impl.GetText();

        /// <summary>
        /// Retrieves Unicode text data from the system clipboard, if Unicode text data exists.
        /// </summary>
        /// <returns>
        /// If Unicode text data is present on the system clipboard, returns a string that
        /// contains the Unicode text data. Otherwise, returns an empty string.
        /// </returns>
        public static Task<string> GetTextAsync() => _impl.GetTextAsync();

        /// <summary>
        /// Queries the clipboard for the presence of data in the UnicodeText format.
        /// Not implemented in the browser. Use <see cref="ContainsTextAsync"/> instead.
        /// </summary>
        /// <returns>
        /// Always return false in the browser.
        /// </returns>
        [Obsolete("Use ContainsTextAsync() instead.")]
        public static bool ContainsText() => _impl.ContainsText();

        /// <summary>
        /// Queries the clipboard for the presence of data in the UnicodeText format.
        /// </summary>
        /// <returns>
        /// true if the system clipboard contains Unicode text data; otherwise, false.
        /// </returns>
        public static Task<bool> ContainsTextAsync() => _impl.ContainsTextAsync();
    }
}
