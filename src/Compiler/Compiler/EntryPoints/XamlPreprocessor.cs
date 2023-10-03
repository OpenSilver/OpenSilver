﻿
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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using OpenSilver.Compiler.Common;
using ILogger = OpenSilver.Compiler.Common.ILogger;
using System.Linq;

namespace OpenSilver.Compiler
{
    public class XamlPreprocessor : Task
    {
        [Required]
        public string SourceFile { get; set; }

        [Required]
        public string OutputFile { get; set; }

        [Required]
        public string FileNameWithPathRelativeToProjectRoot { get; set; }

        [Required]
        public string AssemblyNameWithoutExtension { get; set; }

        [Required]
        public bool IsSecondPass { get; set; }

        /// <summary>
        /// This is an optimization that prevents re-processing the XAML file if the source has not changed.
        /// It is particularly useful to accelerate the design-time compilation. However, it should not be
        /// enabled for standard compilation because, even though a XAML file has not changed, it may be
        /// necessary to recompile it because it may reference some other classes in the project that no
        /// longer exist or that have changed.
        /// </summary>
        [Required]
        public bool OverrideOutputOnlyIfSourceHasChanged { get; set; }

        [Required]
        public bool IsSLMigration { get; set; }

        [Required]
        public string OutputRootPath { get; set; }

        [Required]
        public string OutputAppFilesPath { get; set; }

        [Required]
        public string OutputLibrariesPath { get; set; }

        [Required]
        public string OutputResourcesPath { get; set; }

        [Required]
        public string Flags { get; set; }

        [Required]
        public string Language { get; set; }

        public string RootNamespace { get; set; }

        public override bool Execute()
        {
            return ExecuteImpl(
                SourceFile,
                OutputFile,
                FileNameWithPathRelativeToProjectRoot,
                AssemblyNameWithoutExtension,
                IsSecondPass,
                IsSLMigration,
                new LoggerThatUsesTaskOutput(this),
                OverrideOutputOnlyIfSourceHasChanged,
                OutputRootPath,
                OutputAppFilesPath,
                OutputLibrariesPath,
                OutputResourcesPath,
                Flags,
                LanguageHelpers.GetLanguage(Language),
                RootNamespace);
        }

