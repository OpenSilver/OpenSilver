
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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if BRIDGE
using Bridge;
#endif

internal class INTERNAL_EscapeHelpers
{
    public static string EscapeXml(string str)
    {
#if !BRIDGE
        return (string)JSIL.Verbatim.Expression(@"
            $0.replace(/&/g, '&amp;')
               .replace(/</g, '&lt;')
               .replace(/>/g, '&gt;')
               .replace(/""/g, '&quot;')
               .replace(/'/g, '&apos;')", str);
#else
        return Script.Write<string>(@"
            $0.replace(/&/g, '&amp;')
               .replace(/</g, '&lt;')
               .replace(/>/g, '&gt;')
               .replace(/""/g, '&quot;')
               .replace(/'/g, '&apos;')", str);
#endif
    }

    public static string UnescapeXml(string str)
    {
#if !BRIDGE
        return (string)JSIL.Verbatim.Expression(@"
            $0.replace(/&apos;/g, ""'"")
               .replace(/&quot;/g, '""')
               .replace(/&gt;/g, '>')
               .replace(/&lt;/g, '<')
               .replace(/&amp;/g, '&')", str);
#else
        return Script.Write<string>(@"
            $0.replace(/&apos;/g, ""'"")
               .replace(/&quot;/g, '""')
               .replace(/&gt;/g, '>')
               .replace(/&lt;/g, '<')
               .replace(/&amp;/g, '&')", str);

#endif
    }
}
