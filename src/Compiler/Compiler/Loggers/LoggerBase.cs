using Microsoft.Build.Framework;
using System;

namespace DotNetForHtml5.Compiler
{
    public class LoggerBase
    {
#if REQUIRE_ACTIVATION_FOR_USING_CERTAIN_METHODS
        bool _requiresMissingFeature;
        string _missingFeatureId;
        string _messageForMissingFeature;
        bool _isInTrialMode;

        public void SetRequiresMissingFeature(string missingFeatureId, string messageForMissingFeature)
        {
            _requiresMissingFeature = true;
            _missingFeatureId = missingFeatureId;
            _messageForMissingFeature = messageForMissingFeature;
        }

        public bool RequiresMissingFeature
        {
            get
            {
                return _requiresMissingFeature;
            }
        }

        public string MissingFeatureId
        {
            get
            {
                return _missingFeatureId;
            }
        }

        public string MessageForMissingFeature
        {
            get
            {
                return _messageForMissingFeature;
            }
        }
#endif
    }
}
