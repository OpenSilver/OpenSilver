
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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using OpenSilver.Compiler.Common;
using OpenSilver.Compiler.Common.Helpers;
using ILogger = OpenSilver.Compiler.Common.ILogger;
using Mono.Cecil;
using OpenSilver.Compiler.Resources.Helpers;

namespace OpenSilver.Compiler.Resources
{
    public class ResourcesExtractorAndCopier : Task
    {
        [Required]
        public string SourceAssembly { get; set; }

        [Required]
        public string OutputRootPath { get; set; }

        [Required]
        public string OutputResourcesPath { get; set; }

        [Required]
        public string AssembliesToIgnore { get; set; }

        [Required]
        public string SupportedExtensions { get; set; }

        public string CoreAssemblyFiles { get; set; }

        [Output]
        public bool IsSuccess { get; set; }

        [Output]
        public ITaskItem[] CopiedResXFiles { get; set; }

        public override bool Execute()
        {
            IsSuccess = ExecuteImpl(
                SourceAssembly,
                OutputRootPath,
                OutputResourcesPath,
                AssembliesToIgnore,
                SupportedExtensions,
                new LoggerThatUsesTaskOutput(this),
                CoreAssemblyFiles,
                out List<string> listOfCopiedResXFiles);
            CopiedResXFiles = listOfCopiedResXFiles.Select(s => new TaskItem() { ItemSpec = s }).ToArray();
            return IsSuccess;
        }

        private static bool ExecuteImpl(
            string sourceAssembly,
            string outputRootPath,
            string outputResourcesPath,
            string assembliesToIgnore,
            string supportedExtensions,
            ILogger logger,
            string coreAssemblyFiles,
            out List<string> listOfCopiedResXFiles)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            string operationName = "C#/XAML for HTML5: ResourcesExtractorAndCopier";
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

                    // Create a separate AppDomain so that the types loaded for reflection can be unloaded when done.
                    bool isSuccess = false;
                    using (var storage = new MonoCecilAssemblyStorage())
                    {
                        // Load for the core assemblies first:
                        string[] coreAssemblyFilesArray = coreAssemblyFiles.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string coreAssemblyFile in coreAssemblyFilesArray)
                        {
                            storage.LoadAssembly(
                                coreAssemblyFile,
                                loadReferencedAssembliesToo: false,
                                skipReadingAttributesFromAssemblies: true);
                        }

                        // Prepare some dictionaries:
                        string[] arrayWithSimpleNameOfAssembliesToIgnore = assembliesToIgnore != null ? assembliesToIgnore.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { };
                        HashSet<string> simpleNameOfAssembliesToIgnore = new HashSet<string>(arrayWithSimpleNameOfAssembliesToIgnore);
                        string[] arrayOfSupportedExtensions = supportedExtensions != null ? supportedExtensions.ToLowerInvariant().Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { };
                        HashSet<string> supportedExtensionsLowercase = new HashSet<string>(arrayOfSupportedExtensions);

                        // Do the extraction and copy:
                        isSuccess = ExtractResources(
                            sourceAssembly,
                            outputPathAbsolute,
                            outputResourcesPath,
                            simpleNameOfAssembliesToIgnore,
                            supportedExtensionsLowercase,
                            logger,
                            storage,
                            out listOfCopiedResXFiles);
                    }

                    //------- DISPLAY THE PROGRESS -------
                    logger.WriteMessage(operationName + (isSuccess ? " completed in " + executionTimeMeasuring.StopAndGetTimeInSeconds() + " seconds." : " failed."));

                    return isSuccess;
                }
            }
            catch (Exception ex)
            {
                logger.WriteError(operationName + " failed: " + ex.ToString());
                listOfCopiedResXFiles = null;
                return false;
            }
        }

        private static bool ExtractResources(
            string assemblyPath,
            string outputPathAbsolute,
            string outputResourcesPath,
            HashSet<string> simpleNameOfAssembliesToIgnore,
            HashSet<string> supportedExtensionsLowercase,
            ILogger logger,
            MonoCecilAssemblyStorage storage,
            out List<string> resXFilesCopied)
        {
            resXFilesCopied = new List<string>();
            var assemblySimpleNames = storage.LoadAssembly(assemblyPath, true, skipReadingAttributesFromAssemblies: true);

            foreach (string assemblySimpleName in assemblySimpleNames)
            {
                if (simpleNameOfAssembliesToIgnore.Contains(assemblySimpleName))
                {
                    continue;
                }

                //-----------------------------------------------
                // Process JavaScript, CSS, Image, Video, Audio files:
                //-----------------------------------------------

                Dictionary<string, byte[]> jsAndCssFiles = new ResourcesExtractor().GetManifestResources(storage, assemblySimpleName, supportedExtensionsLowercase);

                // Copy files:
                foreach (KeyValuePair<string, byte[]> file in jsAndCssFiles)
                {
                    string fileName = file.Key;
                    byte[] fileContent = file.Value;

                    // Combine the root output path and the relative "resources" folder path, while also ensuring that there is no forward slash, and that the path ends with a backslash:
                    string absoluteOutputResourcesPath = PathsHelper.CombinePathsWhileEnsuringEndingBackslashAndMore(outputPathAbsolute, outputResourcesPath);

                    // Create the destination folders hierarchy if it does not already exist:
                    string destinationFile = Path.Combine(absoluteOutputResourcesPath, assemblySimpleName + Path.DirectorySeparatorChar, fileName);
                    if (destinationFile.Length < 256)
                    {
                        string destinationDirectory = Path.GetDirectoryName(destinationFile);
                        if (!Directory.Exists(destinationDirectory))
                        {
                            Directory.CreateDirectory(destinationDirectory);
                        }

                        // Create the file:
                        File.WriteAllBytes(destinationFile, fileContent);

                        //put the file name in the list of files:
                        if (fileName.EndsWith(".resx.js"))
                        {
                            resXFilesCopied.Add(destinationFile);
                        }
                    }
                    else
                    {
                        logger.WriteWarning("Could not create the following output file because its path is too long: " + destinationFile);
                    }
                }
            }

            return true;
        }
    }
}
