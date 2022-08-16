

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
using System.Globalization;
using System.IO;
using System.Threading;

namespace DotNetForHtml5.Compiler
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
                if (!Execute(item, OutputFile, new LoggerThatUsesTaskOutput(this)))
                    return false;
            }
            return true;
        }

        public static bool Execute(ITaskItem item, string outputFile, ILogger logger)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            string sourceFile = item.ItemSpec;
            string operationName = "C#/XAML for HTML5: ServiceReferenceFixer";
            try
            {
                // Validate input strings:
                if (string.IsNullOrEmpty(sourceFile))
                    throw new Exception(operationName + " failed because the source file argument is invalid.");
                if (string.IsNullOrEmpty(outputFile))
                    throw new Exception(operationName + " failed because the output file argument is invalid.");

                //------- DISPLAY THE PROGRESS -------
                logger.WriteMessage(operationName + " started for file \"" + sourceFile + "\". Output file: \"" + outputFile + "\"");
                //todo: do not display the output file location?


                // Read file:
                using (StreamReader sr = new StreamReader(sourceFile))
                {
                    String sourceCode = sr.ReadToEnd();
                    bool wasAnythingFixed;

                    // Process the code:
                    sourceCode = FixingServiceReferences.Fix(
                        sourceCode, 
                        item.GetMetadata("ClientBaseToken"), 
                        item.GetMetadata("ClientBaseInterfaceName"), 
                        item.GetMetadata("EndpointCode"), 
                        item.GetMetadata("SoapVersion"), 
                        out wasAnythingFixed);

                    // Create output directory:
                    Directory.CreateDirectory(Path.GetDirectoryName(outputFile));

                    // Save output:
                    using (StreamWriter outfile = new StreamWriter(outputFile))
                    {
                        outfile.Write(sourceCode);
                    }

                    // Display a warning if nothing was fixed:
                    if (!wasAnythingFixed)
                    {
                        //todo: the following message dates back to when the version without the [XmlSerializerFormat] attribute was not supported. We should update this message.
                        logger.WriteWarning("The WCF service may not work as expected when run in the browser. To fix the issue, please add the attribute [XmlSerializerFormat] to the WCF contract class on the server, and then update the Service Reference on the client. Please read the following page for details: http://cshtml5.com/links/wcf-limitations-and-tutorials.aspx");
                    }
                }

                //------- DISPLAY THE PROGRESS -------
                logger.WriteMessage(operationName + " completed.");

                return true;
            }
            catch (Exception ex)
            {
                logger.WriteError(operationName + " failed: " + ex.Message, file: sourceFile);
                return false;
            }
        }
    }
}
