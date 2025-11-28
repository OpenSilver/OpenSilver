
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

namespace System.Windows.Media.Effects;

/// <summary>
/// Specifies how dependency properties with <see cref="Brush"/> values are sampled in a custom shader effect.
/// </summary>
public enum SamplingMode
{
    /// <summary>
    /// The system selects the most appropriate sampling mode.
    /// </summary>
    Auto = 0,

    /// <summary>
    /// Use nearest neighbor sampling.
    /// </summary>
    NearestNeighbor = 1,

    /// <summary>
    /// Use bilinear sampling.
    /// </summary>
    Bilinear = 2,
}
