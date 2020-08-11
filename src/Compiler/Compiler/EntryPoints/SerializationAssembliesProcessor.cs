

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
