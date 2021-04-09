#if WORKINPROGRESS

using System;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    public partial class VisualStateGroup : DependencyObject
    {
        public VisualStateGroup() { }
        public event EventHandler<VisualStateChangedEventArgs> CurrentStateChanged;
    }
}
#endif