// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.Windows.Markup;

#if MIGRATION
using System.Windows.Controls.Primitives;
using System.Windows.Input;
#else
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a spinner control that includes two Buttons.
    /// </summary>
    /// <remarks>
    /// ButtonSpinner inherits from Spinner. 
    /// It adds two button template parts and a content property.
    /// </remarks>
    /// <QualityBand>Preview</QualityBand>
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateMouseOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StatePressed, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]

    [TemplateVisualState(Name = VisualStates.StateFocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateUnfocused, GroupName = VisualStates.GroupFocus)]

    [TemplateVisualState(Name = VisualStates.StateIncreaseEnabled, GroupName = VisualStates.GroupIncrease)]
    [TemplateVisualState(Name = VisualStates.StateIncreaseDisabled, GroupName = VisualStates.GroupIncrease)]

    [TemplateVisualState(Name = VisualStates.StateDecreaseEnabled, GroupName = VisualStates.GroupDecrease)]
    [TemplateVisualState(Name = VisualStates.StateDecreaseDisabled, GroupName = VisualStates.GroupDecrease)]

    [TemplatePart(Name = ButtonSpinner.ElementIncreaseButtonName, Type = typeof(ButtonBase))]
    [TemplatePart(Name = ButtonSpinner.ElementDecreaseButtonName, Type = typeof(ButtonBase))]

    [ContentProperty("Content")]
    public partial class ButtonSpinner : Spinner
    {
        #region template parts
        /// <summary>
        /// Name constant of the IncreaseButton template part.
        /// </summary>
        private const string ElementIncreaseButtonName = "IncreaseButton";

        /// <summary>
        /// Name constant of the DecreaseButton template part.
        /// </summary>
        private const string ElementDecreaseButtonName = "DecreaseButton";

        /// <summary>
        /// Private field for IncreaseButton template part.
        /// </summary>
        private ButtonBase _increaseButton;

        /// <summary>
        /// Gets or sets the IncreaseButton template part.
        /// </summary>
        private ButtonBase IncreaseButton
        {
            get { return _increaseButton; }
            set
            {
                if (_increaseButton != null)
                {
                    _increaseButton.Click -= OnButtonClick;
                }

                _increaseButton = value;

                if (_increaseButton != null)
                {
                    _increaseButton.Click += OnButtonClick;
                }
            }
        }

        /// <summary>
        /// Private field for DecreaseButton template part.
        /// </summary>
        private ButtonBase _decreaseButton;

        /// <summary>
        /// Gets or sets the DecreaseButton template part.
        /// </summary>
        private ButtonBase DecreaseButton
        {
            get { return _decreaseButton; }
            set
            {
                if (_decreaseButton != null)
                {
                    _decreaseButton.Click -= OnButtonClick;
                }

                _decreaseButton = value;

                if (_decreaseButton != null)
                {
                    _decreaseButton.Click += OnButtonClick;
                }
            }
        }
        #endregion

        #region public object Content
        /// <summary>
        /// Gets or sets the content that is contained within the button spinner.
        /// </summary>
        public object Content
        {
            get { return GetValue(ContentProperty) as object; }
            set { SetValue(ContentProperty, value); }
        }

        /// <summary>
        /// Identifies the Content dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(
                "Content",
                typeof(object),
                typeof(ButtonSpinner),
                new PropertyMetadata(null, OnContentPropertyChanged));

        /// <summary>
        /// ContentProperty property changed handler.
        /// </summary>
        /// <param name="d">ButtonSpinner that changed its Content.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ButtonSpinner source = d as ButtonSpinner;
            source.OnContentChanged(e.OldValue, e.NewValue);
        }
        #endregion public object Content

        /// <summary>
        /// Initializes a new instance of the ButtonSpinner class.
        /// </summary>
        public ButtonSpinner() : base()
        {
            DefaultStyleKey = typeof(ButtonSpinner);
        }

        /// <summary>
        /// Builds the visual tree for the ButtonSpinner control when a new 
        /// template is applied.
        /// </summary>
