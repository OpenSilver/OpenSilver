
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

namespace System.Windows.Media.Animation;

/// <summary>
/// Indicates the origin of a seek operation. The offset of the seek operation is relative to this origin.
/// </summary>
public enum TimeSeekOrigin
{
    /// <summary>
    /// The offset is relative to the beginning of the activation period of the <see cref="Timeline"/>.
    /// </summary>
    BeginTime = 0,

    /// <summary>
    /// The offset is relative to the <see cref="Timeline.Duration"/> of the <see cref="Timeline"/>, the 
    /// length of a single iteration. This value has no meaning if the <see cref="Timeline.Duration"/> of 
    /// the <see cref="Timeline"/> is not resolved.
    /// </summary>
    Duration = 1,
}
