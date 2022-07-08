

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
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetForHtml5.Compiler
{
    public class ValidateCompilerVersion : Task
    {
        [Required]
        public int TargetCompilerBuildNumber { get; set; }

        [Output]
        public bool IsCompilerVersionChecked { get; set; }

        public override bool Execute()
        {
            IsCompilerVersionChecked = true;
            return Execute(TargetCompilerBuildNumber, new LoggerThatUsesTaskOutput(this));
        }

        public static bool Execute(int targetCompilerBuildNumber, ILogger logger)
        {
            if(Version.CompilerBuildNumber < targetCompilerBuildNumber) //if this is true, then it means msbuild is using an outdated version of the compiler compared to what should be used by the OpenSilver/CSHTML5 package.
            {
                logger.WriteError("Compiler initialization error. Please restart Visual Studio or manually kill the Msbuild process, then recompile the solution to fix this error.");
                return false;
            }
            return true;
        }
    }
}
