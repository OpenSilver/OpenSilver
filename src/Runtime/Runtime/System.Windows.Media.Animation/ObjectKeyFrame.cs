
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
    /// Defines an animation segment with its own target value and interpolation
    /// method for an ObjectAnimationUsingKeyFrames.
    /// </summary>
    public class ObjectKeyFrame : DependencyObject, IKeyFrame
    {
        /// <summary>
        /// Gets or sets the time at which the key frame's target Value should be reached.
        /// </summary>
        public KeyTime KeyTime
        {
            get { return (KeyTime)GetValue(KeyTimeProperty); }
            set { SetValue(KeyTimeProperty, value); }
        }
        /// <summary>
        /// Identifies the KeyTime dependency property.
        /// </summary>
        public static readonly DependencyProperty KeyTimeProperty =
            DependencyProperty.Register("KeyTime", typeof(KeyTime), typeof(ObjectKeyFrame), new PropertyMetadata(new KeyTime(new TimeSpan())));

        /// <summary>
        /// Gets or sets the key frame's target value.
        /// </summary>
        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        /// <summary>
        /// Identifies the Value dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(ObjectKeyFrame), new PropertyMetadata(null));
    }
}