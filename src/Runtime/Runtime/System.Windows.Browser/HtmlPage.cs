

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
using CSHTML5.Types;

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
        public static HtmlWindow Window => _initWindow ??= new HtmlWindow();

        /// <summary>
        /// Gets the browser's document object.
        /// </summary>
        public static HtmlDocument Document => _initDocument ??= new HtmlDocument();

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
        public static HtmlElement Plugin => _initPlugin ??= new HtmlElement();

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
                    string userAgent = OpenSilver.Interop.ExecuteJavaScriptString("navigator.userAgent", false)
                        ?? throw new InvalidOperationException("Cannot retrieve UserAgent");
                    string platform = OpenSilver.Interop.ExecuteJavaScriptString("navigator.platform", false);

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
            string sScriptKey = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(scriptKey);

            OpenSilver.Interop.ExecuteJavaScriptVoid($"window[{sScriptKey}]={{}};");

            var methods = instance.GetType().GetMethods()
                .Where(m => m.GetCustomAttributes(typeof(ScriptableMemberAttribute), false).Length > 0)
                .ToArray();

            foreach (var method in methods)
            {
                string sMethodName = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(method.Name);
                string sCallback = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS((Func<INTERNAL_JSObjectReference, object>)(jsObjectReference =>
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

                OpenSilver.Interop.ExecuteJavaScriptVoid($"window[{sScriptKey}][{sMethodName}] = function () {{ return {sCallback}(...arguments); }}");
            }

            var events = instance.GetType().GetEvents()
                .Where(e => e.GetCustomAttributes(typeof(ScriptableMemberAttribute), false).Length > 0)
                .ToArray();

            foreach (var eventInfo in events)
            {
                var es = new EventSubscriber(scriptKey, eventInfo.Name);
                var eventHandlerType = eventInfo.EventHandlerType;
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
