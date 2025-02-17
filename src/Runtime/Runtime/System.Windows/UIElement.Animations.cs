
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

using System.Windows.Media.Animation;
using OpenSilver.Internal;
using OpenSilver.Internal.Media.Animation;

namespace System.Windows;

public partial class UIElement
{
    /// <summary>
    /// Starts an animation for a specified animated property on this element.
    /// </summary>
    /// <param name="dp">
    /// The property to animate, which is specified as a dependency property identifier.
    /// </param>
    /// <param name="animation">
    /// The timeline of the animation to start.
    /// </param>
    /// <remarks>
    /// When you check whether a property is animated, note that the animation will begin and be considered 
    /// animated when the first frame beyond the non-animated starting point is rendered.
    /// If the <see cref="Timeline.BeginTime"/> for animation is null, then any current animations are removed 
    /// and the current value of the property is held.
    /// If the entire <paramref name="animation"/> value is null, all animations are removed from the property 
    /// and the property value reverts to its base value. However, the originally associated animation timeline 
    /// is not stopped. Any other animations assigned to that timeline will continue to run.
    /// </remarks>
    public void BeginAnimation(DependencyProperty dp, AnimationTimeline animation)
    {
        if (dp is null)
        {
            throw new ArgumentNullException(nameof(dp));
        }

        if (!IsPropertyAnimatable(this, dp))
        {
            throw new ArgumentException(
                string.Format(Strings.Animation_DependencyPropertyIsNotAnimatable, dp.Name, GetType()),
                nameof(dp));
        }

        if (animation is not null && !IsAnimationValid(dp, animation))
        {
            throw new ArgumentException(
                string.Format(
                    Strings.Animation_AnimationTimelineTypeMismatch,
                    animation.GetType(),
                    dp.Name,
                    dp.PropertyType),
                nameof(animation));
        }

        if (animation is null)
        {
            DetachAnimationClock(dp, true);
        }
        else if (animation.BeginTime.HasValue)
        {
            if (animation.CreateClock() is AnimationClock animationClock)
            {
                animationClock.IsRoot = true;
                animationClock.HasControllableRoot = true;
                animationClock.SetContext(this, dp);

                animationClock.InternalBegin(false);
            }
        }
        else
        {
            DetachAnimationClock(dp, false);
        }
    }

    internal static bool IsPropertyAnimatable(DependencyObject d, DependencyProperty dp)
    {
        if (dp.ReadOnly)
        {
            return false;
        }

        if (dp.GetMetadata(d.DependencyObjectType) is UIPropertyMetadata uiMetadata && uiMetadata.IsAnimationProhibited)
        {
            return false;
        }

        return true;
    }

    internal static bool IsAnimationValid(DependencyProperty dp, AnimationTimeline animation)
    {
        Type targetPropertyType = animation.TargetPropertyType;
        return dp.PropertyType.IsAssignableFrom(targetPropertyType) || targetPropertyType == typeof(object);
    }
}
