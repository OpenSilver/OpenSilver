
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

using OpenSilver.Internal;

namespace CSHTML5.Internal
{
    public static class Performance
    {
        public static double now() => OpenSilver.Interop.ExecuteJavaScriptDouble("performance.now();");

        public static void Counter(string name, double initialTime)
        {
            string sName = INTERNAL_InteropImplementation.GetVariableStringForJS(name);
            string sTime = initialTime.ToInvariantString();
            OpenSilver.Interop.ExecuteJavaScriptVoid(
                $"document.addToPerformanceCounters({sName}, {sTime});");
        }
    }
}
