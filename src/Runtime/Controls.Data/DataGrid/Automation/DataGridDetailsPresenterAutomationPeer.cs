// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

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
/// AutomationPeer for DataGridDetailsPresenter
/// </summary>
/// <QualityBand>Mature</QualityBand>
public class DataGridDetailsPresenterAutomationPeer : FrameworkElementAutomationPeer
    {
        #region Constructors

        /// <summary>
        /// AutomationPeer for DataGridDetailsPresenter
        /// </summary>
        /// <param name="owner">DataGridDetailsPresenter</param>
        public DataGridDetailsPresenterAutomationPeer(DataGridDetailsPresenter owner)
            : base(owner)
        {
        }

        #endregion

        #region AutomationPeer Overrides

        ///
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Custom;
        }

        ///
        protected override string GetClassNameCore()
        {
            return this.Owner.GetType().Name;
        }

        ///
        protected override bool IsControlElementCore()
        {
            return true;
        }

        ///
        protected override bool IsContentElementCore()
        {
            return false;
        }

        #endregion
    }
}
