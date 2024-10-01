
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

            if (string.Equals(assemblyName, "System.Windows", StringComparison.OrdinalIgnoreCase))
            {
                assemblyName = "OpenSilver";
            }
            else if (string.Equals(assemblyName, "System.Windows.Browser", StringComparison.OrdinalIgnoreCase))
            {
                assemblyName = "OpenSilver.Browser";
            }
            else if (string.Equals(assemblyName, "System.Windows.Controls", StringComparison.OrdinalIgnoreCase))
            {
                assemblyName = "OpenSilver.Controls";
            }
            else if (string.Equals(assemblyName, "System.Windows.Controls.Data.Input", StringComparison.OrdinalIgnoreCase))
            {
                assemblyName = "OpenSilver.Controls.Data.Input";
            }
            else if (string.Equals(assemblyName, "System.Windows.Controls.Data", StringComparison.OrdinalIgnoreCase))
            {
                assemblyName = "OpenSilver.Controls.Data";
            }
            else if (string.Equals(assemblyName, "System.Windows.Controls.Data.Toolkit", StringComparison.OrdinalIgnoreCase))
            {
                assemblyName = "OpenSilver.Controls.Data.Toolkit";
            }
            else if (string.Equals(assemblyName, "System.Windows.Controls.Data.DataForm.Toolkit", StringComparison.OrdinalIgnoreCase))
            {
                assemblyName = "OpenSilver.Controls.Data.DataForm.Toolkit";
            }
            else if (string.Equals(assemblyName, "System.Windows.Controls.DataVisualization.Toolkit", StringComparison.OrdinalIgnoreCase))
            {
                assemblyName = "OpenSilver.Controls.DataVisualization.Toolkit";
            }
            else if (string.Equals(assemblyName, "System.Windows.Controls.Navigation", StringComparison.OrdinalIgnoreCase))
            {
                assemblyName = "OpenSilver.Controls.Navigation";
            }
            else if (string.Equals(assemblyName, "System.Windows.Controls.Input", StringComparison.OrdinalIgnoreCase))
            {
                assemblyName = "OpenSilver.Controls.Input";
            }
            else if (string.Equals(assemblyName, "System.Windows.Controls.Input.Toolkit", StringComparison.OrdinalIgnoreCase))
            {
                assemblyName = "OpenSilver.Controls.Input.Toolkit";
            }
            else if (string.Equals(assemblyName, "System.Windows.Controls.Layout.Toolkit", StringComparison.OrdinalIgnoreCase))
            {
                assemblyName = "OpenSilver.Controls.Layout.Toolkit";
            }
            else if (string.Equals(assemblyName, "System.Windows.Controls.Theming.Toolkit", StringComparison.OrdinalIgnoreCase))
            {
                assemblyName = "OpenSilver.Controls.Theming.Toolkit";
            }
            else if (string.Equals(assemblyName, "System.Windows.Controls.Theming.BubbleCreme", StringComparison.OrdinalIgnoreCase))
            {
                assemblyName = "OpenSilver.Controls.Theming.BubbleCreme";
            }
            else if (string.Equals(assemblyName, "System.Windows.Controls.Theming.BureauBlack", StringComparison.OrdinalIgnoreCase))
            {
                assemblyName = "OpenSilver.Controls.Theming.BureauBlack";
            }
            else if (string.Equals(assemblyName, "System.Windows.Controls.Theming.BureauBlue", StringComparison.OrdinalIgnoreCase))
            {
                assemblyName = "OpenSilver.Controls.Theming.BureauBlue";
            }
            else if (string.Equals(assemblyName, "System.Windows.Controls.Theming.ExpressionDark", StringComparison.OrdinalIgnoreCase))
            {
                assemblyName = "OpenSilver.Controls.Theming.ExpressionDark";
            }
            else if (string.Equals(assemblyName, "System.Windows.Controls.Theming.ExpressionLight", StringComparison.OrdinalIgnoreCase))
            {
                assemblyName = "OpenSilver.Controls.Theming.ExpressionLight";
            }
            else if (string.Equals(assemblyName, "System.Windows.Controls.Theming.RainierOrange", StringComparison.OrdinalIgnoreCase))
            {
                assemblyName = "OpenSilver.Controls.Theming.RainierOrange";
            }
            else if (string.Equals(assemblyName, "System.Windows.Controls.Theming.RainierPurple", StringComparison.OrdinalIgnoreCase))
            {
                assemblyName = "OpenSilver.Controls.Theming.RainierPurple";
            }
            else if (string.Equals(assemblyName, "System.Windows.Controls.Theming.ShinyBlue", StringComparison.OrdinalIgnoreCase))
            {
                assemblyName = "OpenSilver.Controls.Theming.ShinyBlue";
            }
            else if (string.Equals(assemblyName, "System.Windows.Controls.Theming.ShinyRed", StringComparison.OrdinalIgnoreCase))
            {
                assemblyName = "OpenSilver.Controls.Theming.ShinyRed";
            }
            else if (string.Equals(assemblyName, "System.Windows.Controls.Theming.TwilightBlue", StringComparison.OrdinalIgnoreCase))
            {
                assemblyName = "OpenSilver.Controls.Theming.TwilightBlue";
            }
            else if (string.Equals(assemblyName, "System.Windows.Controls.Theming.WhistlerBlue", StringComparison.OrdinalIgnoreCase))
            {
                assemblyName = "OpenSilver.Controls.Theming.WhistlerBlue";
            }
            else if (string.Equals(assemblyName, "System.Windows.Controls.Theming.SystemColors", StringComparison.OrdinalIgnoreCase))
            {
                assemblyName = "OpenSilver.Controls.Theming.SystemColors";
            }
            else if (string.Equals(assemblyName, "System.Windows.Controls.Toolkit", StringComparison.OrdinalIgnoreCase))
            {
                assemblyName = "OpenSilver.Controls.Toolkit";
            }
            else if (string.Equals(assemblyName, "System.Windows.Data", StringComparison.OrdinalIgnoreCase))
            {
                assemblyName = "OpenSilver.Data";
            }
            else if (string.Equals(assemblyName, "System.Windows.Interactivity", StringComparison.OrdinalIgnoreCase))
            {
                assemblyName = "OpenSilver.Interactivity";
            }
            else if (string.Equals(assemblyName, "Microsoft.Expression.Interactions", StringComparison.OrdinalIgnoreCase))
            {
                assemblyName = "OpenSilver.Expression.Interactions";
            }
            else if (string.Equals(assemblyName, "Microsoft.Expression.Effects", StringComparison.OrdinalIgnoreCase))
            {
                assemblyName = "OpenSilver.Expression.Effects";
            }
            else if (string.Equals(assemblyName, "System.Windows.Controls.DomainServices", StringComparison.OrdinalIgnoreCase))
            {
                assemblyName = "OpenRiaServices.Controls.DomainServices";
                namespaceName = "OpenRiaServices.Controls";
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
