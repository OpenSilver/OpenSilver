
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
using System.ComponentModel;
using System.Windows.Browser.Internal;
using OpenSilver.Internal;

#if !MIGRATION
using Windows.UI.Xaml;
#endif

namespace System.Windows.Browser
{
    public sealed class HtmlWindow : HtmlObject
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(Helper.ObsoleteMemberMessage + " Use System.Windows.Browser.HtmlPage.Window instead.")]
        public HtmlWindow()
            : this(new WindowRef())
        {
        }

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
            return Invoke("eval", code);
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
    }
}
