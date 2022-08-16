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
    /// Exposes TimePickerPopup types to UI Automation.
    /// </summary>
    [OpenSilver.NotImplemented]
    public abstract class TimePickerPopupAutomationPeer : FrameworkElementAutomationPeer, IValueProvider
    {
        /// <summary>
        /// Gets the TimePickerPopup that owns this AutomationPeer.
        /// </summary>
        /// <returns>The TimePicker that owns this AutomationPeer.</returns>
        protected abstract TimePickerPopup TimePickerPopupOwner { get; }

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="TimePickerPopupAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">The TimePickerPopup that is associated to this 
        /// AutomationPeer.</param>
        protected TimePickerPopupAutomationPeer(TimePickerPopup owner)
            : base(owner)
        {
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

#region IValueProvider Members

        /// <summary>
        /// Gets a value indicating whether the value of a control is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if the value is read-only; false if it can be modified.
        /// </returns>
        /// <remarks>
        /// This API supports the .NET Framework infrastructure and is not 
        /// intended to be used directly from your code.
        /// </remarks>
        public bool IsReadOnly
        {
            get
            {
                return !TimePickerPopupOwner.IsEnabled;
            }
        }

        /// <summary>
        /// Sets the value of a control from a string.
        /// </summary>
        /// <param name="value">The value to set.</param>
        /// <remarks>
        /// This API supports the .NET Framework infrastructure and is not 
        /// intended to be used directly from your code.
        /// </remarks>
        public void SetValue(string value)
        {
            TimePickerPopupOwner.Value = TimePickerPopupOwner.ActualTimeGlobalizationInfo.ParseTime(value, TimePickerPopupOwner.ActualFormat, null);
        }

        /// <summary>
        /// Gets the value of the control.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The value of the control as a string.
        /// </returns>
        /// <remarks>
        /// This API supports the .NET Framework infrastructure and is not 
        /// intended to be used directly from your code.
        /// </remarks>
        public string Value
        {
            get
            {
                return TimePickerPopupOwner.Value == null ? String.Empty : TimePickerPopupOwner.ActualTimeGlobalizationInfo.FormatTime(TimePickerPopupOwner.Value, TimePickerPopupOwner.ActualFormat);
            }
        }

        /// <summary>
        /// Raises the Value automation event.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        internal void RaiseValueAutomationEvent(DateTime? oldValue, DateTime? newValue)
        {
            string oldStringRepresentation = TimePickerPopupOwner.ActualTimeGlobalizationInfo.FormatTime(oldValue, TimePickerPopupOwner.ActualFormat);
            string newStringRepresentation = TimePickerPopupOwner.ActualTimeGlobalizationInfo.FormatTime(newValue, TimePickerPopupOwner.ActualFormat);

            RaisePropertyChangedEvent(ValuePatternIdentifiers.ValueProperty, oldStringRepresentation, newStringRepresentation);
        }
#endregion
    }
}