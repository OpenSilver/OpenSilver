using System;
using System.Collections.Generic;
using System.Text;

#if MIGRATION
namespace System.Windows.Automation.Provider
#else
namespace Windows.UI.Xaml.Automation.Provider
#endif
{
	public partial interface ITableProvider : IGridProvider
	{
		//
		// Summary:
		//     Gets the primary direction of traversal for the table.
		//
		// Returns:
		//     The primary direction of traversal, as a value of the enumeration.
		RowOrColumnMajor RowOrColumnMajor
		{
			get;
		}

		//
		// Summary:
		//     Returns a collection of UI Automation providers that represents all the column
		//     headers in a table.
		//
		// Returns:
		//     An array of UI automation providers.
		IRawElementProviderSimple[] GetColumnHeaders();
		//
		// Summary:
		//     Returns a collection of UI Automation providers that represents all row headers
		//     in the table.
		//
		// Returns:
		//     An array of UI automation providers.
		IRawElementProviderSimple[] GetRowHeaders();
	}
}
