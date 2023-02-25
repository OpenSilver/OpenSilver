
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
using Microsoft.Build.Utilities;

namespace OpenSilver.Compiler.Common
{
    public sealed class LoggerThatUsesTaskOutput : ILogger
    {
        private readonly Task _task;
        private bool _hasErrors;

        public LoggerThatUsesTaskOutput(Task task)
        {
            _task = task;
        }

        public void WriteError(string message, string file = "", int lineNumber = 0, int columnNumber = 0)
        {
            _hasErrors = true;
            var args = new BuildErrorEventArgs(
                string.Empty,
                string.Empty,
                file,
                lineNumber,
                columnNumber,
                0,
                0,
                message,
                string.Empty,
                _task.ToString());
            _task.BuildEngine.LogErrorEvent(args);
        }

        public void WriteMessage(string message, MessageImportance messageImportance)
        {
            var args = new BuildMessageEventArgs(
                message,
                string.Empty,
                _task.ToString(),
                messageImportance);
            _task.BuildEngine.LogMessageEvent(args);
        }

        public void WriteWarning(string message, string file = "", int lineNumber = 0, int columnNumber = 0)
        {
            var args = new BuildWarningEventArgs(
                string.Empty,
                string.Empty,
                file,
                lineNumber,
                columnNumber,
                0,
                0,
                message,
                string.Empty,
                _task.ToString());
            _task.BuildEngine.LogWarningEvent(args);
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
