using JSIL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Resources;
using System.Text;

namespace DotNetForHtml5.Compiler
{
    internal static class ConvertingResXFilesToJavaScript
    {
        public static void Start(string assemblyPath, string outputPath, HashSet<string> simpleNameOfAssembliesThatContainNoResX, TranslationResult translationResult, ILogger logger, ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain, bool isBridgeBasedVersion)
        {
            List<string> assemblySimpleNames;
            reflectionOnSeparateAppDomain.LoadAssemblyAndAllReferencedAssembliesRecursively(assemblyPath, isBridgeBasedVersion: isBridgeBasedVersion, isCoreAssembly: false, nameOfAssembliesThatDoNotContainUserCode: "", assemblySimpleNames: out assemblySimpleNames);

            foreach (string assemblySimpleName in assemblySimpleNames)
            {
                if (!simpleNameOfAssembliesThatContainNoResX.Contains(assemblySimpleName))
                {
                    Dictionary<string, byte[]> resXFiles = reflectionOnSeparateAppDomain.GetManifestResources(assemblySimpleName,
                        (fn) => fn.ToLower().EndsWith(".resources") && !fn.ToLower().EndsWith(".g.resources"));

                    var encoding = new UTF8Encoding(false);

                    foreach (KeyValuePair<string, byte[]> file in resXFiles)
                    {
                        string fileName = file.Key;
                        byte[] fileContent = file.Value;

                        // Create destination folders hierarchy:


                        // Create JSON from RESX:
                        string json = "";
                        using (Stream stream = new MemoryStream(fileContent))
                        {
                            json = ConvertingResXFilesToJavaScript.ConvertResources(stream, fileName);
                        }

                        /*
                        // Save file:
                        string destinationFile = Path.Combine(outputPath, ResourceCopier.NAME_OF_FOLDER_THAT_CONTAINS_RESOURCES + "\\", Path.GetFileNameWithoutExtension(fileName) + ".resj");
                        string destinationDirectory = Path.GetDirectoryName(destinationFile);
                        Directory.CreateDirectory(destinationDirectory);
                        File.WriteAllText(destinationFile, json);
                        */
                        // Add the file to the "translationResult" of JSIL so that it gets automatically created and added to the output Manifest:
                        string destinationFileName = Path.GetFileNameWithoutExtension(fileName) + ".js";
                        var bytes = encoding.GetBytes(json);
                        translationResult.AddFile(
                            "Script",
                            destinationFileName,
                            new ArraySegment<byte>(bytes));
                    }
                }
            }
        }

        static string ConvertResources(Stream resourceStream, string fileName)
        {
            //-------------------------
            // Original Credits: JSIL.org ("ResourceConverter.cs")
            //-------------------------

            var output = new StringBuilder();

            using (var reader = new ResourceReader(resourceStream))
            {
#region Code added by CSHTML5 team so that the file can be considered by JSIL a "Script" rather than a "Resource". This makes it possible to load it also from the "file:///" protocol.
                output.AppendFormat("$jsilbrowserstate.allAssetNames.push('{0}');", fileName); //todo: change file extension to ".js" here?
                output.AppendLine();
                output.AppendFormat("allAssets[getAssetName('{0}')] =", fileName); //todo: change file extension to ".js" here?
                output.AppendLine();
#endregion
                output.AppendLine("{");

                bool first = true;

                var e = reader.GetEnumerator();
                while (e.MoveNext())
                {
                    if (!first)
                        output.AppendLine(",");
                    else
                        first = false;

                    var key = Convert.ToString(e.Key);
                    output.AppendFormat("    {0}: ", JSIL.Internal.Util.EscapeString(key, forJson: true));

                    var value = e.Value;

                    if (value == null)
                    {
                        output.Append("null");
                    }
                    else
                    {
                        switch (value.GetType().FullName)
                        {
                            case "System.String":
                                output.Append(JSIL.Internal.Util.EscapeString((string)value, forJson: true));
                                break;
                            case "System.Single":
                            case "System.Double":
                            case "System.UInt16":
                            case "System.UInt32":
                            case "System.UInt64":
                            case "System.Int16":
                            case "System.Int32":
                            case "System.Int64":
                                output.Append(Convert.ToString(value));
                                break;
                            default:
                                output.Append(JSIL.Internal.Util.EscapeString(Convert.ToString(value), forJson: true));
                                break;
                        }
                    }
                }

                output.AppendLine();
                output.AppendLine("}");
            }

            return output.ToString();
        }
    }
}
