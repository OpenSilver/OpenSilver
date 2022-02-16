using System;
using System.Collections.Generic;
using System.Text;

#if MIGRATION
namespace System.Windows.Printing
#else
namespace Windows.UI.Xaml.Printing
#endif
{
    public partial class PrintDocument : DependencyObject
    {
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

        //
        // Summary:
        //     Starts the bitmap printing process for the specified document by opening the
        //     print dialog box.
        //
        // Parameters:
        //   documentName:
        //     The name of the document to print.
        [OpenSilver.NotImplemented]
        public void PrintBitmap(string documentName)
        {
        }
    }
}
