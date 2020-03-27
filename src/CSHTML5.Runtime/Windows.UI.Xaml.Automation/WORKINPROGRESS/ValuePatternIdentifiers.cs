#if WORKINPROGRESS
#if MIGRATION
namespace System.Windows.Automation
#else
namespace Windows.UI.Xaml.Automation
#endif
{
	//
	// Summary:
	//     Contains values used as automation property identifiers specifically for properties
	//     of the System.Windows.Automation.Provider.IValueProvider pattern.
	public static partial class ValuePatternIdentifiers
	{
		//
		// Summary:
		//     Identifies the System.Windows.Automation.Provider.IValueProvider.IsReadOnly property.
		public static readonly AutomationProperty IsReadOnlyProperty;
		//
		// Summary:
		//     Identifies the System.Windows.Automation.Provider.IValueProvider.Value automation
		//     property.
		//
		// Returns:
		//     The automation property identifier.
		public static readonly AutomationProperty ValueProperty;
	}
}
#endif