
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
    /// Represents an easing function that creates an animation that accelerates
    /// and/or decelerates using an exponential formula.
    /// </summary>
    public sealed class ExponentialEase : EasingFunctionBase
    {
        const string FUNCTION_TYPE_STRING = "ExponentialCustomEasing";
        private static bool velocityInitializationMade = false;

        //// Summary:
        ////     Initializes a new instance of the ExponentialEase class.
        //public ExponentialEase();

        /// <summary>
        /// Gets or sets the exponent used to determine the interpolation of the animation.
        /// </summary>
        public double Exponent
        {
            get { return (double)GetValue(ExponentProperty); }
            set { SetValue(ExponentProperty, value); }
        }
        /// <summary>
        /// Identifies the Exponent dependency property.
        /// </summary>
        public static readonly DependencyProperty ExponentProperty =
            DependencyProperty.Register("Exponent", typeof(double), typeof(ExponentialEase), new PropertyMetadata(2d));


//        Velocity.Easings.myCustomEasing = function (p, opts, tweenDelta) {
//    return 0.5 - Math.cos( p * Math.PI ) / 2;
//};

        internal override Dictionary<string, object> GetAdditionalOptionsForVelocityCall()
        {
            Dictionary<string, object> additionalOptions = new Dictionary<string, object>();
            additionalOptions.Add("exponent", Exponent);
            return additionalOptions;
        }

        internal override string GetFunctionAsString()
        {
            if (!velocityInitializationMade)
            {
                //defining the easeIn EasingMode:

                //note: below,  p is the advancement of the animation in percentage (decimal)
                //              opts (optional) are the options (such as "queue")
                //              tweenDelta (optional) is the delta between the starting value and the end value
                CSHTML5.Interop.ExecuteJavaScript(@"
        Velocity.Easings.easeInExponentialCustomEasing = function (p, opts, tweenDelta) {
    var exponent = 2;
    if(opts != undefined) {
        if(opts.exponent != undefined) {
            exponent = opts.exponent;
        }
    }
    return (Math.exp(opts.exponent * p)- 1) / (Math.exp(opts.exponent) - 1);
};");

                //defining the easeOut EasingMode:

                //note: below,  p is the advancement of the animation in percentage (decimal)
                //              opts (optional) are the options (such as "queue")
                //              tweenDelta (optional) is the delta between the starting value and the end value
                CSHTML5.Interop.ExecuteJavaScript(@"
        Velocity.Easings.easeOutExponentialCustomEasing = function (p, opts, tweenDelta) {
    var exponent = 2;
    if(opts != undefined) {
        if(opts.exponent != undefined) {
            exponent = opts.exponent;
        }
    }
    return 1 - ((Math.exp(opts.exponent * (1-p))- 1) / (Math.exp(opts.exponent) - 1));
};");

                //defining the easeInOut EasingMode:

                //note: below,  p is the advancement of the animation in percentage (decimal)
                //              opts (optional) are the options (such as "queue")
                //              tweenDelta (optional) is the delta between the starting value and the end value
                CSHTML5.Interop.ExecuteJavaScript(@"
        Velocity.Easings.easeInOutExponentialCustomEasing = function (p, opts, tweenDelta) {
    var exponent = 2;
    if(opts != undefined) {
        if(opts.exponent != undefined) {
            exponent = opts.exponent;
        }
    }
    if(p < 0.5) {
        return (Math.exp(opts.exponent * 2 * p)- 1) / (2 * (Math.exp(opts.exponent) - 1));
    }
    else {
        return 1 - ((Math.exp(2 * opts.exponent * (1 - p)) - 1) / (2 * (Math.exp(opts.exponent) - 1)));
    }
};");
        //return 1 - ((Math.exp(opts.exponent * (1 - (2 * (p - 0,5))))- 1) / (2 * (Math.exp(opts.exponent) - 1)));

                velocityInitializationMade = true;
            }
            return GetEasingModeAsString() + FUNCTION_TYPE_STRING;
        }
    }
}