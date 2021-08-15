﻿

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


#if WORKINPROGRESS
#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    public enum WindowStyle
    {
        //
        // Summary:
        //     The window displays a title bar and border.
        SingleBorderWindow = 0,
        //
        // Summary:
        //     The window does not display a title bar or border.
        None = 1,
        //
        // Summary:
        //     The window does not display a title bar or border, and the window corners are
        //     rounded.
        BorderlessRoundCornersWindow = 2
    }
}
#endif