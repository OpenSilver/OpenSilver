using System;
using System.Collections.Generic;
using System.Text;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    [OpenSilver.NotImplemented]
    public class LayoutCycleException : Exception
    {
    }
}
