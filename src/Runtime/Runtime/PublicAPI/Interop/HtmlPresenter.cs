
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

using System.Windows.Markup;
using CSHTML5.Internal;
using OpenSilver.Internal;

#if MIGRATION
using System.Windows;
#else
using Windows.Foundation;
using Windows.UI.Xaml;
#endif

namespace CSHTML5.Native.Html.Controls
{
    [ContentProperty(nameof(Html))]
    public class HtmlPresenter : FrameworkElement
    {
        private object _jsDiv;
        private string _htmlContent;
        private IResizeObserverAdapter _resizeObserver;

        internal sealed override bool EnablePointerEventsCore => true;

        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            object outerDiv = INTERNAL_HtmlDomManager.CreateDomElementAndAppendIt("div", parentRef, this);
            _jsDiv = domElementWhereToPlaceChildren = INTERNAL_HtmlDomManager.CreateDomElementAndAppendIt("div", outerDiv, this);
            return outerDiv;
        }

        protected internal override void INTERNAL_OnAttachedToVisualTree()
        {
            base.INTERNAL_OnAttachedToVisualTree();

            _resizeObserver = ResizeObserverFactory.Create();
            _resizeObserver.Observe(_jsDiv, OnHtmlContentResized);

            ApplyHtmlContent();
        }

        protected internal override void INTERNAL_OnDetachedFromVisualTree()
        {
            base.INTERNAL_OnDetachedFromVisualTree();

            if (_resizeObserver is not null)
            {
                _resizeObserver.Unobserve(_jsDiv);
                _resizeObserver = null;
            }

            _jsDiv = null;
        }

        protected override Size MeasureOverride(Size availableSize) => MeasureArrangeHelper();

        protected override Size ArrangeOverride(Size finalSize) => MeasureArrangeHelper();

        public string Html
        {
            get => _htmlContent;
            set
            {
                _htmlContent = value;
                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
                {
                    ApplyHtmlContent();
                }
            }
        }

        public object DomElement
        {
            get
            {
                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
                {
                    if (_jsDiv is not null)
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

        private Size MeasureArrangeHelper()
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                return INTERNAL_HtmlDomManager.GetBoundingClientSize(_jsDiv);
            }

            return new Size();
        }

        private void ApplyHtmlContent()
        {
            string sDiv = INTERNAL_InteropImplementation.GetVariableStringForJS(_jsDiv);
            string sContent = INTERNAL_InteropImplementation.GetVariableStringForJS(_htmlContent ?? string.Empty);
            OpenSilver.Interop.ExecuteJavaScriptVoid($"{sDiv}.innerHTML = {sContent}");
            InvalidateMeasure();
        }

        private void OnHtmlContentResized(Size size) => InvalidateMeasure();
    }
}
