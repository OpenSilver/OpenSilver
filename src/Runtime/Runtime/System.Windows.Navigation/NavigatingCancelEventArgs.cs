//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel;

#if MIGRATION
namespace System.Windows.Navigation
#else
namespace Windows.UI.Xaml.Navigation
#endif
{
    /// <summary>
    /// Provides data for the <c>Page.OnNavigatingFrom(NavigatingCancelEventArgs)</c>
    /// method and the <c>NavigationService.Navigating</c> event.
    /// </summary>
    public sealed class NavigatingCancelEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigatingCancelEventArgs"/>
        /// class, based on URI and mode.
        /// </summary>
        /// <param name="uri">
        /// The uniform resource identifier (URI) for the content that is being navigated
        /// to.
        /// </param>
        /// <param name="mode">
        /// A value that indicates the type of navigation that is occurring.
        /// </param>
        public NavigatingCancelEventArgs(Uri uri, NavigationMode mode)
        {
            Uri = uri;
            NavigationMode = mode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigatingCancelEventArgs"/>
        /// class, setting all initial property values.
        /// </summary>
        /// <param name="uri">
        /// The uniform resource identifier (URI) for the content that is being navigated
        /// to.
        /// </param>
        /// <param name="mode">
        /// A value that indicates the type of navigation that is occurring.
        /// </param>
        /// <param name="IsCancelable"></param>
        /// <param name="IsNavigationInitiator"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public NavigatingCancelEventArgs(Uri uri, NavigationMode mode, bool IsCancelable, bool IsNavigationInitiator)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a value that indicates the type of navigation that is occurring.
        /// </summary>
        /// <returns>
        /// A value that indicates the type of navigation (<see cref="NavigationMode.Back"/>,
        /// <see cref="NavigationMode.Forward"/>, or <see cref="NavigationMode.New"/>)
        /// that is occurring.
        /// </returns>
        public NavigationMode NavigationMode { get; }

        /// <summary>
        /// Gets the uniform resource identifier (URI) for the content that is being navigated
        /// to.
        /// </summary>
        /// <returns>
        /// A value that represents the URI for the content.
        /// </returns>
        public Uri Uri { get; }

        /// <summary>
        /// Gets a value that indicates whether you can cancel the navigation.
        /// </summary>
        /// <returns>
        /// true if you can cancel the navigation; otherwise, false.
        /// </returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsCancelable => throw new NotImplementedException();

        /// <summary>
        /// Gets a value that indicates whether the current application is the origin and
        /// destination of the navigation.
        /// </summary>
        /// <returns>
        /// true if the navigation starts and ends within the current application; false
        /// if the navigation starts or ends at an external location.
        /// </returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsNavigationInitiator => throw new NotImplementedException();
    }
}
