
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

namespace System.Windows.Media;

/// <summary>
/// Determines the means by which an error tolerance value is interpreted.
/// </summary>
public enum ToleranceType
{
    /// <summary>
    /// Error tolerance is treated as an absolute value.
    /// </summary>
    Absolute = 0,

    /// <summary>
    /// Error tolerance is treated as a relative value.
    /// </summary>
    Relative = 1,
}
