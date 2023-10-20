using DotNetForHtml5.EmulatorWithoutJavascript;
using System.Reflection;

namespace OpenSilver.Simulator
{
    public static class SimulatorLauncher
    {
        internal static SimulatorLaunchParameters Parameters { get; private set; }

        public static int Start(Type userApplicationType, SimulatorLaunchParameters parameters = null)
        {
            if (userApplicationType == null)
            {
                throw new ArgumentNullException(nameof(userApplicationType));
            }

            Parameters = parameters ?? new SimulatorLaunchParameters();

            return Start(() => Activator.CreateInstance(userApplicationType), userApplicationType.Assembly, parameters);
        }

        public static int Start(Action appCreationDelegate, Assembly appAssembly, SimulatorLaunchParameters parameters = null)
        {
            Parameters = parameters ?? new SimulatorLaunchParameters();
            if (appCreationDelegate == null)
            {
                throw new ArgumentNullException(nameof(appCreationDelegate));
            }

            if (appAssembly == null)
            {
                throw new ArgumentNullException(nameof(appAssembly));
            }

            App app = new App();
            app.InitializeComponent();
            return app.Run(new MainWindow(appCreationDelegate, appAssembly, parameters));
        }
    }
}

