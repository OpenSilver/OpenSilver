#if WORKINPROGRESS

using System;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    public abstract partial class DataGridBoundColumn : System.Windows.Controls.DataGridColumn
    {
        public System.Windows.Style ElementStyle { get; set; }
    }
}
#endif