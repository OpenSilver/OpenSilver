#if WORKINPROGRESS

using System;
using System.Collections;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
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
        public WindowCollection Windows { get; }

        /// <summary>
        /// Occurs when an exception that is raised is not handled.
        /// </summary>
        public event EventHandler<ApplicationUnhandledExceptionEventArgs> UnhandledException;

        //        void RaiseUnhandledException(object jsElement)
        //        {
        //#if !MIGRATION
        //            UnhandledException(null, new ApplicationUnhandledExceptionEventArgs())
        //#else
        //            UnhandledException(null, new ApplicationUnhandledExceptionEventArgs(null, true));
        //#endif
        //        }

        public bool IsRunningOutOfBrowser
        {
            get { return false; }
        }

        public bool HasElevatedPermissions { get; set; }

        public IList ApplicationLifetimeObjects { get; }
    }
}

#endif