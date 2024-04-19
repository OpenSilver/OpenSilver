
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
        private const string Using = "using:";
        private const string ClrNamespace = "clr-namespace:";
        private const string Assembly = ";assembly=";

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
                case "System.Windows.Controls.Data.Toolkit":
                    assemblyName = "OpenSilver.Controls.Data.Toolkit";
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
                case "System.Windows.Controls.Theming.Toolkit":
                    assemblyName = "OpenSilver.Controls.Theming.Toolkit";
                    return;
                case "System.Windows.Controls.Theming.BubbleCreme":
                    assemblyName = "OpenSilver.Controls.Theming.BubbleCreme";
                    return;
                case "System.Windows.Controls.Theming.BureauBlack":
                    assemblyName = "OpenSilver.Controls.Theming.BureauBlack";
                    return;
                case "System.Windows.Controls.Theming.BureauBlue":
                    assemblyName = "OpenSilver.Controls.Theming.BureauBlue";
                    return;
                case "System.Windows.Controls.Theming.ExpressionDark":
                    assemblyName = "OpenSilver.Controls.Theming.ExpressionDark";
                    return;
                case "System.Windows.Controls.Theming.ExpressionLight":
                    assemblyName = "OpenSilver.Controls.Theming.ExpressionLight";
                    return;
                case "System.Windows.Controls.Theming.RainierOrange":
                    assemblyName = "OpenSilver.Controls.Theming.RainierOrange";
                    return;
                case "System.Windows.Controls.Theming.RainierPurple":
                    assemblyName = "OpenSilver.Controls.Theming.RainierPurple";
                    return;
                case "System.Windows.Controls.Theming.ShinyBlue":
                    assemblyName = "OpenSilver.Controls.Theming.ShinyBlue";
                    return;
                case "System.Windows.Controls.Theming.ShinyRed":
                    assemblyName = "OpenSilver.Controls.Theming.ShinyRed";
                    return;
                case "System.Windows.Controls.Theming.TwilightBlue":
                    assemblyName = "OpenSilver.Controls.Theming.TwilightBlue";
                    return;
                case "System.Windows.Controls.Theming.WhistlerBlue":
                    assemblyName = "OpenSilver.Controls.Theming.WhistlerBlue";
                    return;
                case "System.Windows.Controls.Theming.SystemColors":
                    assemblyName = "OpenSilver.Controls.Theming.SystemColors";
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
                case "System":
                case "System.Windows":
                case "System.Windows.Browser":
                case "System.Windows.Controls":
                case "System.Windows.Controls.Toolkit":
                case "System.Windows.Controls.Input.Toolkit":
                case "System.Windows.Data":
                    assemblyName = Constants.NAME_OF_CORE_ASSEMBLY_USING_BLAZOR;
                    return;
            }
        }

        public static (string NamespaceName, string AssemblyName) GetClrNamespaceAndAssembly(string ns, bool enableImplicitAssemblyRedirect)
        {
            if (ns.StartsWith(Using, StringComparison.OrdinalIgnoreCase))
            {
                return (ns.Substring(Using.Length), null);
            }
            else if (ns.StartsWith(ClrNamespace, StringComparison.OrdinalIgnoreCase))
            {
                ParseClrNamespaceDeclaration(ns, out string clrNamespace, out string assemblyName);
                if (enableImplicitAssemblyRedirect)
                {
                    FixNamespaceForCompatibility(ref assemblyName, ref clrNamespace);
                }
                return (clrNamespace, assemblyName);
            }
            else if (string.IsNullOrEmpty(ns))
            {
                return ("http://schemas.microsoft.com/winfx/2006/xaml/presentation", null);
            }
            else
            {
                return (ns, null);
            }
        }

        public static void GetClrNamespaceAndLocalName(
            XName xName,
            bool enableImplicitAssemblyRedirect,
            out string namespaceName,
            out string localName,
            out string assemblyNameIfAny)
        {
            localName = xName.LocalName;
            (namespaceName, assemblyNameIfAny) = GetClrNamespaceAndAssembly(xName.NamespaceName, enableImplicitAssemblyRedirect);
        }

        public static void ParseClrNamespaceDeclaration(string input, out string ns, out string assemblyNameIfAny)
        {
            assemblyNameIfAny = null;
            var str = input.Substring(ClrNamespace.Length);
            int indexOfSemiColons = str.IndexOf(';');
            if (indexOfSemiColons > -1)
            {
                ns = str.Substring(0, indexOfSemiColons);
                var str2 = str.Substring(indexOfSemiColons);
                if (str2.StartsWith(Assembly))
                    assemblyNameIfAny = str2.Substring(Assembly.Length);
            }
            else
            {
                ns = str;
            }
        }
    }
}
