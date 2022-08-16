// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

#if MIGRATION
using System.Windows.Automation.Provider;
using System.Windows.Controls;
#else
using Windows.UI.Xaml.Automation.Provider;
using Windows.UI.Xaml.Controls;
#endif

#if MIGRATION
namespace System.Windows.Automation.Peers
#else
namespace Windows.UI.Xaml.Automation.Peers
#endif
{
    /// <summary>
    /// Exposes TimePicker types to UI Automation.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [OpenSilver.NotImplemented]
    public sealed class TimePickerAutomationPeer : PickerAutomationPeer, IValueProvider
    {
        /// <summary>
        /// Gets the TimePicker that owns this AutomationPeer.
        /// </summary>
        private TimePicker OwnerTimePicker
        {
            get
            {
                return (TimePicker)Owner;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimePickerAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">The TimePicker that is associated to this
        /// AutomationPeer.</param>
        public TimePickerAutomationPeer(TimePicker owner) : base(owner)
        {
        }

        /// <summary>
        /// Gets the name of the TimePicker that is associated with this
        /// PickerAutomationPeer. This method is called by
        /// GetClassName.
        /// </summary>
        /// <returns>The name Picker.</returns>
        protected override string GetClassNameCore()
        {
            return "TimePicker";
        }

        /// <summary>
        /// Returns the control pattern for the <see cref="T:System.Windows.UIElement"/>
        /// that is associated with this <see cref="T:System.Windows.Automation.Peers.FrameworkElementAutomationPeer"/>.
        /// </summary>
        /// <param name="patternInterface">One of the enumeration values.</param>
        /// <returns>Returns an AutomationPeer that can handle the the pattern,
        /// or null.</returns>
        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Value)
            {
                return this;
            }
            return base.GetPattern(patternInterface);
        }

#region ValueProvider
        /// <summary>
        /// Sets the control's text value.
        /// </summary>
        /// <param name="value">The string value.</param>
        void IValueProvider.SetValue(string value)
        {
            OwnerTimePicker.Value = OwnerTimePicker.ActualTimeGlobalizationInfo.ParseTime(value, OwnerTimePicker.ActualFormat, OwnerTimePicker.ActualTimeParsers);
        }

        /// <summary>
        /// Gets a value indicating whether the value is read only.
        /// </summary>
        bool IValueProvider.IsReadOnly
        {
            get
            {
                return !OwnerTimePicker.IsEnabled;
            }
        }

        /// <summary>
        /// Gets a string representation of the current text value.
        /// </summary>
        string IValueProvider.Value
        {
            get
            {
                return OwnerTimePicker.Value == null ? String.Empty : OwnerTimePicker.ActualTimeGlobalizationInfo.FormatTime(OwnerTimePicker.Value, OwnerTimePicker.ActualFormat);
            }
        }

        /// <summary>
        /// Raises the Value automation event.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        internal void RaiseValueAutomationEvent(DateTime? oldValue, DateTime? newValue)
        {
            string oldStringRepresentation = OwnerTimePicker.ActualTimeGlobalizationInfo.FormatTime(oldValue, OwnerTimePicker.ActualFormat);
            string newStringRepresentation = OwnerTimePicker.ActualTimeGlobalizationInfo.FormatTime(newValue, OwnerTimePicker.ActualFormat);

            RaisePropertyChangedEvent(ValuePatternIdentifiers.ValueProperty, oldStringRepresentation, newStringRepresentation);
        }
#endregion
    }
}
