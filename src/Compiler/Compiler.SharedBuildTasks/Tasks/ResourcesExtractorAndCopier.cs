
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

namespace OpenSilver.Compiler;

public sealed class ResourcesExtractorAndCopier : Task
{
    private readonly string _sourceDir;
    private string _destinationFolder;

    public ResourcesExtractorAndCopier()
    {
        // set the source directory
        _sourceDir = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar;
    }

    [Required]
    public string OutputResourcesPath { get; set; }

    [Required]
    public string DestinationFolder
    {
        get => _destinationFolder;
        set
        {
            string filePath = value;

            // Get the relative path based on sourceDir
            _destinationFolder = TaskHelper.CreateFullFilePath(filePath, _sourceDir);

            // Make sure OutputDir always ends with Path.DirectorySeparatorChar
            if (!_destinationFolder.EndsWith(string.Empty + Path.DirectorySeparatorChar, StringComparison.Ordinal))
            {
                _destinationFolder += Path.DirectorySeparatorChar;
            }
        }
    }

    [Required]
    public ITaskItem[] ResolvedReferences { get; set; }

    [Output]
    public ITaskItem[] CopiedResources { get; set; }

    public override bool Execute()
    {
        const string operationName = "C#/XAML for HTML5: ResourcesExtractorAndCopier";

        if (OutputResourcesPath != "resources/")
        {
            Log.LogMessage(
                MessageImportance.High,
                $"""
                {operationName}: INFO: The resources folder has been overridden. Make sure to change the value of CSHTML5.Internal.StartupAssemblyInfo.OutputResourcesPath accordingly. You can add the following line in the constructor of your Application:
                CSHTML5.Internal.StartupAssemblyInfo.OutputResourcesPath = @"{OutputResourcesPath}";
                """);
        }

        // Validate input strings:
        if (string.IsNullOrEmpty(DestinationFolder))
        {
            Log.LogMessage($"{operationName} failed: '{nameof(DestinationFolder)}' cannot be null or empty.");
            return false;
        }

        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

        Stopwatch watch = Stopwatch.StartNew();

        try
        {
            //------- DISPLAY THE PROGRESS -------
            Log.LogMessage($"{operationName} started.");

            // Create a separate AppDomain so that the types loaded for reflection can be unloaded when done.
            using (var storage = new MonoCecilAssemblyStorage())
            {
                foreach (ITaskItem reference in ResolvedReferences)
                {
                    storage.LoadAssembly(reference.ItemSpec);
                }

                // Do the extraction and copy:
                CopiedResources = ExtractResources(storage).ToArray();
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

    private List<ITaskItem> ExtractResources(MonoCecilAssemblyStorage storage)
    {
        List<ITaskItem> copiedResources = new();

        // Determine the absolute output path:
        string destinationFolder = NormalizeDirectorySeparator(DestinationFolder);

        foreach (AssemblyDefinition asm in storage.Assemblies)
        {
            if (!ShouldExtractResourcesFromAssembly(asm))
            {
                continue;
            }

            uint compatibilityVersion = GetCompatibilityVersion(asm);

            switch (compatibilityVersion)
            {
                case 0:
                    LegacyExtractResourcesFromAssembly(asm, destinationFolder, copiedResources);
                    break;

                default:
                    ExtractResourcesFromAssembly(asm, destinationFolder, copiedResources);
                    break;
            }
        }

        return copiedResources;
    }

    private void LegacyExtractResourcesFromAssembly(AssemblyDefinition asm, string destinationFolder, List<ITaskItem> copiedResources)
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
            string resourcesRootDir = Path.GetFullPath(
                Path.Combine(destinationFolder, NormalizeDirectorySeparator(OutputResourcesPath), assemblyName.ToLowerInvariant()));

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

            copiedResources.Add(new TaskItem(destinationFile));
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

    private void ExtractResourcesFromAssembly(AssemblyDefinition asm, string destinationFolder, List<ITaskItem> copiedResources)
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
                string resourcesRootDir = Path.GetFullPath(Path.Combine(destinationFolder,
                    NormalizeDirectorySeparator(OutputResourcesPath), assemblyName.ToLowerInvariant()));

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

                copiedResources.Add(new TaskItem(destinationFile));
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

    private static string NormalizeDirectorySeparator(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return path;
        }

        var separator = Path.DirectorySeparatorChar;
        var pathFixed = path.Replace('/', separator).Replace('\\', separator);

        if (!pathFixed.EndsWith(separator.ToString()) && pathFixed != "")
        {
            pathFixed += separator;
        }

        return pathFixed;
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
