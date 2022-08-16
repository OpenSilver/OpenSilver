

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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace DotNetForHtml5.Compiler
{
    //[LoadInSeparateAppDomain]
    //[Serializable]
    public class BeforeXamlPreprocessor : Task // AppDomainIsolatedTask
    {
        [Required]
        public bool IsSecondPass { get; set; }

        [Required]
        public string Flags { get; set; }

#if BRIDGE && !CSHTML5BLAZOR
        [Required]
#endif
        public string ReferencesPaths { get; set; }
        public string SourceAssemblyForPass2 { get; set; }

        [Required]
        public bool IsBridgeBasedVersion { get; set; }

#if BRIDGE && !CSHTML5BLAZOR
        [Required]
#endif
        public string NameOfAssembliesThatDoNotContainUserCode { get; set; }

        public bool IsProcessingCSHTML5Itself { get; set; }

#if BRIDGE && !CSHTML5BLAZOR
        [Required]
#endif
        public string TypeForwardingAssemblyPath { get; set; }

#if CSHTML5BLAZOR
        // the assemblies are setted by the AssemblyReferenceValidator
        public Microsoft.Build.Framework.ITaskItem[] ResolvedReferences { get; set; }
#endif

        public override bool Execute()
        {
#if CSHTML5BLAZOR
            return Execute(IsSecondPass, Flags, ResolvedReferences, SourceAssemblyForPass2, NameOfAssembliesThatDoNotContainUserCode, IsBridgeBasedVersion, IsProcessingCSHTML5Itself, new LoggerThatUsesTaskOutput(this), TypeForwardingAssemblyPath);
#else
            return Execute(IsSecondPass, Flags, ReferencesPaths, SourceAssemblyForPass2, NameOfAssembliesThatDoNotContainUserCode, IsBridgeBasedVersion, IsProcessingCSHTML5Itself, new LoggerThatUsesTaskOutput(this), TypeForwardingAssemblyPath);
#endif
        }

#if CSHTML5BLAZOR
        public static bool Execute(bool isSecondPass, string flagsString, ITaskItem[] resolvedReferences, string sourceAssemblyForPass2, string nameOfAssembliesThatDoNotContainUserCode, bool isBridgeBasedVersion, bool isProcessingCSHTML5Itself, ILogger logger, string typeForwardingAssemblyPath)
#else
        public static bool Execute(bool isSecondPass, string flagsString, string referencePathsString, string sourceAssemblyForPass2, string nameOfAssembliesThatDoNotContainUserCode, bool isBridgeBasedVersion, bool isProcessingCSHTML5Itself, ILogger logger, string typeForwardingAssemblyPath)
#endif

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
                    ReflectionOnSeparateAppDomainHandler.Current = new ReflectionOnSeparateAppDomainHandler(typeForwardingAssemblyPath);
                    ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain = ReflectionOnSeparateAppDomainHandler.Current;

#if BRIDGE
                    //todo: if we are compiling CSHTML5 itself (or CSHTML5.Stubs), we need to process the XAML files in CSHTML5,
                    // and for that we need to load the XAML types, so we need to load the previous version of CSHTML5 (from
                    // the NuGet package). Note: this is not supposed to lead to a circular reference because it is only used
                    // for the XamlPreprocessor to generate the .xaml.g.cs files from the .xaml files.
                    // To do so, we need to stop skipping the processing of the CSHTML5 and CSHTML5.Stubs assemblies (c.f.
                    // "Skip the assembly if it is not a user assembly" in "LoadAndProcessReferencedAssemblies").
#endif
                    // we load the source assembly early in case we are processing the CSHTML5.
                    if (isSecondPass && isProcessingCSHTML5Itself)
                    {
                        reflectionOnSeparateAppDomain.LoadAssembly(sourceAssemblyForPass2, loadReferencedAssembliesToo: true, isBridgeBasedVersion: isBridgeBasedVersion, isCoreAssembly: false, nameOfAssembliesThatDoNotContainUserCode: nameOfAssembliesThatDoNotContainUserCode, skipReadingAttributesFromAssemblies: false);
                    }
#if CSHTML5BLAZOR
                    // work-around: reference path string is not correctly setted so we set it manually
                    string referencePathsString = OpenSilverHelper.ReferencePathsString(resolvedReferences);
#endif
                    // Retrieve paths of referenced .dlls and load them:
                    HashSet<string> referencePaths = (referencePathsString != null) ? new HashSet<string>(referencePathsString.Split(';')) : new HashSet<string>();

                    referencePaths.RemoveWhere(s => !s.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase) || s.Contains("DotNetBrowser") || s.EndsWith(@"\bridge.dll", StringComparison.InvariantCultureIgnoreCase));

                    var coreAssembliesPaths = referencePaths.Where(AssembliesLoadHelper.IsCoreAssembly).ToArray();

                    // Note: we ensure that the Core assemblies are loaded first so that types such as "XmlnsDefinitionAttribute" are known when loading the other assemblies.
                    foreach (var coreAssemblyName in AssembliesLoadHelper.CoreAssembliesNames)
                    {
                        var coreAssemblyPath = coreAssembliesPaths.FirstOrDefault(path => coreAssemblyName.Equals(Path.GetFileNameWithoutExtension(path), StringComparison.InvariantCultureIgnoreCase));
                        if (coreAssemblyPath != null)
                        {
                            reflectionOnSeparateAppDomain.LoadAssembly(coreAssemblyPath, loadReferencedAssembliesToo: false, isBridgeBasedVersion: isBridgeBasedVersion, isCoreAssembly: false, nameOfAssembliesThatDoNotContainUserCode: nameOfAssembliesThatDoNotContainUserCode, skipReadingAttributesFromAssemblies: false);
                            referencePaths.Remove(coreAssemblyPath);
                        }
                    }

                    foreach (string referencedAssembly in referencePaths)
                    {
                        reflectionOnSeparateAppDomain.LoadAssembly(referencedAssembly, loadReferencedAssembliesToo: false, isBridgeBasedVersion: isBridgeBasedVersion, isCoreAssembly: false, nameOfAssembliesThatDoNotContainUserCode: nameOfAssembliesThatDoNotContainUserCode, skipReadingAttributesFromAssemblies: false);
                    }

                    // Load "mscorlib.dll" too (this is useful for resolving Mscorlib types in XAML, such as <system:String x:Key="TestString" xmlns:system="clr-namespace:System;assembly=mscorlib">Test</system:String>)
                    reflectionOnSeparateAppDomain.LoadAssemblyMscorlib(isBridgeBasedVersion: isBridgeBasedVersion, isCoreAssembly: false, nameOfAssembliesThatDoNotContainUserCode: nameOfAssembliesThatDoNotContainUserCode);

                    // Load for reflection the source assembly itself and the referenced assemblies if second path:
                    if (isSecondPass && !isProcessingCSHTML5Itself)
                    {
                        reflectionOnSeparateAppDomain.LoadAssembly(sourceAssemblyForPass2, loadReferencedAssembliesToo: true, isBridgeBasedVersion: isBridgeBasedVersion, isCoreAssembly: false, nameOfAssembliesThatDoNotContainUserCode: nameOfAssembliesThatDoNotContainUserCode, skipReadingAttributesFromAssemblies: false);
                    }

                    bool isSuccess = true;

                    //------- DISPLAY THE PROGRESS -------
                    logger.WriteMessage(operationName + (isSuccess ? " completed in " + executionTimeMeasuring.StopAndGetTimeInSeconds() + " seconds." : " failed.") + "\". IsSecondPass: " + isSecondPass.ToString() + ". Source assembly file: \"" + (sourceAssemblyForPass2 ?? "").ToString());

                    return isSuccess;
                }
            }
            catch (Exception ex)
            {
                if (ReflectionOnSeparateAppDomainHandler.Current != null)
                {
                    ReflectionOnSeparateAppDomainHandler.Current.Dispose();
                }

                logger.WriteError(operationName + " failed: " + ex.ToString());
                return false;
            }
        }
    }
}
