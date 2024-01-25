
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

namespace CSHTML5.Internal
{
    internal sealed class DOMEventManager
    {
        //-----------------------------------------------------------
        // Each "HtmlEventProxy" proxy is the link between a DOM event
        // and a C# event.
        // We create one proxy for each type of DOM event, and for each
        // DOM element. When the DOM event is raised, we call "OnEvent"
        // which then raises the "_actionOnEvent".
        // Usually, the class that instantiates this INTERNAL_EventManager will
        // then call all the C# handlers associated to that event.
        //-----------------------------------------------------------

        private readonly Func<object> _domElementProvider;
        private readonly string[] _domEventsNamesToListenTo;
        private readonly Delegate _handler;

        private object _domElement;
        private bool _isListeningToDomEvents = false; // Note: we listen to DOM events only when there is at least one C# event handler that is attached to this class, for best performance.

        //todo: see if we handle the case where an event is always attached here or in the class that has the event (the class attaches itself to the event)
        // Note: "domElement" here must be of type "object" rather than "dynamic" otherwise JSIL is unable to translate the calling method.
        public DOMEventManager(Func<object> domElementProvider, string domEventsNamesToListenTo, Action<object> actionOnEvent, bool sync = false)
            : this(domElementProvider, new string[1] { domEventsNamesToListenTo }, actionOnEvent, sync) 
        { 
        }

        public DOMEventManager(Func<object> domElementProvider, string[] domEventsNamesToListenTo, Action<object> actionOnEvent, bool sync = false)
        {
            //-----------------------------------
            // Alternatively, when multiple DOM event types correspond to the same behavior,
            // we can call this constructor alternative to pass a list of DOM event types to
            // listen to simultaneously.
            //-----------------------------------

            _domElementProvider = domElementProvider ?? throw new ArgumentNullException(nameof(domElementProvider));
            _domEventsNamesToListenTo = domEventsNamesToListenTo;
            _handler = CreateHandler(actionOnEvent, sync);
        }

        private Delegate CreateHandler(Action<object> handler, bool sync)
        {
            if (sync && !OpenSilver.Interop.IsRunningInTheSimulator)
            {
                return new Func<object, string>(jsEventArg =>
                {
                    handler?.Invoke(jsEventArg);
                    return "";
                });
            }

            return new Action<object>(jsEventArg => handler?.Invoke(jsEventArg));
        }

        public void AttachToDomEvents()
        {
            if (_isListeningToDomEvents)
            {
                return;
            }

            object domElement = _domElementProvider();
            if (domElement != null)
            {
                foreach (string eventName in _domEventsNamesToListenTo)
                {
                    string sElement = OpenSilver.Interop.GetVariableStringForJS(domElement);
                    string sAction = OpenSilver.Interop.GetVariableStringForJS(_handler);
                    OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                        $@"document.addEventListenerSafe({sElement}, ""{eventName}"", {sAction})"
                    );
                }

                _domElement = domElement;
                _isListeningToDomEvents = true;
            }
        }

        public void DetachFromDomEvents()
        {
            if (_isListeningToDomEvents)
            {
                for (int i = _domEventsNamesToListenTo.Length - 1; i >= 0; i--)
                {
                    string sElement = OpenSilver.Interop.GetVariableStringForJS(_domElement);
                    OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                        $@"document.removeEventListenerSafe({sElement}, ""{_domEventsNamesToListenTo[i]}"")"
                    );
                }
            }

            _domElement = null;
            _isListeningToDomEvents = false;
        }
    }
}
