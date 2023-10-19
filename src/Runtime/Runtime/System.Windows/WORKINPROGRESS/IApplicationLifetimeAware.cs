namespace System.Windows
{
	//
	// Summary:
	//     Defines methods that application extension services can optionally implement
	//     in order to respond to application lifetime events.
	public partial interface IApplicationLifetimeAware
	{
		//
		// Summary:
		//     Called by an application immediately after the System.Windows.Application.Exit
		//     event occurs.
		void Exited();
		//
		// Summary:
		//     Called by an application immediately before the System.Windows.Application.Exit
		//     event occurs.
		void Exiting();
		//
		// Summary:
		//     Called by an application immediately after the System.Windows.Application.Startup
		//     event occurs.
		void Started();
		//
		// Summary:
		//     Called by an application immediately before the System.Windows.Application.Startup
		//     event occurs.
		void Starting();
	}
}
