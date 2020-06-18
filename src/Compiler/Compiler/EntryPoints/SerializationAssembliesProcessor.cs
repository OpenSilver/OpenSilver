using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.IO;

namespace DotNetForHtml5.Compiler
{
    public class SerializationAssembliesProcessor : Task
    {
        [Required]
        public string IntermediateOutputDirectory { get; set; }

        [Required]
        public bool SGenIsContinued { get; set; }  // This is True if the SGEN command line exceeds 32,000 characters and is therefore split into 2 calls to SGEN.exe.

        [Output]
        public string FileToIncludeInProject { get; set; }

        public override bool Execute()
        {
            ILogger logger = new LoggerThatUsesTaskOutput(this);
            string operationName = "C#/XAML for HTML5: SerializationAssembliesProcessor";
            try
            {
                string fileToIncludeInProject;
                bool isSuccess = GeneratingSerializationAssemblies.ProcessSourceCode(IntermediateOutputDirectory, SGenIsContinued, logger, out fileToIncludeInProject);
                FileToIncludeInProject = fileToIncludeInProject;
                return isSuccess;
            }
            catch (Exception ex)
            {
                logger.WriteError(operationName + " failed: " + ex.ToString());
                return false;
            }
        }
    }
}
