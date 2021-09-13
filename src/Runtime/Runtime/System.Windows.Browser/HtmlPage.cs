

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
using System.Windows.Browser;

namespace System.Windows.Browser
{
    public static class HtmlPage
    {
        static HtmlWindow _initWindow;
        static HtmlDocument _initDocument;
        static HtmlElement _initPlugin;
        /// <summary>
        /// Gets the browser's window object.
        /// </summary>
        public static HtmlWindow Window
        {
            get
            {
                if (_initWindow == null)
                {
                    _initWindow = new HtmlWindow();
                }
                return _initWindow;
            }
        }

        /// <summary>
        /// Gets the browser's document object.
        /// </summary>
        public static HtmlDocument Document
        {
            get
            {
                if (_initDocument == null)
                {
                    _initDocument = new HtmlDocument();
                }
                return _initDocument;
            }
        }

		[OpenSilver.NotImplemented]
        public static HtmlWindow PopupWindow(Uri navigateToUri, string target, HtmlPopupWindowOptions options)
        {
            return null;
        }

        //
        // Summary:
        //     Gets a reference to the Silverlight plug-in that is defined within an <object>
        //     or <embed> tag on the host HTML page.
        //
        // Returns:
        //     The Silverlight plug-in in the Document Object Model (DOM).
		[OpenSilver.NotImplemented]
        public static HtmlElement Plugin
        {
            get
            {
                if (_initPlugin == null)
                {
                    _initPlugin = new HtmlElement();
                }
                return _initPlugin;
            }
        }

		[OpenSilver.NotImplemented]
        public static bool IsEnabled { get; private set; }

		[OpenSilver.NotImplemented]
        public static BrowserInformation BrowserInformation { get; private set; }

        /// <summary>
        /// Registers a managed object for scriptable access by JavaScript code.
        /// </summary>
        /// <param name="scriptKey">
        /// The name used to register the managed object.
        /// </param>
        /// <param name="instance">
        /// A managed object.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="scriptKey" /> or <paramref name="instance" /> is null.
        /// </exception>
        public static void RegisterScriptableObject(string scriptKey, object instance)
        {
            CSHTML5.Interop.ExecuteJavaScript("window[$0]={};", scriptKey);

            var methods = instance.GetType().GetMethods()
                .Where(m => m.GetCustomAttributes(typeof(ScriptableMemberAttribute), false).Length > 0)
                .ToArray();

            foreach (var method in methods)
            {
                CSHTML5.Interop.ExecuteJavaScript("window[$0][$1] = function () { return $2(...arguments); }", scriptKey, method.Name, (Func<CSHTML5.Types.INTERNAL_JSObjectReference, object>)(jsObjectReference =>
                {
                    var parameters = method.GetParameters();
                    var args = new object[parameters.Length];
                    for (var i = 0; i < args.Length; i++)
                    {
                        jsObjectReference.ArrayIndex = i;
                        args[i] = (jsObjectReference.IsNull() || jsObjectReference.IsUndefined()) ? null : Convert.ChangeType(jsObjectReference, parameters[i].ParameterType);
                    }
                    return method.Invoke(instance, args);
                }));
            }
        }
    }
}
