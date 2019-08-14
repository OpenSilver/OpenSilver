
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



#if !BRIDGE
using JSIL.Meta;
#else
using Bridge;
#endif


#if MIGRATION
using System.Windows;
#else

#endif

namespace CSHTML5
{
    /// <summary>
    /// Provides static methods for profiling performance in the browser.
    /// </summary>
    public static class Profiler
    {
        /// <summary>
        /// Allows measuring the cumulative time between the start and the end of the measure. It returns a number that you need to pass to the "StopMeasuringTime()" method.
        /// </summary>
        /// <returns>A number that you need to pass to the "StopMeasuringTime()" method.</returns>
#if !BRIDGE
        [JSReplacement("performance.now()")]
#else
        [Template("performance.now()")]
#endif
        public static double StartMeasuringTime()
        {
            return 0;
        }

        /// <summary>
        /// Allows measuring the time between the start and the end of the measure. The result is "accrued", meaning that it is cumulative if executed multiple times. You can see the result by calling "ViewProfilerResults()" from the browser console.
        /// </summary>
        /// <param name="measureDescription">An arbitrary text to describe the measure.</param>
        /// <param name="numberReturnedByTheStartMeasuringTimeMethod">The number returned by the call to "StartMeasuringTime()". It is used to calculate the time elapsed between the start and the end of the measure.</param>  
#if !BRIDGE
        [JSReplacement(@"document.addToPerformanceCounters($measureDescription, $numberReturnedByTheStartMeasuringTimeMethod)")]

#else
        [Template(@"document.addToPerformanceCounters({measureDescription}, {numberReturnedByTheStartMeasuringTimeMethod})")]
#endif
        public static void StopMeasuringTime(string measureDescription, double numberReturnedByTheStartMeasuringTimeMethod)
        {
        }

#if !BRIDGE
        [JSReplacement("console.time($label)")]
#else
        [Template("console.time({label})")]
#endif
        public static void ConsoleTime(string label)
        {
        }

#if !BRIDGE
        [JSReplacement("console.timeEnd($label)")]
#else
        [Template("console.timeEnd({label})")]
#endif
        public static void ConsoleTimeEnd(string label)
        {
        }

#if !BRIDGE
        [JSReplacement("console.timeLog($label)")]
#else
        [Template("console.timeLog({label})")]
#endif
        public static void ConsoleTimeLog(string label)
        {
        }
    }
}
