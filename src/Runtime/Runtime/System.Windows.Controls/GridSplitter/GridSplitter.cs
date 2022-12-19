// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections;
using System.Diagnostics;
using System.Windows.Input;

#if MIGRATION
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
#else
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using MouseEventArgs = Windows.UI.Xaml.Input.PointerRoutedEventArgs;
using KeyEventArgs = Windows.UI.Xaml.Input.KeyRoutedEventArgs;
using Key = Windows.System.VirtualKey;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
/// <summary>
/// Represents a control that redistributes space between the rows of
/// columns of a <see cref="T:System.Windows.Controls.Grid" /> control.
/// </summary>
/// <QualityBand>Mature</QualityBand>
    [TemplatePart(Name = GridSplitter.ElementHorizontalTemplateName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = GridSplitter.ElementVerticalTemplateName, Type = typeof(FrameworkElement))]
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateMouseOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateUnfocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateFocused, GroupName = VisualStates.GroupFocus)]
    [StyleTypedProperty(Property = "PreviewStyle", StyleTargetType = typeof(Control))]
    public partial class GridSplitter : Control
    {
        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal const string ElementHorizontalTemplateName = "HorizontalTemplate";

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal const string ElementVerticalTemplateName = "VerticalTemplate";

        /// <summary>
        /// Gets or sets Inherited code: Requires comment.
        /// </summary>
        internal FrameworkElement ElementHorizontalTemplateFrameworkElement { get; set; }

        /// <summary>
        /// Gets or sets Inherited code: Requires comment.
        /// </summary>
        internal FrameworkElement ElementVerticalTemplateFrameworkElement { get; set; }
        
        /// <summary>
        /// Identifies the
        /// <see cref="P:System.Windows.Controls.GridSplitter.ShowsPreview" />
        /// dependency property.
        /// </summary>
        /// <value>
        /// An identifier for the
        /// <see cref="P:System.Windows.Controls.GridSplitter.ShowsPreview" />
        /// dependency property.
        /// </value>
        public static readonly DependencyProperty ShowsPreviewProperty =
            DependencyProperty.Register(
                "ShowsPreview",
                typeof(bool),
                typeof(GridSplitter),
                null);

        /// <summary>
        /// Identifies the
        /// <see cref="P:System.Windows.Controls.GridSplitter.PreviewStyle" />
        /// dependency property.
        /// </summary>
        /// <value>
        /// An identifier for the
        /// <see cref="P:System.Windows.Controls.GridSplitter.PreviewStyle" />
        /// dependency property.
        /// </value>
        public static readonly DependencyProperty PreviewStyleProperty =
            DependencyProperty.Register(
                "PreviewStyle",
                typeof(Style),
                typeof(GridSplitter),
                null);

        /// <summary>
        /// Called when the IsEnabled property changes.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Property changed args.</param>
        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Debug.Assert(e.NewValue is bool, "The new value should be a boolean!");
            bool isEnabled = (bool)e.NewValue;

            if (!isEnabled)
            {
                _isMouseOver = false;
            }
            ChangeVisualState();
        }

        /// <summary>
        /// Gets or sets the resize data.  This is null unless a resize
        /// operation is in progress.
        /// </summary>
        internal ResizeData ResizeDataInternal { get; set; }

        /// <summary>
        /// Is Null until a resize operation is initiated with ShowsPreview ==
        /// True, then it persists for the life of the GridSplitter.
        /// </summary>
        private Canvas _previewLayer;

        /// <summary>
        /// Is initialized in the constructor.
        /// </summary>
        private DragValidator _dragValidator;

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private GridResizeDirection _currentGridResizeDirection = GridResizeDirection.Auto;

        /// <summary>
        /// Holds the state for whether the mouse is over the control or not.
        /// </summary>
        private bool _isMouseOver;

        /// <summary>
        /// Default increment parameter.
        /// </summary>
        private const double DragIncrement = 1.0;

        /// <summary>
        /// Default increment parameter.
        /// </summary>
        private const double KeyboardIncrement = 10.0;
        
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Controls.GridSplitter" /> class.
        /// </summary>
        public GridSplitter()
        {
            this.IsEnabledChanged += new DependencyPropertyChangedEventHandler(OnIsEnabledChanged);
            this.KeyDown += new KeyEventHandler(GridSplitter_KeyDown);
            this.LayoutUpdated += delegate { UpdateTemplateOrientation(); };
            _dragValidator = new DragValidator(this);
            _dragValidator.DragStartedEvent += new EventHandler<DragStartedEventArgs>(DragValidator_DragStartedEvent);
            _dragValidator.DragDeltaEvent += new EventHandler<DragDeltaEventArgs>(DragValidator_DragDeltaEvent);
            _dragValidator.DragCompletedEvent += new EventHandler<DragCompletedEventArgs>(DragValidator_DragCompletedEvent);

#if MIGRATION
            this.MouseEnter += delegate(object sender, MouseEventArgs e)
#else
            this.PointerEntered += delegate(object sender, MouseEventArgs e)
#endif
            {
                _isMouseOver = true;
                ChangeVisualState();
            };

#if MIGRATION
            this.MouseLeave += delegate(object sender, MouseEventArgs e)
#else
            this.PointerExited += delegate(object sender, MouseEventArgs e)
#endif
            {
                _isMouseOver = false;

                // Only change the visual state if we're not currently resizing,
                // the visual state will get updated when the resize operation
                // comples
                if (ResizeDataInternal == null)
                {
                    ChangeVisualState();
                }
            };

            this.GotFocus += delegate(object sender, RoutedEventArgs e)
            {
                ChangeVisualState();
            };

            this.LostFocus += delegate(object sender, RoutedEventArgs e)
            {
                ChangeVisualState();
            };

            DefaultStyleKey = typeof(GridSplitter);
        }

        /// <summary>
        /// Builds the visual tree for the
        /// <see cref="T:System.Windows.Controls.GridSplitter" />
        /// control when a new template is applied.
        /// </summary>
