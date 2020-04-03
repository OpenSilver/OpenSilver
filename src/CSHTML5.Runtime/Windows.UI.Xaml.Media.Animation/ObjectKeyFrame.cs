

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
    public partial class ObjectKeyFrame : DependencyObject, IKeyFrame
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
            DependencyProperty.Register("KeyTime", typeof(KeyTime), typeof(ObjectKeyFrame), new PropertyMetadata(new KeyTime(new TimeSpan()))
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });//, KeyTime_Changed));

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
            DependencyProperty.Register("Value", typeof(object), typeof(ObjectKeyFrame), new PropertyMetadata(null)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });//, Value_Changed));

        //private static void Value_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    //I don't think there is anything to do here
        //    //throw new NotImplementedException();
        //}


    }
}