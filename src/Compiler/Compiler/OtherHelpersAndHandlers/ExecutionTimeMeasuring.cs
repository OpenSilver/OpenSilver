using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetForHtml5.Compiler
{
    internal class ExecutionTimeMeasuring : IDisposable
    {
        Stopwatch _stopwatch;

        public ExecutionTimeMeasuring()
        {
            _stopwatch = Stopwatch.StartNew();
        }

        public string StopAndGetTimeInSeconds()
        {
            _stopwatch.Stop();
            return ((_stopwatch.ElapsedMilliseconds + 1L) / 1000).ToString(CultureInfo.InvariantCulture);
        }

        public void Dispose()
        {
            _stopwatch.Stop();
        }
    }
}
