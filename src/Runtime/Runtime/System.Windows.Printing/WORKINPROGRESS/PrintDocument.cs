using System;
using System.Collections.Generic;
using System.Text;

#if MIGRATION
namespace System.Windows.Printing
#else
namespace Windows.UI.Xaml.Printing
#endif
{
	[OpenSilver.NotImplemented]
	public partial class PrintDocument : DependencyObject
	{
		// Summary:
		//     Occurs after the System.Windows.Printing.PrintDocument.Print(System.String) method
		//     is called and the print dialog box successfully returns, but before the System.Windows.Printing.PrintDocument.PrintPage
		//     event is raised.
		[OpenSilver.NotImplemented]
		public event EventHandler<BeginPrintEventArgs> BeginPrint;
		// Summary:
		//     Occurs when the printing operation is passed to the print spooler or when the
		//     print operation is cancelled by the application author.
		[OpenSilver.NotImplemented]
		public event EventHandler<EndPrintEventArgs> EndPrint;
		// Summary:
		//     Occurs when each page is printing.
		[OpenSilver.NotImplemented]
		public event EventHandler<PrintPageEventArgs> PrintPage;
		// Summary:
		//     Starts the printing process for the specified document by opening the print dialog
		//     box.
		//
		// Parameters:
		//   documentName:
		//     The name of the document to print.
		//
		// Exceptions:
		//   T:System.Security.SecurityException:
		//     The print operation is not user-initiated.
		[OpenSilver.NotImplemented]
		public void Print(string documentName)
		{
		}

		// Summary:
		//     Starts the vector printing process for the specified document by optionally opening
		//     the print dialog box or printing directly to the default printer for trusted
		//     applications.
		//
		// Parameters:
		//   documentName:
		//     The name of the document to print.
		//
		//   printerFallbackSettings:
		//     The settings to use to enable vector printing for printers with limited support.
		//
		//   useDefaultPrinter:
		//     Whether or not to automatically print to the default printer for the computer
		//     without showing a print dialog. This parameter can only be true in trusted applications,
		//     otherwise an exception will occur.
		//
		// Exceptions:
		//   T:System.Security.SecurityException:
		//     The print operation is not user-initiated.-or-useDefaultPrinter is set to true
		//     and the application is not a trusted application.
		[OpenSilver.NotImplemented]
		public void Print(string documentName, PrinterFallbackSettings printerFallbackSettings, bool useDefaultPrinter = false)
		{
		}
	}
}
