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
	public partial class RadioButtonAutomationPeer : ToggleButtonAutomationPeer, ISelectionItemProvider
	{
        [OpenSilver.NotImplemented]
		public RadioButtonAutomationPeer(RadioButton owner): base(owner)
		{
		}

		bool ISelectionItemProvider.IsSelected => throw new NotImplementedException();
		IRawElementProviderSimple ISelectionItemProvider.SelectionContainer => throw new NotImplementedException();
		void ISelectionItemProvider.AddToSelection()
		{
			throw new NotImplementedException();
		}

		void ISelectionItemProvider.RemoveFromSelection()
		{
			throw new NotImplementedException();
		}

		void ISelectionItemProvider.Select()
		{
			throw new NotImplementedException();
		}
	}
}
