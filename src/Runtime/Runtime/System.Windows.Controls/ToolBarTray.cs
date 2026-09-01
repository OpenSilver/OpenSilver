// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CSHTML5.Internal;
using OpenSilver.Internal;
using OpenSilver.Internal.Controls;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup; // IAddChild, ContentPropertyAttribute
using System.Windows.Media;

namespace System.Windows.Controls;

/// <summary>
/// Represents the container that handles the layout of a <see cref="ToolBar"/>.
/// </summary>
[ContentProperty(nameof(ToolBars))]
public class ToolBarTray : FrameworkElement
{
    //-------------------------------------------------------------------
    //
    //  Constructors
    //
    //-------------------------------------------------------------------

    #region Constructors

    static ToolBarTray()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolBarTray), new FrameworkPropertyMetadata(typeof(ToolBarTray)));

        EventManager.RegisterClassHandler(typeof(ToolBarTray), Thumb.DragStartedEvent, new DragStartedEventHandler(OnThumbDragStarted), true);
        EventManager.RegisterClassHandler(typeof(ToolBarTray), Thumb.DragDeltaEvent, new DragDeltaEventHandler(OnThumbDragDelta));
        KeyboardNavigation.ControlTabNavigationProperty.OverrideMetadata(typeof(ToolBarTray), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ToolBarTray"/> class.
    /// </summary>
    public ToolBarTray() { }

    #endregion

    //-------------------------------------------------------------------
    //
    //  Properties
    //
    //-------------------------------------------------------------------

    #region Properties

    /// <summary>
    /// Identifies the <see cref="Background"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BackgroundProperty =
        Panel.BackgroundProperty.AddOwner(
            typeof(ToolBarTray),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnBackgroundChanged)
            {
                MethodToUpdateDom2 = static (d, oldValue, newValue) => ((ToolBarTray)d).SetBackground((Brush)newValue),
            });

    /// <summary>
    /// Gets or sets a brush to use for the background color of the <see cref="ToolBarTray"/>.
    /// </summary>
    /// <returns>
    /// A brush to use for the background color of the <see cref="ToolBarTray"/>.
    /// </returns>
    public Brush Background
    {
        get => (Brush)GetValue(BackgroundProperty);
        set => SetValueInternal(BackgroundProperty, value);
    }

    private static void OnBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var toolBarTray = (ToolBarTray)d;

        toolBarTray.RefreshBackgroundOnSizeChange = e.NewValue is LinearGradientBrush;

        toolBarTray._weakEventToken?.Dispose();
        toolBarTray._weakEventToken = null;

        if (e.NewValue is Brush newBrush && !newBrush.IsSealed)
        {
            toolBarTray._weakEventToken = WeakEvent.Subscribe<ToolBarTray, Brush, EventArgs>(
                toolBarTray,
                newBrush,
                static (instance, sender, args) => instance.OnBackgroundChanged(sender, args),
                static (handler, source) => source.Changed -= new EventHandler(handler),
                static (handler, source) => source.Changed += new EventHandler(handler));
        }

        // Update pointer events
        toolBarTray.CoerceIsHitTestable();
    }

    private void OnBackgroundChanged(object sender, EventArgs e)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
        {
            this.SetBackground((Brush)sender);
        }
    }

    /// <summary>
    /// Identifies the <see cref="Orientation"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty OrientationProperty =
        DependencyProperty.Register(
            nameof(Orientation),
            typeof(Orientation),
            typeof(ToolBarTray),
            new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsParentMeasure, OnOrientationPropertyChanged),
            ScrollBar.IsValidOrientation);

    /// <summary>
    /// Specifies the orientation of a <see cref="ToolBarTray"/>.
    /// </summary>
    /// <returns>
    /// One of the <see cref="Controls.Orientation"/> values. The default is <see cref="Orientation.Horizontal"/>.
    /// </returns>
    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        set => SetValueInternal(OrientationProperty, value);
    }

    // Then ToolBarTray Orientation is changing we need to invalidate its ToolBars Orientation
    private static void OnOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        Collection<ToolBar> toolbarCollection = ((ToolBarTray)d).ToolBars;
        for (int i = 0; i < toolbarCollection.Count; i++)
        {
            toolbarCollection[i].CoerceValue(ToolBar.OrientationProperty);
        }
    }

    /// <summary>
    /// Identifies the <see cref="IsLocked"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsLockedProperty =
        DependencyProperty.RegisterAttached(
            nameof(IsLocked),
            typeof(bool),
            typeof(ToolBarTray),
            new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>
    /// Gets or sets a value that indicates whether a <see cref="ToolBar"/> can be moved inside 
    /// a <see cref="ToolBarTray"/>.
    /// </summary>
    /// <returns>
    /// true if the toolbar cannot be moved inside the toolbar tray; otherwise, false. The default 
    /// is false.
    /// </returns>
    public bool IsLocked
    {
        get => (bool)GetValue(IsLockedProperty);
        set => SetValueInternal(IsLockedProperty, value);
    }

    /// <summary>
    /// Writes the value of the <see cref="IsLocked"/> property to the specified element.
    /// </summary>
    /// <param name="element">
    /// The element to write the property to.
    /// </param>
    /// <param name="value">
    /// The property value to set.
    /// </param>
    public static void SetIsLocked(DependencyObject element, bool value)
    {
        ArgumentNullException.ThrowIfNull(element);
        element.SetValueInternal(IsLockedProperty, value);
    }

    /// <summary>
    /// Reads the value of the <see cref="IsLocked"/> property from the specified element.
    /// </summary>
    /// <param name="element">
    /// The element from which to read the property.
    /// </param>
    /// <returns>
    /// true if the toolbar cannot be moved inside the toolbar tray; otherwise, false. The 
    /// default is false.
    /// </returns>
    public static bool GetIsLocked(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return (bool)element.GetValue(IsLockedProperty);
    }

    /// <summary>
    /// Gets the collection of <see cref="ToolBar"/> elements in the <see cref="ToolBarTray"/>.
    /// </summary>
    /// <returns>
    /// A collection of <see cref="ToolBar"/> objects.
    /// </returns>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public Collection<ToolBar> ToolBars => _toolBarsCollection ??= new ToolBarCollection(this);

    private sealed class ToolBarCollection : Collection<ToolBar>
    {
        public ToolBarCollection(ToolBarTray parent)
        {
            _parent = parent;
        }

        protected override void InsertItem(int index, ToolBar toolBar)
        {
            base.InsertItem(index, toolBar);

            _parent.AddLogicalChild(toolBar);
            _parent.AddVisualChild(toolBar);
            _parent.InvalidateMeasure();
        }

        protected override void SetItem(int index, ToolBar toolBar)
        {
            ToolBar currentToolBar = Items[index];
            if (toolBar != currentToolBar)
            {
                base.SetItem(index, toolBar);

                // remove old item visual and logical links
                _parent.RemoveVisualChild(currentToolBar);
                _parent.RemoveLogicalChild(currentToolBar);

                // add new item visual and logical links
                _parent.AddLogicalChild(toolBar);
                _parent.AddVisualChild(toolBar);
                _parent.InvalidateMeasure();
            }
        }

        protected override void RemoveItem(int index)
        {
            ToolBar currentToolBar = this[index];
            base.RemoveItem(index);

            // remove old item visual and logical links
            _parent.RemoveVisualChild(currentToolBar);
            _parent.RemoveLogicalChild(currentToolBar);
            _parent.InvalidateMeasure();
        }

        protected override void ClearItems()
        {
            int count = Count;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    ToolBar currentToolBar = this[i];
                    _parent.RemoveVisualChild(currentToolBar);
                    _parent.RemoveLogicalChild(currentToolBar);
                }
                _parent.InvalidateMeasure();
            }

            base.ClearItems();
        }


        // Ref to a visual/logical ToolBarTray parent
        private readonly ToolBarTray _parent;
    }

    #endregion Properties


    //-------------------------------------------------------------------
    //
    //  Public Methods
    //
    //-------------------------------------------------------------------

    #region Public Methods

    /// <summary>
    /// Gets an enumerator to the logical child elements of a <see cref="ToolBarTray"/>.
    /// </summary>
    /// <returns>
    /// An enumerator to the children of a <see cref="ToolBarTray"/> element.
    /// </returns>
    protected internal override IEnumerator LogicalChildren
    {
        get
        {
            if (this.VisualChildrenCount == 0)
            {
                return EmptyEnumerator.Instance;
            }

            return this.ToolBars.GetEnumerator();
        }
    }

    #endregion Public Methods


    //-------------------------------------------------------------------
    //
    //  Protected Methods
    //
    //-------------------------------------------------------------------

    #region Protected Methods

    /// <inheritdoc />
    protected internal override void OnRenderSizeChanged(SizeChangedInfo info)
    {
        base.OnRenderSizeChanged(info);

        if (RefreshBackgroundOnSizeChange && INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
        {
            this.SetBackground(Background);
        }
    }

    /// <summary>
    /// Called to remeasure a <see cref="ToolBarTray"/>.
    /// </summary>
    /// <param name="constraint">
    /// The measurement constraints; a <see cref="ToolBarTray"/> cannot return a size larger 
    /// than the constraint.
    /// </param>
    /// <returns>
    /// The size of the control.
    /// </returns>
    protected override Size MeasureOverride(Size constraint)
    {
        GenerateBands();

        Size toolBarTrayDesiredSize = new Size();
        int bandIndex;
        int toolBarIndex;
        bool fHorizontal = Orientation == Orientation.Horizontal;
        Size childConstraint = new Size(double.PositiveInfinity, double.PositiveInfinity);

        for (bandIndex = 0; bandIndex < _bands.Count; bandIndex++)
        {
            // Calculate the available size before we measure the children.
            // remainingLength is the constraint minus sum of all minimum sizes
            double remainingLength = fHorizontal ? constraint.Width : constraint.Height;
            List<ToolBar> band = _bands[bandIndex].Band;
            double bandThickness = 0d;
            double bandLength = 0d;
            for (toolBarIndex = 0; toolBarIndex < band.Count; toolBarIndex++)
            {
                ToolBar toolBar = band[toolBarIndex];
                remainingLength -= toolBar.MinLength;
                if (DoubleUtil.LessThan(remainingLength, 0))
                {
                    remainingLength = 0;
                    break;
                }
            }

            // Measure all children passing the remainingLength as a constraint
            for (toolBarIndex = 0; toolBarIndex < band.Count; toolBarIndex++)
            {
                ToolBar toolBar = band[toolBarIndex];
                remainingLength += toolBar.MinLength;
                if (fHorizontal)
                    childConstraint.Width = remainingLength;
                else
                    childConstraint.Height = remainingLength;
                toolBar.Measure(childConstraint);
                bandThickness = Math.Max(bandThickness, fHorizontal ? toolBar.DesiredSize.Height : toolBar.DesiredSize.Width);
                bandLength += fHorizontal ? toolBar.DesiredSize.Width : toolBar.DesiredSize.Height;
                remainingLength -= fHorizontal ? toolBar.DesiredSize.Width : toolBar.DesiredSize.Height;
                if (DoubleUtil.LessThan(remainingLength, 0))
                {
                    remainingLength = 0;
                }
            }

            // Store band thickness in the BandInfo property
            _bands[bandIndex].Thickness = bandThickness;

            if (fHorizontal)
            {
                toolBarTrayDesiredSize.Height += bandThickness;
                toolBarTrayDesiredSize.Width = Math.Max(toolBarTrayDesiredSize.Width, bandLength);
            }
            else
            {
                toolBarTrayDesiredSize.Width += bandThickness;
                toolBarTrayDesiredSize.Height = Math.Max(toolBarTrayDesiredSize.Height, bandLength);
            }
        }

        return toolBarTrayDesiredSize;
    }

    /// <summary>
    /// Called to arrange and size its <see cref="ToolBar"/> children.
    /// </summary>
    /// <param name="arrangeSize">
    /// The size that the <see cref="ToolBarTray"/> assumes to position its children.
    /// </param>
    /// <returns>
    /// The size of the control.
    /// </returns>
    protected override Size ArrangeOverride(Size arrangeSize)
    {
        int bandIndex;
        int toolBarIndex;
        bool fHorizontal = Orientation == Orientation.Horizontal;
        Rect rcChild = new Rect();

        for (bandIndex = 0; bandIndex < _bands.Count; bandIndex++)
        {
            List<ToolBar> band = _bands[bandIndex].Band;

            double bandThickness = _bands[bandIndex].Thickness;

            if (fHorizontal)
                rcChild.X = 0;
            else
                rcChild.Y = 0;

            for (toolBarIndex = 0; toolBarIndex < band.Count; toolBarIndex++)
            {
                ToolBar toolBar = band[toolBarIndex];
                Size toolBarArrangeSize = new Size(fHorizontal ? toolBar.DesiredSize.Width : bandThickness, fHorizontal ? bandThickness : toolBar.DesiredSize.Height);
                rcChild.Size = toolBarArrangeSize;
                toolBar.Arrange(rcChild);
                if (fHorizontal)
                    rcChild.X += toolBarArrangeSize.Width;
                else
                    rcChild.Y += toolBarArrangeSize.Height;
            }

            if (fHorizontal)
                rcChild.Y += bandThickness;
            else
                rcChild.X += bandThickness;
        }

        return arrangeSize;
    }

    /// <summary>
    /// Gets the number of children that are currently visible.
    /// </summary>
    /// <returns>
    /// The number of visible <see cref="ToolBar"/> objects in the <see cref="ToolBarTray"/>.
    /// </returns>
    protected override int VisualChildrenCount
    {
        get
        {
            if (_toolBarsCollection == null)
            {
                return 0;
            }
            else
            {
                return _toolBarsCollection.Count;
            }
        }
    }

    /// <summary>
    /// Gets the index number of the visible child.
    /// </summary>
    /// <param name="index">
    /// Index of the visual child.
    /// </param>
    /// <returns>
    /// The index number of the visible child.
    /// </returns>
    protected override UIElement GetVisualChild(int index)
    {
        if (_toolBarsCollection is null)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        return _toolBarsCollection[index];
    }

    #endregion Protected Methods

    //-------------------------------------------------------------------
    //
    //  Private Methods
    //
    //-------------------------------------------------------------------

    #region Private Methods

    internal sealed override bool EnablePointerEventsCore => Background is not null;

    private (double HorizontalChange, double VerticalChange) _dragDelta;

    internal void ResetDragData()
    {
        _dragDelta = (0, 0);
    }

    private static void OnThumbDragStarted(object sender, DragStartedEventArgs e)
    {
        ToolBarTray toolBarTray = (ToolBarTray)sender;

        toolBarTray.ResetDragData();
    }

    // Event handler to listen to thumb events.
    private static void OnThumbDragDelta(object sender, DragDeltaEventArgs e)
    {
        ToolBarTray toolBarTray = (ToolBarTray)sender;

        toolBarTray._dragDelta.HorizontalChange += e.HorizontalChange;
        toolBarTray._dragDelta.VerticalChange += e.VerticalChange;

        // Don't move toolbars if IsLocked == true
        if (toolBarTray.IsLocked)
            return;

        toolBarTray.ProcessThumbDragDelta(e);
    }

    private void ProcessThumbDragDelta(DragDeltaEventArgs e)
    {
        // Process thumb event only if Thumb styled parent is a ToolBar under the TollBarTray
        Thumb thumb = e.OriginalSource as Thumb;
        if (thumb != null)
        {
            ToolBar toolBar = thumb.TemplatedParent as ToolBar;
            if (toolBar != null && toolBar.Parent == this)
            {
                // _bandsDirty would be true at this time only when a Measure gets
                // skipped between two mouse moves. Ideally that should not happen
                // but VS has proved that it can. Hence making the code more robust.
                // Uncomment the line below if the measure skip issue ever gets fixed.
                // Debug.Assert(!_bandsDirty, "Bands should not be dirty at this point");
                if (_bandsDirty)
                {
                    GenerateBands();
                }

                bool fHorizontal = Orientation == Orientation.Horizontal;
                int currentBand = toolBar.Band;
                Point pointRelativeToToolBarTray = Mouse.PrimaryDevice.GetPosition((IInputElement)this);
                Point pointRelativeToToolBar = TransformPointToToolBar(toolBar, pointRelativeToToolBarTray);
                int hittestBand = GetBandFromOffset(fHorizontal ? pointRelativeToToolBarTray.Y : pointRelativeToToolBarTray.X);
                double newPosition;
                double thumbChange = fHorizontal ? _dragDelta.HorizontalChange : _dragDelta.VerticalChange;
                double toolBarPosition;
                if (fHorizontal)
                {
                    toolBarPosition = pointRelativeToToolBarTray.X - pointRelativeToToolBar.X;
                }
                else
                {
                    toolBarPosition = pointRelativeToToolBarTray.Y - pointRelativeToToolBar.Y;
                }
                newPosition = toolBarPosition + thumbChange; // New toolBar position

                // Move within the band
                if (hittestBand == currentBand)
                {
                    List<ToolBar> band = _bands[currentBand].Band;
                    int toolBarIndex = toolBar.BandIndex;

                    // Move ToolBar within the band
                    if (DoubleUtil.LessThan(thumbChange, 0)) // Move left/up
                    {
                        double toolBarsTotalMinimum = ToolBarsTotalMinimum(band, 0, toolBarIndex - 1);
                        // Check if minimized toolbars will fit in the range
                        if (DoubleUtil.LessThanOrClose(toolBarsTotalMinimum, newPosition))
                        {
                            ShrinkToolBars(band, 0, toolBarIndex - 1, -thumbChange);
                        }
                        else if (toolBarIndex > 0) // Swap toolbars
                        {
                            ToolBar prevToolBar = band[toolBarIndex - 1];
                            Point pointRelativeToPreviousToolBar = TransformPointToToolBar(prevToolBar, pointRelativeToToolBarTray);
                            // if pointer in on the left side of previous toolbar
                            if (DoubleUtil.LessThan(fHorizontal ? pointRelativeToPreviousToolBar.X : pointRelativeToPreviousToolBar.Y, 0))
                            {
                                prevToolBar.BandIndex = toolBarIndex;
                                band[toolBarIndex] = prevToolBar;

                                toolBar.BandIndex = toolBarIndex - 1;
                                band[toolBarIndex - 1] = toolBar;

                                if (toolBarIndex + 1 == band.Count) // If toolBar was the last item in the band
                                {
                                    prevToolBar.ClearValue(fHorizontal ? WidthProperty : HeightProperty);
                                }
                            }
                            else
                            { // Move to the left/up and shring the other toolbars
                                if (fHorizontal)
                                {
                                    if (DoubleUtil.LessThan(toolBarsTotalMinimum, pointRelativeToToolBarTray.X - pointRelativeToToolBar.X))
                                    {
                                        ShrinkToolBars(band, 0, toolBarIndex - 1, pointRelativeToToolBarTray.X - pointRelativeToToolBar.X - toolBarsTotalMinimum);
                                    }
                                }
                                else
                                {
                                    if (DoubleUtil.LessThan(toolBarsTotalMinimum, pointRelativeToToolBarTray.Y - pointRelativeToToolBar.Y))
                                    {
                                        ShrinkToolBars(band, 0, toolBarIndex - 1, pointRelativeToToolBarTray.Y - pointRelativeToToolBar.Y - toolBarsTotalMinimum);
                                    }
                                }
                            }
                        }
                    }
                    else // Move right/down
                    {
                        double toolBarsTotalMaximum = ToolBarsTotalMaximum(band, 0, toolBarIndex - 1);

                        if (DoubleUtil.GreaterThan(toolBarsTotalMaximum, newPosition))
                        {
                            ExpandToolBars(band, 0, toolBarIndex - 1, thumbChange);
                        }
                        else
                        {
                            if (toolBarIndex < band.Count - 1) // Swap toolbars
                            {
                                ToolBar nextToolBar = band[toolBarIndex + 1];
                                Point pointRelativeToNextToolBar = TransformPointToToolBar(nextToolBar, pointRelativeToToolBarTray);
                                // if pointer in on the right side of next toolbar
                                if (DoubleUtil.GreaterThanOrClose(fHorizontal ? pointRelativeToNextToolBar.X : pointRelativeToNextToolBar.Y, 0))
                                {
                                    nextToolBar.BandIndex = toolBarIndex;
                                    band[toolBarIndex] = nextToolBar;

                                    toolBar.BandIndex = toolBarIndex + 1;
                                    band[toolBarIndex + 1] = toolBar;
                                    if (toolBarIndex + 2 == band.Count) // If toolBar becomes the last item in the band
                                    {
                                        toolBar.ClearValue(fHorizontal ? WidthProperty : HeightProperty);
                                    }
                                }
                                else
                                {
                                    ExpandToolBars(band, 0, toolBarIndex - 1, thumbChange);
                                }
                            }
                            else
                            {
                                ExpandToolBars(band, 0, toolBarIndex - 1, thumbChange);
                            }
                        }
                    }
                }
                else // Move ToolBar to another band
                {
                    _bandsDirty = true;
                    toolBar.Band = hittestBand;
                    toolBar.ClearValue(fHorizontal ? WidthProperty : HeightProperty);

                    // move to another existing band
                    if (hittestBand >= 0 && hittestBand < _bands.Count)
                    {
                        MoveToolBar(toolBar, hittestBand, newPosition);
                    }

                    List<ToolBar> oldBand = _bands[currentBand].Band;
                    // currentBand should restore sizes to Auto
                    for (int i = 0; i < oldBand.Count; i++)
                    {
                        ToolBar currentToolBar = oldBand[i];
                        currentToolBar.ClearValue(fHorizontal ? WidthProperty : HeightProperty);
                    }
                }

                e.Handled = true;
            }
        }
    }

    private Point TransformPointToToolBar(ToolBar toolBar, Point point)
    {
        Matrix transform = InternalTransformToDescendant(toolBar);
        return transform.Transform(point);
    }

    private void ShrinkToolBars(List<ToolBar> band, int startIndex, int endIndex, double shrinkAmount)
    {
        if (Orientation == Orientation.Horizontal)
        {
            for (int i = endIndex; i >= startIndex; i--)
            {
                ToolBar toolBar = band[i];
                if (DoubleUtil.GreaterThanOrClose(toolBar.RenderSize.Width - shrinkAmount, toolBar.MinLength))
                {
                    toolBar.Width = toolBar.RenderSize.Width - shrinkAmount;
                    break;
                }
                else
                {
                    toolBar.Width = toolBar.MinLength;
                    shrinkAmount -= toolBar.RenderSize.Width - toolBar.MinLength;
                }
            }
        }
        else
        {
            for (int i = endIndex; i >= startIndex; i--)
            {
                ToolBar toolBar = band[i];
                if (DoubleUtil.GreaterThanOrClose(toolBar.RenderSize.Height - shrinkAmount, toolBar.MinLength))
                {
                    toolBar.Height = toolBar.RenderSize.Height - shrinkAmount;
                    break;
                }
                else
                {
                    toolBar.Height = toolBar.MinLength;
                    shrinkAmount -= toolBar.RenderSize.Height - toolBar.MinLength;
                }
            }
        }
    }

    private double ToolBarsTotalMinimum(List<ToolBar> band, int startIndex, int endIndex)
    {
        double totalMinLenght = 0d;
        for (int i = startIndex; i <= endIndex; i++)
        {
            totalMinLenght += band[i].MinLength;
        }
        return totalMinLenght;
    }

    private void ExpandToolBars(List<ToolBar> band, int startIndex, int endIndex, double expandAmount)
    {
        if (Orientation == Orientation.Horizontal)
        {
            for (int i = endIndex; i >= startIndex; i--)
            {
                ToolBar toolBar = band[i];
                if (DoubleUtil.LessThanOrClose(toolBar.RenderSize.Width + expandAmount, toolBar.MaxLength))
                {
                    toolBar.Width = toolBar.RenderSize.Width + expandAmount;
                    break;
                }
                else
                {
                    toolBar.Width = toolBar.MaxLength;
                    expandAmount -= toolBar.MaxLength - toolBar.RenderSize.Width;
                }
            }
        }
        else
        {
            for (int i = endIndex; i >= startIndex; i--)
            {
                ToolBar toolBar = band[i];
                if (DoubleUtil.LessThanOrClose(toolBar.RenderSize.Height + expandAmount, toolBar.MaxLength))
                {
                    toolBar.Height = toolBar.RenderSize.Height + expandAmount;
                    break;
                }
                else
                {
                    toolBar.Height = toolBar.MaxLength;
                    expandAmount -= toolBar.MaxLength - toolBar.RenderSize.Height;
                }
            }
        }
    }

    private double ToolBarsTotalMaximum(List<ToolBar> band, int startIndex, int endIndex)
    {
        double totalMaxLength = 0d;
        for (int i = startIndex; i <= endIndex; i++)
        {
            totalMaxLength += band[i].MaxLength;
        }
        return totalMaxLength;
    }

    private void MoveToolBar(ToolBar toolBar, int newBandNumber, double position)
    {
        int i;
        bool fHorizontal = Orientation == Orientation.Horizontal;

        List<ToolBar> newBand = _bands[newBandNumber].Band;
        // calculate the new BandIndex where toolBar should insert
        // calculate Width (layout) of the items before the toolBar
        if (DoubleUtil.LessThanOrClose(position, 0))
        {
            toolBar.BandIndex = -1; // This will position toolBar at the first place
        }
        else
        {
            double toolBarOffset = 0d;
            int newToolBarIndex = -1;
            for (i = 0; i < newBand.Count; i++)
            {
                ToolBar currentToolBar = newBand[i];
                if (newToolBarIndex == -1)
                {
                    toolBarOffset += fHorizontal ? currentToolBar.RenderSize.Width : currentToolBar.RenderSize.Height; // points at the end of currentToolBar
                    if (DoubleUtil.GreaterThan(toolBarOffset, position))
                    {
                        newToolBarIndex = i + 1;
                        toolBar.BandIndex = newToolBarIndex;
                        // Update the currentToolBar width
                        if (fHorizontal)
                            currentToolBar.Width = Math.Max(currentToolBar.MinLength, currentToolBar.RenderSize.Width - toolBarOffset + position);
                        else
                            currentToolBar.Height = Math.Max(currentToolBar.MinLength, currentToolBar.RenderSize.Height - toolBarOffset + position);
                    }
                }
                else // After we insert the toolBar we need to increase the indexes
                {
                    currentToolBar.BandIndex = i + 1;
                }
            }
            if (newToolBarIndex == -1)
            {
                toolBar.BandIndex = i;
            }
        }
    }

    private int GetBandFromOffset(double toolBarOffset)
    {
        if (DoubleUtil.LessThan(toolBarOffset, 0))
            return -1;

        double bandOffset = 0d;
        for (int i = 0; i < _bands.Count; i++)
        {
            bandOffset += _bands[i].Thickness;
            if (DoubleUtil.GreaterThan(bandOffset, toolBarOffset))
                return i;
        }

        return _bands.Count;
    }

    #region Generate and Normalize bands

    // Generate all bands and normalize Band and BandIndex properties
    /// All ToolBars with the same Band are places in one band. After that they are sorted by BandIndex.
    private void GenerateBands()
    {
        if (!IsBandsDirty())
            return;

        Collection<ToolBar> toolbarCollection = ToolBars;

        _bands.Clear();
        for (int i = 0; i < toolbarCollection.Count; i++)
        {
            InsertBand(toolbarCollection[i], i);
        }

        // Normalize bands (make Band and BandIndex property 0,1,2,...)
        for (int bandIndex = 0; bandIndex < _bands.Count; bandIndex++)
        {
            List<ToolBar> band = _bands[bandIndex].Band;
            for (int toolBarIndex = 0; toolBarIndex < band.Count; toolBarIndex++)
            {
                ToolBar toolBar = band[toolBarIndex];
                // This will cause measure/arrange if some property changes
                toolBar.Band = bandIndex;
                toolBar.BandIndex = toolBarIndex;
            }
        }
        _bandsDirty = false;
    }

    // Verify is all toolbars are normalized (sorted in _bands by Band and BandIndex properties)
    private bool IsBandsDirty()
    {
        if (_bandsDirty)
            return true;

        int totalNumber = 0;
        Collection<ToolBar> toolbarCollection = ToolBars;
        for (int bandIndex = 0; bandIndex < _bands.Count; bandIndex++)
        {
            List<ToolBar> band = _bands[bandIndex].Band;
            for (int toolBarIndex = 0; toolBarIndex < band.Count; toolBarIndex++)
            {
                ToolBar toolBar = band[toolBarIndex];
                if (toolBar.Band != bandIndex || toolBar.BandIndex != toolBarIndex || !toolbarCollection.Contains(toolBar))
                    return true;
            }
            totalNumber += band.Count;
        }

        return totalNumber != toolbarCollection.Count;
    }

    // if toolBar.Band does not exist in bands collection when we create a new band
    private void InsertBand(ToolBar toolBar, int toolBarIndex)
    {
        int bandNumber = toolBar.Band;
        for (int i = 0; i < _bands.Count; i++)
        {
            int currentBandNumber = _bands[i].Band[0].Band;
            if (bandNumber == currentBandNumber)
                return;
            if (bandNumber < currentBandNumber)
            {
                // Band number does not exist - Insert
                _bands.Insert(i, CreateBand(toolBarIndex));
                return;
            }
        }

        // Band number does not exist - Add band at trhe end
        _bands.Add(CreateBand(toolBarIndex));
    }

    // Create new band and add all toolbars with the same Band and toolbar with index startIndex
    private BandInfo CreateBand(int startIndex)
    {
        Collection<ToolBar> toolbarCollection = ToolBars;
        BandInfo bandInfo = new BandInfo();
        ToolBar toolBar = toolbarCollection[startIndex];
        bandInfo.Band.Add(toolBar);
        int bandNumber = toolBar.Band;
        for (int i = startIndex + 1; i < toolbarCollection.Count; i++)
        {
            toolBar = toolbarCollection[i];
            if (bandNumber == toolBar.Band)
                InsertToolBar(toolBar, bandInfo.Band);
        }
        return bandInfo;
    }

    // Insert toolbar into band list so band remains sorted
    private void InsertToolBar(ToolBar toolBar, List<ToolBar> band)
    {
        for (int i = 0; i < band.Count; i++)
        {
            if (toolBar.BandIndex < band[i].BandIndex)
            {
                band.Insert(i, toolBar);
                return;
            }
        }
        band.Add(toolBar);
    }

    #endregion Generate and Normalize bands

    #endregion

    //-------------------------------------------------------------------
    //
    //  Private classes
    //
    //-------------------------------------------------------------------

    #region Private classes

    private sealed class BandInfo
    {
        public BandInfo() { }

        public List<ToolBar> Band { get; } = [];

        public double Thickness { get; set; }
    }

    #endregion

    #region Private members

    // ToolBarTray generates list of bands depend on ToolBar.Band property.
    // Each band is a list of toolbars sorted by ToolBar.BandIndex property.
    private readonly List<BandInfo> _bands = [];
    private bool _bandsDirty = true;
    private ToolBarCollection _toolBarsCollection = null;
    private WeakEventToken _weakEventToken;

    #endregion
}