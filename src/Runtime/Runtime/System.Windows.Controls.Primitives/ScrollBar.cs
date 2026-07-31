// (c) Copyright Microsoft Corporation. 
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using OpenSilver.Internal;
using OpenSilver.Internal.Commands;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Automation.Peers;
using System.Windows.Input;

namespace System.Windows.Controls.Primitives
{
    /// <summary>
    /// Represents a control that provides a scroll bar that has a sliding 
    /// <see cref="Thumb" /> whose position corresponds to a value.
    /// </summary> 
    [TemplatePart(Name = ElementHorizontalTemplateName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = ElementHorizontalLargeIncreaseName, Type = typeof(RepeatButton))]
    [TemplatePart(Name = ElementHorizontalLargeDecreaseName, Type = typeof(RepeatButton))]
    [TemplatePart(Name = ElementHorizontalSmallDecreaseName, Type = typeof(RepeatButton))]
    [TemplatePart(Name = ElementHorizontalSmallIncreaseName, Type = typeof(RepeatButton))]
    [TemplatePart(Name = ElementHorizontalThumbName, Type = typeof(Thumb))]
    [TemplatePart(Name = ElementVerticalTemplateName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = ElementVerticalLargeIncreaseName, Type = typeof(RepeatButton))]
    [TemplatePart(Name = ElementVerticalLargeDecreaseName, Type = typeof(RepeatButton))]
    [TemplatePart(Name = ElementVerticalSmallIncreaseName, Type = typeof(RepeatButton))]
    [TemplatePart(Name = ElementVerticalSmallDecreaseName, Type = typeof(RepeatButton))]
    [TemplatePart(Name = ElementVerticalThumbName, Type = typeof(Thumb))]
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateMouseOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]
    public sealed class ScrollBar : RangeBase
    {
        static ScrollBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ScrollBar), new PropertyMetadata(typeof(ScrollBar)));
            IsEnabledProperty.OverrideMetadata(typeof(ScrollBar), new PropertyMetadata(OnIsEnabledChanged));

            // Register Event Handler for the Thumb
            EventManager.RegisterClassHandler<ScrollBar>(Thumb.DragStartedEvent, new DragStartedEventHandler(OnThumbDragStarted));
            EventManager.RegisterClassHandler<ScrollBar>(Thumb.DragDeltaEvent, new DragDeltaEventHandler(OnThumbDragDelta));
            EventManager.RegisterClassHandler<ScrollBar>(Thumb.DragCompletedEvent, new DragCompletedEventHandler(OnThumbDragCompleted));

            EventManager.RegisterClassHandler<ScrollBar>(SizeChangedEvent, new SizeChangedEventHandler(OnSizeChanged));

            var onScrollCommand = new ExecutedRoutedEventHandler(OnScrollCommand);
            var onQueryScrollCommand = new CanExecuteRoutedEventHandler(OnQueryScrollCommand);

            // Vertical Commands
            CommandHelpers.RegisterCommandHandler(typeof(ScrollBar), LineUpCommand, onScrollCommand, onQueryScrollCommand,  new KeyGesture(Key.Up));
            CommandHelpers.RegisterCommandHandler(typeof(ScrollBar), LineDownCommand, onScrollCommand, onQueryScrollCommand, new KeyGesture(Key.Down));
            CommandHelpers.RegisterCommandHandler(typeof(ScrollBar), PageUpCommand, onScrollCommand, onQueryScrollCommand, new KeyGesture(Key.PageUp));
            CommandHelpers.RegisterCommandHandler(typeof(ScrollBar), PageDownCommand, onScrollCommand, onQueryScrollCommand, new KeyGesture(Key.PageDown));
            CommandHelpers.RegisterCommandHandler(typeof(ScrollBar), ScrollToTopCommand, onScrollCommand, onQueryScrollCommand, new KeyGesture(Key.Home, ModifierKeys.Control));
            CommandHelpers.RegisterCommandHandler(typeof(ScrollBar), ScrollToBottomCommand, onScrollCommand, onQueryScrollCommand, new KeyGesture(Key.End, ModifierKeys.Control));

            // Horizontal Commands
            CommandHelpers.RegisterCommandHandler(typeof(ScrollBar), LineLeftCommand, onScrollCommand, onQueryScrollCommand, new KeyGesture(Key.Left));
            CommandHelpers.RegisterCommandHandler(typeof(ScrollBar), LineRightCommand, onScrollCommand, onQueryScrollCommand, new KeyGesture(Key.Right));
            CommandHelpers.RegisterCommandHandler(typeof(ScrollBar), PageLeftCommand, onScrollCommand, onQueryScrollCommand);
            CommandHelpers.RegisterCommandHandler(typeof(ScrollBar), PageRightCommand, onScrollCommand, onQueryScrollCommand);
            CommandHelpers.RegisterCommandHandler(typeof(ScrollBar), ScrollToLeftEndCommand, onScrollCommand, onQueryScrollCommand, new KeyGesture(Key.Home));
            CommandHelpers.RegisterCommandHandler(typeof(ScrollBar), ScrollToRightEndCommand, onScrollCommand, onQueryScrollCommand, new KeyGesture(Key.End));
        }

        /// <summary> 
        /// Initializes a new instance of the <see cref="ScrollBar"/> class.
        /// </summary> 
        public ScrollBar() { }

        // Is the scrollbar outside of a scrollviewer?
        internal bool IsStandalone { get; set; } = true;

        /// <summary>
        /// The command that scrolls a <see cref="ScrollBar"/> by a small amount in the vertical direction,
        /// decreasing its value.
        /// </summary>
        public static readonly RoutedCommand LineUpCommand = new RoutedCommand("LineUp", typeof(ScrollBar));

        /// <summary>
        /// The command that scrolls a <see cref="ScrollBar"/> by a small amount in the vertical direction,
        /// increasing its value.
        /// </summary>
        public static readonly RoutedCommand LineDownCommand = new RoutedCommand("LineDown", typeof(ScrollBar));

        /// <summary>
        /// The command that scrolls a <see cref="ScrollBar"/> by a small amount in the horizontal direction,
        /// decreasing its value.
        /// </summary>
        public static readonly RoutedCommand LineLeftCommand = new RoutedCommand("LineLeft", typeof(ScrollBar));

        /// <summary>
        /// The command that scrolls a <see cref="ScrollBar"/> by a small amount in the horizontal direction,
        /// increasing its value.
        /// </summary>
        public static readonly RoutedCommand LineRightCommand = new RoutedCommand("LineRight", typeof(ScrollBar));

        /// <summary>
        /// The command that scrolls a <see cref="ScrollBar"/> by a large amount in the vertical direction,
        /// decreasing its value.
        /// </summary>
        public static readonly RoutedCommand PageUpCommand = new RoutedCommand("PageUp", typeof(ScrollBar));

        /// <summary>
        /// The command that scrolls a <see cref="ScrollBar"/> by a large amount in the vertical direction,
        /// increasing its value.
        /// </summary>
        public static readonly RoutedCommand PageDownCommand = new RoutedCommand("PageDown", typeof(ScrollBar));

        /// <summary>
        /// The command that scrolls a <see cref="ScrollBar"/> by a large amount in the horizontal direction,
        /// decreasing its value.
        /// </summary>
        public static readonly RoutedCommand PageLeftCommand = new RoutedCommand("PageLeft", typeof(ScrollBar));

        /// <summary>
        /// The command that scrolls a <see cref="ScrollBar"/> by a large amount in the horizontal direction,
        /// increasing its value.
        /// </summary>
        public static readonly RoutedCommand PageRightCommand = new RoutedCommand("PageRight", typeof(ScrollBar));

        /// <summary>
        /// The command that scrolls the content to the lower-right corner of a <see cref="ScrollViewer"/> 
        /// control.
        /// </summary>
        public static readonly RoutedCommand ScrollToEndCommand = new RoutedCommand("ScrollToEnd", typeof(ScrollBar));

        /// <summary>
        /// The command that scrolls the content to the upper-left corner of a <see cref="ScrollViewer"/> 
        /// control.
        /// </summary>
        public static readonly RoutedCommand ScrollToHomeCommand = new RoutedCommand("ScrollToHome", typeof(ScrollBar));

        /// <summary>
        /// The command that scrolls a <see cref="ScrollBar"/> to the <see cref="RangeBase.Maximum"/> value 
        /// for a horizontal <see cref="ScrollBar"/>.
        /// </summary>
        public static readonly RoutedCommand ScrollToRightEndCommand = new RoutedCommand("ScrollToRightEnd", typeof(ScrollBar));

        /// <summary>
        /// The command that scrolls a <see cref="ScrollBar"/> to the <see cref="RangeBase.Minimum"/> value 
        /// for a horizontal <see cref="ScrollBar"/>.
        /// </summary>
        public static readonly RoutedCommand ScrollToLeftEndCommand = new RoutedCommand("ScrollToLeftEnd", typeof(ScrollBar));

        /// <summary>
        /// The command that scrolls a <see cref="ScrollBar"/> to the <see cref="RangeBase.Maximum"/> value 
        /// for a vertical <see cref="ScrollBar"/>.
        /// </summary>
        public static readonly RoutedCommand ScrollToTopCommand = new RoutedCommand("ScrollToTop", typeof(ScrollBar));

        /// <summary>
        /// The command that scrolls a <see cref="ScrollBar"/> to the <see cref="RangeBase.Maximum"/> value
        /// for a horizontal <see cref="ScrollBar"/>.
        /// </summary>
        public static readonly RoutedCommand ScrollToBottomCommand = new RoutedCommand("ScrollToBottom", typeof(ScrollBar));

        /// <summary>
        /// The command that scrolls a horizontal <see cref="ScrollBar"/> in a <see cref="ScrollViewer"/> 
        /// to the value that is provided in <see cref="ExecutedRoutedEventArgs.Parameter"/>.
        /// </summary>
        public static readonly RoutedCommand ScrollToHorizontalOffsetCommand = new RoutedCommand("ScrollToHorizontalOffset", typeof(ScrollBar));

        /// <summary>
        /// The command that scrolls a vertical <see cref="ScrollBar"/> in a <see cref="ScrollViewer"/> 
        /// to the value that is provided in <see cref="ExecutedRoutedEventArgs.Parameter"/>.
        /// </summary>
        public static readonly RoutedCommand ScrollToVerticalOffsetCommand = new RoutedCommand("ScrollToVerticalOffset", typeof(ScrollBar));

        /// <summary>
        /// The command that notifies the <see cref="ScrollViewer"/> that the user is dragging the 
        /// <see cref="Thumb"/> of the horizontal <see cref="ScrollBar"/> to the value that is provided 
        /// in <see cref="ExecutedRoutedEventArgs.Parameter"/>.
        /// </summary>
        public static readonly RoutedCommand DeferScrollToHorizontalOffsetCommand = new RoutedCommand("DeferScrollToToHorizontalOffset", typeof(ScrollBar));

        /// <summary>
        /// The command that notifies the <see cref="ScrollViewer"/> that the user is dragging the 
        /// <see cref="Thumb"/> of the vertical <see cref="ScrollBar"/> to the value that is provided 
        /// in <see cref="ExecutedRoutedEventArgs.Parameter"/>.
        /// </summary>
        public static readonly RoutedCommand DeferScrollToVerticalOffsetCommand = new RoutedCommand("DeferScrollToVerticalOffset", typeof(ScrollBar));

        /// <summary> 
        /// Builds the visual tree for the <see cref="ScrollBar"/> control
        /// when a new template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Get the parts
            ElementHorizontalTemplate = GetTemplateChild(ElementHorizontalTemplateName) as FrameworkElement;
            ElementHorizontalLargeIncrease = GetTemplateChild(ElementHorizontalLargeIncreaseName) as RepeatButton;
            ElementHorizontalLargeDecrease = GetTemplateChild(ElementHorizontalLargeDecreaseName) as RepeatButton;
            ElementHorizontalSmallIncrease = GetTemplateChild(ElementHorizontalSmallIncreaseName) as RepeatButton;
            ElementHorizontalSmallDecrease = GetTemplateChild(ElementHorizontalSmallDecreaseName) as RepeatButton;
            ElementHorizontalThumb = GetTemplateChild(ElementHorizontalThumbName) as Thumb;
            ElementVerticalTemplate = GetTemplateChild(ElementVerticalTemplateName) as FrameworkElement;
            ElementVerticalLargeIncrease = GetTemplateChild(ElementVerticalLargeIncreaseName) as RepeatButton;
            ElementVerticalLargeDecrease = GetTemplateChild(ElementVerticalLargeDecreaseName) as RepeatButton;
            ElementVerticalSmallIncrease = GetTemplateChild(ElementVerticalSmallIncreaseName) as RepeatButton;
            ElementVerticalSmallDecrease = GetTemplateChild(ElementVerticalSmallDecreaseName) as RepeatButton;
            ElementVerticalThumb = GetTemplateChild(ElementVerticalThumbName) as Thumb;

            ElementHorizontalLargeDecrease?.Command = PageLeftCommand;
            ElementHorizontalLargeIncrease?.Command = PageRightCommand;
            ElementHorizontalSmallDecrease?.Command = LineLeftCommand;
            ElementHorizontalSmallIncrease?.Command = LineRightCommand;

            ElementVerticalLargeDecrease?.Command = PageUpCommand;
            ElementVerticalLargeIncrease?.Command = PageDownCommand;
            ElementVerticalSmallDecrease?.Command = LineUpCommand;
            ElementVerticalSmallIncrease?.Command = LineDownCommand;

            // Updating states for parts where properties might have been updated through 
            // XAML before the template was loaded.
            OnOrientationChanged();
            UpdateVisualState(false);
        }

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer()
            => new ScrollBarAutomationPeer(this);

        private static void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var scrollBar = (ScrollBar)sender;
            scrollBar.UpdateTrackLayout(scrollBar.GetTrackLength());
        }

        private static void OnScrollCommand(object target, ExecutedRoutedEventArgs args)
        {
            ScrollBar scrollBar = (ScrollBar)target;

            if (scrollBar.IsStandalone)
            {
                if (scrollBar.Orientation == Orientation.Vertical)
                {
                    if (args.Command == LineUpCommand)
                    {
                        scrollBar.LineUp();
                    }
                    else if (args.Command == LineDownCommand)
                    {
                        scrollBar.LineDown();
                    }
                    else if (args.Command == PageUpCommand)
                    {
                        scrollBar.PageUp();
                    }
                    else if (args.Command == PageDownCommand)
                    {
                        scrollBar.PageDown();
                    }
                    else if (args.Command == ScrollToTopCommand)
                    {
                        scrollBar.ScrollToTop();
                    }
                    else if (args.Command == ScrollToBottomCommand)
                    {
                        scrollBar.ScrollToBottom();
                    }
                }
                else
                {
                    if (args.Command == LineLeftCommand)
                    {
                        scrollBar.LineLeft();
                    }
                    else if (args.Command == LineRightCommand)
                    {
                        scrollBar.LineRight();
                    }
                    else if (args.Command == PageLeftCommand)
                    {
                        scrollBar.PageLeft();
                    }
                    else if (args.Command == PageRightCommand)
                    {
                        scrollBar.PageRight();
                    }
                    else if (args.Command == ScrollToLeftEndCommand)
                    {
                        scrollBar.ScrollToLeftEnd();
                    }
                    else if (args.Command == ScrollToRightEndCommand)
                    {
                        scrollBar.ScrollToRightEnd();
                    }
                }
            }
        }

        private static void OnQueryScrollCommand(object target, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = ((ScrollBar)target).IsStandalone;
        }

        private void SmallDecrement()
        {
            double newValue = Math.Max(Value - SmallChange, Minimum);
            if (Value != newValue)
            {
                Value = newValue;
                RaiseScrollEvent(ScrollEventType.SmallDecrement);
            }
        }

        private void SmallIncrement()
        {
            double newValue = Math.Min(Value + SmallChange, Maximum);
            if (Value != newValue)
            {
                Value = newValue;
                RaiseScrollEvent(ScrollEventType.SmallIncrement);
            }
        }

        private void LargeDecrement()
        {
            double newValue = Math.Max(Value - LargeChange, Minimum);
            if (Value != newValue)
            {
                Value = newValue;
                RaiseScrollEvent(ScrollEventType.LargeDecrement);
            }
        }

        private void LargeIncrement()
        {
            double newValue = Math.Min(Value + LargeChange, Maximum);
            if (Value != newValue)
            {
                Value = newValue;
                RaiseScrollEvent(ScrollEventType.LargeIncrement);
            }
        }

        private void ToMinimum()
        {
            if (Value != Minimum)
            {
                Value = Minimum;
                RaiseScrollEvent(ScrollEventType.First);
            }
        }

        private void ToMaximum()
        {
            if (Value != Maximum)
            {
                Value = Maximum;
                RaiseScrollEvent(ScrollEventType.Last);
            }
        }

        private void LineUp() => SmallDecrement();

        private void LineDown() => SmallIncrement();

        private void PageUp() => LargeDecrement();

        private void PageDown() => LargeIncrement();

        private void ScrollToTop() => ToMinimum();

        private void ScrollToBottom() => ToMaximum();

        private void LineLeft() => SmallDecrement();

        private void LineRight() => SmallIncrement();

        private void PageLeft() => LargeDecrement();

        private void PageRight() => LargeIncrement();

        private void ScrollToLeftEnd() => ToMinimum();

        private void ScrollToRightEnd() => ToMaximum();

        private static void OnThumbDragStarted(object sender, DragStartedEventArgs e) => ((ScrollBar)sender).OnThumbDragStarted();

        private void OnThumbDragStarted()
        {
            _hasScrolled = false;
            _previousValue = Value;
            _dragDelta = new Vector();
        }

        private static void OnThumbDragCompleted(object sender, DragCompletedEventArgs e) => ((ScrollBar)sender).OnThumbDragCompleted();

        private void OnThumbDragCompleted()
        {
            if (_hasScrolled)
            {
                FinishDrag();
                RaiseScrollEvent(ScrollEventType.EndScroll);
            }
        }

        private void FinishDrag()
        {
            double value = Value;
            IInputElement target = CommandTarget;
            RoutedCommand command = Orientation == Orientation.Horizontal ? DeferScrollToHorizontalOffsetCommand : DeferScrollToVerticalOffsetCommand;

            if (command.CanExecute(value, target))
            {
                // If we were reporting drag commands, we need to give a final scroll command
                ChangeValue(value, false /* defer */);
            }
        }

        // Event handler to listen to thumb events.
        private static void OnThumbDragDelta(object sender, DragDeltaEventArgs e) => ((ScrollBar)sender).UpdateValue(e.HorizontalChange, e.VerticalChange);

        private void UpdateValue(double horizontalDragDelta, double verticalDragDelta)
        {
            double offset = 0;
            bool horizontal = Orientation == Orientation.Horizontal;

            double perpendicularDragDelta;

            if (horizontal)
            {
                _dragDelta.Y += verticalDragDelta;
                perpendicularDragDelta = Math.Abs(_dragDelta.Y);

                if (ElementHorizontalThumb != null)
                {
                    offset = horizontalDragDelta / (GetTrackLength() - ElementHorizontalThumb.ActualWidth) * (Maximum - Minimum);
                }
            }
            else
            {
                _dragDelta.X += horizontalDragDelta;
                perpendicularDragDelta = Math.Abs(_dragDelta.X);

                if (ElementVerticalThumb != null)
                {
                    offset = verticalDragDelta / (GetTrackLength() - ElementVerticalThumb.ActualHeight) * (Maximum - Minimum);
                }
            }

            if (!double.IsNaN(offset) && !double.IsInfinity(offset))
            {
                double dragValue;

                if (horizontal)
                {
                    _dragDelta.X += offset;
                    dragValue = _dragDelta.X;
                }
                else
                {
                    _dragDelta.Y += offset;
                    dragValue = _dragDelta.Y;
                }

                double newValue = Math.Min(Maximum, Math.Max(Minimum, _previousValue + dragValue));

                if (DoubleUtil.GreaterThan(perpendicularDragDelta, MaxPerpendicularDelta))
                {
                    newValue = _previousValue;
                }

                if (!DoubleUtil.AreClose(newValue, Value))
                {
                    _hasScrolled = true;
                    ChangeValue(newValue, true);
                    RaiseScrollEvent(ScrollEventType.ThumbTrack);
                }
            }
        }

        private void ChangeValue(double newValue, bool defer)
        {
            if (IsStandalone)
            {
                Value = newValue;
            }
            else
            {
                IInputElement target = CommandTarget;
                RoutedCommand command = null;
                bool horizontal = Orientation == Orientation.Horizontal;

                // Fire the deferred (drag) version of the command
                if (defer)
                {
                    command = horizontal ? DeferScrollToHorizontalOffsetCommand : DeferScrollToVerticalOffsetCommand;
                    if (command.CanExecute(newValue, target))
                    {
                        // The defer version of the command is enabled, fire this command and not the scroll version
                        command.Execute(newValue, target);
                    }
                    else
                    {
                        // The defer version of the command is not enabled, reset and try the scroll version
                        command = null;
                    }
                }

                if (command is null)
                {
                    // Either we're not dragging or the drag command is not enabled, try the scroll version
                    command = horizontal ? ScrollToHorizontalOffsetCommand : ScrollToVerticalOffsetCommand;
                    if (command.CanExecute(newValue, target))
                    {
                        command.Execute(newValue, target);
                    }
                }
            }
        }

        private IInputElement CommandTarget
        {
            get
            {
                if (TemplatedParent is IInputElement target)
                {
                    return target;
                }

                return this;
            }
        }

        /// <summary>
        /// Gets or sets whether the <see cref="ScrollBar"/> is displayed
        /// horizontally or vertically.
        /// </summary>
        /// <returns>
        /// An <see cref="Orientation"/> enumeration value that defines whether
        /// the <see cref="ScrollBar"/> is displayed horizontally or vertically.
        /// The default is <see cref="Orientation.Horizontal"/>.
        /// </returns>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValueInternal(OrientationProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Orientation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(
                nameof(Orientation),
                typeof(Orientation),
                typeof(ScrollBar),
                new PropertyMetadata(Orientation.Vertical, OnOrientationPropertyChanged),
                IsValidOrientation);

        /// <summary> 
        /// OrientationProperty property changed handler.
        /// </summary>
        /// <param name="d">ScrollBar that changed Orientation.</param> 
        /// <param name="e">DependencyPropertyChangedEventArgs.</param> 
        private static void OnOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScrollBar s = d as ScrollBar;
            Debug.Assert(s != null);

            s.OnOrientationChanged();
        }

        internal static bool IsValidOrientation(object o)
        {
            var value = (Orientation)o;
            return value == Orientation.Horizontal || value == Orientation.Vertical;
        }

        /// <summary>
        /// Gets or sets the amount of the scrollable content that is currently visible.
        /// </summary>
        /// <returns>
        /// The amount of the scrollable content that is currently visible. The default is 0.
        /// </returns>
        public double ViewportSize
        {
            get { return (double)GetValue(ViewportSizeProperty); }
            set { SetValueInternal(ViewportSizeProperty, value); }
        }

        /// <summary> 
        /// Identifies the <see cref="ViewportSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewportSizeProperty =
            DependencyProperty.Register(
                nameof(ViewportSize),
                typeof(double),
                typeof(ScrollBar),
                new PropertyMetadata(0.0d, OnViewportSizeChanged));

        private static void OnViewportSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScrollBar s = d as ScrollBar;
            Debug.Assert(s != null);

            s.UpdateTrackLayout(s.GetTrackLength());
        }

        /// <summary> 
        /// Called when the IsEnabled property changes.
        /// </summary>
        /// <param name="d"></param> 
        /// <param name="e">Property changed args</param>
        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ScrollBar)d).UpdateVisualState();
        }

        /// <summary> 
        /// Called when the Value property changes. 
        /// </summary>
        /// <param name="oldValue">Old value of the Value property.</param> 
        /// <param name="newValue">New value of the Value property.</param>
        protected override void OnValueChanged(double oldValue, double newValue)
        {
            double trackLength = GetTrackLength();

            base.OnValueChanged(oldValue, newValue);

            UpdateTrackLayout(trackLength);
        }

        /// <summary>
        /// Called when the Maximum property changes 
        /// </summary>
        /// <param name="oldMaximum">Old value of the Maximum property.</param>
        /// <param name="newMaximum">New value of the Maximum property.</param> 
        protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
            double trackLength = GetTrackLength();

            base.OnMaximumChanged(oldMaximum, newMaximum);
            UpdateTrackLayout(trackLength);
        }

        /// <summary> 
        /// Called when the Minimum property changes 
        /// </summary>
        /// <param name="oldMinimum">Old value of the Minimum property.</param> 
        /// <param name="newMinimum">New value of the Minimum property.</param>
        protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
            double trackLength = GetTrackLength();

            base.OnMinimumChanged(oldMinimum, newMinimum);
            UpdateTrackLayout(trackLength);
        }

        /// <summary>
        /// Responds to the MouseEnter event.
        /// </summary> 
        /// <param name="e">The event data for the MouseEnter event.</param> 
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            if ((Orientation == Orientation.Horizontal && ElementHorizontalThumb != null && !ElementHorizontalThumb.IsDragging) ||
                (Orientation == Orientation.Vertical && ElementVerticalThumb != null && !ElementVerticalThumb.IsDragging))
            {
                UpdateVisualState();
            }
        }

        /// <summary>
        /// Responds to the MouseLeave event. 
        /// </summary>
        /// <param name="e">The event data for the MouseLeave event.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if ((Orientation == Orientation.Horizontal && ElementHorizontalThumb != null && !ElementHorizontalThumb.IsDragging) ||
                (Orientation == Orientation.Vertical && ElementVerticalThumb != null && !ElementVerticalThumb.IsDragging))
            {
                UpdateVisualState();
            }
        }

        /// <summary> 
        /// Responds to the MouseLeftButtonDown event.
        /// </summary>
        /// <param name="e">The event data for the MouseLeftButtonDown event.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.Handled)
            {
                return;
            }
            e.Handled = true;
            CaptureMouse();
        }

        /// <summary> 
        /// Responds to the MouseLeftButtonUp event.
        /// </summary> 
        /// <param name="e">The event data for the MouseLeftButtonUp event.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            if (e.Handled)
            {
                return;
            }
            e.Handled = true;
            ReleaseMouseCapture();
            UpdateVisualState();
        }

        /// <summary>
        /// Identifies the <see cref="Scroll"/> routed event.
        /// </summary>
        public static readonly RoutedEvent ScrollEvent =
            EventManager.RegisterRoutedEvent(
                nameof(Scroll),
                RoutingStrategy.Bubble,
                typeof(ScrollEventHandler),
                typeof(ScrollBar));

        /// <summary>
        /// Occurs one or more times as content scrolls in a <see cref="ScrollBar"/>
        /// when the user moves the <see cref="Thumb"/> by using the mouse.
        /// </summary>
        public event ScrollEventHandler Scroll
        {
            add => AddHandler(ScrollEvent, value);
            remove => RemoveHandler(ScrollEvent, value);
        }

        /// <summary> 
        /// This raises the Scroll event, passing in the scrollEventType 
        /// as a parameter to let the handler know what triggered this event.
        /// </summary> 
        /// <param name="scrollEventType">ScrollEventType</param>
        internal void RaiseScrollEvent(ScrollEventType scrollEventType)
        {
            TimeSpan debounce = DebounceInterval;
            if (debounce > TimeSpan.Zero && scrollEventType != ScrollEventType.EndScroll)
            {
                _debounceDispatcher ??= new DebounceDispatcher();

                _debounceDispatcher.Debounce(
                    debounce,
                    () => RaiseEvent(new ScrollEventArgs(scrollEventType, Value) { Source = this }));
            }
            else
            {
                RaiseEvent(new ScrollEventArgs(scrollEventType, Value)
                {
                    Source = this,
                });
            }
        }

        /// <summary> 
        /// Update the current visual state of the ScrollBar.
        /// </summary> 
        internal void UpdateVisualState()
        {
            UpdateVisualState(true);
        }

        /// <summary>
        /// Update the current visual state of the ScrollBar.
        /// </summary> 
        /// <param name="useTransitions">
        /// true to use transitions when updating the visual state, false to
        /// snap directly to the new visual state. 
        /// </param> 
        internal void UpdateVisualState(bool useTransitions)
        {
            if (!IsEnabled)
            {
                GoToState(useTransitions, VisualStates.StateDisabled);
            }
            else if (IsMouseOver)
            {
                GoToState(useTransitions, VisualStates.StateMouseOver);
            }
            else
            {
                GoToState(useTransitions, VisualStates.StateNormal);
            }
        }

        /// <summary> 
        /// This code will run whenever Orientation changes, to change the template
        /// being used to display this control.
        /// </summary> 
        private void OnOrientationChanged()
        {
            double trackLength = GetTrackLength();

            if (ElementHorizontalTemplate != null)
            {
                ElementHorizontalTemplate.Visibility = (Orientation == Orientation.Horizontal ? Visibility.Visible : Visibility.Collapsed);
            }
            if (ElementVerticalTemplate != null)
            {
                ElementVerticalTemplate.Visibility = (Orientation == Orientation.Horizontal ? Visibility.Collapsed : Visibility.Visible);
            }
            UpdateTrackLayout(trackLength);
        }

        /// <summary>
        /// This method will take the current min, max, and value to
        /// calculate and layout the current control measurements. 
        /// </summary>
        private void UpdateTrackLayout(double trackLength)
        {
            double maximum = Maximum;
            double minimum = Minimum;
            double value = Value;
            double multiplier = (value - minimum) / (maximum - minimum);

            double thumbSize = UpdateThumbSize(trackLength);

            if (Orientation == Orientation.Horizontal && ElementHorizontalLargeDecrease != null && ElementHorizontalThumb != null)
            {
                ElementHorizontalLargeDecrease.Width = Math.Max(0.0f, multiplier * (trackLength - thumbSize));
            }
            else if (Orientation == Orientation.Vertical && ElementVerticalLargeDecrease != null && ElementVerticalThumb != null)
            {
                ElementVerticalLargeDecrease.Height = Math.Max(0.0f, multiplier * (trackLength - thumbSize));
            }
        }

        /// <summary> 
        /// Based on the size of the Large Increase/Decrease RepeatButtons 
        /// and on the Thumb, we will calculate the size of the track area
        /// of the ScrollBar 
        /// </summary>
        /// <returns>The length of the track</returns>
        internal double GetTrackLength()
        {
            double length = Double.NaN;

            if (Orientation == Orientation.Horizontal)
            {
                length = this.ActualWidth;

                if (ElementHorizontalSmallDecrease != null)
                {
                    length -= ElementHorizontalSmallDecrease.ActualWidth + ElementHorizontalSmallDecrease.Margin.Left + ElementHorizontalSmallDecrease.Margin.Right;
                }
                if (ElementHorizontalSmallIncrease != null)
                {
                    length -= ElementHorizontalSmallIncrease.ActualWidth + ElementHorizontalSmallIncrease.Margin.Left + ElementHorizontalSmallIncrease.Margin.Right;
                }
            }
            else
            {
                length = this.ActualHeight;

                if (ElementVerticalSmallDecrease != null)
                {
                    length -= ElementVerticalSmallDecrease.ActualHeight + ElementVerticalSmallDecrease.Margin.Top + ElementVerticalSmallDecrease.Margin.Bottom;
                }
                if (ElementVerticalSmallIncrease != null)
                {
                    length -= ElementVerticalSmallIncrease.ActualHeight + ElementVerticalSmallIncrease.Margin.Top + ElementVerticalSmallIncrease.Margin.Bottom;
                }
            }

            return length;
        }

        /// <summary>
        /// Based on the ViewportSize, the Track's length, and the
        /// Minimum and Maximum values, we will calculate the length 
        /// of the thumb.
        /// </summary>
        /// <returns>Double value representing display unit length</returns> 
        private double ConvertViewportSizeToDisplayUnits(double trackLength)
        {
            double viewRangeValue = Maximum - Minimum;

            return trackLength * ViewportSize / (ViewportSize + viewRangeValue);
        }

        /// <summary>
        /// This will resize the Thumb, based on calculations with the ViewportSize, 
        /// the Track's length, and the Minimum and Maximum values. 
        /// </summary>
        internal double UpdateThumbSize(double trackLength)
        {
            double result = Double.NaN;
            bool hideThumb = trackLength <= 0;

            if (trackLength > 0)
            {
                if (Orientation == Orientation.Horizontal && ElementHorizontalThumb != null)
                {
                    if (Maximum - Minimum != 0)
                    {
                        result = Math.Max(ElementHorizontalThumb.MinWidth, ConvertViewportSizeToDisplayUnits(trackLength));
                    }

                    // hide the thumb if too big
                    if (Maximum - Minimum == 0 || result > ActualWidth || trackLength <= ElementHorizontalThumb.MinWidth)
                    {
                        hideThumb = true;
                    }
                    else
                    {
                        ElementHorizontalThumb.Visibility = Visibility.Visible;
                        ElementHorizontalThumb.Width = result;
                    }
                }
                else if (Orientation == Orientation.Vertical && ElementVerticalThumb != null)
                {
                    if (Maximum - Minimum != 0)
                    {
                        result = Math.Max(ElementVerticalThumb.MinHeight, ConvertViewportSizeToDisplayUnits(trackLength));
                    }

                    // hide the thumb if too big
                    if (Maximum - Minimum == 0 || result > ActualHeight || trackLength <= ElementVerticalThumb.MinHeight)
                    {
                        hideThumb = true;
                    }
                    else
                    {
                        ElementVerticalThumb.Visibility = Visibility.Visible;
                        ElementVerticalThumb.Height = result;
                    }
                }
            }

            if (hideThumb)
            {
                if (ElementHorizontalThumb != null)
                {
                    ElementHorizontalThumb.Visibility = Visibility.Collapsed;
                }

                if (ElementVerticalThumb != null)
                {
                    ElementVerticalThumb.Visibility = Visibility.Collapsed;
                }
            }

            return result;
        }

        /// <summary>
        /// Horizontal template root 
        /// </summary>
        internal FrameworkElement ElementHorizontalTemplate { get; set; }
        internal const string ElementHorizontalTemplateName = "HorizontalRoot";

        /// <summary>
        /// Large increase repeat button 
        /// </summary>
        internal RepeatButton ElementHorizontalLargeIncrease { get; set; }
        internal const string ElementHorizontalLargeIncreaseName = "HorizontalLargeIncrease";

        /// <summary>
        /// Large decrease repeat button 
        /// </summary> 
        internal RepeatButton ElementHorizontalLargeDecrease { get; set; }
        internal const string ElementHorizontalLargeDecreaseName = "HorizontalLargeDecrease";

        /// <summary>
        /// Small increase repeat button 
        /// </summary>
        internal RepeatButton ElementHorizontalSmallIncrease { get; set; }
        internal const string ElementHorizontalSmallIncreaseName = "HorizontalSmallIncrease";

        /// <summary>
        /// Small decrease repeat button 
        /// </summary>
        internal RepeatButton ElementHorizontalSmallDecrease { get; set; }
        internal const string ElementHorizontalSmallDecreaseName = "HorizontalSmallDecrease";

        /// <summary>
        /// Thumb for dragging track 
        /// </summary> 
        internal Thumb ElementHorizontalThumb { get; set; }
        internal const string ElementHorizontalThumbName = "HorizontalThumb";

        /// <summary>
        /// Vertical template root 
        /// </summary>
        internal FrameworkElement ElementVerticalTemplate { get; set; }
        internal const string ElementVerticalTemplateName = "VerticalRoot";

        /// <summary>
        /// Large increase repeat button 
        /// </summary>
        internal RepeatButton ElementVerticalLargeIncrease { get; set; }
        internal const string ElementVerticalLargeIncreaseName = "VerticalLargeIncrease";

        /// <summary>
        /// Large decrease repeat button 
        /// </summary> 
        internal RepeatButton ElementVerticalLargeDecrease { get; set; }
        internal const string ElementVerticalLargeDecreaseName = "VerticalLargeDecrease";

        /// <summary>
        /// Small increase repeat button 
        /// </summary>
        internal RepeatButton ElementVerticalSmallIncrease { get; set; }
        internal const string ElementVerticalSmallIncreaseName = "VerticalSmallIncrease";

        /// <summary>
        /// Small decrease repeat button 
        /// </summary>
        internal RepeatButton ElementVerticalSmallDecrease { get; set; }
        internal const string ElementVerticalSmallDecreaseName = "VerticalSmallDecrease";

        /// <summary>
        /// Thumb for dragging track 
        /// </summary> 
        internal Thumb ElementVerticalThumb { get; set; }
        internal const string ElementVerticalThumbName = "VerticalThumb";

        // Maximum distance you can drag from thumb before it snaps back
        private const double MaxPerpendicularDelta = 150;

        private double _previousValue;
        private Vector _dragDelta;
        private bool _hasScrolled;  // Has the thumb been dragged

        #region Obsolete

        private DebounceDispatcher _debounceDispatcher;

        internal static TimeSpan DefaultDebounceInterval { get; set; } = TimeSpan.Zero;

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(Helper.ObsoleteMemberMessage + " Use ScrollViewer.IsDeferredScrollingEnabled instead.")]
        public static readonly DependencyProperty DebounceProperty =
            DependencyProperty.RegisterAttached(
                nameof(Debounce),
                typeof(TimeSpan?),
                typeof(ScrollBar),
                new PropertyMetadata((TimeSpan?)null));

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(Helper.ObsoleteMemberMessage + " Use ScrollViewer.IsDeferredScrollingEnabled instead.")]
        public TimeSpan Debounce
        {
            get => (TimeSpan?)GetValue(DebounceProperty) ?? DefaultDebounceInterval;
            set => SetValueInternal(DebounceProperty, value);
        }

