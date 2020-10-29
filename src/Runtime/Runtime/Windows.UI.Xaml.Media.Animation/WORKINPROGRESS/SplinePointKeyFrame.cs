﻿

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

#if MIGRATION
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
#if WORKINPROGRESS
    /// <summary>
    /// This class is used as part of a PointKeyFrameCollection in conjunction
    /// with a PointAnimationUsingKeyFrames to animate a Point property value
    /// along a set of key frames.
    ///
    /// </summary>
    public sealed partial class SplinePointKeyFrame : PointKeyFrame
    {
        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Media.Animation.SplinePointKeyFrame.KeySpline" /> dependency property.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="P:System.Windows.Media.Animation.SplinePointKeyFrame.KeySpline" /> dependency property.
        /// </returns>
        public static readonly DependencyProperty KeySplineProperty = DependencyProperty.Register("KeySpline", typeof(KeySpline), typeof(SplinePointKeyFrame), new PropertyMetadata(new KeySpline()));


        /// <summary>
        /// Gets or sets the two control points that define animation progress for this key frame.
        /// </summary>
        /// <returns>
        /// The two control points that specify the cubic Bezier curve that defines the progress of the key frame.
        /// </returns>
        public KeySpline KeySpline
        {
            get => (KeySpline)GetValue(KeySplineProperty);
            set => SetValue(KeySplineProperty, value);
        }
    }
#endif
}

