
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
/// Provides methods to initialize the OpenSilver runtime on WebAssembly.
/// </summary>
public static partial class OpenSilverRuntime
{
    /// <summary>
    /// Initialize the OpenSilver runtime to run on WebAssembly.
    /// </summary>
    /// <returns>
    /// A task representing the asynchronous operation. The task result is <c>true</c> if the OpenSilver runtime was 
    /// initialized successfully (or already initialized); otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method must be called before running any OpenSilver applications to ensure the runtime is correctly set up.
    /// </remarks>
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
