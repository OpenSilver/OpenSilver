

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
