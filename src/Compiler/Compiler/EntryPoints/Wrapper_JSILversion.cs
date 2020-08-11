

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
using System.IO;

namespace DotNetForHtml5.Compiler
{
    //[LoadInSeparateAppDomain]
    //[Serializable]
    public class Wrapper_JSILversion : Task // AppDomainIsolatedTask
    {
        [Required]
        public string AssemblyNameWithoutExtension { get; set; }

        [Required]
        public string OutputRootPath { get; set; }

        [Required]
        public string OutputAppFilesPath { get; set; }

        [Required]
        public string OutputLibrariesPath { get; set; }

        [Required]
        public string OutputResourcesPath { get; set; }

        [Required]
        public string[] SourceLibrariesDirectories { get; set; }

        [Required]
        public string PathOfAssemblyThatContainsEntryPoint { get; set; }

        public override bool Execute()
        {
            return Execute(
                SourceLibrariesDirectories,
                PathOfAssemblyThatContainsEntryPoint,
                new LoggerThatUsesTaskOutput(this),
                OutputRootPath,
                OutputAppFilesPath,
                OutputLibrariesPath,
                OutputResourcesPath,
                AssemblyNameWithoutExtension
                );
        }

        public static bool Execute(
            string[] sourceLibrariesDirectories,
            string pathOfAssemblyThatContainsEntryPoint,
            ILogger logger,
            string outputRootPath,
            string outputAppFilesPath,
            string outputLibrariesPath,
            string outputResourcesPath,
            string assemblyNameWithoutExtension
            )
        {
            string operationName = "C#/XAML for HTML5: Wrapper";
            try
            {
                using (var executionTimeMeasuring = new ExecutionTimeMeasuring())
                {
                    // Validate input strings:
                    if (string.IsNullOrEmpty(pathOfAssemblyThatContainsEntryPoint))
                        throw new Exception(operationName + " failed because the entry point assembly path is invalid.");

                    //------- DISPLAY THE PROGRESS -------
                    logger.WriteMessage(operationName + " started. Source libraries directory: \"" + sourceLibrariesDirectories + "\". Assembly that contains entry point: \"" + pathOfAssemblyThatContainsEntryPoint);
                    //todo: do not display that much info.

                    // Start copying support HTML and JavaScript files:
                    WrappingOutputFiles_JSILversion.CreateAndCopySupportFiles(
                        pathOfAssemblyThatContainsEntryPoint,
                        new List<string>(sourceLibrariesDirectories),
                        outputRootPath,
                        outputAppFilesPath,
                        outputLibrariesPath,
                        outputResourcesPath,
                        assemblyNameWithoutExtension
                        );

                    //------- DISPLAY THE PROGRESS -------
                    logger.WriteMessage(operationName + " completed in " + executionTimeMeasuring.StopAndGetTimeInSeconds() + " seconds.");

                    return true;
                }
            }
            catch (Exception ex)
            {
                logger.WriteError(operationName + " failed: " + ex.ToString());
                return false;
            }
        }
    }
}
