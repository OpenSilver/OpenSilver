
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

using System.Xml.Linq;
using System.Xml;

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

        internal static readonly XNamespace[] DefaultXamlNamespaces = [DefaultXamlNamespace, LegacyXamlNamespace];
        internal static readonly XNamespace xNamespace = "http://schemas.microsoft.com/winfx/2006/xaml"; // Used for example for "x:Name" attributes and {x:Null} markup extensions.

        internal static int GetLineNumber(XNode element)
        {
            // Get the line number in the original XAML file by walking up the tree until we find a node that contains line number information:

            while (element != null)
            {
                // See if the current element has line information:
                if (((IXmlLineInfo)element).HasLineInfo())
                {
                    return ((IXmlLineInfo)element).LineNumber;
                }

                // If not, go to the previous sibling node if any:
                var previousNode = element.PreviousNode;
                if (previousNode != null)
                {
                    element = previousNode;
                }
                else
                {
                    // Alternatively, walk up the tree to go to the parent node:
                    element = element.Parent;
                }
            }
            return -1;
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

        public static bool IsTypeExtension(XElement element)
        {
            if (element.Name.LocalName != "TypeExtension")
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
    }
}
