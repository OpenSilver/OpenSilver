
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

using System.Windows;

namespace OpenSilver.WebAssembly;

/// <summary>
/// Runner.
/// </summary>
public static class Runner
{
    /// <summary>
    /// RunApplicationAsync.
    /// </summary>
    public static async Task<T> RunApplicationAsync<T>(Func<T> createAppDelegate) where T : Application
    {
        ArgumentNullException.ThrowIfNull(createAppDelegate);

        if (await OpenSilverRuntime.StartAsync())
        {
            return createAppDelegate();
        }

        throw new InvalidOperationException("An unexpected error occurred. Please check the browser console for more details.");
    }

    /// <summary>
    /// RunApplicationAsync.
    /// </summary>
    public static Task<T> RunApplicationAsync<T>() where T : Application, new()
        => RunApplicationAsync(() => new T());
}
