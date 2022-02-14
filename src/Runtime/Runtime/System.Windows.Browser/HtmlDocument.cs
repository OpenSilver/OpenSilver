

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
    public sealed partial class HtmlDocument : HtmlObject
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
                Dictionary<string, string> query = new Dictionary<string, string>();
                
                string url = OpenSilver.Interop.ExecuteJavaScript("window.location.search").ToString();
                if (!string.IsNullOrEmpty(url))
                {
                    string[] parts = url.Substring(1).Split(new char[1] { '&' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string part in parts)
                    {
                        int index = part.IndexOf('=');
                        if (index == -1)
                        {
                            query.Add(part, string.Empty);
                        }
                        else
                        {
                            query.Add(part.Substring(0, index), part.Substring(index + 1));
                        }
                    }
                }

                return query;
            }
        }
        //
        // Summary:
        //     Gets a single browser element.
        //
        // Parameters:
        //   id:
        //     A string identifier for a named browser element.
        //
        // Returns:
        //     A reference to a browser element.
        //
        // Exceptions:
        //   System.ArgumentException:
        //     id is an empty string.
        //
        //   System.ArgumentNullException:
        //     id is null.
        //
        //   System.InvalidOperationException:
        //     An unexpected error occurred.
		[OpenSilver.NotImplemented]
        public HtmlElement GetElementById(string id)
        {
            return null;
        }

        /// <summary>
        /// Gets or sets the browser's cookie string. If the browser does not have a 
        /// cookie string, the property returns an empty string.
        /// </summary>
        public string Cookies
        {
            get
            {
                return CSHTML5.Interop.ExecuteJavaScript("document.cookie").ToString() ?? string.Empty;
            }
            set
            {
                CSHTML5.Interop.ExecuteJavaScript("document.cookie = $0", value);
            }
        }
    }
}
