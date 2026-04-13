
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

using System.ComponentModel;

namespace System.Windows.Input
{
    /// <summary>
    /// Provides event data for the RightTapped event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class RightTappedRoutedEventArgs : MouseEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RightTappedRoutedEventArgs"/> class.
        /// </summary>
        public RightTappedRoutedEventArgs()
            : base(Mouse.PrimaryDevice, Environment.TickCount)
        {
        }
    }
}
