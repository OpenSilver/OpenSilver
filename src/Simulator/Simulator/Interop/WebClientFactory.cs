

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



using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DotNetForHtml5.EmulatorWithoutJavascript
{
    public class WebClientFactory
    {
        public object Create()
        {
            return new WebClient();
        }

        public string DownloadString(Dictionary<string, string> headersAsDictionary, Uri address)
        {
            var webClient = new WebClient();
            FillHeaders(webClient, headersAsDictionary);
            return webClient.DownloadString(address);
        }
        public void DownloadStringAsync(Dictionary<string, string> headersAsDictionary, Action<object, dynamic> del, Uri address, object userToken)
        {
            var webClient = new WebClient();
            FillHeaders(webClient, headersAsDictionary);
            webClient.DownloadStringCompleted += (s,e) => del(s,e);
            webClient.DownloadStringAsync(address, userToken);
        }
        public Task<string> DownloadStringTaskAsync(Dictionary<string, string> headersAsDictionary, Uri address)
        {
            var webClient = new WebClient();
            FillHeaders(webClient, headersAsDictionary);
            return webClient.DownloadStringTaskAsync(address);
        }

        public Task<string> UploadStringTaskAsync(Dictionary<string, string> headersAsDictionary, Uri address, string method, string data)
        {
            var webClient = new WebClient();
            //webClient.Encoding = encoding;
            FillHeaders(webClient, headersAsDictionary);
            return webClient.UploadStringTaskAsync(address, method, data);
        }

        public void UploadStringAsync(Dictionary<string, string> headersAsDictionary, Action<object, dynamic> del, Uri address, string method, string data)
        {
            var webClient = new WebClient();
            //webClient.Encoding = encoding;
            FillHeaders(webClient, headersAsDictionary);
            webClient.UploadStringCompleted += (s,e) => del(s,e);
            webClient.UploadStringAsync(address, method, data);
        }

        public string UploadString(Dictionary<string, string> headersAsDictionary, Uri address, string method, string data)
        {
            var webClient = new WebClient();
            //webClient.Encoding = encoding;
            FillHeaders(webClient, headersAsDictionary);
            return webClient.UploadString(address, method, data);
        }

        void FillHeaders(WebClient webClient, Dictionary<string, string> headersAsDictionary)
        {
            var headers = new WebHeaderCollection();
            foreach (string key in headersAsDictionary.Keys)
            {
                headers.Add(key, headersAsDictionary[key]);
            }
            webClient.Headers = headers;
        }
    }
}
