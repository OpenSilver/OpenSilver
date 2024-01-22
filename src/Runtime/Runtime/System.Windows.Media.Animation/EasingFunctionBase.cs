
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
/// Provides the base class for all the easing functions. You can create your own
/// custom easing functions by inheriting from this class.
/// </summary>
public abstract class EasingFunctionBase : DependencyObject, IEasingFunction
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EasingFunctionBase" /> class.
    /// </summary>
    protected EasingFunctionBase() { }

    /// <summary>
    /// Identifies the <see cref="EasingMode"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty EasingModeProperty =
        DependencyProperty.Register(
            nameof(EasingMode),
            typeof(EasingMode),
            typeof(EasingFunctionBase),
            new PropertyMetadata(EasingMode.EaseOut));

    /// <summary>
    /// Gets or sets a value that specifies how the animation interpolates.
    /// </summary>
    public EasingMode EasingMode
    {
        get => (EasingMode)GetValue(EasingModeProperty);
        set => SetValueInternal(EasingModeProperty, value);
    }

    /// <summary>
    /// Transforms normalized time to control the pace of an animation.
    /// </summary>
    /// <param name="normalizedTime">
    /// Normalized time (progress) of the animation.
    /// </param>
    /// <returns>
    /// A double that represents the transformed progress.
    /// </returns>
    public double Ease(double normalizedTime)
    {
        switch (EasingMode)
        {
            case EasingMode.EaseIn:
                return EaseInCore(normalizedTime);

            case EasingMode.EaseOut:
                // EaseOut is the same as EaseIn, except time is reversed & the result is flipped.
                return 1.0 - EaseInCore(1.0 - normalizedTime);

            case EasingMode.EaseInOut:
            default:
                // EaseInOut is a combination of EaseIn & EaseOut fit to the 0-1, 0-1 range.
                return (normalizedTime < 0.5) ?
                    EaseInCore(normalizedTime * 2.0) * 0.5 :
                    (1.0 - EaseInCore((1.0 - normalizedTime) * 2.0)) * 0.5 + 0.5;
        }
    }

    /// <summary>
    /// Provides the logic portion of the easing function that you can override to produce
    /// the <see cref="EasingMode.EaseIn"/> mode of the custom easing function.
    /// </summary>
    /// <param name="normalizedTime">
    /// Normalized time (progress) of the animation.
    /// </param>
    /// <returns>
    /// A double that represents the transformed progress.
    /// </returns>
    protected abstract double EaseInCore(double normalizedTime);
}
