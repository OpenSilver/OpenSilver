extern alias wpf;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

namespace DotNetForHtml5.Compiler
{
    //[LoadInSeparateAppDomain]
    //[Serializable]
    public class JavaScriptGenerator : Task // AppDomainIsolatedTask
    {
        [Required]
        public string SourceAssembly { get; set; }

        [Required]
        public string OutputRootPath { get; set; }

        [Required]
        public string OutputAppFilesPath { get; set; }

        [Required]
        public string OutputLibrariesPath { get; set; }

        [Required]
        public string OutputResourcesPath { get; set; }

        [Required]
        public string IntermediateOutputAbsolutePath { get; set; }

        [Required]
        public string ActivationAppPath { get; set; }

        [Required]
        public string AssembliesThatContainNoResX { get; set; }

        [Required]
        public string Flags { get; set; }

        [Required]
        public bool IsBridgeBasedVersion { get; set; }

        [Output]
        public bool IsSuccess { get; set; }

        // Optional (default is null):
        public string Proxies { get; set; }

        // Optional (default is null):
        public string AdditionalSupportedMethods { get; set; }

        // Optional (default is null):
        public string NameOfAssembliesThatDoNotContainUserCode { get; set; }

        // Optional (default is False):
        public bool IgnoreUnsupportedMethodsErrors { get; set; }

        // Optional (default is False):
        public bool DisableDeadCodeElimination { get; set; }

        // Optional (default is False):
        public bool DisplayDeadCodeEliminationSkippedMembers { get; set; }

        // Optional (default is False):
        public bool DisplayDeadCodeEliminationRetainedMembers { get; set; }

        // Optional (default is False):
        public bool IncludeUserCodeInDeadCodeElimination { get; set; }

        // Optional (default is null):
        public string DeadCodeEliminationWhiteList { get; set; }



        public override bool Execute()
        {
            IsSuccess = Execute(
                SourceAssembly,
                ActivationAppPath,
                AssembliesThatContainNoResX,
                new LoggerThatUsesTaskOutput(this),
                Proxies,
                AdditionalSupportedMethods,
                NameOfAssembliesThatDoNotContainUserCode,
                IgnoreUnsupportedMethodsErrors,
                DisableDeadCodeElimination,
                DisplayDeadCodeEliminationSkippedMembers,
                DisplayDeadCodeEliminationRetainedMembers,
                IncludeUserCodeInDeadCodeElimination,
                DeadCodeEliminationWhiteList,
                OutputRootPath,
                OutputAppFilesPath,
                OutputLibrariesPath,
                OutputResourcesPath,
                IntermediateOutputAbsolutePath,
                Flags,
                IsBridgeBasedVersion
                );
            return IsSuccess;
        }

        public static bool Execute(
            string sourceAssembly,
            string activationAppPath,
            string assembliesThatContainNoResX,
            ILogger logger,
            string proxies,
            string additionalSupportedMethods,
            string nameOfAssembliesThatDoNotContainUserCode,
            bool ignoreUnsupportedMethodsErrors,
            bool disableDeadCodeElimination,
            bool displayDeadCodeEliminationSkippedMembers,
            bool displayDeadCodeEliminationRetainedMembers,
            bool includeUserCodeInDeadCodeElimination,
            string deadCodeEliminationWhiteList,
            string outputRootPath,
            string outputAppFilesPath,
            string outputLibrariesPath,
            string outputResourcesPath,
            string intermediateOutputAbsolutePath,
            string flagsString,
            bool isBridgeBasedVersion
            )
        {
            string operationName = "C#/XAML for HTML5: JavaScriptGenerator";
            try
            {
                using (var executionTimeMeasuring = new ExecutionTimeMeasuring())
                {
                    // Validate input strings:
                    if (string.IsNullOrEmpty(sourceAssembly))
                        throw new Exception(operationName + " failed because the source assembly argument is invalid.");
                    if (string.IsNullOrEmpty(activationAppPath))
                        throw new Exception(operationName + " failed because the activation app path is invalid.");

                    //------- DISPLAY THE PROGRESS -------
                    logger.WriteMessage(operationName + " started for assembly \"" + sourceAssembly + "\".");

                    // Convert the flags into a HashSet for faster lookup:
                    HashSet<string> flags = (flagsString != null ? new HashSet<string>(flagsString.Split(';')) : new HashSet<string>());

                    // Determine the output path:
                    string outputPathAbsolute = PathsHelper.GetOutputPathAbsolute(sourceAssembly, outputRootPath);

                    // Combine the root output path and the relative "app" folder path, while also ensuring that there is no forward slash, and that the path ends with a backslash:
                    string absoluteOutputAppPath = PathsHelper.CombinePathsWhileEnsuringEndingBackslashAndMore(outputPathAbsolute, outputAppFilesPath);

                    // Create the destination folders hierarchy if it does not already exist:
                    if (!Directory.Exists(absoluteOutputAppPath))
                        Directory.CreateDirectory(absoluteOutputAppPath);

                    //// Make sure that the output directory is empty:
                    //FileHelpers.DeleteAllFilesAndFoldersInDirectory(outputPathAbsolute);

                    // Process the arguments:
                    string[] arrayWithSimpleNameOfAssembliesThatContainNoResX = assembliesThatContainNoResX != null ? assembliesThatContainNoResX.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { };
                    HashSet<string> simpleNameOfAssembliesThatContainNoResX = new HashSet<string>(arrayWithSimpleNameOfAssembliesThatContainNoResX);

                    // Start the JavaScript generation:
                    bool isSuccess = ConvertingExeOrDllToJavaScript.Convert(
                        sourceAssembly,
                        absoluteOutputAppPath,
                        activationAppPath,
                        simpleNameOfAssembliesThatContainNoResX,
                        logger,
                        sourceAssembly,
                        proxies,
                        additionalSupportedMethods,
                        nameOfAssembliesThatDoNotContainUserCode,
                        ignoreUnsupportedMethodsErrors,
                        disableDeadCodeElimination,
                        displayDeadCodeEliminationSkippedMembers,
                        displayDeadCodeEliminationRetainedMembers,
                        includeUserCodeInDeadCodeElimination,
                        deadCodeEliminationWhiteList,
                        outputRootPath,
                        outputAppFilesPath,
                        outputLibrariesPath,
                        outputResourcesPath,
                        intermediateOutputAbsolutePath,
                        flags,
                        isBridgeBasedVersion
                        );

                    //------- DISPLAY THE PROGRESS -------
                    logger.WriteMessage(operationName + (isSuccess ? " completed in " + executionTimeMeasuring.StopAndGetTimeInSeconds() + " seconds." : " failed."));

                    return isSuccess;
                }
            }
            catch (Exception ex)
            {
                string message = operationName + " failed: " + ex.ToString();

                if (ex is wpf::System.Windows.Markup.XamlParseException)
                {
                    int lineNumber = ((wpf::System.Windows.Markup.XamlParseException)ex).LineNumber;
                    logger.WriteError(message, lineNumber: lineNumber);
                }
                else
                {
                    logger.WriteError(message);
                }
                return false;
            }
        }
    }
}
