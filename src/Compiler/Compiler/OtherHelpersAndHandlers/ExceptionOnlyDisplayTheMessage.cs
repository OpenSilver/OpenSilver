using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetForHtml5.Compiler
{
    internal class CompilationExceptionWithOptions : Exception
    {
        public CompilationExceptionWithOptions(string message) :
            base(message)
        {
        }

        public bool DisplayOnlyTheMessageInTheOutputNothingElse { get; set; }
    }
}
