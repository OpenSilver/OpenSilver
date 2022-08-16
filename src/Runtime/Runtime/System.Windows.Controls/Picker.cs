// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

#if MIGRATION
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
#else
using Windows.Foundation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Interop;
using Windows.UI.Xaml.Media;
using MouseEventArgs = Windows.UI.Xaml.Input.PointerRoutedEventArgs;
using MouseButtonEventArgs = Windows.UI.Xaml.Input.PointerRoutedEventArgs;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Base class for all controls that have popup functionality.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [TemplateVisualState(Name = VisualStates.StatePopupClosed, GroupName = VisualStates.GroupPopup)]
    [TemplateVisualState(Name = VisualStates.StatePopupOpened, GroupName = VisualStates.GroupPopup)]

    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateMouseOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StatePressed, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateFocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateUnfocused, GroupName = VisualStates.GroupFocus)]

    [TemplatePartAttribute(Name = ElementPopupName, Type = typeof(Popup))]
    [TemplatePartAttribute(Name = ElementDropDownToggleName, Type = typeof(ToggleButton))]
    public abstract class Picker : Control, IUpdateVisualState
    {
#region Template parts name constants
        /// <summary>
        /// Name constant for Popup.
        /// </summary>
        internal const string ElementPopupName = "Popup";

        /// <summary>
        /// Name constant for DropDownToggle.
        /// </summary>
        internal const string ElementDropDownToggleName = "DropDownToggle";
#endregion Template parts name constants

#region TemplateParts
        /// <summary>
        /// Gets or sets the popup part.
        /// </summary>
        /// <value>The popup part.</value>
        internal Popup DropDownPopup
        {
            get { return _dropDownPopup; }
            set
            {
                if (_dropDownPopup != null)
                {
                    // allow popupchild to unhook.
                    PopupChild = null;
                }

                _dropDownPopup = value;

                if (_dropDownPopup != null)
                {
                    PopupChild = DropDownPopup.Child as FrameworkElement;
                }
            }
        }

        /// <summary>
        /// BackingField for PopupPart.
        /// </summary>
        private Popup _dropDownPopup;

        /// <summary>
        /// Gets or sets the drop down toggle part.
        /// </summary>
        /// <value>The drop down toggle part.</value>
        internal ToggleButton DropDownToggleButton
        {
            get { return _dropDownToggleButton; }
            set
            {
                if (_dropDownToggleButton != null)
                {
                    _dropDownToggleButton.Click -= ToggleButtonClick;
                }
                _dropDownToggleButton = value;
                if (_dropDownToggleButton != null)
                {
                    _dropDownToggleButton.Click += ToggleButtonClick;
                    DropDownToggleButton.ClickMode = PopupButtonMode;
                }
            }
        }

        /// <summary>
        /// BackingField for DropDownTogglePart.
        /// </summary>
        private ToggleButton _dropDownToggleButton;
#endregion

#region public bool IsDropDownOpen
        /// <summary>
        /// Gets or sets a value indicating whether the drop-down portion 
        /// of the control is open.
        /// </summary>
        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }

        /// <summary>
        /// Identifies the IsDropDownOpen dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register(
                "IsDropDownOpen",
                typeof(bool),
                typeof(Picker),
                new PropertyMetadata(false, OnIsDropDownOpenPropertyChanged));

        /// <summary>
        /// IsDropDownOpenProperty property changed handler.
        /// </summary>
        /// <param name="d">Picker that changed its IsDropDownOpen.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnIsDropDownOpenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Picker source = (Picker)d;

            // Ignore the change if requested
            if (source._ignorePropertyChange)
            {
                source._ignorePropertyChange = false;
                return;
            }

            bool oldValue = (bool)e.OldValue;
            bool newValue = (bool)e.NewValue;
            bool delayedClosingVisual = source._popupClosedVisualState;
            RoutedPropertyChangingEventArgs<bool> args = new RoutedPropertyChangingEventArgs<bool>(e.Property, oldValue, newValue, true);

            PickerAutomationPeer peer = FrameworkElementAutomationPeer.FromElement(source) as PickerAutomationPeer;
            if (peer != null)
            {
                peer.RaiseExpandCollapseAutomationEvent(oldValue, newValue);
            }

            if (newValue)
            {
                // Opening
                source.OnDropDownOpening(args);

                // Opened
                if (!args.Cancel)
                {
                    source.OpenDropDown(oldValue, newValue);
                }
            }
            else
            {
                // Closing
                source.OnDropDownClosing(args);

                // Immediately close the drop down window:
                // When a popup closed visual state is present, the code path is 
                // slightly different and the actual call to CloseDropDown will 
                // be called only after the visual state's transition is done
                if (!args.Cancel && !delayedClosingVisual)
                {
                    source.CloseDropDown(oldValue, newValue);
                }
            }

            // If canceled, revert the value change
            if (args.Cancel)
            {
                // source._ignorePropertyChange = true;
                source.SetValue(e.Property, oldValue);
            }

            // Closing call when visual states are in use
            if (delayedClosingVisual)
            {
                source.UpdateVisualState(true);
            }
        }
