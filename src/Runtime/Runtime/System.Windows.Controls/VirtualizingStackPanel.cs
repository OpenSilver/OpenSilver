
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

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using OpenSilver.Internal;
using OpenSilver.Internal.Controls;

namespace System.Windows.Controls;

/// <summary>
/// Arranges and virtualizes content on a single line that is oriented either horizontally or vertically.
/// </summary>
public class VirtualizingStackPanel : VirtualizingPanel, IScrollInfo
{
    // Scrolling and virtualization data.  Only used when this is the scrolling panel (IsScrolling is true).
    // When VSP is in pixel mode _scrollData is in units of pixels.  Otherwise the units are logical.
    private ScrollData _scrollData;
    private bool _isVirtualizing;
    private int _firstItemInViewportIndex;
    private double _firstItemInViewportPixelOffset;

    /// <summary>
    /// Initializes a new instance of the <see cref="VirtualizingStackPanel"/> class.
    /// </summary>
    public VirtualizingStackPanel() { }

    /// <summary>
    /// The current virtualization mode of the <see cref="VirtualizingStackPanel"/> (whether it is 
    /// <see cref="VirtualizationMode.Recycling"/> or not).
    /// </summary>
    public static readonly DependencyProperty VirtualizationModeProperty =
        DependencyProperty.RegisterAttached(
            "VirtualizationMode",
            typeof(VirtualizationMode),
            typeof(VirtualizingStackPanel),
            new PropertyMetadata(VirtualizationMode.Recycling));

    /// <summary>
    /// Returns the <see cref="VirtualizationMode"/> for the specified object.
    /// </summary>
    /// <param name="element">
    /// The object from which the <see cref="VirtualizationMode"/> is read.
    /// </param>
    /// <returns>
    /// One of the enumeration values that specifies whether the object uses container recycling.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// element is null.
    /// </exception>
    public static VirtualizationMode GetVirtualizationMode(DependencyObject element)
    {
        if (element is null)
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
        if (element is null)
        {
            throw new ArgumentNullException(nameof(element));
        }

        element.SetValueInternal(VirtualizationModeProperty, value);
    }

    /// <summary>
    /// Identifies the VirtualizingStackPanel.ScrollAmount attached dependency property.
    /// </summary>
    public static readonly DependencyProperty ScrollAmountProperty =
        DependencyProperty.RegisterAttached(
            "ScrollAmount",
            typeof(double),
            typeof(VirtualizingStackPanel),
            new PropertyMetadata(1.0),
            IsScrollAmountValid);

    /// <summary>
    /// Returns the scroll amount for the specified object.
    /// </summary>
    /// <param name="element">
    /// The object from which the scroll amount is read.
    /// </param>
    /// <returns>
    /// A <see cref="double"/> representing the scroll amount. The default value is 1.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// element is null.
    /// </exception>
    public static double GetScrollAmount(DependencyObject element)
    {
        if (element is null)
        {
            throw new ArgumentNullException(nameof(element));
        }

        return (double)element.GetValue(ScrollAmountProperty);
    }

    /// <summary>
    /// Sets the scroll amount on the specified object.
    /// </summary>
    /// <param name="element">
    /// The element on which to set the scroll amount.
    /// </param>
    /// <param name="value">
    /// A <see cref="double"/> that indicates the scrolling amount. 
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// element is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// value is not greater than 0.
    /// </exception>
    public static void SetScrollAmount(DependencyObject element, double value)
    {
        if (element is null)
        {
            throw new ArgumentNullException(nameof(element));
        }

        element.SetValueInternal(ScrollAmountProperty, value);
    }

    private static bool IsScrollAmountValid(object value) => (double)value > 0.0;

    private static readonly DependencyPropertyKey IsVirtualizingPropertyKey =
        DependencyProperty.RegisterAttachedReadOnly(
            "IsVirtualizing",
            typeof(bool),
            typeof(VirtualizingStackPanel),
            new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// A value that indicates whether the <see cref="VirtualizingStackPanel"/> is using virtualization.
    /// </summary>
    public static readonly DependencyProperty IsVirtualizingProperty = IsVirtualizingPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets a value that determines whether the <see cref="VirtualizingStackPanel"/> is virtualizing its content.
    /// </summary>
    /// <param name="o">
    /// The object being virtualized.
    /// </param>
    /// <returns>
    /// true if the <see cref="VirtualizingStackPanel"/> is virtualizing its content; otherwise false.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// o is null.
    /// </exception>
    public static bool GetIsVirtualizing(DependencyObject o)
    {
        if (o is null)
        {
            throw new ArgumentNullException(nameof(o));
        }

        return (bool)o.GetValue(IsVirtualizingProperty);
    }

    internal static void SetIsVirtualizing(DependencyObject o, bool value)
    {
        if (o is null)
        {
            throw new ArgumentNullException(nameof(o));
        }

        o.SetValueInternal(IsVirtualizingPropertyKey, value);
    }

    /// <summary>
    /// Identifies the <see cref="Orientation"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty OrientationProperty =
        DependencyProperty.Register(
            nameof(Orientation),
            typeof(Orientation),
            typeof(VirtualizingStackPanel),
            new FrameworkPropertyMetadata(Orientation.Vertical, FrameworkPropertyMetadataOptions.AffectsMeasure, OnOrientationChanged));

    /// <summary>
    /// Gets or sets a value that describes the horizontal or vertical orientation of stacked content.
    /// The default is <see cref="Orientation.Vertical"/>.
    /// </summary>
    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        set => SetValueInternal(OrientationProperty, value);
    }

