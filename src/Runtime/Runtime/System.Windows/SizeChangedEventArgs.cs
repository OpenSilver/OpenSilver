
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

using System.Windows;

namespace System.Windows
{
    /// <summary>
    /// Provides data related to the SizeChanged event.
    /// </summary>
    public sealed partial class SizeChangedEventArgs : RoutedEventArgs
    {
        public SizeChangedEventArgs(Size newSize)
        {
            NewSize = newSize;
        }

        internal SizeChangedEventArgs(Size previousSize, Size newSize)
        {
            PreviousSize = previousSize;
            NewSize = newSize;
        }

        /// <summary>
        /// Gets the new size of the object reporting the size change.
        /// </summary>
        public Size NewSize { get; }

        /// <summary>
        /// Gets the previous size of the object reporting the size change.
        /// </summary>
        public Size PreviousSize { get; }
    }
}
