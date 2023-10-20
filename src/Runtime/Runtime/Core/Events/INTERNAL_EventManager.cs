
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
    [Obsolete]
    public class INTERNAL_EventManager<EVENT_HANDLER, EVENT_ARGS>
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

        private List<EVENT_HANDLER> _handlers = new List<EVENT_HANDLER>();
        private List<EVENT_HANDLER> _handlersForHandledEventsToo = new List<EVENT_HANDLER>();


        public List<EVENT_HANDLER> Handlers
        {
            get { return _handlers; }
        }
        public List<EVENT_HANDLER> HandlersForHandledEventsToo
        {
            get { return _handlersForHandledEventsToo; }
        }

        public bool IsSync
        {
            get { return _isSync && !OpenSilver.Interop.IsRunningInTheSimulator; }
        }

        private bool _isListeningToDomEvents = false; // Note: we listen to DOM events only when there is at least one C# event handler that is attached to this class, for best performance.

        //todo: see if we handle the case where an event is always attached here or in the class that has the event (the class attaches itself to the event)
        // Note: "domElement" here must be of type "object" rather than "dynamic" otherwise JSIL is unable to translate the calling method.
        public INTERNAL_EventManager(Func<object> domElementProvider, string domEventsNamesToListenTo, Action<object> actionOnEvent, bool sync = false)
        {
            //-----------------------------------
            // We instantiate this class once for each DOM event type and for each DOM element.
            //-----------------------------------

            _domEventsNamesToListenTo = new string[] { domEventsNamesToListenTo };
            _actionOnEvent = actionOnEvent;
            _domElementProvider = domElementProvider;
            _isSync = sync;
        }

        public INTERNAL_EventManager(Func<object> domElementProvider, string[] domEventsNamesToListenTo, Action<object> actionOnEvent, bool sync = false)
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

        public void Add(EVENT_HANDLER value, bool handledEventsToo = false)
        {
            //------------------------------
            // A C# event handler is added
            //------------------------------

            StartListeningToDomEventsIfNotAlreadyListening(); // Note: we listen to DOM events only when there is at least one C# event handler that is attached to this class, for best performance.

            if (handledEventsToo)
            {
                _handlersForHandledEventsToo.Add(value);
            }
            else
            {
                _handlers.Add(value);
            }
        }

        public void Remove(EVENT_HANDLER value)
        {
            //------------------------------
            // A C# event handler is removed
            //------------------------------

            HackToRemoveDelegateFromListOfDelegates(_handlers, value); // Note: we call this method because "_handlers.Remove(value)" does not work when translated to JavaScript due to the fact that the delegate "value" is not found in the list of handlers due to the fact that the comparison between 2 delegates in JSIL does not work properly.
            HackToRemoveDelegateFromListOfDelegates(_handlersForHandledEventsToo, value); // Note: we call this method because "_handlers.Remove(value)" does not work when translated to JavaScript due to the fact that the delegate "value" is not found in the list of handlers due to the fact that the comparison between 2 delegates in JSIL does not work properly.

            // Stop listening to DOM events if there is are remaining C# event handler attached:
            if (_handlers.Count == 0 && _handlersForHandledEventsToo.Count == 0)
                StopListeningToDomEvents();
        }

        void HackToRemoveDelegateFromListOfDelegates(List<EVENT_HANDLER> list, EVENT_HANDLER delegateToRemove)
        {
            // Read comment on the line where this method is called.

            for (int i = 0; i < list.Count; i++)
            {
                if (HackToCompareTwoDelegates(list[i], delegateToRemove))
                {
                    list.RemoveAt(i);
                    break;
                }
            }
        }

        bool HackToCompareTwoDelegates(EVENT_HANDLER delegate1, EVENT_HANDLER delegate2)
        {
            return (delegate1.Equals(delegate2));
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
            if (_handlers.Count > 0 || _handlersForHandledEventsToo.Count > 0)
            {
                StartListeningToDomEventsIfNotAlreadyListening();
            }
        }

        /// <summary>
        /// Attaches the EventManager to the dom event if there are handlers for the event or if the callback method was overriden.
        /// </summary>
        /// <param name="instance">The instance on which the events should be fired (normally "this").</param>
        /// <param name="callbackMethodOriginType">The type where the callback method was first defined (the method that is not an override, normally the type where the event manager was defined).</param>
        /// <param name="callbackMethodName">The name of the callback method that was potentially overriden.</param>
        /// <param name="callbackMethodParameterTypes">The list of the callback method argument types.</param>
        public void AttachToDomEvents(object instance, Type callbackMethodOriginType, string callbackMethodName, Type[] callbackMethodParameterTypes)
        {
            bool isMethodOverridden = INTERNAL_EventsHelper.IsEventCallbackOverridden(instance, callbackMethodOriginType, callbackMethodName, callbackMethodParameterTypes);
            

            //attach to the dom event if needed:
            if (isMethodOverridden || _handlers.Count > 0 || _handlersForHandledEventsToo.Count > 0)
            {
                StartListeningToDomEventsIfNotAlreadyListening();
            }
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
