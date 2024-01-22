
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

using OpenSilver.Internal.Media.Animation;

namespace System.Windows.Media.Animation;

/// <summary>
/// A class that enables you to associate easing functions with a 
/// <see cref="ColorAnimationUsingKeyFrames"/> key frame animation.
/// </summary>
public sealed class EasingColorKeyFrame : ColorKeyFrame
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EasingColorKeyFrame"/> class.
    /// </summary>
    public EasingColorKeyFrame() { }

    /// <summary>
    /// Identifies the <see cref="EasingFunction"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty EasingFunctionProperty =
        DependencyProperty.Register(
            nameof(EasingFunction),
            typeof(IEasingFunction),
            typeof(EasingColorKeyFrame),
            new PropertyMetadata((object)null));

    /// <summary>
    /// Gets or sets the easing function that is applied to the key frame.
    /// </summary>
    /// <returns>
    /// The easing function that is applied to the key frame.
    /// </returns>
    public IEasingFunction EasingFunction
    {
        get => (IEasingFunction)GetValue(EasingFunctionProperty);
        set => SetValueInternal(EasingFunctionProperty, value);
    }

    /// <inheritdoc />
    internal override Color InterpolateValueCore(Color baseValue, double keyFrameProgress)
    {
        if (EasingFunction is IEasingFunction easingFunction)
        {
            keyFrameProgress = easingFunction.Ease(keyFrameProgress);
        }

        return keyFrameProgress switch
        {
            0.0 => baseValue,
            1.0 => Value,
            _ => AnimatedTypeHelpers.InterpolateColor(baseValue, Value, keyFrameProgress)
        };
    }
}
