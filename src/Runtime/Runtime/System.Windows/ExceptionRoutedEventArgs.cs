

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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Windows
{
    /// <summary>
    /// Provides event data for exceptions that are raised as events by asynchronous operations, such as MediaFailed or ImageFailed.
    /// </summary>
    public partial class ExceptionRoutedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// The exception that describes the media failure.
        /// </summary>
        public Exception ErrorException { get; }

        /// <summary>
        /// Initializes a new instance of Windows.UI.Xaml.ExceptionRoutedEventArgs.
        /// </summary>
        public ExceptionRoutedEventArgs(): base() { }
    }
}
