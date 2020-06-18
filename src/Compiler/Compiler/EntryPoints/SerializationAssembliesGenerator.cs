using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace DotNetForHtml5.Compiler
{
    public class SerializationAssembliesGenerator : Task
    {
        [Required]
        public string IntermediateOutputDirectory { get; set; }

        [Required]
        public string SourceAssembly { get; set; }

        [Required]
        public bool IsBridgeBasedVersion { get; set; }

        [Output]
        public string SGenDirectory { get; set; }

        [Output]
        public string SGenCommandLineParameters { get; set; }

        [Output]
        public string SGenCommandLineParametersContinued { get; set; } // This is for the case where the SGEN command line parameters exceed 32,000 characters

        [Output]
        public bool SGenIsContinued { get; set; } // This is True if the SGEN command line exceeds 32,000 characters and is therefore split into "SGenCommandLineParameters" and "SGenCommandLineParametersContinued".

        public override bool Execute()
        {
            ILogger logger = new LoggerThatUsesTaskOutput(this);
            string operationName = "C#/XAML for HTML5: SerializationAssembliesGenerator";
            try
            {
                // Retrieve the full path of sgen.exe:
                string sgenFullPath;
                if (!GeneratingSerializationAssemblies.TryGetLocationOfSGenExe(out sgenFullPath)
                    || string.IsNullOrEmpty(sgenFullPath))
                    throw new Exception("Could not find the file sgen.exe: please contact support@cshtml5.com or ignore this error by adding the following line to your CSPROJ file: <CSharpXamlForHtml5SkipSerializationAssemblies>True</CSharpXamlForHtml5SkipSerializationAssemblies>");

                // Retrieve its directory:
                var sgenDirectoryLongPath = Path.GetDirectoryName(sgenFullPath);
                if (string.IsNullOrEmpty(sgenDirectoryLongPath))
                    throw new Exception("Unable to generate short 8.3 path from long path: please report this issue to support@cshtml5.com");

                //  Convert that directory to the short 8.3-filenames format, so that we don't need to surround the path with double quotes (which don't work with the MSBuild Exec tasc):
                SGenDirectory = ShortPathHelper.GetShortPathName(sgenDirectoryLongPath);

                // Generate command line parameters:
                string sgenCommandLineParameters;
                string sgenCommandLineParametersContinued;
                bool sgenIsContinued;
                bool isSuccess = GeneratingSerializationAssemblies.GenerateSgenCommandLineParameters(IntermediateOutputDirectory, SourceAssembly, new LoggerThatUsesTaskOutput(this), IsBridgeBasedVersion, out sgenCommandLineParameters, out sgenCommandLineParametersContinued, out sgenIsContinued);
                SGenCommandLineParameters = sgenCommandLineParameters;
                SGenCommandLineParametersContinued = sgenCommandLineParametersContinued;
                SGenIsContinued = sgenIsContinued;

                return isSuccess;
            }
            catch (ReflectionTypeLoadException ex)
            {
                logger.WriteError(operationName + " failed. " + Environment.NewLine + Environment.NewLine + "LOADER EXCEPTIONS:" + Environment.NewLine + Environment.NewLine + ConvertLoaderExceptionsToString(ex) + Environment.NewLine + Environment.NewLine + "GENERAL EXCEPTION:" + Environment.NewLine + Environment.NewLine + ex.ToString());
                return false;
            }
            catch (Exception ex)
            {
                logger.WriteError(operationName + " failed : " + ex.ToString());
                return false;
            }
        }

        static string ConvertLoaderExceptionsToString(ReflectionTypeLoadException ex)
        {
            if (ex.LoaderExceptions == null)
            {
                return "n/a";
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                foreach (var loadException in ex.LoaderExceptions)
                {
                    sb.AppendLine(loadException.ToString());
                }
                return sb.ToString().TrimEnd(new char[] { '\n', '\r' });
            }

        }
    }
}
