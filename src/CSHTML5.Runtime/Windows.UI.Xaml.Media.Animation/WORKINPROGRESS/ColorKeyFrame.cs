
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
//
//===============================================================================


using System;
using System.Windows;

#if MIGRATION
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
#if WORKINPROGRESS
    /// <summary>Provides a base class for specific animation key-frame techniques that define an animation segment with a <see cref="T:System.Windows.Media.Color" /> target value. Derived classes each provide a different key-frame interpolation method for a <see cref="T:System.Windows.Media.Color" /> value that is provided for a <see cref="T:System.Windows.Media.Animation.ColorAnimationUsingKeyFrames" /> animation. </summary>
    public abstract class ColorKeyFrame : DependencyObject
    {
        public static readonly DependencyProperty KeyTimeProperty = DependencyProperty.Register("KeyTime", typeof(KeyTime), typeof(ColorKeyFrame), null);
        /// <summary>Gets or sets the time at which the key frame's target <see cref="P:System.Windows.Media.Animation.ColorKeyFrame.Value" /> should be reached. </summary>
        /// <returns>The time at which the key frame's current value should be equal to its <see cref="P:System.Windows.Media.Animation.ColorKeyFrame.Value" /> property. The default is null.</returns>
        public KeyTime KeyTime
        {
            get { return (KeyTime)GetValue(KeyTimeProperty); }
            set { SetValue(KeyTimeProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(Color), typeof(ColorKeyFrame), null);
        /// <summary>Gets or sets the key frame's target value. </summary>
        /// <returns>The key frame's target value, which is the value at its specified <see cref="P:System.Windows.Media.Animation.ColorKeyFrame.KeyTime" />. The default is a <see cref="T:System.Windows.Media.Color" /> with an ARGB value of #00000000.</returns>
        public Color Value
        {
            get { return (Color)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
    }
#endif
}
