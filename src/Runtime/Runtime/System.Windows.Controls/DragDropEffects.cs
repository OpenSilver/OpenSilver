

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


#if MIGRATION
namespace Microsoft.Windows
#else
namespace System.Windows
#endif
{
    /// <summary>
    /// Specifies the effects of a drag-and-drop operation.
    /// </summary>
    [Flags]
    public enum DragDropEffects
    {
        /// <summary>
        /// Scrolling is about to start or is currently occurring in the drop target.
        /// </summary>
        Scroll = -2147483648,

        /// <summary>
        /// The data is copied, removed from the drag source, and scrolled in the drop
        /// target.
        /// </summary>
        All = -2147483645,

        /// <summary>
        /// The drop target does not accept the data.
        /// </summary>
        None = 0,

        /// <summary>
        /// The data is copied to the drop target.
        /// </summary>
        Copy = 1,

        /// <summary>
        /// The data from the drag source is moved to the drop target.
        /// </summary>
        Move = 2,

        /// <summary>
        /// The data from the drag source is linked to the drop target.
        /// </summary>
        Link = 4,
    }
}