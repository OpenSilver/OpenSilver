

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
                        string sDiv = INTERNAL_InteropImplementation.GetVariableStringForJS(_jsDiv);
                        if (OpenSilver.Interop.ExecuteJavaScriptBoolean($"{sDiv} && {sDiv}.hasChildNodes()"))
                        {
                            return OpenSilver.Interop.ExecuteJavaScriptAsync($"{sDiv}.firstChild");
                        }
                    }
                }
                return null;
            }
        }

        void ApplyHtmlContent(string htmlContent)
        {
            string sDiv = INTERNAL_InteropImplementation.GetVariableStringForJS(_jsDiv);
            string sContent = INTERNAL_InteropImplementation.GetVariableStringForJS(_htmlContent);
            OpenSilver.Interop.ExecuteJavaScriptFastAsync($"{sDiv}.innerHTML = {sContent}");
        }

        static bool IsNullOrUndefined(object jsObject) => jsObject == null;
    }
}
