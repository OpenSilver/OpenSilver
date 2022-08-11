

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

namespace DotNetForHtml5.Compiler
{
    //[LoadInSeparateAppDomain]
    //[Serializable]
    public class ResourcesExtractorAndCopier : Task // AppDomainIsolatedTask
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

        [Required]
        public bool IsBridgeBasedVersion { get; set; }

#if BRIDGE
        [Required]
#endif
        public string NameOfAssembliesThatDoNotContainUserCode { get; set; }

#if BRIDGE
        [Required]
#endif
        public string TypeForwardingAssemblyPath { get; set; }

#if BRIDGE
        [Required]
#endif
        public string CoreAssemblyFiles { get; set; }

        [Output]
        public bool IsSuccess { get; set; }

        [Output]
        public ITaskItem[] CopiedResXFiles { get; set; }

        public override bool Execute()
        {
            List<string> listOfCopiedResXFiles;
            IsSuccess = Execute(SourceAssembly, OutputRootPath, OutputResourcesPath, AssembliesToIgnore, SupportedExtensions, new LoggerThatUsesTaskOutput(this), IsBridgeBasedVersion, TypeForwardingAssemblyPath, NameOfAssembliesThatDoNotContainUserCode, CoreAssemblyFiles, out listOfCopiedResXFiles);
            CopiedResXFiles = listOfCopiedResXFiles.Select(s => new TaskItem() { ItemSpec = s }).ToArray();
            return IsSuccess;
        }

        public static bool Execute(string sourceAssembly, string outputRootPath, string outputResourcesPath, string assembliesToIgnore, string supportedExtensions, ILogger logger, bool isBridgeBasedVersion, string typeForwardingAssemblyPath, string nameOfAssembliesThatDoNotContainUserCode, string coreAssemblyFiles, out List<string> listOfCopiedResXFiles)
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
                    using (var reflectionOnSeparateAppDomain = new ReflectionOnSeparateAppDomainHandler(typeForwardingAssemblyPath))
                    {
#if BRIDGE || CSHTML5BLAZOR
                        // Load for the core assemblies first:
                        string[] coreAssemblyFilesArray = coreAssemblyFiles.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string coreAssemblyFile in coreAssemblyFilesArray)
                        {
                            reflectionOnSeparateAppDomain.LoadAssembly(coreAssemblyFile, loadReferencedAssembliesToo: false, isBridgeBasedVersion: isBridgeBasedVersion, isCoreAssembly: true, nameOfAssembliesThatDoNotContainUserCode: nameOfAssembliesThatDoNotContainUserCode, skipReadingAttributesFromAssemblies: true);
                        }
#endif

                        // Prepare some dictionaries:
                        string[] arrayWithSimpleNameOfAssembliesToIgnore = assembliesToIgnore != null ? assembliesToIgnore.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { };
                        HashSet<string> simpleNameOfAssembliesToIgnore = new HashSet<string>(arrayWithSimpleNameOfAssembliesToIgnore);
                        string[] arrayOfSupportedExtensions = supportedExtensions != null ? supportedExtensions.ToLowerInvariant().Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { };
                        HashSet<string> supportedExtensionsLowercase = new HashSet<string>(arrayOfSupportedExtensions);

                        // Do the extraction and copy:
                        isSuccess = ExtractingAndCopyingResources.Start(
                            sourceAssembly,
                            outputPathAbsolute,
                            outputResourcesPath,
                            simpleNameOfAssembliesToIgnore,
                            supportedExtensionsLowercase,
                            logger,
                            reflectionOnSeparateAppDomain,
                            isBridgeBasedVersion,
                            nameOfAssembliesThatDoNotContainUserCode,
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
    }
}
