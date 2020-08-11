

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



using Microsoft.Build.BuildEngine;
using System;
using System.Collections.Generic;
using System.IO;

namespace DotNetForHtml5.Compiler
{
    static class CreatingTemporarySolution
    {
        internal static void ReadAllFilesInSCProjAndCopyProcessThem(
            string csProjFilePath,
            string tempFolder,
            ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain,
            List<string> coreAssembliesForUseByXamlToCSharpConverter,
            List<string> coreAssembliesForUseByCSharpToExeOrDllCompiler,
            ILogger logger)
        {
            // Load CSProj:
            Project project = new Project(new Engine());
            project.Load(csProjFilePath);

            // Process files:
            //foreach (BuildItem buildItem in Enumerable.SelectMany<BuildItemGroup, BuildItem, BuildItem>(Enumerable.Cast<BuildItemGroup>((IEnumerable)project.ItemGroups), (Func<BuildItemGroup, IEnumerable<BuildItem>>)(grp => Enumerable.Cast<BuildItem>((IEnumerable)grp)), (Func<BuildItemGroup, BuildItem, BuildItem>)((grp, item) => item)))
            foreach (BuildItemGroup buildItemGroup in project.ItemGroups)
            {
                List<BuildItem> buildItemsToRemove = new List<BuildItem>();

                foreach (BuildItem buildItem in buildItemGroup)
                {
                    if ((buildItem.Name == "Compile" || buildItem.Name == "Content")
                        && !string.IsNullOrEmpty(buildItem.Include)
                        && (buildItem.Include.ToLower().EndsWith(".cs") || buildItem.Include.ToLower().EndsWith(".xaml")))
                    {
                        ProcessFile(buildItem.Include, csProjFilePath, tempFolder, reflectionOnSeparateAppDomain, logger);

                        // In the CSPROJ, rename ".xaml" files to ".xaml.g.cs", and change their BuildAction from "Content" to "Compile"
                        if (buildItem.Include.ToLower().EndsWith(".xaml"))
                        {
                            buildItem.Include = buildItem.Include + ".g.cs";
                            buildItem.Name = "Compile";
                        }
                    }
                    else if (buildItem.Name == "ProjectReference")
                    {
                        foreach (string coreAssemblyFilePath in coreAssembliesForUseByCSharpToExeOrDllCompiler)
                        {
                            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(coreAssemblyFilePath);
                            if (buildItem.Include.EndsWith(fileNameWithoutExtension + ".csproj"))
                                buildItemsToRemove.Add(buildItem);
                        }
                    }
                }

                // Remove build items marked for removal:
                foreach (BuildItem buildItemToRemove in buildItemsToRemove)
                    buildItemGroup.RemoveItem(buildItemToRemove);
            }

            // Add reference to the core assemblies:
            if (coreAssembliesForUseByXamlToCSharpConverter.Count > 0)
            {
                BuildItemGroup newBuildItemGroup = project.AddNewItemGroup();
                foreach (string coreAssemblyFilePath in coreAssembliesForUseByCSharpToExeOrDllCompiler)
                {
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(coreAssemblyFilePath);
                    BuildItem newBuildItem = newBuildItemGroup.AddNewItem("Reference", fileNameWithoutExtension);
                    newBuildItem.SetMetadata("HintPath", coreAssemblyFilePath);
                }
            }

            // Remove the build target, since we are handling the compilation manually:
            Import importToRemove = null;
            foreach (Import import in project.Imports)
	        {
                if (import.ProjectPath.EndsWith("Build.targets"))
                    importToRemove = import;
	        }
            if (importToRemove != null)
                project.Imports.RemoveImport(importToRemove);

            //todo: copy other file types to temp folder (images, etc.) ?

            // Save the modified CSProj to the temp folder:
            project.Save(Path.Combine(tempFolder, Path.GetFileName(csProjFilePath)));
        }

        private static void ProcessFile(string filePathRelativeToProjectFolder, string csprojFilePath, string tempFolder, ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain, ILogger logger)
        {
            if (filePathRelativeToProjectFolder.StartsWith(".") || filePathRelativeToProjectFolder.Contains(":"))
                throw new Exception("The files included in the project must be located inside the project folder.");

            // Calculte absolute source file with path:
            string sourceFileWithAbsolutePath = Path.Combine(Path.GetDirectoryName(csprojFilePath), filePathRelativeToProjectFolder);

            // Calculate absolute dest file with path:
            string destinationFileWithAbsolutePath = Path.Combine(tempFolder, filePathRelativeToProjectFolder);

            // Create destination directory:
            Directory.CreateDirectory(Path.GetDirectoryName(destinationFileWithAbsolutePath));

            // Process file:
            if (filePathRelativeToProjectFolder.ToLower().EndsWith(".cs"))
            {
                ProcessCSharpFile(sourceFileWithAbsolutePath, destinationFileWithAbsolutePath, csprojFilePath, tempFolder, reflectionOnSeparateAppDomain);
            }
            else if (filePathRelativeToProjectFolder.ToLower().EndsWith(".xaml"))
            {
                ProcessXamlFile(sourceFileWithAbsolutePath, destinationFileWithAbsolutePath, csprojFilePath, tempFolder, reflectionOnSeparateAppDomain, logger);
            }
        }

        private static void ProcessCSharpFile(string sourceFileWithAbsolutePath, string destinationFileWithAbsolutePath, string csprojFilePath, string tempFolder, ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain)
        {
            File.Copy(sourceFileWithAbsolutePath, destinationFileWithAbsolutePath, true);
        }

        private static void ProcessXamlFile(string sourceFileWithAbsolutePath, string destinationFileWithAbsolutePath, string csprojFilePath, string tempFolder, ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain, ILogger logger)
        {
            // Read XAML file:
            using (StreamReader sr = new StreamReader(sourceFileWithAbsolutePath))
            {
                String xaml = sr.ReadToEnd();

                // Convert XAML to CS:
                string generatedCode = ConvertingXamlToCSharp.Convert(xaml, sourceFileWithAbsolutePath, "", "", reflectionOnSeparateAppDomain, isFirstPass: false, isSLMigration: false, outputRootPath: "", outputAppFilesPath: "", outputLibrariesPath: "", outputResourcesPath: "", logger: logger); //todo: fill the missing arguments //todo: support first and second pass?

                // Save output:
                using (StreamWriter outfile = new StreamWriter(destinationFileWithAbsolutePath + ".g.cs"))
                {
                    outfile.Write(generatedCode);
                }
            }

            //File.Copy(sourceFileWithAbsolutePath, destinationFileWithAbsolutePath + ".g.cs", true);
        }
    }
}
