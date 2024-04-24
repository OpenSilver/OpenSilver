
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
/// Defines constants that indicate whether elevated permissions are required for
/// an out-of-browser application.
/// </summary>
public enum ElevatedPermissions
{
    /// <summary>
    /// Elevated permissions are not required to run the application outside the browser.
    /// </summary>
    NotRequired = 0,

    /// <summary>
    /// Elevated permissions are required to run the application outside the browser.
    /// </summary>
    Required = int.MaxValue
}
