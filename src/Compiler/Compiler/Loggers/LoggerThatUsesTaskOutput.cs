using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;

namespace DotNetForHtml5.Compiler
{
    internal class LoggerThatUsesTaskOutput : LoggerBase, ILogger
    {
        //AppDomainIsolatedTask _task;
        Task _task;
        bool _hasErrors;

        public LoggerThatUsesTaskOutput(Task task) // (AppDomainIsolatedTask task)
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
