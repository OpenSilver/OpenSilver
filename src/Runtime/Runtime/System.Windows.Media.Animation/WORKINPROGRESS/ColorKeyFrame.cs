

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


using System;
using System.Windows;

#if MIGRATION
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
    /// <summary>Provides a base class for specific animation key-frame techniques that define an animation segment with a <see cref="T:System.Windows.Media.Color" /> target value. Derived classes each provide a different key-frame interpolation method for a <see cref="T:System.Windows.Media.Color" /> value that is provided for a <see cref="T:System.Windows.Media.Animation.ColorAnimationUsingKeyFrames" /> animation. </summary>
    [OpenSilver.NotImplemented]
    public abstract partial class ColorKeyFrame : DependencyObject, IKeyFrame
    {
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty KeyTimeProperty = DependencyProperty.Register("KeyTime", typeof(KeyTime), typeof(ColorKeyFrame), null);
        /// <summary>Gets or sets the time at which the key frame's target <see cref="P:System.Windows.Media.Animation.ColorKeyFrame.Value" /> should be reached. </summary>
        /// <returns>The time at which the key frame's current value should be equal to its <see cref="P:System.Windows.Media.Animation.ColorKeyFrame.Value" /> property. The default is null.</returns>
        [OpenSilver.NotImplemented]
        public KeyTime KeyTime
        {
            get { return (KeyTime)GetValue(KeyTimeProperty); }
            set { SetValue(KeyTimeProperty, value); }
        }

        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(Color), typeof(ColorKeyFrame), null);
        /// <summary>Gets or sets the key frame's target value. </summary>
        /// <returns>The key frame's target value, which is the value at its specified <see cref="P:System.Windows.Media.Animation.ColorKeyFrame.KeyTime" />. The default is a <see cref="T:System.Windows.Media.Color" /> with an ARGB value of #00000000.</returns>
        [OpenSilver.NotImplemented]
        public Color Value
        {
            get { return (Color)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        object IKeyFrame.Value
        {
            get { return this.Value; }
            set { this.Value = (Color)value; }
        }
    }
}
