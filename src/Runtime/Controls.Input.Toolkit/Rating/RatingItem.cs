// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

#define SILVERLIGHT

using System.Globalization;
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Resource = OpenSilver.Controls.Input.Toolkit.Resources;

namespace System.Windows.Controls
{
    /// <summary>
    /// An item used in a rating control.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateMouseOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StatePressed, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateReadOnly, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateFocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateUnfocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = StateFilled, GroupName = GroupFill)]
    [TemplateVisualState(Name = StateEmpty, GroupName = GroupFill)]
    [TemplateVisualState(Name = StatePartial, GroupName = GroupFill)]
    public class RatingItem : ButtonBase, IUpdateVisualState
    {
        /// <summary>
        /// The state in which the item is filled.
        /// </summary>
        private const string StateFilled = "Filled";

        /// <summary>
        /// The state in which the item is empty.
        /// </summary>
        private const string StateEmpty = "Empty";

        /// <summary>
        /// The group that contains fill states.
        /// </summary>
        private const string GroupFill = "FillStates";

        /// <summary>
        /// The state in which the item is partially filled.
        /// </summary>
        private const string StatePartial = "Partial";

        /// <summary>
        /// The interaction helper used to get the common states working.
        /// </summary>
        private InteractionHelper _interactionHelper;

        #region public double DisplayValue
        /// <summary>
        /// A value indicating whether the actual value is being set.
        /// </summary>
        private bool _settingDisplayValue;

        /// <summary>
        /// Gets the actual value.
        /// </summary>
        public double DisplayValue
        {
            get { return (double)GetValue(DisplayValueProperty); }
            internal set 
            {
                _settingDisplayValue = true;
                try
                {
                    SetValue(DisplayValueProperty, value);
                }
                finally
                {
                    _settingDisplayValue = false;
                }
            }
        }

        /// <summary>
        /// Identifies the DisplayValue dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayValueProperty =
            DependencyProperty.Register(
                "DisplayValue",
                typeof(double),
                typeof(RatingItem),
                new PropertyMetadata(0.0, OnDisplayValueChanged));

        /// <summary>
        /// DisplayValueProperty property changed handler.
        /// </summary>
        /// <param name="d">RatingItem that changed its DisplayValue.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnDisplayValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RatingItem source = (RatingItem)d;
            source.OnDisplayValueChanged((double) e.OldValue, (double) e.NewValue);
        }

        /// <summary>
        /// DisplayValueProperty property changed handler.
        /// </summary> 
        /// <param name="oldValue">The old value.</param> 
        /// <param name="newValue">The new value.</param>
        private void OnDisplayValueChanged(double oldValue, double newValue)
        {
            if (!_settingDisplayValue)
            {
                _settingDisplayValue = true;

                this.DisplayValue = oldValue;

                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentUICulture,
                        Resource.InvalidAttemptToChangeReadOnlyProperty,
                        "DisplayValue"));
            }
            else
            {
                if (newValue <= 0.0)
                {
                    VisualStates.GoToState(this, true, StateEmpty);
                }
                else if (newValue >= 1.0)
                {
                    VisualStates.GoToState(this, true, StateFilled);
                }
                else
                {
                    VisualStates.GoToState(this, true, StatePartial);
                }
            }
        }
        #endregion public double DisplayValue

        #region public bool IsReadOnly
        /// <summary>
        /// A value indicating whether the read only value is being set.
        /// </summary>
        private bool _settingIsReadOnly;

        /// <summary>
        /// Gets a value indicating whether the control is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return (bool) GetValue(IsReadOnlyProperty); }
            internal set 
            {
                _settingIsReadOnly = true;
                try
                {
                    SetValue(IsReadOnlyProperty, value);
                }
                finally
                {
                    _settingIsReadOnly = false;
                }
            }
        }

        /// <summary>
        /// Identifies the IsReadOnly dependency property.
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(
                "IsReadOnly",
                typeof(bool),
                typeof(RatingItem),
                new PropertyMetadata(false, OnIsReadOnlyChanged));

        /// <summary>
        /// IsReadOnlyProperty property changed handler.
        /// </summary>
        /// <param name="d">RatingItem that changed its IsReadOnly.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnIsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RatingItem source = (RatingItem)d;
            bool oldValue = (bool)e.OldValue;
            bool newValue = (bool)e.NewValue;
            source.OnIsReadOnlyChanged(oldValue, newValue);
        }

        /// <summary>
        /// IsReadOnlyProperty property changed handler.
        /// </summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>        
        protected virtual void OnIsReadOnlyChanged(bool oldValue, bool newValue)
        {
            if (!_settingIsReadOnly)
            {
                _settingIsReadOnly = true;
                this.IsReadOnly = oldValue;
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentUICulture,
                        Resource.InvalidAttemptToChangeReadOnlyProperty,
                        "IsReadOnly"));
            }
            else
            {
                _interactionHelper.OnIsReadOnlyChanged(newValue);
            }
        }
        #endregion public bool IsReadOnly

        /// <summary>
        /// Gets or sets the parent rating of this rating item.
        /// </summary>
        internal Rating ParentRating { get; set; }

        #region public double Value
        /// <summary>
        /// Gets or sets the value property.
        /// </summary>
        internal double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Identifies the Value dependency property.
        /// </summary>
        internal static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value",
                typeof(double),
                typeof(RatingItem),
                new PropertyMetadata(0.0));

        /// <summary>
        /// Selects a value and raises the value selected event.
        /// </summary>
        internal void SelectValue()
        {
            if (!this.IsReadOnly)
            {
                this.Value = 1.0;
                OnClick();
            }
        }

        #endregion public double Value

