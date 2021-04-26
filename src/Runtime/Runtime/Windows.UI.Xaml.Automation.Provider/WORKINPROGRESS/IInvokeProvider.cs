#if WORKINPROGRESS

#if MIGRATION
namespace System.Windows.Automation.Provider
#else
namespace Windows.UI.Xaml.Automation.Provider
#endif
{
	/// <summary>
	/// Exposes a method to support UI automation access to controls that initiate or
	/// perform a single, unambiguous action and do not maintain state when activated.
	/// </summary>
	public partial interface IInvokeProvider
	{
		/// <summary>
		/// Sends a request to activate a control and initiate its single, unambiguous action.
		/// </summary>
		void Invoke();
	}
}

#endif