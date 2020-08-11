

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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DotNetForHtml5.Compiler
{
    internal static class ShortPathHelper
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int GetShortPathName(String pathName, StringBuilder shortName, int cbShortName);

        public static string GetShortPathName(string fullAbsolutePath)
        {
            StringBuilder sb = new StringBuilder(300);
            int n = GetShortPathName(fullAbsolutePath, sb, 300);
            if (n == 0) // check for errors
            {
                int errorCode = Marshal.GetLastWin32Error();
                string pleaseReportThisError = "   - Please report this error to support@cshtml5.com";
                if (errorCode == 2)
                    throw new Exception("The following folder was not found: " + fullAbsolutePath + pleaseReportThisError);
                else
                    throw new Exception("GetShortPathName failed with error code " + errorCode.ToString() + pleaseReportThisError);
            }
            else
                return sb.ToString();
        }
    }
}
