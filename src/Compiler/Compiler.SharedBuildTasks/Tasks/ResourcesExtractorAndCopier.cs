
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
using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MSTask = Microsoft.Build.Utilities.Task;

namespace OpenSilver.Compiler;

public sealed class ResourcesExtractorAndCopier : MSTask
{
    private const int MaxAttemptsCount = 10;
    private const int MaxWaitTimeSeconds = 5;
    private const string ResourcesCopierHashDictFile = "OpenSilver.ResourcesCopier.json";
    private const string ResourcesCopierLockFile = "OpenSilver.ResourcesCopier.lock";

    private readonly string _sourceDir;
    private string _destinationFolder;
    private string _baseIntermediateOutputPath;

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
    public string BaseIntermediateOutputPath
    {
        get => _baseIntermediateOutputPath;
        set
        {
            string filePath = value;

            // Get the relative path based on sourceDir
            _baseIntermediateOutputPath = TaskHelper.CreateFullFilePath(filePath, _sourceDir);

            // Make sure OutputDir always ends with Path.DirectorySeparatorChar
            if (!_baseIntermediateOutputPath.EndsWith(string.Empty + Path.DirectorySeparatorChar, StringComparison.Ordinal))
            {
                _baseIntermediateOutputPath += Path.DirectorySeparatorChar;
            }
        }
    }

    [Required]
    public int MaxDegreeOfParallelism { get; set; }

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

        if (MaxDegreeOfParallelism == 0 || MaxDegreeOfParallelism < -1)
        {
            Log.LogWarning($"'{MaxDegreeOfParallelism}' is not a valid value for MaxDegreeOfParallelism. Supported values are -1 or non-zero positive integers.");
            MaxDegreeOfParallelism = 1;
        }

        // Validate input strings:
        if (string.IsNullOrEmpty(DestinationFolder))
        {
            Log.LogMessage($"{operationName} failed: '{nameof(DestinationFolder)}' cannot be null or empty.");
            return false;
        }
        if (string.IsNullOrEmpty(BaseIntermediateOutputPath))
        {
            Log.LogMessage($"{operationName} failed: '{nameof(BaseIntermediateOutputPath)}' cannot be null or empty.");
            return false;
        }

        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

        Stopwatch watch = Stopwatch.StartNew();

        var lockFile = Path.Combine(BaseIntermediateOutputPath, ResourcesCopierLockFile);

