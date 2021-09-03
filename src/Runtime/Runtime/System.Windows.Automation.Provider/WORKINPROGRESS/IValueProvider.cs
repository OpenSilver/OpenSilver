using System;
using System.Collections.Generic;
using System.Text;

#if MIGRATION
namespace System.Windows.Automation.Provider
#else
namespace Windows.UI.Xaml.Automation.Provider
#endif
{
	public partial interface IValueProvider
	{
		//
		// Summary:
		//     Gets a value that indicates whether the value of a control is read-only.
		//
		// Returns:
		//     true if the value is read-only; false if it can be modified.
		bool IsReadOnly
		{
			get;
		}

		//
		// Summary:
		//     Gets the value of the control.
		//
		// Returns:
		//     The value of the control.
		string Value
		{
			get;
		}

		//
		// Summary:
		//     Sets the value of a control.
		//
		// Parameters:
		//   value:
		//     The value to set. The provider is responsible for converting the value to the
		//     appropriate data type.
		void SetValue(string value);
	}
}
