

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


#if !BRIDGE
using JSIL.Meta;
#else
using Bridge;
#endif

using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSHTML5;
using DotNetForHtml5.Core;

#if MIGRATION
using System.Windows;
#else
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
            if (navigateToUri != null && target != null && targetFeatures!=null)
            {
                if (target != "_search")
                {
#if !CSHTML5NETSTANDARD
                    // The Simulator cannot open a URL in another window/tab:
                    if (target == "_blank" && CSHTML5.Interop.IsRunningInTheSimulator)
                    {
                        INTERNAL_Simulator.SimulatorProxy.NavigateToUrlInNewBrowserWindow(navigateToUri.ToString());
                    }
                    else
                    {
#endif
                        if (target == "")
                            target = "_self";

                        CSHTML5.Interop.ExecuteJavaScriptAsync(@"window.open($0, $1, $2)", navigateToUri.ToString(), target, targetFeatures);
#if !CSHTML5NETSTANDARD
                    }
#endif
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
#if !BRIDGE
        [JSReplacement("eval($code)")]
#else
        [Bridge.Script("eval(code)")]
#endif
        public object Eval(string code)
        {
            return CSHTML5.Interop.ExecuteJavaScript(string.Format("eval(\"{0}\")", code)); //Note: this probably doesn't work on multiline code 
        }

		[OpenSilver.NotImplemented]
        public string CurrentBookmark { get; set; }
    }
}
