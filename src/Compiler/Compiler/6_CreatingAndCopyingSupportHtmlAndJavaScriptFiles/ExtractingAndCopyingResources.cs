

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
    internal static class ExtractingAndCopyingResources
    {
        public static bool Start(string assemblyPath, string outputPathAbsolute, string outputResourcesPath, HashSet<string> simpleNameOfAssembliesToIgnore, HashSet<string> supportedExtensionsLowercase, ILogger logger, ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain, bool isBridgeBasedVersion, string nameOfAssembliesThatDoNotContainUserCode, out List<string> resXFilesCopied)
        {
            resXFilesCopied = new List<string>();
            List<string> assemblySimpleNames;
            reflectionOnSeparateAppDomain.LoadAssemblyAndAllReferencedAssembliesRecursively(
                assemblyPath,
                isBridgeBasedVersion: isBridgeBasedVersion,
                isCoreAssembly: false,
                nameOfAssembliesThatDoNotContainUserCode: nameOfAssembliesThatDoNotContainUserCode,
                skipReadingAttributesFromAssemblies: true,
                assemblySimpleNames: out assemblySimpleNames);

            foreach (string assemblySimpleName in assemblySimpleNames)
            {
                if (!simpleNameOfAssembliesToIgnore.Contains(assemblySimpleName))
                {
                    //-----------------------------------------------
                    // Process JavaScript, CSS, Image, Video, Audio files:
                    //-----------------------------------------------

#if !CSHTML5BLAZOR
                    Dictionary<string, byte[]> jsAndCssFiles = reflectionOnSeparateAppDomain.GetResources(assemblySimpleName, supportedExtensionsLowercase);
#else
                    Dictionary<string, byte[]> jsAndCssFiles = reflectionOnSeparateAppDomain.GetManifestResources(assemblySimpleName, supportedExtensionsLowercase);
#endif
                    // Copy files:
                    foreach (KeyValuePair<string, byte[]> file in jsAndCssFiles)
                    {
                        string fileName = file.Key;
                        byte[] fileContent = file.Value;

                        // Combine the root output path and the relative "resources" folder path, while also ensuring that there is no forward slash, and that the path ends with a backslash:
                        string absoluteOutputResourcesPath = PathsHelper.CombinePathsWhileEnsuringEndingBackslashAndMore(outputPathAbsolute, outputResourcesPath);

                        // Create the destination folders hierarchy if it does not already exist:
                        string destinationFile = Path.Combine(absoluteOutputResourcesPath, assemblySimpleName + "\\", fileName);
                        if (destinationFile.Length < 256)
                        {
                            string destinationDirectory = Path.GetDirectoryName(destinationFile);
                            if (!Directory.Exists(destinationDirectory))
                                Directory.CreateDirectory(destinationDirectory);

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


                    #region Commented because this operation is now done in the "JavaScriptGenerator" build task ("ConvertingResXFilesToJavaScript.cs").
                    ////-----------------------------------------------
                    //// Process RESX files:
                    ////-----------------------------------------------

                    //Dictionary<string, byte[]> resXFiles = reflectionOnSeparateAppDomain.GetManifestResources(assemblySimpleName,
                    //    (fn) => fn.ToLower().EndsWith(".resources") && !fn.ToLower().EndsWith(".g.resources"));

                    //foreach (KeyValuePair<string, byte[]> file in resXFiles)
                    //{
                    //    string fileName = file.Key;
                    //    byte[] fileContent = file.Value;

                    //    // Create destination folders hierarchy:
                    //    string destinationFile = Path.Combine(outputPath, ResourceCopier.NAME_OF_FOLDER_THAT_CONTAINS_RESOURCES + "\\", Path.GetFileNameWithoutExtension(fileName) + ".resj");
                    //    string destinationDirectory = Path.GetDirectoryName(destinationFile);
                    //    Directory.CreateDirectory(destinationDirectory);

                    //    // Create JSON from RESX:
                    //    string json = "";
                    //    using (Stream stream = new MemoryStream(fileContent))
                    //    {
                    //        json = ConvertingResXFilesToJavaScript.ConvertResources(stream);
                    //    }

                    //    // Save file:
                    //    File.WriteAllText(destinationFile, json);
                    //}
                    #endregion
                }
            }

            return true;
        }
    }
}
