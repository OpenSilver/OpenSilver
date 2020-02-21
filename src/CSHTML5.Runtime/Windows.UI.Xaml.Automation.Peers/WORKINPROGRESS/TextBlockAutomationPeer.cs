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
	public partial class TextBlockAutomationPeer : FrameworkElementAutomationPeer
	{
		public TextBlockAutomationPeer(TextBlock owner): base(owner)
		{
		}
	}
}
#endif