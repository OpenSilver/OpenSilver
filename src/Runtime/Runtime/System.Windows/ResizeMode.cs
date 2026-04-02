
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
/// Specifies whether a window can be resized and, if so, how it can be resized. Used by the 
/// <see cref="Window.ResizeMode"/> property.
/// </summary>
public enum ResizeMode
{
    /// <summary>
    /// A window cannot be resized. The Minimize and Maximize buttons are not displayed in the 
    /// title bar.
    /// </summary>
    NoResize = 0,

    /// <summary>
    /// A window can only be minimized and restored. The Minimize and Maximize buttons are both 
    /// shown, but only the Minimize button is enabled.
    /// </summary>
    CanMinimize = 1,

    /// <summary>
    /// A window can be resized. The Minimize and Maximize buttons are both shown and enabled.
    /// </summary>
    CanResize = 2,

    /// <summary>
    /// A window can be resized. The Minimize and Maximize buttons are both shown and enabled.
    /// A resize grip appears in the bottom-right corner of the window.
    /// </summary>
    CanResizeWithGrip = 3,
}
