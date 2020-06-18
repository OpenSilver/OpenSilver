using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

namespace DotNetForHtml5.Compiler
{
    //[LoadInSeparateAppDomain]
    //[Serializable]
    public class AssemblyMissingReferenceChecker : Task // AppDomainIsolatedTask
    {
        public Microsoft.Build.Framework.ITaskItem[] References { get; set; }

        public string RequiredAssemblies { get; set; }

        public override bool Execute()
        {
            if (!string.IsNullOrEmpty(RequiredAssemblies))
            {
                ILogger logger = new LoggerThatUsesTaskOutput(this);
                string operationName = "C#/XAML for HTML5: AssemblyMissingReferenceChecker";
                try
                {
                    return CheckingThatNoAssemblyReferenceIsMissing.Check(References, RequiredAssemblies, logger);
                }
                catch (Exception ex)
                {
                    logger.WriteError(operationName + " failed: " + ex.ToString());
                    return false;
                }
            }
            else
                return true;
        }

    }
}
