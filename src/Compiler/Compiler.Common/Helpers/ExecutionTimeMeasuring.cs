
/*===================================================================================
* 
*   Copyright (c) Userware (OpenSilver.net, CSHTML5.com)
*      
*   This file is part of both the OpenSilver Compiler (https://opensilver.net), which
*   is licensed under the MIT license (https://opensource.org/licenses/MIT), and the
*   CSHTML5 Compiler (http://cshtml5.com), which is dual-licensed (MIT + commercial).
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using System;
using System.Diagnostics;
using System.Globalization;

namespace OpenSilver.Compiler.Common
{
    public sealed class ExecutionTimeMeasuring : IDisposable
    {
        private readonly Stopwatch _stopwatch;

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
