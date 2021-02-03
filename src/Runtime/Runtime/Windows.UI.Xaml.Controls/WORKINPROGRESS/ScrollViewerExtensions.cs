#if WORKINPROGRESS

using System;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    public static class ScrollViewerExtensions
    {
        public static void ScrollIntoView(this ScrollViewer viewer, FrameworkElement element)
        {

        }
    }
}
#endif