        for (var i = 0; i < MaxAttemptsCount; i++)
        {
            try
            {
                var success = false;
                using (var fs = new FileStream(lockFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                {
                    try
                    {
                        //------- DISPLAY THE PROGRESS -------
                        Log.LogMessage($"{operationName} started.");

                        // Create a separate AppDomain so that the types loaded for reflection can be unloaded when done.
                        using (var storage = new MonoCecilAssemblyStorage())
                        {
                            foreach (ITaskItem reference in ResolvedReferences)
                            {
                                try
                                {
                                    storage.LoadAssembly(reference.ItemSpec);
                                }
                                catch (Exception ex)
                                {
                                    Log.LogMessage($"Skipped {reference.ItemSpec} - Unable to load assembly. {ex.Message}");
                                }
                            }

                            // Do the extraction and copy:
                            CopiedResources = ExtractResources(storage);
                        }

                        //------- DISPLAY THE PROGRESS -------
                        Log.LogMessage(
                            $"{operationName} completed in {watch.ElapsedMilliseconds} ms.");

                        success = true;
                    }
                    catch (Exception ex)
                    {
                        Log.LogMessage($"{operationName} failed after {watch.ElapsedMilliseconds} ms.");

                        Log.LogErrorFromException(ex, true);
                    }
                }

                File.Delete(lockFile);

                return success;
            }
            catch (IOException)
            {
                // Another process is currently copying resources.
                // We need to wait until it finishes before trying again.
                var directoryPath = Path.GetDirectoryName(lockFile);
                var fileName = Path.GetFileName(lockFile);

                using var fileDeletedEvent = new ManualResetEvent(false);

                using var watcher = new FileSystemWatcher(directoryPath, fileName)
                {
                    NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite
                };

                watcher.Deleted += (object sender, FileSystemEventArgs e) =>
                {
                    fileDeletedEvent.Set();
                };

                // Begin watching.
                watcher.EnableRaisingEvents = true;

                // Maybe already deleted
                if (File.Exists(lockFile))
                {
                    fileDeletedEvent.WaitOne(TimeSpan.FromSeconds(MaxWaitTimeSeconds));
                }
            }
        }

        Log.LogError("Failed to copy resources: Maximum retry attempts exceeded.");
        return false;
    }

    private static void SaveDictionary(ConcurrentDictionary<string, string> dictionary, string filePath)
    {
        var serializer = new DataContractJsonSerializer(typeof(ConcurrentDictionary<string, string>));
        using var xmlWriter = JsonReaderWriterFactory.CreateJsonWriter(new FileStream(filePath, FileMode.Create), Encoding.UTF8, true, true);
        serializer.WriteObject(xmlWriter, dictionary);
    }

    private static ConcurrentDictionary<string, string> LoadDictionary(string filePath)
    {
        if (!File.Exists(filePath))
        {
            // File doesn't exist, so return an empty dictionary
            return new ConcurrentDictionary<string, string>();
        }

        var serializer = new DataContractJsonSerializer(typeof(ConcurrentDictionary<string, string>));
        using var stream = new FileStream(filePath, FileMode.Open);
        return (ConcurrentDictionary<string, string>)serializer.ReadObject(stream)!;
    }

    private ITaskItem[] ExtractResources(MonoCecilAssemblyStorage storage)
    {
        ConcurrentBag<ITaskItem> copiedResources = new();

        var resourcesHashFileName = Path.Combine(BaseIntermediateOutputPath, ResourcesCopierHashDictFile);
        var resourcesHashDict = LoadDictionary(resourcesHashFileName);

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
                    LegacyExtractResourcesFromAssembly(asm, destinationFolder, copiedResources, resourcesHashDict);
                    break;

                default:
                    ExtractResourcesFromAssembly(asm, destinationFolder, copiedResources, resourcesHashDict);
                    break;
            }
        }

        SaveDictionary(resourcesHashDict, resourcesHashFileName);

