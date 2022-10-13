using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSilver.Simulator.Console
{
    public class FileSource : IMessageSource
    {
        public string Path { get; }
        public int Line { get; }

        public string FunctionName { get; }

        public FileSource(string path, string functionName, int line)
        {
            Path = path;
            FunctionName = functionName;
            Line = line;
        }
    }
}
