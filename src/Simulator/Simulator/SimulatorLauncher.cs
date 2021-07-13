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
        public static IList<CookieData> CookiesData { get; set; }
        public static int Start(SimulatorLaunchParameters parameters = null)
        {
            CookiesData = parameters.CookiesData;
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

