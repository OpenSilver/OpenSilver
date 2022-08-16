// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

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
    /// Exposes TimeUpDown types to UI Automation.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [OpenSilver.NotImplemented]
    public class TimeUpDownAutomationPeer : UpDownBaseAutomationPeer<DateTime?>
    {
        /// <summary>
        /// Initializes a new instance of the TimeUpDownAutomationPeer class.
        /// </summary>
        /// <param name="owner">
        /// The TimeUpDown that is associated with this TimeUpDownAutomationPeer.
        /// </param>
        public TimeUpDownAutomationPeer(TimeUpDown owner)
            : base(owner)
        {
        }

#region AutomationPeer Overrides
        /// <summary>
        /// Gets the name of the TimeUpDown that is associated with this
        /// TimeUpDownAutomationPeer.  This method is called by GetClassName.
        /// </summary>
        /// <returns>The name TimeUpDown.</returns>
        protected override string GetClassNameCore()
        {
            return "TimeUpDown";
        }
#endregion
    }
}
