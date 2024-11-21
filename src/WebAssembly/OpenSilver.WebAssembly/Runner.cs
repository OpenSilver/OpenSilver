
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
/// Provides methods for starting an OpenSilver application on WebAssembly.
/// </summary>
public static class Runner
{
    /// <summary>
    /// Starts an OpenSilver application with a specified application factory method.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the application, derived from <see cref="Application"/>.
    /// </typeparam>
    /// <param name="createAppDelegate">
    /// A delegate that creates an instance of the application.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result contains the application instance created by the
    /// <paramref name="createAppDelegate"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="createAppDelegate"/> is null.
    /// </exception>
    /// <remarks>
    /// This method automatically initializes the OpenSilver runtime by calling <see cref="OpenSilverRuntime.StartAsync"/>.
    /// Use this overload when the application creation process is synchronous.
    /// </remarks>
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
    /// Starts an OpenSilver application with an asynchronous application factory method.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the application, derived from <see cref="Application"/>.
    /// </typeparam>
    /// <param name="createAppDelegate">
    /// A delegate that asynchronously creates an instance of the application. 
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result contains the application instance created by the 
    /// <paramref name="createAppDelegate"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// createAppDelegate is null.
    /// </exception>
    /// <remarks>
    /// This method automatically initializes the OpenSilver runtime by calling <see cref="OpenSilverRuntime.StartAsync"/>.
    /// Use this overload when the application creation process involves asynchronous operations.
    /// </remarks>
    public static async Task<T> RunApplicationAsync<T>(Func<Task<T>> createAppDelegate) where T : Application
    {
        ArgumentNullException.ThrowIfNull(createAppDelegate);

        if (await OpenSilverRuntime.StartAsync())
        {
            return await createAppDelegate();
        }

        throw new InvalidOperationException("An unexpected error occurred. Please check the browser console for more details.");
    }

    /// <summary>
    /// Starts an OpenSilver application by creating an instance of the specified application type.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the application, derived from <see cref="Application"/>, with a parameterless constructor.
    /// </typeparam>
    /// <returns>
    /// A task representing the asynchronous operation. The task result contains the application instance created.
    /// </returns>
    /// <remarks>
    /// This method automatically initializes the OpenSilver runtime by calling <see cref="OpenSilverRuntime.StartAsync"/>.
    /// Use this method when no additional setup is required for the application.
    /// </remarks>
    public static Task<T> RunApplicationAsync<T>() where T : Application, new()
        => RunApplicationAsync(() => new T());
}
