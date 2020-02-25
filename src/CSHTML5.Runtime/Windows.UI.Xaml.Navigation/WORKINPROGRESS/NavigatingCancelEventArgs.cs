﻿#if WORKINPROGRESS

using System.ComponentModel;
using System.Runtime.CompilerServices;

#if MIGRATION
namespace System.Windows.Navigation
#else
namespace Windows.UI.Xaml.Navigation
#endif
{
    //
    // Summary:
    //     Provides data for the System.Windows.Controls.Page.OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs)
    //     method and the System.Windows.Navigation.NavigationService.Navigating event.
    [TypeForwardedFrom("System.Windows.Controls.Navigation, Version=2.0.5.0, Culture=Neutral, PublicKeyToken=31bf3856ad364e35")]
    public sealed class NavigatingCancelEventArgs : CancelEventArgs
    {
        //
        // Summary:
        //     Initializes a new instance of the System.Windows.Navigation.NavigatingCancelEventArgs
        //     class, based on URI and mode.
        //
        // Parameters:
        //   uri:
        //     The uniform resource identifier (URI) for the content that is being navigated
        //     to.
        //
        //   mode:
        //     A value that indicates the type of navigation that is occurring.
        public NavigatingCancelEventArgs(Uri uri, NavigationMode mode)
        {

        }
        //
        // Summary:
        //     Initializes a new instance of the System.Windows.Navigation.NavigatingCancelEventArgs
        //     class, setting all initial property values.
        //
        // Parameters:
        //   uri:
        //     The uniform resource identifier (URI) for the content that is being navigated
        //     to.
        //
        //   mode:
        //     A value that indicates the type of navigation that is occurring.
        [EditorBrowsable(EditorBrowsableState.Never)]
        public NavigatingCancelEventArgs(Uri uri, NavigationMode mode, bool IsCancelable, bool IsNavigationInitiator)
        {

        }

        //
        // Summary:
        //     Gets a value that indicates whether you can cancel the navigation.
        //
        // Returns:
        //     true if you can cancel the navigation; otherwise, false.
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsCancelable { get; private set; }
        //
        // Summary:
        //     Gets a value that indicates whether the current application is the origin and
        //     destination of the navigation.
        //
        // Returns:
        //     true if the navigation starts and ends within the current application; false
        //     if the navigation starts or ends at an external location.
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsNavigationInitiator { get; private set; }
        //
        // Summary:
        //     Gets a value that indicates the type of navigation that is occurring.
        //
        // Returns:
        //     A value that indicates the type of navigation (System.Windows.Navigation.NavigationMode.Back,
        //     System.Windows.Navigation.NavigationMode.Forward, or System.Windows.Navigation.NavigationMode.New)
        //     that is occurring.
        public NavigationMode NavigationMode { get; private set; }
        //
        // Summary:
        //     Gets the uniform resource identifier (URI) for the content that is being navigated
        //     to.
        //
        // Returns:
        //     A value that represents the URI for the content.
        public Uri Uri { get; private set; }
    }
}

#endif