

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

using OpenSilver.Internal;
using System;
using System.Collections;
using System.Windows.Resources;

#if !MIGRATION
using System.Windows;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    public delegate void CheckAndDownloadUpdateCompletedEventHandler(object sender,
        CheckAndDownloadUpdateCompletedEventArgs e);

    /// <summary>
    /// Encapsulates the app and its available services.
    /// </summary>
    public partial class Application
    {
        // Summary:
        //     Gets a collection of the System.Windows.Window instances that have been created.
        //
        // Returns:
        //     A collection of the windows used by the application.
        //
        // Exceptions:
        //   T:System.NotSupportedException:
        //     The application is not running outside the browser.
        //
        //   T:System.UnauthorizedAccessException:
        //     The current thread is not the user interface (UI) thread.
        [OpenSilver.NotImplemented]
        public WindowCollection Windows { get; }

        /// <summary>
        /// Occurs when an exception that is raised is not handled.
        /// </summary>
        public event EventHandler<ApplicationUnhandledExceptionEventArgs> UnhandledException;

        /// <summary>Occurs when the <see cref="P:System.Windows.Application.InstallState" /> property value changes. </summary>
        [OpenSilver.NotImplemented]
        public event EventHandler InstallStateChanged;

        internal void OnUnhandledException(Exception exception, bool handled)
        {
            if (UnhandledException != null)
            {
                UnhandledException(this, new ApplicationUnhandledExceptionEventArgs(exception, handled));
            }
        }

        //        void RaiseUnhandledException(object jsElement)
        //        {
        //#if !MIGRATION
        //            UnhandledException(null, new ApplicationUnhandledExceptionEventArgs())
        //#else
        //            UnhandledException(null, new ApplicationUnhandledExceptionEventArgs(null, true));
        //#endif
        //        }

        [OpenSilver.NotImplemented]
        public bool IsRunningOutOfBrowser
        {
            get { return Application.Current.Host.Settings.IsRunningOutOfBrowser; }
        }

        /// <summary>Gets the current out-of-browser installation state of the application.</summary>
        /// <returns>The out-of-browser installation state of the application.</returns>
        [OpenSilver.NotImplemented]
        public InstallState InstallState { get; }

        [OpenSilver.NotImplemented]
        public bool HasElevatedPermissions { get; set; }

        [OpenSilver.NotImplemented]
        public static StreamResourceInfo GetResourceStream(StreamResourceInfo zipPackageStreamResourceInfo, Uri uriResource)
        {
            return null;
        }

        /// <summary>Attempts to install the application so that it can run outside the browser. </summary>
        /// <returns>true if the user gave consent to install the application or the application requires elevated trust; otherwise, false. See Remarks for more information. </returns>
        /// <exception cref="T:System.InvalidOperationException">The application is already installed.-or-The application is hosted at a URI with a scheme other than http, https, or file.</exception>
        [OpenSilver.NotImplemented]
        public bool Install()
        {
            if (Current.Host.Source.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase) || Current.Host.Source.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase) || Current.Host.Source.Scheme.Equals("file", StringComparison.OrdinalIgnoreCase))
            {
                if (InstallState.Installed == InstallState)
                {
                    throw new InvalidOperationException("Application is already installed.");
                }

                throw new NotImplementedException($"Installing application is not implemented yet.");
            }

            throw new InvalidOperationException("Application cannot be installed.");
        }

        [OpenSilver.NotImplemented]
        public event CheckAndDownloadUpdateCompletedEventHandler CheckAndDownloadUpdateCompleted;

        [OpenSilver.NotImplemented]
        public void CheckAndDownloadUpdateAsync()
        {
        }
    }
}
