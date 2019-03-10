
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
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
    }
}
