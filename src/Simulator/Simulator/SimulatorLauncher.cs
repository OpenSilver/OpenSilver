using DotNetForHtml5.EmulatorWithoutJavascript;
using System;

#if OPENSILVER
namespace OpenSilver.Simulator
#else
namespace CSHTML5.Simulator
#endif
{
    public static class SimulatorLauncher
    {
        public static int Start(SimulatorLaunchParameters parameters = null)
        {
            App app = new App();
            app.InitializeComponent();
            return app.Run();
        }

#if BRIDGE
        [STAThread]
        public static int Main(string[] args)
        {
            return Start();
        }
#endif
    }
}

