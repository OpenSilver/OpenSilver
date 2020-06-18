using JSIL;
using Mono.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DotNetForHtml5.Compiler
{
    class TranspilingOnlyAssembliesThatHaveChanged
    {
        const string LastCompilationInfoFileName = "LastCompilationInfo_01.xml";

        public static List<string> GetSimpleNameOfAssembliesToSkip(string intermediateOutputAbsolutePath)
        {
            // Prepare the collection that will contain the list of assemblies that have remained the same, ie. that do not need to be transpiled again:
            List<string> result = new List<string>();

            // Calculate the path of the file that contains information about the last compilation:
            string intermediateOutputAbsolutePathFixed = PathsHelper.EnsureNoForwardSlashAndEnsureItEndsWithABackslash(intermediateOutputAbsolutePath);
            string lastCompilationInfoFileNameWithPath = Path.Combine(intermediateOutputAbsolutePathFixed, LastCompilationInfoFileName);

            // Check if the file exits:
            if (File.Exists(lastCompilationInfoFileNameWithPath))
            {
                // Read the file:
                var serializedLastCompilationInfo = File.ReadAllText(lastCompilationInfoFileNameWithPath);

                // Deserialize the data strucutre that contains information about the last compilation:
                LastCompilationInfo lastCompilationInfo = DeserializeFromString<LastCompilationInfo>(serializedLastCompilationInfo);

                // Check if the compiler version has remained the same (otherwise we need to re-compile everything):
                if (lastCompilationInfo.CompilerVersionNumber == VersionInformation.GetCurrentVersionNumber().ToString())
                {
                    // Check which assemblies have changes since the last compilation. This is useful bacause we do not want to transpile (ie. convert to JavaScript) the assemblies that were already transpiled and that have not changed.
                    foreach (TranspiledAssemblyInfo transpiledAssemblyInfo in lastCompilationInfo.TranspiledAssembliesInfo)
                    {
                        // Check if the assembly has changed since the last time that it was transpiled:
                        if (CheckIfAssemblyHasRemainedTheSame(transpiledAssemblyInfo))
                        {
                            result.Add(transpiledAssemblyInfo.AssemblySimpleName);
                        }
                    }
                }
            }

            return result;
        }

        public static void RememberAssembliesThatHaveBeenTranspiled(TranslationResult translationResult, string sourceAssembly, string outputAppFilesPath, string outputRootPath, string intermediateOutputAbsolutePath)
        {
            // Calculate the file output path:
            string outputPathAbsolute = PathsHelper.GetOutputPathAbsolute(sourceAssembly, outputRootPath);
            string outputAppPathAbsolute = PathsHelper.CombinePathsWhileEnsuringEndingBackslashAndMore(outputPathAbsolute, outputAppFilesPath);
            string intermediateOutputAbsolutePathFixed = PathsHelper.EnsureNoForwardSlashAndEnsureItEndsWithABackslash(intermediateOutputAbsolutePath);
            string lastCompilationInfoFileNameWithPath = Path.Combine(intermediateOutputAbsolutePathFixed, LastCompilationInfoFileName);

            // Create the data structure that will store information about the last compilation:
            LastCompilationInfo lastCompilationInfo = CreateLastCompilationInfo(translationResult, outputAppPathAbsolute);
            
            // Serialize the data:
            string serializedLastCompilationInfo = SerializeToString<LastCompilationInfo>(lastCompilationInfo);

            // Save the data to a file on the disk:
            File.WriteAllText(lastCompilationInfoFileNameWithPath, serializedLastCompilationInfo);
        }

        static bool CheckIfAssemblyHasRemainedTheSame(TranspiledAssemblyInfo transpiledAssemblyInfo)
        {
            bool result = false;

            // Check if both the assembly and the produced JavaScript still exist:
            if (File.Exists(transpiledAssemblyInfo.OriginalDLLFileNameWithPath)
                && File.Exists(transpiledAssemblyInfo.ProducedJavaScriptFileNameWithPath))
            {
                // Check if the time stamp of the source assembly and the time stamp of the produced JavaScript have changed:
                if (GetFileTimeStampString(transpiledAssemblyInfo.OriginalDLLFileNameWithPath) == transpiledAssemblyInfo.OriginalDLLTimeStampString
                    && GetFileTimeStampString(transpiledAssemblyInfo.ProducedJavaScriptFileNameWithPath) == transpiledAssemblyInfo.ProducedJavaScriptFileTimeStampString)
                {
                    // Check if the size of the source assembly and the size of the produced JavaScript have changed:
                    if (GetFileSizeInBytes(transpiledAssemblyInfo.OriginalDLLFileNameWithPath) == transpiledAssemblyInfo.OriginalDLLSizeInBytes
                        && GetFileSizeInBytes(transpiledAssemblyInfo.ProducedJavaScriptFileNameWithPath) == transpiledAssemblyInfo.ProducedJavaScriptFileSizeInBytes)
                    {
                        result = true;
                    }
                }
            }

            return result;            
        }

        static LastCompilationInfo CreateLastCompilationInfo(TranslationResult translationResult, string outputAppPathAbsolute)
        {
            LastCompilationInfo result = new LastCompilationInfo();
            result.CompilerVersionNumber = VersionInformation.GetCurrentVersionNumber().ToString();
            result.TranspiledAssembliesInfo = new List<TranspiledAssemblyInfo>();

            foreach (Mono.Cecil.AssemblyDefinition assemblyDefinition in translationResult.Assemblies)
            {
                // Look for the produced JavaScript file that has the name of the assembly:
                var producedJavaScriptFileWithPath = (translationResult.Files.FirstOrDefault<KeyValuePair<string, TranslationResult.ResultFile>>((res) =>
                    {
                        return string.Equals(res.Key, assemblyDefinition.Name.Name + ".js", StringComparison.InvariantCultureIgnoreCase);
                    })).Value.Filename;
                if (!string.IsNullOrEmpty(producedJavaScriptFileWithPath))
                {
                    // Compute the full path of the producted JavaScript file:
                    producedJavaScriptFileWithPath = Path.Combine(outputAppPathAbsolute, producedJavaScriptFileWithPath);

                    // Get the path of the transpiled assembly:
                    string originalDLLFileNameWithPath = assemblyDefinition.MainModule.FullyQualifiedName;

                    // Verify that both the transpiled assembly and the generated JavaScript files are found on the disk:
                    if (File.Exists(producedJavaScriptFileWithPath)
                        && File.Exists(originalDLLFileNameWithPath))
                    {
                        // Get the timestamp and size in bytes of the transpiled assembly:
                        string originalDLLTimeStampString = GetFileTimeStampString(originalDLLFileNameWithPath);
                        string originalDLLFileSizeInBytes = GetFileSizeInBytes(originalDLLFileNameWithPath);

                        // Get the timestamp and size in bytes of the produced JavaScript file:
                        string producedJavaScriptFileTimeStampString = GetFileTimeStampString(producedJavaScriptFileWithPath);
                        string producedJavaScriptFileSizeInBytes = GetFileSizeInBytes(producedJavaScriptFileWithPath);

                        // Store this information so that, later, we can compare it to avoid transpiling the assemblies that were not modified:
                        var transpiledAssemblyInfo = new TranspiledAssemblyInfo()
                        {
                            AssemblySimpleName = assemblyDefinition.Name.Name,
                            OriginalDLLFileNameWithPath = originalDLLFileNameWithPath,
                            OriginalDLLTimeStampString = originalDLLTimeStampString,
                            OriginalDLLSizeInBytes = originalDLLFileSizeInBytes,
                            ProducedJavaScriptFileNameWithPath = producedJavaScriptFileWithPath,
                            ProducedJavaScriptFileTimeStampString = producedJavaScriptFileTimeStampString,
                            ProducedJavaScriptFileSizeInBytes = producedJavaScriptFileSizeInBytes
                        };
                        result.TranspiledAssembliesInfo.Add(transpiledAssemblyInfo);
                    }
                }
            }

            return result;
        }

        static string SerializeToString<T>(T obj)
        {
            using (var output = new StringWriter())
            {
                using (var writer = new XmlTextWriter(output) { Formatting = Formatting.Indented })
                {
                    var dataContractSerializer = new DataContractSerializer(typeof(T));
                    dataContractSerializer.WriteObject(writer, obj);
                    return output.GetStringBuilder().ToString();
                }
            }
        }

        static T DeserializeFromString<T>(string str)
        {
            using (Stream stream = new MemoryStream())
            {
                byte[] data = System.Text.Encoding.UTF8.GetBytes(str);
                stream.Write(data, 0, data.Length);
                stream.Position = 0;
                var dataContractSerializer = new DataContractSerializer(typeof(T));
                return (T)dataContractSerializer.ReadObject(stream);
            }
        }

        static string GetFileTimeStampString(string fileNameWithPath)
        {
            // Get the time stamp of the specified file:
            DateTime timeStamp = File.GetLastWriteTime(fileNameWithPath);
            string timeStampString = ConvertDateTimeToUniversalString(timeStamp);
            return timeStampString;
        }

        static string GetFileSizeInBytes(string fileNameWithPath)
        {
            // Get the size in bytes of the specified file:
            var fileInfo = new FileInfo(fileNameWithPath);
            long fileSizeInBytes = fileInfo.Length;
            string fileSizeInBytesString = fileSizeInBytes.ToString();
            return fileSizeInBytesString;
        }

        static string ConvertDateTimeToUniversalString(DateTime dateTime)
        {
            return dateTime.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
        }
    }

    public class LastCompilationInfo
    {
        public List<TranspiledAssemblyInfo> TranspiledAssembliesInfo { get; set; }
        public string CompilerVersionNumber { get; set; }
    }

    public class TranspiledAssemblyInfo
    {
        public string OriginalDLLFileNameWithPath { get; set; }
        public string OriginalDLLTimeStampString { get; set; }
        public string OriginalDLLSizeInBytes { get; set; }
        public string ProducedJavaScriptFileNameWithPath { get; set; }
        public string ProducedJavaScriptFileTimeStampString { get; set; }
        public string ProducedJavaScriptFileSizeInBytes { get; set; }
        public string AssemblySimpleName { get; set; }
    }
}
