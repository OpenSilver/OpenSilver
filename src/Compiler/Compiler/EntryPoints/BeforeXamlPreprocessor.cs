
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

using DotNetForHtml5.Compiler.OtherHelpersAndHandlers;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using OpenSilver.Compiler.Common;
using ILogger = OpenSilver.Compiler.Common.ILogger;

namespace OpenSilver.Compiler
{
    public class BeforeXamlPreprocessor : Task
    {
        [Required]
        public bool IsSecondPass { get; set; }

        [Required]
        public bool IsSLMigration { get; set; }

        [Required]
        public string Flags { get; set; }

        [Required]
        public string Language { get; set; }

        public string SourceAssemblyForPass2 { get; set; }

        public bool IsProcessingCSHTML5Itself { get; set; }

        // the assemblies are setted by the AssemblyReferenceValidator
        public ITaskItem[] ResolvedReferences { get; set; }

        public override bool Execute()
        {
            return ExecuteImpl(
                IsSecondPass,
                IsSLMigration,
                Flags,
                LanguageHelpers.GetLanguage(Language),
                ResolvedReferences,
                SourceAssemblyForPass2,
                IsProcessingCSHTML5Itself,
                new LoggerThatUsesTaskOutput(this));
        }

        private static bool ExecuteImpl(
            bool isSecondPass,
            bool isSLMigration,
            string flagsString,
            SupportedLanguage language,
            ITaskItem[] resolvedReferences,
            string sourceAssemblyForPass2,
            bool isProcessingCSHTML5Itself,
            ILogger logger)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            string passNumber = (isSecondPass ? "2" : "1");
            string operationName = string.Format("C#/XAML for HTML5: BeforeXamlPreprocessor (pass {0})", passNumber);
            try
            {
                using (var executionTimeMeasuring = new ExecutionTimeMeasuring())
                {
                    //------- DISPLAY THE PROGRESS -------
                    logger.WriteMessage(operationName + " started.");

                    //-----------------------------------------------------
                    // Note: we create a static instance of the "ReflectionOnSeparateAppDomainHandler" to avoid reloading the assemblies for each XAML file.
                    // We dispose the static instance in the "AfterXamlPreprocessor" task.
                    //-----------------------------------------------------

                    if (isSecondPass && string.IsNullOrEmpty(sourceAssemblyForPass2))
                        throw new Exception(operationName + " failed because the SourceAssembly parameter was not specified during the second pass.");

                    // Create a new static instance of the "ReflectionOnSeparateAppDomainHandler":
                    AssembliesInspector.Current = new AssembliesInspector(isSLMigration, language);
                    AssembliesInspector reflectionOnSeparateAppDomain = AssembliesInspector.Current;

                    // we load the source assembly early in case we are processing the CSHTML5.
                    if (isSecondPass && isProcessingCSHTML5Itself)
                    {
                        reflectionOnSeparateAppDomain.LoadAssembly(
                            sourceAssemblyForPass2,
                            loadReferencedAssembliesToo: true,
                            skipReadingAttributesFromAssemblies: false);
                    }
                    // work-around: reference path string is not correctly setted so we set it manually
                    string referencePathsString = OpenSilverHelper.ReferencePathsString(resolvedReferences);
                    // Retrieve paths of referenced .dlls and load them:
                    HashSet<string> referencePaths = (referencePathsString != null) ? new HashSet<string>(referencePathsString.Split(';')) : new HashSet<string>();

                    referencePaths.RemoveWhere(s => !s.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase));

                    var coreAssembliesPaths = referencePaths.Where(AssembliesLoadHelper.IsCoreAssembly).ToArray();

                    // Note: we ensure that the Core assemblies are loaded first so that types such as "XmlnsDefinitionAttribute" are known when loading the other assemblies.
                    foreach (var coreAssemblyName in AssembliesLoadHelper.CoreAssembliesNames)
                    {
                        var coreAssemblyPath = coreAssembliesPaths.FirstOrDefault(path => coreAssemblyName.Equals(Path.GetFileNameWithoutExtension(path), StringComparison.InvariantCultureIgnoreCase));
                        if (coreAssemblyPath != null)
                        {
                            reflectionOnSeparateAppDomain.LoadAssembly(
                                coreAssemblyPath,
                                loadReferencedAssembliesToo: false,
                                skipReadingAttributesFromAssemblies: false);

                            referencePaths.Remove(coreAssemblyPath);
                        }
                    }

                    foreach (string referencedAssembly in referencePaths)
                    {
                        reflectionOnSeparateAppDomain.LoadAssembly(
                            referencedAssembly,
                            loadReferencedAssembliesToo: false,
                            skipReadingAttributesFromAssemblies: false);
                    }

                    // Load for reflection the source assembly itself and the referenced assemblies if second path:
                    if (isSecondPass && !isProcessingCSHTML5Itself)
                    {
                        reflectionOnSeparateAppDomain.LoadAssembly(
                            sourceAssemblyForPass2,
                            loadReferencedAssembliesToo: true,
                            skipReadingAttributesFromAssemblies: false);
                    }

                    bool isSuccess = true;

                    //------- DISPLAY THE PROGRESS -------
                    logger.WriteMessage(operationName + (isSuccess ? " completed in " + executionTimeMeasuring.StopAndGetTimeInSeconds() + " seconds." : " failed.") + "\". IsSecondPass: " + isSecondPass.ToString() + ". Source assembly file: \"" + (sourceAssemblyForPass2 ?? "").ToString());

                    return isSuccess;
                }
            }
            catch (Exception ex)
            {
                AssembliesInspector.Current?.Dispose();

                logger.WriteError(operationName + " failed: " + ex.ToString());
                return false;
            }
        }
    }
}
