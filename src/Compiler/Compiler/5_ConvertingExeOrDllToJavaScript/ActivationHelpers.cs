

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



using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetForHtml5.Compiler
{
    internal static class ActivationHelpers
    {
        internal static void DisplayActivationApp(string activationAppPath, string missingFeatureId, string messageForMissingFeature)
        {
            string fullPathToActivationApp = Path.Combine(Directory.GetCurrentDirectory(), activationAppPath);
            string arguments = "/custom " + missingFeatureId + " " + messageForMissingFeature;

            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = Path.GetFileName(fullPathToActivationApp);
            psi.WorkingDirectory = Path.GetDirectoryName(fullPathToActivationApp);
            psi.Arguments = arguments;
            Process.Start(psi);

            //System.Diagnostics.Process.Start(fullPathToActivationApp, arguments);

            //System.Diagnostics.Process executable = new System.Diagnostics.Process();
            //executable.StartInfo.FileName = activationAppPath;
            //executable.Start();
            //executable.WaitForExit();

            /*
            System.Threading.AutoResetEvent are = new System.Threading.AutoResetEvent(false);
            var thread = new System.Threading.Thread(() =>
            {
                DotNetForHtml5.Licensing.ActivationApp.MainWindow mw = new Licensing.ActivationApp.MainWindow();
                mw.ShowInTaskbar = true;
                mw.Topmost = true;
                mw.ShowDialog();
                are.Set();
            });
            thread.SetApartmentState(System.Threading.ApartmentState.STA);
            thread.Start();
            are.WaitOne();
            */
        }

#if REQUIRE_ACTIVATION_FOR_USING_CERTAIN_METHODS
        internal static bool IsMethodAllowed(MemberReference methodReference, out string explanationToDisplayInErrorsWindow, out string explanationToDisplayInActivationApp, out string missingFeatureId)
        {
            string typeNameWithNamespace = methodReference.DeclaringType.FullName;
            string methodName = methodReference.Name;

            if (!IsMethodOKWithoutEnterpriseVersion(typeNameWithNamespace, methodName, out explanationToDisplayInErrorsWindow, out explanationToDisplayInActivationApp, out missingFeatureId))
            {
                return false;
            }
            if (!IsMethodOKWithoutSLMigrationVersion(typeNameWithNamespace, methodName, out explanationToDisplayInErrorsWindow, out explanationToDisplayInActivationApp, out missingFeatureId))
            {
                return false;
            }
            if (!IsMethodOKWithoutProfessionalVersion(typeNameWithNamespace, methodName, out explanationToDisplayInErrorsWindow, out explanationToDisplayInActivationApp, out missingFeatureId))
            {
                return false;
            }
            return true;
        }

        internal static bool IsMethodOKWithoutProfessionalVersion(string typeNameWithNamespace, string methodName, out string explanationToDisplayInErrorsWindow, out string explanationToDisplayInActivationApp, out string missingFeatureId)
        {
            if (typeNameWithNamespace == "System.Net.WebClient" && methodName == ".ctor")
            {
                explanationToDisplayInErrorsWindow = "The WebClient class requires the Professional Edition of C#/XAML for HTML5. It can be obtained from http://www.cshtml5.com - Please rebuild the project to try again.";
                explanationToDisplayInActivationApp = "The WebClient class requires the Professional Edition of C#/XAML for HTML5.";
                missingFeatureId = Constants.PROFESSIONAL_EDITION_FEATURE_ID;
                return false;
            }
            if (typeNameWithNamespace == "CSHTML5.Native.JavaScript.Networking.XMLHttpRequest" && methodName == ".ctor")
            {
                explanationToDisplayInErrorsWindow = "The XMLHttpRequest class requires the Professional Edition of C#/XAML for HTML5. It can be obtained from http://www.cshtml5.com - Please rebuild the project to try again.";
                explanationToDisplayInActivationApp = "The XMLHttpRequest class requires the Professional Edition of C#/XAML for HTML5.";
                missingFeatureId = Constants.PROFESSIONAL_EDITION_FEATURE_ID;
                return false;
            }
            else if (typeNameWithNamespace.StartsWith("System.ServiceModel.INTERNAL_WebMethodsCaller") && methodName == "CallWebMethod"
                || typeNameWithNamespace.StartsWith("System.ServiceModel.INTERNAL_WebMethodsCaller") && methodName == "CallWebMethodAsync"
                || typeNameWithNamespace.StartsWith("System.ServiceModel.INTERNAL_WebMethodsCaller") && methodName == "CallWebMethod_WithoutReturnValue"
                || typeNameWithNamespace.StartsWith("System.ServiceModel.INTERNAL_WebMethodsCaller") && methodName == "CallWebMethodAsync_WithoutReturnValue")
            {
                explanationToDisplayInErrorsWindow = "WCF and Service References require the Professional Edition of C#/XAML for HTML5. It can be obtained from http://www.cshtml5.com - Please rebuild the project to try again.";
                explanationToDisplayInActivationApp = "WCF and Service References require the Professional Edition of C#/XAML for HTML5.";
                missingFeatureId = Constants.PROFESSIONAL_EDITION_FEATURE_ID;
                return false;
            }
            else if ((typeNameWithNamespace == "System.Windows.Controls.DataGrid" && methodName == ".ctor")
                || (typeNameWithNamespace == "Windows.UI.Xaml.Controls.DataGrid" && methodName == ".ctor"))
            {
                explanationToDisplayInErrorsWindow = "The DataGrid control requires the Professional Edition of C#/XAML for HTML5. It can be obtained from http://www.cshtml5.com - Please rebuild the project to try again.";
                explanationToDisplayInActivationApp = "The DataGrid control requires the Professional Edition of C#/XAML for HTML5.";
                missingFeatureId = Constants.PROFESSIONAL_EDITION_FEATURE_ID;
                return false;
            }
            else if (typeNameWithNamespace == "CSHTML5.Native.Html.Controls.HtmlPresenter" && methodName == ".ctor")
            {
                explanationToDisplayInErrorsWindow = "The HtmlPresenter control requires the Professional Edition of C#/XAML for HTML5. It can be obtained from http://www.cshtml5.com - Please rebuild the project to try again.";
                explanationToDisplayInActivationApp = "The HtmlPresenter control requires the Professional Edition of C#/XAML for HTML5.";
                missingFeatureId = Constants.PROFESSIONAL_EDITION_FEATURE_ID;
                return false;
            }
            else if (typeNameWithNamespace.StartsWith("CSHTML5.Interop") && methodName == "ExecuteJavaScript")
            {
                explanationToDisplayInErrorsWindow = "The 'ExecuteJavaScript' method, for calling JS code from C#, requires the Professional Edition of C#/XAML for HTML5. It can be obtained from http://www.cshtml5.com - Please rebuild the project to try again.";
                explanationToDisplayInActivationApp = "The 'ExecuteJavaScript' method, for calling JS code from C#, requires the Professional Edition of C#/XAML for HTML5.";
                missingFeatureId = Constants.PROFESSIONAL_EDITION_FEATURE_ID;
                return false;
            }
            else if (typeNameWithNamespace.StartsWith("CSHTML5.Interop"))
            {
                explanationToDisplayInErrorsWindow = "The methods in the 'CSHTML5.Interop' namespace, which allow calling JS code from C#, require the Professional Edition of C#/XAML for HTML5. It can be obtained from http://www.cshtml5.com - Please rebuild the project to try again.";
                explanationToDisplayInActivationApp = "The methods in the 'CSHTML5.Interop' namespace, which allow calling JS code from C#, require the Professional Edition of C#/XAML for HTML5.";
                missingFeatureId = Constants.PROFESSIONAL_EDITION_FEATURE_ID;
                return false;
            }

            explanationToDisplayInErrorsWindow = string.Empty;
            explanationToDisplayInActivationApp = string.Empty;
            missingFeatureId = string.Empty;
            return true;
        }

        internal static bool IsMethodOKWithoutSLMigrationVersion(string typeNameWithNamespace, string methodName, out string explanationToDisplayInErrorsWindow, out string explanationToDisplayInActivationApp, out string missingFeatureId)
        {
            explanationToDisplayInActivationApp = "";
            explanationToDisplayInErrorsWindow = "";
            missingFeatureId = "";
            return true;
        }

        internal static bool IsMethodOKWithoutEnterpriseVersion(string typeNameWithNamespace, string methodName, out string explanationToDisplayInErrorsWindow, out string explanationToDisplayInActivationApp, out string missingFeatureId)
        {
            explanationToDisplayInActivationApp = "";
            explanationToDisplayInErrorsWindow = "";
            missingFeatureId = "";
            return true;
        }
#endif

        internal static bool IsFeatureEnabled(string featureId, HashSet<string> flags)
        {
#if SPECIAL_LICENSE_FOR_APL_USERS
            // If the "apl" flag is specified, we consider that the Pro Edition features are allowed. This was made for the APL community. See the ZenDesk ticket 807 and related.
            if (flags.Contains("apl") && featureId == Constants.PROFESSIONAL_EDITION_FEATURE_ID)
            {
                return true;
            }
#endif
            //------------------------------
            // Look in the Registry:
            //------------------------------
            object value = RegistryHelpers.GetSetting("Feature_" + featureId, null);
            if (value != null)
                return true; //todo: to prevent registry tampering, we should retrieve the activation key and check with the server that it is ok.
            else
                return false;
        }

#if REQUIRE_ACTIVATION_FOR_USING_CERTAIN_METHODS
        internal static bool IsThisProjectTheSampleShowcaseApp(AssemblyDefinition userAssembly)
        {
            // This method is useful to know if the project is the sample showcase app
            // because in this case we don't require the end-user to activate the
            // professional edition to test its features.

            // We base the test on the presence of some of the typical files of the
            // sample showcase app, and we verify that the number of types contained
            // in the project is not too big.


            // Note: be sure to keep the following lists updated if the sample Showcase App changes:

            // Old showcase app:
            HashSet<string> typesToCheckInOldShowcaseApp = new HashSet<string>();
            typesToCheckInOldShowcaseApp.Add("Page1_Controls");
            typesToCheckInOldShowcaseApp.Add("Page2_Panels");

            // New showcase app:
            HashSet<string> typesToCheckInNewShowcaseApp = new HashSet<string>();
            typesToCheckInNewShowcaseApp.Add("Button_Demo");
            typesToCheckInNewShowcaseApp.Add("Border_Demo");
            typesToCheckInNewShowcaseApp.Add("Canvas_Demo");
            typesToCheckInNewShowcaseApp.Add("CheckBox_Demo");
            typesToCheckInNewShowcaseApp.Add("ComboBox_Demo");
            typesToCheckInNewShowcaseApp.Add("HyperlinkButton");
            typesToCheckInNewShowcaseApp.Add("ListBox_Demo");
            typesToCheckInNewShowcaseApp.Add("RadioButton_Demo");
            typesToCheckInNewShowcaseApp.Add("StackPanel_Demo");
            typesToCheckInNewShowcaseApp.Add("TextBlock_Demo");

            int typesCount = 0;

            foreach (Mono.Cecil.ModuleDefinition moduleDefinition in userAssembly.Modules)
            {
                foreach (Mono.Cecil.TypeDefinition typeDefinition in moduleDefinition.Types)
                {
                    string typeName = typeDefinition.Name;

                    if (typesToCheckInOldShowcaseApp.Contains(typeName))
                        typesToCheckInOldShowcaseApp.Remove(typeName);

                    if (typesToCheckInNewShowcaseApp.Contains(typeName))
                        typesToCheckInNewShowcaseApp.Remove(typeName);

                    typesCount++;
                }
            }

            bool isShowcaseApp = (typesToCheckInOldShowcaseApp.Count == 0) || (typesToCheckInNewShowcaseApp.Count < 4); // Note: we use "< 3" instead of "== 0" to give some margin to the user to modify the classes, and also in case that we rename some of those classes in future versions and forget to update this class.

            return isShowcaseApp;
        }
#endif
    }
}
