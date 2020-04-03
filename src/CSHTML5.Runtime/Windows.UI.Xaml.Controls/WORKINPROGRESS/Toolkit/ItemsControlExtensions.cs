#if WORKINPROGRESS

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    public static partial class ItemsControlExtensions
    {
        public static ScrollViewer GetScrollHost(this ItemsControl control)
        {
            return null;
        }
    }
}

#endif