#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

            ElementHorizontalTemplateFrameworkElement = this.GetTemplateChild(GridSplitter.ElementHorizontalTemplateName) as FrameworkElement;
            ElementVerticalTemplateFrameworkElement = this.GetTemplateChild(GridSplitter.ElementVerticalTemplateName) as FrameworkElement;

            // We need to recalculate the orientation, so set
            // _currentGridResizeDirection back to Auto
            _currentGridResizeDirection = GridResizeDirection.Auto;

            UpdateTemplateOrientation();
            ChangeVisualState(false);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the
        /// <see cref="T:System.Windows.Controls.GridSplitter" /> displays a
        /// preview.
        /// </summary>
        /// <value>
        /// True if a preview is displayed; otherwise, false.
        /// </value>
        public bool ShowsPreview
        {
            get { return (bool)GetValue(GridSplitter.ShowsPreviewProperty); }
            set { SetValue(GridSplitter.ShowsPreviewProperty, value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="T:System.Windows.Style" /> that is used
        /// for previewing changes.
        /// </summary>
        /// <value>
        /// The style that is used to preview changes.
        /// </value>
        public Style PreviewStyle
        {
            get { return (Style)GetValue(GridSplitter.PreviewStyleProperty); }
            set { SetValue(GridSplitter.PreviewStyleProperty, value); }
        }

        /// <summary>
        /// Returns a
        /// <see cref="T:System.Windows.Automation.Peers.GridSplitterAutomationPeer" />
        /// for use by the Silverlight automation infrastructure.
        /// </summary>
        /// <returns>
        /// A
        /// <see cref="T:System.Windows.Automation.Peers.GridSplitterAutomationPeer" />
        /// for the <see cref="T:System.Windows.Controls.GridSplitter" />
        /// object.
        /// </returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new GridSplitterAutomationPeer(this);
        }

        /// <summary>
        /// Method to change the visual state of the control.
        /// </summary>
        private void ChangeVisualState()
        {
            ChangeVisualState(true);
        }

        /// <summary>
        /// Change to the correct visual state for the GridSplitter.
        /// </summary>
        /// <param name="useTransitions">
        /// True to use transitions when updating the visual state, false to
        /// snap directly to the new visual state.
        /// </param>
        private void ChangeVisualState(bool useTransitions)
        {
            if (!IsEnabled)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateDisabled, VisualStates.StateNormal);
            }
            else if (_isMouseOver)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateMouseOver, VisualStates.StateNormal);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateNormal);
            }

            if (HasKeyboardFocus && this.IsEnabled)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateFocused, VisualStates.StateUnfocused);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateUnfocused);
            }

            if (GetEffectiveResizeDirection() == GridResizeDirection.Columns)
            {
                this.Cursor = Cursors.SizeWE;
            }
            else
            {
                this.Cursor = Cursors.SizeNS;
            }
        }

        /// <summary>
        /// Handle the drag completed event to commit or cancel the resize
        /// operation in progress.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment.</param>
        /// <param name="e">Inherited code: Requires comment 1.</param>
        internal void DragValidator_DragCompletedEvent(object sender, DragCompletedEventArgs e)
        {
            if (ResizeDataInternal != null)
            {
                if (e.Canceled)
                {
                    CancelResize();
                }
                else
                {
                    if (ResizeDataInternal.ShowsPreview)
                    {
                        MoveSplitter(ResizeDataInternal.PreviewControl.OffsetX, ResizeDataInternal.PreviewControl.OffsetY);
                        RemovePreviewControl();
                    }
                }
                ResizeDataInternal = null;
            }
            ChangeVisualState();
        }

        /// <summary>
        /// Handle the drag delta event to update the UI for the resize
        /// operation in progress.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment.</param>
        /// <param name="e">Inherited code: Requires comment 1.</param>
        internal void DragValidator_DragDeltaEvent(object sender, DragDeltaEventArgs e)
        {
            if (ResizeDataInternal != null)
            {
                double horizontalChange = e.HorizontalChange;
                double verticalChange = e.VerticalChange;

                if (ResizeDataInternal.ShowsPreview)
                {
                    if (ResizeDataInternal.ResizeDirection == GridResizeDirection.Columns)
                    {
                        ResizeDataInternal.PreviewControl.OffsetX = Math.Min(Math.Max(horizontalChange, ResizeDataInternal.MinChange), ResizeDataInternal.MaxChange);
                    }
                    else
                    {
                        ResizeDataInternal.PreviewControl.OffsetY = Math.Min(Math.Max(verticalChange, ResizeDataInternal.MinChange), ResizeDataInternal.MaxChange);
                    }
                }
                else
                {
                    MoveSplitter(horizontalChange, verticalChange);
                }
            }
        }

        /// <summary>
        /// Handle the drag started event to start a resize operation if the
        /// control is enabled.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment.</param>
        /// <param name="e">Inherited code: Requires comment 1.</param>
        internal void DragValidator_DragStartedEvent(object sender, DragStartedEventArgs e)
        {
            if (this.IsEnabled)
            {
                Focus();
                InitializeData(this.ShowsPreview);
            }
        }

        /// <summary>
        /// Handle the key down event to allow keyboard resizing or canceling a
        /// resize operation.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment.</param>
        /// <param name="e">Inherited code: Requires comment 1.</param>
        internal void GridSplitter_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    e.Handled = KeyboardMoveSplitter(FlipForRTL(-KeyboardIncrement), 0.0);
                    return;

                case Key.Up:
                    e.Handled = KeyboardMoveSplitter(0.0, -KeyboardIncrement);
                    return;

                case Key.Right:
                    e.Handled = KeyboardMoveSplitter(FlipForRTL(KeyboardIncrement), 0.0);
                    return;

                case Key.Down:
                    e.Handled = KeyboardMoveSplitter(0.0, KeyboardIncrement);
                    break;

                case Key.Escape:
                    if (ResizeDataInternal == null)
                    {
                        break;
                    }
                    CancelResize();
                    e.Handled = true;
                    return;

                default:
                    return;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the control has keyboard
        /// focus.
        /// </summary>
        private bool HasKeyboardFocus
        {
            get { return FocusManager.GetFocusedElement() == this; }
        }

        /// <summary>
        /// Initialize the resize data and move the splitter by the specified
        /// amount.
        /// </summary>
        /// <param name="horizontalChange">
        /// Horizontal amount to move the splitter.
        /// </param>
        /// <param name="verticalChange">
        /// Vertical amount to move the splitter.
        /// </param>
        /// <returns>Inherited code: Requires comment.</returns>
        internal bool InitializeAndMoveSplitter(double horizontalChange, double verticalChange)
        {
            // resizing directly is not allowed if there is a mouse initiated
            // resize operation in progress
            if (ResizeDataInternal != null)
            {
                return false;
            }

            InitializeData(false);
            if (ResizeDataInternal == null)
            {
                return false;
            }

            MoveSplitter(horizontalChange, verticalChange);
            ResizeDataInternal = null;
            return true;
        }

        /// <summary>
        /// Called by keyboard event handler to move the splitter if allowed.
        /// </summary>
        /// <param name="horizontalChange">
        /// Horizontal amount to move the splitter.
        /// </param>
        /// <param name="verticalChange">
        /// Vertical amount to move the splitter.
        /// </param>
        /// <returns>Inherited code: Requires comment.</returns>
        private bool KeyboardMoveSplitter(double horizontalChange, double verticalChange)
        {
            if (HasKeyboardFocus && this.IsEnabled)
            {
                return InitializeAndMoveSplitter(horizontalChange, verticalChange);
            }
            return false;
        }

        /// <summary>
        /// Creates the preview layer and adds it to the parent grid.
        /// </summary>
        /// <param name="parentGrid">Grid to add the preview layer to.</param>
        private void CreatePreviewLayer(Grid parentGrid)
        {
            Debug.Assert(parentGrid != null, "parentGrid should not be null!");
            Debug.Assert(parentGrid.RowDefinitions != null, "parentGrid.RowDefinitions should not be null!");
            Debug.Assert(parentGrid.ColumnDefinitions != null, "parentGrid.ColumnDefinitions should not be null!");

            _previewLayer = new Canvas();

            // RowSpan and ColumnSpan default to 1 and should not be set to 0 in
            // the case that a Grid has been created without explicitly setting
            // its ColumnDefinitions or RowDefinitions
            if (parentGrid.RowDefinitions.Count > 0)
            {
                _previewLayer.SetValue(Grid.RowSpanProperty, parentGrid.RowDefinitions.Count);
            }
            if (parentGrid.ColumnDefinitions.Count > 0)
            {
                _previewLayer.SetValue(Grid.ColumnSpanProperty, parentGrid.ColumnDefinitions.Count);
            }

            // REMOVE_RTM: Uncomment once Jolt Bug 11276 is fixed
            // this.previewLayer.SetValue(Grid.ZIndex, int.MaxValue);
            parentGrid.Children.Add(_previewLayer);
        }

        /// <summary>
        /// Add the preview layer to the Grid if it is not there already and
        /// then show the preview control.
        /// </summary>
        private void SetupPreview()
        {
            if (ResizeDataInternal.ShowsPreview)
            {
                if (_previewLayer == null)
                {
                    CreatePreviewLayer(ResizeDataInternal.Grid);
                }

                ResizeDataInternal.PreviewControl = new PreviewControl();
                ResizeDataInternal.PreviewControl.Bind(this);
                _previewLayer.Children.Add(ResizeDataInternal.PreviewControl);
                double[] changeRange = GetDeltaConstraints();
                Debug.Assert(changeRange.Length == 2, "The changeRange should have two elements!");
                ResizeDataInternal.MinChange = changeRange[0];
                ResizeDataInternal.MaxChange = changeRange[1];
            }
        }

        /// <summary>
        /// Remove the preview control from the preview layer if it exists.
        /// </summary>
        private void RemovePreviewControl()
        {
            if ((ResizeDataInternal.PreviewControl != null) && (_previewLayer != null))
            {
                Debug.Assert(_previewLayer.Children.Contains(ResizeDataInternal.PreviewControl), "The preview layer should contain the PreviewControl!");
                _previewLayer.Children.Remove(ResizeDataInternal.PreviewControl);
            }
        }

        /// <summary>
        /// Initialize the resizeData object to hold the information for the
        /// resize operation in progress.
        /// </summary>
        /// <param name="showsPreview">
        /// Whether or not the preview should be shown.
        /// </param>
        private void InitializeData(bool showsPreview)
        {
            Grid parent = Parent as Grid;
            if (parent != null)
            {
                ResizeDataInternal = new ResizeData();
                ResizeDataInternal.Grid = parent;
                ResizeDataInternal.ShowsPreview = showsPreview;
                ResizeDataInternal.ResizeDirection = GetEffectiveResizeDirection();
                ResizeDataInternal.ResizeBehavior = GetEffectiveResizeBehavior(ResizeDataInternal.ResizeDirection);
                ResizeDataInternal.SplitterLength = Math.Min(ActualWidth, ActualHeight);
                if (!SetupDefinitionsToResize())
                {
                    ResizeDataInternal = null;
                }
                else
                {
                    SetupPreview();
                }
            }
        }

        /// <summary>
        /// Move the splitter and resize the affected columns or rows.
        /// </summary>
        /// <param name="horizontalChange">
        /// Amount to resize horizontally.
        /// </param>
        /// <param name="verticalChange">
        /// Amount to resize vertically.
        /// </param>
        /// <remarks>
        /// Only one of horizontalChange or verticalChange will be non-zero.
        /// </remarks>
        private void MoveSplitter(double horizontalChange, double verticalChange)
        {
            double resizeChange = (ResizeDataInternal.ResizeDirection == GridResizeDirection.Columns) ? horizontalChange : verticalChange;
            DefinitionAbstraction definition1 = ResizeDataInternal.Definition1;
            DefinitionAbstraction definition2 = ResizeDataInternal.Definition2;
            if ((definition1 != null) && (definition2 != null))
            {
                double definition1ActualLength = GetActualLength(definition1);
                double definition2ActualLength = GetActualLength(definition2);
                if ((ResizeDataInternal.SplitBehavior == SplitBehavior.Split) && !DoubleUtil.AreClose((double)(definition1ActualLength + definition2ActualLength), (double)(ResizeDataInternal.OriginalDefinition1ActualLength + ResizeDataInternal.OriginalDefinition2ActualLength)))
                {
                    this.CancelResize();
                }
                else
                {
                    double[] changeRange = GetDeltaConstraints();
                    Debug.Assert(changeRange.Length == 2, "The changeRange should contain two elements!");
                    double minDelta = changeRange[0];
                    double maxDelta = changeRange[1];

                    resizeChange = Math.Min(Math.Max(resizeChange, minDelta), maxDelta);
                    double newDefinition1Length = definition1ActualLength + resizeChange;
                    double newDefinition2Length = definition2ActualLength - resizeChange;
                    SetLengths(newDefinition1Length, newDefinition2Length);
                }
            }
        }

        /// <summary>
        /// Determine which adjacent column or row definitions need to be
        /// included in the resize operation and set up resizeData accordingly.
        /// </summary>
        /// <returns>True if it is a valid resize operation.</returns>
        private bool SetupDefinitionsToResize()
        {
            int spanAmount = (int)GetValue((ResizeDataInternal.ResizeDirection == GridResizeDirection.Columns) ? Grid.ColumnSpanProperty : Grid.RowSpanProperty);
            if (spanAmount == 1)
            {
                int definition1Index;
                int definition2Index;
                int splitterIndex = (int)GetValue((ResizeDataInternal.ResizeDirection == GridResizeDirection.Columns) ? Grid.ColumnProperty : Grid.RowProperty);
                switch (ResizeDataInternal.ResizeBehavior)
                {
                    case GridResizeBehavior.CurrentAndNext:
                        definition1Index = splitterIndex;
                        definition2Index = splitterIndex + 1;
                        break;

                    case GridResizeBehavior.PreviousAndCurrent:
                        definition1Index = splitterIndex - 1;
                        definition2Index = splitterIndex;
                        break;

                    default:
                        definition1Index = splitterIndex - 1;
                        definition2Index = splitterIndex + 1;
                        break;
                }
                int definitionCount = (ResizeDataInternal.ResizeDirection == GridResizeDirection.Columns) ? ResizeDataInternal.Grid.ColumnDefinitions.Count : ResizeDataInternal.Grid.RowDefinitions.Count;
                if ((definition1Index >= 0) && (definition2Index < definitionCount))
                {
                    ResizeDataInternal.SplitterIndex = splitterIndex;
                    ResizeDataInternal.Definition1Index = definition1Index;
                    ResizeDataInternal.Definition1 = GetGridDefinition(ResizeDataInternal.Grid, definition1Index, ResizeDataInternal.ResizeDirection);
                    ResizeDataInternal.OriginalDefinition1Length = ResizeDataInternal.Definition1.Size;
                    ResizeDataInternal.OriginalDefinition1ActualLength = GetActualLength(ResizeDataInternal.Definition1);
                    ResizeDataInternal.Definition2Index = definition2Index;
                    ResizeDataInternal.Definition2 = GetGridDefinition(ResizeDataInternal.Grid, definition2Index, ResizeDataInternal.ResizeDirection);
                    ResizeDataInternal.OriginalDefinition2Length = ResizeDataInternal.Definition2.Size;
                    ResizeDataInternal.OriginalDefinition2ActualLength = GetActualLength(ResizeDataInternal.Definition2);
                    bool isDefinition1Star = IsStar(ResizeDataInternal.Definition1);
                    bool isDefinition2Star = IsStar(ResizeDataInternal.Definition2);
                    if (isDefinition1Star && isDefinition2Star)
                    {
                        ResizeDataInternal.SplitBehavior = SplitBehavior.Split;
                    }
                    else
                    {
                        ResizeDataInternal.SplitBehavior = !isDefinition1Star ? SplitBehavior.ResizeDefinition1 : SplitBehavior.ResizeDefinition2;
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Cancel the resize operation in progress.
        /// </summary>
        private void CancelResize()
        {
            if (ResizeDataInternal.ShowsPreview)
            {
                RemovePreviewControl();
            }
            else
            {
                SetLengths(ResizeDataInternal.OriginalDefinition1ActualLength, ResizeDataInternal.OriginalDefinition2ActualLength);
            }
            ResizeDataInternal = null;
        }

        /// <summary>
        /// Get the actual length of the given definition.
        /// </summary>
        /// <param name="definition">
        /// Row or column definition to get the actual length for.
        /// </param>
        /// <returns>
        /// Height of a row definition or width of a column definition.
        /// </returns>
        private static double GetActualLength(DefinitionAbstraction definition)
        {
            if (definition.AsColumnDefinition != null)
            {
                return definition.AsColumnDefinition.ActualWidth;
            }
            return definition.AsRowDefinition.ActualHeight;
        }

        /// <summary>
        /// Determine the max and min that the two definitions can be resized.
        /// </summary>
        /// <returns>Inherited code: Requires comment.</returns>
        private double[] GetDeltaConstraints()
        {
            double definition1ActualLength = GetActualLength(ResizeDataInternal.Definition1);
            double definition1MinSize = ResizeDataInternal.Definition1.MinSize;
            double definition1MaxSize = ResizeDataInternal.Definition1.MaxSize;
            double definition2ActualLength = GetActualLength(ResizeDataInternal.Definition2);
            double definition2MinSize = ResizeDataInternal.Definition2.MinSize;
            double definition2MaxSize = ResizeDataInternal.Definition2.MaxSize;
            double minDelta, maxDelta;

            // Can't resize smaller than the size of the splitter control itself
            if (ResizeDataInternal.SplitterIndex == ResizeDataInternal.Definition1Index)
            {
                definition1MinSize = Math.Max(definition1MinSize, ResizeDataInternal.SplitterLength);
            }
            else if (ResizeDataInternal.SplitterIndex == ResizeDataInternal.Definition2Index)
            {
                definition2MinSize = Math.Max(definition2MinSize, ResizeDataInternal.SplitterLength);
            }

            if (ResizeDataInternal.SplitBehavior == SplitBehavior.Split)
            {
                minDelta = -Math.Min((double)(definition1ActualLength - definition1MinSize), (double)(definition2MaxSize - definition2ActualLength));
                maxDelta = Math.Min((double)(definition1MaxSize - definition1ActualLength), (double)(definition2ActualLength - definition2MinSize));
            }
            else if (ResizeDataInternal.SplitBehavior == SplitBehavior.ResizeDefinition1)
            {
                minDelta = definition1MinSize - definition1ActualLength;
                maxDelta = definition1MaxSize - definition1ActualLength;
            }
            else
            {
                minDelta = definition2ActualLength - definition2MaxSize;
                maxDelta = definition2ActualLength - definition2MinSize;
            }

            return new double[] { minDelta, maxDelta };
        }

        /// <summary>
        /// Determine the resize behavior based on the given direction and
        /// alignment.
        /// </summary>
        /// <param name="direction">Inherited code: Requires comment.</param>
        /// <returns>Inherited code: Requires comment 1.</returns>
        private GridResizeBehavior GetEffectiveResizeBehavior(GridResizeDirection direction)
        {
            if (direction != GridResizeDirection.Columns)
            {
                switch (VerticalAlignment)
                {
                    case VerticalAlignment.Top:
                        return GridResizeBehavior.PreviousAndCurrent;

                    case VerticalAlignment.Bottom:
                        return GridResizeBehavior.CurrentAndNext;
                }
                return GridResizeBehavior.PreviousAndNext;
            }
            else
            {
                switch (HorizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        return GridResizeBehavior.PreviousAndCurrent;

                    case HorizontalAlignment.Right:
                        return GridResizeBehavior.CurrentAndNext;
                }
                return GridResizeBehavior.PreviousAndNext;
            }
        }

        /// <summary>
        /// Determine the resize direction based on the horizontal and vertical
        /// alignments.
        /// </summary>
        /// <returns>Inherited code: Requires comment.</returns>
        private GridResizeDirection GetEffectiveResizeDirection()
        {
            if (HorizontalAlignment != HorizontalAlignment.Stretch)
            {
                return GridResizeDirection.Columns;
            }
            if ((VerticalAlignment == VerticalAlignment.Stretch) && (ActualWidth <= ActualHeight))
            {
                return GridResizeDirection.Columns;
            }
            return GridResizeDirection.Rows;
        }

        /// <summary>
        /// Create a DefinitionAbstraction instance for the given row or column
        /// index in the grid.
        /// </summary>
        /// <param name="grid">Inherited code: Requires comment.</param>
        /// <param name="index">Inherited code: Requires comment 1.</param>
        /// <param name="direction">Inherited code: Requires comment 2.</param>
        /// <returns>Inherited code: Requires comment 3.</returns>
        private static DefinitionAbstraction GetGridDefinition(Grid grid, int index, GridResizeDirection direction)
        {
            if (direction != GridResizeDirection.Columns)
            {
                return new DefinitionAbstraction(grid.RowDefinitions[index]);
            }
            return new DefinitionAbstraction(grid.ColumnDefinitions[index]);
        }

        /// <summary>
        /// Flips a given length if FlowDirection is set to RightToLeft.  This is used for 
        /// keyboard handling.
        /// </summary>
        /// <param name="value">Value to flip for RightToLeft.</param>
        /// <returns>Value if FlowDirection is set to RightToLeft; otherwise, value.</returns>
        private double FlipForRTL(double value)
        {
            return (this.FlowDirection == FlowDirection.RightToLeft) ? -value : value;
        }

        /// <summary>
        /// Set the lengths of the two definitions depending on the split
        /// behavior.
        /// </summary>
        /// <param name="definition1Pixels">
        /// Inherited code: Requires comment.
        /// </param>
        /// <param name="definition2Pixels">
        /// Inherited code: Requires comment 1.
        /// </param>
        private void SetLengths(double definition1Pixels, double definition2Pixels)
        {
            if (ResizeDataInternal.SplitBehavior == SplitBehavior.Split)
            {
                IEnumerable enumerable = (ResizeDataInternal.ResizeDirection == GridResizeDirection.Columns) ? ((IEnumerable)ResizeDataInternal.Grid.ColumnDefinitions) : ((IEnumerable)ResizeDataInternal.Grid.RowDefinitions);
                int definitionIndex = 0;
                DefinitionAbstraction definitionAbstraction;
                foreach (DependencyObject definition in enumerable)
                {
                    definitionAbstraction = new DefinitionAbstraction(definition);
                    if (definitionIndex == ResizeDataInternal.Definition1Index)
                    {
                        SetDefinitionLength(definitionAbstraction, new GridLength(definition1Pixels, GridUnitType.Star));
                    }
                    else if (definitionIndex == ResizeDataInternal.Definition2Index)
                    {
                        SetDefinitionLength(definitionAbstraction, new GridLength(definition2Pixels, GridUnitType.Star));
                    }
                    else if (IsStar(definitionAbstraction))
                    {
                        SetDefinitionLength(definitionAbstraction, new GridLength(GetActualLength(definitionAbstraction), GridUnitType.Star));
                    }
                    definitionIndex++;
                }
            }
            else if (ResizeDataInternal.SplitBehavior == SplitBehavior.ResizeDefinition1)
            {
                SetDefinitionLength(ResizeDataInternal.Definition1, new GridLength(definition1Pixels));
            }
            else
            {
                SetDefinitionLength(ResizeDataInternal.Definition2, new GridLength(definition2Pixels));
            }
        }

        /// <summary>
        /// Set the height/width of the given row/column.
        /// </summary>
        /// <param name="definition">Inherited code: Requires comment.</param>
        /// <param name="length">Inherited code: Requires comment 1.</param>
        private static void SetDefinitionLength(DefinitionAbstraction definition, GridLength length)
        {
            if (definition.AsColumnDefinition != null)
            {
                definition.AsColumnDefinition.SetValue(ColumnDefinition.WidthProperty, length);
            }
            else
            {
                definition.AsRowDefinition.SetValue(RowDefinition.HeightProperty, length);
            }
        }

        /// <summary>
        /// Determine if the given definition has its size set to the "*" value.
        /// </summary>
        /// <param name="definition">Inherited code: Requires comment.</param>
        /// <returns>Inherited code: Requires comment 1.</returns>
        private static bool IsStar(DefinitionAbstraction definition)
        {
            if (definition.AsColumnDefinition != null)
            {
                return definition.AsColumnDefinition.Width.IsStar;
            }
            return definition.AsRowDefinition.Height.IsStar;
        }

        /// <summary>
        /// This code will run whenever the effective resize direction changes,
        /// to update the template being used to display this control.
        /// </summary>
        private void UpdateTemplateOrientation()
        {
            GridResizeDirection newGridResizeDirection = GetEffectiveResizeDirection();

            if (_currentGridResizeDirection != newGridResizeDirection)
            {
                if (newGridResizeDirection == GridResizeDirection.Columns)
                {
                    if (ElementHorizontalTemplateFrameworkElement != null)
                    {
                        ElementHorizontalTemplateFrameworkElement.Visibility = Visibility.Collapsed;
                    }
                    if (ElementVerticalTemplateFrameworkElement != null)
                    {
                        ElementVerticalTemplateFrameworkElement.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    if (ElementHorizontalTemplateFrameworkElement != null)
                    {
                        ElementHorizontalTemplateFrameworkElement.Visibility = Visibility.Visible;
                    }
                    if (ElementVerticalTemplateFrameworkElement != null)
                    {
                        ElementVerticalTemplateFrameworkElement.Visibility = Visibility.Collapsed;
                    }
                }
                _currentGridResizeDirection = newGridResizeDirection;
            }
        }
    }
}