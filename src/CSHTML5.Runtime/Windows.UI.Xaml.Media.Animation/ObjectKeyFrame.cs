
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
    /// <summary>
    /// Defines an animation segment with its own target value and interpolation
    /// method for an ObjectAnimationUsingKeyFrames.
    /// </summary>
    public class ObjectKeyFrame : DependencyObject
    {
        ///// <summary>
        ///// Provides base class initialization behavior for ObjectKeyFrame-derived classes.
        ///// </summary>
        //protected ObjectKeyFrame();


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
            DependencyProperty.Register("KeyTime", typeof(KeyTime), typeof(ObjectKeyFrame), new PropertyMetadata(new KeyTime(new TimeSpan())));//, KeyTime_Changed));

        //private static void KeyTime_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    //throw new NotImplementedException(); //I don't think there is anything to do here but we'll see on time.
        //}



        // Returns:
        //     The key frame's target value, which is the value of this key frame at its
        //     specified KeyTime. The default is null.
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
            DependencyProperty.Register("Value", typeof(object), typeof(ObjectKeyFrame), new PropertyMetadata(null));//, Value_Changed));

        //private static void Value_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    //I don't think there is anything to do here
        //    //throw new NotImplementedException();
        //}


    }
}