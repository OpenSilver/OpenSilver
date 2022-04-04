

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
using System.Globalization;
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

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
    [TemplateVisualState(GroupName = "PopupStates", Name = "PopupClosed")]
    [TemplatePart(Name = "DropDownToggle", Type = typeof(ToggleButton))]
    [TemplateVisualState(GroupName = "PopupStates", Name = "PopupOpened")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "Normal")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "MouseOver")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "Pressed")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "Disabled")]
    [TemplateVisualState(GroupName = "FocusStates", Name = "Focused")]
    [TemplateVisualState(GroupName = "FocusStates", Name = "Unfocused")]
    [TemplatePart(Name = "Popup", Type = typeof(Popup))]
    public abstract class Picker : Control, IUpdateVisualState
    {
        /// <summary>Identifies the IsDropDownOpen dependency property.</summary>
        public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register(nameof(IsDropDownOpen), typeof(bool), typeof(Picker), new PropertyMetadata((object)false, new PropertyChangedCallback(Picker.OnIsDropDownOpenPropertyChanged)));
        /// <summary>Identifies the PopupButtonMode dependency property.</summary>
        public static readonly DependencyProperty PopupButtonModeProperty = DependencyProperty.Register(nameof(PopupButtonMode), typeof(ClickMode), typeof(Picker), new PropertyMetadata((object)ClickMode.Release, new PropertyChangedCallback(Picker.OnPopupButtonModePropertyChanged)));
        /// <summary>Identifies the MaxDropDownHeight dependency property.</summary>
        public static readonly DependencyProperty MaxDropDownHeightProperty = DependencyProperty.Register(nameof(MaxDropDownHeight), typeof(double), typeof(Picker), new PropertyMetadata((object)double.PositiveInfinity, new PropertyChangedCallback(Picker.OnMaxDropDownHeightPropertyChanged)));
        /// <summary>Name constant for Popup.</summary>
        internal const string ElementPopupName = "Popup";
        /// <summary>Name constant for DropDownToggle.</summary>
        internal const string ElementDropDownToggleName = "DropDownToggle";
        /// <summary>BackingField for PopupPart.</summary>
        private Popup _dropDownPopup;
        /// <summary>BackingField for DropDownTogglePart.</summary>
        private ToggleButton _dropDownToggleButton;
        /// <summary>BackingField for PopupChild.</summary>
        private FrameworkElement _popupChild;
        /// <summary>Gets or sets the expansive area outside of the popup.</summary>
        private Canvas _outsidePopupCanvas;
        /// <summary>Gets or sets the canvas for the popup child.</summary>
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

        /// <summary>Gets or sets the popup part.</summary>
        /// <value>The popup part.</value>
        internal Popup DropDownPopup
        {
            get
            {
                return this._dropDownPopup;
            }
            set
            {
                if (this._dropDownPopup != null)
                    this.PopupChild = (FrameworkElement)null;
                this._dropDownPopup = value;
                if (this._dropDownPopup == null)
                    return;
                this.PopupChild = this.DropDownPopup.Child as FrameworkElement;
            }
        }

        /// <summary>Gets or sets the drop down toggle part.</summary>
        /// <value>The drop down toggle part.</value>
        internal ToggleButton DropDownToggleButton
        {
            get
            {
                return this._dropDownToggleButton;
            }
            set
            {
                if (this._dropDownToggleButton != null)
                    this._dropDownToggleButton.Click -= new RoutedEventHandler(this.ToggleButtonClick);
                this._dropDownToggleButton = value;
                if (this._dropDownToggleButton == null)
                    return;
                this._dropDownToggleButton.Click += new RoutedEventHandler(this.ToggleButtonClick);
                this.DropDownToggleButton.ClickMode = this.PopupButtonMode;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the drop-down portion
        /// of the control is open.
        /// </summary>
        public bool IsDropDownOpen
        {
            get
            {
                return (bool)this.GetValue(Picker.IsDropDownOpenProperty);
            }
            set
            {
                this.SetValue(Picker.IsDropDownOpenProperty, (object)value);
            }
        }

        /// <summary>IsDropDownOpenProperty property changed handler.</summary>
        /// <param name="d">Picker that changed its IsDropDownOpen.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnIsDropDownOpenPropertyChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            Picker picker = (Picker)d;
            if (picker._ignorePropertyChange)
            {
                picker._ignorePropertyChange = false;
            }
            else
            {
                bool oldValue = (bool)e.OldValue;
                bool newValue = (bool)e.NewValue;
                bool closedVisualState = picker._popupClosedVisualState;
                RoutedPropertyChangingEventArgs<bool> e1 = new RoutedPropertyChangingEventArgs<bool>(e.Property, oldValue, newValue, true);
                if (newValue)
                {
                    picker.OnDropDownOpening(e1);
                    if (!e1.Cancel)
                        picker.OpenDropDown(oldValue, newValue);
                }
                else
                {
                    picker.OnDropDownClosing(e1);
                    if (!e1.Cancel && !closedVisualState)
                        picker.CloseDropDown(oldValue, newValue);
                }
                if (e1.Cancel)
                    picker.SetValue(e.Property, (object)oldValue);
                if (!closedVisualState)
                    return;
                picker.UpdateVisualState(true);
            }
        }

        /// <summary>
        /// Gets or sets the button event that causes the popup portion of the
        /// Picker control to open.
        /// </summary>
        public ClickMode PopupButtonMode
        {
            get
            {
                return (ClickMode)this.GetValue(Picker.PopupButtonModeProperty);
            }
            set
            {
                this.SetValue(Picker.PopupButtonModeProperty, (object)value);
            }
        }

        /// <summary>PopupButtonModeProperty property changed handler.</summary>
        /// <param name="d">Picker that changed its PopupButtonMode.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnPopupButtonModePropertyChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            Picker picker = (Picker)d;
            ClickMode newValue = (ClickMode)e.NewValue;
            int num;
            switch (newValue)
            {
                case ClickMode.Press:
                case ClickMode.Hover:
                    num = 1;
                    break;
                default:
                    num = newValue == ClickMode.Release ? 1 : 0;
                    break;
            }
            if (num == 0)
            {
                picker.SetValue(Picker.PopupButtonModeProperty, e.OldValue);
                throw new ArgumentOutOfRangeException(nameof(e), string.Format((IFormatProvider)CultureInfo.InvariantCulture, "Invalid value: {0}", (object)newValue));
            }
            if (picker.DropDownToggleButton == null)
                return;
            picker.DropDownToggleButton.ClickMode = newValue;
        }

        /// <summary>
        /// Gets or sets the maximum height of the drop-down portion of the
        /// Picker control.
        /// </summary>
        public double MaxDropDownHeight
        {
            get
            {
                return (double)this.GetValue(Picker.MaxDropDownHeightProperty);
            }
            set
            {
                this.SetValue(Picker.MaxDropDownHeightProperty, (object)value);
            }
        }

        /// <summary>MaxDropDownHeightProperty property changed handler.</summary>
        /// <param name="d">Picker that changed its MaxDropDownHeight.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnMaxDropDownHeightPropertyChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            Picker picker = (Picker)d;
            if (picker._ignorePropertyChange)
            {
                picker._ignorePropertyChange = false;
            }
            else
            {
                double newValue = (double)e.NewValue;
                if (newValue < 0.0)
                {
                    picker._ignorePropertyChange = true;
                    picker.SetValue(e.Property, e.OldValue);
                    throw new ArgumentException(string.Format((IFormatProvider)CultureInfo.InvariantCulture, "Invalid {0}", e.NewValue), "value");
                }
                picker.OnMaxDropDownHeightChanged(newValue);
            }
        }

        /// <summary>
        /// Gets the element shown in the drop down portion of the Picker control.
        /// </summary>
        public FrameworkElement PopupChild
        {
            get
            {
                return this._popupChild;
            }
            private set
            {
                if (this._popupChild != null)
                {
                    this.PopupChild.GotFocus -= new RoutedEventHandler(this.PopupChildGotFocus);
                    this.PopupChild.LostFocus -= new RoutedEventHandler(this.PopupChildLostFocus);
                    this.PopupChild.MouseEnter -= new MouseEventHandler(this.PopupChildMouseEnter);
                    this.PopupChild.MouseLeave -= new MouseEventHandler(this.PopupChildMouseLeave);
                    this.PopupChild.SizeChanged -= new SizeChangedEventHandler(this.PopupChildSizeChanged);
                    this._outsidePopupCanvas.MouseLeftButtonDown -= new MouseButtonEventHandler(this.OutsidePopupMouseLeftButtonDown);
                }
                this._popupChild = value;
                if (this._popupChild == null)
                    return;
                this._popupChildCanvas = new Canvas();
                this.DropDownPopup.Child = (UIElement)this._popupChildCanvas;
                this._outsidePopupCanvas = new Canvas();
                this._outsidePopupCanvas.Background = (Brush)new SolidColorBrush(Colors.Transparent);
                this._popupChildCanvas.Children.Add((UIElement)this._outsidePopupCanvas);
                this._popupChildCanvas.Children.Add((UIElement)this.PopupChild);
                this.PopupChild.GotFocus += new RoutedEventHandler(this.PopupChildGotFocus);
                this.PopupChild.LostFocus += new RoutedEventHandler(this.PopupChildLostFocus);
                this.PopupChild.MouseEnter += new MouseEventHandler(this.PopupChildMouseEnter);
                this.PopupChild.MouseLeave += new MouseEventHandler(this.PopupChildMouseLeave);
                this.PopupChild.SizeChanged += new SizeChangedEventHandler(this.PopupChildSizeChanged);
                this._outsidePopupCanvas.MouseLeftButtonDown += new MouseButtonEventHandler(this.OutsidePopupMouseLeftButtonDown);
            }
        }

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
        /// Initializes a new instance of the <see cref="T:System.Windows.Controls.Picker" /> class.
        /// </summary>
        protected Picker()
        {
            this.DefaultStyleKey = (object)typeof(Picker);
            this.Interaction = new InteractionHelper((Control)this);
            this.IsEnabledChanged += new DependencyPropertyChangedEventHandler(this.ControlIsEnabledChanged);
        }

        /// <summary>
        /// Builds the visual tree for the Picker control when a new template is
        /// applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            VisualStateGroup visualStateGroup1 = VisualStates.TryGetVisualStateGroup((DependencyObject)this, "PopupStates");
            if (null != visualStateGroup1)
            {
                visualStateGroup1.CurrentStateChanged -= new EventHandler<VisualStateChangedEventArgs>(this.OnPopupClosedStateChanged);
                this._popupClosedVisualState = false;
            }
            base.OnApplyTemplate();
            bool isDropDownOpen = this.IsDropDownOpen;
            VisualStateGroup visualStateGroup2 = VisualStates.TryGetVisualStateGroup((DependencyObject)this, "PopupStates");
            if (null != visualStateGroup2)
            {
                visualStateGroup2.CurrentStateChanged += new EventHandler<VisualStateChangedEventArgs>(this.OnPopupClosedStateChanged);
                this._popupClosedVisualState = true;
            }
            this.DropDownToggleButton = this.GetTemplateChild("DropDownToggle") as ToggleButton;
            this.DropDownPopup = this.GetTemplateChild("Popup") as Popup;
            this.Interaction.OnApplyTemplateBase();
            if (!isDropDownOpen)
                return;
            this.IsDropDownOpen = true;
        }

        /// <summary>Gets the selected value represented in the control.</summary>
        /// <returns>The value that is picked.</returns>
        public abstract object GetSelectedValue();

        /// <summary>Arrange the drop down popup.</summary>
        private void ArrangePopup()
        {
            if (this.DropDownPopup == null || this.PopupChild == null || (this._outsidePopupCanvas == null || Application.Current == null) || Application.Current.Host == null || Application.Current.Host.Content == null)
                return;
            Content content = Application.Current.Host.Content;
            double actualWidth1 = content.ActualWidth;
            double actualHeight1 = content.ActualHeight;
            this.PopupChild.Height = double.NaN;
            this.PopupChild.Width = double.NaN;
            this.PopupChild.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            double width = this.PopupChild.DesiredSize.Width;
            double height = this.PopupChild.DesiredSize.Height;
            if (actualHeight1 == 0.0 || actualWidth1 == 0.0 || width == 0.0 || height == 0.0)
                return;
            MatrixTransform matrixTransform = (MatrixTransform)null;
            try
            {
                matrixTransform = this.TransformToVisual((UIElement)null) as MatrixTransform;
            }
            catch
            {
                this.IsDropDownOpen = false;
            }
            if (matrixTransform == null)
                return;
            Matrix matrix = matrixTransform.Matrix;
            double offsetX = matrix.OffsetX;
            matrix = matrixTransform.Matrix;
            double offsetY = matrix.OffsetY;
            if (this.FlowDirection == FlowDirection.RightToLeft)
                offsetX -= this.ActualWidth;
            double actualHeight2 = this.ActualHeight;
            double actualWidth2 = this.ActualWidth;
            double num1 = this.MaxDropDownHeight;
            if (double.IsInfinity(num1) || double.IsNaN(num1))
                num1 = (actualHeight1 - actualHeight2) * 3.0 / 5.0;
            double val2_1 = Math.Min(width, actualWidth1);
            double num2 = Math.Min(height, num1);
            double num3 = Math.Max(actualWidth2, val2_1);
            double num4 = offsetX;
            if (actualWidth1 < num4 + num3)
                num4 = Math.Max(0.0, actualWidth1 - num3);
            bool flag = true;
            double num5 = offsetY + actualHeight2;
            if (actualHeight1 < num5 + num2)
            {
                flag = false;
                num5 = offsetY - num2;
                if (num5 < 0.0)
                {
                    if (offsetY < (actualHeight1 - actualHeight2) / 2.0)
                    {
                        flag = true;
                        num5 = offsetY + actualHeight2;
                    }
                    else
                    {
                        flag = false;
                        num5 = offsetY - num2;
                    }
                }
            }
            double val2_2 = flag ? Math.Min(actualHeight1 - num5, num1) : Math.Min(offsetY, num1);
            this.DropDownPopup.HorizontalOffset = 0.0;
            this.DropDownPopup.VerticalOffset = 0.0;
            this._outsidePopupCanvas.Width = actualWidth1;
            this._outsidePopupCanvas.Height = actualHeight1;
            matrixTransform.Matrix.Invert();
            this._outsidePopupCanvas.RenderTransform = (Transform)matrixTransform;
            this.PopupChild.MinWidth = actualWidth2;
            this.PopupChild.MaxWidth = actualWidth1;
            this.PopupChild.MinHeight = 0.0;
            this.PopupChild.MaxHeight = Math.Max(0.0, val2_2);
            this.PopupChild.Width = num3;
            this.PopupChild.HorizontalAlignment = HorizontalAlignment.Left;
            this.PopupChild.VerticalAlignment = VerticalAlignment.Top;
            Canvas.SetLeft((UIElement)this.PopupChild, num4 - offsetX);
            Canvas.SetTop((UIElement)this.PopupChild, num5 - offsetY);
        }

        /// <summary>
        /// Private method that directly opens the popup, checks the expander
        /// button, and then fires the Opened event.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OpenDropDown(bool oldValue, bool newValue)
        {
            if (this.DropDownPopup != null)
                this.DropDownPopup.IsOpen = true;
            if (this.DropDownToggleButton != null)
                this.DropDownToggleButton.IsChecked = new bool?(true);
            this._popupHasOpened = true;
            this.OnDropDownOpened(new RoutedPropertyChangedEventArgs<bool>(oldValue, newValue));
        }

        /// <summary>
        /// Private method that directly closes the popup, flips the Checked
        /// value, and then fires the Closed event.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void CloseDropDown(bool oldValue, bool newValue)
        {
            if (!this._popupHasOpened)
                return;
            if (this.DropDownPopup != null)
                this.DropDownPopup.IsOpen = false;
            if (this.DropDownToggleButton != null)
                this.DropDownToggleButton.IsChecked = new bool?(false);
            this.OnDropDownClosed(new RoutedPropertyChangedEventArgs<bool>(oldValue, newValue));
        }

        /// <summary>
        /// Opens or closes the popup if reacting to button press.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs" />
        /// instance containing the event data.</param>
        private void ToggleButtonClick(object sender, RoutedEventArgs e)
        {
            if (!this.IsEnabled || this.DropDownToggleButton == null)
                return;
            this.IsDropDownOpen = this.DropDownToggleButton.IsPressed;
        }

        /// <summary>
        /// Actually closes the popup after the VSM state animation completes.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event arguments.</param>
        private void OnPopupClosedStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            if (e == null || e.NewState == null)
                return;
            switch (e.NewState.Name)
            {
                case "PopupClosed":
                    this.CloseDropDown(true, false);
                    break;
            }
        }

        /// <summary>The mouse has clicked outside of the popup.</summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void OutsidePopupMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.IsDropDownOpen = false;
        }

        /// <summary>Handle the change of the IsEnabled property.</summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void ControlIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
                return;
            this.IsDropDownOpen = false;
        }

        /// <summary>
        /// Handles MaxDropDownHeightChanged by re-arranging and updating the
        /// popup arrangement.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        private void OnMaxDropDownHeightChanged(double newValue)
        {
            this.ArrangePopup();
            this.UpdateVisualState(true);
        }

        /// <summary>The popup child has received focus.</summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void PopupChildGotFocus(object sender, RoutedEventArgs e)
        {
            this.FocusChanged(this.HasFocus());
        }

        /// <summary>The popup child has lost focus.</summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void PopupChildLostFocus(object sender, RoutedEventArgs e)
        {
            this.FocusChanged(this.HasFocus());
        }

        /// <summary>The popup child has had the mouse enter its bounds.</summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void PopupChildMouseEnter(object sender, MouseEventArgs e)
        {
            this.UpdateVisualState(true);
        }

        /// <summary>The mouse has left the popup child's bounds.</summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void PopupChildMouseLeave(object sender, MouseEventArgs e)
        {
            if (this.PopupButtonMode == ClickMode.Hover)
                this.IsDropDownOpen = false;
            this.UpdateVisualState(true);
        }

        /// <summary>The size of the popup child has changed.</summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void PopupChildSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.ArrangePopup();
        }

        /// <summary>Raises the DropDownOpening event.</summary>
        /// <param name="e">
        /// Provides any observers the opportunity to cancel the operation and
        /// halt opening the drop down.
        /// </param>
        protected virtual void OnDropDownOpening(RoutedPropertyChangingEventArgs<bool> e)
        {
            RoutedPropertyChangingEventHandler<bool> dropDownOpening = this.DropDownOpening;
            if (dropDownOpening == null)
                return;
            dropDownOpening((object)this, e);
        }

        /// <summary>Raises the DropDownOpened event.</summary>
        /// <param name="e">The event data.</param>
        protected virtual void OnDropDownOpened(RoutedPropertyChangedEventArgs<bool> e)
        {
            RoutedPropertyChangedEventHandler<bool> dropDownOpened = this.DropDownOpened;
            if (dropDownOpened == null)
                return;
            dropDownOpened((object)this, e);
        }

        /// <summary>Raises the DropDownClosing event.</summary>
        /// <param name="e">
        /// Provides any observers the opportunity to cancel the operation
        /// and halt closing the drop down.
        /// </param>
        protected virtual void OnDropDownClosing(RoutedPropertyChangingEventArgs<bool> e)
        {
            RoutedPropertyChangingEventHandler<bool> dropDownClosing = this.DropDownClosing;
            if (dropDownClosing == null)
                return;
            dropDownClosing((object)this, e);
        }

        /// <summary>Raises the DropDownClosed event.</summary>
        /// <param name="e">The event data.</param>
        protected virtual void OnDropDownClosed(RoutedPropertyChangedEventArgs<bool> e)
        {
            RoutedPropertyChangedEventHandler<bool> dropDownClosed = this.DropDownClosed;
            if (dropDownClosed == null)
                return;
            dropDownClosed((object)this, e);
        }

        /// <summary>Handles the FocusChanged event.</summary>
        /// <param name="hasFocus">A value indicating whether the control
        /// currently has the focus.</param>
        private void FocusChanged(bool hasFocus)
        {
            if (hasFocus)
                return;
            this.IsDropDownOpen = false;
        }

        /// <summary>Checks to see if the control has focus currently.</summary>
        /// <returns>Returns a value indicating whether the control or its popup
        /// have focus.</returns>
        private new bool HasFocus()
        {
            DependencyObject parent;
            for (DependencyObject reference = FocusManager.GetFocusedElement() as DependencyObject; reference != null; reference = parent)
            {
                if (object.ReferenceEquals((object)reference, (object)this))
                    return true;
                parent = VisualTreeHelper.GetParent(reference);
                if (parent == null && reference is FrameworkElement frameworkElement)
                    parent = frameworkElement.Parent;
            }
            return false;
        }

        /// <summary>Provides handling for the GotFocus event.</summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if (!this.Interaction.AllowGotFocus(e))
                return;
            this.Interaction.OnGotFocusBase();
            base.OnGotFocus(e);
            this.FocusChanged(this.HasFocus());
        }

        /// <summary>Provides handling for the LostFocus event.</summary>
        /// <param name="e">The event data.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (!this.Interaction.AllowLostFocus(e))
                return;
            this.Interaction.OnLostFocusBase();
            base.OnLostFocus(e);
            this.FocusChanged(this.HasFocus());
        }

        /// <summary>Provides handling for the MouseEnter event.</summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (!this.Interaction.AllowMouseEnter(e))
                return;
            this.Interaction.OnMouseEnterBase();
            base.OnMouseEnter(e);
        }

        /// <summary>Provides handling for the MouseLeave event.</summary>
        /// <param name="e">The data for the event.</param>
        protected internal override void OnMouseLeave(MouseEventArgs e)
        {
            if (!this.Interaction.AllowMouseLeave(e))
                return;
            this.Interaction.OnMouseLeaveBase();
            base.OnMouseLeave(e);
        }

        /// <summary>Called before the MouseLeftButtonUp event occurs.</summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (!this.Interaction.AllowMouseLeftButtonUp(e))
                return;
            this.Interaction.OnMouseLeftButtonUpBase();
            base.OnMouseLeftButtonUp(e);
        }

        /// <summary>Called before the MouseLeftButtonDown event occurs.</summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (!this.Interaction.AllowMouseLeftButtonDown(e))
                return;
            this.Interaction.OnMouseLeftButtonDownBase();
            base.OnMouseLeftButtonDown(e);
        }

        void IUpdateVisualState.UpdateVisualState(bool useTransitions)
        {
            this.UpdateVisualState(useTransitions);
        }

        /// <summary>Update the current visual state of the button.</summary>
        /// <param name="useTransitions">
        /// True to use transitions when updating the visual state, false to
        /// snap directly to the new visual state.
        /// </param>
        internal virtual void UpdateVisualState(bool useTransitions)
        {
            VisualStateManager.GoToState((Control)this, this.IsDropDownOpen ? "PopupOpened" : "PopupClosed", useTransitions);
            this.Interaction.UpdateVisualStateBase(useTransitions);
        }
    }
}
