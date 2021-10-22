using System;
using System.Collections.Generic;
using System.Text;

namespace System.Windows
{
    // Summary:
    //     Exposes read-only data about how an application is performing.
	[OpenSilver.NotImplemented]
    public class Analytics
    {
        // Summary:
        //     Gets a factor that reports the total load on the CPU that this process is
        //     using, determined across all cores averaged together.
        //
        // Returns:
        //     A value between 0 and 1 that reports CPU load associated with this process,
        //     with the factor determined by examining all cores of a multi-core system
        //     averaged together. 0 maps to 0% load while 1 maps to 100% load.
		[OpenSilver.NotImplemented]
        public float AverageProcessLoad { get; }
        //
        // Summary:
        //     Gets a factor that reports the total load on the CPU by all processes, determined
        //     across all cores averaged together.
        //
        // Returns:
        //     A value between 0 and 1 that reports CPU load by all processes, with the
        //     factor determined by examining all cores of a multi-core system averaged
        //     together. 0 maps to 0% load while 1 maps to 100% load.
		[OpenSilver.NotImplemented]
        public float AverageProcessorLoad { get; }

    }
}
