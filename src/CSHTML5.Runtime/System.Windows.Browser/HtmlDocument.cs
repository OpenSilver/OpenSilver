
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



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
    }
}