#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            IncreaseButton = GetTemplateChild(ElementIncreaseButtonName) as ButtonBase;
            DecreaseButton = GetTemplateChild(ElementDecreaseButtonName) as ButtonBase;
            Interaction.OnApplyTemplateBase();

            UpdateVisualState(false);

            SetButtonUsage();
        }

        /// <summary>
        /// Occurs when the Content property value changed.
        /// </summary>
        /// <param name="oldValue">The old value of the Content property.</param>
        /// <param name="newValue">The new value of the Content property.</param>
        protected virtual void OnContentChanged(object oldValue, object newValue)
        {
        }

        /// <summary>
        /// Handle click event of IncreaseButton and DecreaseButton template parts,
        /// translating Click to appropriate Spin event.
        /// </summary>
        /// <param name="sender">Event sender, should be either IncreaseButton or DecreaseButton template part.</param>
        /// <param name="e">Event args.</param>
        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            Debug.Assert(
                sender == IncreaseButton || sender == DecreaseButton,
                "This can't happen: OnButtonClick is called on neither IncreaseButton nor DecreaseButton!");

            SpinDirection direction = sender == IncreaseButton ? SpinDirection.Increase : SpinDirection.Decrease;

            OnSpin(new SpinEventArgs(direction));
        }

        /// <summary>
        /// Cancel LeftMouseButtonUp events originating from a button that has
        /// been changed to disabled.
        /// </summary>
        /// <param name="e">The data for the event.</param>
#if MIGRATION
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
#else
        protected override void OnPointerReleased(PointerRoutedEventArgs e)
#endif
        {
#if MIGRATION
            base.OnMouseLeftButtonUp(e);
#else
            base.OnPointerReleased(e);
#endif
            Point mousePosition;
            if (IncreaseButton != null && IncreaseButton.IsEnabled == false)
            {
#if MIGRATION
                mousePosition = e.GetPosition(IncreaseButton);
#else
                mousePosition = e.GetCurrentPoint(IncreaseButton).Position;
#endif
                if (mousePosition.X > 0 && mousePosition.X < IncreaseButton.ActualWidth &&
                    mousePosition.Y > 0 && mousePosition.Y < IncreaseButton.ActualHeight)
                {
                    e.Handled = true;
                }
            }

            if (DecreaseButton != null && DecreaseButton.IsEnabled == false)
            {
#if MIGRATION
                mousePosition = e.GetPosition(DecreaseButton);
#else
                mousePosition = e.GetCurrentPoint(DecreaseButton).Position;
#endif

                if (mousePosition.X > 0 && mousePosition.X < DecreaseButton.ActualWidth &&
                    mousePosition.Y > 0 && mousePosition.Y < DecreaseButton.ActualHeight)
                {
                    e.Handled = true;
                }
            }
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
            }
        }

        /// <summary>
        /// Provides handling for the LostFocus event.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (Interaction.AllowLostFocus(e))
            {
                Interaction.OnLostFocusBase();
                base.OnLostFocus(e);
            }
        }

        /// <summary>
        /// Provides handling for the MouseEnter event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
#if MIGRATION
        protected override void OnMouseEnter(MouseEventArgs e)
#else
        protected override void OnPointerEntered(PointerRoutedEventArgs e)
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
        /// <param name="e">Event arguments.</param>
#if MIGRATION
        protected override void OnMouseLeave(MouseEventArgs e)
#else
        protected override void OnPointerExited(PointerRoutedEventArgs e)
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
        /// Provides handling for the
        /// <see cref="E:System.Windows.UIElement.MouseLeftButtonDown" />
        /// event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="T:System.Windows.Input.MouseButtonEventArgs" /> that
        /// contains the event data.
        /// </param>
#if MIGRATION
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
#else
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
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

        /// <summary>
        /// Called when valid spin direction changed.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnValidSpinDirectionChanged(ValidSpinDirections oldValue, ValidSpinDirections newValue)
        {
            base.OnValidSpinDirectionChanged(oldValue, newValue);

            SetButtonUsage();
        }

        /// <summary>
        /// Disables or enables the buttons based on the valid spin direction.
        /// </summary>
        private void SetButtonUsage()
        {
            // buttonspinner adds buttons that spin, so disable accordingly.
            if (IncreaseButton != null)
            {
                IncreaseButton.IsEnabled = ((ValidSpinDirection & ValidSpinDirections.Increase) == ValidSpinDirections.Increase);
            }

            if (DecreaseButton != null)
            {
                DecreaseButton.IsEnabled = ((ValidSpinDirection & ValidSpinDirections.Decrease) == ValidSpinDirections.Decrease);
            }
        }
    }
}