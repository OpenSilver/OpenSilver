using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetForHtml5.EmulatorWithoutJavascript.Console
{
    public class FileSource : IMessageSource
    {
        public string Path { get; }
        public int Line { get; }

        public FileSource(string path, int line)
        {
            Path = path;
            Line = line;
        }
    }
}
