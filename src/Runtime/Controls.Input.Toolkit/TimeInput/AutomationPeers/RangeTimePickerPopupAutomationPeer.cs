// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Controls;

namespace System.Windows.Automation.Peers
{
    /// <summary>
    /// Exposes RangeTimePicker types to UI Automation.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class RangeTimePickerPopupAutomationPeer : TimePickerPopupAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RangeTimePickerPopupAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">The owner of this AutomationPeer.</param>
        public RangeTimePickerPopupAutomationPeer(RangeTimePickerPopup owner) : base(owner)
        {
        }

        /// <summary>
        /// Gets the RangeTimePickerPopup that owns this AutomationPeer.
        /// </summary>
        /// <returns>The RangeTimePickerPopup that owns this AutomationPeer.</returns>
        protected override TimePickerPopup TimePickerPopupOwner
        {
            get
            {
                return (RangeTimePickerPopup)Owner;
            }
        }

        /// <summary>
        /// Returns the name of the <see cref="UIElement"/> that is 
        /// associated with this <see cref="FrameworkElementAutomationPeer"/>. 
        /// This method is called by <see cref="AutomationPeer.GetClassName"/>.
        /// </summary>
        /// <returns>The string RangeTimePickerPopupAutomationPeer.</returns>
        protected override string GetClassNameCore()
        {
            return "RangeTimePickerPopupAutomationPeer";
        }

        /// <summary>
        /// Gets the type of the automation control.
        /// </summary>
        /// <returns>The Calendar control type.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Calendar;
        }
    }
}
