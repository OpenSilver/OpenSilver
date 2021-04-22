

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
#if !CSHTML5NETSTANDARD
            if (CSHTML5.Interop.IsRunningInTheSimulator)
            {
                INTERNAL_Simulator.ClipboardHandler.SetText(text);
            }
            else
            {
#endif
            CSHTML5.Interop.ExecuteJavaScript(@"
                  // IE specific
                  if (window.clipboardData && window.clipboardData.setData) {
                    return clipboardData.setData(""Text"", $0);
                  }

                  // all other modern
                  var target = document.createElement(""textarea"");
                  target.style.position = ""absolute"";
                  target.style.left = ""-9999px"";
                  target.style.top = ""0"";
                  target.textContent = $0;
                  document.body.appendChild(target);
                  target.focus();
                  target.setSelectionRange(0, target.value.length);

                  // copy the selection of fall back to prompt
                  try {
                    document.execCommand(""copy"");
                    target.remove();
                  } catch(e) {
                    window.prompt(""Please confirm that you would like to copy to the clipboard by pressing Ctrl+C now. The data below will be copied to the clipboard."", $0);
                  }",
                        text
                    );
#if !CSHTML5NETSTANDARD
            }
#endif
        }

#if WORKINPROGRESS
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
#endif
    }
}
