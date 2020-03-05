
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
    /// Provides the base class for all the easing functions.
    /// </summary>
#if WORKINPROGRESS
    public abstract partial class EasingFunctionBase : DependencyObject, IEasingFunction
#else
    public partial class EasingFunctionBase : DependencyObject
#endif
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