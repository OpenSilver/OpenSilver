using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetForHtml5.EmulatorWithoutJavascript
{
    internal static class PathsHelper
    {
        public static string GetProgramFilesX86Path()
        {
            // Credits: http://stackoverflow.com/questions/194157/c-sharp-how-to-get-program-files-x86-on-windows-vista-64-bit

            if (8 == IntPtr.Size
                || (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
            {
                return Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            }

            return Environment.GetEnvironmentVariable("ProgramFiles");
        }

        public static string GetCshtml5RootPath()
        {
            return Path.Combine(GetProgramFilesX86Path(), @"MSBuild\CSharpXamlForHtml5");
        }

        public static string GetActivationAppPath()
        {
            return Path.Combine(GetCshtml5RootPath(), @"InternalStuff\Activation\CSharpXamlForHtml5.Activation.exe");
        }

        public static string GetCompilerPath()
        {
            return Path.Combine(GetCshtml5RootPath(), @"InternalStuff\Compiler");
        }

        public static string GetLibrariesPath()
        {
            return Path.Combine(GetCshtml5RootPath(), @"InternalStuff\Libraries");
        }
    }
}