        private static bool ExecuteImpl(
            string sourceFile,
            string outputFile,
            string fileNameWithPathRelativeToProjectRoot,
            string assemblyNameWithoutExtension,
            bool isSecondPass,
            bool isSLMigration,
            ILogger logger,
            bool overrideOutputOnlyIfSourceHasChanged,
            string outputRootPath,
            string outputAppFilesPath,
            string outputLibrariesPath,
            string outputResourcesPath,
            string flagsString,
            SupportedLanguage language,
            string rootNamespace)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            string passNumber = (isSecondPass ? "2" : "1");
            string operationName = string.Format("C#/XAML for HTML5: XamlPreprocessor (pass {0})", passNumber);
            try
            {
                using (var executionTimeMeasuring = new ExecutionTimeMeasuring())
                {
                    // Validate input strings:
                    if (string.IsNullOrEmpty(sourceFile))
                        throw new Exception(operationName + " failed because the source file argument is invalid.");
                    if (string.IsNullOrEmpty(outputFile))
                        throw new Exception(operationName + " failed because the output file argument is invalid.");
                    if (string.IsNullOrEmpty(fileNameWithPathRelativeToProjectRoot))
                        throw new Exception(operationName + " failed because the FileNameWithPathRelativeToProjectRoot argument is invalid.");
                    if (string.IsNullOrEmpty(assemblyNameWithoutExtension))
                        throw new Exception(operationName + " failed because the AssemblyNameWithoutExtension argument is invalid.");

                    HashSet<string> flags = (flagsString != null ? new HashSet<string>(flagsString.Split(';')) : new HashSet<string>());

                    //------- DISPLAY THE PROGRESS -------
                    logger.WriteMessage(operationName + " started for file \"" + sourceFile + "\". Output file: \"" + outputFile + "\". FileNameWithPathRelativeToProjectRoot: \"" + fileNameWithPathRelativeToProjectRoot + "\". AssemblyNameWithoutExtension: \"" + assemblyNameWithoutExtension + "\". IsSecondPass: " + isSecondPass.ToString() + "\".");
                    //todo: do not display the output file location?

                    // Read the XAML file:
                    using (StreamReader sr = new StreamReader(sourceFile))
                    {
                        string xaml = sr.ReadToEnd();

                        // Determine if the file should be processed or if there is no need to process it again (for example if the XAML has not changed and we are in design-time, we don't want to re-process the XAML):
                        bool shouldTheFileBeProcessed = DetermineIfTheXamlFileNeedsToBeProcessed(xaml, outputFile, overrideOutputOnlyIfSourceHasChanged, isSecondPass);

                        if (shouldTheFileBeProcessed)
                        {
                            // The "ReflectionOnSeparateAppDomainHandler" class lets us use a separate AppDomain to resolve the types so that the types can be unloaded when done (when disposed, it frees any hook on the user application DLL's):
                            AssembliesInspector reflectionOnSeparateAppDomain = AssembliesInspector.Current; // Note: this is not supposed to be null because it was instantiated in the "BeforeXamlPreprocessor" task. We use a static instance to avoid reloading the assemblies for each XAML file that is processed.

                            // Make sure that the reference is not null:
                            if (reflectionOnSeparateAppDomain == null)
                                throw new Exception("ReflectionOnSeparateAppDomainHandler.Current is null. It should not be null because it was supposed to be populated by the 'BeforeXamlPreprocessor' task. Please verify that the MSBuild Targets are up to date.");

                            string generatedCode;
                            if (language == SupportedLanguage.CSharp)
                            {
                                // Convert XAML to CS:
                                generatedCode = ConvertingXamlToCSharp.Convert(
                                    xaml,
                                    sourceFile,
                                    fileNameWithPathRelativeToProjectRoot,
                                    assemblyNameWithoutExtension,
                                    reflectionOnSeparateAppDomain,
                                    isFirstPass: !isSecondPass,
                                    isSLMigration: isSLMigration,
                                    outputRootPath: outputRootPath,
                                    outputAppFilesPath: outputAppFilesPath,
                                    outputLibrariesPath: outputLibrariesPath,
                                    outputResourcesPath: outputResourcesPath,
                                    logger: logger);

                                // Add the header that contains the file hash so as to avoid re-processing the file if not needed:
                                generatedCode = CreateHeaderContainingHash(generatedCode, xaml, isSecondPass)
                                    + Environment.NewLine
                                    + Environment.NewLine
                                    + generatedCode;
                            }
                            else if (language == SupportedLanguage.VBNet)
                            {
                                // Convert XAML to VB:
                                generatedCode = ConvertingXamlToVB.Convert(
                                    xaml,
                                    sourceFile,
                                    fileNameWithPathRelativeToProjectRoot,
                                    assemblyNameWithoutExtension,
                                    rootNamespace,
                                    reflectionOnSeparateAppDomain,
                                    isFirstPass: !isSecondPass,
                                    isSLMigration: isSLMigration,
                                    outputRootPath: outputRootPath,
                                    outputAppFilesPath: outputAppFilesPath,
                                    outputLibrariesPath: outputLibrariesPath,
                                    outputResourcesPath: outputResourcesPath,
                                    logger: logger);

                                // Add the header that contains the file hash so as to avoid re-processing the file if not needed:
                                generatedCode = CreateVBHeaderContainingHash(generatedCode, xaml, isSecondPass)
                                    + Environment.NewLine
                                    + Environment.NewLine
                                    + generatedCode;
                            }
                            else
                            {
                                logger.WriteMessage($"Unsupported language.");
                                return false;
                            }

                            // Create output directory:
                            Directory.CreateDirectory(Path.GetDirectoryName(outputFile));

                            // Save output:
                            using (StreamWriter outfile = new StreamWriter(outputFile))
                            {
                                outfile.Write(generatedCode);
                            }
                        }
                    }

                    //------- DISPLAY THE PROGRESS -------
                    logger.WriteMessage(operationName + " completed in " + executionTimeMeasuring.StopAndGetTimeInSeconds() + " seconds.");

                    return true;
                }
            }
            catch (Exception ex)
            {
                //-----------------------------------------------------
                // Dispose the static instance of the "ReflectionOnSeparateAppDomainHandler":
                //-----------------------------------------------------

                /*
                    We dispose the static instance of the "ReflectionOnSeparateAppDomainHandler"
                    that was created in the "BeforeXamlPreprocessor" task, in order to free any
                    hooks on the user app DLL's.
                    Note: this is normally done in the task named "AfterXamlPreprocessor", but
                    since we are going to cancel the Build process, that task will never be
                    executed, resulting in potential hooks to the DLL not being freed (causing
                    issues when the user recompiles his application). So we free them now.
                 */

                AssembliesInspector.Current.Dispose(); // Note: this is not supposed to be null because it was instantiated in the "BeforeXamlPreprocessor" task.

                //-----------------------------------------------------
                // Display the error and cancel the Build process:
                //-----------------------------------------------------
                string message = $"{operationName} failed: {string.Join(Environment.NewLine, GetInnerExceptions(ex).Select(e => e.Message))}\nNote: the XAML editor sometimes raises errors that are misleading. To see only real non-misleading errors, make sure to close all the XAML editor windows/tabs before compiling.";

                if (ex is XamlParseException)
                {
                    int lineNumber = ((XamlParseException)ex).LineNumber;
                    logger.WriteError(message, file: sourceFile, lineNumber: lineNumber);
                }
                else
                {
                    logger.WriteError(message, file: sourceFile);
                }

                return false;
            }
        }

