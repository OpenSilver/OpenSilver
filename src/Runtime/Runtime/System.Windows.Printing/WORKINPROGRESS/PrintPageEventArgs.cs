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
