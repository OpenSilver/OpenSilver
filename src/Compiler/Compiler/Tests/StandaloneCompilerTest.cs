

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
using System.IO;

namespace DotNetForHtml5.Compiler
{
#if NO_LONGER_USED
    public static class StandaloneCompilerTest
    {
        public static void ProcessProject(
            string csProjFilePath,
            string outputPath,
            List<string> librariesFolders,
            ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain,
            List<string> coreAssembliesForUseByXamlToCSharpConverter,
            List<string> coreAssembliesForUseByCSharpToExeOrDllCompiler,
            ILogger logger)
        {
            // Create temp folder:
            logger.WriteMessage("-------- STARTED CREATING TEMPORARY FOLDER --------");
            string temporaryFolder = FileHelpers.CreateTemporaryFolder();
            logger.WriteMessage("-------- FINISHED CREATING TEMPORARY FOLDER --------");
            logger.WriteMessage("");

            // Copy files to temp folder and process them if necessary:
            logger.WriteMessage("-------- STARTED CREATING TEMPORARY SOLUTION --------");
            CreatingTemporarySolution.ReadAllFilesInSCProjAndCopyProcessThem(csProjFilePath, temporaryFolder, reflectionOnSeparateAppDomain, coreAssembliesForUseByXamlToCSharpConverter, coreAssembliesForUseByCSharpToExeOrDllCompiler);
            logger.WriteMessage("-------- FINISHED CREATING TEMPORARY SOLUTION --------");
            logger.WriteMessage("");

            // Build to EXE/DLL:
            logger.WriteMessage("-------- STARTED COMPILING TEMPORARY SOLUTION --------");
            if (!ConvertingCSharpToExeOrDll.StartBuildProcess(temporaryFolder, Path.GetFileName(csProjFilePath), logger))
            {
                logger.WriteError("-------- COMPILATION FAILED. ABORTING. --------");
                return;
            }
            logger.WriteMessage("-------- FINISHED COMPILING TEMPORARY SOLUTION --------");
            logger.WriteMessage("");

            // Create JavaScript from EXE/DLL:
            logger.WriteMessage("-------- STARTED GENERATING JAVASCRIPT --------");
            string assemblyPath = Path.Combine(temporaryFolder, "bin\\", Path.GetFileNameWithoutExtension(csProjFilePath) + ".dll");
            ConvertingExeOrDllToJavaScript.Convert(
                assemblyPath,
                outputPath,
                logger,
                assemblyPath,
                null,
                null,
                null,
                false,
                false,
                false,
                false,
                false,
                null);
            logger.WriteMessage("-------- FINISHED GENERATING JAVASCRIPT --------");
            logger.WriteMessage("");

            // Copy HTML/JavaScript support files:
            logger.WriteMessage("-------- STARTED COPYING SUPPORT FILES --------");
            CreatingAndCopyingSupportHtmlAndJavaScriptFiles.CreateAndCopySupportFiles(outputPath, assemblyPath, librariesFolders);
            logger.WriteMessage("-------- FINISHED COPYING SUPPORT FILES --------");
            logger.WriteMessage("");

            // Delete temp folder:
            logger.WriteMessage("-------- STARTED DELETING TEMPORARY FOLDER --------");
            FileHelpers.DeleteTemporaryFolder(temporaryFolder);
            logger.WriteMessage("-------- FINISHED DELETING TEMPORARY FOLDER --------");
            logger.WriteMessage("");
        }
    }
#endif
}
