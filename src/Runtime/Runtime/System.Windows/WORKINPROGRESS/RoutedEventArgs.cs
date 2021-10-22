

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

#if !MIGRATION
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Contains state information and event data associated with a routed event.
    /// </summary>
    public partial class RoutedEventArgs
    {
        /// <summary>
        ///     Returns a boolean flag indicating if or not this
        ///     RoutedEvent has been handled this far in the route
        /// </summary>
        /// <remarks>
        ///     Initially starts with a false value before routing
        ///     has begun
        /// </remarks>
		[OpenSilver.NotImplemented]
        public bool Handled { get; set; }
    }
}
