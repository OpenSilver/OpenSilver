
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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Threading;
using Mono.Cecil;

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
        public ITaskItem[] ResolvedReferences { get; set; }

        public override bool Execute()
        {
            const string operationName = "C#/XAML for HTML5: ResourcesExtractorAndCopier";

            // Validate input strings:
            if (string.IsNullOrEmpty(SourceAssembly))
            {
                Log.LogMessage($"{operationName} failed: '{nameof(SourceAssembly)}' cannot be null or empty.");
                return false;
            }

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            Stopwatch watch = Stopwatch.StartNew();

            try
            {
                //------- DISPLAY THE PROGRESS -------
                Log.LogMessage($"{operationName} started for assembly '{SourceAssembly}'.");

                // Create a separate AppDomain so that the types loaded for reflection can be unloaded when done.
                using (var storage = new MonoCecilAssemblyStorage())
                {
                    foreach (ITaskItem reference in ResolvedReferences)
                    {
                        storage.LoadAssembly(reference.ItemSpec);
                    }

                    // Do the extraction and copy:
                    ExtractResources(storage);
                }

                //------- DISPLAY THE PROGRESS -------
                Log.LogMessage(
                    $"{operationName} completed in {watch.ElapsedMilliseconds} ms.");

                return true;
            }
            catch (Exception ex)
            {
                Log.LogMessage(
                    MessageImportance.High,
                    $"{operationName} failed after {watch.ElapsedMilliseconds} ms.");
                
                Log.LogErrorFromException(ex, true);

                return false;
            }
        }

        private void ExtractResources(MonoCecilAssemblyStorage storage)
        {
            // Determine the absolute output path:
            string outputPathAbsolute = GetOutputPathAbsolute(SourceAssembly, OutputRootPath);

            foreach (AssemblyDefinition asm in storage.Assemblies)
            {
                if (!ShouldExtractResourcesFromAssembly(asm))
                {
                    continue;
                }

                uint compatiblityVersion = GetCompatibilityVersion(asm);

                switch (compatiblityVersion)
                {
                    case 0:
                        LegacyExtractResourcesFromAssembly(asm, outputPathAbsolute);
                        break;

                    default:
                        ExtractResourcesFromAssembly(asm, outputPathAbsolute);
                        break;
                }
            }
        }

        private void LegacyExtractResourcesFromAssembly(AssemblyDefinition asm, string outputPathAbsolute)
        {
            string assemblyName = asm.Name.Name;

            //-----------------------------------------------
            // Process JavaScript, CSS, Image, Video, Audio files:
            //-----------------------------------------------

            // Copy files:
            foreach (EmbeddedResource resource in GetManifestResources(asm))
            {
                string fileRelativePath = ResourceIDHelper.GetResourceIDFromRelativePath(resource.Name, UriFormat.Unescaped);
                byte[] fileContent = resource.GetResourceData();

                // Combine the root output path and the relative "resources" folder path, while also ensuring that there is no forward slash, and that the path ends with a backslash:
                string resourcesRootDir = Path.GetFullPath(Path.Combine(outputPathAbsolute, OutputResourcesPath, assemblyName.ToLowerInvariant()));

                // Create the destination folders hierarchy if it does not already exist:
                string destinationFile = Path.GetFullPath(Path.Combine(resourcesRootDir, fileRelativePath));

                if (destinationFile.Length >= 256)
                {
                    Log.LogWarning($"Could not create the following output file because its path is too long: {destinationFile}");
                    continue;
                }

                if (!destinationFile.StartsWith(resourcesRootDir, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string destinationDir = Path.GetDirectoryName(destinationFile);
                if (!Directory.Exists(destinationDir))
                {
                    Directory.CreateDirectory(destinationDir);
                }

                // Create the file:
                File.WriteAllBytes(destinationFile, fileContent);
            }

            static IEnumerable<EmbeddedResource> GetManifestResources(AssemblyDefinition asm)
            {
                foreach (Resource resource in asm.MainModule.Resources)
                {
                    if (string.Equals(Path.GetExtension(resource.Name), ".xaml", StringComparison.OrdinalIgnoreCase) ||
                        resource.ResourceType != ResourceType.Embedded)
                    {
                        continue;
                    }

                    yield return (EmbeddedResource)resource;
                }
            }
        }

        private void ExtractResourcesFromAssembly(AssemblyDefinition asm, string outputPathAbsolute)
        {
            if (GetResourceManifest(asm) is not EmbeddedResource manifest)
            {
                return;
            }

            using (var resourceSet = new ResourceSet(manifest.GetResourceStream()))
            {
                string assemblyName = asm.Name.Name;

                var enumerator = resourceSet.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Value is not Stream stream)
                    {
                        continue;
                    }

                    string resourceId = ResourceIDHelper.GetResourceIDFromRelativePath(enumerator.Key.ToString(), UriFormat.Unescaped);

                    // Combine the root output path and the relative "resources" folder path, while also ensuring that there is no forward slash, and that the path ends with a backslash:
                    string resourcesRootDir = Path.GetFullPath(Path.Combine(outputPathAbsolute, OutputResourcesPath, assemblyName.ToLowerInvariant()));

                    // Create the destination folders hierarchy if it does not already exist:
                    string destinationFile = Path.GetFullPath(Path.Combine(resourcesRootDir, resourceId));

                    if (destinationFile.Length >= 256)
                    {
                        Log.LogWarning($"Could not create the following output file because its path is too long: {destinationFile}");
                        continue;
                    }

                    if (!destinationFile.StartsWith(resourcesRootDir, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    string destinationDir = Path.GetDirectoryName(destinationFile);
                    if (!Directory.Exists(destinationDir))
                    {
                        Directory.CreateDirectory(destinationDir);
                    }

                    // Create the file:
                    using (var fs = File.Create(destinationFile))
                    {
                        stream.CopyTo(fs);
                    }
                }
            }

            static EmbeddedResource GetResourceManifest(AssemblyDefinition asm)
            {
                string resourceManifestName = $"{asm.Name.Name}.g.resources";

                if (asm.MainModule.Resources.FirstOrDefault(r => r.Name == resourceManifestName) is Resource manifest)
                {
                    if (manifest.ResourceType == ResourceType.Embedded)
                    {
                        return (EmbeddedResource)manifest;
                    }
                }

                return null;
            }
        }

        private static string GetOutputPathAbsolute(string assemblyFullNameAndPath, string outputRootPath)
        {
            //--------------------------
            // Note: this method is similar to the one in the Simulator.
            // IMPORTANT: If you update this method, make sure to update the other one as well.
            //--------------------------

            var separator = Path.DirectorySeparatorChar;
            var outputRootPathFixed = outputRootPath.Replace('/', separator).Replace('\\', separator);
            if (!outputRootPathFixed.EndsWith(separator.ToString()) && outputRootPathFixed != "")
            {
                outputRootPathFixed += separator;
            }

            // If the path is already ABSOLUTE, we return it directly, otherwise we concatenate it to the path of the assembly:
            string outputPathAbsolute;
            if (Path.IsPathRooted(outputRootPathFixed))
            {
                outputPathAbsolute = outputRootPathFixed;
            }
            else
            {
                outputPathAbsolute = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(assemblyFullNameAndPath)), outputRootPathFixed);

                outputPathAbsolute = outputPathAbsolute.Replace('/', separator).Replace('\\', separator);

                if (!outputPathAbsolute.EndsWith(separator.ToString()) && outputPathAbsolute != "")
                {
                    outputPathAbsolute += separator;
                }
            }

            return outputPathAbsolute;
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

        private static uint GetCompatibilityVersion(AssemblyDefinition asm)
        {
            if (asm.CustomAttributes.FirstOrDefault(IsOpenSilverCompatibilityVersionAttribute) is CustomAttribute ca)
            {
                CustomAttributeArgument arg = ca.ConstructorArguments[0];
                return arg.Value switch
                {
                    uint version => version,
                    string s when uint.TryParse(s, out uint version) => version,
                    _ => 0,
                };
            }

            return 0;
        }

        private static bool IsOpenSilverAssembly(AssemblyDefinition asm) =>
            asm.HasCustomAttributes && asm.CustomAttributes.Any(IsOpenSilverAssemblyAttribute);

        private static bool IsOpenSilverAssemblyAttribute(CustomAttribute ca) =>
            ca.AttributeType.FullName == "OpenSilver.Runtime.CompilerServices.OpenSilverAssemblyAttribute" &&
            ca.AttributeType.Scope.Name == "OpenSilver";

        private static bool IsOpenSilverCompatibilityVersionAttribute(CustomAttribute ca) =>
            ca.AttributeType.FullName == "OpenSilver.Runtime.CompilerServices.OpenSilverCompatibilityVersionAttribute" &&
            ca.AttributeType.Scope.Name == "OpenSilver";

        private static bool IsOpenSilverResourceExposureAttribute(CustomAttribute ca) =>
            ca.AttributeType.FullName == "OpenSilver.Runtime.CompilerServices.OpenSilverResourceExposureAttribute" &&
            ca.AttributeType.Scope.Name == "OpenSilver";
    }
}
