
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

using OpenSilver.Internal;
using OpenSilver.Internal.Controls;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;

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

    // The container that a BringIndexIntoView operation realized and is trying to scroll into view.
    // It is kept alive (not recycled) by the virtualization cleanup until the scroll has settled.
    private FrameworkElement _bringIntoViewContainer;

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
        ArgumentNullException.ThrowIfNull(element);

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
        ArgumentNullException.ThrowIfNull(element);

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
        ArgumentNullException.ThrowIfNull(element);

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
        ArgumentNullException.ThrowIfNull(element);

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
        ArgumentNullException.ThrowIfNull(o);

        return (bool)o.GetValue(IsVirtualizingProperty);
    }

    internal static void SetIsVirtualizing(DependencyObject o, bool value)
    {
        ArgumentNullException.ThrowIfNull(o);

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
            new FrameworkPropertyMetadata(Orientation.Vertical, FrameworkPropertyMetadataOptions.AffectsMeasure, OnOrientationChanged),
            ScrollBar.IsValidOrientation);

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
    /// Identifies the <b>VirtualizingStackPanel.CleanUpVirtualizedItem</b> attached event.
    /// </summary>
    public static readonly RoutedEvent CleanUpVirtualizedItemEvent =
        EventManager.RegisterRoutedEvent(
            "CleanUpVirtualizedItemEvent",
            RoutingStrategy.Direct,
            typeof(CleanUpVirtualizedItemEventHandler),
            typeof(VirtualizingStackPanel));

    /// <summary>
    /// Adds an event handler for the <b>VirtualizingStackPanel.CleanUpVirtualizedItem</b> attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="DependencyObject"/> that is listening for this event.
    /// </param>
    /// <param name="handler">
    /// The event handler that is to be added.
    /// </param>
    public static void AddCleanUpVirtualizedItemHandler(DependencyObject element, CleanUpVirtualizedItemEventHandler handler)
        => AddHandler(element, CleanUpVirtualizedItemEvent, handler);

    /// <summary>
    /// Removes an event handler for the <b>VirtualizingStackPanel.CleanUpVirtualizedItem</b> attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="DependencyObject"/> from which the handler is being removed.
    /// </param>
    /// <param name="handler">
    /// Specifies the event handler that is to be removed.
    /// </param>
    public static void RemoveCleanUpVirtualizedItemHandler(DependencyObject element, CleanUpVirtualizedItemEventHandler handler)
        => RemoveHandler(element, CleanUpVirtualizedItemEvent, handler);

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
            int beyond = 0;
            double viewportSize = 0;
            double constraintSize = isHorizontal ? constraint.Width : constraint.Height;
            (int firstItemInViewportIndex, double firstItemInViewportLogicalOffset) = ComputeFirstItemInViewportIndex(isHorizontal, itemCount);
            GeneratorPosition start = generator.GeneratorPositionFromIndex(firstItemInViewportIndex);
            List<UIElement> children = UnsafeGetChildren();

            using (generator.StartAt(start, GeneratorDirection.Forward, true))
            {
                int insertAt = start.Offset == 0 ? start.Index : start.Index + 1;

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

            if (beyond == 0 && viewportSize < constraintSize)
            {
                using (generator.StartAt(start, GeneratorDirection.Backward, true))
                {
                    UIElement firstChild = (UIElement)generator.GenerateNext(out _);
                    Size firstChildSize = firstChild.DesiredSize;

                    numberOfItemsInViewport += firstItemInViewportLogicalOffset;
                    viewportSize += _firstItemInViewportPixelOffset;
                    _firstItemInViewportPixelOffset = 0.0;

                    if (viewportSize > constraintSize)
                    {
                        _firstItemInViewportPixelOffset = viewportSize - constraintSize;
                        viewportSize = constraintSize;
                        if (isHorizontal)
                        {
                            numberOfItemsInViewport -= 1 - (firstChildSize.Width - _firstItemInViewportPixelOffset) / firstChildSize.Width;
                        }
                        else
                        {
                            numberOfItemsInViewport -= 1 - (firstChildSize.Height - _firstItemInViewportPixelOffset) / firstChildSize.Height;
                        }
                    }
                    else
                    {
                        int insertAt = start.Offset == 0 ? start.Index : start.Index + 1;

                        for (int i = firstItemInViewportIndex - 1; i >= 0; i--)
                        {
                            UIElement child = (UIElement)generator.GenerateNext(out bool isNewlyRealized);
                            if (isNewlyRealized || !IsChildRealized(child, insertAt, children))
                            {
                                // Add newly created children to the panel
                                if (insertAt < 0)
                                {
                                    InsertInternalChild(0, child);
                                }
                                else
                                {
                                    InsertInternalChild(insertAt, child);
                                }

                                generator.PrepareItemContainer(child);
                            }
                            else
                            {
                                insertAt--;
                            }

                            child.Measure(childConstraint);

                            Size size = child.DesiredSize;
                            nvisible++;

                            firstItemInViewportIndex = i;

                            if (isHorizontal)
                            {
                                stackDesiredSize.Width += size.Width;
                                stackDesiredSize.Height = Math.Max(stackDesiredSize.Height, size.Height);
                                viewportSize += size.Width;

                                if (viewportSize >= constraint.Width)
                                {
                                    _firstItemInViewportPixelOffset = viewportSize - constraint.Width;
                                    numberOfItemsInViewport += 1 - (_firstItemInViewportPixelOffset / size.Width);
                                    break;
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

                                if (viewportSize >= constraint.Height)
                                {
                                    _firstItemInViewportPixelOffset = viewportSize - constraint.Height;
                                    numberOfItemsInViewport += 1 - (_firstItemInViewportPixelOffset / size.Height);
                                    break;
                                }
                                else
                                {
                                    numberOfItemsInViewport += 1.0;
                                }
                            }
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

    private static bool IsChildRealized(UIElement child, int index, List<UIElement> children)
    {
        return index switch
        {
            < 0 => false,
            0 => children[0] == child,
            _ => children[index - 1] == child || children[index] == child,
        };
    }

    private void CleanupContainers(ItemsControl owner, int firstItemInViewportIndex, int count)
    {
        List<UIElement> children = UnsafeGetChildren();
        IRecyclingItemContainerGenerator generator = ItemContainerGenerator as IRecyclingItemContainerGenerator;
        int last = firstItemInViewportIndex + count - 1;
        int index = 0;

        var pos = new GeneratorPosition(children.Count - 1, 0);
        while (pos.Index >= 0)
        {
            int item = generator.IndexFromGeneratorPosition(pos);
            UIElement child = children[pos.Index];

            if ((item < firstItemInViewportIndex || item > last) &&
                !((IGeneratorHost)owner).IsItemItsOwnContainer(owner.Items[item]) &&
                !child.IsKeyboardFocusWithin &&
                child != _bringIntoViewContainer &&
                NotifyCleanupItem(child, owner))
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
            Source = this
        };

        OnCleanUpVirtualizedItem(e);

        return !e.Cancel;
    }

    private (int Index, double Offset) ComputeFirstItemInViewportIndex(bool isHorizontal, int itemCount)
    {
        if (IsScrolling)
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

        return (0, 0);
    }

    private Size MeasureNonItemsHost(Size constraint)
    {
        Size stackDesiredSize = new();
        List<UIElement> children = UnsafeGetChildren();
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

    private void VerifyScrollingData(Size viewportSize, Size extentSize, Vector viewportOffset)
    {
        Debug.Assert(IsScrolling);

        // Detect changes to the viewportSize, extentSize, and computedViewportOffset
        bool viewportSizeChanged = !DoubleUtil.AreClose(viewportSize, _scrollData._viewport);
        bool extentSizeChanged = !DoubleUtil.AreClose(extentSize, _scrollData._extent);
        bool computedViewportOffsetChanged = !DoubleUtil.AreClose(viewportOffset, _scrollData._computedOffset);

        _scrollData._offset = viewportOffset;

        // Update data and fire scroll change notifications
        if (viewportSizeChanged || extentSizeChanged || computedViewportOffsetChanged)
        {
            Vector oldViewportOffset = _scrollData._computedOffset;
            Size oldViewportSize = _scrollData._viewport;

            _scrollData._viewport = viewportSize;
            _scrollData._extent = extentSize;
            _scrollData._computedOffset = viewportOffset;

            // Report changes to the viewportSize
            if (viewportSizeChanged)
            {
                OnViewportSizeChanged(oldViewportSize, viewportSize);
            }

            // Report changes to the computedViewportOffset
            if (computedViewportOffsetChanged)
            {
                OnViewportOffsetChanged(oldViewportOffset, viewportOffset);
            }

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
        List<UIElement> children = UnsafeGetChildren();

        if (IsScrolling)
        {
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

        List<UIElement> children = UnsafeGetChildren();
        for (int i = 0; i < _firstItemInViewportIndex && i < children.Count; i++)
        {
            UIElement child = children[i];
            offset += isHorizontal ? child.DesiredSize.Width : child.DesiredSize.Height;
        }

        return offset;
    }

    private Size ArrangeNonItemsHost(Size arrangeSize)
    {
        List<UIElement> children = UnsafeGetChildren();
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

        List<UIElement> children = UnsafeGetChildren();
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
            List<UIElement> children = UnsafeGetChildren();
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
    /// Called when the size of the viewport changes.
    /// </summary>
    /// <param name="oldViewportSize">
    /// The old size of the viewport.
    /// </param>
    /// <param name="newViewportSize">
    /// The new size of the viewport.
    /// </param>
    protected virtual void OnViewportSizeChanged(Size oldViewportSize, Size newViewportSize)
    {
    }

    /// <summary>
    /// Called when the offset of the viewport changes as a user scrolls through content.
    /// </summary>
    /// <param name="oldViewportOffset">
    /// The old offset of the viewport.
    /// </param>
    /// <param name="newViewportOffset">
    /// The new offset of the viewport.
    /// </param>
    protected virtual void OnViewportOffsetChanged(Vector oldViewportOffset, Vector newViewportOffset)
    {
    }

    /// <summary>
    /// Called when an item that is hosted by the <see cref="VirtualizingStackPanel"/> is re-virtualized.
    /// </summary>
    /// <param name="e">
    /// Data about the event.
    /// </param>
    protected virtual void OnCleanUpVirtualizedItem(CleanUpVirtualizedItemEventArgs e)
    {
        ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);
        itemsControl?.RaiseEvent(e);
    }

    /// <summary>
    /// Generates the item at the specified index and calls BringIntoView on it.
    /// </summary>
    /// <param name="index">
    /// Specify the item index that should become visible. This is the index into <see cref="ItemsControl.Items"/> collection.
    /// </param>
    protected override void BringIndexIntoView(int index)
    {
        ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);
        if (itemsControl is null)
        {
            return;
        }

        BringContainerIntoView(itemsControl, index);
    }

    // Realizes the container for the item at itemIndex (generating it if necessary) and brings it
    // into view by delegating to the container's BringIntoView, which routes through
    // IScrollInfo.MakeVisible for minimal scrolling.
    private void BringContainerIntoView(ItemsControl itemsControl, int itemIndex)
    {
        if (itemIndex < 0 || itemIndex >= itemsControl.Items.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(itemIndex));
        }

        EnsureGenerator();

        UIElement child;
        IItemContainerGenerator generator = ItemContainerGenerator;
        List<UIElement> children = UnsafeGetChildren();
        GeneratorPosition position = generator.GeneratorPositionFromIndex(itemIndex);
        int childIndex = position.Offset == 0 ? position.Index : position.Index + 1;

        using (generator.StartAt(position, GeneratorDirection.Forward, true))
        {
            child = (UIElement)generator.GenerateNext(out bool isNewlyRealized);
            if (child is not null && (isNewlyRealized || childIndex >= children.Count || children[childIndex] != child))
            {
                if (childIndex < children.Count)
                {
                    InsertInternalChild(childIndex, child);
                }
                else
                {
                    AddInternalChild(child);
                }

                generator.PrepareItemContainer(child);
            }
        }

        if (child is FrameworkElement childFE)
        {
            _bringIntoViewContainer = childFE;

            // Carefully remove the _bringIntoViewContainer after the storm of layouts to bring it into view has subsided
            Dispatcher.InvokeAsync(() => _bringIntoViewContainer = null, DispatcherPriority.Loaded);

            childFE.BringIntoView();
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
            Vector oldViewportOffset = _scrollData._offset;

            // Store the new offset
            _scrollData._offset.X = scrollX;

            // Report the change in offset
            OnViewportOffsetChanged(oldViewportOffset, _scrollData._offset);

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
            Vector oldViewportOffset = _scrollData._offset;

            // Store the new offset
            _scrollData._offset.Y = scrollY;

            // Report the change in offset
            OnViewportOffsetChanged(oldViewportOffset, _scrollData._offset);

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
        // The goal is to change offsets to bring the child into view, and return a rectangle in our space to make visible.
        // The rectangle we return is in the physical dimension the input target rect transformed into our space.
        // In the logical (stacking) dimension, it is our immediate child's rect.
        // Note: This code presently assumes we/children are layout clean.

        var newOffset = new Vector();
        var newRect = new Rect();
        Rect originalRect = rectangle;
        bool isHorizontal = Orientation == Orientation.Horizontal;

        // We can only work on visuals that are us or children.
        // An empty rect has no size or position. We can't meaningfully use it.
        if (rectangle.IsEmpty || visual is null || visual == this || !IsAncestorOf(visual))
        {
            return Rect.Empty;
        }

        // Compute the child's rect relative to (0,0) in our coordinate space.
        Matrix childTransform = visual.InternalTransformToAncestor(this);
        rectangle.Transform(childTransform);

        // We can't do any work unless we're scrolling.
        if (!IsScrolling)
        {
            return rectangle;
        }

        // Make ourselves visible in the non-stacking direction (physical/pixel based).
        MakeVisiblePhysicalHelper(rectangle, ref newOffset, ref newRect, !isHorizontal);

        // Bring the child containing the visual into view. OpenSilver only supports logical
        // (item based) scrolling in the stacking direction, so always use the logical helper here.
        int childIndex = FindChildLogicalIndex(visual);
        MakeVisibleLogicalHelper(childIndex, rectangle, ref newOffset, ref newRect);

        // We have computed the scrolling offsets; validate and scroll to them.
        newOffset.X = ScrollContentPresenter.CoerceOffset(newOffset.X, _scrollData._extent.Width, _scrollData._viewport.Width);
        newOffset.Y = ScrollContentPresenter.CoerceOffset(newOffset.Y, _scrollData._extent.Height, _scrollData._viewport.Height);

        if (!LayoutDoubleUtil.AreClose(newOffset.X, _scrollData._offset.X) ||
            !LayoutDoubleUtil.AreClose(newOffset.Y, _scrollData._offset.Y))
        {
            Vector oldOffset = _scrollData._offset;
            _scrollData._offset = newOffset;

            OnViewportOffsetChanged(oldOffset, newOffset);

            InvalidateMeasure();
            OnScrollChange();

            // When layout gets updated it may happen that the visual is obscured by a ScrollBar.
            // Call MakeVisible again to make sure the element is visible in this case.
            ScrollOwner?.MakeVisible(visual, originalRect);
        }

        return newRect;
    }

    // Finds the logical (item) index of the child container that is, or is an ancestor of, the given visual.
    // Returns -1 if no such child exists.
    private int FindChildLogicalIndex(UIElement visual)
    {
        List<UIElement> children = UnsafeGetChildren();
        for (int i = 0; i < children.Count; i++)
        {
            UIElement child = children[i];
            if (child == visual || child.IsAncestorOf(visual))
            {
                // When hosting items, the visual child index maps to an item index through the generator.
                // Otherwise (standalone scrolling panel) the logical offset is the visual child index itself.
                if (IsItemsHost && ItemContainerGenerator is IItemContainerGenerator generator)
                {
                    return generator.IndexFromGeneratorPosition(new GeneratorPosition(i, 0));
                }

                return i;
            }
        }

        return -1;
    }

    // Adjusts the offset in the non-stacking (physical/pixel) direction to bring the target rect into view.
    // This is very similar to the work that ScrollContentPresenter does for MakeVisible.
    private void MakeVisiblePhysicalHelper(Rect r, ref Vector newOffset, ref Rect newRect, bool isHorizontal)
    {
        double viewportOffset;
        double viewportSize;
        double targetRectOffset;
        double targetRectSize;

        if (isHorizontal)
        {
            viewportOffset = _scrollData._computedOffset.X;
            viewportSize = ViewportWidth;
            targetRectOffset = r.X;
            targetRectSize = r.Width;
        }
        else
        {
            viewportOffset = _scrollData._computedOffset.Y;
            viewportSize = ViewportHeight;
            targetRectOffset = r.Y;
            targetRectSize = r.Height;
        }

        targetRectOffset += viewportOffset;
        double minPhysicalOffset = ScrollContentPresenter.ComputeScrollOffsetWithMinimalScroll(
            viewportOffset, viewportOffset + viewportSize, targetRectOffset, targetRectOffset + targetRectSize);

        // Compute the visible rectangle of the child relative to the viewport.
        double start = targetRectOffset - minPhysicalOffset;
        double end = start + targetRectSize;

        double visibleStart = Math.Max(start, 0);
        double visibleEnd = Math.Max(Math.Min(end, viewportSize), visibleStart);

        if (isHorizontal)
        {
            newOffset.X = minPhysicalOffset;
            newRect.X = visibleStart;
            newRect.Width = visibleEnd - visibleStart;
        }
        else
        {
            newOffset.Y = minPhysicalOffset;
            newRect.Y = visibleStart;
            newRect.Height = visibleEnd - visibleStart;
        }
    }

    // Adjusts the offset in the stacking (logical/item) direction to bring the child at childIndex into view.
    private void MakeVisibleLogicalHelper(int childIndex, Rect r, ref Vector newOffset, ref Rect newRect)
    {
        bool fHorizontal = Orientation == Orientation.Horizontal;
        int firstChildInView;
        int viewportSize;
        double childOffsetWithinViewport = fHorizontal ? r.X : r.Y;

        if (fHorizontal)
        {
            firstChildInView = (int)_scrollData._computedOffset.X;
            viewportSize = (int)_scrollData._viewport.Width;
        }
        else
        {
            firstChildInView = (int)_scrollData._computedOffset.Y;
            viewportSize = (int)_scrollData._viewport.Height;
        }

        int newFirstChild = firstChildInView;

        // If the target child is before the current viewport, move the viewport to put the child at the top.
        if (childIndex < firstChildInView)
        {
            childOffsetWithinViewport = 0;
            newFirstChild = childIndex;
        }
        // If the target child is after the current viewport, move the viewport to put the child at the bottom.
        else if (childIndex > firstChildInView + Math.Max(viewportSize - 1, 0))
        {
            newFirstChild = childIndex - viewportSize + 1;
            double pixelSize = fHorizontal ? ActualWidth : ActualHeight;
            childOffsetWithinViewport = pixelSize * (1.0 - (1.0 / viewportSize));
        }

        if (fHorizontal)
        {
            newOffset.X = newFirstChild;
            newRect.X = childOffsetWithinViewport;
            newRect.Width = r.Width;
        }
        else
        {
            newOffset.Y = newFirstChild;
            newRect.Y = childOffsetWithinViewport;
            newRect.Height = r.Height;
        }
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
