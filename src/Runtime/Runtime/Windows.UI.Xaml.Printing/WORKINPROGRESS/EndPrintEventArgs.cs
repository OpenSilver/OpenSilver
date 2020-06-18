using System;

#if WORKINPROGRESS
#if MIGRATION
namespace System.Windows.Printing
#else
namespace Windows.UI.Xaml.Printing
#endif
{
	//
	// Summary:
	//     Provides data for the System.Windows.Printing.PrintDocument.EndPrint event.
	public sealed partial class EndPrintEventArgs : EventArgs
	{
		//
		// Summary:
		//     Initializes a new instance of the System.Windows.Printing.EndPrintEventArgs class.
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
		public Exception Error
		{
			get;
			private set;
		}
	}
}
#endif