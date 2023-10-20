
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

using System.Dynamic;
using System;
using CSHTML5.Internal;
using System.Collections.Generic;

public static partial class CSharpXamlForHtml5
{
    public static partial class DomManagement
    {
        public static partial class Types
        {
            // Verify : this class is not supposed to be called by bridge.NET itself
            // Note: this class is intented to be used by the Simulator only, not when compiled to JavaScript.

            public class DynamicDomElement : DynamicObject
            {
                Dictionary<Tuple<string, Action<object>>, HtmlEventProxy> _eventNameAndHandlerToHtmlEventProxy = null;

                INTERNAL_HtmlDomElementReference _domElementRef;

                internal DynamicDomElement(INTERNAL_HtmlDomElementReference domElementRef)
                {
                    _domElementRef = domElementRef;
                }

                public override bool TrySetMember(SetMemberBinder binder, object value)
                {
                    INTERNAL_HtmlDomManager.SetDomElementAttribute(_domElementRef, binder.Name, value);
                    return true;
                }

                public override bool TryGetMember(GetMemberBinder binder, out object result)
                {
                    string attributeName = binder.Name;
                    if (attributeName == "style")
                        result = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(_domElementRef);
                    else
                        result = INTERNAL_HtmlDomManager.GetDomElementAttribute(_domElementRef, attributeName);
                    return true;
                }

                public void removeAttribute(string attributeName) //todo: generalize to all the methods (thanks to DynamicObject)
                {
                    INTERNAL_HtmlDomManager.RemoveDomElementAttribute(_domElementRef, attributeName, forceSimulatorExecuteImmediately: true);
                }

                public void addEventListener(string eventName, Action<object> handler)
                {
                    HtmlEventProxy proxy = INTERNAL_EventsHelper.AttachToDomEvents(eventName, _domElementRef, handler);
                    if (_eventNameAndHandlerToHtmlEventProxy == null)
                    {
                        _eventNameAndHandlerToHtmlEventProxy = new Dictionary<Tuple<string, Action<object>>, HtmlEventProxy>();
                    }
                    _eventNameAndHandlerToHtmlEventProxy.Add(new Tuple<string, Action<object>>(eventName, handler), proxy);
                }

                public void removeEventListener(string eventName, Action<object> handler)
                {
                    Tuple<string, Action<object>> key = new Tuple<string, Action<object>>(eventName, handler);
                    if (_eventNameAndHandlerToHtmlEventProxy !=  null && _eventNameAndHandlerToHtmlEventProxy.ContainsKey(key))
                    {
                        INTERNAL_EventsHelper.DetachEvent(eventName, _domElementRef, _eventNameAndHandlerToHtmlEventProxy[key], handler);
                    }
                    //else do nothing.
                }

                public void appendChild(object domElementRef)
                {
                    INTERNAL_HtmlDomManager.AppendChild_ForUseByPublicAPIOnly_SimulatorOnly(((DynamicDomElement)domElementRef)._domElementRef, this._domElementRef);
                }


                DynamicDomElementChildrenCollection _childNodes;
                public DynamicDomElementChildrenCollection childNodes
                {
                    get
                    {
                        if (_childNodes == null)
                        {
                            _childNodes = new DynamicDomElementChildrenCollection(this._domElementRef);
                        }
                        return _childNodes;
                    }
                }
            }
        }
    }
}
