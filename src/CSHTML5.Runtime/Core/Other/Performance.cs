

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
