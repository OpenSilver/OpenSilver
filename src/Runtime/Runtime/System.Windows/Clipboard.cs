

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
using System.Threading.Tasks;

namespace System.Windows
{
    /// <summary>
    /// Provides static methods that facilitate transferring data to and from the
    /// system clipboard. In Silverlight 5, this access is limited to Unicode text
    /// strings.
    /// </summary>
    public static class Clipboard
    {
        private static void InitClipboard()
        {
            Text.StringBuilder function = new Text.StringBuilder();
            function.Append("if(!navigator.clipboard)");
            function.Append("{");
            function.Append("	navigator.clipboard = {};");
            function.Append("	navigator.clipboard.writeText = function(data)");
            function.Append("	{");
            function.Append("		var $tempElement = document.createElement('input');");
            function.Append("		document.body.append($tempElement);");
            function.Append("		$tempElement.value=data;$tempElement.select();");
            function.Append("		document.execCommand('Copy');");
            function.Append("		$tempElement.remove();");
            function.Append("	};");
            function.Append("	navigator.clipboard.readText = function()");
            function.Append("	{");
            function.Append("	    return new Promise((resolve,reject)=>{");
            function.Append("		    var $tempElement = document.createElement('input');");
            function.Append("		    document.body.append($tempElement);");
            function.Append("		    $tempElement.focus();");
            function.Append("		    document.execCommand('paste');");
            function.Append("		    var returnValue = $tempElement.value;");
            function.Append("		    $tempElement.remove();");
            function.Append("		    resolve(returnValue);");
            function.Append("	    });");
            function.Append("   };");
            function.Append("}");
            OpenSilver.Interop.ExecuteJavaScript(function.ToString());
        }
        /// <summary>
        /// Sets text data to store on the clipboard.
        /// </summary>
        /// <param name="text">A string that contains the Unicode text data to store on the clipboard.</param>
        public static void SetText(string text)
        {

            // Credits: https://stackoverflow.com/questions/400212/how-do-i-copy-to-the-clipboard-in-javascript
#if OPENSILVER
            if (OpenSilver.Interop.IsRunningInTheSimulator_WorkAround)
#else
            if (CSHTML5.Interop.IsRunningInTheSimulator)
#endif
            {
                INTERNAL_Simulator.ClipboardHandler.SetText(text);
            }
            else
            {
                InitClipboard();
                OpenSilver.Interop.ExecuteJavaScript(@"navigator.clipboard.writeText($0);", text);
            }
        }

        public static Task<string> GetText()
        {
            TaskCompletionSource<String> readBlockTaskCompletionSource = new TaskCompletionSource<String>();
            Action<string> ReadBlockCallback = (content) =>
            {
                try
                {
                    readBlockTaskCompletionSource.SetResult(content);
                }
                catch (Exception ex)
                {
                    readBlockTaskCompletionSource.SetException(ex);
                }
            };
#if OPENSILVER
            if (OpenSilver.Interop.IsRunningInTheSimulator_WorkAround)
#else
            if (CSHTML5.Interop.IsRunningInTheSimulator)
#endif
            {
                readBlockTaskCompletionSource.SetResult(INTERNAL_Simulator.ClipboardHandler.GetText());
                return readBlockTaskCompletionSource.Task;
            }
            else
            {
                InitClipboard();
                OpenSilver.Interop.ExecuteJavaScript("navigator.clipboard.readText().then(clipText=> $0(clipText));", ReadBlockCallback);
                return readBlockTaskCompletionSource.Task;
            }
        }

        public static Task<bool> ContainsText()
        {
            TaskCompletionSource<bool> readBlockTaskCompletionSource = new TaskCompletionSource<bool>();
            Action<string> ReadBlockCallback = (content) =>
            {
                try
                {
                    readBlockTaskCompletionSource.SetResult(!String.IsNullOrEmpty(content));
                }
                catch (Exception ex)
                {
                    readBlockTaskCompletionSource.SetException(ex);
                }
            };
#if OPENSILVER
            if (OpenSilver.Interop.IsRunningInTheSimulator_WorkAround)
#else
            if (CSHTML5.Interop.IsRunningInTheSimulator)
#endif
            {
                readBlockTaskCompletionSource.SetResult(INTERNAL_Simulator.ClipboardHandler.GetText());
                return readBlockTaskCompletionSource.Task;
            }
            else
            {                
                InitClipboard();
                OpenSilver.Interop.ExecuteJavaScript("navigator.clipboard.readText().then(clipText=> $0(clipText));", ReadBlockCallback);
                return readBlockTaskCompletionSource.Task;
            }
        }
    }
}
