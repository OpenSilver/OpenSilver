// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace System.Windows.Controls
{
    /// <summary>
    /// Base class for all controls that provide value manipulation with a 
    /// Spinner and a text box.
    /// </summary>
    /// <remarks>
    /// This non generic base class is used to specify default template,
    /// and simulate covariance among sub classes of UpDownBase&lt;T&gt;.
    /// </remarks>
    /// <QualityBand>Stable</QualityBand>
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateMouseOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StatePressed, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]

    [TemplateVisualState(Name = VisualStates.StateFocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateUnfocused, GroupName = VisualStates.GroupFocus)]

    [TemplateVisualState(Name = VisualStates.StateValid, GroupName = VisualStates.GroupValidation)]
    [TemplateVisualState(Name = VisualStates.StateInvalidFocused, GroupName = VisualStates.GroupValidation)]
    [TemplateVisualState(Name = VisualStates.StateInvalidUnfocused, GroupName = VisualStates.GroupValidation)]

    [TemplatePart(Name = UpDownBase.ElementTextName, Type = typeof(TextBox))]
    [TemplatePart(Name = UpDownBase.ElementSpinnerName, Type = typeof(Spinner))]
    [StyleTypedProperty(Property = UpDownBase.SpinnerStyleName, StyleTargetType = typeof(Spinner))]
    public abstract class UpDownBase : Control
    {
        #region Template Parts Name Constants
        /// <summary>
        /// Name constant for Text template part.
        /// </summary>
        internal const string ElementTextName = "Text";

        /// <summary>
        /// Name constant for Spinner template part.
        /// </summary>
        internal const string ElementSpinnerName = "Spinner";

        /// <summary>
        /// Name constant for SpinnerStyle property.
        /// </summary>
        internal const string SpinnerStyleName = "SpinnerStyle";
        #endregion 

        #region public Style SpinnerStyle
        /// <summary>
        /// Gets or sets the Style that is applied to the spinner.
        /// </summary>
        public Style SpinnerStyle
        {
            get { return (Style)GetValue(SpinnerStyleProperty); }
            set { SetValue(SpinnerStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the SpinnerStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty SpinnerStyleProperty =
            DependencyProperty.Register(
                "SpinnerStyle",
                typeof(Style),
                typeof(UpDownBase),
                new PropertyMetadata(null, OnSpinnerStylePropertyChanged));

        /// <summary>
        /// Property changed callback for SpinnerStyleProperty.
        /// </summary>
        /// <param name="d">UpDownBase whose SpinnerStyleProperty changed.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnSpinnerStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UpDownBase source = d as UpDownBase;
            Style oldValue = e.OldValue as Style;
            Style newValue = e.NewValue as Style;
            source.OnSpinnerStyleChanged(oldValue, newValue);
        }
        #endregion public Style SpinnerStyle

        /// <summary>
        /// Initializes a new instance of the UpDownBase class.
        /// </summary>
        internal UpDownBase()
        {
            DefaultStyleKey = typeof(UpDownBase);
            Interaction = new InteractionHelper(this);
        }

        /// <summary>
        /// Called when SpinnerStyle property value has changed.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnSpinnerStyleChanged(Style oldValue, Style newValue)
        {
        }

        /// <summary>
        /// GetValue method for returning UpDownBase&lt;T&gt;.Value as object.
        /// </summary>
        /// <returns>Value as object type.</returns>
        public abstract object GetValue();

        /// <summary>
        /// SetValue method for setting UpDownBase&lt;T&gt;.Value through object type parameter.
        /// </summary>
        /// <param name="value">New value in object type.</param>
        public abstract void SetValue(object value);

        #region visual state management
        /// <summary>
        /// Gets or sets the helper that provides all of the standard
        /// interaction functionality. Making it internal for subclass access.
        /// </summary>
        internal InteractionHelper Interaction { get; set; }

        /// <summary>
        /// Update current visual state.
        /// </summary>
        /// <param name="useTransitions">
        /// True to use transitions when updating the visual state, false to
        /// snap directly to the new visual state.
        /// </param>
        internal virtual void UpdateVisualState(bool useTransitions)
        {
            // Handle the Common and Focused states
            Interaction.UpdateVisualStateBase(useTransitions);
        }
        #endregion
    }
}
