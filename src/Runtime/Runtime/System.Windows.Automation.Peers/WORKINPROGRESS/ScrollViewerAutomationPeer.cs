using System;
using System.Collections.Generic;
using System.Text;

#if MIGRATION
using System.Windows.Automation.Provider;
using System.Windows.Controls;
#else
using Windows.UI.Xaml.Automation.Provider;
using Windows.UI.Xaml.Controls;
#endif

#if MIGRATION
namespace System.Windows.Automation.Peers
#else
namespace Windows.UI.Xaml.Automation.Peers
#endif
{
    [OpenSilver.NotImplemented]
	public partial class ScrollViewerAutomationPeer : FrameworkElementAutomationPeer, IScrollProvider
	{
        [OpenSilver.NotImplemented]
		public ScrollViewerAutomationPeer(ScrollViewer owner): base(owner)
		{
		}

		bool IScrollProvider.HorizontallyScrollable => throw new NotImplementedException();
		double IScrollProvider.HorizontalScrollPercent => throw new NotImplementedException();
		double IScrollProvider.HorizontalViewSize => throw new NotImplementedException();
		bool IScrollProvider.VerticallyScrollable => throw new NotImplementedException();
		double IScrollProvider.VerticalScrollPercent => throw new NotImplementedException();
		double IScrollProvider.VerticalViewSize => throw new NotImplementedException();
		void IScrollProvider.Scroll(ScrollAmount horizontalAmount, ScrollAmount verticalAmount)
		{
			throw new NotImplementedException();
		}

		void IScrollProvider.SetScrollPercent(double horizontalPercent, double verticalPercent)
		{
			throw new NotImplementedException();
		}
	}
}
