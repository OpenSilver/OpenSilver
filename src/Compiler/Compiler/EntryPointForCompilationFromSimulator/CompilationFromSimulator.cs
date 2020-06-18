using DotNetForHtml5.Compiler.Common;
using System;
using System.Collections.Generic;
using System.IO;

namespace DotNetForHtml5.Compiler
{
    public class CompilationFromSimulator : MarshalByRefObject, ICompilationFromSimulator
    {
        const string COMPILATION_SUCCEEDED_KEYWORD = "COMPILATION SUCCEEDED"; // IMPORTANT: If you change this string, also change the one in the Simulator.

        public string StartCompilation(
            string sourceAssembly,
            string outputRootPath,
            string outputAppFilesPath,
            string outputLibrariesPath,
            string outputResourcesPath,
            string intermediateOutputAbsolutePath,
            string assemblyNameWithoutExtension,
            string flagsString,
            bool isBridgeBasedVersion
            )
        {
            //---------------
            // Returns the compilation log. If the compilation succeeded, it adds the COMPILATION_SUCCEEDED_KEYWORD to the end of the log.
            //---------------

            var logger = new LoggerThatAppendsText();

            var proxiesArray = new string[]
            {
                Path.Combine(PathsHelper.GetCompilerPath(), "JSIL.Proxies.4.0.dll"),
                Path.Combine(PathsHelper.GetCompilerPath(), "JSIL.Proxies.Bcl.dll")
            };

            string activationAppPath = PathsHelper.GetActivationAppPath();
            string proxies = string.Join("|", proxiesArray);
            string assembliesThatContainNoResX = "mscorlib|System.Core|Microsoft.CSharp|JSIL.Meta";
#if SILVERLIGHTCOMPATIBLEVERSION
            string nameOfAssembliesThatDoNotContainUserCode = "SLMigration.CSharpXamlForHtml5|SLMigration.CSharpXamlForHtml5.System.dll|SLMigration.CSharpXamlForHtml5.System.Xaml.dll|SLMigration.CSharpXamlForHtml5.System.Xml.dll|SLMigration.CSharpXamlForHtml5.System.Runtime.Serialization.dll|SLMigration.CSharpXamlForHtml5.System.ServiceModel.dll|mscorlib";
#else
            string nameOfAssembliesThatDoNotContainUserCode = "CSharpXamlForHtml5|CSharpXamlForHtml5.System.dll|CSharpXamlForHtml5.System.Xaml.dll|CSharpXamlForHtml5.System.Xml.dll|CSharpXamlForHtml5.System.Runtime.Serialization.dll|CSharpXamlForHtml5.System.ServiceModel.dll|mscorlib";
#endif
            string additionalSupportedMethods = null;
            bool ignoreUnsupportedMethodsErrors = false;
            bool disableDeadCodeElimination = false;
            bool displayDeadCodeEliminationSkippedMembers = false;
            bool displayDeadCodeEliminationRetainedMembers = false;
            bool includeUserCodeInDeadCodeElimination = false;
            string deadCodeEliminationWhiteList = null;

            // Start the compilation:
            JavaScriptGenerator.Execute(
                sourceAssembly,
                activationAppPath,
                assembliesThatContainNoResX,
                logger,
                proxies,
                additionalSupportedMethods,
                nameOfAssembliesThatDoNotContainUserCode,
                ignoreUnsupportedMethodsErrors,
                disableDeadCodeElimination,
                displayDeadCodeEliminationSkippedMembers,
                displayDeadCodeEliminationRetainedMembers,
                includeUserCodeInDeadCodeElimination,
                deadCodeEliminationWhiteList,
                outputRootPath: outputRootPath,
                outputAppFilesPath: outputAppFilesPath,
                outputLibrariesPath: outputLibrariesPath,
                outputResourcesPath: outputResourcesPath,
                intermediateOutputAbsolutePath: intermediateOutputAbsolutePath,
                flagsString: flagsString,
                isBridgeBasedVersion: isBridgeBasedVersion
                );

            // Call the "Wrapper" which generates the "index.html" file and copies the JSIL libraries:
            string[] librariesDirectories = new string[]
            {
                PathsHelper.GetLibrariesPath()
            };
            Wrapper_JSILversion.Execute(
                sourceLibrariesDirectories: librariesDirectories,
                pathOfAssemblyThatContainsEntryPoint: sourceAssembly,
                logger: logger,
                outputRootPath: outputRootPath,
                outputAppFilesPath: outputAppFilesPath,
                outputLibrariesPath: outputLibrariesPath,
                outputResourcesPath: outputResourcesPath,
                assemblyNameWithoutExtension: assemblyNameWithoutExtension
                );

            return logger.Log + (!logger.HasErrors ? Environment.NewLine + Environment.NewLine + COMPILATION_SUCCEEDED_KEYWORD : "");
        }

        public bool IsFirstTimeJavaScriptCompilation(string intermediateOutputAbsolutePath)
        {
            List<string> simpleNameOfAssembliesToSkip =
                TranspilingOnlyAssembliesThatHaveChanged.GetSimpleNameOfAssembliesToSkip(intermediateOutputAbsolutePath: intermediateOutputAbsolutePath);
            return (simpleNameOfAssembliesToSkip == null || simpleNameOfAssembliesToSkip.Count == 0);
        }
    }
}
