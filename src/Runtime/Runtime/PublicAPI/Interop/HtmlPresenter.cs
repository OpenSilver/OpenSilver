
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
using System.ComponentModel;
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
        private INTERNAL_HtmlDomElementReference _jsDiv;
        private ResizeObserverAdapter _resizeObserver;

        static HtmlPresenter()
        {
            IsHitTestableProperty.OverrideMetadata(typeof(HtmlPresenter), new PropertyMetadata(BooleanBoxes.TrueBox));
        }

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
                        string sDiv = OpenSilver.Interop.GetVariableStringForJS(htmlPresenter._jsDiv);
                        string sContent = OpenSilver.Interop.GetVariableStringForJS((string)newValue ?? string.Empty);
                        if (htmlPresenter.IsUsingShadowDOM)
                        {
                            OpenSilver.Interop.ExecuteJavaScriptVoidAsync($"{sDiv}.shadowRoot.innerHTML = {sContent}");
                        }
                        else
                        {
                            OpenSilver.Interop.ExecuteJavaScriptVoidAsync($"{sDiv}.innerHTML = {sContent}");
                        }
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
            set => SetValueInternal(HtmlProperty, value);
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
            htmlPresenter.OuterDiv.Style.overflow = mode switch
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
            set => SetValueInternal(ScrollModeProperty, value);
        }

        private static bool IsValidScrollMode(object value)
        {
            ScrollMode mode = (ScrollMode)value;
            return mode == ScrollMode.Disabled || mode == ScrollMode.Enabled || mode == ScrollMode.Auto;
        }

        /// <summary>
        /// Identifies the <see cref="UseShadowDom" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty UseShadowDomProperty =
            DependencyProperty.Register(
                nameof(UseShadowDom),
                typeof(bool),
                typeof(HtmlPresenter),
                new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Get or set a value that indicates if the <see cref="HtmlPresenter"/> should create 
        /// a shadow DOM to isolate its content from the rest of the DOM.
        /// </summary>
        /// <returns>
        /// true to create a shadow DOM, false otherwise.
        /// </returns>
        public bool UseShadowDom
        {
            get => (bool)GetValue(UseShadowDomProperty);
            set => SetValue(UseShadowDomProperty, value);
        }

        internal bool IsUsingShadowDOM { get; private set; }

        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object DomElement
        {
            get
            {
                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
                {
                    if (_jsDiv is not null)
                    {
                        string sDiv = OpenSilver.Interop.GetVariableStringForJS(_jsDiv);
                        if (IsUsingShadowDOM)
                        {
                            if (OpenSilver.Interop.ExecuteJavaScriptBoolean($"{sDiv} && {sDiv}.shadowRoot && {sDiv}.shadowRoot.hasChildNodes()"))
                            {
                                return OpenSilver.Interop.ExecuteJavaScriptAsync($"{sDiv}.shadowRoot.firstChild");
                            }
                        }
                        else
                        {
                            if (OpenSilver.Interop.ExecuteJavaScriptBoolean($"{sDiv} && {sDiv}.hasChildNodes()"))
                            {
                                return OpenSilver.Interop.ExecuteJavaScriptAsync($"{sDiv}.firstChild");
                            }
                        }
                    }
                }

                return null;
            }
        }

        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            domElementWhereToPlaceChildren = null;
            IsUsingShadowDOM = UseShadowDom;
            (var outerDiv, _jsDiv) = INTERNAL_HtmlDomManager.CreateHtmlPresenterElementAndAppendIt(
                (INTERNAL_HtmlDomElementReference)parentRef, this);

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
            IsUsingShadowDOM = false;
        }

        /// <inheritdoc />
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            string sElement = OpenSilver.Interop.GetVariableStringForJS(OuterDiv);
            string sArgs = OpenSilver.Interop.GetVariableStringForJS(e.UIEventArg);
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

            string sElement = OpenSilver.Interop.GetVariableStringForJS(OuterDiv);
            string sArgs = OpenSilver.Interop.GetVariableStringForJS(e.UIEventArg);
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

        private void OnHtmlContentResized(Size size) => InvalidateMeasure();
    }
}
