

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



using Microsoft.Build.Utilities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DotNetForHtml5.Compiler
{
    internal static class GeneratingSerializationAssemblies
    {
        internal static bool TryGetLocationOfSGenExe(out string sgenFullPath)
        {
            //--------------------------------------
            // Note: "SGen.exe" (x86) is the tool we use to generate the serialization assemblies.
            // Depending on the computer, the location of that file is different.
            // Therefore we need to try looking for it in different locations until we find it.
            //--------------------------------------

            //------------------
            // First, try with the "ToolLocationHelper.GetPathToDotNetFrameworkSdkFile" command, using various .NET target versions:
            //------------------
            sgenFullPath = ToolLocationHelper.GetPathToDotNetFrameworkSdkFile("sgen.exe", TargetDotNetFrameworkVersion.VersionLatest);
            if (!string.IsNullOrEmpty(sgenFullPath) && File.Exists(sgenFullPath))
                return true;
            sgenFullPath = ToolLocationHelper.GetPathToDotNetFrameworkSdkFile("sgen.exe", TargetDotNetFrameworkVersion.Version45);
            if (!string.IsNullOrEmpty(sgenFullPath) && File.Exists(sgenFullPath))
                return true;
            sgenFullPath = ToolLocationHelper.GetPathToDotNetFrameworkSdkFile("sgen.exe", TargetDotNetFrameworkVersion.Version40);
            if (!string.IsNullOrEmpty(sgenFullPath) && File.Exists(sgenFullPath))
                return true;

            //------------------
            // If we still could not find it, we look in the Registry for the SDK location:
            //------------------
            // Note: we need the x86 version of the "SGen.exe" file, because the Compiler is x86.

            // We search in the 64bit-machine registry hive first, and then in the 32bit-machine:
            string[] registryLocationsToSearch = new string[]
            {
                @"SOFTWARE\Wow6432Node\Microsoft\Microsoft SDKs\Windows", // <--- This is for 64-bit machines
                @"SOFTWARE\Microsoft\Microsoft SDKs\Windows" // <--- This is for 32-bit machines
            };
            foreach (string rootRegistryKeyString in registryLocationsToSearch)
            {
                // Get all the subkeys that contain "WinSDK-NetFx40Tools-x86":
                RegistryKey rootRegistryKey = Registry.LocalMachine.OpenSubKey(rootRegistryKeyString);
                List<RegistryKey> subkeysFound = new List<RegistryKey>();
                GetAllSubKeysThatContainKeyword("WinSDK-NetFx40Tools-x86", rootRegistryKey, subkeysFound);

                // Then, order them in counter-alphabetical order so that we start looking for the most recent version:
                var sortedSubKeysFound = subkeysFound.OrderByDescending(registryKey => registryKey.Name);

                // Then, for each of them, look for the "InstallationFolder" value:
                foreach (RegistryKey registryKey in sortedSubKeysFound)
                {
                    object value = registryKey.GetValue("InstallationFolder", null);
                    // Check if the "InstallationFolder" value was found:
                    if (value != null && value is string && !string.IsNullOrEmpty((string)value))
                    {
                        string directory = (string)value;

                        // Check if the folder exists on the disk:
                        if (Directory.Exists(directory))
                        {
                            // Check if the file "sgen.exe" is found:
                            sgenFullPath = Path.Combine(directory, "sgen.exe");
                            if (File.Exists(sgenFullPath))
                                return true;
                        }
                    }
                }
            }

            sgenFullPath = string.Empty;
            return false;
        }

        static void GetSubKeysRecursion(RegistryKey SubKey)
        {
            foreach (string sub in SubKey.GetSubKeyNames())
            {
                RegistryKey local = SubKey.OpenSubKey(sub);
                GetSubKeysRecursion(local); // By recalling itselfit makes sure it get all the subkey names
            }
        }

        static void GetAllSubKeysThatContainKeyword(string keywordToSearchCaseInsensitive, RegistryKey currentKeyForRecursion, List<RegistryKey> result)
        {
            foreach (string sub in currentKeyForRecursion.GetSubKeyNames())
            {
                RegistryKey nextKeyForRecursion = currentKeyForRecursion.OpenSubKey(sub);

                // Add to result if match:
                if (sub.ToLowerInvariant().Contains(keywordToSearchCaseInsensitive.ToLowerInvariant()))
                    result.Add(nextKeyForRecursion);

                // Continue the recursion:
                GetAllSubKeysThatContainKeyword(keywordToSearchCaseInsensitive, nextKeyForRecursion, result);
            }
        }

        internal static bool GenerateSgenCommandLineParameters(string intermediateOutputDirectory, string sourceAssembly, ILogger logger, bool isBridgeBasedVersion, out string sgenCommandLineParameters, out string sgenCommandLineParametersContinued, out bool sgenIsContinued)
        {
            //-------------------------------------------------------------------------------
            // Create or clear the obj/Debug/TempSGen folder
            //-------------------------------------------------------------------------------
            string tempFolderForSerializationAssemblies = GetTempFolderForSerializationAssemblies(intermediateOutputDirectory);
            if (Directory.Exists(tempFolderForSerializationAssemblies))
            {
                foreach (string filePath in Directory.GetFiles(tempFolderForSerializationAssemblies))
                {
                    try
                    {
                        File.Delete(filePath);
                    }
                    catch (Exception ex)
                    {
                        logger.WriteError("Could not delete the following temporary file: " + filePath + "   - Please delete the file manually. If the error persists, please contact support@cshtml5.com - " + ex.Message);
                        sgenCommandLineParameters = null;
                        sgenCommandLineParametersContinued = null;
                        sgenIsContinued = false;
                        return false;
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(tempFolderForSerializationAssemblies);
            }

            //-------------------------------------------------------------------------------
            // Find all the types for which we need to create the serialization assemblies:
            //-------------------------------------------------------------------------------

            string[] typesThatAreSerializable;

            // Create the "TypesResolver" on a separate AppDomain so that the types loaded for reflection can be unloaded when done.
            using (var reflectionOnSeparateAppDomain = new ReflectionOnSeparateAppDomainHandler())
            {
                string assemblySimpleName = reflectionOnSeparateAppDomain.LoadAssembly(sourceAssembly, loadReferencedAssembliesToo: false, isBridgeBasedVersion: isBridgeBasedVersion, isCoreAssembly: false, nameOfAssembliesThatDoNotContainUserCode: "", skipReadingAttributesFromAssemblies: false);
                string commaSeparatedTypesThatAreSerializable = reflectionOnSeparateAppDomain.FindCommaSeparatedTypesThatAreSerializable(assemblySimpleName);
                typesThatAreSerializable = commaSeparatedTypesThatAreSerializable.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }

            //-------------------------------------------------------------------------------
            // Generate the command line call to the "sgen.exe" tool, that will generate the C# source code for the serialization assemblies:
            //-------------------------------------------------------------------------------

            if (typesThatAreSerializable.Length > 0)
            {
                StringBuilder commandLineBuilder = new StringBuilder();
                StringBuilder commandLineBuilderContinued = new StringBuilder();

                string sourceAssemblyAbsolutePath = Path.Combine(Directory.GetCurrentDirectory(), sourceAssembly);

                string shortPathName = ShortPathHelper.GetShortPathName(sourceAssemblyAbsolutePath); // Note: we call "ShortPathHelper.GetShortPathName" so that we don't need to surround the path with double quotes (which don't work with the MSBuild Exec tasc):

                commandLineBuilder.Append(shortPathName);
                commandLineBuilderContinued.Append(shortPathName);

                commandLineBuilder.Append(" /keep"); // Note: "keep" will prevent sgen from deleting the ".cs" source file after generating the serialization assembly
                commandLineBuilderContinued.Append(" /keep");

                commandLineBuilder.Append(" /v"); // Note: "v" will display verbose output for debugging, and will list types from the target assembly that cannot be serialized with the XmlSerializer.
                commandLineBuilderContinued.Append(" /v");

                commandLineBuilder.Append(" /force"); // Note: "force" will force replace the generated assembly if it already exists.
                commandLineBuilderContinued.Append(" /force");

                int charactersCount = 0; // We count the characters because command lines are limited to 32,768 characters.
                sgenIsContinued = false;
                Dictionary<string, string> typesLocalNameToFullName = new Dictionary<string, string>();
                foreach (string serializableType in typesThatAreSerializable)
                {
                    // Verify that there are no 2 classes with the same local name (SGEN.exe does not support processing two classes with the same local name, unless they have some XML attribute to specify the namespace, but the current version of the XmlSerializer does not support such XML namespace attributes:
                    string serializableTypeLocalName = (serializableType.Contains('.') ? serializableType.Substring(serializableType.LastIndexOf('.') + 1) : serializableType);
                    if (typesLocalNameToFullName.ContainsKey(serializableTypeLocalName))
                    {
                        throw new Exception(@"The following serializable classes have the same name: """ + serializableType + @""" and """ + typesLocalNameToFullName[serializableTypeLocalName] + @""". The current version of C#/XAML for HTML5 does not allow serializing two classes that have the same name. This is due to the fact that the XmlSerializer does not support namespaces at the moment. To work around this limitation, please rename one of the two classes, or remove its [Serializable] or [DataContract] attribute.");
                    }
                    else
                    {
                        typesLocalNameToFullName.Add(serializableTypeLocalName, serializableType);
                    }

                    // Build the command line parameter related to the list of types that are serializable:
                    string fragment = " /type:" + serializableType; // Note: the full type name (ie. namespace + name) is required.

                    if (!sgenIsContinued) // This is due to the fact that command lines are limited to 32,768 characters, so we split into two calls if necessary.
                    {
                        commandLineBuilder.Append(fragment);
                    }
                    else
                    {
                        commandLineBuilderContinued.Append(fragment);
                    }

                    charactersCount += fragment.Length;
                    if (charactersCount > 32000)
                    {
                        sgenIsContinued = true;
                    }
                    if (charactersCount > 64000)
                    {
                        throw new Exception("The maximum length of the SGEN command line has been exceeded (64,000 characters). Please reduce the number of serializable types and try again.");
                    }
                }

                string outParam = @" /out:" + ShortPathHelper.GetShortPathName(tempFolderForSerializationAssemblies); // Note: we call "ShortPathHelper.GetShortPathName" so that we don't need to surround the path with double quotes (which don't work with the MSBuild Exec tasc):
                commandLineBuilder.Append(outParam);
                commandLineBuilderContinued.Append(outParam);


                sgenCommandLineParameters = commandLineBuilder.ToString();

                if (sgenIsContinued)
                    sgenCommandLineParametersContinued = commandLineBuilderContinued.ToString();
                else
                    sgenCommandLineParametersContinued = string.Empty;


                // Fix the 8192 characters length limitation (for more information, read the comments in the method "Fix8192charactersIssue"):
                sgenCommandLineParameters = Fix8192charactersIssue(sgenCommandLineParameters);
                sgenCommandLineParametersContinued = Fix8192charactersIssue(sgenCommandLineParametersContinued);
                
            }
            else
            {
                sgenCommandLineParameters = string.Empty;
                sgenCommandLineParametersContinued = string.Empty;
                sgenIsContinued = false;
            }

            logger.WriteMessage("SGEN command line parameters: " + sgenCommandLineParameters, Microsoft.Build.Framework.MessageImportance.Low);
            return true;
        }

        static string Fix8192charactersIssue(string commandLineParameters)
        {
            // Hack: We fix the 8192 characters length limitation, which strips one character at the 8192th position and at every multiple of 8192.
            // To fix it, we double (ie. repeat) the character at that position. For example, if the word "Service" is at that position, it will become "Servicce" with two "c", so that when processed by the MSBuild "Exec" task, it becomes again "Service".
            // Note1: to reproduce the issue, use the "ServiceReferences.zip" file that the user "Harald Mejlholm <harald@mejlholm.dk>" sent us by email on 2015.10.27 (Subject: "Sender: Service References.zip")
            // Note2: you can also reproduce the issue by using the "ServiceReferences.zip" file sent by Michael M. on 2017.04.28 in ZenDesk ticket #586.

            int currentIndex = 0;
            currentIndex += 8192;
            while (currentIndex <= commandLineParameters.Length + 1)
            {
                char characterToRepeat = commandLineParameters[currentIndex - 2];
                commandLineParameters = commandLineParameters.Insert(currentIndex - 1, characterToRepeat.ToString());

                //sgenCommandLineParameters = sgenCommandLineParameters.Insert(currentIndex - 1, "$");

                currentIndex += 8192;
            }

            return commandLineParameters;
        }

        internal static bool ProcessSourceCode(string intermediateOutputDirectory, bool sgenIsContinued, ILogger logger, out string fileToIncludeInProject)
        {
            // Find the source code of the generated serialization assembly:
            string tempFolderForSerializationAssemblies = GetTempFolderForSerializationAssemblies(intermediateOutputDirectory);
            if (!Directory.Exists(tempFolderForSerializationAssemblies))
                throw new Exception("Directory not found: " + tempFolderForSerializationAssemblies);
            var directoryInfo = new DirectoryInfo(tempFolderForSerializationAssemblies);
            var filesThatContainSourceCode = directoryInfo.GetFiles("*.cs", SearchOption.TopDirectoryOnly).ToList();
            if (filesThatContainSourceCode.Count > 1 && !sgenIsContinued)
                throw new Exception("The following folder contains more than one .cs file. It should contain only 1 file. Please report this error to support@cshtml5.com  :  " + tempFolderForSerializationAssemblies);
            else if ((filesThatContainSourceCode.Count == 1 || filesThatContainSourceCode.Count > 2) && sgenIsContinued)
                throw new Exception("The following folder should contain exactly two .cs files. Please report this error to support@cshtml5.com  :  " + tempFolderForSerializationAssemblies);
            if (filesThatContainSourceCode.Count < 1)
                throw new Exception("Error generating serialization assemblies. Please look for details in the Output pane: there should be some warnings before this exception, such as members or types that cannot be serialized. If the problem persists, please report it to support@cshtml5.com -  " + tempFolderForSerializationAssemblies);
            string fileThatContainSourceCode = (filesThatContainSourceCode[0]).FullName; // Note: includes the full path.

            //--------------------------------------------------------------------------
            // Create a copy of the file but where the "AssemblyVersion" attribute has been removed (because it collides with the attribute defined in "assembly.cs"), and that contains other changes as well:
            //--------------------------------------------------------------------------

            // Read the source file:
            List<string> lines = new List<string>(File.ReadAllLines(fileThatContainSourceCode));

            // Remove the "AssemblyVersion" attribute (because it collides with the attribute defined in "assembly.cs"):
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].Contains(@"[assembly:System.Reflection.AssemblyVersionAttribute("))
                    lines[i] = string.Empty;
            }

            // Add the "pragma warning disable" so as to prevent the warnings during compilation:
            lines.Insert(0, "#pragma warning disable 219 // Prevents warning CS0219 ('The variable '...' is assigned but its value is never used')");
            lines.Add("#pragma warning restore 219");

            // Merge the lines:
            string fileContent = string.Join(Environment.NewLine, lines);

            // If there are 2 .cs files, merge them into a single one (this is the case when the "sgenIsContinued" variable is True, which means that the SGEN command line was longer than 32,000 characters and was therefore split into 2 calls to SGEN.exe):
            if (filesThatContainSourceCode.Count > 1)
            {
                // Load the second file:
                string secondFileContent = File.ReadAllText((filesThatContainSourceCode[1]).FullName);

                // Modify the second file to avoid collisions:
                secondFileContent = MergingTwoSerializationAssemblies.ModifySecondFileToAvoidCollisions(secondFileContent);

                // Merge the content of the two files:
                fileContent = MergingTwoSerializationAssemblies.Merge(fileContent, secondFileContent);
            }

            // Save the new file:
            string pathToNewFile = tempFolderForSerializationAssemblies + "sa_" + Guid.NewGuid().ToString("N") + ".cs"; //eg. "sa_14576102e95448d0816762446a865739.cs"
            File.WriteAllText(pathToNewFile, fileContent);

            // Return the name of that file:
            fileToIncludeInProject = pathToNewFile;
            return true;
        }

        static string GetTempFolderForSerializationAssemblies(string intermediateOutputDirectory)
        {
            if (!intermediateOutputDirectory.EndsWith(@"\"))
                intermediateOutputDirectory += @"\";
            return intermediateOutputDirectory + @"TempSGen\";
        }
    }
}
