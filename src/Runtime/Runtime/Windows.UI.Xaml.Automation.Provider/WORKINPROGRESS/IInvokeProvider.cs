#if WORKINPROGRESS
#if MIGRATION
namespace System.Windows.Automation.Provider
#else
namespace Windows.UI.Xaml.Automation.Provider
#endif
{
	//
	// Summary:
	//     Exposes a method to support UI automation access to controls that initiate or
	//     perform a single, unambiguous action and do not maintain state when activated.
	public partial interface IInvokeProvider
	{
		//
		// Summary:
		//     Sends a request to activate a control and initiate its single, unambiguous action.
		void Invoke();
	}
}
#endif