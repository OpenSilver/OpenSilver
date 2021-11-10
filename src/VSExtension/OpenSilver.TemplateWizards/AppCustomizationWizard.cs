using EnvDTE;
using Microsoft.VisualStudio.TemplateWizard;
using OpenSilver.TemplateWizards.AppCustomizationWindow;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace OpenSilver.TemplateWizards
{
    class AppCustomizationWizard : IWizard
    {
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

        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            XElement openSilverInfo = XElement.Parse(replacementsDictionary["$wizarddata$"]);

            XNamespace defaultNamespace = openSilverInfo.GetDefaultNamespace();

            string openSilverAPI = openSilverInfo.Element(defaultNamespace + "Api").Value;
            string openSilverType = openSilverInfo.Element(defaultNamespace + "Type").Value;


            AppConfigurationWindow window = new AppConfigurationWindow(openSilverType);

            if (openSilverType != "Library") // In the case of a class Library, the user has no other choices to make so we do not show the app configuration window.
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
                    case BlazorVersion.Net5:
                        replacementsDictionary.Add("$blazortargetframework$", "net5.0");
                        replacementsDictionary.Add("$blazorpackagesversion$", "5.0.7");
                        break;
                    case BlazorVersion.Net6:
                        replacementsDictionary.Add("$blazortargetframework$", "net6.0");
                        replacementsDictionary.Add("$blazorpackagesversion$", "6.0.0");
                        break;
                }
            }


            replacementsDictionary.Add("$opensilverpackageversion$", "1.0.0");
        }

        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }
    }
}
