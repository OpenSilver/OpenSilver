

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
using System.Globalization;
using System.Reflection;
using System.Diagnostics;
using System.Windows;

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
        private Delegate _handler;

        // Constructor
        internal HtmlEventProxy(string eventName, object domElementRef, Action<object> originalEventHandler, bool sync)
        {
            this._eventName = eventName;
            this._domElementRef = domElementRef;
            this._sender = this;
            this._eventHandler = (EventHandler<HtmlEventProxy.EventArgsWithJSEventObject>)((s, e) => { originalEventHandler(e.JSEventObject); });
            this._originalEventHandler = originalEventHandler;
            this._handler = CreateHandler(sync);
        }

        public Delegate Handler
        {
            get { return _handler; }
        }

        private void OnEventImpl(object jsEventArg)
        {
            if (this._eventHandler != null)
            {
                this._eventHandler(this._sender, new EventArgsWithJSEventObject(jsEventArg));
            }
        }

        private Delegate CreateHandler(bool sync)
        {
            if (sync)
            {
                return new Func<object, string>(jsEventArg =>
                {
                    OnEventImpl(jsEventArg);
                    return "";
                });
            }

            return new Action<object>(jsEventArg => OnEventImpl(jsEventArg));
        }

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
                _handler = null;
            }
        }

    }
}