#endregion public bool IsDropDownOpen

#region public ClickMode PopupButtonMode
        /// <summary>
        /// Gets or sets the button event that causes the popup portion of the 
        /// Picker control to open.
        /// </summary>
        public ClickMode PopupButtonMode
        {
            get { return (ClickMode)GetValue(PopupButtonModeProperty); }
            set { SetValue(PopupButtonModeProperty, value); }
        }

        /// <summary>
        /// Identifies the PopupButtonMode dependency property.
        /// </summary>
        public static readonly DependencyProperty PopupButtonModeProperty =
            DependencyProperty.Register(
                "PopupButtonMode",
                typeof(ClickMode),
                typeof(Picker),
                new PropertyMetadata(ClickMode.Release, OnPopupButtonModePropertyChanged));

        /// <summary>
        /// PopupButtonModeProperty property changed handler.
        /// </summary>
        /// <param name="d">Picker that changed its PopupButtonMode.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnPopupButtonModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Picker source = (Picker)d;
            ClickMode newValue = (ClickMode)e.NewValue;

            if (newValue != ClickMode.Hover &&
                newValue != ClickMode.Press &&
                newValue != ClickMode.Release)
            {
                // revert to old value
                source.SetValue(PopupButtonModeProperty, e.OldValue);

                // todo: move to resource
                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Invalid value: {0}",
                    newValue);

                throw new ArgumentOutOfRangeException("e", message);
            }

            if (source.DropDownToggleButton != null)
            {
                source.DropDownToggleButton.ClickMode = newValue;
            }
        }
#endregion public ClickMode PopupButtonMode

#region public double MaxDropDownHeight
        /// <summary>
        /// Gets or sets the maximum height of the drop-down portion of the 
        /// Picker control.
        /// </summary>
        public double MaxDropDownHeight
        {
            get { return (double)GetValue(MaxDropDownHeightProperty); }
            set { SetValue(MaxDropDownHeightProperty, value); }
        }

        /// <summary>
        /// Identifies the MaxDropDownHeight dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxDropDownHeightProperty =
            DependencyProperty.Register(
                "MaxDropDownHeight",
                typeof(double),
                typeof(Picker),
                new PropertyMetadata(double.PositiveInfinity, OnMaxDropDownHeightPropertyChanged));

        /// <summary>
        /// MaxDropDownHeightProperty property changed handler.
        /// </summary>
        /// <param name="d">Picker that changed its MaxDropDownHeight.</param>
        /// <param name="e">Event arguments.</param>
        [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification = "The exception will be called through a CLR setter in most cases.")]
        private static void OnMaxDropDownHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Picker source = (Picker)d;
            if (source._ignorePropertyChange)
            {
                source._ignorePropertyChange = false;
                return;
            }

            double newValue = (double)e.NewValue;

            // Revert to the old value if invalid (negative)
            if (newValue < 0)
            {
                source._ignorePropertyChange = true;
                source.SetValue(e.Property, e.OldValue);

                throw new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Invalid maximum drop down height value '{0}'. The value must be greater than or equal to zero.", e.NewValue),
                    "value");
            }

            source.OnMaxDropDownHeightChanged(newValue);
        }