    private static void OnOrientationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        ((VirtualizingStackPanel)sender).ResetScrolling();
    }

    /// <summary>
    /// Gets a value that represents the <see cref="Controls.Orientation"/> of the <see cref="VirtualizingStackPanel"/>.
    /// </summary>
    /// <returns>
    /// An <see cref="Controls.Orientation"/> value.
    /// </returns>
    protected internal override Orientation LogicalOrientation => Orientation;

    /// <summary>
    /// Gets a value that indicates if this <see cref="VirtualizingStackPanel"/> has a vertical or horizontal orientation.
    /// </summary>
    /// <returns>
    /// This property always returns true.
    /// </returns>
    protected internal override bool HasLogicalOrientation => true;

    /// <summary>
    /// Gets or sets a value that identifies the container that controls scrolling behavior in this <see cref="VirtualizingStackPanel"/>.
    /// </summary>
    /// <returns>
    /// The <see cref="ScrollViewer"/> that owns scrolling for this <see cref="VirtualizingStackPanel"/>.
    /// </returns>
    public ScrollViewer ScrollOwner
    {
        get { return _scrollData?._scrollOwner; }
        set
        {
            EnsureScrollData();
            if (value != _scrollData._scrollOwner)
            {
                ResetScrolling();
                _scrollData._scrollOwner = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets a value that indicates whether a <see cref="VirtualizingStackPanel"/> can scroll in the horizontal dimension.
    /// </summary>
    /// <returns>
    /// true if content can scroll in the horizontal dimension; otherwise, false. The default is false.
    /// </returns>
    public bool CanHorizontallyScroll
    {
        get { return _scrollData is not null && _scrollData._allowHorizontal; }
        set
        {
            EnsureScrollData();
            if (_scrollData._allowHorizontal != value)
            {
                _scrollData._allowHorizontal = value;
                InvalidateMeasure();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value that indicates whether content can scroll in the vertical dimension.
    /// </summary>
    /// <returns>
    /// true if content can scroll in the vertical dimension; otherwise, false. The default is false.
    /// </returns>
    public bool CanVerticallyScroll
    {
        get { return _scrollData is not null && _scrollData._allowVertical; }
        set
        {
            EnsureScrollData();
            if (_scrollData._allowVertical != value)
            {
                _scrollData._allowVertical = value;
                InvalidateMeasure();
            }
        }
    }

    /// <summary>
    /// Gets a value that contains the horizontal size of the extent.
    /// </summary>
    /// <returns>
    /// A <see cref="double"/> that represents the horizontal size of the extent, in pixels. The default is 0.
    /// </returns>
    public double ExtentWidth => _scrollData is null ? 0.0 : _scrollData._extent.Width;

    /// <summary>
    /// Gets a value that contains the vertical size of the extent.
    /// </summary>
    /// <returns>
    /// A <see cref="double"/> that represents the vertical size of the extent, in pixels. The default is 0.
    /// </returns>
    public double ExtentHeight => _scrollData is null ? 0.0 : _scrollData._extent.Height;

    /// <summary>
    /// Gets a value that contains the horizontal offset of the scrolled content.
    /// </summary>
    /// <returns>
    /// A <see cref="double"/> that represents the horizontal offset of the scrolled content, in pixels. The default is 0.
    /// </returns>
    public double HorizontalOffset => _scrollData is null ? 0.0 : _scrollData._computedOffset.X;

    /// <summary>
    /// Gets a value that represents how far down the content is currently scrolled.
    /// </summary>
    /// <returns>
    /// A <see cref="double"/> that represents the vertical offset of the scrolled content, in pixels. The default is 0.
    /// </returns>
    public double VerticalOffset => _scrollData is null ? 0.0 : _scrollData._computedOffset.Y;

    /// <summary>
    /// Gets a value that contains the horizontal size of the viewport (visible area) of the content.
    /// </summary>
    /// <returns>
    /// A <see cref="double"/> that represents the vertical size of the viewport (visible area) of the content, in pixels.
    /// The default is 0.
    /// </returns>
    public double ViewportWidth => _scrollData is null ? 0.0 : _scrollData._viewport.Width;

    /// <summary>
    /// Gets a value that contains the vertical size of the viewport (visible area) of the content.
    /// </summary>
    /// <returns>
    /// A <see cref="double"/> that represents the vertical size of the viewport (visible area) of the content, in pixels.
    /// The default is 0.
    /// </returns>
    public double ViewportHeight => _scrollData is null ? 0.0 : _scrollData._viewport.Height;

    /// <summary>
    /// Occurs when an item that is hosted by the <see cref="VirtualizingStackPanel"/> is re-virtualized.
    /// </summary>
    public event CleanUpVirtualizedItemEventHandler CleanUpVirtualizedItemEvent;

    /// <summary>
    /// Measures the child elements of a <see cref="VirtualizingStackPanel"/> in anticipation of arranging them during 
    /// the <see cref="ArrangeOverride(Size)"/> pass.
    /// </summary>
    /// <param name="constraint">
    /// An upper limit <see cref="Size"/> that should not be exceeded.
    /// </param>
    /// <returns>
    /// The <see cref="Size"/> that represents the desired size of the element.
    /// </returns>
    protected override Size MeasureOverride(Size constraint)
    {
        if (IsItemsHost)
        {
            return MeasureItemsHost(constraint);
        }
        else
        {
            return MeasureNonItemsHost(constraint);
        }
    }

    private Size MeasureItemsHost(Size constraint)
    {
        // Ensure we always touch ItemContainerGenerator as by accessing this property
        // we hook up to some events on it.
        EnsureGenerator();
        bool isHorizontal = Orientation == Orientation.Horizontal;
        ItemsControl owner = ItemsControl.GetItemsOwner(this);
        int itemCount = owner.Items.Count;
        Size stackDesiredSize = new();
        double numberOfItemsInViewport = 0.0;
        _firstItemInViewportIndex = 0;
        _firstItemInViewportPixelOffset = 0.0;

        SetVirtualizationState(owner);

        IItemContainerGenerator generator = ItemContainerGenerator;
        if (itemCount > 0)
        {
            Size childConstraint = constraint;
            if (CanHorizontallyScroll || isHorizontal)
            {
                childConstraint.Width = double.PositiveInfinity;
            }
            if (CanVerticallyScroll || !isHorizontal)
            {
                childConstraint.Height = double.PositiveInfinity;
            }

            // Next, prepare and measure the extents of our viewable items...
            int nvisible = 0;
            (int firstItemInViewportIndex, double firstItemInViewportLogicalOffset) = ComputeFirstItemInViewportIndex(isHorizontal, itemCount);
            GeneratorPosition start = generator.GeneratorPositionFromIndex(firstItemInViewportIndex);

            using (generator.StartAt(start, GeneratorDirection.Forward, true))
            {
                double viewportSize = 0;
                int insertAt = start.Offset == 0 ? start.Index : start.Index + 1;
                int beyond = 0;
                List<UIElement> children = InternalChildren;

                for (int i = firstItemInViewportIndex; i < itemCount && beyond < 2; i++, insertAt++)
                {
                    // Generate the child container
                    UIElement child = (UIElement)generator.GenerateNext(out bool isNewlyRealized);
                    if (isNewlyRealized || insertAt >= children.Count || children[insertAt] != child)
                    {
                        // Add newly created children to the panel
                        if (insertAt < children.Count)
                        {
                            InsertInternalChild(insertAt, child);
                        }
                        else
                        {
                            AddInternalChild(child);
                        }

                        generator.PrepareItemContainer(child);
                    }

                    child.Measure(childConstraint);
                    Size size = child.DesiredSize;
                    nvisible++;

                    // Include the offset of the first item in the viewport, if any.
                    if (i == firstItemInViewportIndex)
                    {
                        // If we display only one item, we need to adjust the scroll offset because we don't want to scroll
                        // past the end of the last element. Currently, there is only one item if the first display item is
                        // the last item of the itemscontrol. This is guaranteed because we always 2 items after the last
                        // fully visible item, if possible. If this behavior changes in the future, the following condition
                        // will not be valid anymore.
                        if (i == itemCount - 1)
                        {
                            if (isHorizontal)
                            {
                                double overflow = size.Width - constraint.Width;
                                firstItemInViewportLogicalOffset = overflow switch
                                {
                                    > 0 => Math.Min(firstItemInViewportLogicalOffset, overflow / size.Width),
                                    _ => 0,
                                };
                            }
                            else
                            {
                                double overflow = size.Height - constraint.Height;
                                firstItemInViewportLogicalOffset = overflow switch
                                {
                                    > 0 => Math.Min(firstItemInViewportLogicalOffset, overflow / size.Height),
                                    _ => 0,
                                };
                            }
                        }

                        numberOfItemsInViewport -= firstItemInViewportLogicalOffset;
                        _firstItemInViewportPixelOffset = firstItemInViewportLogicalOffset * (isHorizontal ? size.Width : size.Height);
                        viewportSize -= _firstItemInViewportPixelOffset;
                    }

                    // Accumulate child size.
                    if (isHorizontal)
                    {
                        stackDesiredSize.Height = Math.Max(stackDesiredSize.Height, size.Height);
                        stackDesiredSize.Width += size.Width;
                        viewportSize += size.Width;

                        if (viewportSize > constraint.Width)
                        {
                            if (beyond == 0)
                            {
                                double overflow = viewportSize - constraint.Width;
                                numberOfItemsInViewport += 1 - (overflow / size.Width);
                            }

                            beyond++;
                        }
                        else
                        {
                            numberOfItemsInViewport += 1.0;
                        }
                    }
                    else
                    {
                        stackDesiredSize.Width = Math.Max(stackDesiredSize.Width, size.Width);
                        stackDesiredSize.Height += size.Height;
                        viewportSize += size.Height;

                        if (viewportSize > constraint.Height)
                        {
                            if (beyond == 0)
                            {
                                double overflow = viewportSize - constraint.Height;
                                numberOfItemsInViewport += 1 - (overflow / size.Height);
                            }

                            beyond++;
                        }
                        else
                        {
                            numberOfItemsInViewport += 1.0;
                        }
                    }
                }
            }

            if (nvisible > 0)
            {
                CleanupContainers(owner, firstItemInViewportIndex, nvisible);
            }
        }

        // Update our Extent and Viewport values

        if (IsScrolling)
        {
            Size viewport = isHorizontal ? new(numberOfItemsInViewport, constraint.Height) : new(constraint.Width, numberOfItemsInViewport);
            Size extent = isHorizontal ? new(itemCount, stackDesiredSize.Height) : new(stackDesiredSize.Width, itemCount);
            Vector offset = new(
                ScrollContentPresenter.CoerceOffset(_scrollData._offset.X, extent.Width, viewport.Width),
                ScrollContentPresenter.CoerceOffset(_scrollData._offset.Y, extent.Height, viewport.Height));

            VerifyScrollingData(viewport, extent, offset);

            stackDesiredSize.Width = Math.Min(stackDesiredSize.Width, constraint.Width);
            stackDesiredSize.Height = Math.Min(stackDesiredSize.Height, constraint.Height);
            _scrollData._maxDesiredSize.Width = Math.Max(stackDesiredSize.Width, _scrollData._maxDesiredSize.Width);
            _scrollData._maxDesiredSize.Height = Math.Max(stackDesiredSize.Height, _scrollData._maxDesiredSize.Height);
            stackDesiredSize = _scrollData._maxDesiredSize;
        }

        return stackDesiredSize;
    }

    /// <summary>
    /// Sets up IsVirtualizing and VirtualizationMode
    /// </summary>
    private void SetVirtualizationState(ItemsControl itemsControl)
    {
        if (itemsControl is null)
        {
            IsVirtualizing = false;
            InRecyclingMode = false;
            return;
        }

        if (IsScrolling)
        {
            IsVirtualizing = true;
            if (IsVirtualizing)
            {
                SetIsVirtualizing(itemsControl, true);
            }
        }

        InRecyclingMode = GetVirtualizationMode(itemsControl) == VirtualizationMode.Recycling;
    }

    private void CleanupContainers(ItemsControl owner, int firstItemInViewportIndex, int count)
    {
        List<UIElement> children = InternalChildren;
        IRecyclingItemContainerGenerator generator = ItemContainerGenerator as IRecyclingItemContainerGenerator;
        int last = firstItemInViewportIndex + count - 1;
        int index = 0;

        var pos = new GeneratorPosition(children.Count - 1, 0);
        while (pos.Index >= 0)
        {
            int item = generator.IndexFromGeneratorPosition(pos);

            if ((item < firstItemInViewportIndex || item > last) &&
                !((IGeneratorHost)owner).IsItemItsOwnContainer(owner.Items[item]) &&
                !FocusManager.HasFocus(children[pos.Index], false) &&
                NotifyCleanupItem(children[pos.Index], owner))
            {
                RemoveInternalChildRange(pos.Index, 1);

                if (InRecyclingMode)
                {
                    generator.Recycle(pos, 1);
                }
                else
                {
                    generator.Remove(pos, 1);
                }
            }
            else if (item < firstItemInViewportIndex)
            {
                index++;
            }

            pos.Index--;
        }

        _firstItemInViewportIndex = index;
    }

    private bool NotifyCleanupItem(UIElement child, ItemsControl itemsControl)
    {
        var e = new CleanUpVirtualizedItemEventArgs(itemsControl.ItemContainerGenerator.ItemFromContainer(child), child)
        {
            OriginalSource = this
        };

        OnCleanUpVirtualizedItem(e);

        return !e.Cancel;
    }

    private (int Index, double Offset) ComputeFirstItemInViewportIndex(bool isHorizontal, int itemCount)
    {
        double rawOffset = isHorizontal ? _scrollData._offset.X : _scrollData._offset.Y;

        if (rawOffset < 0)
        {
            return (0, 0);
        }
        else if (rawOffset >= itemCount)
        {
            return (itemCount - 1, 1);
        }
        else
        {
            int index = (int)rawOffset;
            double offset = rawOffset - index;
            return (index, offset);
        }
    }

    private Size MeasureNonItemsHost(Size constraint)
    {
        Size stackDesiredSize = new();
        List<UIElement> children = InternalChildren;
        Size layoutSlotSize = constraint;
        bool fHorizontal = Orientation == Orientation.Horizontal;
        int firstViewport;          // First child index in the viewport.
        int lastViewport = -1;      // Last child index in the viewport.  -1 indicates we have not yet iterated through the last child.

        double logicalVisibleSpace, childLogicalSize;

        //
        // Initialize child sizing and iterator data
        // Allow children as much size as they want along the stack.
        //
        if (fHorizontal)
        {
            layoutSlotSize.Width = double.PositiveInfinity;
            if (IsScrolling && CanVerticallyScroll)
            {
                layoutSlotSize.Height = double.PositiveInfinity;
            }
            firstViewport = IsScrolling ? CoerceOffsetToInteger(_scrollData._offset.X, children.Count) : 0;
            logicalVisibleSpace = constraint.Width;
        }
        else
        {
            layoutSlotSize.Height = double.PositiveInfinity;
            if (IsScrolling && CanHorizontallyScroll)
            {
                layoutSlotSize.Width = double.PositiveInfinity;
            }
            firstViewport = IsScrolling ? CoerceOffsetToInteger(_scrollData._offset.Y, children.Count) : 0;
            logicalVisibleSpace = constraint.Height;
        }

        //
        //  Iterate through children.
        //  While we still supported virtualization, this was hidden in a child iterator (see source history).
        //
        for (int i = 0, count = children.Count; i < count; ++i)
        {
            // Get next child.
            UIElement child = children[i];

            // Measure the child.
            child.Measure(layoutSlotSize);
            Size childDesiredSize = child.DesiredSize;

            // Accumulate child size.
            if (fHorizontal)
            {
                stackDesiredSize.Width += childDesiredSize.Width;
                stackDesiredSize.Height = Math.Max(stackDesiredSize.Height, childDesiredSize.Height);
                childLogicalSize = childDesiredSize.Width;
            }
            else
            {
                stackDesiredSize.Width = Math.Max(stackDesiredSize.Width, childDesiredSize.Width);
                stackDesiredSize.Height += childDesiredSize.Height;
                childLogicalSize = childDesiredSize.Height;
            }

            // Adjust remaining viewport space if we are scrolling and within the viewport region.
            // While scrolling (not virtualizing), we always measure children before and after the viewport.
            if (IsScrolling && lastViewport == -1 && i >= firstViewport)
            {
                logicalVisibleSpace -= childLogicalSize;
                if (DoubleUtil.LessThanOrClose(logicalVisibleSpace, 0.0))
                {
                    lastViewport = i;
                }
            }
        }

        //
        // Compute Scrolling stuff.
        //
        if (IsScrolling)
        {
            // Compute viewport and extent.
            Size viewport = constraint;
            Size extent = stackDesiredSize;
            Vector offset = _scrollData._offset;

            // If we have not yet set the last child in the viewport, set it to the last child.
            if (lastViewport == -1)
            {
                lastViewport = children.Count - 1;
            }

            // If we or children have resized, it's possible that we can now display more content.
            // This is true if we started at a nonzero offeset and still have space remaining.
            // In this case, we loop back through previous children until we run out of space.
            while (firstViewport > 0)
            {
                double projectedLogicalVisibleSpace = logicalVisibleSpace;
                if (fHorizontal)
                {
                    projectedLogicalVisibleSpace -= children[firstViewport - 1].DesiredSize.Width;
                }
                else
                {
                    projectedLogicalVisibleSpace -= children[firstViewport - 1].DesiredSize.Height;
                }

                // If we have run out of room, break.
                if (DoubleUtil.LessThan(projectedLogicalVisibleSpace, 0.0))
                {
                    break;
                }

                // Adjust viewport
                firstViewport--;
                logicalVisibleSpace = projectedLogicalVisibleSpace;
            }

            int logicalExtent = children.Count;
            int logicalViewport = lastViewport - firstViewport;

            // We are conservative when estimating a viewport, not including the last element in case it is only partially visible.
            // We want to count it if it is fully visible (>= 0 space remaining) or the only element in the viewport.
            if (logicalViewport == 0 || DoubleUtil.GreaterThanOrClose(logicalVisibleSpace, 0.0))
            {
                logicalViewport++;
            }

            if (fHorizontal)
            {
                viewport.Width = logicalViewport;
                extent.Width = logicalExtent;
                offset.X = firstViewport;
                offset.Y = Math.Max(0, Math.Min(offset.Y, extent.Height - viewport.Height));
            }
            else
            {
                viewport.Height = logicalViewport;
                extent.Height = logicalExtent;
                offset.Y = firstViewport;
                offset.X = Math.Max(0, Math.Min(offset.X, extent.Width - viewport.Width));
            }

            // Since we can offset and clip our content, we never need to be larger than the parent suggestion.
            // If we returned the full size of the content, we would always be so big we didn't need to scroll.  :)
            stackDesiredSize.Width = Math.Min(stackDesiredSize.Width, constraint.Width);
            stackDesiredSize.Height = Math.Min(stackDesiredSize.Height, constraint.Height);

            // Verify Scroll Info, invalidate ScrollOwner if necessary.
            VerifyScrollingData(viewport, extent, offset);
        }

        return stackDesiredSize;
    }

    private static int CoerceOffsetToInteger(double offset, int numberOfItems)
    {
        int iNewOffset;

        if (double.IsNegativeInfinity(offset))
        {
            iNewOffset = 0;
        }
        else if (double.IsPositiveInfinity(offset))
        {
            iNewOffset = numberOfItems - 1;
        }
        else
        {
            iNewOffset = (int)offset;
            iNewOffset = Math.Max(Math.Min(numberOfItems - 1, iNewOffset), 0);
        }

        return iNewOffset;
    }

    private void VerifyScrollingData(Size viewport, Size extent, Vector offset)
    {
        bool fValid = true;

        Debug.Assert(IsScrolling);

        fValid &= DoubleUtil.AreClose(viewport, _scrollData._viewport);
        fValid &= DoubleUtil.AreClose(extent, _scrollData._extent);
        fValid &= DoubleUtil.AreClose(offset, _scrollData._computedOffset);
        _scrollData._offset = offset;

        if (!fValid)
        {
            _scrollData._viewport = viewport;
            _scrollData._extent = extent;
            _scrollData._computedOffset = offset;
            OnScrollChange();
        }
    }

    private void OnScrollChange() => ScrollOwner?.InvalidateScrollInfo();

    /// <summary>
    /// Arranges the content of a <see cref="VirtualizingStackPanel"/> element.
    /// </summary>
    /// <param name="arrangeSize">
    /// The <see cref="Size"/> that this element should use to arrange its child elements.
    /// </param>
    /// <returns>
    /// The <see cref="Size"/> that represents the arranged size of this <see cref="VirtualizingStackPanel"/> element and its child elements.
    /// </returns>
    protected override Size ArrangeOverride(Size arrangeSize)
    {
        if (IsItemsHost)
        {
            return ArrangeItemsHost(arrangeSize);
        }
        else
        {
            return ArrangeNonItemsHost(arrangeSize);
        }
    }

    private Size ArrangeItemsHost(Size arrangeSize)
    {
        bool isHorizontal = Orientation == Orientation.Horizontal;
        var rcChild = new Rect(arrangeSize);
        double previousChildSize = 0.0;
        List<UIElement> children = InternalChildren;

        if (isHorizontal)
        {
            rcChild.X = -ComputeFirstItemInViewportOffset(true);
            rcChild.Y = -_scrollData._computedOffset.Y;
        }
        else
        {
            rcChild.X = -_scrollData._computedOffset.X;
            rcChild.Y = -ComputeFirstItemInViewportOffset(false);
        }

        for (int i = 0; i < children.Count; i++)
        {
            UIElement child = children[i];

            Size desiredSize = child.DesiredSize;

            if (isHorizontal)
            {
                rcChild.X += previousChildSize;
                previousChildSize = desiredSize.Width;
                rcChild.Width = desiredSize.Width;
                rcChild.Height = Math.Max(arrangeSize.Height, desiredSize.Height);
            }
            else
            {
                rcChild.Y += previousChildSize;
                previousChildSize = desiredSize.Height;
                rcChild.Width = Math.Max(arrangeSize.Width, desiredSize.Width);
                rcChild.Height = desiredSize.Height;
            }

            child.Arrange(rcChild);
        }

        return arrangeSize;
    }

    private double ComputeFirstItemInViewportOffset(bool isHorizontal)
    {
        double offset = _firstItemInViewportPixelOffset;

        List<UIElement> children = InternalChildren;
        for (int i = 0; i < _firstItemInViewportIndex && i < children.Count; i++)
        {
            UIElement child = children[i];
            offset += isHorizontal ? child.DesiredSize.Width : child.DesiredSize.Height;
        }

        return offset;
    }

    private Size ArrangeNonItemsHost(Size arrangeSize)
    {
        List<UIElement> children = InternalChildren;
        bool fHorizontal = Orientation == Orientation.Horizontal;
        Rect rcChild = new Rect(arrangeSize);
        double previousChildSize = 0.0;

        //
        // Compute scroll offset and seed it into rcChild.
        //
        if (IsScrolling)
        {
            if (fHorizontal)
            {
                rcChild.X = ComputePhysicalFromLogicalOffset(_scrollData._computedOffset.X, true);
                rcChild.Y = -1.0 * _scrollData._computedOffset.Y;
            }
            else
            {
                rcChild.X = -1.0 * _scrollData._computedOffset.X;
                rcChild.Y = ComputePhysicalFromLogicalOffset(_scrollData._computedOffset.Y, false);
            }
        }

        //
        // Arrange and Position Children.
        //
        for (int i = 0, count = children.Count; i < count; ++i)
        {
            UIElement child = children[i];

            if (fHorizontal)
            {
                rcChild.X += previousChildSize;
                previousChildSize = child.DesiredSize.Width;
                rcChild.Width = previousChildSize;
                rcChild.Height = Math.Max(arrangeSize.Height, child.DesiredSize.Height);
            }
            else
            {
                rcChild.Y += previousChildSize;
                previousChildSize = child.DesiredSize.Height;
                rcChild.Height = previousChildSize;
                rcChild.Width = Math.Max(arrangeSize.Width, child.DesiredSize.Width);
            }

            child.Arrange(rcChild);
        }
        return arrangeSize;
    }

    // Translates a logical (child index) offset to a physical (1/96") when scrolling.
    // If virtualizing, it makes the assumption that the logicalOffset is always the first in the visual collection
    // and thus returns 0.
    // If not virtualizing, it assumes that children are Measure clean; should only be called after running Measure.
    private double ComputePhysicalFromLogicalOffset(double logicalOffset, bool fHorizontal)
    {
        double physicalOffset = 0.0;

        List<UIElement> children = InternalChildren;
        Debug.Assert(logicalOffset == 0 || (logicalOffset > 0 && logicalOffset < children.Count));

        for (int i = 0; i < logicalOffset; i++)
        {
            physicalOffset -= fHorizontal ? children[i].DesiredSize.Width : children[i].DesiredSize.Height;
        }

        return physicalOffset;
    }

    /// <summary>
    /// Called when the collection of child elements is cleared by the base <see cref="Panel"/> class.
    /// </summary>
    protected override void OnClearChildren()
    {
        base.OnClearChildren();
        _firstItemInViewportIndex = 0;
        _firstItemInViewportPixelOffset = 0;
    }

    /// <summary>
    /// Called when the <see cref="ItemsControl.Items"/> collection that is associated with the <see cref="ItemsControl"/>
    /// for this <see cref="Panel"/> changes.
    /// </summary>
    /// <param name="sender">
    /// The <see cref="object"/> that raised the event.
    /// </param>
    /// <param name="args">
    /// Provides data for the <see cref="ItemContainerGenerator.ItemsChanged"/> event.
    /// </param>
    protected override void OnItemsChanged(object sender, ItemsChangedEventArgs args)
    {
        base.OnItemsChanged(sender, args);

        bool resetMaximumDesiredSize = false;

        switch (args.Action)
        {
            case NotifyCollectionChangedAction.Remove:
                OnItemsRemove(args);
                resetMaximumDesiredSize = true;
                break;

            case NotifyCollectionChangedAction.Replace:
                OnItemsReplace(args);
                resetMaximumDesiredSize = true;
                break;

            case NotifyCollectionChangedAction.Move:
                OnItemsMove(args);
                break;

            case NotifyCollectionChangedAction.Reset:
                resetMaximumDesiredSize = true;
                break;
        }

        if (resetMaximumDesiredSize && IsScrolling)
        {
            _scrollData._maxDesiredSize = new Size();
        }
    }

    private void OnItemsRemove(ItemsChangedEventArgs args) => RemoveChildRange(args.Position, args.ItemCount, args.ItemUICount);

    private void OnItemsReplace(ItemsChangedEventArgs args) => RemoveChildRange(args.Position, args.ItemCount, args.ItemUICount);

    private void OnItemsMove(ItemsChangedEventArgs args) => RemoveChildRange(args.OldPosition, args.ItemCount, args.ItemUICount);

    private void RemoveChildRange(GeneratorPosition position, int itemCount, int itemUICount)
    {
        if (IsItemsHost)
        {
            List<UIElement> children = InternalChildren;
            int pos = position.Index;
            if (position.Offset > 0)
            {
                // An item is being removed after the one at the index
                pos++;
            }

            if (pos < children.Count)
            {
                int uiCount = itemUICount;
                Debug.Assert((itemCount == itemUICount) || (itemUICount == 0), "Both ItemUICount and ItemCount should be equal or ItemUICount should be 0.");
                if (uiCount > 0)
                {
                    RemoveInternalChildRange(pos, uiCount);
                }
            }
        }
    }

    /// <summary>
    /// Called when an item that is hosted by the <see cref="VirtualizingStackPanel"/> is re-virtualized.
    /// </summary>
    /// <param name="e">
    /// Data about the event.
    /// </param>
    protected virtual void OnCleanUpVirtualizedItem(CleanUpVirtualizedItemEventArgs e) => CleanUpVirtualizedItemEvent?.Invoke(this, e);

    /// <summary>
    /// Generates the item at the specified index and calls BringIntoView on it.
    /// </summary>
    /// <param name="index">
    /// Specify the item index that should become visible. This is the index into <see cref="ItemsControl.Items"/> collection.
    /// </param>
    protected override void BringIndexIntoView(int index)
    {
        if (Orientation == Orientation.Horizontal)
        {
            SetHorizontalOffset(index);
        }
        else
        {
            SetVerticalOffset(index);
        }
    }

    /// <summary>
    /// Scrolls content upward by one logical unit.
    /// </summary>
    public virtual void LineUp()
    {
        double offset = Orientation == Orientation.Horizontal ? ScrollViewer.LineDelta : ScrollAmount;
        SetVerticalOffset(VerticalOffset - offset);
    }

    /// <summary>
    /// Scrolls content downward by one logical unit.
    /// </summary>
    public virtual void LineDown()
    {
        double offset = Orientation == Orientation.Horizontal ? ScrollViewer.LineDelta : ScrollAmount;
        SetVerticalOffset(VerticalOffset + offset);
    }

    /// <summary>
    /// Scrolls content to the left by one logical unit.
    /// </summary>
    public virtual void LineLeft()
    {
        double offset = Orientation == Orientation.Vertical ? ScrollViewer.LineDelta : ScrollAmount;
        SetHorizontalOffset(HorizontalOffset - offset);
    }

    /// <summary>
    /// Scrolls content to the right by one logical unit.
    /// </summary>
    public virtual void LineRight()
    {
        double offset = Orientation == Orientation.Vertical ? ScrollViewer.LineDelta : ScrollAmount;
        SetHorizontalOffset(HorizontalOffset + offset);
    }

    /// <summary>
    /// Scrolls content logically upward in response to an upward click of the mouse wheel button.
    /// </summary>
    public virtual void MouseWheelUp()
    {
        double offset = Orientation == Orientation.Horizontal ?
            SystemParameters.WheelScrollLines * ScrollViewer.LineDelta :
            SystemParameters.WheelScrollLines * ScrollAmount;

        SetVerticalOffset(VerticalOffset - offset);
    }

    /// <summary>
    /// Scrolls content logically downward in response to a downward click of the mouse wheel button.
    /// </summary>
    public virtual void MouseWheelDown()
    {
        double offset = Orientation == Orientation.Horizontal ?
            SystemParameters.WheelScrollLines * ScrollViewer.LineDelta :
            SystemParameters.WheelScrollLines * ScrollAmount;

        SetVerticalOffset(VerticalOffset + offset);
    }

    /// <summary>
    /// Scrolls content logically to the left in response to a left click of the mouse wheel button.
    /// </summary>
    public virtual void MouseWheelLeft()
    {
        double offset = Orientation == Orientation.Vertical ?
            SystemParameters.WheelScrollLines * ScrollViewer.LineDelta :
            SystemParameters.WheelScrollLines * ScrollAmount;

        SetHorizontalOffset(HorizontalOffset - offset);
    }

    /// <summary>
    /// Scrolls content logically to the right in response to a right click of the mouse wheel button.
    /// </summary>
    public virtual void MouseWheelRight()
    {
        double offset = Orientation == Orientation.Vertical ?
            SystemParameters.WheelScrollLines * ScrollViewer.LineDelta :
            SystemParameters.WheelScrollLines * ScrollAmount;

        SetHorizontalOffset(HorizontalOffset + offset);
    }

    /// <summary>
    /// Scrolls content upward by one page.
    /// </summary>
    public virtual void PageUp() => SetVerticalOffset(VerticalOffset - ViewportHeight);

    /// <summary>
    /// Scrolls content downward by one page.
    /// </summary>
    public virtual void PageDown() => SetVerticalOffset(VerticalOffset + ViewportHeight);

    /// <summary>
    /// Scrolls content to the left by one page.
    /// </summary>
    public virtual void PageLeft() => SetHorizontalOffset(HorizontalOffset - ViewportWidth);

    /// <summary>
    /// Scrolls content to the right by one page.
    /// </summary>
    public virtual void PageRight() => SetHorizontalOffset(HorizontalOffset + ViewportWidth);

    /// <summary>
    /// Sets the value of the <see cref="HorizontalOffset"/> property.
    /// </summary>
    /// <param name="offset">
    /// The value of the <see cref="HorizontalOffset"/> property.
    /// </param>
    public void SetHorizontalOffset(double offset)
    {
        if (!IsScrolling) return;

        double scrollX = ScrollContentPresenter.ValidateInputOffset(offset, nameof(HorizontalOffset));
        if (!DoubleUtil.AreClose(scrollX, _scrollData._offset.X))
        {
            // Store the new offset
            _scrollData._offset.X = scrollX;
            InvalidateMeasure();
        }
    }

    /// <summary>
    /// Sets the value of the <see cref="VerticalOffset"/> property.
    /// </summary>
    /// <param name="offset">
    /// The value of the <see cref="VerticalOffset"/> property.
    /// </param>
    public void SetVerticalOffset(double offset)
    {
        if (!IsScrolling) return;

        double scrollY = ScrollContentPresenter.ValidateInputOffset(offset, nameof(VerticalOffset));
        if (!DoubleUtil.AreClose(scrollY, _scrollData._offset.Y))
        {
            // Store the new offset
            _scrollData._offset.Y = scrollY;
            InvalidateMeasure();
        }
    }

    /// <summary>
    /// Scrolls to the specified coordinates and makes that portion of a <see cref="UIElement"/> visible.
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

        foreach (UIElement child in InternalChildren)
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

    private bool IsScrolling => ScrollOwner is not null;

    private bool IsVirtualizing
    {
        get { return _isVirtualizing; }
        set
        {
            // We must be the ItemsHost to turn on Virtualization.
            bool isVirtualizing = IsItemsHost && value;

            _isVirtualizing = isVirtualizing;
        }
    }

    private double ScrollAmount
    {
        get
        {
            if (ItemsControl.GetItemsOwner(this) is ItemsControl owner)
            {
                return GetScrollAmount(owner);
            }

            return 1.0;
        }
    }

    private bool InRecyclingMode { get; set; }

    private void EnsureScrollData() => _scrollData ??= new ScrollData();

    private void ResetScrolling()
    {
        InvalidateMeasure();

        // Clear scrolling data.  Because of thrash (being disconnected & reconnected, &c...), we may
        if (IsScrolling)
        {
            _scrollData.ClearLayout();
        }
    }

    // Helper class to hold scrolling data.
    // This class exists to reduce working set when VirtualizingStackPanel is used outside a scrolling situation.
    private sealed class ScrollData
    {
        // Clears layout generated data.
        // Does not clear scrollOwner, because unless resetting due to a scrollOwner change, we won't get reattached.
        internal void ClearLayout()
        {
            _offset = new Vector();
            _viewport = _extent = _maxDesiredSize = new Size();
        }

        internal ScrollViewer _scrollOwner;
        internal bool _allowHorizontal;
        internal bool _allowVertical;
        internal Vector _offset;
        internal Vector _computedOffset;
        internal Size _viewport;
        internal Size _extent;
        internal Size _maxDesiredSize;
    }
}
