
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

        // Keeping this allows us to detach it from the Dom element when there is no subscriber left:
        private List<HtmlEventProxy> _htmlEventProxies = new List<HtmlEventProxy>();

        private string[] _domEventsNamesToListenTo;
        private Action<object> _actionOnEvent;
        private Func<object> _domElementProvider;
        private readonly bool _isSync;

        public bool IsSync
        {
            get { return _isSync && !OpenSilver.Interop.IsRunningInTheSimulator; }
        }

        private bool _isListeningToDomEvents = false; // Note: we listen to DOM events only when there is at least one C# event handler that is attached to this class, for best performance.

        //todo: see if we handle the case where an event is always attached here or in the class that has the event (the class attaches itself to the event)
        // Note: "domElement" here must be of type "object" rather than "dynamic" otherwise JSIL is unable to translate the calling method.
        public DOMEventManager(Func<object> domElementProvider, string domEventsNamesToListenTo, Action<object> actionOnEvent, bool sync = false)
        {
            //-----------------------------------
            // We instantiate this class once for each DOM event type and for each DOM element.
            //-----------------------------------

            _domEventsNamesToListenTo = new string[] { domEventsNamesToListenTo };
            _actionOnEvent = actionOnEvent;
            _domElementProvider = domElementProvider;
            _isSync = sync;
        }

        public DOMEventManager(Func<object> domElementProvider, string[] domEventsNamesToListenTo, Action<object> actionOnEvent, bool sync = false)
        {
            //-----------------------------------
            // Alternatively, when multiple DOM event types correspond to the same behavior,
            // we can call this constructor alternative to pass a list of DOM event types to
            // listen to simultaneously.
            //-----------------------------------

            _domEventsNamesToListenTo = domEventsNamesToListenTo;
            _actionOnEvent = actionOnEvent;
            _domElementProvider = domElementProvider;
            _isSync = sync;
        }

        /// <summary>
        /// Raises the event
        /// </summary>
        /// <param name="jsEventArg">The javascript event argument.</param>
        void OnEvent(object jsEventArg)
        {
            if (_htmlEventProxies != null && _htmlEventProxies.Count != 0)
            {
                _actionOnEvent(jsEventArg);
            }
        }

        public void AttachToDomEvents()
        {
            StartListeningToDomEventsIfNotAlreadyListening();
        }

        public void DetachFromDomEvents()
        {
            if (_isListeningToDomEvents && _htmlEventProxies != null)
            {
                StopListeningToDomEvents();
            }
        }

        void StartListeningToDomEventsIfNotAlreadyListening()
        {
            if (!_isListeningToDomEvents)
            {
                var domElement = _domElementProvider();
                if (domElement != null)
                {
                    if (_htmlEventProxies == null)
                        _htmlEventProxies = new List<HtmlEventProxy>();

                    foreach (string eventName in _domEventsNamesToListenTo)
                    {
                        _htmlEventProxies.Add(INTERNAL_EventsHelper.AttachToDomEvents(eventName, domElement, (Action<object>)(jsEventArg =>
                        {
                            OnEvent(jsEventArg);
                        }), IsSync));
                    }

                    _isListeningToDomEvents = true;
                }
            }
        }

        void StopListeningToDomEvents() // Note: we stop listening to DOM events if there are no more C# event handlers attached to this class, for best performance, or when detaching the object from the visual tree.
        {
            if (_isListeningToDomEvents && _htmlEventProxies != null)
            {
                for (int i = _htmlEventProxies.Count - 1; i >= 0; i--)
                {
                    _htmlEventProxies[i].Dispose(); // Note: this will detach the DOM event.
                    _htmlEventProxies.RemoveAt(i);
                }
            }
            _isListeningToDomEvents = false;
        }
    }
}
