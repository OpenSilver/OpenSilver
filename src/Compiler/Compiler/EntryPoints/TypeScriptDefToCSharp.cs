

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
using System.Threading;
using TypeScriptDefToCSharp;

namespace DotNetForHtml5.Compiler
{
    public class TypeScriptDefToCSharp : Task
    {
        public string InputFiles { get; set; }

        [Required]
        public string OutputDirectory { get; set; }

        [Output]
        public ITaskItem[] GeneratedFiles { get; set; }

        public bool NoRecompile { get; set; }

        public override bool Execute()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            if (InputFiles == null)
                return true;
            ILogger logger = new LoggerThatUsesTaskOutput(this);
            string operationName = "C#/XAML for HTML5: TypeScriptDefToCSharp";
            try
            {
                // Execute the task if needed
                if (NoRecompile == false)
                    ProcessTypeScriptDef.ProcessTypeScriptDefFile(this.InputFiles, this.OutputDirectory, logger);

                // Get the file list from the XML
                List<string> ListOfGeneratedFiles = new List<string>();
                if (File.Exists(OutputDirectory + global::TypeScriptDefToCSharp.Constants.NAME_OF_TYPESCRIPT_DEFINITIONS_CACHE_FILE))
                {
                    // Read the XML
                    string content = File.ReadAllText(OutputDirectory + global::TypeScriptDefToCSharp.Constants.NAME_OF_TYPESCRIPT_DEFINITIONS_CACHE_FILE);
                    // Deserialize
                    TypeScriptDefToCSharpOutput output = Tool.Deserialize<TypeScriptDefToCSharpOutput>(content);

                    // Store every generated file in our list
                    foreach (var file in output.TypeScriptDefinitionFiles)
                    {
                        ListOfGeneratedFiles.AddRange(file.CSharpGeneratedFiles);
                    }
                }
                // Convert them to get the output
                GeneratedFiles = ListOfGeneratedFiles.Select(s => new TaskItem() { ItemSpec = s }).ToArray();

                logger.WriteMessage("Output directory: " + this.OutputDirectory, MessageImportance.High);
            }
            catch (Exception ex)
            {
                logger.WriteError(operationName + " failed: " + ex.ToString());
                return false;
            }
            return true;
        }
    }
}
