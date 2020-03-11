

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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSHTML5;

namespace System.Windows.Browser
{
#if WORKINPROGRESS
    public sealed partial class HtmlDocument : HtmlObject
#else
    public sealed partial class HtmlDocument
#endif
    {
        public HtmlDocument()
        {

        }

        /// <summary>
        /// Gets a Uniform Resource Identifier (URI) object that represents the current HTML
        /// document.
        /// </summary>
        public Uri DocumentUri
        {
            get
            {
                return new Uri(CSHTML5.Interop.ExecuteJavaScript("window.location.href").ToString());
            }
        }

        /// <summary>
        /// Gets a navigable, read-only collection of name/value pairs that represent
        /// the query string parameters on the current page's URL.
        /// </summary>
        public IDictionary<string, string> QueryString
        {
            get
            {
                //get the current page's URL
                //get what is after '?'
                //split it using '&'
                //for each element of the array we obtained, split it (with '=') and put the first element in the "key" and the second in the "value"
                Dictionary<string, string> returnValue = new Dictionary<string, string>();
                string url = CSHTML5.Interop.ExecuteJavaScript("document.URL").ToString();
                int index = url.IndexOf('?');
                if (index != -1)
                {
                    url = url.Substring(index + 1);
                    string[] splittedParameters = url.Split('&');
                    foreach (string str in splittedParameters)
                    {
                        string[] splittedParameter = str.Split('=');
                        returnValue.Add(splittedParameter[0], splittedParameter[1]);
                    }
                }
                return returnValue;
            }
        }

#if WORKINPROGRESS
        public HtmlElement Body { get; private set; }

        public HtmlElement CreateElement(string tagName)
        {
            return null;
        }
#endif
    }
}
