using OpenSilver.Simulator;
using System;
using System.Collections.Generic;
using System.Reflection;

#if OPENSILVER
namespace OpenSilver.Simulator
#else
namespace CSHTML5.Simulator
#endif
{
    public static class SimulatorLauncher
    {
#if OPENSILVER
        public static int Start(Type clientAppType, SimulatorLaunchParameters parameters = null)
        {
            if (clientAppType == null)
            {
                throw new ArgumentNullException(nameof(clientAppType));
            }

            return Start(() => Activator.CreateInstance(clientAppType), clientAppType.Assembly, parameters);
        }

        public static int Start(Action clientAppStarup, Assembly clientAppAssembly, SimulatorLaunchParameters parameters = null)
        {
            if (clientAppStarup == null)
            {
                throw new ArgumentNullException(nameof(clientAppStarup));
            }

            if (clientAppAssembly == null)
            {
                throw new ArgumentNullException(nameof(clientAppAssembly));
            }

            App app = new App();
            app.InitializeComponent();
            return app.Run(new MainWindow(clientAppStarup, clientAppAssembly, parameters));
        }
#elif BRIDGE
        [STAThread]
        public static int Main(string[] args)
        {
            App app = new App();
            app.InitializeComponent();
            return app.Run(new MainWindow());
        }
#endif
    }
}

