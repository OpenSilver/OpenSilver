
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
