#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
	//
	// Summary:
	//     Defines methods that application extension services must implement in order to
	//     enable an application to start and stop the service.
	public partial interface IApplicationService
	{
		//
		// Summary:
		//     Called by an application in order to initialize the application extension service.
		//
		// Parameters:
		//   context:
		//     Provides information about the application state.
		void StartService(ApplicationServiceContext context);
		//
		// Summary:
		//     Called by an application in order to stop the application extension service.
		void StopService();
	}
}
