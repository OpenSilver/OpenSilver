
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
using DotNetForHtml5.Core;

namespace DotNetForHtml5
{
    public static class Cshtml5Initializer
    {
        public static int PendingJsBufferSize { get; set; } = 1024 * 1024 * 2; // 2 MB

        public static void Initialize(IWebAssemblyExecutionHandler executionHandler)
        {
            INTERNAL_Simulator.WebAssemblyExecutionHandler = executionHandler;
#if MIGRATION
            EmulatorWithoutJavascript.StaticConstructorsCaller.EnsureStaticConstructorOfCommonTypesIsCalled(typeof(System.Windows.Controls.Button).Assembly);
#else
            EmulatorWithoutJavascript.StaticConstructorsCaller.EnsureStaticConstructorOfCommonTypesIsCalled(typeof(Windows.UI.Xaml.Controls.Button).Assembly);
#endif
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Initialize()
        {
            Initialize(new JavaScriptExecutionHandler());
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Initialize(IJavaScriptExecutionHandler executionHandler)
        {
            IWebAssemblyExecutionHandler jsRuntime = executionHandler as IWebAssemblyExecutionHandler
                ?? new JSRuntimeWrapper(executionHandler);

            Initialize(jsRuntime);
        }
    }
}
