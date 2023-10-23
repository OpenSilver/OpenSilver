
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
/// Represents an easing function that creates an animation that accelerates and/or
/// decelerates using a circular function.
/// </summary>
public sealed class CircleEase : EasingFunctionBase
{
    /// <inheritdoc />
    protected override double EaseInCore(double normalizedTime)
    {
        normalizedTime = Math.Max(0.0, Math.Min(1.0, normalizedTime));
        return 1.0 - Math.Sqrt(1.0 - normalizedTime * normalizedTime);
    }
}