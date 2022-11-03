

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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if BRIDGE
using Bridge;
#endif
#if !MIGRATION
using Windows.UI.Xaml;
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
            string sOptions = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(options);
            if (callbackForWhenfinished == null)
            {
                CSHTML5.Interop.ExecuteJavaScriptFastAsync($@"
{sOptions}.easing = ""{CSHTML5.Internal.INTERNAL_HtmlDomManager.EscapeStringForUseInJavaScript(easingFunctionAsString)}"";
{sOptions}.duration = {duration.ToString(CultureInfo.InvariantCulture)};
{sOptions}.queue = false;
{sOptions}.queue = ""{visualStateGroupName}"";
");
            }
            else
            {
                string sAction = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(callbackForWhenfinished);
                CSHTML5.Interop.ExecuteJavaScriptFastAsync($@"
{sOptions}.easing = ""{CSHTML5.Internal.INTERNAL_HtmlDomManager.EscapeStringForUseInJavaScript(easingFunctionAsString)}"";
{sOptions}.duration = {duration.ToString(CultureInfo.InvariantCulture)};
{sOptions}.queue = false;
{sOptions}.queue = ""{visualStateGroupName}"";
{sOptions}.complete = {sAction};
");
            }

            if (easingFunction != null)
            {
                Dictionary<string, object> additionalOptions = easingFunction.GetAdditionalOptionsForVelocityCall();
                if (additionalOptions != null)
                {
                    foreach (string key in additionalOptions.Keys)
                    {
                        string sAdditionalOptions = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(additionalOptions[key]);
                        CSHTML5.Interop.ExecuteJavaScriptFastAsync($@"{sOptions}[""{CSHTML5.Internal.INTERNAL_HtmlDomManager.EscapeStringForUseInJavaScript(key)}""] = {sAdditionalOptions};");
                    }
                }
            }

            string sElement = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(domElement);
            string sValues = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(jsFromToValues);
            CSHTML5.Interop.ExecuteJavaScriptFastAsync($@"Velocity({sElement}, {sValues}, {sOptions});
                                                     Velocity.Utilities.dequeue({sElement}, ""{visualStateGroupName}"");");
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
