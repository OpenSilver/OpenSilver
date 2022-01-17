

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
                data = Convert.ToString(CSHTML5.Interop.ExecuteJavaScript("$0.handled", e));

            if (this.OnMessage != null)
                OnMessage(this, new OnMessageEventArgs(data));
        }

        void OnErrorCallback(object e)
        {
            string data = string.Empty;

            if (!Convert.ToBoolean(CSHTML5.Interop.ExecuteJavaScript("(typeof $0 === 'undefined')", e)))
                data = Convert.ToString(CSHTML5.Interop.ExecuteJavaScript("$0.handled", e));

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