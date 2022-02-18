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
    public sealed partial class PrintPageEventArgs : EventArgs
    {
        //
        // Summary:
        //     Initializes a new instance of the System.Windows.Printing.PrintPageEventArgs
        //     class.
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
        public bool HasMorePages { get; set; }

        //
        // Summary:
        //     Gets or sets the visual element to print.
        //
        // Returns:
        //     The visual element to print. The default is null.
        public UIElement PageVisual { get; set; }
    }
}
