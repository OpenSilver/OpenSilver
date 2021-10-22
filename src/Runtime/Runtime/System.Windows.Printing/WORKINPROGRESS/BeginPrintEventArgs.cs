using System;

#if MIGRATION
namespace System.Windows.Printing
#else
namespace Windows.UI.Xaml.Printing
#endif
{
	//
	// Summary:
	//     Provides data for the System.Windows.Printing.PrintDocument.BeginPrint event.
	[OpenSilver.NotImplemented]
	public sealed partial class BeginPrintEventArgs : EventArgs
	{
		//
		// Summary:
		//     Initializes a new instance of the System.Windows.Printing.BeginPrintEventArgs
		//     class.
		[OpenSilver.NotImplemented]
		public BeginPrintEventArgs()
		{
		}
	}
}
