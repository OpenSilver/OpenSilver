
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

using System.Collections.Generic;

namespace System.Windows.Media.Animation
{
    /// <summary>
    /// Provides the base class for all the easing functions.
    /// </summary>
    public abstract partial class EasingFunctionBase : DependencyObject, IEasingFunction
    {
        /// <summary>
        /// Gets or sets a value that specifies how the animation interpolates.
        /// </summary>
        public EasingMode EasingMode
        {
            get { return (EasingMode)GetValue(EasingModeProperty); }
            set { SetValue(EasingModeProperty, value); }
        }
        /// <summary>
        /// Identifies the EasingMode dependency property.
        /// </summary>
        public static readonly DependencyProperty EasingModeProperty =
            DependencyProperty.Register("EasingMode", typeof(EasingMode), typeof(EasingFunctionBase), new PropertyMetadata(EasingMode.EaseOut));

        ///// <summary>
        ///// Transforms normalized time to control the pace of an animation.
        ///// </summary>
        ///// <param name="normalizedTime">Normalized time (progress) of the animation.</param>
        ///// <returns>A double that represents the transformed progress.</returns>
        //public double Ease(double normalizedTime);

        internal virtual string GetFunctionAsString()
        {
            return "linear";
        }

        internal protected string GetEasingModeAsString()
        {
            switch (EasingMode)
            {
                case EasingMode.EaseOut:
                    return "easeOut";
                case EasingMode.EaseIn:
                    return "easeIn";
                case EasingMode.EaseInOut:
                    return "easeInOut";
                default:
                    return "easeOut";
            }
        }

        /// <summary>
        /// Returns a Dictionary of the values to add in the options section of the Velocity Call. Implemented in inheriting classes that define their own easing function (see ExponentialEase)
        /// </summary>
        /// <returns></returns>
        internal virtual Dictionary<string, object> GetAdditionalOptionsForVelocityCall() { return null; }
    }
}