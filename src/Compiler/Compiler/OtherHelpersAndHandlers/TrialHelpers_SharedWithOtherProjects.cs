using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetForHtml5
{
    internal static class TrialHelpers
    {
#if BRIDGE
        public const int TRIAL_DURATION_IN_DAYS = 14;
#else
        public const int TRIAL_DURATION_IN_DAYS = 60;
#endif

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
