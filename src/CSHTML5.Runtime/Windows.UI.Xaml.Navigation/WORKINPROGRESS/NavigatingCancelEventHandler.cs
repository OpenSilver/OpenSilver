#if WORKINPROGRESS

using System.Runtime.CompilerServices;

#if MIGRATION
namespace System.Windows.Navigation
#else
namespace Windows.UI.Xaml.Navigation
#endif
{
    //
    // Summary:
    //     Represents the method that will handle the System.Windows.Navigation.NavigationService.Navigating
    //     event, which is a cancelable event.
    //
    // Parameters:
    //   sender:
    //     The source of the event.
    //
    //   e:
    //     The data for the event.
    //[TypeForwardedFrom("System.Windows.Controls.Navigation, Version=2.0.5.0, Culture=Neutral, PublicKeyToken=31bf3856ad364e35")]
    public delegate void NavigatingCancelEventHandler(object sender, NavigatingCancelEventArgs e);
}

#endif