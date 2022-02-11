

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
#if BRIDGE
using Bridge;
#endif
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

        internal static void ApplyValue(DependencyObject target, PropertyPath propertyPath, object value)
        {
            propertyPath.INTERNAL_PropertySetAnimationValue(target, value);
        }

        //Note: this method is needed because JSIL doesn't know that a nullable whose value is null is equal to null. (Color? v = null; if(v == null) ...)
        internal static bool IsValueNull(object from)
        {
            return from == null || CheckIfObjectIsNullNullable(from);
        }

        //Note: CheckIfObjectIsNullNullable and CheckIfNullableIsNotNull below come from DataContractSerializer_Helpers.cs
        internal static bool CheckIfObjectIsNullNullable(object obj)
        {
            Type type = obj.GetType();
            if (type.FullName.StartsWith("System.Nullable`1"))
            {
                //I guess we'll have to use reflection here
                return !CheckIfNullableIsNotNull(obj);
            }
            else
            {
                return false;
            }
        }

#if !BRIDGE
        [JSIL.Meta.JSReplacement("$obj.hasValue")]
#else
        [Template("{obj}.hasValue")]
#endif
        internal static bool CheckIfNullableIsNotNull(object obj)
        {
            return (obj != null);
        }
    }
}
