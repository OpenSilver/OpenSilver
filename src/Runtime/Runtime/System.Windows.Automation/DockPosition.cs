
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

namespace System.Windows.Automation
{
    /// <summary>
    /// Contains values that specify the dock position of an object within a docking container.
    /// </summary>
    public enum DockPosition
    {
        /// <summary>
        /// Indicates that the UI automation element is docked along the top edge of the docking container.
        /// </summary>
        Top,
        /// <summary>
        /// Indicates that the UI automation element is docked along the left edge of the docking container.
        /// </summary>
        Left,
        /// <summary>
        /// Indicates that the UI automation element is docked along the bottom edge of the docking container.
        /// </summary>
        Bottom,
        /// <summary>
        /// Indicates that the UI automation element is docked along the right edge of the docking container.
        /// </summary>
        Right,
        /// <summary>
        /// Indicates that the UI automation element is docked along all edges of the docking container and 
        /// fills all available space within the container.
        /// </summary>
        Fill,
        /// <summary>
        /// Indicates that the UI automation element is not docked to any edge of the docking container.
        /// </summary>
        None,
    }
}
