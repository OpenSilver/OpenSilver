using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

namespace DotNetForHtml5.Compiler
{
    //[LoadInSeparateAppDomain]
    //[Serializable]
    public class AssemblyReferenceValidator : Task // AppDomainIsolatedTask
    {
        public Microsoft.Build.Framework.ITaskItem[] References { get; set; }

        [Required]
        public string AllowedAssemblies { get; set; }

        public string ActivationAppPath { get; set; }

        [Required]
        public string Flags { get; set; }

        [Required]
        public bool IsBridgeBasedVersion { get; set; }

#if BRIDGE
        [Required]
#endif
        public string NameOfAssembliesThatDoNotContainUserCode { get; set; }

#if BRIDGE
        [Required]
#endif
        public string ProjectDir { get; set; }

#if BRIDGE
        [Required]
#endif
        public string ReferencesPaths { get; set; }

#if BRIDGE
        [Required]
#endif
        public string TypeForwardingAssemblyPath { get; set; }

        public override bool Execute()
        {
            ILogger logger = new LoggerThatUsesTaskOutput(this);
            string operationName = "C#/XAML for HTML5: AssemblyReferenceValidator";
            try
            {
                return CheckingThatAssembliesCanBeReferenced.Check(References, AllowedAssemblies, ActivationAppPath, Flags, logger, IsBridgeBasedVersion, NameOfAssembliesThatDoNotContainUserCode, ProjectDir, ReferencesPaths, TypeForwardingAssemblyPath);
            }
            catch (CompilationExceptionWithOptions ex)
            {
                if (ex.DisplayOnlyTheMessageInTheOutputNothingElse)
                    logger.WriteError(ex.Message);
                else
                    logger.WriteError(operationName + " failed: " + ex.ToString());
                return false;
            }
            catch (Exception ex)
            {
                logger.WriteError(operationName + " failed: " + ex.ToString());
                return false;
            }

            //todo: verify also that the "ProjectReferences" (not just the "References") are of type C#/XAML for HTML5. In fact, in the current version, we do the test for the referenced DLLs, but not for the referenced projects (ie. when a project references another project).
        }
    }
}
