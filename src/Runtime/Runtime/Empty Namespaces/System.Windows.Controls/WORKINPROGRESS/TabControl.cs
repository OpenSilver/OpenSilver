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
    public partial class TabControl : System.Windows.Controls.ItemsControl
    {
        public System.Windows.Controls.Dock TabStripPlacement { get; set; }
    }
}
#endif