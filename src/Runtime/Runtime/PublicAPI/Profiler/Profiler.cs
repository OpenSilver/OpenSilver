
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
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace OpenSilver
{
    /// <summary>
    /// Provides static methods for profiling performance in the browser.
    /// </summary>
    public static class Profiler
    {
        // Idea for improvement: Add "results management" methods such as Clear or stuff like that.
        private static readonly Dictionary<string, PerformanceCounter> PerformanceCounters = new Dictionary<string, PerformanceCounter>();

        static Profiler()
        {
            string sCallback = Interop.GetVariableStringForJS((Action)ViewProfilerResults);
            Interop.ExecuteJavaScriptVoid($"window.ViewProfilerResults = {sCallback}");
        }

        /// <summary>
        /// Allows measuring the cumulative time between the start and the end of the measure. It returns a number that you need to pass to the "StopMeasuringTime()" method.
        /// </summary>
        /// <returns>A number that you need to pass to the "StopMeasuringTime()" method.</returns>
        public static long StartMeasuringTime()
        {
            return Stopwatch.GetTimestamp();
        }

        /// <summary>
        /// Allows measuring the time between the start and the end of the measure. The result is "accrued", meaning that it is cumulative if executed multiple times. You can see the result by calling "ViewProfilerResults()" from the browser console.
        /// </summary>
        /// <param name="measureDescription">An arbitrary text to describe the measure.</param>
        /// <param name="numberReturnedByTheStartMeasuringTimeMethod">The number returned by the call to "StartMeasuringTime()". It is used to calculate the time elapsed between the start and the end of the measure.</param>  
        public static void StopMeasuringTime(string measureDescription, long numberReturnedByTheStartMeasuringTimeMethod)
        {
            AddToPerformanceCounters(measureDescription, numberReturnedByTheStartMeasuringTimeMethod);
        }

        private static void AddToPerformanceCounters(string measureDescription, long initialTime)
        {
            long elapsedTime = Stopwatch.GetTimestamp() - initialTime;
            PerformanceCounters.TryGetValue(measureDescription, out PerformanceCounter counter);
            if (counter == null)
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
            if (PerformanceCounters.Count > 0)
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
            if (Interop.IsRunningInTheSimulator)
            {
                Debug.WriteLine(text);
            }
            else
            {
                Console.WriteLine(text);
            }
        }

        public static void ConsoleTime(string label)
        {
            Interop.ExecuteJavaScriptVoid(
                $"console.time({Interop.GetVariableStringForJS(label)})");
        }

        public static void ConsoleTimeEnd(string label)
        {
            Interop.ExecuteJavaScriptVoid(
                $"console.timeEnd({Interop.GetVariableStringForJS(label)})");
        }

        public static void ConsoleTimeLog(string label)
        {
            Interop.ExecuteJavaScriptVoid(
                $"console.timeLog({Interop.GetVariableStringForJS(label)})");
        }
    }

    class PerformanceCounter
    {
        public int Count { get; set; }
        public long Time { get; set; }
    }
}
