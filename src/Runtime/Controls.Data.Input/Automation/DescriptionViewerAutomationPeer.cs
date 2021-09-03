//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if OPENSILVER

#if MIGRATION
using System.Windows.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

#if MIGRATION
namespace System.Windows.Automation.Peers
#else
namespace Windows.UI.Xaml.Automation.Peers
#endif
{
    /// <summary>
    /// Exposes <see cref="DescriptionViewer" /> types to UI Automation.
    /// </summary>
    [OpenSilver.NotImplemented]
    public class DescriptionViewerAutomationPeer : FrameworkElementAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DescriptionViewerAutomationPeer" /> class.
        /// </summary>
        /// <param name="owner">
        /// The <see cref="DescriptionViewer" /> that is associated with this <see cref="DescriptionViewerAutomationPeer" />.
        /// </param>
        public DescriptionViewerAutomationPeer(DescriptionViewer owner) : base(owner) { }

        /// <summary>
        /// Called by GetClassName that gets a human readable name that, in addition to AutomationControlType, 
        /// differentiates the control represented by this AutomationPeer.
        /// </summary>
        /// <returns>The string that contains the name.</returns>
        protected override string GetClassNameCore()
        {
            return typeof(DescriptionViewer).Name;
        }

        /// <summary>
        /// Called by GetName that gets a human readable name that, in addition to AutomationControlType, 
        /// differentiates the control represented by this AutomationPeer.
        /// </summary>
        /// <returns>The string that contains the name.</returns>
        protected override string GetNameCore()
        {
            DescriptionViewer dv = Owner as DescriptionViewer;
            if (dv != null)
            {
                return dv.Description;
            }
            return base.GetNameCore();
        }

        /// <summary>
        /// Gets the control type for the element that is associated with the UI Automation peer.
        /// </summary>
        /// <returns>The control type.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Text;
        }
    }
}

#endif