

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
using System.Windows.Input;
#else
using Windows.UI.Xaml.Input;
#endif

#if MIGRATION
namespace Microsoft.Windows
#else
namespace System.Windows
#endif
{
    /// <summary>
    /// Provides helper methods and fields for drag-and-drop operations.
    /// </summary>
    public static class DragDrop
    {
        /// <summary>
        /// Gets a value indicating whether a drag is in progress.
        /// </summary>
        public static bool IsDragInProgress
        {
            get
            {
                return (Pointer.INTERNAL_captured != null);
            }
        }
    }
}
