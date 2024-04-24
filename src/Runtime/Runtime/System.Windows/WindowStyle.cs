
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

namespace System.Windows;

/// <summary>
/// Defines constants that indicate the appearance of the title bar and border of an 
/// out-of-browser application window.
/// </summary>
public enum WindowStyle
{
    /// <summary>
    /// The window displays a title bar and border.
    /// </summary>
    SingleBorderWindow = 0,

    /// <summary>
    /// The window does not display a title bar or border.
    /// </summary>
    None = 1,

    /// <summary>
    /// The window does not display a title bar or border, and the window corners are rounded.
    /// </summary>
    BorderlessRoundCornersWindow = 2,
}