#endregion public double MaxDropDownHeight

        /// <summary>
        /// Gets the element shown in the drop down portion of the Picker control.
        /// </summary>
        public FrameworkElement PopupChild
        {
            get { return _popupChild; }
            private set
            {
                if (_popupChild != null)
                {
                    PopupChild.GotFocus -= PopupChildGotFocus;
                    PopupChild.LostFocus -= PopupChildLostFocus;
#if MIGRATION
                    PopupChild.MouseEnter -= PopupChildMouseEnter;
                    PopupChild.MouseLeave -= PopupChildMouseLeave;
#else
                    PopupChild.PointerEntered -= PopupChildMouseEnter;
                    PopupChild.PointerExited -= PopupChildMouseLeave;
#endif
                    PopupChild.SizeChanged -= PopupChildSizeChanged;
#if MIGRATION
                    _outsidePopupCanvas.MouseLeftButtonDown -= OutsidePopupMouseLeftButtonDown;
#else
                    _outsidePopupCanvas.PointerPressed -= OutsidePopupMouseLeftButtonDown;
#endif
                }

                _popupChild = value;

                if (_popupChild != null)
                {
                    // Replace the popup child with a canvas
                    _popupChildCanvas = new Canvas();
                    DropDownPopup.Child = _popupChildCanvas;

                    _outsidePopupCanvas = new Canvas();
                    _outsidePopupCanvas.Background = new SolidColorBrush(Colors.Transparent);

                    _popupChildCanvas.Children.Add(_outsidePopupCanvas);
                    _popupChildCanvas.Children.Add(PopupChild);

                    PopupChild.GotFocus += PopupChildGotFocus;
                    PopupChild.LostFocus += PopupChildLostFocus;
#if MIGRATION
                    PopupChild.MouseEnter += PopupChildMouseEnter;
                    PopupChild.MouseLeave += PopupChildMouseLeave;
#else
                    PopupChild.PointerEntered += PopupChildMouseEnter;
                    PopupChild.PointerExited += PopupChildMouseLeave;
#endif
                    PopupChild.SizeChanged += PopupChildSizeChanged;
#if MIGRATION
                    _outsidePopupCanvas.MouseLeftButtonDown += OutsidePopupMouseLeftButtonDown;
#else
                    _outsidePopupCanvas.PointerPressed += OutsidePopupMouseLeftButtonDown;
#endif
                }
            }
        }

        /// <summary>
        /// BackingField for PopupChild.
        /// </summary>
        private FrameworkElement _popupChild;

        /// <summary>
        /// Gets or sets the expansive area outside of the popup.
        /// </summary>
        private Canvas _outsidePopupCanvas;

        /// <summary>
        /// Gets or sets the canvas for the popup child.
        /// </summary>
        private Canvas _popupChildCanvas;

        /// <summary>
        /// Gets or sets a value indicating whether to ignore calling a pending 
        /// change handlers. 
        /// </summary>
        private bool _ignorePropertyChange;

        /// <summary>
        /// Gets or sets a value indicating whether a visual popup state is 
        /// being used in the current template for the Closed state. Setting 
        /// this value to true will delay the actual setting of Popup.IsOpen 
        /// to false until after the visual state's transition for Closed is 
        /// complete.
        /// </summary>
        private bool _popupClosedVisualState;

        /// <summary>
        /// A value indicating whether the popup has been opened at least once.
        /// </summary>
        private bool _popupHasOpened;

        /// <summary>
        /// Gets or sets the helper that provides all of the standard
        /// interaction functionality. Making it internal for subclass access.
        /// </summary>
        internal InteractionHelper Interaction { get; set; }

        /// <summary>
        /// Occurs when the value of the IsDropDownOpen property is changing from 
        /// false to true.
        /// </summary>
        public event RoutedPropertyChangingEventHandler<bool> DropDownOpening;

        /// <summary>
        /// Occurs when the value of the IsDropDownOpen property has changed from 
        /// false to true and the drop-down is open.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<bool> DropDownOpened;

        /// <summary>
        /// Occurs when the IsDropDownOpen property is changing from true to false.
        /// </summary>
        public event RoutedPropertyChangingEventHandler<bool> DropDownClosing;

        /// <summary>
        /// Occurs when the IsDropDownOpen property was changed from true to false 
        /// and the drop-down is open.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<bool> DropDownClosed;

        /// <summary>
        /// Initializes a new instance of the <see cref="Picker"/> class.
        /// </summary>
        protected Picker()
        {
            DefaultStyleKey = typeof(Picker);
            Interaction = new InteractionHelper(this);

            IsEnabledChanged += ControlIsEnabledChanged;
        }

        /// <summary>
        /// Builds the visual tree for the Picker control when a new template is 
        /// applied.
        /// </summary>
