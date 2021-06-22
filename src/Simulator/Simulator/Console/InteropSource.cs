using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetForHtml5.EmulatorWithoutJavascript.Console
{
    public class InteropSource : IMessageSource
    {
        public string Code { get; }

        public InteropSource(string code)
        {
            Code = code;
        }
    }
}
