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

        // note: this can grow very very fast, so if you don't need it, I suggest you turn it off
        public bool LogExecutedJavaScriptCode { get; set; } = true;

        public Action<WPFBrowserView> BrowserCreatedCallback { get; set; }

        /// <summary>
        /// Action to call when the provided app class is created successfully.
        /// </summary>
        public Action AppStartedCallback { get; set; }

        /// <summary>
        /// Sets or gets custom cookies to the simulator
        /// </summary>
        public IList<CookieData> CookiesData { get; set; }
    
        /// <summary>
        /// Sets the application init parameters
        /// </summary>
        public string InitParams { get; set; }
    }
}