#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

            // Unhook the event handler for the popup closed visual state group.
            // This code is used to enable visual state transitions before 
            // actually setting the underlying Popup.IsOpen property to false.
            VisualStateGroup groupPopupClosed = VisualStates.TryGetVisualStateGroup(this, VisualStates.GroupPopup);
            if (null != groupPopupClosed)
            {
                groupPopupClosed.CurrentStateChanged -= OnPopupClosedStateChanged;
                _popupClosedVisualState = false;
            }

            base.OnApplyTemplate();

            bool dropDownOpenInitialValue = IsDropDownOpen;

            // todo: having the group does not mean having the closed state, which seems to be assumed.
            groupPopupClosed = VisualStates.TryGetVisualStateGroup(this, VisualStates.GroupPopup);
            if (null != groupPopupClosed)
            {
                groupPopupClosed.CurrentStateChanged += OnPopupClosedStateChanged;
                _popupClosedVisualState = true;
            }

            // Set the template parts. Individual part setters remove and add 
            // any event handlers.
            DropDownToggleButton = GetTemplateChild(ElementDropDownToggleName) as ToggleButton;
            DropDownPopup = GetTemplateChild(ElementPopupName) as Popup;

            Interaction.OnApplyTemplateBase();

            // initialization has finished, we have all the necessary references
            // possibly open DropDown.
            if (dropDownOpenInitialValue)
            {
                IsDropDownOpen = true;
            }
        }

        /// <summary>
        /// Gets the selected value represented in the control.
        /// </summary>
        /// <returns>The value that is picked.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Decided on a method.")]
        public abstract object GetSelectedValue();

