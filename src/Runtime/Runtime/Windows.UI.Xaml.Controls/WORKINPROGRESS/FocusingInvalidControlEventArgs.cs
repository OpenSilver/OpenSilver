using System;

#if WORKINPROGRESS
#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
	//
	// Summary:
	//     Provides data for the System.Windows.Controls.ValidationSummary.FocusingInvalidControl
	//     event.
	public partial class FocusingInvalidControlEventArgs : EventArgs
	{
		//
		// Summary:
		//     Initializes a new instance of the System.Windows.Controls.FocusingInvalidControlEventArgs
		//     class.
		//
		// Parameters:
		//   item:
		//     The error that is selected in the System.Windows.Controls.ValidationSummary list.
		//
		//   target:
		//     The control/property pair that will receive focus.
		public FocusingInvalidControlEventArgs(ValidationSummaryItem item, ValidationSummaryItemSource target)
		{
		}

		//
		// Summary:
		//     Gets or sets a value that indicates whether setting the focus was handled.
		//
		// Returns:
		//     true if setting the focus was handled; otherwise, false. The default is false.
		public bool Handled
		{
			get;
			set;
		}

		//
		// Summary:
		//     Gets the error that is selected in the System.Windows.Controls.ValidationSummary
		//     error list.
		//
		// Returns:
		//     The error that is selected in the System.Windows.Controls.ValidationSummary error
		//     list.
		public ValidationSummaryItem Item
		{
			get;
			private set;
		}

		//
		// Summary:
		//     Gets or sets the control/property pair that will receive focus.
		//
		// Returns:
		//     The control/property pair that will receive focus.
		public ValidationSummaryItemSource Target
		{
			get;
			set;
		}
	}
}
#endif