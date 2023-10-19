
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

namespace System.Windows.Controls.Primitives
{
    /// <summary>
    /// Specifies the preferred location for positioning a ToolTip relative to a visual element.
    /// </summary>
    public enum PlacementMode
    {
        /// <summary>
        /// Indicates that the preferred location of the tooltip is at the bottom of the target element.
        /// </summary>
        Bottom = 2,

        /// <summary>
        /// Indicates that the preferred location of the tooltip is at the right of the target element.
        /// </summary>
        Right = 4,

        /// <summary>
        /// Indicates that the preferred location of the tooltip is at the mouse pointer location.
        /// </summary>
        Mouse = 7,

        /// <summary>
        /// Indicates that the preferred location of the tooltip is at the left of the target element.
        /// </summary>
        Left = 9,

        /// <summary>
        /// Indicates that the preferred location of the tooltip is at the top of the target element.
        /// </summary>
        Top = 10,
    }
}
