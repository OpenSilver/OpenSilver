
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



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
