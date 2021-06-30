using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetForHtml5.EmulatorWithoutJavascript.Console
{
    public class ConsoleMessage
    {
        public enum MessageLevel
        {
            Log,
            Warning,
            Error
        }

        public string Message { get; }
        public MessageLevel Level { get; }
        public IMessageSource Source { get; }

        public ConsoleMessage(string message, MessageLevel level, IMessageSource source = null)
        {
            Message = message;
            Level = level;
            Source = source;
        }

        public override string ToString()
        {
            if (Source is FileSource fileSource)
            {
                return $"{Message}\n\nat: {fileSource.Path}:{fileSource.Line}";
            }
            else if (Source is InteropSource interopSource)
            {
                return $"{Message}\n\nError in the following code:\n{interopSource.Code}";
            }
            else
            {
                return Message;
            }
        }
    }
}
