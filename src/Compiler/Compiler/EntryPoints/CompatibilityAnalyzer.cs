

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
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;

namespace DotNetForHtml5.Compiler
{
    //[LoadInSeparateAppDomain]
    //[Serializable]
    public class CompatibilityAnalyzer : Task // AppDomainIsolatedTask
    {
        [Required]
        public string SourceAssembly { get; set; }

        [Required]
        public string ActivationAppPath { get; set; }

        [Required]
        public string Flags { get; set; }

        [Output]
        public bool IsSuccess { get; set; }

        public override bool Execute()
        {
            IsSuccess = Execute(SourceAssembly, ActivationAppPath, Flags, new LoggerThatUsesTaskOutput(this));
            return IsSuccess;
        }

        public static bool Execute(string sourceAssembly, string activationAppPath, string flagsString, ILogger logger)
        {
            string operationName = "C#/XAML for HTML5: CompatibilityAnalyzer";
            try
            {
                List<UnsupportedMethodInfo> outputListOfUnsupportedMethods = new List<UnsupportedMethodInfo>();
                AssemblyDefinition assembly = LoadAssembly(sourceAssembly);
                AssemblyDefinition[] assemblies = new AssemblyDefinition[] { assembly };

                HashSet<string> flags = (flagsString != null ? new HashSet<string>(flagsString.Split(';')) : new HashSet<string>());

                var checkIfUnsupportedMethods = new CheckingThatNoUnsupportedMethodIsCalled();
                checkIfUnsupportedMethods.InitializeListOfSupportedMethods(new List<string>(), logger);

                //--------------------------------------------
                // CHECK FOR UNSUPPORTED METHODS:
                //--------------------------------------------
                checkIfUnsupportedMethods.Check(assemblies, "", "", "mscorlib", flags, 
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
                                logger.WriteError(unsupportedMethodInfo.ExplanationToDisplayInErrorsWindow);
                        }
                        else
                        {
                            // Cause an error to appear and the compilation to abort:
                            logger.WriteError(unsupportedMethodInfo.ExplanationToDisplayInErrorsWindow);
                            outputListOfUnsupportedMethods.Add(unsupportedMethodInfo);
                        }
#else
                        // Cause an error to appear and the compilation to abort:
                        logger.WriteError(unsupportedMethodInfo.ExplanationToDisplayInErrorsWindow);
                        outputListOfUnsupportedMethods.Add(unsupportedMethodInfo);
#endif
                    });

#if REQUIRE_ACTIVATION_FOR_USING_CERTAIN_METHODS
                // If a feature is missing (eg. the "Professional Edition" is required), display the window for activation:
                if (logger.RequiresMissingFeature)
                    ActivationHelpers.DisplayActivationApp(activationAppPath, logger.MissingFeatureId, logger.MessageForMissingFeature);
#endif

                bool isSuccess = !logger.HasErrors;
                return isSuccess;
            }
            catch (Exception ex)
            {
                logger.WriteError(operationName + " failed: " + ex.ToString());
                return false;
            }
        }

        static AssemblyDefinition LoadAssembly(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
                throw new InvalidDataException("Assembly path was empty.");

            var readerParameters = new ReaderParameters
            {
                ReadingMode = ReadingMode.Immediate,
                ReadSymbols = false
            };

            try
            {
                AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(path, readerParameters);

                if (assembly == null)
                    throw new FileNotFoundException("Could not load the assembly '" + path + "'");

                return assembly;
            }
            catch (Exception ex)
            {
                throw new Exception("Could not load the assembly '" + path + "'", ex);
            }
        }

        public class UnsupportedMethod
        {
            public string FullMethodName;
            public string CallingMethodFullName;
            public string UserAssemblyName;
            public string MethodAssemblyName;

            public UnsupportedMethod(string fullMethodName, string callingMethodFullName, string userAssemblyName, string methodAssemblyName)
            {
                this.FullMethodName = fullMethodName;
                this.CallingMethodFullName = callingMethodFullName;
                this.UserAssemblyName = userAssemblyName;
                this.MethodAssemblyName = methodAssemblyName;
            }
        }
    }
}
