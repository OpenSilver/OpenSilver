
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
