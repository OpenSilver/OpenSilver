
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
#if !MIGRATION
using Windows.UI.Xaml;
#endif

namespace System.Windows.Browser
{
    public sealed partial class HtmlWindow : HtmlObject
    {
        public HtmlWindow()
        {

        }

        /// <summary>
        /// Opens the specified page in the specified browser instance, with the indicated user interface features.
        /// </summary>
        /// <param name="navigateToUri"></param>
        /// <param name="target"></param>
        /// <param name="targetFeatures"></param>
        public void Navigate(Uri navigateToUri, string target = "_self", string targetFeatures = "")
        {
            if (navigateToUri != null && target != null && targetFeatures != null)
            {
                if (target != "_search")
                {
                    if (target == "")
                    {
                        target = "_self";
                    }

                    string sUri = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(navigateToUri.ToString());
                    string sTarget = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(target);
                    string sTargetFeatures = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(targetFeatures);
                    OpenSilver.Interop.ExecuteJavaScriptFastAsync($"window.open({sUri}, {sTarget}, {sTargetFeatures})");
                }
                else
                {
                    throw new NotImplementedException("The search target is not implemented.");
                }
            }
            else
            {
                throw new ArgumentNullException();
            }

        }

        /// <summary>
        /// Evaluates a string that contains arbitrary JavaScript code.
        /// </summary>
        /// <param name="code">Javascript code.</param>
        /// <returns>The results of the JavaScript engine's evaluation of the string in the code parameter.</returns>
        public object Eval(string code)
        {
            return OpenSilver.Interop.ExecuteJavaScript("eval($0)", code);
        }

        [OpenSilver.NotImplemented]
        public string CurrentBookmark { get; set; }

        /// <summary>
        /// Displays a dialog box that contains an application-defined message.
        /// </summary>
        /// <param name="alertText">
        /// The text to display.
        /// </param>
        public void Alert(string alertText)
        {
            if (OpenSilver.Interop.IsRunningInTheSimulator)
            {
                MessageBox.Show(alertText);
            }
            else
            {
                OpenSilver.Interop.ExecuteJavaScriptVoid($"alert({CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(alertText)})");
            }
        }

        /// <summary>
        /// Displays a confirmation dialog box that contains an optional message as well
        /// as OK and Cancel buttons.
        /// </summary>
        /// <param name="confirmText">
        /// The text to display.
        /// </param>
        /// <returns>
        /// true if the user clicked the OK button; otherwise, false.
        /// </returns>
#if BRIDGE
        [Bridge.Template("confirm(confirmText)")]
#endif
        public bool Confirm(string confirmText)
        {
            if (OpenSilver.Interop.IsRunningInTheSimulator)
            {
                return MessageBox.Show(confirmText, MessageBoxButton.OKCancel) == MessageBoxResult.OK;
            }
            else
            {
                return OpenSilver.Interop.ExecuteJavaScriptBoolean($"confirm({CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(confirmText)})");
            }
        }

        /// <summary>
        /// Gets the value of a property that is identified by name on the current <see cref="HtmlWindow" />
        /// </summary>
        /// <param name="name">
        /// The name of the property.
        /// </param>
        /// <returns>
        /// <see langword="null" /> if the property does not exist.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="name"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="name"/> is an empty string or contains an embedded null character (\0).
        /// </exception>
#if BRIDGE
        [Bridge.Template("{name}")]
#endif
        public override object GetProperty(string name)
        {
            var result = default(object);

            if (name is null)
            {
                throw new ArgumentNullException(name);
            }
            else if (string.IsNullOrEmpty(name) || name.Contains("\\0"))
            {
                throw new ArgumentException($"{nameof(name)} is an empty string or contains an embedded null character (\0)");
            }

            var jsObject = OpenSilver.Interop.ExecuteJavaScript(name);

            if (!jsObject.ToString().Equals("undefined"))
            {
                result = jsObject;
            }

            return result;
        }
    }
}
