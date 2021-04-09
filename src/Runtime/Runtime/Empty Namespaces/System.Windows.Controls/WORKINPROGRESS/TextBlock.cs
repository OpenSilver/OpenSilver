#if WORKINPROGRESS

using System;
using System.Windows.Documents;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    public partial class TextBlock : Control
    {
        public FontSource FontSource { get; set; }

        public static DependencyProperty LineHeightProperty
        {
            get; set;
        }
    }
}
#endif