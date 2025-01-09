
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
/// Contains properties that you can use to query system settings.
/// </summary>
public static class SystemParameters
{
    /// <summary>
    /// Gets a value that indicates whether the client computer is in high-contrast mode.
    /// </summary>
    /// <returns>
    /// true if the client computer is in high-contrast mode; otherwise, false.
    /// </returns>
    [OpenSilver.NotImplemented]
    public static bool HighContrast => false;

    /// <summary>
    /// Gets a value that indicates the number of lines to scroll vertically in response to mouse wheel events.
    /// </summary>
    /// <returns>
    /// The number of lines to scroll vertically in response to mouse wheel events. This always return 3.
    /// </returns>
    public static int WheelScrollLines => 3;
}
