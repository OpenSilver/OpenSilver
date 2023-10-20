
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

namespace CSHTML5.Internal
{
    internal static class INTERNAL_EventsHelper
    {
        public static HtmlEventProxy AttachToDomEvents(string eventName, object domElementRef, Action<object> eventHandlerWithJsEventArg, bool isSync = false)
        {
            HtmlEventProxy newProxy = new HtmlEventProxy(eventName, domElementRef, eventHandlerWithJsEventArg, isSync);
            AttachEvent(eventName, domElementRef, newProxy, eventHandlerWithJsEventArg);
            return newProxy;
        }
        // Dictionary to remember which type overrides which event callback:
        static Dictionary<Type, Dictionary<string, bool>> _typesToOverridenCallbacks = new Dictionary<Type, Dictionary<string, bool>>();

        static void AttachEvent(string eventName, object domElementRef, HtmlEventProxy newProxy, Action<object> originalEventHandler)
        {
            string sAction = INTERNAL_InteropImplementation.GetVariableStringForJS(newProxy.Handler);
            if (domElementRef is INTERNAL_HtmlDomElementReference)
                OpenSilver.Interop.ExecuteJavaScriptFastAsync($@"document.addEventListenerSafe(""{((INTERNAL_HtmlDomElementReference)domElementRef).UniqueIdentifier}"", ""{eventName}"", {sAction})");
            else
                OpenSilver.Interop.ExecuteJavaScriptFastAsync($@"document.addEventListenerSafe({INTERNAL_InteropImplementation.GetVariableStringForJS(domElementRef)}, ""{eventName}"", {sAction})");
        }

        internal static void DetachEvent(string eventName, object domElementRef, HtmlEventProxy proxy, Action<object> originalEventHandler)
        {
            string sAction = INTERNAL_InteropImplementation.GetVariableStringForJS(proxy.Handler);
            if (domElementRef is INTERNAL_HtmlDomElementReference)
                OpenSilver.Interop.ExecuteJavaScriptFastAsync($@"document.removeEventListenerSafe(""{((INTERNAL_HtmlDomElementReference)domElementRef).UniqueIdentifier}"", ""{eventName}"", {sAction})");
            else
                OpenSilver.Interop.ExecuteJavaScriptFastAsync($@"document.removeEventListenerSafe({INTERNAL_InteropImplementation.GetVariableStringForJS(domElementRef)}, ""{eventName}"", {sAction})");
        }


        /// <summary>
        /// Checks whether the method passed as parameter is an override by checking whether its DeclaringType is the same as the type passed as parameter.
        /// </summary>
        /// <param name="instance">The instance on which the events should be fired (normally "this").</param>
        /// <param name="callbackMethodOriginType">The type where the method was originally declared.</param>
        /// <param name="callbackMethodName">The method that will be checked whether it was declared in the type passed as parameter.</param>
        /// <param name="callbackMethodParameterTypes">The list of the callback method argument types.</param>
        /// <returns>True if the method is an override (its DeclaringType is different than the type passed as parameter).</returns>
        public static bool IsEventCallbackOverridden(object instance, Type callbackMethodOriginType, string callbackMethodName, Type[] callbackMethodParameterTypes)
        {
            bool isMethodOverridden = false;
            bool needReflection = true;
            Type instanceType = instance.GetType();
            if (_typesToOverridenCallbacks.ContainsKey(instanceType))
            {
                //Note: if _typesToOverridenCallbacks contains the instance type, we already initialized the corresponding dictionary.
                if (_typesToOverridenCallbacks[instanceType].ContainsKey(callbackMethodName))
                {
                    isMethodOverridden = _typesToOverridenCallbacks[instanceType][callbackMethodName];
                    needReflection = false;
                }
            }
            else
            {
                //initialize the dictionary for the type:
                _typesToOverridenCallbacks.Add(instanceType, new Dictionary<string, bool>());
            }
            if (needReflection)
            {
                if (instanceType == callbackMethodOriginType)
                {
                    isMethodOverridden = false;
                }
                else
                {
                    isMethodOverridden = instanceType.GetMethod(callbackMethodName, global::System.Reflection.BindingFlags.NonPublic | global::System.Reflection.BindingFlags.Instance, null, callbackMethodParameterTypes, null).DeclaringType != callbackMethodOriginType;
                }
                // Remember whether the event callback was overriden or not for the next time:
                _typesToOverridenCallbacks[instanceType].Add(callbackMethodName, isMethodOverridden);
            }

            return isMethodOverridden;
        }
    }
}
