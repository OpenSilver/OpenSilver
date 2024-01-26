
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
using System.Globalization;
using System.Threading;
using OpenSilver.Compiler.Common;
using ILogger = OpenSilver.Compiler.Common.ILogger;
using System.IO;

namespace OpenSilver.Compiler
{
    public class XamlDesignerAttributesProcessor : Task
    {
        [Required]
        public string AssemblyInclude { get; set; }

        public string AssemblyParameter { get; set; }

        [Required]
        public string OutputFile { get; set; }

        public override bool Execute()
        {
            string generatedCode = @$"
namespace Microsoft.BuildSettings
[<assembly: {AssemblyInclude}(""{AssemblyParameter}"")>]
do ()
";

            // Create output directory:
            Directory.CreateDirectory(Path.GetDirectoryName(OutputFile));

            // Save output:
            using (StreamWriter outfile = new StreamWriter(OutputFile))
            {
                outfile.Write(generatedCode);
            }

            return true;
        }

    }
}
