
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
using System.Xml.Linq;

namespace OpenSilver.Compiler
{
    internal interface ICodeGenerator
    {
        string Generate();
    }

    internal static class GeneratingCode
    {
        internal const string DefaultXamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        internal const string LegacyXamlNamespace = "http://schemas.microsoft.com/client/2007"; // XAML namespace used for Silverlight 1.0 application
        internal const string SdkXamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation/sdk";

        internal static readonly XNamespace[] DefaultXamlNamespaces = [DefaultXamlNamespace, LegacyXamlNamespace];
        internal static readonly XNamespace xNamespace = "http://schemas.microsoft.com/winfx/2006/xaml"; // Used for example for "x:Name" attributes and {x:Null} markup extensions.

        internal static string GetAttributeValue(XAttribute attribute)
        {
            string value = attribute.Value;

            if (value is not null && value.StartsWith("{}"))
            {
                return value.Substring(2);
            }

            return value;
        }

        internal static bool IsXNameAttribute(XAttribute attr) =>
            attr.Name.LocalName == "Name" && attr.Name.NamespaceName == xNamespace;

        internal static bool IsNameAttribute(XAttribute attr) =>
            attr.Name.LocalName == "Name" && string.IsNullOrEmpty(attr.Name.NamespaceName);

        internal static string GetUniqueName(XElement element) =>
            element.Attribute(GeneratingUniqueNames.UniqueNameAttribute).Value;

        public static bool IsDataTemplate(XElement element, string assemblyName) =>
            IsXElementOfType(element, "DataTemplate", KnownNamespaces.SystemWindows, assemblyName);

        public static bool IsItemsPanelTemplate(XElement element, string assemblyName) =>
            IsXElementOfType(element, "ItemsPanelTemplate", KnownNamespaces.SystemWindowsControls, assemblyName);

        public static bool IsControlTemplate(XElement element, string assemblyName) =>
            IsXElementOfType(element, "ControlTemplate", KnownNamespaces.SystemWindowsControls, assemblyName);

        public static bool IsBinding(XElement element, string assemblyName) =>
            IsXElementOfType(element, "Binding", KnownNamespaces.SystemWindowsData, assemblyName);

        public static bool IsStyle(XElement element, string assemblyName) =>
            IsXElementOfType(element, "Style", KnownNamespaces.SystemWindows, assemblyName);

        public static bool IsTextBlock(XElement element, string assemblyName) =>
            IsXElementOfType(element, "TextBlock", KnownNamespaces.SystemWindowsControls, assemblyName);

        public static bool IsRun(XElement element, string assemblyName) =>
            IsXElementOfType(element, "Run", KnownNamespaces.SystemWindowsDocuments, assemblyName);

        public static bool IsSpan(XElement element, string assemblyName) =>
            IsXElementOfType(element, "Span", KnownNamespaces.SystemWindowsDocuments, assemblyName);

        public static bool IsItalic(XElement element, string assemblyName) =>
            IsXElementOfType(element, "Italic", KnownNamespaces.SystemWindowsDocuments, assemblyName);

        public static bool IsUnderline(XElement element, string assemblyName) =>
            IsXElementOfType(element, "Underline", KnownNamespaces.SystemWindowsDocuments, assemblyName);

        public static bool IsBold(XElement element, string assemblyName) =>
            IsXElementOfType(element, "Bold", KnownNamespaces.SystemWindowsDocuments, assemblyName);

        public static bool IsHyperlink(XElement element, string assemblyName) =>
            IsXElementOfType(element, "Hyperlink", KnownNamespaces.SystemWindowsDocuments, assemblyName);

        public static bool IsParagraph(XElement element, string assemblyName) =>
            IsXElementOfType(element, "Paragraph", KnownNamespaces.SystemWindowsDocuments, assemblyName);

        private static bool IsXElementOfType(XElement element, string typeName, string namespaceName, string processedAssemblyName)
        {
            XName name = element.Name;
            if (name.LocalName == typeName)
            {
                for (int i = 0; i < DefaultXamlNamespaces.Length; i++)
                {
                    if (name.Namespace == DefaultXamlNamespaces[i])
                    {
                        return true;
                    }
                }

                GettingInformationAboutXamlTypes.ParseClrNamespaceDeclaration(name.NamespaceName, out string ns, out string assemblyName);
                if (ns == namespaceName)
                {
                    return assemblyName == "OpenSilver" || (assemblyName == null && processedAssemblyName == "OpenSilver");
                }
            }

            return false;
        }

        public static bool IsNullExtension(XElement element)
        {
            if (element.Name.LocalName != "NullExtension")
            {
                return false;
            }

            if (element.Name.NamespaceName == xNamespace.NamespaceName ||
                element.Name.NamespaceName == DefaultXamlNamespace ||
                element.Name.NamespaceName == LegacyXamlNamespace)
            {
                return true;
            }

            (string ns, string assemblyName) = GettingInformationAboutXamlTypes.GetClrNamespaceAndAssembly(element.Name.NamespaceName);

            return ns == "System.Windows.Markup" && assemblyName == "OpenSilver";
        }

        public static bool IsStaticExtension(XElement element)
        {
            if (element.Name.LocalName != "StaticExtension")
            {
                return false;
            }

            if (element.Name.NamespaceName == xNamespace.NamespaceName ||
                element.Name.NamespaceName == DefaultXamlNamespace ||
                element.Name.NamespaceName == LegacyXamlNamespace)
            {
                return true;
            }

            (string ns, string assemblyName) = GettingInformationAboutXamlTypes.GetClrNamespaceAndAssembly(element.Name.NamespaceName);

            return ns == "System.Windows.Markup" && assemblyName == "OpenSilver";
        }

