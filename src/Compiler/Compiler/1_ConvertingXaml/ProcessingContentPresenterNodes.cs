
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
    internal static class ProcessingContentPresenterNodes
    {
        [ThreadStatic]
        private static Random _random;

        private static Random Random => _random ??= new Random();

        //------------------------------------------------------------
        // This class will process the "ContentPresenter" nodes
        // in order to transform "<ContentPresenter />" into
        // "<ContentPresenter Content="{TemplateBinding Content}"
        // ContentTemplate="{TemplateBinding ContentTemplate}" />"
        //------------------------------------------------------------

        public static void Process(XDocument doc, ConversionSettings settings)
        {
            TraverseNextElement(doc.Root, false, settings);
        }

        private static void TraverseNextElement(
            XElement currentElement,
            bool isInsideControlTemplate,
            ConversionSettings settings)
        {
            if (GeneratingCode.IsControlTemplate(currentElement, settings.AssemblyName))
            {
                isInsideControlTemplate = true;
            }

            if (isInsideControlTemplate && !currentElement.Name.LocalName.Contains(".") &&
                settings.Inspector.IsAssignableFrom(KnownNamespaces.SystemWindowsControls, "ContentPresenter",
                    currentElement.Name.NamespaceName, currentElement.Name.LocalName))
            {
                bool hasContentAttribute = HasAttribute(currentElement, "Content", settings.Inspector);
                bool hasContentTemplateAttribute = HasAttribute(currentElement, "ContentTemplate", settings.Inspector);
                bool hasContentTemplateSelectorAttribute = HasAttribute(currentElement, "ContentTemplateSelector", settings.Inspector);

                if (!hasContentAttribute || (!hasContentTemplateAttribute && !hasContentTemplateSelectorAttribute))
                {
                    string systemWindowsPrefix = string.Empty, systemWindowsControlsPrefix = string.Empty;

                    // First look for the default namespace, it should cover 99% of cases.
                    if (Array.IndexOf(GeneratingCode.DefaultXamlNamespaces, currentElement.GetDefaultNamespace()) == -1)
                    {
                        systemWindowsPrefix = GenerateXmlnsPrefix(currentElement);
                        currentElement.SetAttributeValue(XNamespace.Xmlns.GetName(systemWindowsPrefix), "clr-namespace:System.Windows;assembly=OpenSilver");

                        systemWindowsControlsPrefix = GenerateXmlnsPrefix(currentElement);
                        currentElement.SetAttributeValue(XNamespace.Xmlns.GetName(systemWindowsControlsPrefix), "clr-namespace:System.Windows.Controls;assembly=OpenSilver");
                    }

                    string xPrefix = currentElement.GetPrefixOfNamespace(GeneratingCode.xNamespace);
                    if (xPrefix is null)
                    {
                        xPrefix = GenerateXmlnsPrefix(currentElement);
                        currentElement.SetAttributeValue(XNamespace.Xmlns.GetName(xPrefix), GeneratingCode.xNamespace.NamespaceName);
                    }

                    if (!hasContentAttribute)
                    {
                        SetTemplateBinding(currentElement, "Content", systemWindowsPrefix, systemWindowsControlsPrefix, xPrefix);
                    }

                    if (!hasContentTemplateAttribute && !hasContentTemplateSelectorAttribute)
                    {
                        SetTemplateBinding(currentElement, "ContentTemplate", systemWindowsPrefix, systemWindowsControlsPrefix, xPrefix);
                        SetTemplateBinding(currentElement, "ContentTemplateSelector", systemWindowsPrefix, systemWindowsControlsPrefix, xPrefix);
                    }
                }
            }

            // Recursion:
            foreach (var childElements in currentElement.Elements())
            {
                TraverseNextElement(childElements, isInsideControlTemplate, settings);
            }
        }

        private static bool HasAttribute(XElement cp, string attributeName, AssembliesInspector reflectionOnSeparateAppDomain)
        {
            bool found = cp.Attribute(attributeName) != null;
            if (!found)
            {
                foreach (var child in cp.Elements())
                {
                    string namespaceName = child.Name.NamespaceName;
                    string[] typeAndProperty = child.Name.LocalName.Split('.');

                    if (typeAndProperty.Length == 2)
                    {
                        // First check if this is the right property.
                        if (typeAndProperty[1].Trim() == attributeName)
                        {
                            // Then make sure this is not an attached property.
                            bool isProperty = reflectionOnSeparateAppDomain.IsAssignableFrom(
                                KnownNamespaces.SystemWindowsControls,
                                "ContentPresenter",
                                namespaceName,
                                typeAndProperty[0]);

                            if (isProperty)
                            {
                                found = true;
                                break;
                            }
                        }
                    }
                }
            }

            return found;
        }

        private static void SetTemplateBinding(XElement element, string propertyName, string systemWindowsPrefix, string systemWindowsControlsPrefix, string xPrefix)
        {
            element.SetAttributeValue(propertyName, (systemWindowsPrefix, systemWindowsControlsPrefix, xPrefix) switch
            {
                (null or "", null or "", null or "") => $"{{TemplateBinding Property={{Static ContentControl.{propertyName}Property}}}}",
                (null or "", null or "", _) => $"{{TemplateBinding Property={{{xPrefix}:Static ContentControl.{propertyName}Property}}}}",
                (null or "", _, null or "") => $"{{TemplateBinding Property={{Static {systemWindowsControlsPrefix}:ContentControl.{propertyName}Property}}}}",
                (_, null or "", null or "") => $"{{{systemWindowsPrefix}:TemplateBinding Property={{Static ContentControl.{propertyName}Property}}}}",
                (null or "", _, _) => $"{{TemplateBinding Property={{{xPrefix}:Static {systemWindowsControlsPrefix}:ContentControl.{propertyName}Property}}}}",
                (_, null or "", _) => $"{{{systemWindowsPrefix}:TemplateBinding Property={{{xPrefix}:Static ContentControl.{propertyName}Property}}}}",
                (_, _, null or "") => $"{{{systemWindowsPrefix}:TemplateBinding Property={{Static {systemWindowsControlsPrefix}:ContentControl.{propertyName}Property}}}}",
                (_, _, _) => $"{{{systemWindowsPrefix}:TemplateBinding Property={{{xPrefix}:Static {systemWindowsControlsPrefix}:ContentControl.{propertyName}Property}}}}",
            });
        }

        private static string GenerateXmlnsPrefix(XElement element)
        {
            string prefix = GenerateXmlnsPrefix();
            while (element.GetNamespaceOfPrefix(prefix) is not null)
            {
                prefix = GenerateXmlnsPrefix();
            }
            return prefix;
        }

        private static string GenerateXmlnsPrefix()
        {
            const string Choices = "abcdefghijklmnopqrstuvwxyz";

            Span<char> items = stackalloc char[8];

            for (int i = 0; i < items.Length; i++)
            {
                items[i] = Choices[Random.Next(Choices.Length)];
            }

            return items.ToString();
        }
    }
}