#pragma warning disable CS0618 // Type or member is obsolete
        private TimeSpan DebounceInterval
        {
            get
            {
                // We attempt to get a debounce interval in 3 steps
                // 1 - From the ScrollBar.
                // 2 - From the ScrollBar's Templated parent (usually a ScrollViewer)
                // 3 - Attempt to get interval from the ScrollViewer's templated parent
                TimeSpan? debounce = (TimeSpan?)GetValue(DebounceProperty);
                if (debounce.HasValue)
                {
                    return debounce.Value;
                }

                if (TemplatedParent is FrameworkElement parent1)
                {
                    debounce = (TimeSpan?)parent1.GetValue(DebounceProperty);
                    if (debounce.HasValue)
                    {
                        return debounce.Value;
                    }

                    if (parent1.TemplatedParent is FrameworkElement parent2)
                    {
                        debounce = (TimeSpan?)parent2.GetValue(DebounceProperty);
                        if (debounce.HasValue)
                        {
                            return debounce.Value;
                        }
                    }
                }

                return DefaultDebounceInterval;
            }
        }
#pragma warning restore CS0618 // Type or member is obsolete

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(Helper.ObsoleteMemberMessage + " Use ScrollViewer.IsDeferredScrollingEnabled instead.")]
        public static TimeSpan GetDebounce(FrameworkElement fe)
        {
            ArgumentNullException.ThrowIfNull(fe);

            return (TimeSpan?)fe.GetValue(DebounceProperty) ?? DefaultDebounceInterval;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(Helper.ObsoleteMemberMessage + " Use ScrollViewer.IsDeferredScrollingEnabled instead.")]
        public static void SetDebounce(FrameworkElement fe, TimeSpan debounce)
        {
            ArgumentNullException.ThrowIfNull(fe);

            fe.SetValueInternal(DebounceProperty, (TimeSpan?)debounce);
        }

        #endregion Obsolete
    }
}
