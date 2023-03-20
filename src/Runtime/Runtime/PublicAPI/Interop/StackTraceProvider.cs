
/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OpenSilver.Internal
{
    [DebuggerDisplay("{FunctionName} {FileName}:{Line}")]
    internal sealed class FunctionDetails
    {
        public string FunctionName = "???";
        public string FileName = "";
        public string FolderName = "";
        public int Line;

        public string FriendlyStr()
        {
            var friendly = FunctionName;
            if (FileName != "")
                friendly += $"{FileName}:{Line}   ({FolderName})";
            return friendly;
        }
    }

    internal sealed class StackTraceProvider
    {
        private static FunctionDetails StackFunctionDetails(string line)
        {
            var fd = new FunctionDetails();
            var endOfFunctionIdx = line.IndexOf("(");
            var atIdx = line.IndexOf(" at ");
            if (endOfFunctionIdx < 0 || atIdx < 0)
                return fd;

            atIdx += 4;
            fd.FunctionName = line.Substring(atIdx, endOfFunctionIdx - atIdx);

            var inIdx = line.IndexOf(" in ");
            if (inIdx < 0)
                return fd;

            inIdx += 4;
            // ... + 2, ignore the drive's ":" char
            var endOfFileIdx = line.IndexOf(":", inIdx + 2);
            var fullFileName = line.Substring(inIdx, endOfFileIdx - inIdx);

            var idxOfLine = line.IndexOf("line ", endOfFileIdx);
            if (idxOfLine > 0)
            {
                idxOfLine += 5;
                if (int.TryParse(line.Substring(idxOfLine), out var idx))
                    fd.Line = idx;
            }

            var lastDelimiterIdx = fullFileName.LastIndexOf("\\");
            if (lastDelimiterIdx >= 0)
            {
                fd.FileName = fullFileName.Substring(lastDelimiterIdx + 1);
                fd.FolderName = fullFileName.Substring(0, lastDelimiterIdx);
            }
            else
                fd.FileName = fullFileName;

            return fd;
        }

        public static IReadOnlyList<FunctionDetails> StackTrace()
        {
            var stack = Environment.StackTrace;
            // line 0: - the Environment.Stacktrace call
            // line 1: - this function
            return stack.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Skip(2).Select(StackFunctionDetails).ToList();
        }

    }
}
