
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
using System.Diagnostics;
using OpenSilver.Internal;

#if MIGRATION
using System.Windows.Controls.Primitives;
#else
using Windows.Foundation;
using Windows.UI.Xaml.Controls.Primitives;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Displays the content of a <see cref="ScrollViewer"/> control.
    /// </summary>
    public sealed class ScrollContentPresenter : ContentPresenter, IScrollInfo
    {
        private IScrollInfo _scrollInfo;
        private ScrollData _scrollData;

        /// <summary>
        /// Gets or sets the <see cref="ScrollViewer"/> element that controls scrolling
        /// behavior.
        /// </summary>
        /// <returns>
        /// The <see cref="ScrollViewer"/> element that controls scrolling behavior.
        /// </returns>
        public ScrollViewer ScrollOwner
        {
            get { return IsScrollClient ? _scrollData._scrollOwner : null; }
            set { if (IsScrollClient) _scrollData._scrollOwner = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether scrolling on the horizontal axis
        /// is possible.
        /// </summary>
        /// <returns>
        /// true if scrolling is possible; otherwise, false.
        /// </returns>
        public bool CanHorizontallyScroll
        {
            get { return IsScrollClient ? _scrollData._canHorizontallyScroll : false; }
            set
            {
                if (IsScrollClient && _scrollData._canHorizontallyScroll != value)
                {
                    _scrollData._canHorizontallyScroll = value;
                    InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether scrolling on the vertical axis is
        /// possible.
        /// </summary>
        /// <returns>
        /// true if scrolling is possible; otherwise, false.
        /// </returns>
        public bool CanVerticallyScroll
        {
            get { return IsScrollClient ? _scrollData._canVerticallyScroll : false; }
            set
            {
                if (IsScrollClient && _scrollData._canVerticallyScroll != value)
                {
                    _scrollData._canVerticallyScroll = value;
                    InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// Gets or sets the distance the content has been scrolled horizontally.
        /// </summary>
        /// <returns>
        /// The distance the content has been scrolled horizontally.
        /// </returns>
        public double HorizontalOffset => IsScrollClient ? _scrollData._computedOffset.X : 0.0;

        /// <summary>
        /// Sets the distance the content has been scrolled horizontally.
        /// </summary>
        /// <param name="offset">
        /// The distance the content has been scrolled horizontally.
        /// </param>
        public void SetHorizontalOffset(double offset)
        {
            if (!IsScrollClient) return;

            if (_scrollData._canHorizontallyScroll && !DoubleUtil.AreClose(_scrollData._offset.X, offset))
            {
                _scrollData._offset.X = offset;
                InvalidateArrange();
            }
        }

        /// <summary>
        /// Gets or sets the distance the content has been scrolled vertically.
        /// </summary>
        /// <returns>
        /// The distance the content has been scrolled vertically.
        /// </returns>
        public double VerticalOffset => IsScrollClient ? _scrollData._computedOffset.Y : 0.0;

        /// <summary>
        /// Sets the distance the content has been scrolled vertically.
        /// </summary>
        /// <param name="offset">
        /// The distance the content has been scrolled vertically.
        /// </param>
        public void SetVerticalOffset(double offset)
        {
            if (!IsScrollClient) return;

            if (_scrollData._canVerticallyScroll && !DoubleUtil.AreClose(_scrollData._offset.Y, offset))
            {
                _scrollData._offset.Y = offset;
                InvalidateArrange();
            }
        }

        /// <summary>
        /// Gets the horizontal size of the extent.
        /// </summary>
        /// <returns>
        /// The horizontal size of the extent.
        /// </returns>
        public double ExtentWidth => IsScrollClient ? _scrollData._extent.Width : 0.0;

        /// <summary>
        /// Gets the vertical size of the extent.
        /// </summary>
        /// <returns>
        /// The vertical size of the extent.
        /// </returns>
        public double ExtentHeight => IsScrollClient ? _scrollData._extent.Height : 0.0;

        /// <summary>
        /// Gets the horizontal size of the viewport.
        /// </summary>
        /// <returns>
        /// The horizontal size of the viewport.
        /// </returns>
        public double ViewportWidth => IsScrollClient ? _scrollData._viewport.Width : 0.0;

        /// <summary>
        /// Gets the vertical size of the viewport.
        /// </summary>
        /// <returns>
        /// The vertical size of the viewport.
        /// </returns>
        public double ViewportHeight => IsScrollClient ? _scrollData._viewport.Height : 0.0;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScrollContentPresenter"/> class.
        /// </summary>
        public ScrollContentPresenter() { }

        /// <summary>
        /// Builds the visual tree for the <see cref="ScrollContentPresenter"/>
        /// when a new template is applied.
        /// </summary>
#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

            if (TemplatedParent is ScrollViewer sv)
            {
                IScrollInfo info = Content as IScrollInfo;
                if (info is null)
                {
                    if (Content is ItemsPresenter presenter)
                    {
                        presenter.ApplyTemplate();
                        if (presenter.TemplateChild != null)
                        {
                            info = presenter.TemplateChild as IScrollInfo;
                        }
                    }
                }

                if (info is null)
                {
                    info = this;
                    EnsureScrollData();
                }

                // Detach any differing previous IScrollInfo from ScrollViewer
                if (info != _scrollInfo && _scrollInfo is not null)
                {
                    if (IsScrollClient) _scrollData = null;
                    else _scrollInfo.ScrollOwner = null;
                }

                _scrollInfo = info;
                info.ScrollOwner = sv;
                sv.ScrollInfo = info;
            }
            else if (_scrollInfo is not null)
            {
                if (_scrollInfo.ScrollOwner is not null)
                {
                    _scrollInfo.ScrollOwner.ScrollInfo = null;
                }

                _scrollInfo.ScrollOwner = null;
                _scrollInfo = null;
                _scrollData = null;
            }
        }

        private bool IsScrollClient => _scrollInfo == this;

        private ScrollData EnsureScrollData() => _scrollData ??= new ScrollData();

        protected override Size MeasureOverride(Size constraint)
        {
            Size desiredSize;

            if (!IsScrollClient)
            {
                desiredSize = base.MeasureOverride(constraint);
            }
            else
            {
                var childConstraint = new Size(
                    _scrollData._canHorizontallyScroll ? double.PositiveInfinity : constraint.Width,
                    _scrollData._canVerticallyScroll ? double.PositiveInfinity : constraint.Height);

                desiredSize = base.MeasureOverride(childConstraint);
            }

            // If we're handling scrolling (as the physical scrolling client, validate properties.
            if (IsScrollClient)
            {
                UpdateExtents(constraint, desiredSize);
            }

            desiredSize.Width = Math.Min(constraint.Width, desiredSize.Width);
            desiredSize.Height = Math.Min(constraint.Height, desiredSize.Height);

            return desiredSize;
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            // Verifies IScrollInfo properties & invalidates ScrollViewer if necessary.
            if (IsScrollClient)
            {
                UpdateExtents(arrangeSize, _scrollData._extent);
            }

            int count = VisualChildrenCount;

            if (count > 0)
            {
                if (GetVisualChild(0) is UIElement child)
                {
                    var childRect = new Rect(child.DesiredSize);

                    if (IsScrollClient)
                    {
                        childRect.X = -HorizontalOffset;
                        childRect.Y = -VerticalOffset;
                    }

                    //this is needed to stretch the child to arrange space,
                    childRect.Width = Math.Max(childRect.Width, arrangeSize.Width);
                    childRect.Height = Math.Max(childRect.Height, arrangeSize.Height);

                    child.Arrange(childRect);
                }
            }

            return arrangeSize;
        }

        internal override Rect? GetLayoutClip(Size layoutSlotSize)
        {
            return base.GetLayoutClip(layoutSlotSize) ?? new Rect(RenderSize);
        }

        private void UpdateExtents(Size viewport, Size extents)
        {
            Debug.Assert(IsScrollClient);

            bool changed = !DoubleUtil.AreClose(_scrollData._viewport, viewport) || !DoubleUtil.AreClose(_scrollData._extent, extents);
            _scrollData._viewport = viewport;
            _scrollData._extent = extents;

            changed |= ClampOffsets();

            if (changed)
            {
                ScrollOwner.InvalidateScrollInfo();
            }
        }

        private bool ClampOffsets()
        {
            bool changed = false;
            double result = CanHorizontallyScroll ? Math.Min(_scrollData._offset.X, ExtentWidth - ViewportWidth) : 0;
            result = Math.Max(0, result);
            if (!DoubleUtil.AreClose(result, _scrollData._computedOffset.X))
            {
                _scrollData._computedOffset.X = result;
                changed = true;
            }

            result = CanVerticallyScroll ? Math.Min(_scrollData._offset.Y, ExtentHeight - ViewportHeight) : 0;
            result = Math.Max(0, result);
            if (!DoubleUtil.AreClose(result, _scrollData._computedOffset.Y))
            {
                _scrollData._computedOffset.Y = result;
                changed = true;
            }
            return changed;
        }

        /// <summary>
        /// Scrolls the <see cref="ScrollContentPresenter"/> content downward by
        /// one line.
        /// </summary>
        public void LineDown()
        {
            if (IsScrollClient) SetVerticalOffset(VerticalOffset + ScrollViewer.LineDelta);
        }

        /// <summary>
        /// Scrolls the <see cref="ScrollContentPresenter"/> content to the left
        /// by a predetermined amount.
        /// </summary>
        public void LineLeft()
        {
            if (IsScrollClient) SetHorizontalOffset(HorizontalOffset - ScrollViewer.LineDelta);
        }

        /// <summary>
        /// Scrolls the <see cref="ScrollContentPresenter"/> content to the right
        /// by a predetermined amount.
        /// </summary>
        public void LineRight()
        {
            if (IsScrollClient) SetHorizontalOffset(HorizontalOffset + ScrollViewer.LineDelta);
        }

        /// <summary>
        /// Scrolls the <see cref="ScrollContentPresenter"/> content upward by
        /// one line.
        /// </summary>
        public void LineUp()
        {
            if (IsScrollClient) SetVerticalOffset(VerticalOffset - ScrollViewer.LineDelta);
        }

        // FIXME: how does one invoke MouseWheelUp/Down/etc? Need to figure out proper scrolling amounts
        /// <summary>
        /// Scrolls down within content after a user clicks the wheel button on a mouse.
        /// </summary>
        public void MouseWheelDown()
        {
            if (IsScrollClient) SetVerticalOffset(VerticalOffset + ScrollViewer.WheelDelta);
        }

        /// <summary>
        /// Scrolls left within content after a user clicks the wheel button on a mouse.
        /// </summary>
        public void MouseWheelLeft()
        {
            if (IsScrollClient) SetHorizontalOffset(HorizontalOffset - ScrollViewer.WheelDelta);
        }

        /// <summary>
        /// Scrolls right within content after a user clicks the wheel button on a mouse.
        /// </summary>
        public void MouseWheelRight()
        {
            if (IsScrollClient) SetHorizontalOffset(HorizontalOffset + ScrollViewer.WheelDelta);
        }

        /// <summary>
        /// Scrolls up within content after a user clicks the wheel button on a mouse.
        /// </summary>
        public void MouseWheelUp()
        {
            if (IsScrollClient) SetVerticalOffset(VerticalOffset - ScrollViewer.WheelDelta);
        }

        /// <summary>
        /// Scrolls down within the content by one page.
        /// </summary>
        public void PageDown()
        {
            if (IsScrollClient) SetVerticalOffset(VerticalOffset + ViewportHeight);
        }

        /// <summary>
        /// Scrolls left within the content by one page.
        /// </summary>
        public void PageLeft()
        {
            if (IsScrollClient) SetHorizontalOffset(HorizontalOffset - ViewportWidth);
        }

        /// <summary>
        /// Scrolls right within the content by one page.
        /// </summary>
        public void PageRight()
        {
            if (IsScrollClient) SetHorizontalOffset(HorizontalOffset + ViewportWidth);
        }

        /// <summary>
        /// Scrolls up within the content by one page.
        /// </summary>
        public void PageUp()
        {
            if (IsScrollClient) SetVerticalOffset(VerticalOffset - ViewportHeight);
        }

        /// <summary>
        /// Forces content to scroll until the coordinate space of a visual object is visible.
        /// </summary>
        /// <param name="visual">
        /// A <see cref="UIElement"/> that becomes visible.
        /// </param>
        /// <param name="rectangle">
        /// The bounding rectangle that identifies the coordinate space to make visible.
        /// </param>
        /// <returns>
        /// A <see cref="Rect"/> that represents the visible region.
        /// </returns>
        [OpenSilver.NotImplemented]
        public Rect MakeVisible(UIElement visual, Rect rectangle)
        {
            throw new NotImplementedException();
        }

        private sealed class ScrollData
        {
            internal ScrollViewer _scrollOwner;
            internal bool _canHorizontallyScroll;
            internal bool _canVerticallyScroll;
            internal Point _offset;
            internal Point _computedOffset;
            internal Size _viewport;
            internal Size _extent;
        }
    }
}
