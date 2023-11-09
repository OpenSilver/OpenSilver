
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

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using CSHTML5.Internal;
using OpenSilver.Internal;

namespace CSHTML5.Native.Html.Controls
{
    [ContentProperty(nameof(Html))]
    public class HtmlPresenter : FrameworkElement
    {
        private object _jsDiv;
        private ResizeObserverAdapter _resizeObserver;

        /// <summary>
        /// Identifies the <see cref="Html"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HtmlProperty =
            DependencyProperty.Register(
                nameof(Html),
                typeof(string),
                typeof(HtmlPresenter),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) =>
                    {
                        var htmlPresenter = (HtmlPresenter)d;
                        string sDiv = INTERNAL_InteropImplementation.GetVariableStringForJS(htmlPresenter._jsDiv);
                        string sContent = INTERNAL_InteropImplementation.GetVariableStringForJS((string)newValue ?? string.Empty);
                        OpenSilver.Interop.ExecuteJavaScriptVoid($"{sDiv}.shadowRoot.innerHTML = {sContent};");
                    },
                });
        
        /// <summary>
        /// Gets or sets the content of the <see cref="HtmlPresenter" />.
        /// </summary>
        /// <returns>
        /// The html content of the <see cref="HtmlPresenter" />.
        /// </returns>
        public string Html
        {
            get => (string)GetValue(HtmlProperty);
            set => SetValue(HtmlProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ScrollMode" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ScrollModeProperty =
            DependencyProperty.Register(
                nameof(ScrollMode),
                typeof(ScrollMode),
                typeof(HtmlPresenter),
                new FrameworkPropertyMetadata(ScrollMode.Auto)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) => SetScrollMode((HtmlPresenter)d, (ScrollMode)newValue),
                },
                IsValidScrollMode);

        private static void SetScrollMode(HtmlPresenter htmlPresenter, ScrollMode mode)
        {
            var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(htmlPresenter.INTERNAL_OuterDomElement);
            style.overflow = mode switch
            {
                ScrollMode.Enabled => "scroll",
                ScrollMode.Auto => "auto",
                _ => "hidden",
            };
        }

        /// <summary>
        /// Gets or sets a value that indicates how the <see cref="HtmlPresenter"/> interacts with
        /// its overflowing content.
        /// </summary>
        /// <returns>
        /// A <see cref="System.Windows.Controls.ScrollMode" /> value that indicates how the overflowing
        /// content is displayed. The default value is <see cref="ScrollMode.Auto" />.
        /// </returns>
        public ScrollMode ScrollMode
        {
            get => (ScrollMode)GetValue(ScrollModeProperty);
            set => SetValue(ScrollModeProperty, value);
        }

        private static bool IsValidScrollMode(object value)
        {
            ScrollMode mode = (ScrollMode)value;
            return mode == ScrollMode.Disabled || mode == ScrollMode.Enabled || mode == ScrollMode.Auto;
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
                        if (OpenSilver.Interop.ExecuteJavaScriptBoolean($"{sDiv} && {sDiv}.shadowRoot && {sDiv}.shadowRoot.hasChildNodes()"))
                        {
                            return OpenSilver.Interop.ExecuteJavaScriptAsync($"{sDiv}.shadowRoot.firstChild");
                        }
                    }
                }

                return null;
            }
        }

        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            (var outerDiv, _jsDiv) = INTERNAL_HtmlDomManager.CreateHtmlPresenterElementAndAppendIt(
                (INTERNAL_HtmlDomElementReference)parentRef, this);

            domElementWhereToPlaceChildren = _jsDiv;
            return outerDiv;
        }

        protected internal override void INTERNAL_OnAttachedToVisualTree()
        {
            base.INTERNAL_OnAttachedToVisualTree();

            _resizeObserver = new ResizeObserverAdapter();
            _resizeObserver.Observe(_jsDiv, OnHtmlContentResized);

            SetScrollMode(this, ScrollMode);
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

        /// <inheritdoc />
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            string sElement = INTERNAL_InteropImplementation.GetVariableStringForJS(INTERNAL_OuterDomElement);
            string sArgs = INTERNAL_InteropImplementation.GetVariableStringForJS(e.UIEventArg);
            if (OpenSilver.Interop.ExecuteJavaScriptBoolean($"document.htmlPresenterHelpers.onWheelNative({sElement}, {sArgs})"))
            {
                e.Handled = true;
                e.Cancellable = false;
            }
        }

        /// <inheritdoc />
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            string sElement = INTERNAL_InteropImplementation.GetVariableStringForJS(INTERNAL_OuterDomElement);
            string sArgs = INTERNAL_InteropImplementation.GetVariableStringForJS(e.UIEventArg);
            if (OpenSilver.Interop.ExecuteJavaScriptBoolean($"document.htmlPresenterHelpers.onKeyDownNative({sElement}, {sArgs})"))
            {
                e.Handled = true;
                e.Cancellable = false;
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                Size size = INTERNAL_HtmlDomManager.GetBoundingClientSize(_jsDiv);
                return new Size(Math.Min(availableSize.Width, size.Width), Math.Min(availableSize.Height, size.Height));
            }

            return new Size();
        }

        protected override Size ArrangeOverride(Size finalSize) => finalSize;

        internal sealed override bool EnablePointerEventsCore => true;

        internal sealed override void AddEventListeners() => InputManager.Current.AddEventListeners(this, true);

        private void OnHtmlContentResized(Size size) => InvalidateMeasure();
    }
}
