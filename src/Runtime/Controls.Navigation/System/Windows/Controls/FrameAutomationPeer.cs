//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows.Controls;

namespace System.Windows.Automation.Peers
{
    /// <summary>
    /// Exposes Frame types to UI Automation.
    /// </summary>
    /// <QualityBand>Stable</QualityBand>
    public class FrameAutomationPeer : FrameworkElementAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the FrameAutomationPeer class.
        /// </summary>
        /// <param name="owner">The Frame.</param>
        public FrameAutomationPeer(Frame owner)
            : base(owner)
        {
        }

#region AutomationPeer overrides

        /// <summary>
        /// Gets the control type for the element that is associated with the UI
        /// Automation peer.
        /// </summary>
        /// <returns>The control type.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Pane;
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
        /// Called by GetName.
        /// </summary>
        /// <returns>
        /// Returns the first of these that is not null or empty:
        /// - Value returned by the base implementation
        /// - Name of the owning Frame
        /// - Frame class name
        /// </returns>
        protected override string GetNameCore()
        {
            string name = base.GetNameCore();
            
            if (string.IsNullOrEmpty(name))
            {
                name = (this.Owner as Frame).Name;
            }

            if (string.IsNullOrEmpty(name))
            {
                name = this.GetClassName();
            }

            return name;
        }

#endregion AutomationPeer overrides
    }
}
