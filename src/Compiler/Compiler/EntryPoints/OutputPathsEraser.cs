

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
    public class OutputPathsEraser : Task // AppDomainIsolatedTask
    {
        [Required]
        public string SourceAssembly { get; set; }

        [Required]
        public string OutputRootPath { get; set; }

        [Required]
        public string OutputAppFilesPath { get; set; }

        [Required]
        public string OutputLibrariesPath { get; set; }

        [Required]
        public string OutputResourcesPath { get; set; }

        public override bool Execute()
        {
            return Execute(SourceAssembly, OutputRootPath, OutputAppFilesPath, OutputLibrariesPath, OutputResourcesPath, new LoggerThatUsesTaskOutput(this));
        }

        public static bool Execute(string sourceAssembly, string outputRootPath, string outputAppFilesPath, string outputLibrariesPath, string outputResourcesPath, ILogger logger)
        {
            string operationName = "C#/XAML for HTML5: OutputPathsEraser";
            try
            {
                using (var executionTimeMeasuring = new ExecutionTimeMeasuring())
                {
                    // Validate input strings:
                    if (string.IsNullOrEmpty(sourceAssembly))
                        throw new Exception(operationName + " failed because the source assembly argument is invalid.");

                    //------- DISPLAY THE PROGRESS -------
                    logger.WriteMessage(operationName + " started for assembly \"" + sourceAssembly + "\".");

                    // Determine the absolute output path:
                    string outputPathAbsolute = PathsHelper.GetOutputPathAbsolute(sourceAssembly, outputRootPath);

                    // Create the output directory if it does not already exist:
                    Directory.CreateDirectory(Path.GetDirectoryName(outputPathAbsolute));

                    // Verify that the paths are relative, not absolute, because the HTML app will use them at runtime to locate the resources, so they must be located in a subfolder of the output folder. Futhermore, this reduces the chances of accidentally deleting other user files:
                    if (Path.IsPathRooted(outputAppFilesPath))
                        throw new Exception("The output app files path ('Cshtml5OutputAppFilesPath') must be relative, not absolute: " + outputAppFilesPath);
                    if (Path.IsPathRooted(outputLibrariesPath))
                        throw new Exception("The output libraries path ('Cshtml5OutputLibrariesPath') must be relative, not absolute: " + outputLibrariesPath);
                    if (Path.IsPathRooted(outputResourcesPath))
                        throw new Exception("The output resources path ('Cshtml5OutputResourcesPath') must be relative, not absolute: " + outputResourcesPath);

                    // Determine the absolute path of the AppFiles, Libraries, and Resources subfolders of the output folder:
                    string absoluteOutputAppFilesPath = PathsHelper.CombinePathsWhileEnsuringEndingBackslashAndMore(outputPathAbsolute, outputAppFilesPath);
                    string absoluteOutputLibrariesPath = PathsHelper.CombinePathsWhileEnsuringEndingBackslashAndMore(outputPathAbsolute, outputLibrariesPath);
                    string absoluteOutputResourcesPath = PathsHelper.CombinePathsWhileEnsuringEndingBackslashAndMore(outputPathAbsolute, outputResourcesPath);

                    // Make sure that the AppFiles, Libraries, and Resources subfolders of the output directories are empty:
                    if (Directory.Exists(absoluteOutputLibrariesPath))
                        FileHelpers.DeleteAllFilesAndFoldersInDirectory(absoluteOutputLibrariesPath); //todo: Display a warning in case that the user has specified an output path that contains stuff other than ours, such as "C:\"?
                    if (Directory.Exists(absoluteOutputResourcesPath))
                        FileHelpers.DeleteAllFilesAndFoldersInDirectory(absoluteOutputResourcesPath); //todo: Display a warning in case that the user has specified an output path that contains stuff other than ours, such as "C:\"?
                    /*
                    NOTE: The following line was commented on October 24, 2017. In fact, starting in CSHTML5 Beta 12.3, we no longer clean up the "app" folder on recompilation, in order to speed the compilation up by re-using the assemblies that have already been compiled.
                    if (Directory.Exists(absoluteOutputAppFilesPath))
                        FileHelpers.DeleteAllFilesAndFoldersInDirectory(absoluteOutputAppFilesPath);
                    */

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