#region Pro active
        /// <summary>
        /// Arrange the drop down popup.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "This try-catch pattern is used by other popup controls to keep the runtime up.")]
        private void ArrangePopup()
        {
            if (DropDownPopup == null || PopupChild == null || _outsidePopupCanvas == null || Application.Current == null || Application.Current.Host == null || Application.Current.Host.Content == null)
            {
                return;
            }

            // get a reference to the host, which represent the silverlight plugin.
            Content hostContent = Application.Current.Host.Content;
            double rootWidth = hostContent.ActualWidth;
            double rootHeight = hostContent.ActualHeight;

            // The PopupChild was assigned a Width/Height in a previous ArrangePass
            // that needs to be undone to get correct figures.
            PopupChild.Height = Double.NaN;
            PopupChild.Width = Double.NaN;
            PopupChild.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

            double popupContentWidth = PopupChild.DesiredSize.Width;
            double popupContentHeight = PopupChild.DesiredSize.Height;

            if (rootHeight == 0 || rootWidth == 0 || popupContentWidth == 0 || popupContentHeight == 0)
            {
                return;
            }

            // Getting the transform will throw if the popup is no longer in 
            // the visual tree.  This can happen if you first open the popup 
            // and then click on something else on the page that removes it 
            // from the live tree.
            MatrixTransform mt = null;
            try
            {
                mt = TransformToVisual(null) as MatrixTransform;
            }
            catch
            {
                IsDropDownOpen = false;
            }
            if (mt == null)
            {
                return;
            }

            double rootOffsetX = mt.Matrix.OffsetX;
            double rootOffsetY = mt.Matrix.OffsetY;
            if (FlowDirection == FlowDirection.RightToLeft)
            {
                // Adjust rootOffsetX to be the left corner as expected by the layout code below
                rootOffsetX -= ActualWidth;
            }

            double myControlHeight = ActualHeight;
            double myControlWidth = ActualWidth;

            // TODO: Revisit the magic numbers

            // Use or come up with a maximum popup height.
            double popupMaxHeight = MaxDropDownHeight;
            if (double.IsInfinity(popupMaxHeight) || double.IsNaN(popupMaxHeight))
            {
                popupMaxHeight = (rootHeight - myControlHeight) * 3 / 5;
            }

            popupContentWidth = Math.Min(popupContentWidth, rootWidth);
            popupContentHeight = Math.Min(popupContentHeight, popupMaxHeight);
            popupContentWidth = Math.Max(myControlWidth, popupContentWidth);

            // We prefer to align the popup box with the left edge of the 
            // control, if it will fit.
            double popupX = rootOffsetX;
            if (rootWidth < popupX + popupContentWidth)
            {
                // Since it doesn't fit when strictly left aligned, we shift it 
                // to the left until it does fit.
                popupX = rootWidth - popupContentWidth;
                popupX = Math.Max(0, popupX);
            }

            // We prefer to put the popup below the combobox if it will fit.
            bool below = true;
            double popupY = rootOffsetY + myControlHeight;
            if (rootHeight < popupY + popupContentHeight)
            {
                below = false;
                // It doesn't fit below the combobox, lets try putting it above 
                // the combobox.
                popupY = rootOffsetY - popupContentHeight;
                if (popupY < 0)
                {
                    // doesn't really fit below either.  Now we just pick top 
                    // or bottom based on wich area is bigger.
                    if (rootOffsetY < (rootHeight - myControlHeight) / 2)
                    {
                        below = true;
                        popupY = rootOffsetY + myControlHeight;
                    }
                    else
                    {
                        below = false;
                        popupY = rootOffsetY - popupContentHeight;
                    }
                }
            }

            // Now that we have positioned the popup we may need to truncate 
            // its size.
            popupMaxHeight = below ? Math.Min(rootHeight - popupY, popupMaxHeight) : Math.Min(rootOffsetY, popupMaxHeight);

            DropDownPopup.HorizontalOffset = 0;
            DropDownPopup.VerticalOffset = 0;

            _outsidePopupCanvas.Width = rootWidth;
            _outsidePopupCanvas.Height = rootHeight;

            // Transform the transparent canvas to the plugin's coordinate 
            // space origin.
            Matrix transformToRootMatrix = mt.Matrix;
            Matrix newMatrix;
            transformToRootMatrix.Invert(out newMatrix);
            mt.Matrix = newMatrix;
            _outsidePopupCanvas.RenderTransform = mt;

            PopupChild.MinWidth = myControlWidth;
            PopupChild.MaxWidth = rootWidth;
            PopupChild.MinHeight = 0;
            PopupChild.MaxHeight = Math.Max(0, popupMaxHeight);

            PopupChild.Width = popupContentWidth;
            // TODO: RESEARCH: This next line was commented out previously
            // PopupChild.Height = popupContentHeight;
            PopupChild.HorizontalAlignment = HorizontalAlignment.Left;
            PopupChild.VerticalAlignment = VerticalAlignment.Top;

            // Set the top left corner for the actual drop down.
            Canvas.SetLeft(PopupChild, popupX - rootOffsetX);
            Canvas.SetTop(PopupChild, popupY - rootOffsetY);
        }

        /// <summary>
        /// Private method that directly opens the popup, checks the expander 
        /// button, and then fires the Opened event.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OpenDropDown(bool oldValue, bool newValue)
        {
            if (DropDownPopup != null)
            {
                DropDownPopup.IsOpen = true;
            }
            if (DropDownToggleButton != null)
            {
                DropDownToggleButton.IsChecked = true;
            }

            _popupHasOpened = true;

            RoutedPropertyChangedEventArgs<bool> e = new RoutedPropertyChangedEventArgs<bool>(oldValue, newValue);
            OnDropDownOpened(e);
        }

        /// <summary>
        /// Private method that directly closes the popup, flips the Checked 
        /// value, and then fires the Closed event.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void CloseDropDown(bool oldValue, bool newValue)
        {
            if (_popupHasOpened)
            {
                if (DropDownPopup != null)
                {
                    DropDownPopup.IsOpen = false;
                }
                if (DropDownToggleButton != null)
                {
                    DropDownToggleButton.IsChecked = false;
                }

                RoutedPropertyChangedEventArgs<bool> e = new RoutedPropertyChangedEventArgs<bool>(oldValue, newValue);
                OnDropDownClosed(e);
            }
        }
#endregion Pro active

