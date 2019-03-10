
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
//
//===============================================================================



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
#if WORKINPROGRESS
    public sealed class HtmlWindow : HtmlObject
#else
    public sealed class HtmlWindow
#endif
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
                    if (target == "_blank" && Interop.IsRunningInTheSimulator)
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
        [Template("eval({code})")]
#endif
        public object Eval(string code)
        {
            return Interop.ExecuteJavaScript(string.Format("eval(\"{0}\")", code)); //Note: this probably doesn't work on multiline code 
        }
    }
}
