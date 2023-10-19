
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

namespace System.Windows
{
    //
    // Summary:
    //     Defines constants that indicate the state of an out-of-browser application window.
    public enum WindowState
    {
        //
        // Summary:
        //     The application window is in its normal state, occupying screen space based on
        //     its System.Windows.Window.Height and System.Windows.Window.Width values.
        Normal = 0,
        //
        // Summary:
        //     The application window is minimized to the taskbar.
        Minimized = 1,
        //
        // Summary:
        //     The application window is maximized to occupy the entire client area of the screen.
        Maximized = 2
    }
}
