

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


#if !BUILDINGDOCUMENTATION && !CSHTML5NETSTANDARD && !BRIDGE
using DotNetBrowser;
using DotNetBrowser.DOM.Events;
#endif


#if !BRIDGE
using JSIL.Meta;
#else
using Bridge;
#endif

using System;
using System.Globalization;
using System.Reflection;
using System.Diagnostics;
#if MIGRATION
using System.Windows;
#else
using Windows.UI.Core;
#endif

namespace CSHTML5.Internal
{
    /// <summary>
    /// </summary>
    /// <exclude/>
    public class HtmlEventProxy : IDisposable
    {
        /// <summary>
        /// </summary>
        /// <exclude/>
        public class EventArgsWithJSEventObject : EventArgs
        {
            public readonly object JSEventObject;
            public EventArgsWithJSEventObject(object jsEventObject)
            {
                JSEventObject = jsEventObject;
            }
        }

        // Fields
        private EventHandler<EventArgsWithJSEventObject> _eventHandler;
        private Action<object> _originalEventHandler;
        private object _sender;
        private object _domElementRef;
        private string _eventName = null;

        // Constructor
        internal HtmlEventProxy(string eventName, object domElementRef, Action<object> originalEventHandler)
        {
            this._eventName = eventName;
            this._domElementRef = domElementRef;
            this._sender = this;
            this._eventHandler = (EventHandler<HtmlEventProxy.EventArgsWithJSEventObject>)((s, e) => { originalEventHandler(e.JSEventObject); });
            this._originalEventHandler = originalEventHandler;
        }
#if !BUILDINGDOCUMENTATION
#if !BRIDGE
        [JSIgnore]
#else
        [External]
#endif
        public void OnEvent(object jsEventArg)
        {
            if (this._eventHandler != null)
            {
                //this._eventHandler(this._sender, new EventArgsWithJSEventObject(new INTERNAL_SimulatorJSEventObject(jsEventArg)));
                this._eventHandler(this._sender, new EventArgsWithJSEventObject(jsEventArg));
            }

            //dynamic mouseEvent = (JSObject)arguments[0];
            //String message = String.Format("Mouse over at: {0}x{1}", mouseEvent.clientX, mouseEvent.clientY);
        }
#else
        public void OnEvent(object arguments) { }
#endif

        public void Dispose()
        {
            if (_domElementRef != null)
            {
                INTERNAL_EventsHelper.DetachEvent(_eventName, _domElementRef, this, _originalEventHandler);

                // Free memory:
                _domElementRef = null;
                _sender = null;
                _eventHandler = null;
                _originalEventHandler = null;
            }
        }

    }
}
