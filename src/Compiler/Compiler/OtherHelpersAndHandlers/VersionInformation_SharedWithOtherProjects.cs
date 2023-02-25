
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

namespace OpenSilver
{
    internal static class VersionInformation
    {
        public static DateTime GetCurrentVersionReleaseDate()
        {
            // Note: Keep this date updated every time that a new version is released.
            // This date is used by the "ActivationApp" project to check if the end-user has the right to activate this app (because they usually get 1 year of free updates only).
            return new DateTime(2020, 08, 31); // Year, Month, Day
        }

        public static Version GetCurrentVersionNumber()
        {
            // Note: Keep this version number updated every time that a new version is released.
            // This version is used by the Compiler to inject the compiler version attributes into the user code.
            return new Version("1.0.0.007"); // Note: here, we use the "string" constructor of the Version class so that we can copy/paste the string to/from the Setup project, which leads to a more reliable distribution process (ie. fewer chances of mismatch or typos).
        }

        public static string GetCurrentVersionFriendlyName()
        {
            // Note: Keep this version friendly name updated every time that a new version is released.
            // This version is used by the Compiler to inject the compiler version attributes into the user code.
            return "Version 1.0.0-alpha-007 (2020.08.31)";
        }
    }
}
