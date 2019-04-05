
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



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
