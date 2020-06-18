using Microsoft.Build.Framework;
using System;

namespace DotNetForHtml5.Compiler
{
    internal class LoggerThatUsesConsoleWriteLine : LoggerBase, ILogger
    {
        bool _hasErrors;

        public void WriteError(string message, string file = "", int lineNumber = 0, int columnNumber = 0)
        {
            _hasErrors = true;
            Console.WriteLine("ERROR: " + message + (!string.IsNullOrEmpty(file) ? " in \"" + file + "\"" + (lineNumber > 0 ? " (line " + lineNumber.ToString() + ")" : "") + (columnNumber > 0 ? " (column " + columnNumber.ToString() + ")" : "") + "." : ""));
        }

        public void WriteMessage(string message, MessageImportance messageImportance)
        {
            Console.WriteLine(message);
        }

        public void WriteWarning(string message, string file = "", int lineNumber = 0, int columnNumber = 0)
        {
            Console.WriteLine("WARNING: " + message);
        }

        public bool HasErrors
        {
            get
            {
                return _hasErrors;
            }
        }
    }
}
