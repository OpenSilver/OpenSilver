

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


using DotNetForHtml5.Core;

namespace System.Windows
{
    /// <summary>
    /// Provides static methods that facilitate transferring data to and from the
    /// system clipboard. In Silverlight 5, this access is limited to Unicode text
    /// strings.
    /// </summary>
    public static class Clipboard
    {
        /// <summary>
        /// Sets text data to store on the clipboard.
        /// </summary>
        /// <param name="text">A string that contains the Unicode text data to store on the clipboard.</param>
        public static void SetText(string text)
        {
            // Credits: https://stackoverflow.com/questions/400212/how-do-i-copy-to-the-clipboard-in-javascript
#if OPENSILVER
            if (CSHTML5.Interop.IsRunningInTheSimulator_WorkAround)
#else
            if (CSHTML5.Interop.IsRunningInTheSimulator)
#endif
            {
                INTERNAL_Simulator.ClipboardHandler.SetText(text);
            }
            else
            {
                CSHTML5.Interop.ExecuteJavaScript(@"
if (window.clipboardData && window.clipboardData.setData) // IE specific
{
    clipboardData.setData(""Text"", $0);
}
else if (navigator && navigator.clipboard && navigator.clipboard.writeText) // all other modern browsers
{
    navigator.clipboard.writeText($0);
}
", 
                    text);
            }
        }

		[OpenSilver.NotImplemented]
        public static string GetText()
        {
            return string.Empty;
        }

		[OpenSilver.NotImplemented]
        public static bool ContainsText()
        {
            return false;
        }
    }
}
