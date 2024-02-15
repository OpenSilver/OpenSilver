
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

namespace System.Windows
{
    /// <summary>
    /// Provides data for the <see cref="Application.UnhandledException"/> event.
    /// </summary>
    public class ApplicationUnhandledExceptionEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationUnhandledExceptionEventArgs"/> class.
        /// </summary>
        /// <param name="ex">
        /// The exception that is being thrown as unhandled.
        /// </param>
        /// <param name="handled">
        /// A value that indicates whether the exception has been handled and should not be processed further.
        /// </param>
        public ApplicationUnhandledExceptionEventArgs(Exception ex, bool handled)
        {
            ExceptionObject = ex;
            Handled = handled;
        }

        /// <summary>
        /// Gets or sets the unhandled exception.
        /// </summary>
        /// <returns>
        /// The unhandled exception.
        /// </returns>
        public Exception ExceptionObject { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the exception is handled.
        /// </summary>
        /// <returns>
        /// true to mark the exception as handled, which indicates that Silverlight should
        /// not process it further; otherwise, false.
        /// </returns>
        public bool Handled { get; set; }
    }
}
