
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
        private const string LegacyXamlNamespace = "http://schemas.microsoft.com/client/2007"; // XAML namespace used for Silverlight 1.0 application

        internal static readonly XNamespace[] DefaultXamlNamespaces = new XNamespace[2] { DefaultXamlNamespace, LegacyXamlNamespace };
        internal static readonly XNamespace xNamespace = @"http://schemas.microsoft.com/winfx/2006/xaml"; // Used for example for "x:Name" attributes and {x:Null} markup extensions.

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

        internal static bool IsXNameAttribute(XAttribute attr)
            => attr.Name.LocalName == "Name" && attr.Name.NamespaceName == xNamespace;

        internal static bool IsNameAttribute(XAttribute attr)
            => attr.Name.LocalName == "Name" && string.IsNullOrEmpty(attr.Name.NamespaceName);

        internal static string GetUniqueName(XElement element)
        {
            return element.Attribute(GeneratingUniqueNames.UniqueNameAttribute).Value;
        }

        public static bool IsDataTemplate(XElement element) => IsXElementOfType(element, "DataTemplate");

        public static bool IsItemsPanelTemplate(XElement element) => IsXElementOfType(element, "ItemsPanelTemplate");

        public static bool IsControlTemplate(XElement element) => IsXElementOfType(element, "ControlTemplate");

        public static bool IsBinding(XElement element) => IsXElementOfType(element, "Binding");

        public static bool IsStyle(XElement element) => IsXElementOfType(element, "Style");

        public static bool IsTextBlock(XElement element) => IsXElementOfType(element, "TextBlock");

        public static bool IsRun(XElement element) => IsXElementOfType(element, "Run");

        public static bool IsSpan(XElement element) => IsXElementOfType(element, "Span");

        public static bool IsItalic(XElement element) => IsXElementOfType(element, "Italic");

        public static bool IsUnderline(XElement element) => IsXElementOfType(element, "Underline");

        public static bool IsBold(XElement element) => IsXElementOfType(element, "Bold");

        public static bool IsHyperlink(XElement element) => IsXElementOfType(element, "Hyperlink");

        public static bool IsParagraph(XElement element) => IsXElementOfType(element, "Paragraph");

        public static bool IsColorAnimation(XElement element) => IsXElementOfType(element, "ColorAnimation");

        private static bool IsXElementOfType(XElement element, string typeName)
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
            }

            return false;
        }
    }
}
