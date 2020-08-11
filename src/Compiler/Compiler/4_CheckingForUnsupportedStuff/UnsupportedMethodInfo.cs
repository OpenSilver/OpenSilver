

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

namespace DotNetForHtml5.Compiler
{
    public class UnsupportedMethodInfo
    {
        /// <summary>
        /// The error explanation to display in the output of VS.
        /// </summary>
        public string ExplanationToDisplayInErrorsWindow { get; set; }

#if REQUIRE_ACTIVATION_FOR_USING_CERTAIN_METHODS
        /// <summary>
        /// True if a missing feature is required (such as the "Professional Edition" feature), false otherwise.
        /// </summary>
        public bool RequiresMissingFeature { get; set; }

        /// <summary>
        /// The ID of the missing feature (such as "PROFESSIONAL_EDITION", cf. the constant ActivationHelpers.PROFESSIONAL_EDITION_FEATURE_ID).
        /// </summary>
        public string MissingFeatureId { get; set; }

        /// <summary>
        /// the message to display in the popup when the feature is missing (eg. "explanationToDisplayInActivationApp").
        /// </summary>
        public string MessageForMissingFeature { get; set; }

        /// <summary>
        /// True if the feature is in valid trial mode (eg. evaluating the "Professional Edition" feature), false otherwise.
        /// </summary>
        public bool IsInValidTrialMode { get; set; }
#endif

        /// <summary>
        /// The full name of the unsupported method.
        /// </summary>
        public string FullMethodName { get; set; }

        /// <summary>
        /// The location (full method name) where the unsupported method is used/called.
        /// </summary>
        public string CallingMethodFullName { get; set; }

        /// <summary>
        /// The location (name of the source code file with its path) where the unsupported method is used/called.
        /// </summary>
        public string CallingMethodFileNameWithPath { get; set; }

        /// <summary>
        /// The location (the line numner in the source code file) where the unsupported method is used/called.
        /// </summary>
        public int CallingMethodLineNumber { get; set; }

        /// <summary>
        /// The name of the assembly where the unsupported missing method is used/called.
        /// </summary>
        public string UserAssemblyName { get; set; }

        /// <summary>
        /// The name of the assembly where the unsupported method is located (eg. "mscorlib").
        /// </summary>
        public string MethodAssemblyName { get; set; }
    }
}
