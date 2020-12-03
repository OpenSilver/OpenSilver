

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
#if !BUILDINGDOCUMENTATION && !CSHTML5NETSTANDARD
using DotNetBrowser;
using DotNetBrowser.DOM;
using DotNetBrowser.WPF;
using DotNetBrowser.DOM.Events;
using DotNetBrowser.Events;
#endif

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
        public static HtmlEventProxy AttachToDomEvents(string eventName, object domElementRef, Action<object> eventHandlerWithJsEventArg)
        {
            HtmlEventProxy newProxy = new HtmlEventProxy(eventName, domElementRef, eventHandlerWithJsEventArg);
            AttachEvent(eventName, domElementRef, newProxy, eventHandlerWithJsEventArg);
            return newProxy;
        }

#if !BRIDGE
        [JSReplacement("$domElementRef.addEventListener($eventName,$originalEventHandler)")]
#else
        [Template("{domElementRef}.addEventListener({eventName}, {originalEventHandler})")]
#endif
        static void AttachEvent(string eventName, object domElementRef, HtmlEventProxy newProxy, Action<object> originalEventHandler)
        {
#if !BUILDINGDOCUMENTATION

            Interop.ExecuteJavaScriptAsync(@"$0.addEventListener($1, $2)", domElementRef, eventName, (Action<object>)newProxy.OnEvent);

            /*
            DOMEventType eventType;
             * 
            // Listen to the event:
            if (TryConvertStringToDOMEventType(eventName, out eventType))
            {
                // Get the DOMDocument (a DotNetBrowser object) from the Simulator:
                DOMDocument document = INTERNAL_Simulator.DOMDocument;

                // Get the domElement from the id:
                var domElement = document.GetElementById(((INTERNAL_HtmlDomElementReference)(domElementRef)).UniqueIdentifier);

                // Add the listener:
                domElement.AddEventListener(eventType, newProxy.OnEvent, false); //todo: replace "false"?
            }
            */
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

            Interop.ExecuteJavaScriptAsync(@"
                var element = $0;
                if (element) {
                    element.removeEventListener($1, $2)
                }", domElementRef, eventName, (Action<object>)proxy.OnEvent);

            /*
            DOMEventType eventType;

            //add the event to the domElement
            if (TryConvertStringToDOMEventType(eventName, out eventType))
            {
                // Get the DOMDocument (a DotNetBrowser object) from the Simulator:
                DOMDocument document = INTERNAL_Simulator.DOMDocument;

                // Get the domElement from the id:
                var domElement = document.GetElementById(((INTERNAL_HtmlDomElementReference)(domElementRef)).UniqueIdentifier);

                // Remove the listener:
                if (domElement != null) //todo: this line was added when we moved from Awesomium to DotNetBrowser because otherwise a NullReferenceException occurred. However, it is strange that thre was no NullReferenceException with Awesomium. We should investigate to understand why, because it may be a sign of a change somewhere that has broken something.
                {
                    domElement.RemoveEventListener(eventType, proxy.OnEvent, false); //todo: replace "false"?
                }
            }
            */
#endif
        }


        /// <summary>
        /// Checks whether the method passed as parameter is an override by checking whether its DeclaringType is the same as the type passed as parameter.
        /// </summary>
        /// <param name="callbackMethodOriginType">The type where the method was originally declared.</param>
        /// <param name="callbackMethod">The method that will be checked whether it was declared in the type passed as parameter.</param>
        /// <returns>True if the method is an override (its DeclaringType is different than the type passed as parameter).</returns>
        public static bool IsEventCallbackOverridden(Type callbackMethodOriginType, Delegate callbackMethod)
        {
            return callbackMethod.Method.DeclaringType != callbackMethodOriginType;
        }


        /*
#if !BUILDINGDOCUMENTATION
        // This private method exists because of the current CORE architecture : 
        //
        //    at the function AttachEvent, we have a string 'eventName', which explains what kind
        //    of event we want to create. Due to the old 'Awesomium' system, we had to do it through
        //    javascript.
        //
        //    But the new one 'DotNetBrowser' allows us to use enum, and several methods that simplify
        //    the global task of this.
        //
        //    So, for now, to avoid changing the whole architecture, we created this private static method in order to
        //    keep this architecture, and make the right transition to DotNetBrowser.
        [JSIgnore]
        private static bool TryConvertStringToDOMEventType(string value, out DOMEventType domEventType)
        {
            string valueLowercase = value.ToLower();

            if (string.Equals(valueLowercase, "click"))
            {
                domEventType = DOMEventType.OnClick;
                return true;
            }
            else if (string.Equals(valueLowercase, "abort"))
            {
                domEventType = DOMEventType.OnAbort;
                return true;
            }
            else if (string.Equals(valueLowercase, "blur"))
            {
                domEventType = DOMEventType.OnBlur;
                return true;
            }
            else if (string.Equals(valueLowercase, "change"))
            {
                domEventType = DOMEventType.OnChange;
                return true;
            }
            else if (string.Equals(valueLowercase, "doubleclick"))
            {
                domEventType = DOMEventType.OnDoubleClick;
                return true;
            }
            else if (string.Equals(valueLowercase, "error"))
            {
                domEventType = DOMEventType.OnError;
                return true;
            }
            else if (string.Equals(valueLowercase, "focus"))
            {
                domEventType = DOMEventType.OnFocus;
                return true;
            }
            else if (string.Equals(valueLowercase, "keydown"))
            {
                domEventType = DOMEventType.OnKeyDown;
                return true;
            }
            else if (string.Equals(valueLowercase, "keypress"))
            {
                domEventType = DOMEventType.OnKeyPress;
                return true;
            }
            else if (string.Equals(valueLowercase, "keyup"))
            {
                domEventType = DOMEventType.OnKeyUp;
                return true;
            }
            else if (string.Equals(valueLowercase, "load"))
            {
                domEventType = DOMEventType.OnLoad;
                return true;
            }
            else if (string.Equals(valueLowercase, "mousedown"))
            {
                domEventType = DOMEventType.OnMouseDown;
                return true;
            }
            else if (string.Equals(valueLowercase, "mousemove"))
            {
                domEventType = DOMEventType.OnMouseMove;
                return false; //disable for test
            }
            else if (string.Equals(valueLowercase, "mouseout"))
            {
                domEventType = DOMEventType.OnMouseOut;
                return true;
            }
            else if (string.Equals(valueLowercase, "mouseover"))
            {
                domEventType = DOMEventType.OnMouseOver;
                return false; //disable for test
            }
            else if (string.Equals(valueLowercase, "mouseup"))
            {
                domEventType = DOMEventType.OnMouseUp;
                return true;
            }
            else if (string.Equals(valueLowercase, "reset"))
            {
                domEventType = DOMEventType.OnReset;
                return true;
            }
            else if (string.Equals(valueLowercase, "resize"))
            {
                domEventType = DOMEventType.OnResize;
                return true;
            }
            else if (string.Equals(valueLowercase, "scroll"))
            {
                domEventType = DOMEventType.OnScroll;
                return true;
            }
            else if (string.Equals(valueLowercase, "select"))
            {
                domEventType = DOMEventType.OnSelect;
                return true;
            }
            else if (string.Equals(valueLowercase, "submit"))
            {
                domEventType = DOMEventType.OnSubmit;
                return true;
            }
            else if (string.Equals(valueLowercase, "unload"))
            {
                domEventType = DOMEventType.OnUnload;
                return true;
            }
            else
            {
                domEventType = DOMEventType.Custom;
                return false;
            }
        }
#endif
         */
    }
}
