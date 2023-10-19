
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
    /// This class is used as part of a ByteKeyFrameCollection in
    /// conjunction with a KeyFrameByteAnimation to animate a
    /// Byte property value along a set of key frames.
    ///
    /// This ByteKeyFrame interpolates between the Byte Value of
    /// the previous key frame and its own Value to produce its output value.
    /// </summary>
    [OpenSilver.NotImplemented]
    public sealed class SplineDoubleKeyFrame : DoubleKeyFrame
    {
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty KeySplineProperty = DependencyProperty.Register("KeySpline", typeof(KeySpline), typeof(SplineDoubleKeyFrame), new PropertyMetadata(new KeySpline()));

        [OpenSilver.NotImplemented]
        public KeySpline KeySpline
        {
            get { return (KeySpline)GetValue(KeySplineProperty); }
            set { SetValue(KeySplineProperty, value); }
        }

        // todo: implement this. At the moment the animation is linear.
        internal override EasingFunctionBase INTERNAL_GetEasingFunction()
        {
            return base.INTERNAL_GetEasingFunction();
        }
    }
}
