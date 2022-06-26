
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
    /// <summary>
    /// Defines values for the type of devices the tablet device uses.
    /// </summary>
    public enum TabletDeviceType
    {
        /// <summary>
        /// Indicates the tablet device is a mouse.
        /// </summary>
        Mouse = 0,
        /// <summary>
        /// Indicates the tablet device is a stylus.
        /// </summary>
        Stylus = 1,
        /// <summary>
        /// Indicates the tablet device is a touch screen.
        /// </summary>
        Touch = 2
    }
}
