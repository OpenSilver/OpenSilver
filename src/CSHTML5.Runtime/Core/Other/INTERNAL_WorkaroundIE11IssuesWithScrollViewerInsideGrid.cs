

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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSHTML5.Internal
{
    internal static class INTERNAL_WorkaroundIE11IssuesWithScrollViewerInsideGrid
    {
        public static void RefreshLayoutIfIE()
        {
            // On Internet Explorer, force refresh the layout to workaround the issues when a scrollviewer is inside a grid:
            if (IsRunningInJavaScript())
            {
#if !BRIDGE
                JSIL.Verbatim.Expression(@"
if (window.IE_VERSION)
{
    // Force refresh the layout to workaround the issues when a scrollviewer is inside a grid:
    var temp = document.createElement('div');
    document.body.appendChild(temp);
    document.body.removeChild(temp);
}
");
#else
                //verify is verbatim works.
                Script.Write(@"
if (window.IE_VERSION)
{
    // Force refresh the layout to workaround the issues when a scrollviewer is inside a grid:
    var temp = document.createElement('div');
    document.body.appendChild(temp);
    document.body.removeChild(temp);
}
");
#endif
            }
        }

#if !BRIDGE
        [JSReplacement("true")]
#else
        [Template("true")]
#endif
        static bool IsRunningInJavaScript()
        {
            return false;
        }
    }
}
