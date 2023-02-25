
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

using System.Collections.Generic;
using ILogger = OpenSilver.Compiler.Common.ILogger;

namespace TypeScriptDefToCSharp
{
    public class TypeScriptDefContext
    {
        // Directory where the files are outputed (obj directory)
        public string OutputDirectory { get; set; }

        public ILogger Logger { get; set; }

        // List of generated files to add them for compilation
        public List<string> CurrentGeneratedFiles { get; set; }

        public TypeScriptDefContext()
        {
            CurrentGeneratedFiles = new List<string>();
        }
    }
}
