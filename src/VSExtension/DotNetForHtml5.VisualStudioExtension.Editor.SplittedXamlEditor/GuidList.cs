using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetForHtml5.VisualStudioExtension.Editor.SplittedXamlEditor
{
    static class GuidList
    {
        public const string guidEditorPkgString = "d2aaf760-876a-4b94-81bb-309d05d7896e";
        public const string guidEditorCmdSetString = "4f73c4fa-d924-4d67-9614-533521c88086";
        public const string guidEditorFactoryString = "9e5f34d9-b31c-4bf6-99fa-d1167a3a51a8";

        public static readonly Guid guidEditorCmdSet = new Guid(guidEditorCmdSetString);
        public static readonly Guid guidEditorFactory = new Guid(guidEditorFactoryString);

        public const string guidXmlChooserEditorFactory = @"{32CC8DFA-2D70-49b2-94CD-22D57349B778}";
    };
}
