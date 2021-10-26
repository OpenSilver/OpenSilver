
/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using System;
using System.ComponentModel;

#if MIGRATION
namespace System.Windows.Navigation
#else
namespace Windows.UI.Xaml.Navigation
#endif
{
    /// <summary>
    /// Provides data for navigation methods and event handlers that cannot cancel the
    /// navigation request.
    /// </summary>
    public sealed class NavigationEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationEventArgs"/>
        /// class, based on content and URI.
        /// </summary>
        /// <param name="content">
        /// Initializes the <see cref="Content"/> property.
        /// </param>
        /// <param name="uri">
        /// Initializes the <see cref="Uri"/> property.
        /// </param>
        public NavigationEventArgs(object content, Uri uri)
        {
            Content = content;
            Uri = uri;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationEventArgs"/>
        /// class based on content, URI, navigation type, and whether the navigation is internal
        /// to the current application.
        /// </summary>
        /// <param name="content">
        /// Initializes the <see cref="Content"/> property.
        /// </param>
        /// <param name="uri">
        /// Initializes the <see cref="Uri"/> property.
        /// </param>
        /// <param name="NavigationMode"></param>
        /// <param name="IsNavigationInitiator"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public NavigationEventArgs(object content, Uri uri, NavigationMode NavigationMode, bool IsNavigationInitiator)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the content of the target being navigated to.
        /// </summary>
        /// <returns>
        /// An object that represents the target content.
        /// </returns>
        public object Content { get; }

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

        /// <summary>
        /// Gets a value that indicates whether the navigation is forward, back, or a new
        /// navigation.
        /// </summary>
        /// <returns>
        /// A value that indicates whether the navigation is forward, back, or a new navigation.
        /// The default is <see cref="NavigationMode.New"/>.
        /// </returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public NavigationMode NavigationMode { get; }

        /// <summary>
        /// Gets the uniform resource identifier (URI) of the target.
        /// </summary>
        /// <returns>
        /// A value that represents the URI.
        /// </returns>
        public Uri Uri { get; }
    }
}