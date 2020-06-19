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