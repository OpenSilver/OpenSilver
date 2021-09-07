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

#if MIGRATION
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
    /// <summary>
    /// A class that enables you to associate easing functions with a 
    /// <see cref="ColorAnimationUsingKeyFrames"/> key frame animation.
    /// </summary>
    [OpenSilver.NotImplemented]
    public sealed class EasingColorKeyFrame : ColorKeyFrame
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EasingColorKeyFrame"/> class.
        /// </summary>
        [OpenSilver.NotImplemented]
        public EasingColorKeyFrame()
        {
        }

        /// <summary>
        /// Identifies the <see cref="EasingColorKeyFrame.EasingFunction"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty EasingFunctionProperty =
            DependencyProperty.Register(
                "EasingFunction",
                typeof(IEasingFunction),
                typeof(EasingColorKeyFrame),
                new PropertyMetadata((object)null));

        /// <summary>
        /// Gets or sets the easing function that is applied to the key frame.
        /// </summary>
        [OpenSilver.NotImplemented]
        public IEasingFunction EasingFunction
        {
            get { return (IEasingFunction)this.GetValue(EasingFunctionProperty); }
            set { this.SetValue(EasingFunctionProperty, value); }
        }
    }
}
