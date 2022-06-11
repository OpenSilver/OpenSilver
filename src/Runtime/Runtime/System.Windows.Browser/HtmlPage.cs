

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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Browser;

namespace System.Windows.Browser
{
    public static class HtmlPage
    {
        private static HtmlWindow _initWindow;
        private static HtmlDocument _initDocument;
        private static HtmlElement _initPlugin;
        private static BrowserInformation _browserInformation;

        /// <summary>
        /// Gets the browser's window object.
        /// </summary>
        public static HtmlWindow Window => _initWindow ?? (_initWindow = new HtmlWindow());

        /// <summary>
        /// Gets the browser's document object.
        /// </summary>
        public static HtmlDocument Document => _initDocument ?? (_initDocument = new HtmlDocument());

        [OpenSilver.NotImplemented]
        public static bool IsPopupWindowAllowed => false;

        [OpenSilver.NotImplemented]
        public static HtmlWindow PopupWindow(Uri navigateToUri, string target, HtmlPopupWindowOptions options)
        {
            return null;
        }

        /// <summary>
        /// Gets a reference to the Silverlight plug-in that is defined within an &lt;object&gt;
        /// or &lt;embed&gt; tag on the host HTML page.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static HtmlElement Plugin => _initPlugin ?? (_initPlugin = new HtmlElement());

        [OpenSilver.NotImplemented]
        public static bool IsEnabled { get; private set; }

        /// <summary>
        /// Gets general information about the browser, such as name, version, and operating system.
        /// </summary>
        /// <returns>
        /// An object that represents general information about the hosting browser.
        /// </returns>
        public static BrowserInformation BrowserInformation
        {
            get
            {
                if (_browserInformation == null)
                {
                    string userAgent = Convert.ToString(OpenSilver.Interop.ExecuteJavaScript("navigator.userAgent"))
                        ?? throw new InvalidOperationException("Cannot retrieve UserAgent");
                    string platform = Convert.ToString(OpenSilver.Interop.ExecuteJavaScript("navigator.platform"));

                    _browserInformation = new BrowserInformation(userAgent, platform);
                }

                return _browserInformation;
            }
        }

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
            OpenSilver.Interop.ExecuteJavaScript("window[$0]={};", scriptKey);

            var methods = instance.GetType().GetMethods()
                .Where(m => m.GetCustomAttributes(typeof(ScriptableMemberAttribute), false).Length > 0)
                .ToArray();

            foreach (var method in methods)
            {
                OpenSilver.Interop.ExecuteJavaScript("window[$0][$1] = function () { return $2(...arguments); }", scriptKey, method.Name, (Func<CSHTML5.Types.INTERNAL_JSObjectReference, object>)(jsObjectReference =>
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

            var events = instance.GetType().GetEvents()
                .Where(e => e.GetCustomAttributes(typeof(ScriptableMemberAttribute), false).Length > 0)
                .ToArray();

            foreach (var eventInfo in events)
            {
                var es = new EventSubscriber(scriptKey, eventInfo.Name);
#if BRIDGE
                //Bridge.Net does not support EventHandlerType.
                var eventHandlerType = eventInfo.AddMethod.GetParameters()[0].ParameterType;
#else
                var eventHandlerType = eventInfo.EventHandlerType;
#endif

                var methodName = "OnRaisedBridgeNet";
                var method = eventHandlerType.GetMethod("Invoke");
                if (method != null)
                {
                    //We are in OpenSilver or in the Simulator
                    methodName = "OnRaised" + method.GetParameters().Length;
                }

                var d = Delegate.CreateDelegate(eventHandlerType, es,
                    es.GetType().GetMethods().First(mi => mi.Name == methodName));
                eventInfo.AddEventHandler(instance, d);
            }
        }
    }
}
