#if WORKINPROGRESS
#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
	//
	// Summary:
	//     Represents the source of a System.Windows.Controls.ValidationSummaryItem.
	public partial class ValidationSummaryItemSource
	{
		//
		// Summary:
		//     Initializes a new instance of the System.Windows.Controls.ValidationSummaryItemSource
		//     class with the specified property associated with this error.
		//
		// Parameters:
		//   propertyName:
		//     The name of the property associated with this error.
		public ValidationSummaryItemSource(string propertyName)
		{
		}

		//
		// Summary:
		//     Initializes a new instance of the System.Windows.Controls.ValidationSummaryItemSource
		//     class with the specified property and control associated with this error.
		//
		// Parameters:
		//   propertyName:
		//     The name of the property associated with this error.
		//
		//   control:
		//     The control associated with this error.
		public ValidationSummaryItemSource(string propertyName, Control control)
		{
		}

		//
		// Summary:
		//     Gets the control that is the source of this error.
		//
		// Returns:
		//     The control that is the source of this error.
		public Control Control
		{
			get;
			private set;
		}

		//
		// Summary:
		//     Gets the name of the property that is the source of this error.
		//
		// Returns:
		//     The name of the property that is the source of this error.
		public string PropertyName
		{
			get;
			private set;
		}

		//
		// Summary:
		//     Determines whether the specified System.Windows.Controls.ValidationSummaryItemSource
		//     is equal to the current System.Windows.Controls.ValidationSummaryItemSource.
		//
		// Parameters:
		//   obj:
		//     The System.Windows.Controls.ValidationSummaryItemSource to compare to the current
		//     System.Windows.Controls.ValidationSummaryItemSource.
		//
		// Returns:
		//     true if the specified System.Windows.Controls.ValidationSummaryItemSource is
		//     equal to the current System.Windows.Controls.ValidationSummaryItemSource; otherwise,
		//     false.
		public override bool Equals(object obj)
		{
			return false;
		}

		//
		// Summary:
		//     Returns a hash code based on the System.Windows.Controls.ValidationSummaryItemSource.PropertyName
		//     and the System.Windows.Controls.ValidationSummaryItemSource.ControlSystem.Windows.FrameworkElement.Name.
		//
		// Returns:
		//     The hash code for this instance.
		public override int GetHashCode()
		{
			return 1000000000;
		}
	}
}
#endif