
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

namespace OpenSilver.Compiler;

internal static class ProcessingContentPresenterNodes
{
    private const string SystemWindowsClrNamespace = $"clr-namespace:{KnownNamespaces.SystemWindows};assembly={Constants.OPENSILVER_ASSEMBLY_NAME}";
    private const string SystemWindowsControlsClrNamespace = $"clr-namespace:{KnownNamespaces.SystemWindowsControls};assembly={Constants.OPENSILVER_ASSEMBLY_NAME}";

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
        XElement element,
        bool isInsideControlTemplate,
        ConversionSettings settings)
    {
        if (!XamlParser.IsMemberNode(element))
        {
            settings.XamlNameParser.GetClrNamespaceAndLocalName(element.Name,
                out string namespaceName, out string typeName, out string assemblyName);

            if (settings.Inspector.IsControlTemplate(namespaceName, typeName, assemblyName, element))
            {
                isInsideControlTemplate = IsContentControlTargetType(element, settings);
            }
            else if (isInsideControlTemplate && settings.Inspector.IsContentPresenter(namespaceName, typeName, assemblyName, element))
            {
                bool hasContentAttribute = HasAttribute(element, "Content", settings);
                bool hasContentTemplateAttribute = HasAttribute(element, "ContentTemplate", settings);
                bool hasContentTemplateSelectorAttribute = HasAttribute(element, "ContentTemplateSelector", settings);

                if (!hasContentAttribute || (!hasContentTemplateAttribute && !hasContentTemplateSelectorAttribute))
                {
                    string systemWindowsPrefix = string.Empty, systemWindowsControlsPrefix = string.Empty;

                    // First look for the default namespace, it should cover 99% of cases.
                    if (Array.IndexOf(GeneratingCode.DefaultXamlNamespaces, element.GetDefaultNamespace()) == -1)
                    {
                        systemWindowsPrefix = GenerateXmlnsPrefix(element);
                        element.SetAttributeValue(XNamespace.Xmlns.GetName(systemWindowsPrefix), SystemWindowsClrNamespace);

                        systemWindowsControlsPrefix = GenerateXmlnsPrefix(element);
                        element.SetAttributeValue(XNamespace.Xmlns.GetName(systemWindowsControlsPrefix), SystemWindowsControlsClrNamespace);
                    }

                    string xPrefix = element.GetPrefixOfNamespace(GeneratingCode.xNamespace);
                    if (xPrefix is null)
                    {
                        xPrefix = GenerateXmlnsPrefix(element);
                        element.SetAttributeValue(XNamespace.Xmlns.GetName(xPrefix), GeneratingCode.xNamespace.NamespaceName);
                    }

                    if (!hasContentAttribute)
                    {
                        SetTemplateBinding(element, "Content", systemWindowsPrefix, systemWindowsControlsPrefix, xPrefix);
                    }

                    if (!hasContentTemplateAttribute && !hasContentTemplateSelectorAttribute)
                    {
                        SetTemplateBinding(element, "ContentTemplate", systemWindowsPrefix, systemWindowsControlsPrefix, xPrefix);
                        SetTemplateBinding(element, "ContentTemplateSelector", systemWindowsPrefix, systemWindowsControlsPrefix, xPrefix);
                    }
                }
            }
        }

        // Recursion:
        foreach (var childElements in element.Elements())
        {
            TraverseNextElement(childElements, isInsideControlTemplate, settings);
        }
    }

    private static bool IsContentControlTargetType(XElement element, ConversionSettings settings)
    {
        if (element.Attribute("TargetType") is XAttribute targetType)
        {
            string namespaceName, typeName, assemblyName;

            int index = targetType.Value.IndexOf(':');
            if (index > -1)
            {
                string prefix = targetType.Value.Substring(0, index);
                if (element.GetNamespaceOfPrefix(prefix) is not XNamespace xmlns)
                {
                    throw new XamlParseException($"'{prefix}' is an undeclared prefix.", targetType);
                }

                (namespaceName, assemblyName) = settings.XamlNameParser.GetClrNamespaceAndAssembly(
                    xmlns.NamespaceName);

                typeName = targetType.Value.Substring(index + 1);
            }
            else
            {
                (namespaceName, assemblyName) = settings.XamlNameParser.GetClrNamespaceAndAssembly(
                    element.GetDefaultNamespace().NamespaceName);

                typeName = targetType.Value;
            }

            return settings.Inspector.IsContentControl(namespaceName, typeName, assemblyName, element);
        }

        return false;
    }

    private static bool HasAttribute(XElement cp, string attributeName, ConversionSettings settings)
    {
        bool found = cp.Attribute(attributeName) != null;
        if (!found)
        {
            foreach (var child in cp.Elements())
            {
                (string namespaceName, string assemblyName) = settings.XamlNameParser.GetClrNamespaceAndAssembly(
                    child.Name.NamespaceName);

                string[] typeAndProperty = child.Name.LocalName.Split('.');

                if (typeAndProperty.Length == 2)
                {
                    // First check if this is the right property.
                    if (typeAndProperty[1].Trim() == attributeName)
                    {
                        // Then make sure this is not an attached property.
                        bool isProperty = settings.Inspector.IsContentPresenter(namespaceName, typeAndProperty[0], assemblyName, child);

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
