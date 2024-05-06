
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

namespace OpenSilver.Compiler
{
    public class ServiceReferenceFixer : Task
    {
        [Required]
        public ITaskItem[] SourceFile { get; set; }

        [Required]
        public string OutputFile { get; set; }

        public override bool Execute()
        {
            foreach (ITaskItem item in SourceFile)
            {
                if (!ProcessItem(item))
                {
                    return false;
                }
            }

            return true;
        }

        public bool ProcessItem(ITaskItem item)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            string sourceFile = item.ItemSpec;
            string operationName;
            if (OutputFile.EndsWith(".cs"))
            {
                operationName = "C#/XAML for HTML5: ServiceReferenceFixer";
            }
            else if (OutputFile.EndsWith(".vb"))
            {
                operationName = "VB.Net/XAML for HTML5: ServiceReferenceFixer";
            }
            else
            {
                operationName = "F#/XAML for HTML5: ServiceReferenceFixer";
            }

            try
            {
                // Validate input strings:
                if (string.IsNullOrEmpty(sourceFile))
                {
                    Log.LogError($"{operationName} failed because the source file argument is invalid.");
                    return false;
                }
                if (string.IsNullOrEmpty(OutputFile))
                {
                    Log.LogError($"{operationName} failed because the '{nameof(OutputFile)}' argument is invalid.");
                    return false;
                }

                //------- DISPLAY THE PROGRESS -------
                Log.LogMessage($"{operationName} started for file \"{sourceFile}\". Output file: \"{OutputFile}\"");
                //todo: do not display the output file location?

                // Read file:
                using (var sr = new StreamReader(sourceFile))
                {
                    string sourceCode = sr.ReadToEnd();
                    bool wasAnythingFixed;

                    // Process the code:
                    if (OutputFile.EndsWith(".cs"))
                    {
                        sourceCode = FixingServiceReferences.Fix(
                            sourceCode,
                            item.GetMetadata("ClientBaseToken"),
                            item.GetMetadata("ClientBaseInterfaceName"),
                            item.GetMetadata("EndpointCode"),
                            item.GetMetadata("SoapVersion"),
                            out wasAnythingFixed);
                    }
                    else if (OutputFile.EndsWith(".vb"))
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
                        return false;
                    }

                    // Create output directory:
                    Directory.CreateDirectory(Path.GetDirectoryName(OutputFile));

                    // Save output:
                    using (var outfile = new StreamWriter(OutputFile))
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

                //------- DISPLAY THE PROGRESS -------
                Log.LogMessage($"{operationName} completed.");

                return true;
            }
            catch (Exception ex)
            {
                Log.LogMessage(MessageImportance.High, $"{operationName} failed.");
                Log.LogErrorFromException(ex, true, false, sourceFile);
                return false;
            }
        }
    }
}