        public static bool IsTypeExtension(XElement element) => IsTypeExtension(element.Name);

        public static bool IsTypeExtension(XName name)
        {
            if (name.LocalName != "TypeExtension")
            {
                return false;
            }

            if (name.NamespaceName == xNamespace.NamespaceName ||
                name.NamespaceName == DefaultXamlNamespace ||
                name.NamespaceName == LegacyXamlNamespace)
            {
                return true;
            }

            (string ns, string assemblyName) = GettingInformationAboutXamlTypes.GetClrNamespaceAndAssembly(name.NamespaceName);

            return ns == "System.Windows.Markup" && assemblyName == "OpenSilver";
        }

        public static bool IsDynamicResourceExtension(XElement element)
        {
            if (element.Name.LocalName != "DynamicResourceExtension")
            {
                return false;
            }

            if (element.Name.NamespaceName == DefaultXamlNamespace ||
                element.Name.NamespaceName == LegacyXamlNamespace)
            {
                return true;
            }

            (string ns, string assemblyName) = GettingInformationAboutXamlTypes.GetClrNamespaceAndAssembly(element.Name.NamespaceName);

            return ns == "System.Windows" && assemblyName == "OpenSilver";
        }

        public static bool IsResponsiveExtension(XElement element)
        {
            if (element.Name.LocalName != "ResponsiveExtension")
            {
                return false;
            }

            if (element.Name.NamespaceName == DefaultXamlNamespace ||
                element.Name.NamespaceName == LegacyXamlNamespace)
            {
                return true;
            }

            (string ns, string assemblyName) = GettingInformationAboutXamlTypes.GetClrNamespaceAndAssembly(element.Name.NamespaceName);

            return ns == "System.Windows" && assemblyName == "OpenSilver";
        }

        public static bool IsUriAbsolute(string path)
        {
            if (path.StartsWith("~"))
            {
                return true;
            }

            int index = path.IndexOf(':');
            if (index >= 0)
            {
                string scheme = path.Substring(0, index);
                return Uri.CheckSchemeName(scheme);
            }

            return false;
        }

        public static bool IsComponentUri(string value) => value.Contains(";component/", StringComparison.OrdinalIgnoreCase);

        public static bool IsUriMapping(string namespaceName, string typeName, string assemblyName, string processedAssemblyName)
        {
            return IsOfType(
                namespaceName, typeName, assemblyName,
                KnownNamespaces.SystemWindowsNavigation, [DefaultXamlNamespace, SdkXamlNamespace], "UriMapping", Constants.OPENSILVER_CONTROLS_NAVIGATION_ASSEMBLY_NAME,
                processedAssemblyName);
        }

        public static bool IsFrame(string namespaceName, string typeName, string assemblyName, string processedAssemblyName)
        {
            return IsOfType(
                namespaceName, typeName, assemblyName,
                KnownNamespaces.SystemWindowsControls, [DefaultXamlNamespace, SdkXamlNamespace], "Frame", Constants.OPENSILVER_CONTROLS_NAVIGATION_ASSEMBLY_NAME,
                processedAssemblyName);
        }

        public static bool IsHyperlinkButton(string namespaceName, string typeName, string assemblyName, string processedAssemblyName)
        {
            return IsOfType(
                namespaceName, typeName, assemblyName,
                KnownNamespaces.SystemWindowsControls, [DefaultXamlNamespace, LegacyXamlNamespace], "HyperlinkButton", Constants.OPENSILVER_ASSEMBLY_NAME,
                processedAssemblyName);
        }

        public static bool IsHyperlink(string namespaceName, string typeName, string assemblyName, string processedAssemblyName)
        {
            return IsOfType(
                namespaceName, typeName, assemblyName,
                KnownNamespaces.SystemWindowsDocuments, [DefaultXamlNamespace, LegacyXamlNamespace], "Hyperlink", Constants.OPENSILVER_ASSEMBLY_NAME,
                processedAssemblyName);
        }

        public static bool IsApplicationStartupUriProperty(string propertyName, string namespaceName, string typeName, string assemblyName, string processedAssemblyName)
        {
            if (propertyName != "StartupUri")
            {
                return false;
            }

            return IsOfType(
                namespaceName, typeName, assemblyName,
                KnownNamespaces.SystemWindows, [DefaultXamlNamespace, LegacyXamlNamespace], "Application", Constants.OPENSILVER_ASSEMBLY_NAME,
                processedAssemblyName);
        }

        private static bool IsOfType(
            string namespaceName, string typeName, string assemblyName,
            string targetClrNamespace, string[] targetXmlNamespaces, string targetTypeName, string targetAssemblyName,
            string processedAssemblyName)
        {
            if (typeName == targetTypeName)
            {
                for (int i = 0; i < targetXmlNamespaces.Length; i++)
                {
                    if (namespaceName == targetXmlNamespaces[i])
                    {
                        return true;
                    }
                }

                if (namespaceName == targetClrNamespace)
                {
                    return assemblyName == targetAssemblyName || (assemblyName == null && processedAssemblyName == targetAssemblyName);
                }
            }

            return false;
        }
    }
}
