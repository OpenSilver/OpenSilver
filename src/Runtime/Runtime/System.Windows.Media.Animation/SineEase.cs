
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
/// decelerates using a sine formula.
/// </summary>
public sealed class SineEase : EasingFunctionBase
{
    /// <inheritdoc />
    protected override double EaseInCore(double normalizedTime) =>
        1.0 - Math.Sin(Math.PI * 0.5 * (1 - normalizedTime));
}
