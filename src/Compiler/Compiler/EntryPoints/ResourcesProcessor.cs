

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
using System.Linq;
using System.Text;
using System.Threading;

namespace DotNetForHtml5.Compiler
{
    public class ResourcesProcessor : Task
    {
        [Required]
        public string SourceFile { get; set; }

        [Required]
        public string OutputFile { get; set; }

        public override bool Execute()
        {
            return Execute(SourceFile, OutputFile, new LoggerThatUsesTaskOutput(this));
        }


        public static bool Execute(string sourceFile, string outputFile, ILogger logger)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            string operationName = "C#/XAML for HTML5: ResourcesProcessor";
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

                    //escape the code:
                    sourceCode = sourceCode.Replace("\"", "\\\""); // escape the " symbol.
                    sourceCode = sourceCode.Replace("\r", "\\\r"); // escape carriage return.
                    sourceCode = sourceCode.Replace("\n", "\\\n"); // escape line feed.

//                    // Process the code:
//                    sourceCode = string.Format(@"
//<script type=""application/javascript"">
//  window.File{0} = ""{1}"";
//</script>", sourceFile, sourceCode); //sourceFile probably needs to be changed so that we only keep the last part of the name. (we should maybe use outputfile)

                    // Process the code:
                    if (sourceFile.Replace('\\', '/').ToLower().EndsWith("/app.config"))
                    {
                        sourceCode = string.Format(@"window.AppConfig = ""{0}"";", sourceCode); //todo: if needed, make a unique name instead of AppConfig
                    }
                    else if (sourceFile.Replace('\\', '/').ToLower().EndsWith("/servicereferences.clientconfig"))
                    {
                        sourceCode = string.Format(@"window.ServiceReferencesClientConfig = ""{0}"";", sourceCode); //todo: if needed, make a unique name instead of ServiceReferencesClientConfig
                    }
                    else
                    {
                        sourceCode = string.Format(@"window.FileContent = ""{0}"";", sourceCode); //todo: if needed, make a unique name instead of FileContent
                    }


                    // Create output directory:
                    Directory.CreateDirectory(Path.GetDirectoryName(outputFile));

                    // Save output:
                    using (StreamWriter outfile = new StreamWriter(outputFile))
                    {
                        outfile.Write(sourceCode);
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
