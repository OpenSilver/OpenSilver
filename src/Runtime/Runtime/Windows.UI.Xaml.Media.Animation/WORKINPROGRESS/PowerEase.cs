
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
    ///     This class implements an easing function that gives a polynomial curve of arbitrary degree.
    ///     If the curve you desire is cubic, quadratic, quartic, or quintic it is better to use the 
    ///     specialized easing functions.
    /// </summary>
    public class PowerEase : EasingFunctionBase
    {
        /// <summary>
        /// Power Property
        /// </summary>
        public static readonly DependencyProperty PowerProperty =
            DependencyProperty.Register(
                    "Power",
                    typeof(double),
                    typeof(PowerEase),
                    new PropertyMetadata(2.0));

        /// <summary>
        /// Specifies the power for the polynomial equation.
        /// </summary>
        public double Power
        {
            get => (double)GetValue(PowerProperty);
            set => SetValue(PowerProperty, value);
        }

        protected override double EaseInCore(double normalizedTime)
        {
            double power = Math.Max(0.0, Power);
            return Math.Pow(normalizedTime, power);
        }
    }
}
