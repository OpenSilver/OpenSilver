
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
/// Represents an easing function that creates an animation that accelerates
/// and/or decelerates using the formula f(t) = t5.
/// </summary>
public sealed class QuinticEase : EasingFunctionBase
{
    /// <inheritdoc />
    protected override double EaseInCore(double normalizedTime) =>
        normalizedTime * normalizedTime * normalizedTime * normalizedTime * normalizedTime;
}
