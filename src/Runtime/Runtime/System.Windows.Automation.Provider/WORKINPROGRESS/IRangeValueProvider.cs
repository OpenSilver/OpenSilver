#if MIGRATION
namespace System.Windows.Automation.Provider
#else
namespace Windows.UI.Xaml.Automation.Provider
#endif
{
	public partial interface IRangeValueProvider
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
		//     Gets the value that is added to or subtracted from the System.Windows.Automation.Provider.IRangeValueProvider.Value
		//     property when a large change is made, such as with the PAGE DOWN key.
		//
		// Returns:
		//     The large-change value that is supported by the control, or null if the control
		//     does not support System.Windows.Automation.Provider.IRangeValueProvider.LargeChange.
		double LargeChange
		{
			get;
		}

		//
		// Summary:
		//     Gets the maximum range value that is supported by the control.
		//
		// Returns:
		//     The maximum value that is supported by the control, or null if the control does
		//     not support System.Windows.Automation.Provider.IRangeValueProvider.Maximum.
		double Maximum
		{
			get;
		}

		//
		// Summary:
		//     Gets the minimum range value that is supported by the control.
		//
		// Returns:
		//     The minimum value that is supported by the control, or null if the control does
		//     not support System.Windows.Automation.Provider.IRangeValueProvider.Minimum.
		double Minimum
		{
			get;
		}

		//
		// Summary:
		//     Gets the value that is added to or subtracted from the System.Windows.Automation.Provider.IRangeValueProvider.Value
		//     property when a small change is made, such as with an arrow key.
		//
		// Returns:
		//     The small-change value supported by the control, or null if the control does
		//     not support System.Windows.Automation.Provider.IRangeValueProvider.SmallChange.
		double SmallChange
		{
			get;
		}

		//
		// Summary:
		//     Gets the value of the control.
		//
		// Returns:
		//     The value of the control, or null if the control does not support System.Windows.Automation.Provider.IRangeValueProvider.Value.
		double Value
		{
			get;
		}

		//
		// Summary:
		//     Sets the value of the control.
		//
		// Parameters:
		//   value:
		//     The value to set.
		void SetValue(double value);
	}
}
