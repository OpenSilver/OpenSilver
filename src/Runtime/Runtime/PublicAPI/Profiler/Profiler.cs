

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
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
#else
using Bridge;
#endif


#if MIGRATION
using System.Windows;
#else

#endif

#if OPENSILVER
namespace OpenSilver
#else
namespace CSHTML5 
#endif
{
    /// <summary>
    /// Provides static methods for profiling performance in the browser.
    /// </summary>
    public static class Profiler
    {
        // Idea for improvement: Add "results management" methods such as Clear or stuff like that.
#if OPENSILVER
        static Dictionary<string, PerformanceCounter> PerformanceCounters = new Dictionary<string, PerformanceCounter>();

        static Profiler()
        {
            OpenSilver.Interop.ExecuteJavaScript("window.ViewProfilerResults = $0", (Action)ViewProfilerResults);
        }
#endif

        /// <summary>
        /// Allows measuring the cumulative time between the start and the end of the measure. It returns a number that you need to pass to the "StopMeasuringTime()" method.
        /// </summary>
        /// <returns>A number that you need to pass to the "StopMeasuringTime()" method.</returns>
#if !BRIDGE
        [JSReplacement("performance.now()")]
#else
        [Template("performance.now()")]
#endif
#if OPENSILVER
        public static long StartMeasuringTime()
        {
            return System.Diagnostics.Stopwatch.GetTimestamp();
#else
        public static double StartMeasuringTime()
        {
            return Convert.ToDouble(OpenSilver.Interop.ExecuteJavaScript("performance.now()")); 
#endif
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
#if OPENSILVER
        public static void StopMeasuringTime(string measureDescription, long numberReturnedByTheStartMeasuringTimeMethod)
        {
            AddToPerformanceCounters(measureDescription, numberReturnedByTheStartMeasuringTimeMethod);
#else
        public static void StopMeasuringTime(string measureDescription, double numberReturnedByTheStartMeasuringTimeMethod)
        {
            OpenSilver.Interop.ExecuteJavaScript("document.addToPerformanceCounters($0, $1)", measureDescription, numberReturnedByTheStartMeasuringTimeMethod); 
#endif
        }

#if OPENSILVER
        private static void AddToPerformanceCounters(string measureDescription, long initialTime)
        {
            long elapsedTime = (long)(System.Diagnostics.Stopwatch.GetTimestamp() - initialTime); 
            PerformanceCounter counter;
            PerformanceCounters.TryGetValue(measureDescription, out counter);
            if(counter == null)
            {
                counter = new PerformanceCounter();
                PerformanceCounters[measureDescription] = counter;
            }
            ++counter.Count;
            counter.Time += elapsedTime;
        }

        /// <summary>
        /// Shows the measures obtained so far in the Output window of Visual Studio if in the Simulator, in the browser's developer tools' console if in the Browser.
        /// </summary>
        public static void ViewProfilerResults()
        {
            //Ideas for improvements: Add parameter(s) to decide the output type (messageBox, file, Output, ...?)
            if(PerformanceCounters.Count > 0)
            {
                var sortedCounters = PerformanceCounters.OrderBy(x => x.Key);
                string csvFormat = "Description,Total time in ms, Number of calls (OS)" + Environment.NewLine;
                long ticksPerMs = Stopwatch.Frequency / 1000; //Note: Stopwatch.Frequency is the amount of ticks per second in the values returned, it is not representative of the accuracy. For example, a test today gave an Frequency 100 times bigger in the browser than in the Simulator despite the fact that the values returned in the Simulator are A LOT more precise than in the browser which only has an accuracy of .1ms.
                foreach (var counterKVP in sortedCounters) //KVP for KeyValuePair
                {
                    var name = counterKVP.Key;
                    var counter = counterKVP.Value;

                    double time = ((double)(counter.Time)) / ticksPerMs;
                    //Write the information on the current call:
                    WriteLine("=== " + name + " ===");
                    
                    WriteLine("Total time: " + time + "ms");
                    WriteLine("Number of calls: " + counter.Count);
                    if (counter.Count > 0)
                        WriteLine("Average time per call: " + (time / counter.Count) + "ms");
                    WriteLine("");

                    //prepare the csv format string:
                    csvFormat += name + ',' + time + ',' + counter.Count + '\n';
                }

                WriteLine("### RESULTS IN CSV FORMAT: ###");
                WriteLine(csvFormat);
            }
        }

        static void WriteLine(string text)
        {
            if(OpenSilver.Interop.IsRunningInTheSimulator)
            {
                System.Diagnostics.Debug.WriteLine(text);
            }
            else
            {
                Console.WriteLine(text);
            }
        }
#endif

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

#if OPENSILVER
    class PerformanceCounter
    {
        public int Count { get; set; }
        public long Time { get; set; }
    } 
#endif
}
