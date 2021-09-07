using System.Collections.Generic;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
	//
	// Summary:
	//     Represents the initial state of a Silverlight application when application extension
	//     services are started.
	[OpenSilver.NotImplemented]
	public partial class ApplicationServiceContext
	{
		//
		// Summary:
		//     Gets the initialization parameters specified by the host Web page when embedding
		//     the Silverlight plug-in.
		//
		// Returns:
		//     The initialization parameters as a dictionary of key/value pairs.
		[OpenSilver.NotImplemented]
		public Dictionary<string, string> ApplicationInitParams
		{
			get;
			private set;
		}
	}
}
