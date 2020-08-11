

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
using System.Text;
using System.Threading.Tasks;

namespace DotNetForHtml5
{
    internal static class VersionInformation
    {
        public static DateTime GetCurrentVersionReleaseDate()
        {
            // Note: Keep this date updated every time that a new version is released.
            // This date is used by the "ActivationApp" project to check if the end-user has the right to activate this app (because they usually get 1 year of free updates only).

#if BRIDGE
            return new DateTime(2020, 08, 05); // Year, Month, Day
#elif CSHTML5BLAZOR
            return new DateTime(2020, 08, 05); // Year, Month, Day
#else
            return new DateTime(2019, 10, 17); // Year, Month, Day
#endif
        }

        public static Version GetCurrentVersionNumber()
        {
            // Note: Keep this version number updated every time that a new version is released.
            // This version is used by the Compiler to inject the compiler version attributes into the user code.

#if BRIDGE
            return new Version("2.0.0.70090"); // Note: here, we use the "string" constructor of the Version class so that we can copy/paste the string to/from the Setup project, which leads to a more reliable distribution process (ie. fewer chances of mismatch or typos).
#elif CSHTML5BLAZOR
            return new Version("1.0.0.006"); // Note: here, we use the "string" constructor of the Version class so that we can copy/paste the string to/from the Setup project, which leads to a more reliable distribution process (ie. fewer chances of mismatch or typos).
#else
            return new Version("1.2.137.191017"); // Note: here, we use the "string" constructor of the Version class so that we can copy/paste the string to/from the Setup project, which leads to a more reliable distribution process (ie. fewer chances of mismatch or typos).
#endif
        }

        public static string GetCurrentVersionFriendlyName()
        {
            // Note: Keep this version friendly name updated every time that a new version is released.
            // This version is used by the Compiler to inject the compiler version attributes into the user code.

#if BRIDGE
            return "Version 2.0 Preview 0.7 (2.0.0-alpha70-090) (2020.08.05)";
#elif CSHTML5BLAZOR
            return "Version 1.0.0-alpha-006 (2020.08.05)";
#else
            return "Version 1.2.4 R1";
#endif
        }
    }
}
