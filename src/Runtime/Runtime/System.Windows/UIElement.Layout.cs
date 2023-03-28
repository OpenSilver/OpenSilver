
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
using CSHTML5.Internal;
using OpenSilver.Internal;

#if MIGRATION
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Media;
#else
using Windows.Foundation;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    public partial class UIElement
    {
        /// <summary>
        /// Gets the final render size of a <see cref="UIElement"/>.
        /// </summary>
        /// <returns>
        /// The rendered size for this object. There is no default value.
        /// </returns>
        public Size RenderSize { get; internal set; }

        /// <summary>
        /// Gets the size that this <see cref="UIElement"/> computed during the measure
        /// pass of the layout process.
        /// </summary>
        /// <returns>
        /// The size that this <see cref="UIElement"/> computed during the measure pass
        /// of the layout process.
        /// </returns>
        public Size DesiredSize { get; private set; }

        /// <summary>
        /// Invalidates the measurement state (layout) for a <see cref="UIElement"/>.
        /// </summary>
        public void InvalidateMeasure()
        {
            if (!IsMeasureValid)
            {
                return;
            }

            IsMeasureValid = false;

            LayoutManager.Current.AddMeasure(this);
        }

        /// <summary>
        /// Invalidates the arrange state (layout) for a <see cref="UIElement"/>. After
        /// the invalidation, the <see cref="UIElement"/> will have its layout updated,
        /// which will occur asynchronously.
        /// </summary>
        public void InvalidateArrange()
        {
            if (!IsArrangeValid)
            {
                return;
            }

            IsArrangeValid = false;

            LayoutManager.Current.AddArrange(this);
        }

        /// <summary>
        /// Updates the <see cref="DesiredSize"/> of a <see cref="UIElement"/>.
        /// Typically, objects that implement custom layout for their layout children call
        /// this method from their own <see cref="FrameworkElement.MeasureOverride(Size)"/>
        /// implementations to form a recursive layout update.
        /// </summary>
        /// <param name="availableSize">
        /// The available space that a parent can allocate a child object. A child object
        /// can request a larger space than what is available; the provided size might be
        /// accommodated if scrolling or other resize behavior is possible in that particular
        /// container.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// availableSize contained a <see cref="double.NaN"/> value. See Remarks.
        /// </exception>
        /// <remarks>
        /// There is no reason to call <see cref="Measure"/> or <see cref="Arrange"/> outside 
        /// of the context of overriding methods that perform custom layout actions. Silverlight 
        /// layout works autonomously, based on detecting changes to the object tree and layout-relevant 
        /// properties at run time.
        /// The availableSize you pass to <see cref="Measure"/> cannot have a <see cref="double.NaN"/> 
        /// value for either the <see cref="Size.Height"/> or <see cref="Size.Width"/> of the <see cref="Size"/>.
        /// availableSize values can be any number from zero to infinite. Elements participating in 
        /// layout should return the minimum <see cref="Size"/> they require for a given availableSize.
        /// </remarks>
        public void Measure(Size availableSize)
        {
            if (INTERNAL_OuterDomElement == null)
            {
                LayoutManager.Current.RemoveMeasure(this);
                PreviousAvailableSize = availableSize;
                IsMeasureValid = true;
                return;
            }

            using (Dispatcher.DisableProcessing())
            {
                //enforce that Measure can not receive NaN size .
                if (double.IsNaN(availableSize.Width) || double.IsNaN(availableSize.Height))
                    throw new InvalidOperationException(
                        "UIElement.Measure(availableSize) cannot be called with NaN size.");

                bool previousMeasureValid = IsMeasureValid;
                Size savedPreviousAvailableSize = PreviousAvailableSize;
                PreviousAvailableSize = availableSize;

                LayoutManager.Current.RemoveMeasure(this);

                if (!IsVisible)
                {
                    DesiredSize = new Size();
                    _previousDesiredSize = Size.Empty;
                    IsMeasureValid = true;
                    return;
                }
                else if (previousMeasureValid && savedPreviousAvailableSize.IsClose(availableSize) && _previousDesiredSize != Size.Empty)
                {
                    IsMeasureValid = true;
                    return;
                }

                Size previousDesiredSizeInMeasure = this.DesiredSize;
                _measureInProgress = true;
                try
                {
                    DesiredSize = MeasureCore(availableSize);
                }
                finally
                {
                    _measureInProgress = false;
                }

                //enforce that MeasureCore can not return PositiveInfinity size even if given Infinte availabel size.
                //Note: NegativeInfinity can not be returned by definition of Size structure.
                if (double.IsPositiveInfinity(DesiredSize.Width) || double.IsPositiveInfinity(DesiredSize.Height))
                    throw new InvalidOperationException(string.Format(
                        "Layout measurement override of element '{0}' should not return PositiveInfinity as its DesiredSize, even if Infinity is passed in as available size.",
                        GetType().FullName));

                //enforce that MeasureCore can not return NaN size .
                if (double.IsNaN(DesiredSize.Width) || double.IsNaN(DesiredSize.Height))
                    throw new InvalidOperationException(string.Format(
                        "Layout measurement override of element '{0}' should not return NaN values as its DesiredSize.",
                        GetType().FullName));

                IsMeasureValid = true;

                if (previousDesiredSizeInMeasure != DesiredSize)
                {
                    InvalidateArrange();

                    UIElement parent = GetLayoutParent(this);
                    if (parent != null && !parent._measureInProgress)
                    {
                        parent.InvalidateMeasure();
                    }
                }

                PreviousAvailableSize = availableSize;
                _previousDesiredSize = DesiredSize;
            }
        }

        internal virtual Size MeasureCore(Size availableSize)
        {
            return new Size(0, 0);
        }

        /// <summary>
        /// Positions child objects and determines a size for a <see cref="UIElement"/>.
        /// Parent objects that implement custom layout for their child elements should call
        /// this method from their layout override implementations to form a recursive layout
        /// update.
        /// </summary>
        /// <param name="finalRect">
        /// The final size that the parent computes for the child in layout, provided as
        /// a <see cref="Rect"/> value.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// finalRect contained a <see cref="double.NaN"/> or infinite value. See Remarks.
        /// </exception>
        /// <remarks>
        /// There is no reason to call <see cref="Measure"/> or <see cref="Arrange"/> outside of 
        /// the context of overriding methods that perform custom layout actions. Silverlight 
        /// layout works autonomously, based on detecting changes to the object tree and 
        /// layout-relevant properties at run time. The finalRect you pass to <see cref="Measure"/> 
        /// cannot have a <see cref="double.NaN"/> value for any <see cref="Rect"/> value. Also, 
        /// finalRect cannot have any infinite values for any <see cref="Rect"/> value.
        /// Typically, calls to <see cref="Arrange"/> will incorporate a finalRect that uses the 
        /// height and width values from <see cref="DesiredSize"/> for each element . Exceptions 
        /// to this typical behavior might be necessary if an element holds a <see cref="DesiredSize"/> 
        /// that the layout parent cannot accommodate, or if the sum total of all child element 
        /// <see cref="DesiredSize"/> values cannot be accommodated or arranged. In such cases the 
        /// child element content might be clipped, resized, or placed in a scroll region, which all 
        /// depends on the specific functionality that is enabled in the layout parent container.
        /// </remarks>
        public void Arrange(Rect finalRect)
        {
            if (INTERNAL_OuterDomElement == null)
            {
                LayoutManager.Current.RemoveArrange(this);
                PreviousFinalRect = finalRect;
                IsArrangeValid = true;
                IsRendered = false;
                return;
            }

            using (Dispatcher.DisableProcessing())
            {
                //enforce that Arrange can not come with Infinity size or NaN
                if (double.IsPositiveInfinity(finalRect.Width)
                    || double.IsPositiveInfinity(finalRect.Height)
                    || double.IsNaN(finalRect.Width)
                    || double.IsNaN(finalRect.Height))
                {
                    DependencyObject parent = GetLayoutParent(this);
                    throw new InvalidOperationException(string.Format(
                        "Cannot call Arrange on a UIElement with infinite size or NaN. Parent of type '{0}' invokes the UIElement. Arrange called on element of type '{1}'.",
                        parent == null ? string.Empty : parent.GetType().FullName,
                        GetType().FullName));
                }

                bool previousArrangeValid = IsArrangeValid;
                Rect savedPreviousFinalRect = PreviousFinalRect;
                PreviousFinalRect = finalRect;
                LayoutManager.Current.RemoveArrange(this);

                if (!IsVisible)
                {
                    IsRendered = false;
                    IsArrangeValid = true;
                    return;
                }

                if (IsRendered && previousArrangeValid && finalRect.Location.IsClose(savedPreviousFinalRect.Location) && finalRect.Size.IsClose(savedPreviousFinalRect.Size))
                {
                    IsArrangeValid = true;
                    return;
                }

                if (!IsMeasureValid)
                {
                    Size previousDesiredSizeInArrange = DesiredSize;
                    Measure(PreviousAvailableSize);
                    if (previousDesiredSizeInArrange != DesiredSize)
                    {
                        InvalidateParentMeasure();
                        InvalidateParentArrange();
                    }
                }

                ArrangeCore(finalRect);

                _visualBounds = GetRenderedBounds(finalRect.Size);

                IsArrangeValid = true;
                PreviousFinalRect = finalRect;

                // Render with new size & location
                Render();

                LayoutManager.Current.AddUpdatedElement(this);
            }
        }

        internal virtual void ArrangeCore(Rect finalRect) { }

        internal virtual Rect GetRenderedBounds(Size layoutSlotSize)
        {
            return new Rect(VisualOffset, RenderSize);
        }

        /// <summary>
        /// Ensures that all positions of child objects of a <see cref="UIElement"/> are
        /// properly updated for layout.
        /// </summary>
        public void UpdateLayout()
        {
            LayoutManager.Current.UpdateLayout();
        }

        internal void UpdateCustomLayout(Size newSize)
        {
            _layoutLastSize = newSize;
            if (_layoutProcessing)
                return;

            _layoutProcessing = true;
            Dispatcher.BeginInvoke(BeginUpdateCustomLayout);
        }

        private void BeginUpdateCustomLayout()
        {
            Size savedLastSize = _layoutLastSize;
            Size availableSize = _layoutLastSize;
            FrameworkElement fe = this as FrameworkElement;
            if (fe != null)
            {
                if (fe.IsAutoWidthOnCustomLayoutInternal)
                    availableSize.Width = double.PositiveInfinity;
                if (fe.IsAutoHeightOnCustomLayoutInternal)
                    availableSize.Height = double.PositiveInfinity;
            }
            if (_layoutMeasuredSize == availableSize)
            {
                _layoutProcessing = false;
                return;
            }

            Measure(availableSize);
            _layoutMeasuredSize = availableSize;

            if (savedLastSize != _layoutLastSize)
            {
                BeginUpdateCustomLayout();
                return;
            }
            if (fe != null)
            {
                if (fe.IsAutoWidthOnCustomLayoutInternal)
                    availableSize.Width = this.DesiredSize.Width;

                if (fe.IsAutoHeightOnCustomLayoutInternal)
                    availableSize.Height = this.DesiredSize.Height;
            }

            Arrange(new Rect(availableSize));
            if (savedLastSize != _layoutLastSize)
            {
                BeginUpdateCustomLayout();
                return;
            }

            _layoutProcessing = false;
        }

        internal void InvalidateParentMeasure() => GetLayoutParent(this)?.InvalidateMeasure();

        internal void InvalidateParentArrange() => GetLayoutParent(this)?.InvalidateArrange();

        internal static UIElement GetLayoutParent(UIElement element)
            => VisualTreeHelper.GetParent(element) switch
            {
                GridNotLogical gnl => GetLayoutParent(gnl),
                UIElement uie => uie,
                null when element is FrameworkElement fe && fe.Parent is Popup popup => popup.PopupRoot?.Content,
                _ => null,
            };

        internal void ClearMeasureAndArrangeValidation()
        {
            if (!IsCustomLayoutRoot)
            {
                IsArrangeValid = false;
                IsMeasureValid = false;
            }
            IsRendered = false;
            RenderedVisualBounds = Rect.Empty;
            _previousDesiredSize = Size.Empty;
        }

        private void Render()
        {
            if (IsCustomLayoutRoot)
            {
                IsRendered = true;
                if (RenderedVisualBounds != _visualBounds)
                {
                    Size renderedSize = _visualBounds.Size;
                    FrameworkElement fe = this as FrameworkElement;

                    if (RenderedVisualBounds.Width != renderedSize.Width && fe.IsAutoWidthOnCustomLayoutInternal)
                    {
                        INTERNAL_HtmlDomManager.GetDomElementStyleForModification(INTERNAL_OuterDomElement).width =
                            $"{renderedSize.Width.ToInvariantString()}px";
                    }

                    if (RenderedVisualBounds.Height != renderedSize.Height && fe.IsAutoHeightOnCustomLayoutInternal)
                    {
                        INTERNAL_HtmlDomManager.GetDomElementStyleForModification(INTERNAL_OuterDomElement).height =
                            $"{renderedSize.Height.ToInvariantString()}px";
                    }

                    RenderedVisualBounds = _visualBounds;
                }
                return;
            }

            if (this is not Window && this is not PopupRoot)
            {
                var uiStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(INTERNAL_OuterDomElement);
                if (RenderedVisualBounds != _visualBounds)
                {
                    RenderedVisualBounds = _visualBounds;

                    if (!IsRendered)
                    {
                        INTERNAL_HtmlDomManager.SetVisualBounds(uiStyle, _visualBounds, true, false, false);
                        IsRendered = true;
                    }
                    else
                    {
                        INTERNAL_HtmlDomManager.SetVisualBounds(uiStyle, _visualBounds, false, false, false);
                    }
                }
            }
        }

        internal Point VisualOffset { get; set; }

        internal Rect RenderedVisualBounds { get; private set; }

        internal Rect PreviousFinalRect { get; private set; }

        internal Size PreviousAvailableSize { get; private set; }

        internal bool IsMeasureValid { get; private set; }

        internal bool IsArrangeValid { get; private set; }

        internal bool IsRendered { get; private set; }

        internal bool IsFirstRendering { get; set; }

        internal bool KeepHiddenInFirstRender { get; set; }

        internal int VisualLevel
        {
            get
            {
                if (_visualLevel == -1)
                {
                    _visualLevel = (VisualTreeHelper.GetParent(this) as UIElement)?.VisualLevel + 1 ?? 0;
                }

                return _visualLevel;
            }
        }

        internal bool UseCustomLayout { get; set; }

        internal bool IsCustomLayoutRoot => UseCustomLayout && !IsUnderCustomLayout;

        internal bool IsUnderCustomLayout => GetLayoutParent(this)?.UseCustomLayout ?? false;

        private Rect _visualBounds;
        private Size _previousDesiredSize;
        private Size _layoutMeasuredSize;
        private Size _layoutLastSize;
        private bool _layoutProcessing;
        private bool _measureInProgress;
        private int _visualLevel;
    }
}
