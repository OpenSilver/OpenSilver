using DotNetForHtml5.EmulatorWithoutJavascript;
using System;
using System.Collections.Generic;

#if OPENSILVER
namespace OpenSilver.Simulator
#else
namespace CSHTML5.Simulator
#endif
{
    public static class SimulatorLauncher
    {
#if OPENSILVER
        public static int Start(Type userApplicationType, SimulatorLaunchParameters parameters = null)
        {
            CookiesData = parameters.CookiesData;
            App app = new App();
            app.InitializeComponent();
            return app.Run(new MainWindow(userApplicationType, parameters));
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

