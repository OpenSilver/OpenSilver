// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Controls;

namespace System.Windows.Automation.Peers
{
    /// <summary>
    /// Exposes DomainUpDown types to UI Automation.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class DomainUpDownAutomationPeer : UpDownBaseAutomationPeer<object>
    {
        /// <summary>
        /// Initializes a new instance of the DomainUpDownAutomationPeer class.
        /// </summary>
        /// <param name="owner">
        /// The DomainUpDown that is associated with this DomainUpDownAutomationPeer.
        /// </param>
        public DomainUpDownAutomationPeer(DomainUpDown owner)
            : base(owner)
        {
        }

        #region AutomationPeer Overrides
        /// <summary>
        /// Gets the name of the DomainUpDown that is associated with this
        /// DomainUpDownAutomationPeer.  This method is called by GetClassName.
        /// </summary>
        /// <returns>The name DomainUpDown.</returns>
        protected override string GetClassNameCore()
        {
            return "DomainUpDown";
        }
        #endregion
    }
}