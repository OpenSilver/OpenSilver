
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

namespace OpenSilver.Compiler.Resources
{
    public class ResourcesExtractorAndCopier : Task
    {
        public readonly ILogger _logger;

        public ResourcesExtractorAndCopier()
        {
            _logger = new LoggerThatUsesTaskOutput(this);
        }

        [Required]
        public string SourceAssembly { get; set; }

        [Required]
        public string OutputRootPath { get; set; }

        [Required]
        public string OutputResourcesPath { get; set; }

        [Required]
        public ITaskItem[] ResolvedReferences { get; set; }

        public override bool Execute()
        {
            const string operationName = "C#/XAML for HTML5: ResourcesExtractorAndCopier";

            // Validate input strings:
            if (string.IsNullOrEmpty(SourceAssembly))
            {
                _logger.WriteMessage($"{operationName} failed: '{nameof(SourceAssembly)}' cannot be null or empty.");
                return false;
            }

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            using (var executionTimeMeasuring = new ExecutionTimeMeasuring())
            {
                try
                {
                    //------- DISPLAY THE PROGRESS -------
                    _logger.WriteMessage($"{operationName} started for assembly '{SourceAssembly}'.");

                    // Determine the absolute output path:
                    string outputPathAbsolute = PathsHelper.GetOutputPathAbsolute(SourceAssembly, OutputRootPath);

                    // Create a separate AppDomain so that the types loaded for reflection can be unloaded when done.
                    using (var storage = new MonoCecilAssemblyStorage())
                    {
                        foreach (ITaskItem reference in ResolvedReferences)
                        {
                            storage.LoadAssembly(reference.GetMetadata("Identity"));
                        }

                        // Do the extraction and copy:
                        ExtractResources(
                            outputPathAbsolute,
                            OutputResourcesPath,
                            _logger,
                            storage);
                    }

                    //------- DISPLAY THE PROGRESS -------
                    _logger.WriteMessage(
                        $"{operationName} completed in {executionTimeMeasuring.StopAndGetElapsedTime().TotalMilliseconds} ms.");

                    return true;
                }
                catch (Exception ex)
                {
                    _logger.WriteMessage(
                        $"{operationName} failed after {executionTimeMeasuring.StopAndGetElapsedTime().TotalMilliseconds} ms: {ex}");

                    return false;
                }
            }
        }

        private static void ExtractResources(
            string outputPathAbsolute,
            string outputResourcesPath,
            ILogger logger,
            MonoCecilAssemblyStorage storage)
        {
            foreach (AssemblyDefinition asm in storage.Assemblies)
            {
                if (!ShouldExtractResourcesFromAssembly(asm))
                {
                    continue;
                }

                string assemblyName = asm.Name.Name;

                //-----------------------------------------------
                // Process JavaScript, CSS, Image, Video, Audio files:
                //-----------------------------------------------

                // Copy files:
                foreach (EmbeddedResource resource in GetManifestResources(asm))
                {
                    string fileName = resource.Name.ToLowerInvariant();
                    byte[] fileContent = resource.GetResourceData();

                    // Combine the root output path and the relative "resources" folder path, while also ensuring that there is no forward slash, and that the path ends with a backslash:
                    string absoluteOutputResourcesPath = PathsHelper.CombinePathsWhileEnsuringEndingBackslashAndMore(outputPathAbsolute, outputResourcesPath);

                    // Create the destination folders hierarchy if it does not already exist:
                    string destinationFile = Path.Combine(absoluteOutputResourcesPath, assemblyName.ToLowerInvariant(), fileName);
                    if (destinationFile.Length < 256)
                    {
                        string destinationDirectory = Path.GetDirectoryName(destinationFile);
                        if (!Directory.Exists(destinationDirectory))
                        {
                            Directory.CreateDirectory(destinationDirectory);
                        }

                        // Create the file:
                        File.WriteAllBytes(destinationFile, fileContent);
                    }
                    else
                    {
                        logger.WriteWarning("Could not create the following output file because its path is too long: " + destinationFile);
                    }
                }
            }
        }

        internal static List<EmbeddedResource> GetManifestResources(AssemblyDefinition asm)
        {
            var resources = new List<EmbeddedResource>();

            foreach (Resource resource in asm.MainModule.Resources)
            {
                if (string.Equals(Path.GetExtension(resource.Name), ".xaml", StringComparison.OrdinalIgnoreCase) ||
                    resource.ResourceType != ResourceType.Embedded)
                {
                    continue;
                }

                resources.Add((EmbeddedResource)resource);
            }

            return resources;
        }

        private static bool ShouldExtractResourcesFromAssembly(AssemblyDefinition asm)
        {
            if (!IsOpenSilverAssembly(asm))
            {
                return false;
            }

            CustomAttribute ca = asm.CustomAttributes.FirstOrDefault(IsOpenSilverResourceExposureAttribute);

            if (ca is not null)
            {
                CustomAttributeArgument arg = ca.ConstructorArguments[0];
                return arg.Value switch
                {
                    bool b => b,
                    string s when bool.TryParse(s, out bool b) => b,
                    _ => false,
                };
            }

            return true;
        }

        private static bool IsOpenSilverAssembly(AssemblyDefinition asm) =>
            asm.HasCustomAttributes && asm.CustomAttributes.Any(IsOpenSilverAssemblyAttribute);

        private static bool IsOpenSilverAssemblyAttribute(CustomAttribute ca) =>
            ca.AttributeType.FullName == "OpenSilver.Runtime.CompilerServices.OpenSilverAssemblyAttribute" &&
            ca.AttributeType.Scope.Name == "OpenSilver";

        private static bool IsOpenSilverResourceExposureAttribute(CustomAttribute ca) =>
            ca.AttributeType.FullName == "OpenSilver.Runtime.CompilerServices.OpenSilverResourceExposureAttribute" &&
            ca.AttributeType.Scope.Name == "OpenSilver";
    }
}
