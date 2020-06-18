#if WORKINPROGRESS
using System;
using System.Collections.Generic;
using System.Text;

#if MIGRATION
using System.Windows.Automation.Provider;
#else
using Windows.UI.Xaml.Automation.Provider;
#endif

#if MIGRATION
namespace System.Windows.Automation.Peers
#else
namespace Windows.UI.Xaml.Automation.Peers
#endif
{
	public abstract partial class ItemAutomationPeer : FrameworkElementAutomationPeer
	{
		protected ItemAutomationPeer(UIElement item): base((FrameworkElement)item)
		{
		}

		protected ItemAutomationPeer(object item, ItemsControlAutomationPeer itemsControlAutomationPeer): base((FrameworkElement)itemsControlAutomationPeer.Owner)
		{
		}

		protected object Item
		{
			get;
			private set;
		}

		protected internal ItemsControlAutomationPeer ItemsControlAutomationPeer
		{
			get;
			private set;
		}

		public override object GetPattern(PatternInterface patternInterface)
		{
			return null;
		}

		protected override string GetClassNameCore()
		{
			return null;
		}

		protected override string GetHelpTextCore()
		{
			return null;
		}
	}
}
#endif