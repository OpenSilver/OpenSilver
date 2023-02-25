
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
using System.Globalization;

namespace OpenSilver
{
    internal static class TrialHelpers
    {
        public const int TRIAL_DURATION_IN_DAYS = 60;

        public const string TRIAL_VERSION_IDENTIFIER = "0005"; // Modify this string to reset the trial period of end-users, so that they can start a new trial.
        // Versions history:
        //      (empty) = Prior to v1.0 Beta 5
        //      0001 = v1.0 Beta 5 and above (Sept. 2015)
        //      0002 = v1.0 Beta 6 and above (Nov. 2015)
        //      0003 = v1.0 Beta 7.2 and above (Feb. 2016)
        //      0004 = v1.0 Beta 12 and above (Sept. 2017)
        //      0005 = v1.0 Release Candidate 1 and above (Feb. 2018)

        public enum TrialStatus
        {
            NotStarted, Running, Expired
        }

        public static TrialStatus IsTrial(string featureId, out int numberOfDaysLeft)
        {
            numberOfDaysLeft = 0;
            string value = RegistryHelpers.GetSetting("Trial_" + TRIAL_VERSION_IDENTIFIER + "_" + featureId, null);
            double valueAsDouble;
            if (value != null && Double.TryParse(value.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out valueAsDouble)) // Note: the "Replace" method is here only for backward compatibility, due to the fact that up to Beta 5 included, we didn't store the value with "InvariantCulture".
            {
                DateTime initialDate = DateTime.FromOADate(valueAsDouble);
                numberOfDaysLeft = TRIAL_DURATION_IN_DAYS - ((int)(DateTime.Now - initialDate).TotalDays);
                if (numberOfDaysLeft > 0)
                    return TrialStatus.Running;
                else
                    return TrialStatus.Expired;
            }
            else
                return TrialStatus.NotStarted;
        }
    }
}
