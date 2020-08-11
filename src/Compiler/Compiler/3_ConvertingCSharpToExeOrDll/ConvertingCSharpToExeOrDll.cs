

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



using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetForHtml5.Compiler
{
    internal static class ConvertingCSharpToExeOrDll
    {
        internal static bool StartBuildProcess(string temporaryFolder, string csProjFileNameWithoutPath, ILogger logger)
        {
            string csProjFileNameWithPath = Path.Combine(temporaryFolder, csProjFileNameWithoutPath);
            ProjectCollection pc = new ProjectCollection();
            Dictionary<string, string> GlobalProperty = new Dictionary<string, string>();
            GlobalProperty.Add("Configuration", "Debug");
            GlobalProperty.Add("Platform", "x86");
            GlobalProperty.Add("OutputPath", "bin");
            BuildParameters buildParameters = new BuildParameters(pc);
            BuildLogger customLogger = new BuildLogger();
            customLogger.OnWrite += (s, e) =>
                {
                    logger.WriteMessage(e.Text);
                };
            buildParameters.Loggers = new List<Microsoft.Build.Framework.ILogger>() { customLogger };
            BuildRequestData BuidlRequest = new BuildRequestData(csProjFileNameWithPath, GlobalProperty, null, new string[] { "Build" }, null);
            BuildResult buildResult = BuildManager.DefaultBuildManager.Build(buildParameters, BuidlRequest);
            return (buildResult.OverallResult == BuildResultCode.Success);
        }
    }
}
