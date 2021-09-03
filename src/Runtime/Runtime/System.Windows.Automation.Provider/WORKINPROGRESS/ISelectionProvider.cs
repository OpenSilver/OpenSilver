using System;
using System.Collections.Generic;
using System.Text;

#if MIGRATION
namespace System.Windows.Automation.Provider
#else
namespace Windows.UI.Xaml.Automation.Provider
#endif
{
	public partial interface ISelectionProvider
	{
		bool CanSelectMultiple
		{
			get;
		}

		bool IsSelectionRequired
		{
			get;
		}

		IRawElementProviderSimple[] GetSelection();
	}
}
