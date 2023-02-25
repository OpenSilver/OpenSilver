
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

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace OpenSilver
{
    internal static class ActivationHelpers
    {
        internal static bool IsFeatureEnabled(string featureId)
        {
            return IsFeatureEnabled(featureId, null);
        }

        internal static bool IsFeatureEnabled(string featureId, HashSet<string> flags)
        {
#if SPECIAL_LICENSE_FOR_APL_USERS
            // If the "apl" flag is specified, we consider that the Pro Edition features are allowed. This was made for the APL community. See the ZenDesk ticket 807 and related.
            if (flags != null && flags.Contains("apl") && featureId == Constants.PROFESSIONAL_EDITION_FEATURE_ID)
            {
                return true;
            }
#endif

            object value = RegistryHelpers.GetSetting("Feature_" + featureId, null);
            if (value != null)
                return true; //todo: to prevent registry tampering, we should retrieve the activation key and check with the server that it is ok.
            else
                return false;
        }

        internal static string GetActivationAppPath()
        {
            // otherwise we use the activation app of internal stuff
            return PathsHelper.GetActivationAppPath();
        }

        internal static void DisplayActivationApp(string activationAppPath, string missingFeatureId, string messageForMissingFeature)
        {
            string fullPathToActivationApp = Path.Combine(Directory.GetCurrentDirectory(), activationAppPath);
            //string arguments = "/custom " + missingFeatureId + " " + messageForMissingFeature;

            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = Path.GetFileName(fullPathToActivationApp);
            psi.WorkingDirectory = Path.GetDirectoryName(fullPathToActivationApp);
            //psi.Arguments = arguments;
            Process proc = Process.Start(psi);
            proc.WaitForExit();
        }
    }
}
