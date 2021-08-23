// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
#if MIGRATION
using System.Windows.Controls.Primitives;
#else
using Windows.UI.Xaml.Controls.Primitives;
#endif

#if MIGRATION
namespace System.Windows.Automation.Peers
#else
namespace Windows.UI.Xaml.Automation.Peers
#endif
{
    /// <summary>
    /// AutomationPeer for DataGridRowsPresenter
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    public class DataGridRowsPresenterAutomationPeer : FrameworkElementAutomationPeer
    {
        #region Constructors

        /// <summary>
        /// AutomationPeer for DataGridColumnHeadersPresenter
        /// </summary>
        /// <param name="owner">DataGridColumnHeadersPresenter</param>
        public DataGridRowsPresenterAutomationPeer(DataGridRowsPresenter owner)
            : base(owner)
        {
        }

        #endregion

        #region Properties

        private DataGridAutomationPeer GridPeer
        {
            get
            {
                if (this.OwningRowsPresenter.OwningGrid != null)
                {
                    return CreatePeerForElement(this.OwningRowsPresenter.OwningGrid) as DataGridAutomationPeer;
                }
                return null;
            }
        }

        private DataGridRowsPresenter OwningRowsPresenter
        {
            get
            {
                return (DataGridRowsPresenter)Owner;
            }
        }

        #endregion

        #region AutomationPeer Overrides

        /// <summary>
        /// Gets the control type for the element that is associated with the UI Automation peer.
        /// </summary>
        /// <returns>The control type.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Custom;
        }

        ///
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Required method signature for automation peers")]
        protected override List<AutomationPeer> GetChildrenCore()
        {
            if (this.OwningRowsPresenter.OwningGrid == null)
            {
                return new List<AutomationPeer>();
            }

            return this.GridPeer.GetChildPeers();
        }

        /// <summary>
        /// Called by GetClassName that gets a human readable name that, in addition to AutomationControlType, 
        /// differentiates the control represented by this AutomationPeer.
        /// </summary>
        /// <returns>The string that contains the name.</returns>
        protected override string GetClassNameCore()
        {
            return Owner.GetType().Name;
        }

        /// <summary>
        /// Gets a value that specifies whether the element is a content element.
        /// </summary>
        /// <returns>true if the element is a content element; otherwise false</returns>
        protected override bool IsContentElementCore()
        {
            return false;
        }

        #endregion
    }
}
