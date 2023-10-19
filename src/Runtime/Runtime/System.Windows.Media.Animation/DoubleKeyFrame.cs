
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

namespace System.Windows.Media.Animation
{
    /// <summary>
    /// An abstract class that defines an animation segment with its own target value
    /// and interpolation method for a DoubleAnimationUsingKeyFrames.
    /// </summary>
    public abstract class DoubleKeyFrame : DependencyObject, IKeyFrame
    {
        public static readonly DependencyProperty KeyTimeProperty = DependencyProperty.Register("KeyTime", typeof(KeyTime), typeof(DoubleKeyFrame), new PropertyMetadata(new KeyTime()));
        /// <summary>Gets or sets the time at which the key frame's target <see cref="P:System.Windows.Media.Animation.DoubleKeyFrame.Value" /> should be reached.</summary>
        /// <returns>The time at which the key frame's current value should be equal to its <see cref="P:System.Windows.Media.Animation.DoubleKeyFrame.Value" /> property. The default is null.</returns>
        public KeyTime KeyTime
        {
            get { return (KeyTime)GetValue(KeyTimeProperty); }
            set { SetValue(KeyTimeProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(DoubleKeyFrame), new PropertyMetadata(0d));
        /// <summary>Gets or sets the key frame's target value. </summary>
        /// <returns>The key frame's target value, which is the value of this key frame at its specified <see cref="P:System.Windows.Media.Animation.DoubleKeyFrame.KeyTime" />. The default is 0.</returns>
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// The value of this key frame at the KeyTime specified.
        /// </summary>
        object IKeyFrame.Value
        {
            get
            {
                return Value;
            }
            set
            {
                Value = (double)value;
            }
        }

        internal virtual EasingFunctionBase INTERNAL_GetEasingFunction()
        {
            return null;
        }
    }
}
