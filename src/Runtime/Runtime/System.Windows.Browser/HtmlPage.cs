
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
using System.Windows.Browser.Internal;
using CSHTML5.Internal;
using CSHTML5.Types;

namespace System.Windows.Browser
{
    public static class HtmlPage
    {
        private static HtmlElement _plugin;
        private static BrowserInformation _browserInformation;

        /// <summary>
        /// Gets the browser's window object.
        /// </summary>
        public static HtmlWindow Window { get; } = new HtmlWindow(new WindowRef());

        /// <summary>
        /// Gets the browser's document object.
        /// </summary>
        public static HtmlDocument Document { get; } = new HtmlDocument(new DocumentRef());

        [OpenSilver.NotImplemented]
        public static bool IsPopupWindowAllowed => false;

        [OpenSilver.NotImplemented]
        public static HtmlWindow PopupWindow(Uri navigateToUri, string target, HtmlPopupWindowOptions options)
        {
            if (options != null)
            {
                Window.Navigate(navigateToUri, target, options.ToFeaturesString());
            }
            else
            {
                Window.Navigate(navigateToUri, target);
            }
            return null;
        }

        /// <summary>
        /// Gets a reference to the Silverlight plug-in that is defined within an &lt;object&gt;
        /// or &lt;embed&gt; tag on the host HTML page.
        /// </summary>
        public static HtmlElement Plugin
        {
            get
            {
                if (_plugin is null)
                {
                    if (Application.Current?.GetRootDiv() is INTERNAL_HtmlDomElementReference root)
                    {
                        _plugin = Document.GetElementById(root.UniqueIdentifier);
                    }
                }

                return _plugin;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the rest of the public surface area of the
        /// HTML Bridge feature is enabled.
        /// </summary>
        /// <returns>
        /// true if the HTML Bridge feature is enabled; otherwise, false.
        /// </returns>
        public static bool IsEnabled
        {
            get
            {
                var app = Application.Current;
                if (app is not null)
                {
                    return app.Host.Settings.EnableHTMLAccess;
                }

                return true;
            }
        }

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

        /// <summary>
        /// Registers a managed type as available for creation from JavaScript code, through
        /// the Content.services.createObject and Content.services.createManagedObject helper
        /// methods.
        /// </summary>
        /// <param name="scriptAlias">
        /// The name used to register the managed type.
        /// </param>
        /// <param name="type">
        /// A managed type.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// scriptAlias or type is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// type is not public.-or-type does not have a public, parameterless constructor.-or-type
        /// is a string, primitive or value type, managed delegate, or empty string.-or-type
        /// contains an embedded null character (<see cref="char.MinValue"/>).-or-An attempt is made to reregister
        /// type.
        /// </exception>
        [OpenSilver.NotImplemented]
        public static void RegisterCreateableType(string scriptAlias, Type type)
        {
            ScriptObject.ValidateParameter(scriptAlias);

            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (!type.IsVisible)
            {
                throw new ArgumentException($"'{nameof(type)}' must be a public type.", nameof(type));
            }

            if (type.IsAbstract)
            {
                throw new ArgumentException($"'{nameof(type)}' cannot be abstract.", nameof(type));
            }

            if (type.GetConstructor(Type.EmptyTypes) is null)
            {
                throw new ArgumentException($"'{nameof(type)}' must have a public parameterless constructor.", nameof(type));
            }

            if (type == typeof(string) || type.IsPrimitive || typeof(Delegate).IsAssignableFrom(type))
            {
                throw new ArgumentException($"'{nameof(type)}' is not a valid type.", nameof(type));
            }
        }
    }
}
