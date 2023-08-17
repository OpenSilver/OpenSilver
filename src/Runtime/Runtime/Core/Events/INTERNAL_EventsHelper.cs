

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


#if !BRIDGE
using JSIL.Meta;
#else
using Bridge;
#endif
using CSHTML5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CSHTML5.Internal
{
    static class INTERNAL_EventsHelper
    {
        public static HtmlEventProxy AttachToDomEvents(string eventName, object domElementRef, Action<object> eventHandlerWithJsEventArg, bool isSync = false)
        {
            HtmlEventProxy newProxy = new HtmlEventProxy(eventName, domElementRef, eventHandlerWithJsEventArg, isSync);
            AttachEvent(eventName, domElementRef, newProxy, eventHandlerWithJsEventArg);
            return newProxy;
        }
        // Dictionary to remember which type overrides which event callback:
        static Dictionary<Type, Dictionary<string, bool>> _typesToOverridenCallbacks = new Dictionary<Type, Dictionary<string, bool>>();

#if !BRIDGE
        [JSReplacement("$domElementRef.addEventListener($eventName,$originalEventHandler)")]
#else
        [Template("{domElementRef}.addEventListener({eventName}, {originalEventHandler})")]
#endif
        static void AttachEvent(string eventName, object domElementRef, HtmlEventProxy newProxy, Action<object> originalEventHandler)
        {
#if !BUILDINGDOCUMENTATION
            string sAction = INTERNAL_InteropImplementation.GetVariableStringForJS(newProxy.Handler);
            if (domElementRef is INTERNAL_HtmlDomElementReference)
                Interop.ExecuteJavaScriptFastAsync($@"document.addEventListenerSafe(""{((INTERNAL_HtmlDomElementReference)domElementRef).UniqueIdentifier}"", ""{eventName}"", {sAction})");
            else
                Interop.ExecuteJavaScriptFastAsync($@"document.addEventListenerSafe({INTERNAL_InteropImplementation.GetVariableStringForJS(domElementRef)}, ""{eventName}"", {sAction})");

#endif
        }

#if !BRIDGE
        [JSReplacement("$domElementRef.removeEventListener($eventName,$originalEventHandler)")]
#else
        [Template("{domElementRef}.removeEventListener({eventName},{originalEventHandler})")]
#endif
        internal static void DetachEvent(string eventName, object domElementRef, HtmlEventProxy proxy, Action<object> originalEventHandler)
        {
#if !BUILDINGDOCUMENTATION
            string sAction = INTERNAL_InteropImplementation.GetVariableStringForJS(proxy.Handler);
            if (domElementRef is INTERNAL_HtmlDomElementReference)
                Interop.ExecuteJavaScriptFastAsync($@"document.removeEventListenerSafe(""{((INTERNAL_HtmlDomElementReference)domElementRef).UniqueIdentifier}"", ""{eventName}"", {sAction})");
            else
                Interop.ExecuteJavaScriptFastAsync($@"document.removeEventListenerSafe({INTERNAL_InteropImplementation.GetVariableStringForJS(domElementRef)}, ""{eventName}"", {sAction})");
#endif
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
#if !NETSTANDARD
                    if (Interop.IsRunningInTheSimulator) //Bridge does not provide a fitting existing (existing as in it exists in C#) overload for Type.GetMethod to get a method from a type, a name, and specific parameters when the method is not public, so we do our own. 
                    {
                        var methods = instanceType.GetMethods(global::System.Reflection.BindingFlags.NonPublic | global::System.Reflection.BindingFlags.Instance).Where((method) => { return method.Name == callbackMethodName; }).ToArray();
                        if (methods.Length == 0)
                        {
                            throw new MissingMethodException("Could not find the method \"" + callbackMethodName + "\" in type " + instanceType.FullName);
                        }
                        global::System.Reflection.MethodInfo fittingMethod = null;
                        foreach (var method in methods)
                        {
                            int i = 0;
                            bool isTheOne = true;
                            var parameters = method.GetParameters();
                            foreach (var parameter in parameters)
                            {
                                if (parameter.ParameterType != callbackMethodParameterTypes[i])
                                {
                                    isTheOne = false;
                                    break;
                                }
                                ++i;
                            }
                            if (isTheOne)
                            {
                                fittingMethod = method;
                                break;
                            }
                        }

                        if (fittingMethod == null)
                        {
                            List<string> parametersTypesAsString = new List<string>();
                            foreach (var paramType in callbackMethodParameterTypes)
                            {
                                parametersTypesAsString.Add(paramType.FullName);
                            }
                            throw new MissingMethodException("Could not find the method \"" + callbackMethodName + "(" + string.Join(", ", parametersTypesAsString) + ")" + "\" in type " + instanceType.FullName);
                        }

                        // We foud a fitting method, now we check whether its declaring type is the same as the origin type:
                        isMethodOverridden = fittingMethod.DeclaringType != callbackMethodOriginType;
                    }
                    else //Bridge provides Type.GetMethod(String, BindingFlags, Types[]) which does what we want but does not exist in actual C# so we use it when we are not in the simulator:
                    {
                        isMethodOverridden = IsMethodOverriden_BrowserOnly(callbackMethodOriginType, callbackMethodName, callbackMethodParameterTypes, instanceType);
                    }
#else
                    isMethodOverridden = instanceType.GetMethod(callbackMethodName, global::System.Reflection.BindingFlags.NonPublic | global::System.Reflection.BindingFlags.Instance, null, callbackMethodParameterTypes, null).DeclaringType != callbackMethodOriginType;
#endif
                }
                // Remember whether the event callback was overriden or not for the next time:
                _typesToOverridenCallbacks[instanceType].Add(callbackMethodName, isMethodOverridden);
            }

            return isMethodOverridden;
        }
#if !NETSTANDARD
        private static bool IsMethodOverriden_BrowserOnly(Type callbackMethodOriginType, string callbackMethodName, Type[] callbackMethodParameterTypes, Type instanceType)
        {
            // Note: This is in its own method because the signature of Type.GetMethod below does not exist in actual C# and we get an exception in the simulator
            //       the moment we try to enter a method containing calls to methods that "do not exist" (it is defined in Bridge but not in C# so it does not exist from the point of vue of C#).
            return instanceType.GetMethod(callbackMethodName, global::System.Reflection.BindingFlags.NonPublic | global::System.Reflection.BindingFlags.Instance, callbackMethodParameterTypes).DeclaringType != callbackMethodOriginType;
        }
#endif
    }
}
