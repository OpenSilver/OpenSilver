
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

namespace System.Windows.Controls
{
    /// <summary>
    /// Specifies the Dock position of a child element that
    /// is inside a DockPanel.
    /// </summary>
    public enum Dock
    {
        /// <summary>
        /// A child element that is positioned on the left side of the DockPanel.
        /// </summary>
        Left = 0,
        /// <summary>
        /// A child element that is positioned at the top of the DockPanel.
        /// </summary>
        Top = 1,
        /// <summary>
        /// A child element that is positioned on the right side of the DockPanel.
        /// </summary>
        Right = 2,
        /// <summary>
        /// A child element that is positioned at the bottom of the DockPanel.
        /// </summary>
        Bottom = 3,
    }
}