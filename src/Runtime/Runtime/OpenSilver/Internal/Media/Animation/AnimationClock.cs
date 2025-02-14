
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

using System.Windows;
using System.Windows.Media.Animation;

namespace OpenSilver.Internal.Media.Animation;

internal abstract class AnimationClock : TimelineClock
{
    protected AnimationClock(AnimationTimeline owner)
        : base(owner)
    {
    }

    public abstract object GetCurrentValue();

    public abstract void SetContext(DependencyObject target, DependencyProperty targetProperty);
}
