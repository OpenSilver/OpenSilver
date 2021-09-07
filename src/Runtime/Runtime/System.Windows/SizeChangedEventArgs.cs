

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


#if MIGRATION
using System.Windows;
#else
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Provides data related to the SizeChanged event.
    /// </summary>
    public sealed partial class SizeChangedEventArgs : RoutedEventArgs
    {
        Size _newSize;

        public SizeChangedEventArgs(Size newSize)
        {
            _newSize = newSize;
        }

        /// <summary>
        /// Gets the new size of the object reporting the size change.
        /// </summary>
        public Size NewSize
        {
            get
            {
                return _newSize;
            }
        }

        /// <summary>
        /// Gets the previous size of the object reporting the size change.
        /// </summary>
        [OpenSilver.NotImplemented]
        public Size PreviousSize { get; private set; }
    }
}
