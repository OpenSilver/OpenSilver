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
    [OpenSilver.NotImplemented]
	public abstract partial class ItemAutomationPeer : FrameworkElementAutomationPeer
	{
        [OpenSilver.NotImplemented]
		protected ItemAutomationPeer(UIElement item): base((FrameworkElement)item)
		{
		}

        [OpenSilver.NotImplemented]
		protected ItemAutomationPeer(object item, ItemsControlAutomationPeer itemsControlAutomationPeer): base((FrameworkElement)itemsControlAutomationPeer.Owner)
		{
		}

        [OpenSilver.NotImplemented]
		protected object Item
		{
			get;
			private set;
		}

        [OpenSilver.NotImplemented]
		protected internal ItemsControlAutomationPeer ItemsControlAutomationPeer
		{
			get;
			private set;
		}

        [OpenSilver.NotImplemented]
		public override object GetPattern(PatternInterface patternInterface)
		{
			return null;
		}

        [OpenSilver.NotImplemented]
		protected override string GetClassNameCore()
		{
			return null;
		}

        [OpenSilver.NotImplemented]
		protected override string GetHelpTextCore()
		{
			return null;
		}
	}
}
