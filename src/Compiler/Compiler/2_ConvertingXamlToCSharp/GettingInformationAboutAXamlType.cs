
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
using System.Linq;
using System.Xml.Linq;

namespace OpenSilver.Compiler
{
    internal static class GettingInformationAboutXamlTypes
    {
        internal static void FixNamespaceForCompatibility(ref string assemblyName, ref string namespaceName)
        {
            if (assemblyName == null)
            {
                return;
            }

            switch (assemblyName)
            {
                case "System.Windows.Controls.Data.Input":
                    assemblyName = "OpenSilver.Controls.Data.Input";
                    return;
                case "System.Windows.Controls.Data":
                    assemblyName = "OpenSilver.Controls.Data";
                    return;
                case "System.Windows.Controls.Data.DataForm.Toolkit":
                    assemblyName = "OpenSilver.Controls.Data.DataForm.Toolkit";
                    return;
                case "System.Windows.Controls.DataVisualization.Toolkit":
                    assemblyName = "OpenSilver.Controls.DataVisualization.Toolkit";
                    return;
                case "System.Windows.Controls.Navigation":
                    assemblyName = "OpenSilver.Controls.Navigation";
                    return;
                case "System.Windows.Controls.Input":
                    assemblyName = "OpenSilver.Controls.Input";
                    return;
                case "System.Windows.Controls.Layout.Toolkit":
                    assemblyName = "OpenSilver.Controls.Layout.Toolkit";
                    return;
                case "System.Windows.Interactivity":
                    assemblyName = "OpenSilver.Interactivity";
                    return;
                case "Microsoft.Expression.Interactions":
                    assemblyName = "OpenSilver.Expression.Interactions";
                    return;
                case "Microsoft.Expression.Effects":
                    assemblyName = "OpenSilver.Expression.Effects";
                    return;
                case "System.Windows.Controls.DomainServices":
                    assemblyName = "OpenRiaServices.Controls.DomainServices";
                    namespaceName = "OpenRiaServices.Controls";
                    return;
                default:
                    if (assemblyName == "System" || assemblyName.StartsWith("System."))
                    {
                        assemblyName = Constants.NAME_OF_CORE_ASSEMBLY_USING_BLAZOR;
                    }
                    return;
            }
        }

        public static void GetClrNamespaceAndLocalName(
            XName xName,
            bool enableImplicitAssemblyRedirect,
            out string namespaceName,
            out string localName,
            out string assemblyNameIfAny)
        {
            namespaceName = xName.Namespace.NamespaceName;
            localName = xName.LocalName;
            assemblyNameIfAny = null;
            if (namespaceName.ToLower().StartsWith("using:"))
            {
                string ns = namespaceName.Substring("using:".Length);
                namespaceName = ns;
            }
            else if (namespaceName.ToLower().StartsWith("clr-namespace:"))
            {
                ParseClrNamespaceDeclaration(namespaceName, out string ns, out assemblyNameIfAny);
                namespaceName = ns;
                if (enableImplicitAssemblyRedirect)
                {
                    FixNamespaceForCompatibility(ref assemblyNameIfAny, ref namespaceName);
                }
            }
            else
            {
                // If namespace is empty, use the default XAML namespace:
                if (string.IsNullOrEmpty(namespaceName))
                    namespaceName = "http://schemas.microsoft.com/winfx/2006/xaml/presentation"; //todo: instead of hard-coding this, use the default namespace that applies to the current XML node.
            }
        }

        public static void ParseClrNamespaceDeclaration(string input, out string ns, out string assemblyNameIfAny)
        {
            assemblyNameIfAny = null;
            var str = input.Substring("clr-namespace:".Length);
            int indexOfSemiColons = str.IndexOf(';');
            if (indexOfSemiColons > -1)
            {
                ns = str.Substring(0, indexOfSemiColons);
                var str2 = str.Substring(indexOfSemiColons);
                if (str2.StartsWith(";assembly="))
                    assemblyNameIfAny = str2.Substring(";assembly=".Length);
            }
            else
            {
                ns = str;
            }
        }
    }
}
