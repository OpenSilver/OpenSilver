using EnvDTE;
using Microsoft.VisualStudio.TemplateWizard;
using OpenSilver.TemplateWizards.AppCustomizationWindow;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Xml.Linq;

namespace OpenSilver.TemplateWizards
{
    class AppCustomizationWizard : IWizard
    {
        private const string NugetConfig = "Nuget.Config";

        private static string GetVsixFullPath(string filename)
        {
            var vsixDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (string.IsNullOrEmpty(vsixDir))
            {
                return null;
            }

            return Path.Combine(
                vsixDir,
                filename
            );
        }

        private static bool IsProjectAndSolutionSameDir(Dictionary<string, string> replacementsDictionary)
        {
            var solutionNameKey = "$specifiedsolutionname$";
            var solutionName = replacementsDictionary.ContainsKey(solutionNameKey) ? replacementsDictionary[solutionNameKey] : null;
            return string.IsNullOrEmpty(solutionName);
        }

        private static void CopyNugetConfig(Dictionary<string, string> replacementsDictionary)
        {
            try
            {
                var directoryKey = IsProjectAndSolutionSameDir(replacementsDictionary) ? "$destinationdirectory$" : "$solutiondirectory$";
                var solutionDir =  replacementsDictionary.ContainsKey(directoryKey) ? replacementsDictionary[directoryKey] : null;
                var vsixNugetConfig = GetVsixFullPath(NugetConfig);
                if (solutionDir != null && vsixNugetConfig != null)
                {
                    File.Copy(vsixNugetConfig, Path.Combine(solutionDir, NugetConfig));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nuget.Config has not been created.\r\nError Message:\r\n" + ex.Message);
            }
        }

        public void BeforeOpeningFile(ProjectItem projectItem)
        {

        }

        public void ProjectFinishedGenerating(Project project)
        {

        }

        public void ProjectItemFinishedGenerating(ProjectItem projectItem)
        {

        }

        public void RunFinished()
        {
        }

        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary,
            WizardRunKind runKind, object[] customParams)
        {
            XElement openSilverInfo = XElement.Parse(replacementsDictionary["$wizarddata$"]);

            XNamespace defaultNamespace = openSilverInfo.GetDefaultNamespace();

            string openSilverAPI = openSilverInfo.Element(defaultNamespace + "Api").Value;
            string openSilverType = openSilverInfo.Element(defaultNamespace + "Type").Value;


            AppConfigurationWindow window = new AppConfigurationWindow(openSilverType);

            // In the case of a class Library, the user has no other choices to make so we do not show the app configuration window.
            if (openSilverType != "Library")
            {
                bool? result = window.ShowDialog();
                if (!result.HasValue || !result.Value)
                {
                    throw new WizardBackoutException("OpenSilver project creation was cancelled by user");
                }
            }

            if (openSilverAPI == "Silverlight")
            {
                switch (window.OpenSilverBuildType)
                {
                    case OpenSilverBuildType.Stable:
                        replacementsDictionary.Add("$opensilverpackagename$", "OpenSilver");
                        break;
                }
            }
            else if (openSilverAPI == "UWP")
            {
                switch (window.OpenSilverBuildType)
                {
                    case OpenSilverBuildType.Stable:
                        replacementsDictionary.Add("$opensilverpackagename$", "OpenSilver.UWPCompatible");
                        break;
                }
            }
            else
            {
                throw new ArgumentNullException($"Unknown OpenSilver API '{openSilverAPI}'");
            }

            if (openSilverType == "Application")
            {
                switch (window.BlazorVersion)
                {
                    case BlazorVersion.Net6:
                        replacementsDictionary.Add("$blazortargetframework$", "net6.0");
                        replacementsDictionary.Add("$blazorpackagesversion$", "6.0.0");
                        break;
                    case BlazorVersion.Net7:
                        replacementsDictionary.Add("$blazortargetframework$", "net7.0");
                        replacementsDictionary.Add("$blazorpackagesversion$", "7.0.*");
                        break;
                }

                CopyNugetConfig(replacementsDictionary);
            }

            replacementsDictionary.Add("$opensilverpackageversion$", "1.1.0");
        }

        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }
    }
}
