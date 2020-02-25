#if WORKINPROGRESS

#if MIGRATION
namespace System.Windows.Navigation
#else
namespace Windows.UI.Xaml.Navigation
#endif
{
    public sealed class NavigationService
    {
        public bool CanGoBack { get; }

        public void GoBack()
        {

        }
    }
}

#endif