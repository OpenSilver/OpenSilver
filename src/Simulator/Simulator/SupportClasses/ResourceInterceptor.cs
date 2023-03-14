

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



using DotNetBrowser.Net.Handlers;
using System.Reflection;
using System.Text.RegularExpressions;
using Path = System.IO.Path;

namespace DotNetForHtml5.EmulatorWithoutJavascript
{
    public class ResourceInterceptor
    {
        private readonly string _baseURL;
        private readonly string _rootPath;

        public ResourceInterceptor(string baseURL)
        {
            _baseURL = baseURL.ToLower();
            _rootPath = Uri.EscapeDataString(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Replace(@"\", "/"));
            _rootPath = Regex.Replace(_rootPath, "%2F", "/", RegexOptions.IgnoreCase);
        }

        internal SendUrlRequestResponse ProcessUrlRequest(SendUrlRequestParameters e)
        {
            string url = e.UrlRequest.Url.Replace(_baseURL, $"file:///{_rootPath}/").Replace("[PARENT]", "..");
            return SendUrlRequestResponse.Override(url);
        }
    }
}