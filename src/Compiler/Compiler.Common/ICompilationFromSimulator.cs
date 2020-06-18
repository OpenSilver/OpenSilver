using System.Collections.Generic;

namespace DotNetForHtml5.Compiler.Common
{
    // This is referenced by both the Simulator and the MSBUILD-based compiler so that the two can communicate even though they are in separate AppDomains.

    public interface ICompilationFromSimulator
    {
        string StartCompilation(
            string sourceAssembly,
            string outputRootPath,
            string outputAppFilesPath,
            string outputLibrariesPath,
            string outputResourcesPath,
            string intermediateOutputAbsolutePath,
            string assemblyNameWithoutExtension,
            string flags,
            bool isBridgeBasedVersion
            );

        bool IsFirstTimeJavaScriptCompilation(string intermediateOutputAbsolutePath);
    }
}
