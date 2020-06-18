#define ENABLE_CODE_ELIMINATION


using JSIL;
using JSIL.Compiler.Extensibility.DeadCodeAnalyzer;
using JSIL.Translator;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetForHtml5.Compiler
{
    internal static class ConvertingExeOrDllToJavaScript
    {
        public static bool Convert(
            string assemblyPath,
            string outputPath,
            string activationAppPath,
            HashSet<string> simpleNameOfAssembliesThatContainNoResX,
            ILogger logger,
            string pathOfAssemblyThatContainsEntryPoint,
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
            HashSet<string> flags,
            bool isBridgeBasedVersion
            )
        {
            // Prepare the configuration
            var translatorConfiguration = new Configuration
            {
                ApplyDefaults = false,
                Assemblies =
                {
                    Stubbed = {
                            "mscorlib,",
                            //"System.XML",


                            //// SCENARIO2 : FOR STUBBED SYSTEM.DLL ONLY, UNCOMMENT THE FOLLOWING:
                            //"^System,",
                            //"System,",
                            //"System.dll,",
                        },
                    Proxies =
                    {
                        //Path.Combine(Path.GetDirectoryName(assemblyPath), "DotNetForHtml5.Proxies.dll")
                        //@"C:\DotNetForHtml5\DotNetForHtml5\DotNetForHtml5.Proxies\bin\Debug\DotNetForHtml5.Proxies.dll"
                        //@"C:\DotNetForHtml5\DotNetForHtml5\DotNetForHtml5.Proxies\bin\Debug\DotNetForHtml5.Proxies.dll"
                    },
                    Ignored = {
                            //-----------------------
                            // Example of input string: "System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
                            //-----------------------


                            //@"^System.(?!XML|Xml.Serialization)(.*)",
                            //@"^System(.+)",
                            //@"^System.*",

                            //"^(?!System.dll|System.Xaml.dll|System.Xml.Serialization.dll)System.*",


                            //"^System$",

                            @"^(?!SLMigration(.*))(?!CSharpXamlForHtml5(.*))((.*)System(.*))", // This means that if it contains "System" but it is not preceeded by "CSharpXamlForHtml5", we ignore it. Just for information: ^(?!b)c matches only the elements that start with c while ^(?!b)((.*)c(.*)) matches anything starting which doesn't start with b and contains c

                            @"(.*)ToBeReplacedAtRuntime(.*)",

                            //"System.Dynamic.Runtime",
                            //"System.Linq.Expressions",
                            //"System.Runtime",
                            //"System.Core",
                            //"System.Configuration",
                            //"System.Xml",
                            //"System.Security",
                            //"System.Numerics",
                            //"System.Data.SqlXml",
                            //"System.EnterpriseServices,",

                            "^Microsoft.*",
                            "^Microsoft.VisualC,",
                            "^Accessibility,",
                            "^SMDiagnostics,",
                            "^JSIL.Meta,",

                            "^DotNetBrowser*",

                            ////// SCENARIO2 : FOR STUBBED SYSTEM.DLL ONLY, UNCOMMENT THE FOLLOWING:
                            //"Microsoft\\.VisualC,",
                            //"Accessibility,",
                            //"SMDiagnostics,",
                            //"System\\.EnterpriseServices,",
                            //"System\\.Security,",
                            //"System\\.Runtime\\.Serialization\\.Formatters\\.Soap,",
                            //"System\\.Runtime\\.DurableInstancing,",
                            //"System\\.Data\\.SqlXml,",
                            //"JSIL\\.Meta,",
                            //"System\\.Configuration,",
                            //"System\\.Core,",
                            //"System\\.Dynamic,",
                            //"System\\.Numerics,",
                            //"System\\.Xaml,",
                            //"System\\.Xml,",
                        }
                },
                CodeGenerator =
                {
                    EnableUnsafeCode = true
                },
                FrameworkVersion = 4.0,
                GenerateSkeletonsForStubbedAssemblies = false,
                GenerateContentManifest = true,
                IncludeDependencies = true,
                UseSymbols = true,
                UseThreads = false,
                RunBugChecks = true
            };

            // Prepare the DeadCodeAnalyzer:
            DeadCodeAnalyzer dce = new DeadCodeAnalyzer(pathOfAssemblyThatContainsEntryPoint, disableDeadCodeElimination, deadCodeEliminationWhiteList);

            // Prepare the code that checks that no unsupported method is called:
            string[] listOfProxies = proxies != null ? proxies.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { };
            var checkIfUnsupportedMethods = new CheckingThatNoUnsupportedMethodIsCalled();
            var checkIfUnsupportedProjects = new CheckingThatNoUnsupportedProjectIsBeingProcessed();
            checkIfUnsupportedMethods.InitializeListOfSupportedMethods(listOfProxies, logger);
            checkIfUnsupportedMethods.AddAdditionalSupportedMethods(additionalSupportedMethods);

            //// Add the proxies:
            //foreach (var proxy in listOfProxies)
            //    translatorConfiguration.Assemblies.Proxies.Add(proxy);

            // Get the list of assemblies that do not need to be transpiled again:
            List<string> simpleNameOfAssembliesToSkip =
                TranspilingOnlyAssembliesThatHaveChanged.GetSimpleNameOfAssembliesToSkip(intermediateOutputAbsolutePath: intermediateOutputAbsolutePath);

            // Prepare the translator:
            AssemblyTranslator translator = new AssemblyTranslator(translatorConfiguration);
            var translatorOutput = new StringBuilder();
            HookTranslatorEvents(translator, translatorOutput, activationAppPath, logger, dce, checkIfUnsupportedMethods, checkIfUnsupportedProjects, nameOfAssembliesThatDoNotContainUserCode, ignoreUnsupportedMethodsErrors, displayDeadCodeEliminationSkippedMembers, displayDeadCodeEliminationRetainedMembers, includeUserCodeInDeadCodeElimination, flags);
            var translateStartedTime = DateTime.UtcNow.Ticks;

            // Start the conversion:
            TranslationResult translationResult = translator.Translate(assemblyPath, simpleNameOfAssembliesToSkip, scanForProxies: true);

#if REQUIRE_ACTIVATION_FOR_USING_CERTAIN_METHODS
            // If a feature is missing (eg. the "Professional Edition" is required), display the window for activation:
            if (logger.RequiresMissingFeature)
                ActivationHelpers.DisplayActivationApp(activationAppPath, logger.MissingFeatureId, logger.MessageForMissingFeature);
#endif

            // If there were errors (like due to not supported methods, cf. "NotSupportedException" in "HookTranslatorEvents"), cancel:
            if (logger.HasErrors)
                return false;

            // Process ResX files:
            using (var reflectionOnSeparateAppDomain = new ReflectionOnSeparateAppDomainHandler())
            {
                ConvertingResXFilesToJavaScript.Start(assemblyPath, outputPath, simpleNameOfAssembliesThatContainNoResX, translationResult, logger, reflectionOnSeparateAppDomain, isBridgeBasedVersion);
            }

            // Generate the manifest:
            AssemblyTranslator.GenerateManifest(translator.Manifest, "index", translationResult, outputAppFilesPath);

            // Save files to disk:
            translationResult.WriteToDirectory(outputPath, manifestPrefix: "index.");

            // Remember the information about the transpiled assemblies so that, next time, we can avoid transpiling again the assemblies that have not changed:
            TranspilingOnlyAssembliesThatHaveChanged.RememberAssembliesThatHaveBeenTranspiled(
                translationResult: translationResult,
                sourceAssembly: assemblyPath,
                outputAppFilesPath: outputAppFilesPath,
                outputRootPath: outputRootPath,
                intermediateOutputAbsolutePath: intermediateOutputAbsolutePath);

            return true;
        }

        static void HookTranslatorEvents(AssemblyTranslator translator, StringBuilder translatorOutput, string activationAppPath, ILogger logger, DeadCodeAnalyzer dce, CheckingThatNoUnsupportedMethodIsCalled checkIfUnsupportedMethods, CheckingThatNoUnsupportedProjectIsBeingProcessed checkIfUnsupportedProjects, string nameOfAssembliesThatDoNotContainUserCode, bool ignoreUnsupportedMethodsErrors, bool displayDeadCodeEliminationSkippedMembers, bool displayDeadCodeEliminationRetainedMembers, bool includeUserCodeInDeadCodeElimination, HashSet<string> flags)
        {
            translator.AssembliesLoaded += (AssemblyDefinition[] definitions) =>
                {
                    //--------------------------------------------
                    // CHECK FOR UNSUPPORTED PROJECTS:
                    //--------------------------------------------
                    checkIfUnsupportedProjects.Check(definitions, nameOfAssembliesThatDoNotContainUserCode,
                        whatToDoWhenNotSupportedMethodFound: (explanationToDisplayInErrorsWindow) =>
                        {
                            // Cause an error to appear and the compilation to abort:
                            logger.WriteError(explanationToDisplayInErrorsWindow);
                        });

#if ENABLE_DEAD_CODE_ANALYSER
                    dce.AddAssemblies(definitions);
#endif

                    //--------------------------------------------
                    // CHECK FOR UNSUPPORTED METHODS:
                    //--------------------------------------------
                    checkIfUnsupportedMethods.Check(definitions, nameOfAssembliesThatDoNotContainUserCode, activationAppPath, "mscorlib", flags,
                        whatToDoWhenNotSupportedMethodFound: (unsupportedMethodInfo) =>
                        {
#if REQUIRE_ACTIVATION_FOR_USING_CERTAIN_METHODS
                            // First, we distinguish whether the reason why the method is not allowed is because it requires a license (aka a "missing feature"), or because it is not supported:
                            if (unsupportedMethodInfo.RequiresMissingFeature)
                            {
                                // Cause the popup to appear that says that a license is required:
                                logger.SetRequiresMissingFeature(unsupportedMethodInfo.MissingFeatureId, unsupportedMethodInfo.MessageForMissingFeature);

                                // Cause an error to appear and the compilation to abort:
                                if (!unsupportedMethodInfo.IsInValidTrialMode)
                                    logger.WriteError(
                                        unsupportedMethodInfo.ExplanationToDisplayInErrorsWindow,
                                        unsupportedMethodInfo.CallingMethodFileNameWithPath,
                                        unsupportedMethodInfo.CallingMethodLineNumber);
                            }
                            else
#endif
                            // Cause an error to appear and the compilation to abort:
                            if (!ignoreUnsupportedMethodsErrors)
                                logger.WriteError(
                                    unsupportedMethodInfo.ExplanationToDisplayInErrorsWindow,
                                    unsupportedMethodInfo.CallingMethodFileNameWithPath,
                                    unsupportedMethodInfo.CallingMethodLineNumber);
                        });
                };

            translator.AnalyzeStarted += () =>
                {
#if ENABLE_DEAD_CODE_ANALYSER
                    dce.Analyze(translator._TypeInfoProvider);
#endif
                };

            translator.MemberCanBeSkipped += (MemberReference member) =>
                {
#if ENABLE_DEAD_CODE_ANALYSER
                    //----------------------
                    // CODE ELIMINATION BASED ON THE RECURSIVE DEAD CODE ELIMINATION ALGORITHM:
                    //----------------------

                    bool canBeSkipped = (includeUserCodeInDeadCodeElimination || member.Module.Name == "CommonLanguageRuntimeLibrary") && dce.MemberCanBeSkipped(member);
                    if (canBeSkipped && displayDeadCodeEliminationSkippedMembers)
                    {
                        logger.WriteMessage("DCE member skipped: [" + member.Module.Name + "] " + member.FullName);
                    }
                    else if (!canBeSkipped && displayDeadCodeEliminationRetainedMembers)
                    {
                        logger.WriteMessage("DCE member retained: [" + member.Module.Name + "] " + member.FullName);
                    }
                    return canBeSkipped;
#elif ENABLE_CODE_ELIMINATION

                    //----------------------
                    // CODE ELIMINATION BASED ON THE JSIL SUPPORTED TYPES RATHER THAN A REAL RECURSIVE DEAD CODE ELIMINATION ALGORITHM:
                    //----------------------

                    if (member.Module.Name == "CommonLanguageRuntimeLibrary") //Note: this is the Corlib DLL
                    {
                        if (member is TypeDefinition && !((TypeDefinition)member).IsInterface)
                        {
                            string typeName = (member.DeclaringType != null ? member.DeclaringType.Name + "+" : "") + member.Name;
                            bool isTypeSupported = RemovalOfUnsupportedTypesFromCorlib.IsTypeSupported(typeName);
                            bool isTypeInList = checkIfUnsupportedMethods.IsTypeSupported(typeName);
                            return !isTypeSupported && !isTypeInList;
                        }
                        else if (member is MethodDefinition)
                        {
                            string typeName = member.DeclaringType.Name;
                            string methodName = member.Name;
                            bool isMethodSupported = checkIfUnsupportedMethods.IsMethodSupported(typeName, methodName);
                            return !isMethodSupported;
                        }
                        else if (member is FieldDefinition)
                        {
                            return false; // Do not skip

                            //string fieldTypeName = ((FieldDefinition)member).FieldType.Name;
                            //if (fieldTypeName.IndexOf('[') > -1)
                            //    fieldTypeName = fieldTypeName.Substring(0, fieldTypeName.IndexOf('['));
                            //if (fieldTypeName.IndexOf(' ') > -1)
                            //    fieldTypeName = fieldTypeName.Substring(0, fieldTypeName.IndexOf(' '));

                            //bool isFieldAKnownType = checkIfUnsupported.IsTypeSupported(fieldTypeName) || RemovalOfUnsupportedTypesFromCorlib.IsTypeSupported(fieldTypeName);

                            //if (!isFieldAKnownType)
                            //    System.Diagnostics.Debug.WriteLine("Unknown field type: " + (((FieldDefinition)member).FieldType.DeclaringType != null ? ((FieldDefinition)member).FieldType.DeclaringType.Name + "+" : "") + ((FieldDefinition)member).FieldType.Namespace + "." + fieldTypeName);

                            //return !isFieldAKnownType;
                        }
                        else
                            return false; // Do not skip
                    }
                    else
                        return false; // Do not skip
#else
                    return false;
#endif
                };

            translator.CouldNotDecompileMethod += (s, exception) =>
            {
                if (!s.Contains("HtmlEventProxy")) // This enabled to remove the following warning, which for some reason cannot be removed with "[JSIgnore]":   Could not decompile method 'System.Void DotNetForHtml5.Core.HtmlEventProxy/<>c__DisplayClassa::<OnEvent>b__9()': Call sites of type 'InvokeConstructor' not implemented.
                {
                    lock (logger)
                        logger.WriteWarning(string.Format(
                            "Could not decompile method '{0}': {1}",
                            s, exception.Message
                        ));
                }
            };

            translator.CouldNotResolveAssembly += (s, exception) =>
            {
                if (!s.Contains("mscorlib"))
                {
                    lock (logger)
                        logger.WriteWarning(string.Format(
                            "Could not resolve assembly '{0}': {1}",
                            s, exception.Message
                        ));
                }
            };

            translator.CouldNotLoadSymbols += (s, exception) =>
            {
                if (!exception.Message.Contains("mscorlib.dll")
                    && !exception.Message.Contains("CSharpXamlForHtml5."))
                {
                    lock (logger)
                        logger.WriteWarning(exception.Message);
                }
            };

            translator.Warning += (s) =>
            {
                lock (logger)
                    logger.WriteWarning("[WARNING] " + s);
            };

            translator.Decompiling += (progress) =>
            {
                lock (logger)
                    logger.WriteMessage("Decompiling ");

                var previous = new int[1] { 0 };

                progress.ProgressChanged += (s, p, max) =>
                {
                    var current = p * 20 / max;
                    if (current != previous[0])
                    {
                        previous[0] = current;
                        //lock (logger)
                        //    logger.Write(".");
                    }
                };

                progress.Finished += (s, e) =>
                {
                    lock (logger)
                        logger.WriteMessage(" done");
                };
            };

            translator.Writing += (progress) =>
            {
                lock (logger)
                    logger.WriteMessage("Writing JavaScript ");

                var previous = new int[1] { 0 };

                progress.ProgressChanged += (s, p, max) =>
                {
                    var current = p * 20 / max;
                    if (current != previous[0])
                    {
                        previous[0] = current;
                        //lock (logger)
                        //    logger.Write(".");
                    }
                };

                progress.Finished += (s, e) =>
                {
                    lock (logger)
                        logger.WriteMessage(" done");
                };
            };

        }

    }
}
