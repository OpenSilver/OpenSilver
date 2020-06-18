#if WORKINPROGRESS
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
	public abstract partial class ItemsControlAutomationPeer : FrameworkElementAutomationPeer
	{
		protected ItemsControlAutomationPeer(ItemsControl owner): base(owner)
		{
		}

		protected virtual ItemAutomationPeer CreateItemAutomationPeer(object item)
		{
			return null;
		}

		protected override List<AutomationPeer> GetChildrenCore()
		{
			return null;
		}
	}
}
#endif