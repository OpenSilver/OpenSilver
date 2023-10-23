
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

using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Animation;

namespace OpenSilver.Internal.Media.Animation;

internal sealed class FromToByAnimator<T> : IValueAnimator<T>
        where T : struct
{
    private readonly IFromByToAnimation<T> _owner;

    public FromToByAnimator(IFromByToAnimation<T> owner)
    {
        Debug.Assert(owner is not null);
        _owner = owner;
    }

    T IValueAnimator<T>.GetCurrentValue(T initialValue, DependencyProperty dp, TimelineClock clock)
    {
        double progress = clock.CurrentProgress;

        if (_owner.EasingFunction is IEasingFunction easingFunction)
        {
            progress = easingFunction.Ease(progress);
        }

        T from = _owner.From ?? initialValue;
        T to = _owner.To ?? from;

        return _owner.InterpolateValue(from, to, progress);
    }
}
