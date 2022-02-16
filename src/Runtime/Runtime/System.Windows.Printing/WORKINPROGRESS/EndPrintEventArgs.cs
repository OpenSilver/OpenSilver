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
