using OpenSilver.Simulator;
using System;

namespace TestApplication.OpenSilver.Simulator
{
    static class Startup
    {
        [STAThread]
        static int Main(string[] args)
        {
            return SimulatorLauncher.Start(typeof(App));
        }
    }
}
