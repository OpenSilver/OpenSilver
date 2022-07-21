
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

#if MIGRATION
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
    /// <summary>
    /// Provides a base class for specific animation key-frame techniques that define
    /// an animation segment with a <see cref="Color"/> target value. Derived
    /// classes each provide a different key-frame interpolation method for a <see cref="Color"/>
    /// value that is provided for a <see cref="ColorAnimationUsingKeyFrames"/>
    /// animation.
    /// </summary>
    public abstract class ColorKeyFrame : DependencyObject, IKeyFrame
    {
        /// <summary>
        /// Identifies the <see cref="KeyTime"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty KeyTimeProperty =
            DependencyProperty.Register(
                nameof(KeyTime),
                typeof(KeyTime),
                typeof(ColorKeyFrame),
                null);

        /// <summary>
        /// Gets or sets the time at which the key frame's target <see cref="Value"/>.
        /// </summary>
        /// <returns>
        /// The time at which the key frame's current value should be equal to its <see cref="Value"/>
        /// property. The default is null.
        /// </returns>
        public KeyTime KeyTime
        {
            get => (KeyTime)GetValue(KeyTimeProperty);
            set => SetValue(KeyTimeProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Value"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                nameof(Value),
                typeof(Color),
                typeof(ColorKeyFrame),
                null);

        /// <summary>
        /// Gets or sets the key frame's target value.
        /// </summary>
        /// <returns>
        /// The key frame's target value, which is the value at its specified <see cref="KeyTime"/>.
        /// The default is a <see cref="Color"/> with an ARGB value of #00000000.
        /// </returns>
        public Color Value
        {
            get => (Color)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        object IKeyFrame.Value
        {
            get => Value;
            set => Value = (Color)value;
        }

        internal virtual EasingFunctionBase INTERNAL_GetEasingFunction() => null;
    }
}
