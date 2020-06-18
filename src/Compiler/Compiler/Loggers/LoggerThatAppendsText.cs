using System.Text;

namespace DotNetForHtml5.Compiler
{
    public class LoggerThatAppendsText : LoggerBase, ILogger
    {
        bool _hasErrors;
        StringBuilder _stringBuilder = new StringBuilder();
        
        public void WriteError(string message, string file = "", int lineNumber = 0, int columnNumber = 0)
        {
            _hasErrors = true;
            _stringBuilder.AppendLine("ERROR: " + message);
        }

        public void WriteMessage(string message, Microsoft.Build.Framework.MessageImportance messageImportance = Microsoft.Build.Framework.MessageImportance.High)
        {
            _stringBuilder.AppendLine(message);
        }

        public void WriteWarning(string message, string file = "", int lineNumber = 0, int columnNumber = 0)
        {
            _stringBuilder.AppendLine("WARNING: " + message);
        }

        public bool HasErrors
        {
            get { return _hasErrors; }
        }

        public string Log
        {
            get { return _stringBuilder.ToString(); }
        }
    }
}
