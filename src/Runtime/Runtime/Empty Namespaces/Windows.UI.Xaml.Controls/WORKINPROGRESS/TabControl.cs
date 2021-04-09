#if WORKINPROGRESS

using System;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    public partial class TabControl : ItemsControl
    {
        public Dock TabStripPlacement { get; set; }
    }
}
#endif