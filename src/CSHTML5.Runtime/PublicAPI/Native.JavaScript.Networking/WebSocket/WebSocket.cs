
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



/*
using System;

namespace CSHTML5.Native.JavaScript.WebSockets
{
    public class WebSocket
    {
        object _referenceToTheJavaScriptWebSocketObject;

        public event EventHandler OnOpen;
        public event EventHandler OnClose;
        public event EventHandler<OnMessageEventArgs> OnMessage;
        public event EventHandler<OnErrorEventArgs> OnError;

        public WebSocket(string uri)
        {
            _referenceToTheJavaScriptWebSocketObject = CSHTML5.Interop.ExecuteJavaScript("new WebSocket($0)", uri);

            CSHTML5.Interop.ExecuteJavaScript(
                    @"$0.onopen = $1;
                      $0.onclose = $2;
                      $0.onmessage = $3;
                      $0.onerror = $4",
                _referenceToTheJavaScriptWebSocketObject,
                (Action<object>)this.OnOpenCallback,
                (Action<object>)this.OnCloseCallback,
                (Action<object>)this.OnMessageCallback,
                (Action<object>)this.OnErrorCallback);
        }

        public void Send(string message)
        {
            CSHTML5.Interop.ExecuteJavaScript("$0.send($1)", _referenceToTheJavaScriptWebSocketObject, message);
        }

        public void Close()
        {
            CSHTML5.Interop.ExecuteJavaScript("$0.close()", _referenceToTheJavaScriptWebSocketObject);
        }

        void OnOpenCallback(object e)
        {
            if (this.OnOpen != null)
                OnOpen(this, new EventArgs());
        }

        void OnCloseCallback(object e)
        {
            if (this.OnClose != null)
                OnClose(this, new EventArgs());
        }

        void OnMessageCallback(object e)
        {
            string data = string.Empty;

            if (!Convert.ToBoolean(CSHTML5.Interop.ExecuteJavaScript("(typeof $0 === 'undefined')", e)))
                data = Convert.ToString(CSHTML5.Interop.ExecuteJavaScript("$0.data", e));

            if (this.OnMessage != null)
                OnMessage(this, new OnMessageEventArgs(data));
        }

        void OnErrorCallback(object e)
        {
            string data = string.Empty;

            if (!Convert.ToBoolean(CSHTML5.Interop.ExecuteJavaScript("(typeof $0 === 'undefined')", e)))
                data = Convert.ToString(CSHTML5.Interop.ExecuteJavaScript("$0.data", e));

            if (this.OnError != null)
                OnError(this, new OnErrorEventArgs(data));
        }

        public ReadyState ReadyState
        {
            get
            {
                int readyStateInt = Convert.ToInt32(CSHTML5.Interop.ExecuteJavaScript(@"$0.readyState", _referenceToTheJavaScriptWebSocketObject));
                return (ReadyState)readyStateInt;
            }
        }
    }
}
*/