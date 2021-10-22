#if MIGRATION
namespace System.Windows.Automation
#else
namespace Windows.UI.Xaml.Automation
#endif
{
	//
	// Summary:
	//     Contains values used as automation property identifiers specifically for properties
	//     of the System.Windows.Automation.Provider.IMultipleViewProvider pattern.
    [OpenSilver.NotImplemented]
	public static class MultipleViewPatternIdentifiers
	{
		//
		// Summary:
		//     Identifies the System.Windows.Automation.Provider.IMultipleViewProvider.CurrentView
		//     automation property.
		//
		// Returns:
		//     The automation property identifier.
        [OpenSilver.NotImplemented]
		public static readonly AutomationProperty CurrentViewProperty;
		//
		// Summary:
		//     Identifies the automation property that gets the control-specific collection
		//     of views.
		//
		// Returns:
		//     The automation property identifier.
        [OpenSilver.NotImplemented]
		public static readonly AutomationProperty SupportedViewsProperty;
	}
}
