
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
/// Defines constants that indicate the state of an out-of-browser application window.
/// </summary>
public enum WindowState
{
    /// <summary>
    /// The application window is in its normal state, occupying screen space based on
    /// its <see cref="P:Window.Height"/> and <see cref="P:Window.Width"/> values.
    /// </summary>
    Normal = 0,

    /// <summary>
    /// The application window is minimized to the taskbar.
    /// </summary>
    Minimized = 1,

    /// <summary>
    /// The application window is maximized to occupy the entire client area of the screen.
    /// </summary>
    Maximized = 2,
}
