
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

using System.Runtime.InteropServices.JavaScript;
using DotNetForHtml5;

namespace OpenSilver.WebAssembly;

/// <summary>
/// OpenSilverRuntime.
/// </summary>
public static partial class OpenSilverRuntime
{
    /// <summary>
    /// StartAsync.
    /// </summary>
    public static async Task<bool> StartAsync()
    {
        if (await StartNativeAsync())
        {
            Cshtml5Initializer.Initialize(NativeMethods.Instance);
            return true;
        }
        return false;
    }

    [JSImport("globalThis._openSilverRuntime.startAsync")]
    private static partial Task<bool> StartNativeAsync();
}
