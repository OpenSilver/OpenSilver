using DotNetForHtml5.Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypeScriptDefToCSharp.Model;

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
