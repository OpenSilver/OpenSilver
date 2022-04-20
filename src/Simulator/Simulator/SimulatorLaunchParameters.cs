using DotNetBrowser.WPF;
using DotNetForHtml5.EmulatorWithoutJavascript;
using System;
using System.Collections.Generic;

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

        /// <summary>
        /// Action to call when the provided app class is created successfully.
        /// </summary>
        public Action AppStartedCallback { get; set; }

        /// <summary>
        /// Gets or sets custom cookies to the simulator
        /// </summary>
        public IList<CookieData> CookiesData { get; set; }

        /// <summary>
        /// Gets or sets the argument parameters for the App constructor
        /// </summary>
        public object[] ConstructorArguments { get; set; }
    }
}