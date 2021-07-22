using DotNetBrowser.WPF;
using DotNetForHtml5.EmulatorWithoutJavascript;
using System;

#if OPENSILVER
namespace OpenSilver.Simulator
#else
namespace CSHTML5.Simulator
#endif
{
    public class SimulatorLaunchParameters
    {
        // Add stuff as needed, like cookies, etc.

        public Action<WPFBrowserView> BrowserCreatedCallback { get; set; }
    }
}
