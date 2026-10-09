
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
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MSTask = Microsoft.Build.Utilities.Task;

namespace OpenSilver.Compiler
{
    public class ServiceReferenceFixer : MSTask
    {
        [Required]
        public ITaskItem[] SourceFiles { get; set; }

        [Required]
        public string IntermediateOutputPath { get; set; }

        [Required]
        public int MaxDegreeOfParallelism { get; set; }

        [Output]
        public ITaskItem[] GeneratedFiles { get; set; }

        public override bool Execute()
        {
            if (MaxDegreeOfParallelism == 0 || MaxDegreeOfParallelism < -1)
            {
                Log.LogWarning($"'{MaxDegreeOfParallelism}' is not a valid value for MaxDegreeOfParallelism. Supported values are -1 or non-zero positive integers.");
                MaxDegreeOfParallelism = 1;
            }

            if (string.IsNullOrEmpty(IntermediateOutputPath))
            {
                Log.LogError($"OpenSilver: ServiceReferenceFixer failed because the '{nameof(IntermediateOutputPath)}' argument is invalid.");
                return false;
            }

            ITaskItem[] generatedFiles = new ITaskItem[SourceFiles.Length];

            Parallel.For(0, SourceFiles.Length, new ParallelOptions { MaxDegreeOfParallelism = MaxDegreeOfParallelism }, i =>
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

                ITaskItem item = SourceFiles[i];
                ITaskItem generatedFile = ProcessFile(item);
                generatedFiles[i] = generatedFile;
            });

            GeneratedFiles = generatedFiles;

            return !Log.HasLoggedErrors;
        }

        private ITaskItem ProcessFile(ITaskItem item)
        {
            string sourceFile = item.ItemSpec;
            string extension = item.GetMetadata("Extension");

            string operationName;
            if (string.Equals(extension, ".cs", StringComparison.OrdinalIgnoreCase))
            {
                operationName = "OpenSilver: ServiceReferenceFixer (C#)";
            }
            else if (string.Equals(extension, ".vb", StringComparison.OrdinalIgnoreCase))
            {
                operationName = "OpenSilver: ServiceReferenceFixer (VB)";
            }
            else
            {
                operationName = "OpenSilver: ServiceReferenceFixer (F#)";
            }

            if (string.IsNullOrEmpty(sourceFile))
            {
                Log.LogError($"{operationName} failed because the source file argument is invalid.");
                return item;
            }

            try
            {
                Log.LogMessage($"{operationName} started for file '{sourceFile}'.");

                string outputFilePath = GetOutputFile(item);

                using (var sr = new StreamReader(sourceFile))
                {
                    string sourceCode = sr.ReadToEnd();
                    bool wasAnythingFixed;

                    // Process the code:
                    if (string.Equals(extension, ".cs", StringComparison.OrdinalIgnoreCase))
                    {
                        sourceCode = FixingServiceReferences.Fix(
                            sourceCode,
                            item.GetMetadata("ClientBaseToken"),
                            item.GetMetadata("ClientBaseInterfaceName"),
                            item.GetMetadata("EndpointCode"),
                            item.GetMetadata("SoapVersion"),
                            out wasAnythingFixed);
                    }
                    else if (string.Equals(extension, ".vb", StringComparison.OrdinalIgnoreCase))
                    {
                        sourceCode = FixingServiceReferencesVB.Fix(
                            sourceCode,
                            item.GetMetadata("ClientBaseToken"),
                            item.GetMetadata("ClientBaseInterfaceName"),
                            item.GetMetadata("EndpointCode"),
                            item.GetMetadata("SoapVersion"),
                            out wasAnythingFixed);
                    }
                    else
                    {
                        Log.LogError("The compiler doesn't support this file.");
                        return item;
                    }

                    Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath));
                    using (var outfile = new StreamWriter(outputFilePath))
                    {
                        outfile.Write(sourceCode);
                    }

                    // Display a warning if nothing was fixed:
                    if (!wasAnythingFixed)
                    {
                        //todo: the following message dates back to when the version without the [XmlSerializerFormat] attribute was not supported. We should update this message.
                        Log.LogWarning(
                            "The WCF service may not work as expected when run in the browser. To fix the issue, please add the attribute [XmlSerializerFormat] to the WCF contract class on the server, and then update the Service Reference on the client. Please read the following page for details: http://cshtml5.com/links/wcf-limitations-and-tutorials.aspx");
                    }
                }

                Log.LogMessage($"  {GetFileIdentity(item)} -> {outputFilePath}.");

                return new TaskItem(outputFilePath);
            }
            catch (Exception ex)
            {
                Log.LogMessage(MessageImportance.High, $"{operationName} failed.");
                Log.LogErrorFromException(ex, true, false, sourceFile);
                return item;
            }
        }

        private string GetOutputFile(ITaskItem item) => Path.Combine(IntermediateOutputPath, GetFileName(item));

        private string GetFileName(ITaskItem item)
        {
            string fileIdentity = GetFileIdentity(item);
            string fileExtension = item.GetMetadata("Extension");
            return Path.ChangeExtension(fileIdentity, $"g{fileExtension}");
        }

        private static string GetFileIdentity(ITaskItem item)
        {
            string identity = item.GetMetadata("Link");
            if (string.IsNullOrEmpty(identity))
            {
                identity = item.GetMetadata("Identity");
            }
            return identity;
        }
    }
}
