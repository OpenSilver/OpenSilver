
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
using OpenSilver.Internal;

namespace DotNetForHtml5;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class Cshtml5Initializer
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Initialize(INativeMethods nativeMethods) => Initialize((IJavaScriptExecutionHandler)nativeMethods);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete(Helper.ObsoleteMemberMessage + " Use Cshtml5Initializer.Initialize(IWebAssemblyExecutionHandler2) instead.", true)]
    public static void Initialize(IWebAssemblyExecutionHandler executionHandler)
    {
        Initialize((IJavaScriptExecutionHandler)executionHandler);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Initialize(IJavaScriptExecutionHandler executionHandler)
    {
        INTERNAL_Simulator.JavaScriptExecutionHandler = executionHandler;
    }
}
