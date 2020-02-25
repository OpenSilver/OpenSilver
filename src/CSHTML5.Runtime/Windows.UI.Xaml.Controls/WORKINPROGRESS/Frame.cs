#if WORKINPROGRESS

#if MIGRATION
using System.Windows.Navigation;
#else
using System;
using Windows.UI.Xaml.Navigation;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    public partial class Frame : ContentControl, INavigate
    {
        public Uri CurrentSource { get; private set; }

        public event NavigatingCancelEventHandler Navigating;

        public event NavigationStoppedEventHandler NavigationStopped;
    }
}

#endif