#region Reactive
        /// <summary>
        /// Opens or closes the popup if reacting to button press.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> 
        /// instance containing the event data.</param>
        private void ToggleButtonClick(object sender, RoutedEventArgs e)
        {
            if (IsEnabled && DropDownToggleButton != null)
            {
                IsDropDownOpen = DropDownToggleButton.IsPressed;
            }
        }

        /// <summary>
        /// Actually closes the popup after the VSM state animation completes.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event arguments.</param>
        private void OnPopupClosedStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            // validate
            if (e == null || e.NewState == null)
            {
                return;
            }

            switch (e.NewState.Name)
            {
                // Delayed closing of the popup until now
                case VisualStates.StatePopupClosed:
                    CloseDropDown(true, false);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// The mouse has clicked outside of the popup.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void OutsidePopupMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IsDropDownOpen = false;
        }

        /// <summary>
        /// Handle the change of the IsEnabled property.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void ControlIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // todo: check for null of e

            bool isEnabled = (bool)e.NewValue;
            if (!isEnabled)
            {
                IsDropDownOpen = false;
            }
        }

        /// <summary>
        /// Handles MaxDropDownHeightChanged by re-arranging and updating the 
        /// popup arrangement.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "newValue", Justification = "This makes it easy to add validation or other changes in the future.")]
        private void OnMaxDropDownHeightChanged(double newValue)
        {
            ArrangePopup();
            UpdateVisualState(true);
        }

        /// <summary>
        /// The popup child has received focus.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void PopupChildGotFocus(object sender, RoutedEventArgs e)
        {
            FocusChanged(HasFocus());
        }

        /// <summary>
        /// The popup child has lost focus.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void PopupChildLostFocus(object sender, RoutedEventArgs e)
        {
            FocusChanged(HasFocus());
        }

        /// <summary>
        /// The popup child has had the mouse enter its bounds.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void PopupChildMouseEnter(object sender, MouseEventArgs e)
        {
            UpdateVisualState(true);
        }

        /// <summary>
        /// The mouse has left the popup child's bounds.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void PopupChildMouseLeave(object sender, MouseEventArgs e)
        {
            if (PopupButtonMode == ClickMode.Hover)
            {
                IsDropDownOpen = false;
            }

            UpdateVisualState(true);
        }

        /// <summary>
        /// The size of the popup child has changed.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void PopupChildSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ArrangePopup();
        }
#endregion Reactive

