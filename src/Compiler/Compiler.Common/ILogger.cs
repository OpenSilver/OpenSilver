using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetForHtml5.Compiler
{
    public interface ILogger
    {
        void WriteError(string message, string file = "", int lineNumber = 0, int columnNumber = 0);
        void WriteMessage(string message, MessageImportance messageImportance = MessageImportance.Normal);
        void WriteWarning(string message, string file = "", int lineNumber = 0, int columnNumber = 0);
        bool HasErrors { get; }
#if REQUIRE_ACTIVATION_FOR_USING_CERTAIN_METHODS
        void SetRequiresMissingFeature(string missingFeatureId, string messageForMissingFeature);
        bool RequiresMissingFeature { get; }
        string MissingFeatureId { get; }
        string MessageForMissingFeature { get; }
#endif
    }
}
