using System;

#if MIGRATION
using System.Windows.Controls.Primitives;
using System.Windows.Media;
#else
using Windows.UI.Xaml.Controls.Primitives;
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
	public sealed partial class ScrollContentPresenter : ContentPresenter, IScrollInfo
	{
        static readonly double LineDelta = 16.0;

        bool canHorizontallyScroll;
        bool canVerticallyScroll;
        Point cachedOffset;
        Size viewport;
        Size extents;

        public ScrollViewer ScrollOwner { get; set; }

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

        public double HorizontalOffset
        {
            get; private set;
        }

        public void SetHorizontalOffset(double offset)
        {
            if (!CanHorizontallyScroll || cachedOffset.X == offset)
                return;

            cachedOffset.X = offset;
            InvalidateArrange();
        }

        public double VerticalOffset
        {
            get; private set;
        }

        public void SetVerticalOffset(double offset)
        {
            if (!CanVerticallyScroll || cachedOffset.Y == offset)
                return;

            cachedOffset.Y = offset;
            InvalidateArrange();
        }

        public double ExtentWidth
        {
            get { return extents.Width; }
        }

        public double ExtentHeight
        {
            get { return extents.Height; }
        }

        public double ViewportWidth
        {
            get { return viewport.Width; }
        }

        public double ViewportHeight
        {
            get { return viewport.Height; }
        }

        public ScrollContentPresenter()
        {
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

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ScrollViewer sv = null;
            for (DependencyObject parent = VisualTreeHelper.GetParent(this); parent != null; parent = VisualTreeHelper.GetParent(parent))
            {
                if (parent as ScrollViewer != null)
                {
                    sv = parent as ScrollViewer;
                    break;
                }
            }
            if (sv == null)
                return;

            IScrollInfo info = Content as IScrollInfo;
            if (info == null)
            {
                var presenter = Content as ItemsPresenter;
                if (presenter != null)
                {
                    if (presenter.ItemsHost == null)
                    {
                        presenter.ApplyTemplate();
                    }
                    info = presenter.ItemsHost as IScrollInfo;
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

        public void LineDown()
        {
            SetVerticalOffset(VerticalOffset + LineDelta);
        }

        public void LineLeft()
        {
            SetHorizontalOffset(HorizontalOffset - LineDelta);
        }

        public void LineRight()
        {
            SetHorizontalOffset(HorizontalOffset + LineDelta);
        }

        public void LineUp()
        {
            SetVerticalOffset(VerticalOffset - LineDelta);
        }

        // FIXME: how does one invoke MouseWheelUp/Down/etc? Need to figure out proper scrolling amounts
        public void MouseWheelDown()
        {
            SetVerticalOffset(VerticalOffset + LineDelta);
        }

        public void MouseWheelLeft()
        {
            SetHorizontalOffset(HorizontalOffset - LineDelta);
        }

        public void MouseWheelRight()
        {
            SetHorizontalOffset(HorizontalOffset + LineDelta);
        }

        public void MouseWheelUp()
        {
            SetVerticalOffset(VerticalOffset - LineDelta);
        }

        public void PageDown()
        {
            SetVerticalOffset(VerticalOffset + ViewportHeight);
        }

        public void PageLeft()
        {
            SetHorizontalOffset(HorizontalOffset - ViewportWidth);
        }

        public void PageRight()
        {
            SetHorizontalOffset(HorizontalOffset + ViewportWidth);
        }

        public void PageUp()
        {
            SetVerticalOffset(VerticalOffset - ViewportHeight);
        }

        public Rect MakeVisible(UIElement visual, Rect rectangle)
        {
            throw new NotImplementedException();
        }
    }
}
