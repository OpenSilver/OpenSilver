#if MIGRATION
namespace System.Windows.Automation.Provider
#else
namespace Windows.UI.Xaml.Automation.Provider
#endif
{
	//
	// Summary:
	//     Exposes methods and properties to support UI automation client access to controls
	//     that provide, and are able to switch between, multiple representations of the
	//     same set of information or child controls.
	public partial interface IMultipleViewProvider
	{
		//
		// Summary:
		//     Gets the current control-specific view.
		//
		// Returns:
		//     The value (viewId) for the current view of the UI automation element.
		int CurrentView
		{
			get;
		}

		//
		// Summary:
		//     Retrieves a collection of control-specific view identifiers.
		//
		// Returns:
		//     A collection of values that identifies the views available for a UI automation
		//     element.
		int[] GetSupportedViews();
		//
		// Summary:
		//     Retrieves the name of a control-specific view.
		//
		// Parameters:
		//   viewId:
		//     The view identifier.
		//
		// Returns:
		//     A localized name for the view.
		string GetViewName(int viewId);
		//
		// Summary:
		//     Sets the current control-specific view.
		//
		// Parameters:
		//   viewId:
		//     A view identifier.
		void SetCurrentView(int viewId);
	}
}