        return copiedResources.ToArray();
    }

    private string GetHash(Stream stream)
    {
        using (var sha256 = SHA256.Create())
        {
            stream.Position = 0;
            var hashBytes = sha256.ComputeHash(stream);
            var hashString = new StringBuilder();
            foreach (var b in hashBytes)
            {
                hashString.Append(b.ToString("X2", CultureInfo.InvariantCulture));
            }

            stream.Position = 0;

            return hashString.ToString();
        }
    }

    private string GetHash(byte[] data)
    {
        using (var ms = new MemoryStream(data))
        {
            return GetHash(ms);
        }
    }

    private void LegacyExtractResourcesFromAssembly(AssemblyDefinition asm, string destinationFolder, ConcurrentBag<ITaskItem> copiedResources, ConcurrentDictionary<string, string> resourcesHashDict)
    {
        string assemblyName = asm.Name.Name;

        //-----------------------------------------------
        // Process JavaScript, CSS, Image, Video, Audio files:
        //-----------------------------------------------

        // Copy files:
        Parallel.ForEach(GetManifestResources(asm), new ParallelOptions { MaxDegreeOfParallelism = MaxDegreeOfParallelism }, resource =>
        {
            string resourceId = ResourceIDHelper.GetResourceIDFromRelativePath(resource.Name, UriFormat.Unescaped);
            byte[] fileContent = resource.GetResourceData();
            string hash = GetHash(fileContent);

            // Combine the root output path and the relative "resources" folder path, while also ensuring that there is no forward slash, and that the path ends with a backslash:
            string resourcesRootDir = Path.GetFullPath(
                Path.Combine(destinationFolder, NormalizeDirectorySeparator(OutputResourcesPath), assemblyName.ToLowerInvariant()));

            // Create the destination folders hierarchy if it does not already exist:
            string destinationFile = Path.GetFullPath(Path.Combine(resourcesRootDir, resourceId));

            if (!resourcesHashDict.ContainsKey(resourceId) || resourcesHashDict[resourceId] != hash || !File.Exists(destinationFile))
            {
                // If the file is new, has been modified, or the destination file does not exist, copy it

                if (destinationFile.Length >= 256)
                {
                    Log.LogWarning($"Could not create the following output file because its path is too long: {destinationFile}");
                    return;
                }

                if (!destinationFile.StartsWith(resourcesRootDir, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                string destinationDir = Path.GetDirectoryName(destinationFile);
                if (!Directory.Exists(destinationDir))
                {
                    Directory.CreateDirectory(destinationDir);
                }

                // Create the file:
                File.WriteAllBytes(destinationFile, fileContent);

                resourcesHashDict[resourceId] = hash;
            }
            else
            {
                Log.LogMessage($"Skipped {resourceId} - the resource did not change.");
            }

            copiedResources.Add(new TaskItem(TaskHelper.GetRootRelativePath(_sourceDir, destinationFile)));
        });

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

    private void ExtractResourcesFromAssembly(AssemblyDefinition asm, string destinationFolder, ConcurrentBag<ITaskItem> copiedResources, ConcurrentDictionary<string, string> resourcesHashDict)
    {
        if (GetResourceManifest(asm) is not EmbeddedResource manifest)
        {
            return;
        }


        using (var resourceSet = new ResourceSet(manifest.GetResourceStream()))
        {
            string assemblyName = asm.Name.Name;

            Parallel.ForEach(resourceSet.Cast<DictionaryEntry>(), entry =>
            {
                using Stream stream = entry.Value switch
                {
                    Stream embeddedStream => embeddedStream,
                    string sourceFilePath => OpenFile(sourceFilePath),
                    _ => null,
                };

                if (stream is null)
                {
                    return;
                }

                string resourceId = ResourceIDHelper.GetResourceIDFromRelativePath(entry.Key.ToString(), UriFormat.Unescaped);

                string hash = GetHash(stream);

                // Combine the root output path and the relative "resources" folder path, while also ensuring that there is no forward slash, and that the path ends with a backslash:
                string resourcesRootDir = Path.GetFullPath(Path.Combine(destinationFolder,
                    NormalizeDirectorySeparator(OutputResourcesPath), assemblyName.ToLowerInvariant()));

                // Create the destination folders hierarchy if it does not already exist:
                string destinationFile = Path.GetFullPath(Path.Combine(resourcesRootDir, resourceId));

                if (!resourcesHashDict.ContainsKey(resourceId) || resourcesHashDict[resourceId] != hash || !File.Exists(destinationFile))
                {
                    // If the file is new, has been modified, or the destination file does not exist, copy it

                    if (destinationFile.Length >= 256)
                    {
                        Log.LogWarning($"Could not create the following output file because its path is too long: {destinationFile}");
                        return;
                    }

                    if (!destinationFile.StartsWith(resourcesRootDir, StringComparison.OrdinalIgnoreCase))
                    {
                        return;
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

                    resourcesHashDict[resourceId] = hash;
                }
                else
                {
                    Log.LogMessage($"Skipped {resourceId} - the resource did not change.");
                }

                copiedResources.Add(new TaskItem(TaskHelper.GetRootRelativePath(_sourceDir, destinationFile)));
            });
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

        static FileStream OpenFile(string path)
        {
            try
            {
                return File.OpenRead(path);
            }
            catch
            {
                return null;
            }
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
        GetAssemblyName(ca.AttributeType) == "OpenSilver";

    private static bool IsOpenSilverCompatibilityVersionAttribute(CustomAttribute ca) =>
        ca.AttributeType.FullName == "OpenSilver.Runtime.CompilerServices.OpenSilverCompatibilityVersionAttribute" &&
        GetAssemblyName(ca.AttributeType) == "OpenSilver";

    private static bool IsOpenSilverResourceExposureAttribute(CustomAttribute ca) =>
        ca.AttributeType.FullName == "OpenSilver.Runtime.CompilerServices.OpenSilverResourceExposureAttribute" &&
        GetAssemblyName(ca.AttributeType) == "OpenSilver";

    private static string GetAssemblyName(TypeReference typeRef)
    {
        return typeRef.Scope switch
        {
            AssemblyNameReference anr => anr.Name,
            ModuleDefinition md => md.Assembly.Name.Name,
            _ => typeRef.Scope.Name,
        };
    }
}
