
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
/// Defines constants that represent the edges and corners of a Silverlight out-of-browser
/// application window.
/// </summary>
public enum WindowResizeEdge
{
    /// <summary>
    /// The left edge of the window.
    /// </summary>
    Left = 1,

    /// <summary>
    /// The right edge of the window.
    /// </summary>
    Right = 2,

    /// <summary>
    /// The upper edge of the window.
    /// </summary>
    Top = 3,

    /// <summary>
    /// The upper-left corner of the window.
    /// </summary>
    TopLeft = 4,

    /// <summary>
    /// The upper-right corner of the window.
    /// </summary>
    TopRight = 5,

    /// <summary>
    /// The lower edge of the window.
    /// </summary>
    Bottom = 6,

    /// <summary>
    /// The lower-left corner of the window.
    /// </summary>
    BottomLeft = 7,

    /// <summary>
    /// The lower-right corner of the window.
    /// </summary>
    BottomRight = 8,
}
