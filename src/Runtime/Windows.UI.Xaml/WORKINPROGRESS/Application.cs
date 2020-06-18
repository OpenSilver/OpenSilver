

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

        ApplicationLifetimeObjectsCollection lifetime_objects;
        public IList ApplicationLifetimeObjects {
            get 
            {
                if(lifetime_objects == null)
                {
                    lifetime_objects = new ApplicationLifetimeObjectsCollection();
                }

                return lifetime_objects; 
            } 
        }
    }
}

#endif