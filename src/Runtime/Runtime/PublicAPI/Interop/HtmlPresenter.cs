

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


using CSHTML5.Internal;
using System;
using System.Windows.Markup;
#if !CSHTML5NETSTANDARD
using DotNetBrowser;
#endif

#if MIGRATION
using System.Windows;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

#if BRIDGE
using Bridge;
#endif

namespace CSHTML5.Native.Html.Controls
{
    [ContentProperty("Html")]
    public class HtmlPresenter : FrameworkElement
    {
        private object _jsDiv;
        private string _htmlContent;

        internal override bool EnablePointerEventsCore
        {
            get { return true; }
        }

        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            _jsDiv = INTERNAL_HtmlDomManager.CreateDomElementAndAppendIt("div", parentRef, this);
            domElementWhereToPlaceChildren = _jsDiv;
            return _jsDiv;
        }

        protected internal override void INTERNAL_OnAttachedToVisualTree()
        {
            base.INTERNAL_OnAttachedToVisualTree();

            if (_htmlContent != null)
            {
                ApplyHtmlContent(_htmlContent);
            }
        }

        public string Html
        {
            get
            {
                return _htmlContent;
            }
            set
            {
                _htmlContent = value;

                if (this.IsLoaded)
                {
                    ApplyHtmlContent(_htmlContent);
                }
            }
        }

        public object DomElement
        {
            get
            {
                if (this.IsLoaded)
                {
                    if (!IsNullOrUndefined(_jsDiv))
                    {
                        if (Convert.ToBoolean(Interop.ExecuteJavaScript( "$0 && $0.hasChildNodes()", _jsDiv)))
                        {
                            return Interop.ExecuteJavaScriptAsync("$0.firstChild", _jsDiv);
                        }
                    }
                }
                return null;
            }
        }

        void ApplyHtmlContent(string htmlContent)
        {
            Interop.ExecuteJavaScriptAsync("$0.innerHTML = $1", _jsDiv, _htmlContent);
        }

        static bool IsNullOrUndefined(object jsObject)
        {
            if (Interop.IsRunningInTheSimulator)
            {
                if (jsObject == null)
                    return true;
#if CSHTML5NETSTANDARD 
                return false;
#else
                if (!(jsObject is JSValue))
                    return false;
                JSValue value = ((JSValue)jsObject);
                return value.IsNull() || value.IsUndefined();
#endif
            }
            else
                return Convert.ToBoolean(Interop.ExecuteJavaScript("(typeof $0 === 'undefined' || $0 === null)", jsObject));
        }
    }
}
