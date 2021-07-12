﻿using EnvDTE;
using Microsoft.VisualStudio.TemplateWizard;
using OpenSilver.TemplateConfiguration;
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

            bool? result = window.ShowDialog();
            if (!result.HasValue || !result.Value)
            {
                throw new WizardBackoutException("OpenSilver project creation was cancelled by user");
            }

            if (openSilverAPI == "Silverlight")
            {
                switch (window.OpenSilverBuildType)
                {
                    case OpenSilverBuildType.Stable:
                        replacementsDictionary.Add("$opensilverpackagename$", "OpenSilver");
                        break;
                    case OpenSilverBuildType.WorkInProgress:
                        replacementsDictionary.Add("$opensilverpackagename$", "OpenSilver.WorkInProgress");
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
                    case OpenSilverBuildType.WorkInProgress:
                        replacementsDictionary.Add("$opensilverpackagename$", "OpenSilver.UWPCompatible"); //TODO: change this when we have a UWP WorkInProgress package
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
                        replacementsDictionary.Add("$blazorpackagesversion$", "6.0.0-preview.5.21301.17");
                        break;
                }
            }
        }

        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }
    }
}
