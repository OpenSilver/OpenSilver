﻿using DotNetForHtml5.EmulatorWithoutJavascript;
using Microsoft.Web.WebView2.Wpf;

namespace OpenSilver.Simulator
{
    public class SimulatorLaunchParameters
    {

        // Add stuff as needed, like cookies, etc.

        // note: this can grow very very fast, so if you don't need it, I suggest you turn it off
        public bool LogExecutedJavaScriptCode { get; set; } = true;

        public Action<WebView2> BrowserCreatedCallback { get; set; }

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

        /// <summary>
        /// The URL used by the simulator's browser instance.
        /// This can be useful for cases where a project requires "http" instead of "https" or any other 
        /// URL customization. If not specified, the URL <b>https://simulator.opensilver/</b> will be used.
        /// </summary>
        public string SimulatorUrl { get; set; }
    }
}
