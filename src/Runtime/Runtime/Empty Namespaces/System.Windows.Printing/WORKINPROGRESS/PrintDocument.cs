#if WORKINPROGRESS

using System;

#if MIGRATION
namespace System.Windows.Printing
#else
namespace Windows.UI.Xaml.Input.Printing
#endif
{
    public partial class PrintDocument : DependencyObject
    {
        public void PrintBitmap(string documentName)
        {

        }
    }
}
#endif