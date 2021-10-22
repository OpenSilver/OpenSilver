using System;
using System.Collections.Generic;
using System.Text;

#if MIGRATION
using System.Windows.Automation.Provider;
using System.Windows.Controls.Primitives;
#else
using Windows.UI.Xaml.Automation.Provider;
using Windows.UI.Xaml.Controls.Primitives;
#endif

#if MIGRATION
namespace System.Windows.Automation.Peers
#else
namespace Windows.UI.Xaml.Automation.Peers
#endif
{
    [OpenSilver.NotImplemented]
	public partial class ToggleButtonAutomationPeer : ButtonBaseAutomationPeer, IToggleProvider
	{
        [OpenSilver.NotImplemented]
		public ToggleButtonAutomationPeer(ToggleButton owner) : base(owner)
		{
		}

		ToggleState IToggleProvider.ToggleState => throw new NotImplementedException();
		void IToggleProvider.Toggle()
		{
			throw new NotImplementedException();
		}
	}
}
