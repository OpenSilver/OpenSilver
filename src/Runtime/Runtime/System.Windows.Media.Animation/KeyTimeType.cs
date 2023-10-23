
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
/// Represents the different types that may represent a <see cref="KeyTime"/> instance.
/// </summary>
public enum KeyTimeType : byte
{
    /// <summary>
    /// The allotted total time for an animation sequence is divided evenly amongst each
    /// of the key frames.
    /// </summary>
    Uniform = 0,

    /// <summary>
    /// Each <see cref="KeyTime"/> is expressed as a <see cref="KeyTime.TimeSpan"/> value 
    /// relative to the <see cref="Timeline.BeginTime"/> of an animation sequence.
    /// </summary>
    TimeSpan = 2,
}
