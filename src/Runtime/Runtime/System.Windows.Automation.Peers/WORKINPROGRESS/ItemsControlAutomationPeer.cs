using System;
using System.Collections.Generic;
using System.Text;

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
    [OpenSilver.NotImplemented]
	public abstract partial class ItemsControlAutomationPeer : FrameworkElementAutomationPeer
	{
        [OpenSilver.NotImplemented]
		protected ItemsControlAutomationPeer(ItemsControl owner): base(owner)
		{
		}

        [OpenSilver.NotImplemented]
		protected virtual ItemAutomationPeer CreateItemAutomationPeer(object item)
		{
			return null;
		}

        [OpenSilver.NotImplemented]
		protected override List<AutomationPeer> GetChildrenCore()
		{
			return null;
		}
	}
}
