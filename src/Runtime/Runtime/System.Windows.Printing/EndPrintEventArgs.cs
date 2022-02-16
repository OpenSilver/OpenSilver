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
    public sealed partial class EndPrintEventArgs : EventArgs
    {
        //
        // Summary:
        //     Initializes a new instance of the System.Windows.Printing.EndPrintEventArgs class.
        public EndPrintEventArgs()
        {
        }
    }
}
