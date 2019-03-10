
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
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
    public sealed class HtmlDocument
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
                return new Uri(Interop.ExecuteJavaScript("window.location.href").ToString());
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
                string url = Interop.ExecuteJavaScript("document.URL").ToString();
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
