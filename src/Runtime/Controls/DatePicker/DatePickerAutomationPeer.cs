// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Runtime.CompilerServices;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using Resource = OpenSilver.Controls.Resources;

namespace System.Windows.Automation.Peers
{
    /// <summary>
    /// Exposes <see cref="DatePicker" /> types to UI automation.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    public partial class DatePickerAutomationPeer :
        FrameworkElementAutomationPeer,
        IExpandCollapseProvider,
        IValueProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatePickerAutomationPeer" /> class.
        /// </summary>
        /// <param name="owner">
        /// The <see cref="DatePicker" /> to associate with the <see cref="DatePickerAutomationPeer" />.
        /// </param>
        public DatePickerAutomationPeer(DatePicker owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Gets Inherited code: Requires comment.
        /// </summary>
        private DatePicker OwningDatePicker
        {
            get { return this.Owner as DatePicker; }
        }

        /// <summary>
        /// Gets the control pattern for the <see cref="DatePicker" /> that is
        /// associated with this <see cref="DatePickerAutomationPeer" />.
        /// </summary>
        /// <param name="patternInterface">
        /// One of the enumeration values.
        /// </param>
        /// <returns>
        /// The object that implements the pattern interface, or null if the
        /// specified pattern interface is not implemented by this peer.
        /// </returns>
        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.ExpandCollapse || patternInterface == PatternInterface.Value)
            {
                return this;
            }
            return base.GetPattern(patternInterface);
        }

        /// <summary>
        /// Gets the control type for the element that is associated with the UI
        /// Automation peer.
        /// </summary>
        /// <returns>The control type.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.ComboBox;
        }

        /// <summary>
        /// Called by GetClassName that gets a human readable name that, in
        /// addition to AutomationControlType, differentiates the control
        /// represented by this AutomationPeer.
        /// </summary>
        /// <returns>The string that contains the name.</returns>
        protected override string GetClassNameCore()
        {
            return Owner.GetType().Name;
        }

        /// <summary>
        /// Overrides the GetLocalizedControlTypeCore method for DatePicker.
        /// </summary>
        /// <returns>Inherited code: Requires comment.</returns>
        protected override string GetLocalizedControlTypeCore()
        {
            return Resource.DatePickerAutomationPeer_LocalizedControlType;
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <returns>Inherited code: Requires comment 1.</returns>
        protected override string GetNameCore()
        {
            string nameCore = base.GetNameCore();

            if (string.IsNullOrEmpty(nameCore))
            {
                AutomationPeer labeledByCore = this.GetLabeledByCore();
                if (labeledByCore != null)
                {
                    nameCore = labeledByCore.GetName();
                }
                if (string.IsNullOrEmpty(nameCore))
                {
                    nameCore = this.OwningDatePicker.ToString();
                }
            }
            return nameCore;
        }

        /// <summary>
        /// Gets the state, expanded or collapsed, of the control.
        /// </summary>
        /// <value>The state, expanded or collapsed, of the control.</value>
        ExpandCollapseState IExpandCollapseProvider.ExpandCollapseState
        {
            get
            {
                if (this.OwningDatePicker.IsDropDownOpen)
                {
                    return ExpandCollapseState.Expanded;
                }
                else
                {
                    return ExpandCollapseState.Collapsed;
                }
            }
        }

        /// <summary>
        /// Hides all nodes, controls, or content that are descendants of the
        /// control.
        /// </summary>
        void IExpandCollapseProvider.Collapse()
        {
            this.OwningDatePicker.IsDropDownOpen = false;
        }

        /// <summary>
        /// Displays all child nodes, controls, or content of the control.
        /// </summary>
        void IExpandCollapseProvider.Expand()
        {
            this.OwningDatePicker.IsDropDownOpen = true;
        }

        /// <summary>
        /// Gets a value indicating whether the value of a control is
        /// read-only.
        /// </summary>
        /// <value>Returns false.</value>
        bool IValueProvider.IsReadOnly { get { return false; } }

        /// <summary>
        /// Gets the value of the control.
        /// </summary>
        /// <value>The value of the control as a string.</value>
        string IValueProvider.Value { get { return this.OwningDatePicker.ToString(); } }

        /// <summary>
        /// Sets the value of a control.
        /// </summary>
        /// <param name="value">
        /// The value to set. The provider is responsible for converting the
        /// value to the appropriate data type.
        /// </param>
        void IValueProvider.SetValue(string value)
        {
            this.OwningDatePicker.Text = value;
        }

        #region Internal Methods

        /// <summary>
        /// Raises the automation peer's ValuePropertyChanged event.
        /// Never inline, as we don't want to unnecessarily link the automation DLL.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal void RaiseValuePropertyChangedEvent(string oldValue, string newValue)
        {
            if (oldValue != newValue)
            {
                RaisePropertyChangedEvent(ValuePatternIdentifiers.ValueProperty, oldValue, newValue);
            }
        }

        #endregion
    }
}