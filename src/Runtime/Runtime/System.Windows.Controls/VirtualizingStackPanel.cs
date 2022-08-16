
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
using System.Collections.Specialized;
using CSHTML5.Internals.Controls;

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
    /// Arranges and virtualizes content on a single line that is oriented either horizontally
    /// or vertically.
    /// </summary>
    public partial class VirtualizingStackPanel : VirtualizingPanel, IScrollInfo
    {
        private const double LineDelta = 14.7;
        private const double Wheelitude = 3;

        /// <summary>
        /// The current virtualization mode of the <see cref="VirtualizingStackPanel"/>
        /// (whether it is <see cref="VirtualizationMode.Recycling"/> or not).
        /// </summary>
        public static readonly DependencyProperty VirtualizationModeProperty =
            DependencyProperty.RegisterAttached(
                "VirtualizationMode", 
                typeof(VirtualizationMode), 
                typeof(VirtualizingStackPanel), 
                new PropertyMetadata(VirtualizationMode.Recycling));

        /// <summary>
        /// A value that indicates whether the <see cref="VirtualizingStackPanel"/>
        /// is using virtualization.
        /// </summary>
        public static readonly DependencyProperty IsVirtualizingProperty =
            DependencyProperty.RegisterAttached(
                "IsVirtualizing", 
                typeof(bool), 
                typeof(VirtualizingStackPanel), 
                new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="Orientation"/> dependency
        /// property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(
                nameof(Orientation), 
                typeof(Orientation), 
                typeof(VirtualizingStackPanel), 
                new PropertyMetadata(Orientation.Vertical, OrientationChanged));

        /// <summary>
        /// Gets or sets a value that describes the horizontal or vertical orientation of
        /// stacked content.
        /// The default is <see cref="Orientation.Vertical"/>.
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)this.GetValue(OrientationProperty); }
            set { this.SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualizingStackPanel"/>
        /// class.
        /// </summary>
        public VirtualizingStackPanel()
        {
        }

        /// <summary>
        /// Gets a value that determines whether the <see cref="VirtualizingStackPanel"/>
        /// is virtualizing its content.
        /// </summary>
        /// <param name="o">
        /// The object being virtualized.
        /// </param>
        /// <returns>
        /// true if the <see cref="VirtualizingStackPanel"/> is virtualizing its
        /// content; otherwise false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// o is null.
        /// </exception>
        public static bool GetIsVirtualizing(DependencyObject o)
        {
            if (o == null)
                throw new ArgumentNullException("o");

            return (bool)o.GetValue(IsVirtualizingProperty);
        }

        internal static void SetIsVirtualizing(DependencyObject o, bool value)
        {
            if (o == null)
                throw new ArgumentNullException("o");

            o.SetValue(VirtualizingStackPanel.IsVirtualizingProperty, value);
        }

        /// <summary>
        /// Returns the <see cref="VirtualizationMode"/> for the specified object.
        /// </summary>
        /// <param name="element">
        /// The object from which the <see cref="VirtualizationMode"/> is read.
        /// </param>
        /// <returns>
        /// One of the enumeration values that specifies whether the object uses container
        /// recycling.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// element is null.
        /// </exception>
        public static VirtualizationMode GetVirtualizationMode(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            return (VirtualizationMode)element.GetValue(VirtualizationModeProperty);
        }

        /// <summary>
        /// Sets the <see cref="VirtualizationMode"/> on the specified object.
        /// </summary>
        /// <param name="element">
        /// The element on which to set the <see cref="VirtualizationMode"/>.
        /// </param>
        /// <param name="value">
        /// One of the enumeration values that specifies whether element uses container recycling.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// element is null.
        /// </exception>
        public static void SetVirtualizationMode(DependencyObject element, VirtualizationMode value)
        {
            element.SetValue(VirtualizationModeProperty, value);
        }

        static void OrientationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((VirtualizingStackPanel)sender).InvalidateMeasure();
        }

        bool canHorizontallyScroll;
        bool canVerticallyScroll;

        //
        // Helper Methods
        //
        void RemoveUnusedContainers(int first, int count)
        {
            IRecyclingItemContainerGenerator generator = ItemContainerGenerator as IRecyclingItemContainerGenerator;
            ItemsControl owner = ItemsControl.GetItemsOwner(this);
            VirtualizationMode mode = GetVirtualizationMode(this);
            int last = first + count - 1;
            GeneratorPosition pos;
            int item;

            //Console.WriteLine ("VSP.RemoveUnusedContainers ({0}, {1});", first, count);
            pos = new GeneratorPosition(Children.Count - 1, 0);
            while (pos.Index >= 0)
            {
                item = generator.IndexFromGeneratorPosition(pos);

                // Note: we should not remove the child if it is an item of its owner ItemsControl be
                // we remove it anyway because ScrollContentPresenter does not properly work with an
                // external IScrollInfo which leads to layout issues
                if ((item < first || item > last) &&
                    /*!((IGeneratorHost)owner).IsItemItsOwnContainer(owner.Items[item]) &&*/ 
                    NotifyCleanupItem(Children[pos.Index], owner))
                {
                    RemoveInternalChildRange(pos.Index, 1);

                    if (mode == VirtualizationMode.Recycling &&
                        !((IGeneratorHost)owner).IsItemItsOwnContainer(owner.Items[item]))
                        generator.Recycle(pos, 1);
                    else
                        generator.Remove(pos, 1);
                }

                pos.Index--;
            }
        }

        private bool NotifyCleanupItem(UIElement child, ItemsControl itemsControl)
        {
            CleanUpVirtualizedItemEventArgs e = new CleanUpVirtualizedItemEventArgs(child, itemsControl.ItemContainerGenerator.ItemFromContainer(child));
            e.OriginalSource = this;
            OnCleanUpVirtualizedItem(e);

            return !e.Cancel;
        }

        /// <summary>
        /// Measures the child elements of a <see cref="VirtualizingStackPanel"/>
        /// in anticipation of arranging them during the <see cref="ArrangeOverride(Size)"/>
        /// pass.
        /// </summary>
        /// <param name="constraint">
        /// An upper limit <see cref="Size"/> that should not be exceeded.
        /// </param>
        /// <returns>
        /// The <see cref="Size"/> that represents the desired size of the element.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            ItemsControl owner = ItemsControl.GetItemsOwner(this);
            Size measured = new Size(0, 0);
            bool invalidate = false;
            int nvisible = 0;
            int beyond = 0;
            int index;

            if (Orientation == Orientation.Horizontal)
                index = (int)HorizontalOffset;
            else
                index = (int)VerticalOffset;

            // Ensure we always touch ItemContainerGenerator as by accessing this property
            // we hook up to some events on it.
            IItemContainerGenerator generator = ItemContainerGenerator;
            if (owner.Items.Count > 0)
            {
                GeneratorPosition start;
                int insertAt;

                Size childAvailable = constraint;
                if (CanHorizontallyScroll || Orientation == Orientation.Horizontal)
                    childAvailable.Width = double.PositiveInfinity;
                if (CanVerticallyScroll || Orientation == Orientation.Vertical)
                    childAvailable.Height = double.PositiveInfinity;

                // Next, prepare and measure the extents of our viewable items...
                start = generator.GeneratorPositionFromIndex(index);
                insertAt = (start.Offset == 0) ? start.Index : start.Index + 1;

                using (generator.StartAt(start, GeneratorDirection.Forward, true))
                {
                    bool isNewlyRealized;

                    for (int i = index; i < owner.Items.Count && beyond < 2; i++, insertAt++)
                    {
                        // Generate the child container
                        UIElement child = (UIElement)generator.GenerateNext(out isNewlyRealized);
                        if (isNewlyRealized || insertAt >= Children.Count || Children[insertAt] != child)
                        {
                            // Add newly created children to the panel
                            if (insertAt < Children.Count)
                            {
                                InsertInternalChild(insertAt, child);
                            }
                            else
                            {
                                AddInternalChild(child);
                            }

                            generator.PrepareItemContainer(child);
                        }

                        // Call Measure() on the child to both force layout and also so
                        // that we can figure out when to stop adding children (e.g. when
                        // we go beyond the viewable area)
                        child.Measure(childAvailable);
                        Size size = child.DesiredSize;
                        nvisible++;

                        if (Orientation == Orientation.Vertical)
                        {
                            measured.Width = Math.Max(measured.Width, size.Width);
                            measured.Height += size.Height;

                            if (measured.Height > constraint.Height)
                                beyond++;
                        }
                        else
                        {
                            measured.Height = Math.Max(measured.Height, size.Height);
                            measured.Width += size.Width;

                            if (measured.Width > constraint.Width)
                                beyond++;
                        }
                    }
                }
            }

            VirtualizingStackPanel.SetIsVirtualizing(owner, true);
            // FIXME: this if-check is a workaround for a bug
            // exposed by NBC Olympics but should not normally be
            // here.
            if (nvisible > 0)
                RemoveUnusedContainers(index, nvisible);

            nvisible -= beyond;

            // Update our Extent and Viewport values
            if (Orientation == Orientation.Vertical)
            {
                if (ExtentHeight != owner.Items.Count)
                {
                    ExtentHeight = owner.Items.Count;
                    invalidate = true;
                }

                if (ExtentWidth != measured.Width)
                {
                    ExtentWidth = measured.Width;
                    invalidate = true;
                }

                if (ViewportHeight != nvisible)
                {
                    ViewportHeight = nvisible;
                    invalidate = true;
                }

                if (ViewportWidth != constraint.Width)
                {
                    ViewportWidth = constraint.Width;
                    invalidate = true;
                }
            }
            else
            {
                if (ExtentHeight != measured.Height)
                {
                    ExtentHeight = measured.Height;
                    invalidate = true;
                }

                if (ExtentWidth != owner.Items.Count)
                {
                    ExtentWidth = owner.Items.Count;
                    invalidate = true;
                }

                if (ViewportHeight != constraint.Height)
                {
                    ViewportHeight = constraint.Height;
                    invalidate = true;
                }

                if (ViewportWidth != nvisible)
                {
                    ViewportWidth = nvisible;
                    invalidate = true;
                }
            }

            if (invalidate && ScrollOwner != null)
                ScrollOwner.InvalidateScrollInfo();

            return measured;
        }

        /// <summary>
        /// Arranges the content of a <see cref="VirtualizingStackPanel"/> element.
        /// </summary>
        /// <param name="arrangeSize">
        /// The <see cref="Size"/> that this element should use to arrange its child elements.
        /// </param>
        /// <returns>
        /// The <see cref="Size"/> that represents the arranged size of this <see cref="VirtualizingStackPanel"/>
        /// element and its child elements.
        /// </returns>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            Size arranged = arrangeSize;

            if (Orientation == Orientation.Vertical)
                arranged.Height = 0;
            else
                arranged.Width = 0;

            // Arrange our children
            foreach (UIElement child in Children)
            {
                Size size = child.DesiredSize;
                if (Orientation == Orientation.Vertical)
                {
                    size.Width = arrangeSize.Width;
                    Rect childFinal = new Rect(-HorizontalOffset, arranged.Height, size.Width, size.Height);

                    if (childFinal.IsEmpty)
                        child.Arrange(new Rect());
                    else
                        child.Arrange(childFinal);

                    arranged.Width = Math.Max(arranged.Width, size.Width);
                    arranged.Height += size.Height;
                }
                else
                {
                    size.Height = arrangeSize.Height;
                    Rect childFinal = new Rect(arranged.Width, -VerticalOffset, size.Width, size.Height);

                    if (childFinal.IsEmpty)
                        child.Arrange(new Rect());
                    else
                        child.Arrange(childFinal);

                    arranged.Width += size.Width;
                    arranged.Height = Math.Max(arranged.Height, size.Height);
                }
            }

            if (Orientation == Orientation.Vertical)
                arranged.Height = Math.Max(arranged.Height, arrangeSize.Height);
            else
                arranged.Width = Math.Max(arranged.Width, arrangeSize.Width);

            return arranged;
        }

        /// <summary>
        /// Called when the collection of child elements is cleared by the base <see cref="Panel"/>
        /// class.
        /// </summary>
        protected override void OnClearChildren()
        {
            base.OnClearChildren();

            HorizontalOffset = 0;
            VerticalOffset = 0;

            InvalidateMeasure();

            if (ScrollOwner != null)
                ScrollOwner.InvalidateScrollInfo();
        }

        /// <summary>
        /// Called when the <see cref="ItemsControl.Items"/> collection that is
        /// associated with the <see cref="ItemsControl"/> for this <see cref="Panel"/>
        /// changes.
        /// </summary>
        /// <param name="sender">
        /// The <see cref="object"/> that raised the event.
        /// </param>
        /// <param name="args">
        /// Provides data for the <see cref="ItemContainerGenerator.ItemsChanged"/>
        /// event.
        /// </param>
        protected override void OnItemsChanged(object sender, ItemsChangedEventArgs args)
        {
            base.OnItemsChanged(sender, args);
            IItemContainerGenerator generator = ItemContainerGenerator;
            ItemsControl owner = ItemsControl.GetItemsOwner(this);
            int index, offset, viewable;

            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    // The following logic is meant to keep the current viewable items in view
                    // after adjusting for added items.
                    index = generator.IndexFromGeneratorPosition(args.Position);
                    if (Orientation == Orientation.Horizontal)
                        offset = (int)HorizontalOffset;
                    else
                        offset = (int)VerticalOffset;

                    if (index <= offset)
                    {
                        // items have been added earlier in the list than what is viewable
                        offset += args.ItemCount;
                    }

                    if (Orientation == Orientation.Horizontal)
                        SetHorizontalOffset(offset);
                    else
                        SetVerticalOffset(offset);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    // The following logic is meant to keep the current viewable items in view
                    // after adjusting for removed items.
                    index = generator.IndexFromGeneratorPosition(args.Position);
                    if (Orientation == Orientation.Horizontal)
                    {
                        offset = (int)HorizontalOffset;
                        viewable = (int)ViewportWidth;
                    }
                    else
                    {
                        viewable = (int)ViewportHeight;
                        offset = (int)VerticalOffset;
                    }

                    if (index < offset)
                    {
                        // items earlier in the list than what is viewable have been removed
                        offset = Math.Max(offset - args.ItemCount, 0);
                    }

                    // adjust for items removed in the current view and/or beyond the current view
                    offset = Math.Min(offset, owner.Items.Count - viewable);
                    offset = Math.Max(offset, 0);

                    if (Orientation == Orientation.Horizontal)
                        SetHorizontalOffset(offset);
                    else
                        SetVerticalOffset(offset);

                    RemoveInternalChildRange(args.Position.Index, args.ItemUICount);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    RemoveInternalChildRange(args.Position.Index, args.ItemUICount);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    // DO NOTHING
                    break;
            }

            InvalidateMeasure();

            if (ScrollOwner != null)
                ScrollOwner.InvalidateScrollInfo();
        }

        /// <summary>
        /// Called when an item that is hosted by the <see cref="VirtualizingStackPanel"/>
        /// is re-virtualized.
        /// </summary>
        /// <param name="e">
        /// Data about the event.
        /// </param>
        protected virtual void OnCleanUpVirtualizedItem(CleanUpVirtualizedItemEventArgs e)
        {
            CleanUpVirtualizedItemEventHandler handler = CleanUpVirtualizedItemEvent;

            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Generates the item at the specified index and calls BringIntoView on it.
        /// </summary>
        /// <param name="index">Specify the item index that should become visible. This is the index into ItemsControl.Items collection</param>
        protected override void BringIndexIntoView(int index)
        {
            if (Orientation == Orientation.Horizontal)
                SetHorizontalOffset(index);
            else
                SetVerticalOffset(index);
        }

        /// <summary>
        /// Occurs when an item that is hosted by the <see cref="VirtualizingStackPanel"/>
        /// is re-virtualized.
        /// </summary>
        public event CleanUpVirtualizedItemEventHandler CleanUpVirtualizedItemEvent;

        #region "IScrollInfo"

        /// <summary>
        /// Gets or sets a value that indicates whether a <see cref="VirtualizingStackPanel"/>
        /// can scroll in the horizontal dimension.
        /// </summary>
        /// <returns>
        /// true if content can scroll in the horizontal dimension; otherwise, false. The
        /// default is false.
        /// </returns>
        public bool CanHorizontallyScroll
        {
            get { return canHorizontallyScroll; }
            set
            {
                canHorizontallyScroll = value;
                InvalidateMeasure();
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether content can scroll in the vertical
        /// dimension.
        /// </summary>
        /// <returns>
        /// true if content can scroll in the vertical dimension; otherwise, false. The default
        /// is false.
        /// </returns>
        public bool CanVerticallyScroll
        {
            get { return canVerticallyScroll; }
            set
            {
                canVerticallyScroll = value;
                InvalidateMeasure();
            }
        }

        /// <summary>
        /// Gets a value that contains the horizontal size of the extent.
        /// </summary>
        /// <returns>
        /// A <see cref="double"/> that represents the horizontal size of the extent, in pixels.
        /// The default is 0.
        /// </returns>
        public double ExtentWidth
        {
            get; private set;
        }

        /// <summary>
        /// Gets a value that contains the vertical size of the extent.
        /// </summary>
        /// <returns>
        /// A <see cref="double"/> that represents the vertical size of the extent, in pixels. The
        /// default is 0.
        /// </returns>
        public double ExtentHeight
        {
            get; private set;
        }

        /// <summary>
        /// Gets a value that contains the horizontal offset of the scrolled content.
        /// </summary>
        /// <returns>
        /// A <see cref="double"/> that represents the horizontal offset of the scrolled content,
        /// in pixels. The default is 0.
        /// </returns>
        public double HorizontalOffset
        {
            get; private set;
        }

        /// <summary>
        /// Gets a value that represents how far down the content is currently scrolled.
        /// </summary>
        /// <returns>
        /// A <see cref="double"/> that represents the vertical offset of the scrolled content,
        /// in pixels. The default is 0.
        /// </returns>
        public double VerticalOffset
        {
            get; private set;
        }

        /// <summary>
        /// Gets a value that contains the horizontal size of the viewport (visible area)
        /// of the content.
        /// </summary>
        /// <returns>
        /// A <see cref="double"/> that represents the vertical size of the viewport (visible area)
        /// of the content, in pixels. The default is 0.
        /// </returns>
        public double ViewportWidth
        {
            get; private set;
        }

        /// <summary>
        /// Gets a value that contains the vertical size of the viewport (visible area) of
        /// the content.
        /// </summary>
        /// <returns>
        /// A <see cref="double"/> that represents the vertical size of the viewport (visible area)
        /// of the content, in pixels. The default is 0.
        /// </returns>
        public double ViewportHeight
        {
            get; private set;
        }

        /// <summary>
        /// Gets or sets a value that identifies the container that controls scrolling behavior
        /// in this <see cref="VirtualizingStackPanel"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="ScrollViewer"/> that owns scrolling for this <see cref="VirtualizingStackPanel"/>.
        /// </returns>
        public ScrollViewer ScrollOwner { get; set; }

        //
        // Note: When scrolling along the stacking orientation,
        // Silverlight will perform logical scrolling. That
        // is, to say, it will scroll by items and not pixels.
        //

        /// <summary>
        /// Scrolls content downward by one logical unit.
        /// </summary>
        public virtual void LineDown()
        {
            if (Orientation == Orientation.Horizontal)
                SetVerticalOffset(VerticalOffset + LineDelta);
            else
                SetVerticalOffset(VerticalOffset + 1);
        }

        /// <summary>
        /// Scrolls content to the left by one logical unit.
        /// </summary>
        public virtual void LineLeft()
        {
            if (Orientation == Orientation.Vertical)
                SetHorizontalOffset(HorizontalOffset - LineDelta);
            else
                SetHorizontalOffset(HorizontalOffset - 1);
        }

        /// <summary>
        /// Scrolls content to the right by one logical unit.
        /// </summary>
        public virtual void LineRight()
        {
            if (Orientation == Orientation.Vertical)
                SetHorizontalOffset(HorizontalOffset + LineDelta);
            else
                SetHorizontalOffset(HorizontalOffset + 1);
        }

        /// <summary>
        /// Scrolls content upward by one logical unit.
        /// </summary>
        public virtual void LineUp()
        {
            if (Orientation == Orientation.Horizontal)
                SetVerticalOffset(VerticalOffset - LineDelta);
            else
                SetVerticalOffset(VerticalOffset - 1);
        }

        /// <summary>
        /// Scrolls to the specified coordinates and makes that portion of a <see cref="UIElement"/>
        /// visible.
        /// </summary>
        /// <param name="visual">
        /// The <see cref="UIElement"/> that becomes visible.
        /// </param>
        /// <param name="rectangle">
        /// A <see cref="Rect"/> that represents the coordinate space within a <see cref="UIElement"/>.
        /// </param>
        /// <returns>
        /// Rectangular area of the System.Windows.UIElement now visible.
        /// </returns>
        public Rect MakeVisible(UIElement visual, Rect rectangle)
        {
            Rect exposed = new Rect(0, 0, 0, 0);

            foreach (UIElement child in Children)
            {
                if (child == visual)
                {
                    if (Orientation == Orientation.Vertical)
                    {
                        if (rectangle.X != HorizontalOffset)
                            SetHorizontalOffset(rectangle.X);

                        exposed.Width = Math.Min(child.RenderSize.Width, ViewportWidth);
                        exposed.Height = child.RenderSize.Height;
                        exposed.X = HorizontalOffset;
                    }
                    else
                    {
                        if (rectangle.Y != VerticalOffset)
                            SetVerticalOffset(rectangle.Y);

                        exposed.Height = Math.Min(child.RenderSize.Height, ViewportHeight);
                        exposed.Width = child.RenderSize.Width;
                        exposed.Y = VerticalOffset;
                    }

                    return exposed;
                }

                if (Orientation == Orientation.Vertical)
                    exposed.Y += child.RenderSize.Height;
                else
                    exposed.X += child.RenderSize.Width;
            }

            throw new ArgumentException("Visual is not a child of this Panel");
        }

        /// <summary>
        /// Scrolls content logically downward in response to a downward click of the mouse
        /// wheel button.
        /// </summary>
        public virtual void MouseWheelDown()
        {
            if (Orientation == Orientation.Horizontal)
                SetVerticalOffset(VerticalOffset + LineDelta * Wheelitude);
            else
                SetVerticalOffset(VerticalOffset + Wheelitude);
        }

        /// <summary>
        /// Scrolls content logically to the left in response to a left click of the mouse
        /// wheel button.
        /// </summary>
        public virtual void MouseWheelLeft()
        {
            if (Orientation == Orientation.Vertical)
                SetHorizontalOffset(HorizontalOffset - LineDelta * Wheelitude);
            else
                SetHorizontalOffset(HorizontalOffset - Wheelitude);
        }

        /// <summary>
        /// Scrolls content logically to the right in response to a right click of the mouse
        /// wheel button.
        /// </summary>
        public virtual void MouseWheelRight()
        {
            if (Orientation == Orientation.Vertical)
                SetHorizontalOffset(HorizontalOffset + LineDelta * Wheelitude);
            else
                SetHorizontalOffset(HorizontalOffset + Wheelitude);
        }

        /// <summary>
        /// Scrolls content logically upward in response to an upward click of the mouse
        /// wheel button.
        /// </summary>
        public virtual void MouseWheelUp()
        {
            if (Orientation == Orientation.Horizontal)
                SetVerticalOffset(VerticalOffset - LineDelta * Wheelitude);
            else
                SetVerticalOffset(VerticalOffset - Wheelitude);
        }

        /// <summary>
        /// Scrolls content downward by one page.
        /// </summary>
        public virtual void PageDown()
        {
            SetVerticalOffset(VerticalOffset + ViewportHeight);
        }

        /// <summary>
        /// Scrolls content to the left by one page.
        /// </summary>
        public virtual void PageLeft()
        {
            SetHorizontalOffset(HorizontalOffset - ViewportWidth);
        }

        /// <summary>
        /// Scrolls content to the right by one page.
        /// </summary>
        public virtual void PageRight()
        {
            SetHorizontalOffset(HorizontalOffset + ViewportWidth);
        }

        /// <summary>
        /// Scrolls content upward by one page.
        /// </summary>
        public virtual void PageUp()
        {
            SetVerticalOffset(VerticalOffset - ViewportHeight);
        }

        /// <summary>
        /// Sets the value of the <see cref="HorizontalOffset"/>
        /// property.
        /// </summary>
        /// <param name="offset">
        /// The value of the <see cref="HorizontalOffset"/>
        /// property.
        /// </param>
        public void SetHorizontalOffset(double offset)
        {
            if (offset < 0 || ViewportWidth >= ExtentWidth)
                offset = 0;
            else if (offset + ViewportWidth >= ExtentWidth)
                offset = ExtentWidth - ViewportWidth;

            if (HorizontalOffset == offset)
                return;

            HorizontalOffset = offset;

            if (Orientation == Orientation.Horizontal)
                InvalidateMeasure();
            else
                InvalidateArrange();

            if (ScrollOwner != null)
                ScrollOwner.InvalidateScrollInfo();
        }

        /// <summary>
        /// Sets the value of the <see cref="VerticalOffset"/>
        /// property.
        /// </summary>
        /// <param name="offset">
        /// The value of the <see cref="VerticalOffset"/>
        /// property.
        /// </param>
        public void SetVerticalOffset(double offset)
        {
            if (offset < 0 || ViewportHeight >= ExtentHeight)
                offset = 0;
            else if (offset + ViewportHeight >= ExtentHeight)
                offset = ExtentHeight - ViewportHeight;

            if (VerticalOffset == offset)
                return;

            VerticalOffset = offset;

            if (Orientation == Orientation.Vertical)
                InvalidateMeasure();
            else
                InvalidateArrange();

            if (ScrollOwner != null)
                ScrollOwner.InvalidateScrollInfo();
        }
        #endregion "IScrollInfo"
    }
}
