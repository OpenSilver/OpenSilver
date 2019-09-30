
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



using CSHTML5.Internal;
using System;
using System.Windows.Markup;

#if MIGRATION
using System.Windows;
using DotNetBrowser;
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

#if REVAMPPOINTEREVENTS
        internal override bool INTERNAL_ManageFrameworkElementPointerEventsAvailability()
        {
            return true;
        }
#endif

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
            if (Interop.IsRunningInTheSimulator)
            {
                if (jsObject == null)
                    return true;
                if (!(jsObject is JSValue))
                    return false;
                JSValue value = ((JSValue)jsObject);
                return value.IsNull() || value.IsUndefined();
            }
            else
                return Convert.ToBoolean(Interop.ExecuteJavaScript("(typeof $0 === 'undefined' || $0 === null)", jsObject));
        }
    }
}