#if !SILVERLIGHT
        /// <summary>
        /// Initializes the static members of the ColumnDataPoint class.
        /// </summary>
        static RatingItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RatingItem), new FrameworkPropertyMetadata(typeof(RatingItem)));
        }

#endif  
        /// <summary>
        /// Initializes a new instance of the RatingItem class.
        /// </summary>
        public RatingItem()
        {
#if SILVERLIGHT
            this.DefaultStyleKey = typeof(RatingItem);
#endif
            _interactionHelper = new InteractionHelper(this);
        }

        /// <summary>
        /// Provides handling for the RatingItem's MouseLeftButtonDown event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (_interactionHelper.AllowMouseLeftButtonDown(e))
            {
                _interactionHelper.OnMouseLeftButtonDownBase();
            }
            base.OnMouseLeftButtonDown(e);
        }

        /// <summary>
        /// Provides handling for the RatingItem's MouseLeftButtonUp event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (_interactionHelper.AllowMouseLeftButtonUp(e))
            {
                _interactionHelper.OnMouseLeftButtonUpBase();
            }
            base.OnMouseLeftButtonUp(e);
        }

        /// <summary>
        /// This method is invoked when the mouse enters the rating item.
        /// </summary>
        /// <param name="e">Information about the event.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (_interactionHelper.AllowMouseEnter(e))
            {
                _interactionHelper.UpdateVisualStateBase(true);
            }
            base.OnMouseEnter(e);
        }

        /// <summary>
        /// This method is invoked when the mouse leaves the rating item.
        /// </summary>
        /// <param name="e">Information about the event.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (_interactionHelper.AllowMouseLeave(e))
            {
                _interactionHelper.UpdateVisualStateBase(true);
            }
            base.OnMouseLeave(e);
        }

        /// <summary>
        /// Sets the value to 1.0 when clicked.
        /// </summary>
        protected override void OnClick()
        {
            base.OnClick();
        }

        /// <summary>
        /// Updates the visual state.
        /// </summary>
        /// <param name="useTransitions">A value indicating whether to use 
        /// transitions.</param>
        void IUpdateVisualState.UpdateVisualState(bool useTransitions)
        {
            _interactionHelper.UpdateVisualStateBase(useTransitions);
        }

        /// <summary>
        /// Returns a AccordionItemAutomationPeer for use by the Silverlight
        /// automation infrastructure.
        /// </summary>
        /// <returns>A AccordionItemAutomationPeer object for the AccordionItem.</returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RatingItemAutomationPeer(this);
        }
    }
}
