

/*===================================================================================
* 
*   Copyright (c) Userware (OpenSilver.net, CSHTML5.com)
*      
*   This file is part of both the OpenSilver Simulator (https://opensilver.net), which
*   is licensed under the MIT license (https://opensource.org/licenses/MIT), and the
*   CSHTML5 Simulator (http://cshtml5.com), which is dual-licensed (MIT + commercial).
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/



using OpenSilver;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetForHtml5.EmulatorWithoutJavascript
{
    internal static class TrialHelpers_MoreMethods
    {
        public static void Reset(string featureId)
        {
            RegistryHelpers.DeleteSetting("IsCommunity");
            RegistryHelpers.DeleteSetting("Trial_" + TrialHelpers.TRIAL_VERSION_IDENTIFIER + "_" + featureId);
            RegistryHelpers.DeleteSetting("TrialMessageLastDisplayDate_" + TrialHelpers.TRIAL_VERSION_IDENTIFIER);
        }

        public static void StartTrial(string featureId)
        {
            RegistryHelpers.SaveSetting("Trial_" + TrialHelpers.TRIAL_VERSION_IDENTIFIER + "_" + featureId, DateTime.Now.ToOADate().ToString(CultureInfo.InvariantCulture));
        }

        public static void RememberThatTheTrialMessageWasDisplayedToday()
        {
            RegistryHelpers.SaveSetting("TrialMessageLastDisplayDate_" + TrialHelpers.TRIAL_VERSION_IDENTIFIER, DateTime.Today.ToOADate().ToString(CultureInfo.InvariantCulture));
        }

        public static bool WasTheTrialMessageAlreadyDisplayedToday
        {
            get
            {
                // This method is used to prevent showing the trial message too many times every day: we only display it once a day.

                string value = RegistryHelpers.GetSetting("TrialMessageLastDisplayDate_" + TrialHelpers.TRIAL_VERSION_IDENTIFIER, null);

                double valueAsDouble;
                if (value != null && double.TryParse(value.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out valueAsDouble)) // Note: the "Replace" method is here only for backward compatibility, due to the fact that up to Beta 5 included, we didn't store the value with "InvariantCulture".
                {
                    DateTime lastDisplayDate = DateTime.FromOADate(valueAsDouble);
                    if (DateTime.Today == lastDisplayDate)
                        return true; // Means that we have already displayed the Trial message today.
                    else
                        return false;
                }
                else
                    return false;
            }
        }
    }
}
