
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

namespace CSHTML5.Internal
{
    public static class Performance
    {
#if !BRIDGE
        [JSReplacement("performance.now()")]
#else
        [Template("performance.now()")]
#endif
        public static double now()
        {
            return 0;
        }

#if !BRIDGE
        [JSReplacement(@"document.addToPerformanceCounters($name, $initialTime)")]
#else
        [Template("document.addToPerformanceCounters({name}, {initialTime})")]
#endif
        public static void Counter(string name, double initialTime)
        {
        }
    }
}
