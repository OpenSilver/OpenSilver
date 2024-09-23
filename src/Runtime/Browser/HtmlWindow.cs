
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

using System.Windows.Browser.Internal;

namespace System.Windows.Browser
{
    /// <summary>
    /// Provides the managed representation of the JavaScript window object.
    /// </summary>
    public sealed class HtmlWindow : HtmlObject
    {
        internal HtmlWindow(IJSObjectRef jsObject)
            : base(jsObject)
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

                    string sUri = OpenSilver.Interop.GetVariableStringForJS(navigateToUri.ToString());
                    string sTarget = OpenSilver.Interop.GetVariableStringForJS(target);
                    string sTargetFeatures = OpenSilver.Interop.GetVariableStringForJS(targetFeatures);
                    OpenSilver.Interop.ExecuteJavaScriptVoidAsync($"window.open({sUri}, {sTarget}, {sTargetFeatures})");
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
            return Invoke("eval", code);
        }

        /// <summary>
        /// Gets or sets a string that represents the hash value of the current page's URL.
        /// </summary>
        /// <returns>
        /// The hash value of the current page's URL. If the URL has no hash value, this
        /// property returns an empty string.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// The return value is null.
        /// </exception>
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
                OpenSilver.Interop.ExecuteJavaScriptVoid($"alert({OpenSilver.Interop.GetVariableStringForJS(alertText)})");
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
        public bool Confirm(string confirmText)
        {
            if (OpenSilver.Interop.IsRunningInTheSimulator)
            {
                return MessageBox.Show(confirmText, MessageBoxButton.OKCancel) == MessageBoxResult.OK;
            }
            else
            {
                return OpenSilver.Interop.ExecuteJavaScriptBoolean($"confirm({OpenSilver.Interop.GetVariableStringForJS(confirmText)})");
            }
        }
    }
}
