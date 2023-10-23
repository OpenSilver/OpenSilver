
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
/// Animates from the <see cref="Color"/> value of the previous key frame
/// to its own <see cref="ColorKeyFrame.Value"/> using discrete values.
/// </summary>
public sealed class DiscreteColorKeyFrame : ColorKeyFrame
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DiscreteColorKeyFrame"/> class.
    /// </summary>
    public DiscreteColorKeyFrame() { }

    /// <inheritdoc />
    internal override Color InterpolateValueCore(Color baseValue, double keyFrameProgress) =>
        keyFrameProgress switch
        {
            < 1.0 => baseValue,
            _ => Value
        };
}
