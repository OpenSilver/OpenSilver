

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
using CSHTML5.Internal;
using OpenSilver.Internal;
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
        internal static void CallVelocity(
            AnimationTimeline animation,
            object domElement,
            Duration Duration,
            EasingFunctionBase easingFunction,
            string visualStateGroupName,
            Action callbackForWhenfinished,
            string jsFromToValues)
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

            string sElement = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(domElement);
            string elementId = (domElement as INTERNAL_HtmlDomElementReference).UniqueIdentifier;

            StringBuilder sb = StringBuilderFactory.Get();
            sb.AppendLine("(function(el) {")
              .AppendLine($@"const options = {{
easing:""{INTERNAL_HtmlDomManager.EscapeStringForUseInJavaScript(easingFunctionAsString)}"",
duration:{duration.ToInvariantString()},
queue:false,
queue:""{visualStateGroupName}""
}};");

            if (callbackForWhenfinished != null)
            {
                var jsCallback = JavaScriptCallbackHelper.CreateSelfDisposedJavaScriptCallback(
                    callbackForWhenfinished,
                    !OpenSilver.Interop.IsRunningInTheSimulator);
                sb.Append($"options.complete = {CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(jsCallback)};");
                animation.RegisterCallback(jsCallback);
            }

            if (easingFunction != null)
            {
                Dictionary<string, object> additionalOptions = easingFunction.GetAdditionalOptionsForVelocityCall();
                if (additionalOptions != null)
                {
                    foreach (string key in additionalOptions.Keys)
                    {
                        string sAdditionalOptions = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(additionalOptions[key]);
                        sb.Append($@"options.{INTERNAL_HtmlDomManager.EscapeStringForUseInJavaScript(key)} = {sAdditionalOptions};");
                    }
                }
            }

            sb.AppendLine($"document.velocityHelpers.animate(el, {jsFromToValues}, options, '{visualStateGroupName}');")
              .Append($"}})({sElement});");

            string javascript = sb.ToString();
            StringBuilderFactory.Return(sb);

            OpenSilver.Interop.ExecuteJavaScriptFastAsync(javascript);
        }

        internal static void StopVelocity(string domElement, string visualStateGroupName)
        {
            OpenSilver.Interop.ExecuteJavaScriptFastAsync($@"Velocity({domElement}, ""stop"", ""{visualStateGroupName}"");");
        }

        internal static void ApplyValue(DependencyObject target, PropertyPath propertyPath, object value)
        {
            propertyPath.INTERNAL_PropertySetAnimationValue(target, value);
        }
    }
}
