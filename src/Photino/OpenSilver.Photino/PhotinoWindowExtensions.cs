
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

using OpenSilver.Photino.Runner;
using Photino.NET;

namespace OpenSilver.Photino
{
    /// <summary>
    /// Provides extension methods to configure and run an OpenSilver application
    /// within a Photino window.
    /// </summary>
    public static class PhotinoWindowExtensions
    {
        /// <summary>
        /// Configures the Photino window to run an OpenSilver application,
        /// instantiated using the default constructor.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the OpenSilver application, derived from <see cref="System.Windows.Application"/>,
        /// with a parameterless constructor.
        /// </typeparam>
        /// <param name="window">The Photino window instance to configure.</param>
        /// <returns>The configured Photino window instance.</returns>
        public static PhotinoWindow ConfigureOpenSilver<T>(this PhotinoWindow window)
            where T : System.Windows.Application, new()
        {
            ConfigureOpenSilver(window, () => new T());
            return window;
        }

        /// <summary>
        /// Configures the Photino window to run an OpenSilver application,
        /// instantiated using a specified delegate.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the OpenSilver application, derived from <see cref="System.Windows.Application"/>.
        /// </typeparam>
        /// <param name="window">The Photino window instance to configure.</param>
        /// <param name="createAppDelegate">
        /// A delegate that creates and returns an instance of the OpenSilver application.
        /// </param>
        /// <returns>The configured Photino window instance.</returns>
        public static PhotinoWindow ConfigureOpenSilver<T>(this PhotinoWindow window, Func<T> createAppDelegate)
            where T : System.Windows.Application
        {
            ConfigureOpenSilver(window, () => Task.FromResult(createAppDelegate()));
            return window;
        }

        /// <summary>
        /// Configures the Photino window to run an OpenSilver application,
        /// instantiated using an asynchronous delegate.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the OpenSilver application, derived from <see cref="System.Windows.Application"/>.
        /// </typeparam>
        /// <param name="window">The Photino window instance to configure.</param>
        /// <param name="createAppDelegate">
        /// An asynchronous delegate that creates and returns an instance of the OpenSilver application.
        /// </param>
        /// <returns>The configured Photino window instance.</returns>
        public static PhotinoWindow ConfigureOpenSilver<T>(this PhotinoWindow window, Func<Task<T>> createAppDelegate)
            where T : System.Windows.Application
        {
            new PhotinoRunner(window).RunApplicationAsync(createAppDelegate);
            return window;
        }
    }
}
