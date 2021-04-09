#if WORKINPROGRESS

using System;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    public abstract partial class DataGridBoundColumn : DataGridColumn
    {
        public Style ElementStyle { get; set; }
    }
}
#endif