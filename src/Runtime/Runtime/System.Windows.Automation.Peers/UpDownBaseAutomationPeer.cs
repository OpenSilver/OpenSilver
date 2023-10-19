// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Automation.Provider;
using System.Windows.Controls;

namespace System.Windows.Automation.Peers
{
    /// <summary>
    /// Exposes UpDownBase types to UI Automation.
    /// </summary>
    /// <typeparam name="T">Type of the items.</typeparam>
    /// <QualityBand>Stable</QualityBand>
    public class UpDownBaseAutomationPeer<T> : FrameworkElementAutomationPeer, IValueProvider
    {
        /// <summary>
        /// Gets the UpDownBase that owns this UpDownBaseAutomationPeer.
        /// </summary>
        /// <value>The domain up down.</value>
        private UpDownBase<T> OwnerUpDown { get { return (UpDownBase<T>)Owner; } }

        /// <summary>
        /// Initializes a new instance of the UpDownBaseAutomationPeer class.
        /// </summary>
        /// <param name="owner">
        /// The UpDownBase that is associated with this UpDownBaseAutomationPeer.
        /// </param>
        public UpDownBaseAutomationPeer(UpDownBase<T> owner)
            : base(owner)
        {
        }

#region AutomationPeer Overrides
        /// <summary>
        /// Gets the control type for the UpDownBase that is associated
        /// with this UpDownBaseAutomationPeer.  This method is called by
        /// GetAutomationControlType.
        /// </summary>
        /// <returns>Group AutomationControlType.</returns>
        /// <remarks>Return AutomationControlType.Spinner per MSDN article.</remarks>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Spinner;
        }

        /// <summary>
        /// Gets the name of the UpDownBase that is associated with this
        /// UpDownBaseAutomationPeer.  This method is called by GetClassName.
        /// </summary>
        /// <returns>The name UpDownBase.</returns>
        protected override string GetClassNameCore()
        {
            return "UpDownBase";
        }

        /// <summary>
        /// Gets the control pattern for the UpDownBase that is associated
        /// with this UpDownBaseAutomationPeer.
        /// </summary>
        /// <param name="patternInterface">The desired PatternInterface.</param>
        /// <returns>The desired AutomationPeer or null.</returns>
        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Value)
            {
                return this;
            }

            return base.GetPattern(patternInterface);
        }
#endregion

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
                if (OwnerUpDown.IsEnabled)
                {
                    return !OwnerUpDown.IsEditable;
                }
                return true;
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
            OwnerUpDown.ApplyValue(value);
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
                return OwnerUpDown.FormatValue();
            }
        }
#endregion
    }
}
