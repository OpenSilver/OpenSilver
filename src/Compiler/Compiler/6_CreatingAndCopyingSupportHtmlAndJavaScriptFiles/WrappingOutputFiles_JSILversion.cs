

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
using System.Text;

namespace DotNetForHtml5.Compiler
{
    internal static class WrappingOutputFiles_JSILversion
    {
        public static void CreateAndCopySupportFiles(
            string pathOfAssemblyThatContainsEntryPoint,
            List<string> librariesFolders,
            string outputRootPath,
            string outputAppFilesPath,
            string outputLibrariesPath,
            string outputResourcesPath,
            string assemblyNameWithoutExtension
            )
        {
            StringBuilder codeForReferencingAdditionalLibraries = new StringBuilder();

            // Determine the output path:
            string outputPathAbsolute = PathsHelper.GetOutputPathAbsolute(pathOfAssemblyThatContainsEntryPoint, outputRootPath);

            // Combine the output path and the relative "Libraries" folder path, while also ensuring that there is no forward slash, and that the path ends with a backslash:
            string absoluteOutputLibrariesPath = PathsHelper.CombinePathsWhileEnsuringEndingBackslashAndMore(outputPathAbsolute, outputLibrariesPath); // Note: when it was hard-coded, it was Path.Combine(outputPath, @"Libraries\");

            // Create the destination folders hierarchy if it does not already exist:
            if (!Directory.Exists(absoluteOutputLibrariesPath))
                Directory.CreateDirectory(absoluteOutputLibrariesPath);

            // Copy the content of the "Libraries" files:
            bool mainJsilFileFound = false;
            foreach (string librariesFolder in librariesFolders)
            {
                foreach (var file in Directory.GetFiles(librariesFolder))
                {
                    string fileName = Path.GetFileName(file);
                    string outputFileWithFullPath = Path.Combine(absoluteOutputLibrariesPath, fileName);
                    if (fileName.ToUpper() == "JSIL.JS")
                    {
                        // Replace some JS code that is inside the file "JSIL.js":
                        string stringToReplace = @"src=\"""" + uri + ""\""";
                        string replaceWith = @"src=\"""" + uri + """ + String.Format("?{0:yyyyMdHHmm}", DateTime.Now) + @"\""";
                        CopyFileWhilePerformingStringReplace(file, outputFileWithFullPath, stringToReplace, replaceWith, throwIfNotFound: true);
                        mainJsilFileFound = true;
                    }
                    else
                    {
                        File.Copy(file, Path.Combine(absoluteOutputLibrariesPath, fileName), true);
                    }
                    if (!fileName.ToUpper().StartsWith("JSIL."))
                        codeForReferencingAdditionalLibraries.AppendLine(string.Format(@"<script src=""{0}{1}"" type=""text/javascript""></script>", PathsHelper.EnsureNoBackslashAndEnsureItEndsWithAForwardSlash(outputLibrariesPath), fileName + String.Format("?{0:yyyyMdHHmm}", DateTime.Now)));
                }
            }
            if (!mainJsilFileFound)
                throw new Exception("The file \"jsil.js\" was not found.");

            // Generate the startup code that instantiates the application class:
            string startupCode = GenerateStartupCode(pathOfAssemblyThatContainsEntryPoint);

            // Create and save the "index.html" file:
            CreateAndSaveIndexHtml(
                outputPathAbsolute,
                startupCode,
                codeForReferencingAdditionalLibraries.ToString(),
                outputRootPath,
                outputAppFilesPath,
                outputLibrariesPath,
                outputResourcesPath,
                assemblyNameWithoutExtension
                );
        }

        static void CopyFileWhilePerformingStringReplace(string sourceFileWithFullPath, string destinationFileWithFullPath, string stringToReplace, string replaceWith, bool throwIfNotFound = true)
        {
            // Read the source file:
            string sourceFileContent = File.ReadAllText(sourceFileWithFullPath);

            // Verify that the string was found:
            if (throwIfNotFound && !sourceFileContent.Contains(stringToReplace))
                throw new Exception("String not found: " + stringToReplace);

            // Replace:
            string newContent = sourceFileContent.Replace(stringToReplace, replaceWith);

            // Save the file:
            File.WriteAllText(destinationFileWithFullPath, newContent);
        }

        static void CreateAndSaveIndexHtml(
            string outputPathAbsolute,
            string startupCode,
            string codeForReferencingAdditionalLibraries,
            string outputRootPath,
            string outputAppFilesPath,
            string outputLibrariesPath,
            string outputResourcesPath,
            string assemblyNameWithoutExtension
            )
        {
            // Read the "index.html" template:
            string indexHtmlFileTemplate = WrapperHelpers.ReadTextFileFromEmbeddedResource("index.html");

            // Replace the placeholders:
            string indexHtmlFile = indexHtmlFileTemplate.Replace("[BOOT_JAVASCRIPT_CODE_GOES_HERE]", startupCode);
            indexHtmlFile = indexHtmlFile.Replace("[ADDITIONAL_LIBRARIES_GO_HERE]", codeForReferencingAdditionalLibraries);
            indexHtmlFile = indexHtmlFile.Replace("[LIBRARIES_PATH_GOES_HERE]", PathsHelper.EnsureNoBackslashAndEnsureItEndsWithAForwardSlash(outputLibrariesPath));
            indexHtmlFile = indexHtmlFile.Replace("[APPFILES_PATH_GOES_HERE]", PathsHelper.EnsureNoBackslashAndEnsureItEndsWithAForwardSlash(outputAppFilesPath));

            // Prevent browser caching of the JSIL and CSHTML5 libraries:
            indexHtmlFile = WrapperHelpers.AppendDateToLibraryFileName("JSIL.js", indexHtmlFile);
            indexHtmlFile = WrapperHelpers.AppendDateToLibraryFileName("cshtml5.js", indexHtmlFile);
            indexHtmlFile = WrapperHelpers.AppendDateToLibraryFileName("cshtml5.css", indexHtmlFile);

            // Read the "App.Config" file for future use by the ClientBase.
            string relativePathToAppConfigFolder = PathsHelper.CombinePathsWhileEnsuringEndingBackslashAndMore(outputResourcesPath, assemblyNameWithoutExtension);
            string relativePathToAppConfig = Path.Combine(relativePathToAppConfigFolder, "app.config.g.js");
            if (File.Exists(Path.Combine(outputPathAbsolute, relativePathToAppConfig)))
            {
                string scriptToReadAppConfig = "<script type=\"application/javascript\" src=\"" + relativePathToAppConfig + "\"></script>";
                indexHtmlFile = indexHtmlFile.Replace("[SCRIPT_TO_READ_APPCONFIG_GOES_HERE]", scriptToReadAppConfig);
            }
            else
            {
                indexHtmlFile = indexHtmlFile.Replace("[SCRIPT_TO_READ_APPCONFIG_GOES_HERE]", string.Empty);
            }

            // Read the "ServiceReferences.ClientConfig" file for future use by the ClientBase.
            string relativePathToServiceReferencesClientConfig = Path.Combine(relativePathToAppConfigFolder, "servicereferences.clientconfig.g.js");
            if (File.Exists(Path.Combine(outputPathAbsolute, relativePathToServiceReferencesClientConfig)))
            {
                string scriptToReadServiceReferencesClientConfig = "<script src=\"" + relativePathToServiceReferencesClientConfig.Replace('\\', '/') + "\"></script>";
                indexHtmlFile = indexHtmlFile.Replace("[SCRIPT_TO_READ_SERVICEREFERENCESCLIENTCONFIG_GOES_HERE]", scriptToReadServiceReferencesClientConfig);
            }
            else
            {
                indexHtmlFile = indexHtmlFile.Replace("[SCRIPT_TO_READ_SERVICEREFERENCESCLIENTCONFIG_GOES_HERE]", string.Empty);
            }

            // Save the "index.html" to the final folder:
            File.WriteAllText(Path.Combine(outputPathAbsolute, "index.html"), indexHtmlFile);
        }

        static string GenerateStartupCode(string pathOfAssemblyThatContainsEntryPoint)
        {
            // Get the assembly that is supposed to contain the entry point, ie. a class that inherits from "Application":
            string applicationClassFullName;
            string assemblyName;
            string assemblyFullName;
            ApplicationEntryPointFinder.GetFullNameOfClassThatInheritsFromApplication(pathOfAssemblyThatContainsEntryPoint, out applicationClassFullName, out assemblyName, out assemblyFullName);

            //Generate the JavaScript code for instantiating the application:
            string generatedCode = string.Format(@"
                JSIL.LocalStorage.Initialize(""{0}"");
                var asm = JSIL.GetAssembly(""{1}"", true);
                var app = new asm.{2}();
                ", assemblyName, assemblyFullName, applicationClassFullName);

            // Return the generated code:
            return generatedCode;
        }
    }
}
