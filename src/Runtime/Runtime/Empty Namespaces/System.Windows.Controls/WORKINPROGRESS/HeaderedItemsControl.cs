#if WORKINPROGRESS

using System;
using System.Windows.Documents;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    public partial class HeaderedItemsControl : ItemsControl
    {
        public object Header { get; set; }
    }
}
#endif