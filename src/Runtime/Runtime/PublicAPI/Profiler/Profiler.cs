

/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/


using System;

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
            return Convert.ToDouble(OpenSilver.Interop.ExecuteJavaScript("performance.now()"));
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
            OpenSilver.Interop.ExecuteJavaScript("document.addToPerformanceCounters($0, $1)", measureDescription, numberReturnedByTheStartMeasuringTimeMethod);
        }

#if !BRIDGE
        [JSReplacement("console.time($label)")]
#else
        [Template("console.time({label})")]
#endif
        public static void ConsoleTime(string label)
        {
            OpenSilver.Interop.ExecuteJavaScript("console.time($0)", label);
        }

#if !BRIDGE
        [JSReplacement("console.timeEnd($label)")]
#else
        [Template("console.timeEnd({label})")]
#endif
        public static void ConsoleTimeEnd(string label)
        {
            OpenSilver.Interop.ExecuteJavaScript("console.timeEnd($0)", label);
        }

#if !BRIDGE
        [JSReplacement("console.timeLog($label)")]
#else
        [Template("console.timeLog({label})")]
#endif
        public static void ConsoleTimeLog(string label)
        {
            OpenSilver.Interop.ExecuteJavaScript("console.timeLog($0)", label);
        }
    }
}
