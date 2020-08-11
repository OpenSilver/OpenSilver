

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
