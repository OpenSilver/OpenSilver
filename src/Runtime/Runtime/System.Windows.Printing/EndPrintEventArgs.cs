
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

namespace System.Windows.Printing
{
    /// <summary>
    /// Provides data for the <see cref="PrintDocument.EndPrint"/> event.
    /// </summary>
    public sealed class EndPrintEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EndPrintEventArgs"/> class.
        /// </summary>
        public EndPrintEventArgs()
        {
        }

        /// <summary>
        /// Gets an exception that indicates what kind of error occurred during printing,
        /// if an error occurred.
        /// </summary>
        /// <returns>
        /// An exception that indicates what kind of error occurred during printing, if an
        /// error occurred. The default is null.
        /// </returns>
        [OpenSilver.NotImplemented]
        public Exception Error
        {
            get;
            private set;
        }
    }
}
