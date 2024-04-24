
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
/// Defines constants that indicate how an out-of-browser application window is positioned at startup.
/// </summary>
public enum WindowStartupLocation
{
    /// <summary>
    /// The application window is centered in the screen, and the <see cref="WindowSettings.Top"/>
    /// and <see cref="WindowSettings.Left"/> settings are ignored.
    /// </summary>
    CenterScreen = 0,

    /// <summary>
    /// The application window is positioned according to the <see cref="WindowSettings.Top"/>
    /// and <see cref="WindowSettings.Left"/> settings.
    /// </summary>
    Manual = 1
}
