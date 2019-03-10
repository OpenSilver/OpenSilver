
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
//
//===============================================================================


using CSHTML5.Internal;
using System;
using System.Windows.Markup;

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
                        if (Convert.ToBoolean(Interop.ExecuteJavaScript("$0.hasChildNodes()", _jsDiv)))
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
            return Convert.ToBoolean(Interop.ExecuteJavaScript("(typeof $0 === 'undefined' || $0 === null)", jsObject));
        }
    }
}
