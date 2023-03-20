

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


#if BRIDGE
using Bridge;
#endif
using System.Linq;

namespace System.Windows.Browser
{
    internal sealed class EventSubscriber
    {
        private string _scriptKey;
        private string _functionName;

        public EventSubscriber(string scriptKey, string functionName)
        {
            _scriptKey = scriptKey;
            _functionName = functionName;
        }

        public void OnRaised0()
        {
            OpenSilver.Interop.ExecuteJavaScriptVoid("callScriptableObjectEvent($0, $1, [])", flushQueue:false, 
                _scriptKey, _functionName);
        }

        public void OnRaised1(object obj1)
        {
            OpenSilver.Interop.ExecuteJavaScriptVoid("callScriptableObjectEvent($0, $1, [$2])", flushQueue:false, 
                _scriptKey, _functionName, obj1);
        }

        public void OnRaised2(object obj1, object obj2)
        {
            OpenSilver.Interop.ExecuteJavaScriptVoid("callScriptableObjectEvent($0, $1, [$2, $3])", flushQueue:false, 
                _scriptKey, _functionName, obj1, obj2);
        }

        public void OnRaised3(object obj1, object obj2, object obj3)
        {
            OpenSilver.Interop.ExecuteJavaScriptVoid("callScriptableObjectEvent($0, $1, [$2, $3, $4])", flushQueue:false, 
                _scriptKey, _functionName, obj1, obj2, obj3);
        }

        public void OnRaised4(object obj1, object obj2, object obj3, object obj4)
        {
            OpenSilver.Interop.ExecuteJavaScriptVoid("callScriptableObjectEvent($0, $1, [$2, $3, $4, $5])", flushQueue:false, 
                _scriptKey, _functionName, obj1, obj2, obj3, obj4);
        }

        public void OnRaised5(object obj1, object obj2, object obj3, object obj4, object obj5)
        {
            OpenSilver.Interop.ExecuteJavaScriptVoid("callScriptableObjectEvent($0, $1, [$2, $3, $4, $5, $6])", flushQueue:false, 
                _scriptKey, _functionName, obj1, obj2, obj3, obj4, obj5);
        }

#if BRIDGE
        public void OnRaisedBridgeNet(object obj1, object obj2, object obj3, object obj4, object obj5)
        {
            if (obj1 == Script.Undefined)
            {
                OnRaised0();
            }
            else if (obj2 == Script.Undefined)
            {
                OnRaised1(obj1);
            }
            else if (obj3 == Script.Undefined)
            {
                OnRaised2(obj1, obj2);
            }
            else if (obj4 == Script.Undefined)
            {
                OnRaised3(obj1, obj2, obj3);
            }
            else if (obj5 == Script.Undefined)
            {
                OnRaised4(obj1, obj2, obj3, obj4);
            }
            else
            {
                OnRaised5(obj1, obj2, obj3, obj4, obj5);
            }
        }
#endif
    }
}
