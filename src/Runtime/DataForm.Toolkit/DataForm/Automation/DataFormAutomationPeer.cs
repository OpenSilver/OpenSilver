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
    /// Represents an automation peer for the <see cref="DataForm"/>.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class DataFormAutomationPeer : FrameworkElementAutomationPeer
    {
        /// <summary>
        /// Constructs a new <see cref="DataFormAutomationPeer"/>.
        /// </summary>
        /// <param name="owner">The <see cref="DataForm"/>.</param>
        public DataFormAutomationPeer(DataForm owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Gets the owning <see cref="DataForm"/>.
        /// </summary>
        private DataForm OwningDataForm
        {
            get
            {
                return this.Owner as DataForm;
            }
        }

        /// <summary>
        /// Returns the automation control type.
        /// </summary>
        /// <returns>The group automation control type.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Group;
        }

        /// <summary>
        /// Returns the class name.
        /// </summary>
        /// <returns>The string "DataForm".</returns>
        protected override string GetClassNameCore()
        {
            return typeof(DataForm).Name;
        }

        /// <summary>
        /// Returns the name.
        /// </summary>
        /// <returns>The <see cref="DataForm"/> header content, if it is a string.</returns>
        protected override string GetNameCore()
        {
            string headerString = this.OwningDataForm.Header as string;

            if (!string.IsNullOrEmpty(headerString))
            {
                return headerString;
            }

            return base.GetNameCore();
        }
    }
}
