using DotNetForHtml5.EmulatorWithoutJavascript;
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

        /// <summary>
        /// Sets or gets custom cookies to the simulator
        /// </summary>
        public  IList<CookieData> CookiesData { get; set; }

    }
}
