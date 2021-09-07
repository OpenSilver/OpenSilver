

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

#if MIGRATION
namespace System.Windows.Input
#else
namespace Windows.UI.Xaml.Input
#endif
{
    //
    // Summary:
    //     Defines values for the type of devices the tablet device uses.
    public enum TabletDeviceType
    {
        //
        // Summary:
        //     Indicates the tablet device is a mouse.
        Mouse = 0,
        //
        // Summary:
        //     Indicates the tablet device is a stylus.
        Stylus = 1,
        //
        // Summary:
        //     Indicates the tablet device is a touch screen.
        Touch = 2
    }
}