#region Raise DropDown Events
        /// <summary>
        /// Raises the DropDownOpening event.
        /// </summary>
        /// <param name="e">
        /// Provides any observers the opportunity to cancel the operation and 
        /// halt opening the drop down.
        /// </param>
        protected virtual void OnDropDownOpening(RoutedPropertyChangingEventArgs<bool> e)
        {
            RoutedPropertyChangingEventHandler<bool> handler = DropDownOpening;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the DropDownOpened event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void OnDropDownOpened(RoutedPropertyChangedEventArgs<bool> e)
        {
            RoutedPropertyChangedEventHandler<bool> handler = DropDownOpened;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the DropDownClosing event.
        /// </summary>
        /// <param name="e">
        /// Provides any observers the opportunity to cancel the operation 
        /// and halt closing the drop down.
        /// </param>
        protected virtual void OnDropDownClosing(RoutedPropertyChangingEventArgs<bool> e)
        {
            RoutedPropertyChangingEventHandler<bool> handler = DropDownClosing;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the DropDownClosed event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void OnDropDownClosed(RoutedPropertyChangedEventArgs<bool> e)
        {
            RoutedPropertyChangedEventHandler<bool> handler = DropDownClosed;
            if (handler != null)
            {
                handler(this, e);
            }
        }
#endregion Raise DropDown Events

#region Focus
        /// <summary>
        /// Handles the FocusChanged event.
        /// </summary>
        /// <param name="hasFocus">A value indicating whether the control 
        /// currently has the focus.</param>
        private void FocusChanged(bool hasFocus)
        {
            // todo: Every call to FocusChanged passes in HasFocused().  Perhaps FocusChanged should do that.

            // The OnGotFocus & OnLostFocus are asynchronously and cannot 
            // reliably tell you that have the focus.  All they do is let you 
            // know that the focus changed sometime in the past.  To determine 
            // if you currently have the focus you need to do consult the 
            // FocusManager (see HasFocus()).

            if (!hasFocus)
            {
                IsDropDownOpen = false;
            }
        }

        /// <summary>
        /// Checks to see if the control has focus currently.
        /// </summary>
        /// <returns>Returns a value indicating whether the control or its popup
        /// have focus.</returns>
        private bool HasFocus()
        {
            DependencyObject focused = FocusManager.GetFocusedElement() as DependencyObject;
            while (focused != null)
            {
                if (object.ReferenceEquals(focused, this))
                {
                    return true;
                }

                // This helps deal with popups that may not be in the same 
                // visual tree
                DependencyObject parent = VisualTreeHelper.GetParent(focused);
                if (parent == null)
                {
                    // Try the logical parent.
                    FrameworkElement element = focused as FrameworkElement;
                    if (element != null)
                    {
                        parent = element.Parent;
                    }
                }
                focused = parent;
            }
            return false;
        }

        /// <summary>
        /// Provides handling for the GotFocus event.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if (Interaction.AllowGotFocus(e))
            {
                Interaction.OnGotFocusBase();
                base.OnGotFocus(e);
                FocusChanged(HasFocus());
            }
        }

        /// <summary>
        /// Provides handling for the LostFocus event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (Interaction.AllowLostFocus(e))
            {
                Interaction.OnLostFocusBase();
                base.OnLostFocus(e);
                FocusChanged(HasFocus());
            }
        }
#endregion

        /// <summary>
        /// Provides handling for the MouseEnter event.
        /// </summary>
        /// <param name="e">The data for the event.</param>
#if MIGRATION
        protected override void OnMouseEnter(MouseEventArgs e)
#else
        protected override void OnPointerEntered(MouseEventArgs e)
#endif
        {
            if (Interaction.AllowMouseEnter(e))
            {
                Interaction.OnMouseEnterBase();
#if MIGRATION
                base.OnMouseEnter(e);
#else
                base.OnPointerEntered(e);
#endif
            }
        }

        /// <summary>
        /// Provides handling for the MouseLeave event.
        /// </summary>
        /// <param name="e">The data for the event.</param>
#if MIGRATION
        protected internal override void OnMouseLeave(MouseEventArgs e)
#else
        protected internal override void OnPointerExited(MouseEventArgs e)
#endif
        {
            if (Interaction.AllowMouseLeave(e))
            {
                Interaction.OnMouseLeaveBase();
#if MIGRATION
                base.OnMouseLeave(e);
#else
                base.OnPointerExited(e);
#endif
            }
        }

        /// <summary>
        /// Called before the MouseLeftButtonUp event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
#if MIGRATION
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
#else
        protected override void OnPointerReleased(MouseButtonEventArgs e)
#endif
        {
            if (Interaction.AllowMouseLeftButtonUp(e))
            {
                Interaction.OnMouseLeftButtonUpBase();
#if MIGRATION
                base.OnMouseLeftButtonUp(e);
#else
                base.OnPointerReleased(e);
#endif
            }
        }

        /// <summary>
        /// Called before the MouseLeftButtonDown event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
#if MIGRATION
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
#else
        protected override void OnPointerPressed(MouseButtonEventArgs e)
#endif
        {
            if (Interaction.AllowMouseLeftButtonDown(e))
            {
                Interaction.OnMouseLeftButtonDownBase();
#if MIGRATION
                base.OnMouseLeftButtonDown(e);
#else
                base.OnPointerPressed(e);
#endif
            }
        }

#region Visual state management
        /// <summary>
        /// Update the visual state of the control.
        /// </summary>
        /// <param name="useTransitions">
        /// A value indicating whether to automatically generate transitions to
        /// the new state, or instantly transition to the new state.
        /// </param>
        void IUpdateVisualState.UpdateVisualState(bool useTransitions)
        {
            UpdateVisualState(useTransitions);
        }

        /// <summary>
        /// Update the current visual state of the button.
        /// </summary>
        /// <param name="useTransitions">
        /// True to use transitions when updating the visual state, false to
        /// snap directly to the new visual state.
        /// </param>
        internal virtual void UpdateVisualState(bool useTransitions)
        {
            // Popup
            VisualStateManager.GoToState(this, IsDropDownOpen ? VisualStates.StatePopupOpened : VisualStates.StatePopupClosed, useTransitions);

            // Handle the Common and Focused states
            Interaction.UpdateVisualStateBase(useTransitions);
        }
#endregion Visual state management
    }
}