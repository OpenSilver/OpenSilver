
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
    /// This class is used as part of a ColorKeyFrameCollection in
    /// conjunction with a KeyFrameColorAnimation to animate a
    /// Color property value along a set of key frames.
    ///
    /// This ColorKeyFrame interpolates between the Color Value of
    /// the previous key frame and its own Value to produce its output value.
    /// </summary>
    [OpenSilver.NotImplemented]
    public sealed class SplineColorKeyFrame : ColorKeyFrame
    {
        /// <summary>Identifies the <see cref="P:System.Windows.Media.Animation.SplineColorKeyFrame.KeySpline" /> dependency property.</summary>
        /// <returns>The identifier for the <see cref="P:System.Windows.Media.Animation.SplineColorKeyFrame.KeySpline" /> dependency property.</returns>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty KeySplineProperty = DependencyProperty.Register("KeySpline", typeof(KeySpline), typeof(SplineColorKeyFrame), new PropertyMetadata(new KeySpline()));

        /// <summary>
        /// Gets or sets the two control points that define animation progress for this key frame.
        /// </summary>
        /// <returns>
        /// The two control points that specify the cubic Bezier curve which defines the progress of the key frame.
        /// </returns>
        [OpenSilver.NotImplemented]
        public KeySpline KeySpline
        {
            get => (KeySpline)GetValue(KeySplineProperty);
            set => SetValue(KeySplineProperty, value);
        }
    }
}
