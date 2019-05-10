
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



using System;
#if MIGRATION
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

#if MIGRATION
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
    /// <summary>
    /// An abstract class that defines an animation segment with its own target value
    /// and interpolation method for a DoubleAnimationUsingKeyFrames.
    /// </summary>

#if WORKINPROGRESS
    public abstract class DoubleKeyFrame : Freezable, IKeyFrame
    {
        public static readonly DependencyProperty KeyTimeProperty = DependencyProperty.Register("KeyTime", typeof(KeyTime), typeof(DoubleKeyFrame), new PropertyMetadata(new TimeSpan()));
        /// <summary>Gets or sets the time at which the key frame's target <see cref="P:System.Windows.Media.Animation.DoubleKeyFrame.Value" /> should be reached.</summary>
        /// <returns>The time at which the key frame's current value should be equal to its <see cref="P:System.Windows.Media.Animation.DoubleKeyFrame.Value" /> property. The default is null.</returns>
        public KeyTime KeyTime
        {
            get { return (KeyTime)GetValue(KeyTimeProperty); }
            set { SetValue(KeyTimeProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(DoubleKeyFrame), new PropertyMetadata(null));
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
#endif
}
