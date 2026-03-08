extern alias opensilver;

using DotNetForHtml5.EmulatorWithoutJavascript;
using System.ComponentModel;
using System.Reflection;

namespace OpenSilver.Simulator
{
    /// <summary>
    /// Provides methods for starting an OpenSilver application on in the Simulator.
    /// </summary>
    public static class SimulatorLauncher
    {
        /// <summary>
        /// Starts an OpenSilver application by creating an instance of the specified type.
        /// </summary>
        /// <param name="userApplicationType">
        /// The type of the application.
        /// </param>
        /// <param name="parameters">
        /// Options to configure the application.
        /// </param>
        /// <returns>
        /// The <see cref="int"/> application exit code that is returned to the operating system when the application 
        /// shuts down. By default, the exit code value is 0.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="userApplicationType"/> does not derive from <see cref="opensilver.System.Windows.Application"/>.
        /// </exception>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use SimulatorLauncher.Start<T>(SimulatorLaunchParameters) instead.")]
        public static int Start(Type userApplicationType, SimulatorLaunchParameters parameters = null)
        {
            ArgumentNullException.ThrowIfNull(userApplicationType);

            if (!typeof(opensilver::System.Windows.Application).IsAssignableFrom(userApplicationType))
            {
                throw new ArgumentException(
                    $"'{nameof(userApplicationType)}' must derive from '{typeof(opensilver::System.Windows.Application).FullName}'.",
                    nameof(userApplicationType));
            }

            return StartImpl(
                () => Task.FromResult((opensilver::System.Windows.Application)Activator.CreateInstance(userApplicationType)),
                userApplicationType.Assembly,
                parameters);
        }

        /// <summary>
        /// Starts an OpenSilver application with a specified application factory method.
        /// </summary>
        /// <param name="appCreationDelegate">
        /// A delegate that creates an instance of the application.
        /// </param>
        /// <param name="appAssembly">
        /// The assembly is which the application type is defined.
        /// </param>
        /// <param name="parameters">
        /// Options to configure the application.
        /// </param>
        /// <returns>
        /// The <see cref="int"/> application exit code that is returned to the operating system when the application 
        /// shuts down. By default, the exit code value is 0.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="appCreationDelegate"/> is null or <paramref name="appAssembly"/> is null.
        /// </exception>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use SimulatorLauncher.Start<T>(Func<T>, SimulatorLaunchParameters) instead.")]
        public static int Start(Action appCreationDelegate, Assembly appAssembly, SimulatorLaunchParameters parameters = null)
        {
            ArgumentNullException.ThrowIfNull(appCreationDelegate);
            ArgumentNullException.ThrowIfNull(appAssembly);

            return StartImpl(
                () =>
                {
                    appCreationDelegate();
                    return Task.FromResult(opensilver::System.Windows.Application.Current);
                },
                appAssembly,
                parameters);
        }

        /// <summary>
        /// Starts an OpenSilver application by creating an instance of the specified application type.
        /// </summary>
        /// <typeparam name="TApplication">
        /// The type of the application, derived from <see cref="opensilver::System.Windows.Application"/>.
        /// </typeparam>
        /// <param name="parameters">
        /// Options to configure the application.
        /// </param>
        /// <returns>
        /// The <see cref="int"/> application exit code that is returned to the operating system when the application 
        /// shuts down. By default, the exit code value is 0.
        /// </returns>
        public static int Start<TApplication>(SimulatorLaunchParameters parameters = null)
            where TApplication : opensilver::System.Windows.Application, new()
        {
            return Start(() => Task.FromResult(new TApplication()), parameters);
        }

        /// <summary>
        /// Starts an OpenSilver application with a specified application factory method.
        /// </summary>
        /// <typeparam name="TApplication">
        /// The type of the application, derived from <see cref="opensilver::System.Windows.Application"/>.
        /// </typeparam>
        /// <param name="appCreationDelegate">
        /// A delegate that creates an instance of the application.
        /// </param>
        /// <param name="parameters">
        /// Options to configure the application.
        /// </param>
        /// <returns>
        /// The <see cref="int"/> application exit code that is returned to the operating system when the application 
        /// shuts down. By default, the exit code value is 0.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="appCreationDelegate"/> is null.
        /// </exception>
        public static int Start<TApplication>(Func<TApplication> appCreationDelegate, SimulatorLaunchParameters parameters = null)
            where TApplication : opensilver::System.Windows.Application
        {
            ArgumentNullException.ThrowIfNull(appCreationDelegate);

            return Start(() => Task.FromResult(appCreationDelegate()), parameters);
        }

        /// <summary>
        /// Starts an OpenSilver application with an asynchronous application factory method.
        /// </summary>
        /// <typeparam name="TApplication">
        /// The type of the application, derived from <see cref="opensilver::System.Windows.Application"/>.
        /// </typeparam>
        /// <param name="appCreationDelegate">
        /// A delegate that asynchronously creates an instance of the application.
        /// </param>
        /// <param name="parameters">
        /// Options to configure the application.
        /// </param>
        /// <returns>
        /// The <see cref="int"/> application exit code that is returned to the operating system when the application 
        /// shuts down. By default, the exit code value is 0.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="appCreationDelegate"/> is null.
        /// </exception>
        public static int Start<TApplication>(Func<Task<TApplication>> appCreationDelegate, SimulatorLaunchParameters parameters = null)
            where TApplication : opensilver::System.Windows.Application
        {
            ArgumentNullException.ThrowIfNull(appCreationDelegate);

            return StartImpl(appCreationDelegate, typeof(TApplication).Assembly, parameters);
        }

        private static int StartImpl<TApplication>(
            Func<Task<TApplication>> appCreationDelegate,
            Assembly appAssembly,
            SimulatorLaunchParameters parameters = null)
            where TApplication : opensilver::System.Windows.Application
        {
            var app = new App();
            app.InitializeComponent();
            return app.Run(new MainWindow(async () => await appCreationDelegate(), appAssembly, parameters ?? new SimulatorLaunchParameters()));
        }
    }
}

