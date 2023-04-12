
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
using System.ComponentModel;
using System.Security;
using CSHTML5;
using OpenSilver.Internal;

namespace System.Windows.Browser
{
    public sealed class HtmlDocument : HtmlObject
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(Helper.ObsoleteMemberMessage + " Use System.Windows.Browser.HtmlPage.Document instead.")]
        public HtmlDocument()
            : this(new DocumentRef())
        {
        }

        internal HtmlDocument(IJSObjectRef jsObject)
            : base(jsObject)
        {
        }

        /// <summary>
        /// Gets a Uniform Resource Identifier (URI) object that represents the current HTML
        /// document.
        /// </summary>
        public Uri DocumentUri => new Uri(OpenSilver.Interop.ExecuteJavaScriptString("window.location.href"));

        /// <summary>
        /// Gets a navigable, read-only collection of name/value pairs that represent
        /// the query string parameters on the current page's URL.
        /// </summary>
        public IDictionary<string, string> QueryString
        {
            get
            {
                Dictionary<string, string> query = new Dictionary<string, string>();
                
                string url = OpenSilver.Interop.ExecuteJavaScriptString("window.location.search");
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

        /// <summary>
        /// Gets a single browser element.
        /// </summary>
        /// <param name="id">
        /// A string identifier for a named browser element.
        /// </param>
        /// <returns>
        /// A reference to a browser element.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// id is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// id is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// An unexpected error occurred.
        /// </exception>
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
            get => OpenSilver.Interop.ExecuteJavaScriptString("document.cookie") ?? string.Empty;
            set => OpenSilver.Interop.ExecuteJavaScriptVoid($"document.cookie = {INTERNAL_InteropImplementation.GetVariableStringForJS(value)}");
        }

        [OpenSilver.NotImplemented]
        public HtmlElement Body { get; private set; }

        [OpenSilver.NotImplemented]
        public HtmlElement CreateElement(string tagName)
        {
            return default(HtmlElement);
        }

        /// <summary>
        /// Gets a collection of browser elements.
        /// </summary>
        /// <param name="tagName">
        /// A browser element's tag name.
        /// </param>
        /// <returns>
        /// A collection of references to HTML elements that correspond to the requested
        /// tag name.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// tagName is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// tagName is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// An unexpected error occurred.
        /// </exception>
        [SecuritySafeCritical]
        [OpenSilver.NotImplemented]
        public ScriptObjectCollection GetElementsByTagName(string tagName)
        {
            return default(ScriptObjectCollection);
        }
    }
}
