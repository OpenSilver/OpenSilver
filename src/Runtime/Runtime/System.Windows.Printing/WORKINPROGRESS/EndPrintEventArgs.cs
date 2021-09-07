using System;

#if MIGRATION
namespace System.Windows.Printing
#else
namespace Windows.UI.Xaml.Printing
#endif
{
	//
	// Summary:
	//     Provides data for the System.Windows.Printing.PrintDocument.EndPrint event.
	[OpenSilver.NotImplemented]
	public sealed partial class EndPrintEventArgs : EventArgs
	{
		//
		// Summary:
		//     Initializes a new instance of the System.Windows.Printing.EndPrintEventArgs class.
		[OpenSilver.NotImplemented]
		public EndPrintEventArgs()
		{
		}

		//
		// Summary:
		//     Gets an exception that indicates what kind of error occurred during printing,
		//     if an error occurred.
		//
		// Returns:
		//     An exception that indicates what kind of error occurred during printing, if an
		//     error occurred. The default is null.
		[OpenSilver.NotImplemented]
		public Exception Error
		{
			get;
			private set;
		}
	}
}