        private static bool DetermineIfTheXamlFileNeedsToBeProcessed(string xaml, string outputFile, bool overrideOutputOnlyIfSourceHasChanged, bool isSecondPass)
        {
            //----------------------------------------------------------------
            // This method checks if the Hash of the source XAML has changed.
            //----------------------------------------------------------------

            string passNumber = (isSecondPass ? "2" : "1");

            // Check whether the option "OverrideOutputOnlyIfSourceHasChanged" is enabled. This is typically the case during the design-time compilation, where we don't want to re-process the XAML files at every compilation (for performance reasons):
            if (overrideOutputOnlyIfSourceHasChanged)
            {
                // Check if the output file exists:
                if (File.Exists(outputFile))
                {
                    // Read the header of the output file (the first line of the file), which contains the hash of the previous XAML that it was compiled from:
                    string fileHeader = "";
                    using (StreamReader reader = new StreamReader(outputFile))
                    {
                        fileHeader = reader.ReadLine();
                    }
                    if (!string.IsNullOrEmpty(fileHeader))
                    {
                        // Read the previous hash (if any), and the previous pass number:
                        int x1 = fileHeader.IndexOf("<XamlHash>");
                        int x2 = fileHeader.IndexOf("</XamlHash>");
                        int x3 = fileHeader.IndexOf("<PassNumber>");
                        int x4 = fileHeader.IndexOf("</PassNumber>");

                        if (x1 > 0 && x2 > 0 && x3 > 0 && x4 > 0
                            && (x1 + "<XamlHash>".Length) < x2
                            && (x3 + "<PassNumber>".Length) < x4)
                        {
                            string previousXamlHash = fileHeader.Substring(x1 + "<XamlHash>".Length, (x2 - (x1 + "<XamlHash>".Length)));
                            string previousPassNumber = fileHeader.Substring(x3 + "<PassNumber>".Length, (x4 - (x3 + "<PassNumber>".Length)));

                            // Calculate the new hash:
                            string xamlHash = GetHashString(xaml);

                            // Compare the previous and the new stuff:
                            if (previousXamlHash == xamlHash
                                //&& int.Parse(previousPassNumber) >= int.Parse(passNumber))
                                && previousPassNumber == passNumber)
                            {
                                // If everything is identical, there is no need to precess the file again, so we return False:
                                return false;
                            }
                        }
                    }


                }
            }

            return true;
        }

        /// <summary>
        /// Adds a header to avoid processing the XAML file multiple times if its hash has not changed since the last time.
        /// </summary>
        /// <param name="generatedCode"></param>
        /// <param name="originalXaml"></param>
        /// <param name="isSecondPass"></param>
        /// <returns></returns>
        private static string CreateHeaderContainingHash(string generatedCode, string originalXaml, bool isSecondPass)
        {
            string fileHash = GetHashString(originalXaml);
            string passNumber = (isSecondPass ? "2" : "1");
            string header = string.Format("// <CSHTML5><XamlHash>{0}</XamlHash><PassNumber>{1}</PassNumber><CompilationDate>{2}</CompilationDate></CSHTML5>", fileHash, passNumber, DateTime.Now.ToString());

            return header;
        }
        static string CreateVBHeaderContainingHash(string generatedCode, string originalXaml, bool isSecondPass)
        {
            string fileHash = GetHashString(originalXaml);
            string passNumber = (isSecondPass ? "2" : "1");
            string header = string.Format("' <CSHTML5><XamlHash>{0}</XamlHash><PassNumber>{1}</PassNumber><CompilationDate>{2}</CompilationDate></CSHTML5>", fileHash, passNumber, DateTime.Now.ToString());

            return header;
        }

        private static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        private static byte[] GetHash(string inputString)
        {
            HashAlgorithm algorithm = MD5.Create();  //or use SHA1.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        private static  IEnumerable<Exception> GetInnerExceptions(Exception ex)
        {
            if (ex == null)
            {
                throw new ArgumentNullException(nameof(ex));
            }

            var innerException = ex;
            do
            {
                yield return innerException;
                innerException = innerException.InnerException;
            }
            while (innerException != null);
        }
    }
}
