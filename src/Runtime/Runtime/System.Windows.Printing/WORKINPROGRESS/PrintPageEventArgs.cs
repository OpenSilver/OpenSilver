using System;

#if !MIGRATION
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows.Printing
#else
namespace Windows.UI.Xaml.Printing
#endif
{
	//
	// Summary:
	//     Provides data for the System.Windows.Printing.PrintDocument.PrintPage event.
	[OpenSilver.NotImplemented]
	public sealed partial class PrintPageEventArgs : EventArgs
	{
		//
		// Summary:
		//     Initializes a new instance of the System.Windows.Printing.PrintPageEventArgs
		//     class.
		[OpenSilver.NotImplemented]
		public PrintPageEventArgs()
		{
		}

		//
		// Summary:
		//     Gets or sets whether there are more pages to print.
		//
		// Returns:
		//     true if there are additional pages to print; otherwise, false. The default is
		//     false.
		[OpenSilver.NotImplemented]
		public bool HasMorePages
		{
			get;
			set;
		}

		//
		// Summary:
		//     Gets the margins of the page that is currently printing.
		//
		// Returns:
		//     The margins of the page that is currently printing.
		[OpenSilver.NotImplemented]
		public Thickness PageMargins
		{
			get;
			private set;
		}

		//
		// Summary:
		//     Gets or sets the visual element to print.
		//
		// Returns:
		//     The visual element to print. The default is null.
		[OpenSilver.NotImplemented]
		public UIElement PageVisual
		{
			get;
			set;
		}

		//
		// Summary:
		//     Gets the size of the printable area.
		//
		// Returns:
		//     The size of the printable area in screen pixels. The default is 0.0F.
		[OpenSilver.NotImplemented]
		public Size PrintableArea
		{
			get;
			private set;
		}
	}
}
