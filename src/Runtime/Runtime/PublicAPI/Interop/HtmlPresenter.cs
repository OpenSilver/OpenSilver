
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
using OpenSilver;
using OpenSilver.Internal;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace CSHTML5.Native.Html.Controls
{
    [ContentProperty(nameof(Html))]
    public class HtmlPresenter : FrameworkElement, IResizeObserverListener
    {
        private HtmlElementReference _jsDiv;
        private IDisposable _resizeObserver;

        static HtmlPresenter()
        {
            FlowDirectionProperty.OverrideMetadata(
                typeof(HtmlPresenter),
                new FrameworkPropertyMetadata(
                    FlowDirection.LeftToRight,
                    FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsParentArrange)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) => ((HtmlPresenter)d).SetDirection((FlowDirection)newValue),
                });
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
                        var sContent = OpenSilver.Interop.GetVariableStringForJS((string)newValue ?? string.Empty);
                        OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                            $"osjs.htmlPresenter.setHtml('{htmlPresenter._jsDiv.Uid}', {sContent})");
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
            htmlPresenter.OuterDiv.SetCssStyleProperty(CssPropertyNames.Overflow, mode switch
            {
                ScrollMode.Enabled => "scroll",
                ScrollMode.Auto => "auto",
                _ => "hidden",
            });
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

        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object DomElement
        {
            get
            {
                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
                {
                    if (_jsDiv.IsConnected)
                    {
                        return OpenSilver.Interop.ExecuteJavaScriptAsync($"osjs.htmlPresenter.getDomElement('{_jsDiv.Uid}')");
                    }
                }

                return null;
            }
        }

        /// <inheritdoc />
        protected internal override HtmlElementReference CreateDomElement(HtmlElementReference parent)
        {
            (var outerDiv, _jsDiv) = INTERNAL_HtmlDomManager.CreateHtmlPresenterElementAndAppendIt(parent, this);
            return outerDiv;
        }

        protected internal override void INTERNAL_OnAttachedToVisualTree()
        {
            base.INTERNAL_OnAttachedToVisualTree();

            _resizeObserver = ResizeObserver.Observe(_jsDiv, this);

            SetScrollMode(this, ScrollMode);
        }

        protected internal override void INTERNAL_OnDetachedFromVisualTree()
        {
            base.INTERNAL_OnDetachedFromVisualTree();

            _resizeObserver?.Dispose();
            _resizeObserver = null;

            _jsDiv = default;
        }

        /// <inheritdoc />
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }

            if (ScrollMode != ScrollMode.Disabled)
            {
                string sArgs = OpenSilver.Interop.GetVariableStringForJS(e.UIEventArg);
                if (OpenSilver.Interop.ExecuteJavaScriptBoolean($"osjs.htmlPresenter.onWheelNative('{OuterDiv.Uid}', {sArgs})"))
                {
                    e.Handled = true;
                    e.Cancellable = false;
                }
            }

            if (!e.Handled)
            {
                base.OnMouseWheel(e);
            }
        }

        /// <inheritdoc />
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            string sArgs = OpenSilver.Interop.GetVariableStringForJS(e.UIEventArg);
            if (OpenSilver.Interop.ExecuteJavaScriptBoolean($"osjs.htmlPresenter.onKeyDownNative('{OuterDiv.Uid}', {sArgs})"))
            {
                e.Handled = true;
                e.Cancellable = false;
            }
        }

        /// <inheritdoc />
        protected override Size MeasureOverride(Size availableSize)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                Size size = MeasureNative(_jsDiv);
                return new Size(Math.Min(availableSize.Width, size.Width), Math.Min(availableSize.Height, size.Height));
            }

            return new Size();
        }

        private static Size MeasureNative(HtmlElementReference element)
        {
            if (element.IsConnected)
            {
                return Size.Parse(OpenSilver.Interop.ExecuteJavaScriptString(
                    $"osjs.htmlPresenter.measureNative('{element.Uid}')"));
            }

            return new Size();
        }

        /// <inheritdoc />
        protected override Size ArrangeOverride(Size finalSize) => finalSize;

        internal sealed override bool EnablePointerEventsCore => true;

        internal sealed override bool ShouldApplyMirrorTransform() =>
            GetFlowDirectionFromVisual(VisualTreeHelper.GetParent(this)) == FlowDirection.RightToLeft;

        void IResizeObserverListener.OnSizeChanged(Size size) => InvalidateMeasure();
    }
}
