
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

#if MIGRATION
using System.Windows.Controls.Primitives;
using System.Windows.Media;
#else
using Windows.Foundation;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
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
        private const double LineDelta = 16.0;

        bool canHorizontallyScroll;
        bool canVerticallyScroll;
        Point cachedOffset;
        Size viewport;
        Size extents;

        /// <summary>
        /// Gets or sets the <see cref="ScrollViewer"/> element that controls scrolling
        /// behavior.
        /// </summary>
        /// <returns>
        /// The <see cref="ScrollViewer"/> element that controls scrolling behavior.
        /// </returns>
        public ScrollViewer ScrollOwner { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether scrolling on the horizontal axis
        /// is possible.
        /// </summary>
        /// <returns>
        /// true if scrolling is possible; otherwise, false.
        /// </returns>
        public bool CanHorizontallyScroll
        {
            get { return canHorizontallyScroll; }
            set
            {
                if (canHorizontallyScroll != value)
                {
                    canHorizontallyScroll = value;
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
            get { return canVerticallyScroll; }
            set
            {
                if (canVerticallyScroll != value)
                {
                    canVerticallyScroll = value;
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
        public double HorizontalOffset
        {
            get; private set;
        }

        /// <summary>
        /// Sets the distance the content has been scrolled horizontally.
        /// </summary>
        /// <param name="offset">
        /// The distance the content has been scrolled horizontally.
        /// </param>
        public void SetHorizontalOffset(double offset)
        {
            if (!CanHorizontallyScroll || cachedOffset.X == offset)
                return;

            cachedOffset.X = offset;
            InvalidateArrange();
        }

        /// <summary>
        /// Gets or sets the distance the content has been scrolled vertically.
        /// </summary>
        /// <returns>
        /// The distance the content has been scrolled vertically.
        /// </returns>
        public double VerticalOffset
        {
            get; private set;
        }

        /// <summary>
        /// Sets the distance the content has been scrolled vertically.
        /// </summary>
        /// <param name="offset">
        /// The distance the content has been scrolled vertically.
        /// </param>
        public void SetVerticalOffset(double offset)
        {
            if (!CanVerticallyScroll || cachedOffset.Y == offset)
                return;

            cachedOffset.Y = offset;
            InvalidateArrange();
        }

        /// <summary>
        /// Gets the horizontal size of the extent.
        /// </summary>
        /// <returns>
        /// The horizontal size of the extent.
        /// </returns>
        public double ExtentWidth
        {
            get { return extents.Width; }
        }

        /// <summary>
        /// Gets the vertical size of the extent.
        /// </summary>
        /// <returns>
        /// The vertical size of the extent.
        /// </returns>
        public double ExtentHeight
        {
            get { return extents.Height; }
        }

        /// <summary>
        /// Gets the horizontal size of the viewport.
        /// </summary>
        /// <returns>
        /// The horizontal size of the viewport.
        /// </returns>
        public double ViewportWidth
        {
            get { return viewport.Width; }
        }

        /// <summary>
        /// Gets the vertical size of the viewport.
        /// </summary>
        /// <returns>
        /// The vertical size of the viewport.
        /// </returns>
        public double ViewportHeight
        {
            get { return viewport.Height; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScrollContentPresenter"/>
        /// class.
        /// </summary>
        public ScrollContentPresenter()
        {
            this.ClipToBounds = true;
        }

        bool ClampOffsets()
        {
            bool changed = false;
            double result = CanHorizontallyScroll ? Math.Min(cachedOffset.X, ExtentWidth - ViewportWidth) : 0;
            result = Math.Max(0, result);
            if (result != HorizontalOffset)
            {
                HorizontalOffset = result;
                changed = true;
            }

            result = CanVerticallyScroll ? Math.Min(cachedOffset.Y, ExtentHeight - ViewportHeight) : 0;
            result = Math.Max(0, result);
            if (result != VerticalOffset)
            {
                VerticalOffset = result;
                changed = true;
            }
            return changed;
        }

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

            ScrollViewer sv = TemplatedParent as ScrollViewer;
            if (sv == null)
                return;

            IScrollInfo info = Content as IScrollInfo;
            if (info == null)
            {
                var presenter = Content as ItemsPresenter;
                if (presenter != null)
                {
                    presenter.ApplyTemplate();
                    if (presenter.TemplateChild != null)
                    {
                        info = presenter.TemplateChild as IScrollInfo;
                    }
                }
            }
            info = info ?? this;
            info.CanHorizontallyScroll = sv.HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled;
            info.CanVerticallyScroll = sv.VerticalScrollBarVisibility != ScrollBarVisibility.Disabled;
            info.ScrollOwner = sv;
            info.ScrollOwner.ScrollInfo = info;
            sv.InvalidateScrollInfo();
        }

        protected override Size MeasureOverride(Size constraint)
        {
            UIElement _contentRoot = this.Content as UIElement;
            if (null == ScrollOwner || _contentRoot == null)
                return base.MeasureOverride(constraint);

            Size ideal = new Size(
                CanHorizontallyScroll ? double.PositiveInfinity : constraint.Width,
                CanVerticallyScroll ? double.PositiveInfinity : constraint.Height
            );

            _contentRoot.Measure(ideal);
            UpdateExtents(constraint, _contentRoot.DesiredSize);

            return constraint.Min(extents);
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            UIElement _contentRoot = this.Content as UIElement;
            if (null == ScrollOwner || _contentRoot == null)
                return base.ArrangeOverride(arrangeSize);

            if (ClampOffsets())
                ScrollOwner.InvalidateScrollInfo();

            Size desired = _contentRoot.DesiredSize;
            Point start = new Point(
                -HorizontalOffset,
                -VerticalOffset
            );

            _contentRoot.Arrange(new Rect(start, desired.Max(arrangeSize)));
            UpdateExtents(arrangeSize, extents);
            return arrangeSize;
        }

        void UpdateExtents(Size viewport, Size extents)
        {
            bool changed = this.viewport != viewport || this.extents != extents;
            this.viewport = viewport;
            this.extents = extents;

            changed |= ClampOffsets();
            if (changed)
                ScrollOwner.InvalidateScrollInfo();
        }

        /// <summary>
        /// Scrolls the <see cref="ScrollContentPresenter"/> content downward by
        /// one line.
        /// </summary>
        public void LineDown()
        {
            SetVerticalOffset(VerticalOffset + LineDelta);
        }

        /// <summary>
        /// Scrolls the <see cref="ScrollContentPresenter"/> content to the left
        /// by a predetermined amount.
        /// </summary>
        public void LineLeft()
        {
            SetHorizontalOffset(HorizontalOffset - LineDelta);
        }

        /// <summary>
        /// Scrolls the <see cref="ScrollContentPresenter"/> content to the right
        /// by a predetermined amount.
        /// </summary>
        public void LineRight()
        {
            SetHorizontalOffset(HorizontalOffset + LineDelta);
        }

        /// <summary>
        /// Scrolls the <see cref="ScrollContentPresenter"/> content upward by
        /// one line.
        /// </summary>
        public void LineUp()
        {
            SetVerticalOffset(VerticalOffset - LineDelta);
        }

        // FIXME: how does one invoke MouseWheelUp/Down/etc? Need to figure out proper scrolling amounts
        /// <summary>
        /// Scrolls down within content after a user clicks the wheel button on a mouse.
        /// </summary>
        public void MouseWheelDown()
        {
            SetVerticalOffset(VerticalOffset + LineDelta);
        }

        /// <summary>
        /// Scrolls left within content after a user clicks the wheel button on a mouse.
        /// </summary>
        public void MouseWheelLeft()
        {
            SetHorizontalOffset(HorizontalOffset - LineDelta);
        }

        /// <summary>
        /// Scrolls right within content after a user clicks the wheel button on a mouse.
        /// </summary>
        public void MouseWheelRight()
        {
            SetHorizontalOffset(HorizontalOffset + LineDelta);
        }

        /// <summary>
        /// Scrolls up within content after a user clicks the wheel button on a mouse.
        /// </summary>
        public void MouseWheelUp()
        {
            SetVerticalOffset(VerticalOffset - LineDelta);
        }

        /// <summary>
        /// Scrolls down within the content by one page.
        /// </summary>
        public void PageDown()
        {
            SetVerticalOffset(VerticalOffset + ViewportHeight);
        }

        /// <summary>
        /// Scrolls left within the content by one page.
        /// </summary>
        public void PageLeft()
        {
            SetHorizontalOffset(HorizontalOffset - ViewportWidth);
        }

        /// <summary>
        /// Scrolls right within the content by one page.
        /// </summary>
        public void PageRight()
        {
            SetHorizontalOffset(HorizontalOffset + ViewportWidth);
        }

        /// <summary>
        /// Scrolls up within the content by one page.
        /// </summary>
        public void PageUp()
        {
            SetVerticalOffset(VerticalOffset - ViewportHeight);
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
    }
}
