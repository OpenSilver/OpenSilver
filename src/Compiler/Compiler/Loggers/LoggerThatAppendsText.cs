

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
