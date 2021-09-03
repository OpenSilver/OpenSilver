using System;
using System.Collections.Generic;
using System.Text;

#if MIGRATION
namespace System.Windows.Automation.Provider
#else
namespace Windows.UI.Xaml.Automation.Provider
#endif
{
	//
	// Summary:
	//     Exposes methods and properties to support access by a UI automation client to
	//     controls that visually expand to display content and that collapse to hide content.
	public partial interface IExpandCollapseProvider
	{
		//
		// Summary:
		//     Gets the state (expanded or collapsed) of the control.
		//
		// Returns:
		//     The state (expanded or collapsed) of the control.
		ExpandCollapseState ExpandCollapseState
		{
			get;
		}

		//
		// Summary:
		//     Hides all nodes, controls, or content that are descendants of the control.
		void Collapse();
		//
		// Summary:
		//     Displays all child nodes, controls, or content of the control.
		void Expand();
	}
}
