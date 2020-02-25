#if WORKINPROGRESS

#if MIGRATION
namespace System.Windows.Navigation
#else
namespace Windows.UI.Xaml.Navigation
#endif
{
    //
    // Summary:
    //     Represents the method that will handle the System.Windows.Navigation.NavigationService.NavigationStopped
    //     event.
    //
    // Parameters:
    //   sender:
    //     The source of the event.
    //
    //   e:
    //     The data for the event.
    public delegate void NavigationStoppedEventHandler(object sender, NavigationEventArgs e);
}

#endif