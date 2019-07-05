
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
#if !MIGRATION
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;
#endif

#if MIGRATION
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
    internal static class AnimationHelpers
    {
        internal static void CallVelocity(object domElement, Duration Duration, EasingFunctionBase easingFunction, string visualStateGroupName, Action callbackForWhenfinished, object jsFromToValues)
        {
            string easingFunctionAsString = "linear";
            if (easingFunction != null)
            {
                easingFunctionAsString = easingFunction.GetFunctionAsString();
            }

            double duration = Duration.TimeSpan.TotalMilliseconds;
            if (duration == 0)
            {
                ++duration;
            }

            object options = CSHTML5.Interop.ExecuteJavaScriptAsync(@"new Object()");
            if (callbackForWhenfinished == null)
            {
                CSHTML5.Interop.ExecuteJavaScriptAsync(@"
$0.easing = $1;
$0.duration = $2;
$0.queue = false;
$0.queue = $3;
", options, easingFunctionAsString, duration, visualStateGroupName);
            }
            else
            {
                CSHTML5.Interop.ExecuteJavaScriptAsync(@"
$0.easing = $1;
$0.duration = $2;
$0.queue = false;
$0.queue = $3;
$0.complete = $4;
", options, easingFunctionAsString, duration, visualStateGroupName, callbackForWhenfinished);
            }

            if (easingFunction != null)
            {
                Dictionary<string, object> additionalOptions = easingFunction.GetAdditionalOptionsForVelocityCall();
                if (additionalOptions != null)
                {
                    foreach (string key in additionalOptions.Keys)
                    {
                        CSHTML5.Interop.ExecuteJavaScriptAsync(@"$0[$1] = $2;", options, key, additionalOptions[key]);
                    }
                }
            }

            CSHTML5.Interop.ExecuteJavaScriptAsync(@"Velocity($0, $1, $2);
                                                     Velocity.Utilities.dequeue($0, $3);", 
                                                     domElement, jsFromToValues, options, visualStateGroupName);
        }

        internal static void ApplyInstantAnimation(DependencyObject target, PropertyPath propertyPath, double to, bool isVisualStateChange)
        {
            if (isVisualStateChange)
            {
                propertyPath.INTERNAL_PropertySetVisualState(target, to);
            }
            else
            {
                propertyPath.INTERNAL_PropertySetLocalValue(target, to);
            }
        }
    }
}
