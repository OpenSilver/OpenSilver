

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



using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DotNetForHtml5.Compiler
{
    internal static class CheckingThatAssembliesCanBeReferenced
    {
        enum ResultEnum
        {
            ReferenceIsOK, ReferenceIsNotCompatible, ReferenceRequiresNewerVersionOfTheCompiler
#if REQUIRE_PRO_EDITION_FOR_REFERENCING_ASSEMBLIES
                , ReferenceRequiresToUpgradeToTheProEdition
#endif
        }

        public static bool Check(Microsoft.Build.Framework.ITaskItem[] references, string allowedReferencesAsString, string activationAppPath, string flagsString, ILogger logger, bool isBridgeBasedVersion, string nameOfAssembliesThatDoNotContainUserCode, string projectDir, string referencesPaths, string typeForwardingAssemblyPath)
        {
            List<string> referencesThatAreOK = new List<string>();
            List<string> referencesThatAreNotCompatible = new List<string>();
#if REQUIRE_PRO_EDITION_FOR_REFERENCING_ASSEMBLIES
            List<string> referencesThatRequireToUpgradeToTheProEdition = new List<string>();
#endif
            List<string> referencesThatRequireNewerVersionOfTheCompiler = new List<string>();
            string overall_minimumRequiredCompilerVersionFriendlyNameIfAny = null;

            HashSet<string> flags = (flagsString != null ? new HashSet<string>(flagsString.Split(';')) : new HashSet<string>());

            foreach (Microsoft.Build.Framework.ITaskItem taskItem in (references ?? new Microsoft.Build.Framework.ITaskItem[] { }))
            {
                string referencedName = GetAssemblyNameFromIdentity(taskItem.GetMetadata("identity"));  //Note: "GetMetadata" and "GetAssemblyName" never return null.
                string referencedHintPathIfAny = taskItem.GetMetadata("HintPath");  //Note: "GetMetadata" never return null.

                ResultEnum result;
                string minimumRequiredCompilerVersionFriendlyNameIfAny;
                IsReferenceAcceptable(referencedName, referencedHintPathIfAny, allowedReferencesAsString, flags, isBridgeBasedVersion, nameOfAssembliesThatDoNotContainUserCode, projectDir, referencesPaths, typeForwardingAssemblyPath, out result, out minimumRequiredCompilerVersionFriendlyNameIfAny);
                switch (result)
                {
                    case ResultEnum.ReferenceIsOK:
                        referencesThatAreOK.Add(referencedName);
                        break;
                    case ResultEnum.ReferenceIsNotCompatible:
                        referencesThatAreNotCompatible.Add(referencedName);
                        break;
                    case ResultEnum.ReferenceRequiresNewerVersionOfTheCompiler:
                        referencesThatRequireNewerVersionOfTheCompiler.Add(referencedName);
                        overall_minimumRequiredCompilerVersionFriendlyNameIfAny = minimumRequiredCompilerVersionFriendlyNameIfAny;
                        break;
#if REQUIRE_PRO_EDITION_FOR_REFERENCING_ASSEMBLIES
                    case ResultEnum.ReferenceRequiresToUpgradeToTheProEdition:
                        referencesThatRequireToUpgradeToTheProEdition.Add(referencedName);
                        break;
#endif
                    default:
                        throw new NotSupportedException();
                }
            }

            if (referencesThatAreNotCompatible.Count == 1)
            {
                logger.WriteError(string.Format("Please remove the reference \"{0}\" from the Project references.  - Note: a CSHTML5 project can only reference assemblies compiled with CSHTML5. Sometimes Visual Studio automatically adds references such as \"System\" but those references should be removed. Other errors below can be ignored.", referencesThatAreNotCompatible[0]));
                return false;
            }
            else if (referencesThatAreNotCompatible.Count > 1)
            {
                logger.WriteError(string.Format("Please remove the following references from the Project references: {0}  - Note: a CSHTML5 project can only reference assemblies compiled with CSHTML5. Sometimes Visual Studio automatically adds references such as \"System\" but those references should be removed. Other errors below can be ignored.", string.Join(", ", referencesThatAreNotCompatible)));
                return false;
            }
            else if (referencesThatRequireNewerVersionOfTheCompiler.Count > 0)
            {
                if (referencesThatRequireNewerVersionOfTheCompiler.Count == 1)
                    logger.WriteError(string.Format("The referenced assembly '{1}' requires a newer version of the CSHTML5 compiler ({0} or newer). You can download it from: http://www.cshtml5.com", overall_minimumRequiredCompilerVersionFriendlyNameIfAny ?? "", referencesThatRequireNewerVersionOfTheCompiler[0]));
                else
                    logger.WriteError(string.Format("The following referenced assemblies require a newer version of the CSHTML5 compiler ({0} or newer): {1}", overall_minimumRequiredCompilerVersionFriendlyNameIfAny ?? "", string.Join(", ", referencesThatRequireNewerVersionOfTheCompiler)));
                return false;
            }
#if REQUIRE_PRO_EDITION_FOR_REFERENCING_ASSEMBLIES
            else if (referencesThatRequireToUpgradeToTheProEdition.Count > 0)
            {
                // Display the ActivationApp:
                string explanationToDisplayInActivationApp = string.Format("Referencing compiled DLLs such as '{0}' requires the Professional Edition of CSHTML5. You can work around this limitation if you have the source code of '{0}': just add the project to the solution and reference it instead of referencing the compiled DLL. With the Free Edition, projects can reference other projects in the same solution, but they cannot reference external compiled DLLs. Upgrade to the Pro Edition to remove all the limitations.", referencesThatRequireToUpgradeToTheProEdition[0] + ".dll");
                //string explanationToDisplayInActivationApp = string.Format("With the Free Edition, projects can reference other projects in the same solution, but referencing a compiled DLL such as \"{0}\" requires the Professional Edition of C#/XAML for HTML5.", referencesThatRequireToUpgradeToTheProEdition[0] + ".dll");
                string explanationToDisplayInErrorsWindow = explanationToDisplayInActivationApp + " It can be obtained from http://www.cshtml5.com - Please rebuild the project to try again.";
                ActivationHelpers.DisplayActivationApp(activationAppPath, Constants.PROFESSIONAL_EDITION_FEATURE_ID, explanationToDisplayInActivationApp);

                // If we are in trial mode, we can continue without displaying any errors, otherwise we must stop compilation and raise the error:
                int unused;
                if (TrialHelpers.IsTrial(Constants.SL_MIGRATION_EDITION_FEATURE_ID, out unused) == TrialHelpers.TrialStatus.Running)
                {
                    return true;
                }
                else
                {
                    if (TrialHelpers.IsTrial(Constants.PROFESSIONAL_EDITION_FEATURE_ID, out unused) == TrialHelpers.TrialStatus.Running)
                    {
                        return true;
                    }
                    else
                    {
                        logger.WriteError(explanationToDisplayInErrorsWindow);
                        return false;
                    }
                }
            }
#endif
            else
                return true;
        }

        static string GetAssemblyNameFromIdentity(string assemblyIdentity)
        {
            // Never returns null

            if (assemblyIdentity != null)
                return assemblyIdentity.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0];
            else
                return string.Empty;
        }

        static void IsReferenceAcceptable(string referenceName, string referenceHintPath, string allowedAssemblies, HashSet<string> flags, bool isBridgeBasedVersion, string nameOfAssembliesThatDoNotContainUserCode, string projectDir, string referencesPaths, string typeForwardingAssemblyPath, out ResultEnum result, out string minimumRequiredCompilerVersionFriendlyNameIfAny)
        {
            minimumRequiredCompilerVersionFriendlyNameIfAny = null;

            //--------------------------------------------------------------------------------
            // This method will verify that the referenced DLL is itself of type C#/XAML for HTML5, and that the minimum version number is OK.
            //--------------------------------------------------------------------------------

#if !BRIDGE
            // Check if the reference is a file that belongs to "C#/XAML for HTML5":
            if (referenceName.ToLower().Contains(Constants.LOWERCASE_CORE_ASSEMBLY_NAME)
                || (!string.IsNullOrEmpty(referenceHintPath) && (Path.GetFileName(referenceHintPath)).ToLower().Contains(Constants.LOWERCASE_CORE_ASSEMBLY_NAME)))
            {
                result = ResultEnum.ReferenceIsOK;
                return;
            }
#else
            // In the Bridge version, we use the "AllowedAssemblies" below to allow the CSHTML5 assemblies.
#endif

            // Check if the reference is among the specially allowed ones:
            string[] allowedAssembliesArray = allowedAssemblies != null ? allowedAssemblies.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { };
            foreach (var allowedAssembly in allowedAssembliesArray)
            {
                if (referenceName.ToLower() == allowedAssembly.ToLower())
                {
                    result = ResultEnum.ReferenceIsOK;
                    return;
                }
            }

            // Otherwise, check if the reference was compiled with "C#/XAML for HTML5"
            // and if the user is running the Professional Edition
            // and if the version of the compiler is OK:
            if (!string.IsNullOrEmpty(referenceHintPath))
            {
                // Perform reflection on a separate AppDomain so that the types loaded for reflection can be unloaded when done.
                using (var reflectionOnSeparateAppDomain = new ReflectionOnSeparateAppDomainHandler(typeForwardingAssemblyPath))
                {
#if BRIDGE
                    // In the Bridge.NET version, we use "Assembly.LoadFile" instead of "Assembly.LoadFrom", so we need to convert a Relative path into an Absolute path:
                    if (!Path.IsPathRooted(referenceHintPath))
                        referenceHintPath = Path.Combine(projectDir, referenceHintPath);

                    // Verify that the file was found:
                    if (!File.Exists(referenceHintPath))
                        throw new CompilationExceptionWithOptions("File not found: " + referenceHintPath) { DisplayOnlyTheMessageInTheOutputNothingElse = true };

                    // In the Bridge.NET version, we need to preload the CSHTML5 assemblies so that it can find types such as "XmlnsDefinitionAttribute" or "CompilerVersionNumberAttribute":
                    HashSet<string> referencesPathsHasSet = (referencesPaths != null) ? new HashSet<string>(referencesPaths.Split(';')) : new HashSet<string>();
                    referencesPathsHasSet.RemoveWhere(s => !s.ToLower().EndsWith(".dll") || s.Contains("DotNetBrowser") || s.ToLower().EndsWith(@"\bridge.dll"));
                    foreach (string referencedAssembly in AssembliesLoadHelper.EnsureCoreAssemblyIsFirstInList(referencesPathsHasSet)) // Note: we ensure that the Core assembly is loaded first so that types such as "XmlnsDefinitionAttribute" are known when loading the other assemblies.
                    {
                        reflectionOnSeparateAppDomain.LoadAssembly(referencedAssembly, loadReferencedAssembliesToo: false, isBridgeBasedVersion: isBridgeBasedVersion, isCoreAssembly: false, nameOfAssembliesThatDoNotContainUserCode: nameOfAssembliesThatDoNotContainUserCode);
                    }
#endif

                    string assemblySimpleName = reflectionOnSeparateAppDomain.LoadAssembly(referenceHintPath, loadReferencedAssembliesToo: false, isBridgeBasedVersion: isBridgeBasedVersion, isCoreAssembly: false, nameOfAssembliesThatDoNotContainUserCode: nameOfAssembliesThatDoNotContainUserCode, skipReadingAttributesFromAssemblies: false);


                    //-----------------------------------------------------------------------------
                    // Check if the reference was compiled with "C#/XAML for HTML5" by reading the "C#/XAML for HTML5" compiler version attributes:
                    //-----------------------------------------------------------------------------

                    string compilerVersionNumber = reflectionOnSeparateAppDomain.GetCSharpXamlForHtml5CompilerVersionNumberOrNull(assemblySimpleName);
                    string compilerVersionFriendlyName = reflectionOnSeparateAppDomain.GetCSharpXamlForHtml5CompilerVersionFriendlyNameOrNull(assemblySimpleName);

                    // If at least one of those attributes exists, it means that the assembly was compiled with C#/XAML fot HTML5:
                    bool wasCompiledWithCSharpXamlForHtml5 = (compilerVersionNumber != null || compilerVersionFriendlyName != null);

                    if (wasCompiledWithCSharpXamlForHtml5)
                    {
                        // The reference is OK, it was compiled with "C#/XAML for HTML5".


                        //----------------------------------------------------------------------
                        // Now check if the "minimum compiler version" (if any) required by the DLL is compatible with the current version:
                        //----------------------------------------------------------------------

                        string minimumRequiredCompilerVersionNumberIfAny = reflectionOnSeparateAppDomain.GetCSharpXamlForHtml5MinimumRequiredCompilerVersionNumberOrNull(assemblySimpleName);
                        minimumRequiredCompilerVersionFriendlyNameIfAny = reflectionOnSeparateAppDomain.GetCSharpXamlForHtml5MinimumRequiredCompilerVersionFriendlyNameOrNull(assemblySimpleName);

                        if (minimumRequiredCompilerVersionNumberIfAny == null
                            || compilerVersionNumber == null
                            || (new Version(minimumRequiredCompilerVersionNumberIfAny) <= new Version(compilerVersionNumber)))
                        {
                            // The reference is OK, the version of the "C#/XAML for HTML5" compiler is compatible.

#if REQUIRE_PRO_EDITION_FOR_REFERENCING_ASSEMBLIES
                            //-------------------------------------------------------------------------
                            // Now check if the user is running the Professional Edition, which is required for referencing non-default DLLs:
                            //-------------------------------------------------------------------------

                            if (ActivationHelpers.IsFeatureEnabled(Constants.ENTERPRISE_EDITION_FEATURE_ID, flags))
                            {
                                // It's OK to proceed, the Enterprise Edition is being used.

                                // Everything is OK:
                                result = ResultEnum.ReferenceIsOK;
                                return;

                            }
                            else if (ActivationHelpers.IsFeatureEnabled(Constants.SL_MIGRATION_EDITION_FEATURE_ID, flags))
                            {
                                // It's OK to proceed, the SL Migration Edition is being used.
                                //todo: once (if it happens) sl migration and enterprise editions will be different, add a test like for the professional edition.

                                // Everything is OK:
                                result = ResultEnum.ReferenceIsOK;
                                return;

                            }
                            else if (ActivationHelpers.IsFeatureEnabled(Constants.PROFESSIONAL_EDITION_FEATURE_ID, flags))
                            {
                                // It's OK to proceed, the Professional Edition is being used.
                                //todo: actually test what the references tries to use to make sure it limits to the professional edition

                                // Everything is OK:
                                result = ResultEnum.ReferenceIsOK;
                                return;

                            }
                            else
                            {
                                // Display the activation app and, if we are not in trial mode, stop the compilation and raise the compilation error:
                                result = ResultEnum.ReferenceRequiresToUpgradeToTheProEdition;
                                return;
                            }
#else
                            // Everything is OK:
                            result = ResultEnum.ReferenceIsOK;
                            return;
#endif
                        }
                        else
                        {
                            // A new version of the compiler is required:
                            result = ResultEnum.ReferenceRequiresNewerVersionOfTheCompiler;
                            if (minimumRequiredCompilerVersionFriendlyNameIfAny == null)
                                minimumRequiredCompilerVersionFriendlyNameIfAny = "Build " + minimumRequiredCompilerVersionNumberIfAny;
                            return;
                        }
                    }
                    else
                    {
                        // Otherwise, fail:
                        result = ResultEnum.ReferenceIsNotCompatible;
                        return;
                    }
                }
            }
            else
            {
                // Otherwise, fail:
                result = ResultEnum.ReferenceIsNotCompatible;
                return;
            }
        }
    }
}
