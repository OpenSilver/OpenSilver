﻿
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows.Navigation
#else
namespace Windows.UI.Xaml.Navigation
#endif
{
    /// <summary>
    /// Provides data for navigation methods and event handlers that cannot cancel
    /// the navigation request.</summary>
    public sealed partial class NavigationEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the NavigationEventArgs class, based on content and URI.
        /// </summary>
        /// <param name="content">Initializes the NavigationEventArgs.Content property.</param>
        /// <param name="uri">Initializes the NavigationEventArgs.Uri property.</param>
        public NavigationEventArgs(object content, Uri uri)
        {
            _content = content;
            _uri = uri;
        }

        /// <summary>
        /// Gets the content of the target being navigated to.
        /// </summary>
        private object _content;
        public object Content
        {
            get { return _content; }
        }

       
        /// <summary>
        /// Gets the uniform resource identifier (URI) of the target.
        /// </summary>
        private Uri _uri;
        public Uri Uri
        {
            get { return _uri; }
        }


        #region not implemented

        ///// <summary>
        ///// Initializes a new instance of the System.Windows.Navigation.NavigationEventArgs
        ///// class based on content, URI, navigation type, and whether the navigation
        ///// is internal to the current application.
        ///// </summary>
        ///// <param name="content">Initializes the System.Windows.Navigation.NavigationEventArgs.Content property.</param>
        ///// <param name="uri">Initializes the System.Windows.Navigation.NavigationEventArgs.Uri property.</param>
        ///// <param name="NavigationMode">Initializes the System.Windows.Navigation.NavigationEventArgs.NavigationMode property.</param>
        ///// <param name="IsNavigationInitiator">Initializes the System.Windows.Navigation.NavigationEventArgs.IsNavigationInitiator property.</param>
        ////[EditorBrowsable(EditorBrowsableState.Never)]
        //public NavigationEventArgs(object content, Uri uri, NavigationMode NavigationMode, bool IsNavigationInitiator);

        //// Returns:
        ////     true if the navigation starts and ends within the current application; false
        ////     if the navigation starts or ends at an external location.
        ///// <summary>
        ///// Gets a value that indicates whether the current application is the origin
        ///// and destination of the navigation.
        ///// </summary>
        ////[EditorBrowsable(EditorBrowsableState.Never)]
        //public bool IsNavigationInitiator { get; }

        //// Returns:
        ////     A value that indicates whether the navigation is forward, back, or a new
        ////     navigation. The default is System.Windows.Navigation.NavigationMode.New.
        ///// <summary>
        ///// Gets a value that indicates whether the navigation is forward, back, or a
        ///// new navigation.
        ///// </summary>
        ////[EditorBrowsable(EditorBrowsableState.Never)]
        //public NavigationMode NavigationMode { get; }
        #endregion
    }
}