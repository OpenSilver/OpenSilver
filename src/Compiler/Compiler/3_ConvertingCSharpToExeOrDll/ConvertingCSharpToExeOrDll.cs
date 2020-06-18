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
