

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



using DotNetBrowser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace DotNetForHtml5.EmulatorWithoutJavascript
{
    public class ResourceInterceptor : DefaultNetworkDelegate
    {
        private readonly string _baseURL;
        private readonly string _rootPath;

        public ResourceInterceptor(string baseURL)
        {
            _baseURL = baseURL.ToLower();
            _rootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Replace(@"\", "/");
        }

        public override void OnBeforeURLRequest(BeforeURLRequestParams parameters)
        {
            string url = parameters.Url.Replace(_baseURL, $"file:///{_rootPath}/").Replace("[PARENT]", "..");
            parameters.SetUrl(url);
        }
    }
}