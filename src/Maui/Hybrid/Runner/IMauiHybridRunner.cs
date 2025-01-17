
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

namespace OpenSilver.MauiHybrid.Runner
{
    /// <summary>
    /// Provides methods for starting an OpenSilver application on MAUI Hybrid.
    /// </summary>
    public interface IMauiHybridRunner
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
        /// Use this overload when the application creation process is synchronous.
        /// </remarks>
        Task<T> RunApplicationAsync<T>(Func<T> createAppDelegate) where T : System.Windows.Application;

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
        /// Use this overload when the application creation process involves asynchronous operations.
        /// </remarks>
        Task<T> RunApplicationAsync<T>(Func<Task<T>> createAppDelegate) where T : System.Windows.Application;

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
        /// Use this method when no additional setup is required for the application.
        /// </remarks>
        Task<T> RunApplicationAsync<T>() where T : System.Windows.Application, new();
    }
}
