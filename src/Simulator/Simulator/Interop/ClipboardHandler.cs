

/*===================================================================================
* 
*   Copyright (c) Userware (OpenSilver.net, CSHTML5.com)
*      
*   This file is part of both the OpenSilver Simulator (https://opensilver.net), which
*   is licensed under the MIT license (https://opensource.org/licenses/MIT), and the
*   CSHTML5 Simulator (http://cshtml5.com), which is dual-licensed (MIT + commercial).
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

extern alias opensilver;

using System.Windows;

namespace DotNetForHtml5.EmulatorWithoutJavascript
{
    public class ClipboardHandler : opensilver::OpenSilver.Internal.IAsyncClipboard
    {
        public void SetText(string text) => Clipboard.SetText(text);

        public Task SetTextAsync(string text)
        {
            SetText(text);
            return Task.CompletedTask;
        }

        public string GetText() => Clipboard.GetText();

        public Task<string> GetTextAsync() => Task.FromResult(GetText());
        
        public bool ContainsText() => Clipboard.ContainsText();

        public Task<bool> ContainsTextAsync() => Task.FromResult(ContainsText());
    }
}
