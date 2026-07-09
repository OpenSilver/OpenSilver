
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
using System.Linq;
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
        TraverseNextElement(doc.Root, false, null, settings);
    }

    private static void TraverseNextElement(
        XElement element,
        bool isInsideControlTemplate,
        TargetTypeData targetType,
        ConversionSettings settings)
    {
        if (!XamlParser.IsMemberNode(element))
        {
            settings.XamlNameParser.GetClrNamespaceAndLocalName(element.Name,
                out string namespaceName, out string typeName, out string assemblyName);

            TypeDefinition type = settings.Inspector.GetTypeDefinition(namespaceName, typeName, assemblyName, element);

            if (settings.Inspector.IsControlTemplate(type))
            {
                targetType = GetTargetTypeData(element, settings);
                isInsideControlTemplate = targetType is not null;
            }
            else if (isInsideControlTemplate)
            {
                bool isContentControl = settings.Inspector.IsContentControl(targetType.Type);

                if ((settings.HasFeature(XamlPreprocessorFeatures.AlwaysAutoAliasContentPresenter) || isContentControl) && settings.Inspector.IsContentPresenter(type))
                {
                    (string contentSource, bool isContentSourceSet) = GetContentSource(element, settings);
                    bool isContentPropertyDefined = HasAttribute(element, "Content", settings);
                    bool isContentTemplatePropertyDefined = HasAttribute(element, "ContentTemplate", settings);
                    bool isContentTemplateSelectorPropertyDefined = HasAttribute(element, "ContentTemplateSelector", settings);
                    bool isContentStringFormatPropertyDefined = HasAttribute(element, "ContentStringFormat", settings);

                    if (!isContentSourceSet && isContentControl)
                    {
                        SetDefaultTemplateBindings(element,
                            isContentPropertyDefined,
                            isContentTemplatePropertyDefined,
                            isContentTemplateSelectorPropertyDefined,
                            isContentStringFormatPropertyDefined);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(contentSource) && !isContentSourceSet)
                        {
                            contentSource = "Content";
                        }

                        SetTemplateBindings(element,
                            targetType,
                            contentSource,
                            isContentPropertyDefined,
                            isContentTemplatePropertyDefined,
                            isContentTemplateSelectorPropertyDefined,
                            isContentStringFormatPropertyDefined);
                    }
                }
            }
        }

        foreach (var childElements in element.Elements())
        {
            TraverseNextElement(childElements, isInsideControlTemplate, targetType, settings);
        }
    }

    private static void SetDefaultTemplateBindings(XElement element,
        bool isContentPropertyDefined,
        bool isContentTemplatePropertyDefined,
        bool isContentTemplateSelectorPropertyDefined,
        bool isContentStringFormatPropertyDefined)
    {
        if (!isContentPropertyDefined || (!isContentTemplatePropertyDefined && !isContentTemplateSelectorPropertyDefined && !isContentStringFormatPropertyDefined))
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

            if (!isContentPropertyDefined)
            {
                SetTemplateBinding(element, "Content", systemWindowsPrefix, systemWindowsControlsPrefix, xPrefix);
            }

            if (!isContentTemplatePropertyDefined && !isContentTemplateSelectorPropertyDefined && !isContentStringFormatPropertyDefined)
            {
                SetTemplateBinding(element, "ContentTemplate", systemWindowsPrefix, systemWindowsControlsPrefix, xPrefix);
                SetTemplateBinding(element, "ContentTemplateSelector", systemWindowsPrefix, systemWindowsControlsPrefix, xPrefix);
                SetTemplateBinding(element, "ContentStringFormat", systemWindowsPrefix, systemWindowsControlsPrefix, xPrefix);
            }
        }

        static void SetTemplateBinding(XElement element,
            string propertyName,
            string systemWindowsPrefix,
            string systemWindowsControlsPrefix,
            string xPrefix)
        {
            var attribute = new ExtendedXAttribute(
                systemWindowsControlsPrefix switch
                {
                    null or "" => XNamespace.None.GetName($"ContentPresenter.{propertyName}"),
                    _ => element.GetNamespaceOfPrefix(systemWindowsControlsPrefix).GetName($"ContentPresenter.{propertyName}"),
                },
                (systemWindowsPrefix, systemWindowsControlsPrefix, xPrefix) switch
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

            attribute.SetLineInfo(element);

            element.Add(attribute);
        }
    }

    private static void SetTemplateBindings(XElement element,
        TargetTypeData targetType,
        string contentSource,
        bool isContentPropertyDefined,
        bool isContentTemplatePropertyDefined,
        bool isContentTemplateSelectorPropertyDefined,
        bool isContentStringFormatPropertyDefined)
    {
        if (string.IsNullOrEmpty(contentSource))
        {
            return;
        }

        if (!isContentPropertyDefined || (!isContentTemplatePropertyDefined && !isContentTemplateSelectorPropertyDefined && !isContentStringFormatPropertyDefined))
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

            string targetTypePrefix = element.GetPrefixOfNamespace(targetType.XmlNamespace);

            if (targetTypePrefix is null && element.GetDefaultNamespace() != targetType.XmlNamespace)
            {
                targetTypePrefix = GenerateXmlnsPrefix(element);
                element.SetAttributeValue(XNamespace.Xmlns.GetName(targetTypePrefix), targetType.XmlNamespace.NamespaceName);
            }

            if (!isContentPropertyDefined)
            {
                SetTemplateBinding(element,
                    "Content",
                    contentSource,
                    systemWindowsPrefix,
                    systemWindowsControlsPrefix,
                    targetTypePrefix,
                    targetType.Type.Name);
            }

            if (!isContentTemplatePropertyDefined && !isContentTemplateSelectorPropertyDefined && !isContentStringFormatPropertyDefined)
            {
                SetTemplateBinding(element,
                    "ContentTemplate",
                    $"{contentSource}Template",
                    systemWindowsPrefix,
                    systemWindowsControlsPrefix,
                    targetTypePrefix,
                    targetType.Type.Name);

                SetTemplateBinding(element,
                    "ContentTemplateSelector",
                    $"{contentSource}TemplateSelector",
                    systemWindowsPrefix,
                    systemWindowsControlsPrefix,
                    targetTypePrefix,
                    targetType.Type.Name);

                SetTemplateBinding(element,
                    "ContentStringFormat",
                    $"{contentSource}StringFormat",
                    systemWindowsPrefix,
                    systemWindowsControlsPrefix,
                    targetTypePrefix,
                    targetType.Type.Name);
            }
        }

        static void SetTemplateBinding(XElement element,
            string propertyName,
            string contentSourceProperty,
            string systemWindowsPrefix,
            string systemWindowsControlsPrefix,
            string targetTypePrefix,
            string targetTypeName)
        {
            var attribute = new ExtendedXAttribute(
                systemWindowsControlsPrefix switch
                {
                    null or "" => XNamespace.None.GetName($"ContentPresenter.{propertyName}"),
                    _ => element.GetNamespaceOfPrefix(systemWindowsControlsPrefix).GetName($"ContentPresenter.{propertyName}"),
                },
                (systemWindowsPrefix, targetTypePrefix) switch
                {
                    (null or "", null or "") => $"{{TemplateBinding DependencyPropertyName={contentSourceProperty}, DependencyPropertyOwnerType={targetTypeName}}}",
                    (null or "", _) => $"{{TemplateBinding DependencyPropertyName={contentSourceProperty}, DependencyPropertyOwnerType={targetTypePrefix}:{targetTypeName}}}",
                    (_, null or "") => $"{{{systemWindowsPrefix}:TemplateBinding DependencyPropertyName={contentSourceProperty}, DependencyPropertyOwnerType={targetTypeName}}}",
                    (_, _) => $"{{{systemWindowsPrefix}:TemplateBinding DependencyPropertyName={contentSourceProperty}, DependencyPropertyOwnerType={targetTypePrefix}:{targetTypeName}}}",
                });

            attribute.SetLineInfo(element);

            element.Add(attribute);
        }
    }

    private static TargetTypeData GetTargetTypeData(XElement element, ConversionSettings settings)
    {
        if (element.Attribute("TargetType") is XAttribute targetType)
        {
            XNamespace xmlns;
            string typeName;

            int index = targetType.Value.IndexOf(':');
            if (index > -1)
            {
                string prefix = targetType.Value.Substring(0, index);
                xmlns = element.GetNamespaceOfPrefix(prefix) ?? throw new XamlParseException($"'{prefix}' is an undeclared prefix.", targetType);
                typeName = targetType.Value.Substring(index + 1);
            }
            else
            {
                xmlns = element.GetDefaultNamespace();
                typeName = targetType.Value;
            }

            (string namespaceName, string assemblyName) = settings.XamlNameParser.GetClrNamespaceAndAssembly(
                xmlns.NamespaceName);

            TypeDefinition type = settings.Inspector.GetTypeDefinition(namespaceName, typeName, assemblyName, element);
            return new TargetTypeData(type, xmlns);
        }

        return null;
    }

    private static (string Value, bool IsSet) GetContentSource(XElement cp, ConversionSettings settings)
    {
        if (cp.Attribute("ContentSource") is XAttribute contentSourceAttribute)
        {
            if (MarkupExtensionDescriptor.LooksLikeAMarkupExtension(contentSourceAttribute.Value))
            {
                return (null, true);
            }

            return (GeneratingCode.GetAttributeValue(contentSourceAttribute), true);
        }

        foreach (var child in cp.Elements())
        {
            int index = child.Name.LocalName.IndexOf('.');

            if (index == -1)
            {
                continue;
            }

            // First check if this is the right property.
            if (child.Name.LocalName.AsSpan(index + 1).Trim().Equals("ContentSource", StringComparison.Ordinal))
            {
                (string namespaceName, string assemblyName) =
                    settings.XamlNameParser.GetClrNamespaceAndAssembly(child.Name.NamespaceName);

                TypeDefinition type = settings.Inspector.GetTypeDefinition(
                    namespaceName, child.Name.LocalName.Substring(0, index), assemblyName, child);

                if (settings.Inspector.IsContentPresenter(type))
                {
                    if (child.Elements().Count() == 1 &&
                        TryReadStringElement(child.Elements().First(), settings, out string value))
                    {
                        return (value, true);
                    }

                    return (null, true);
                }
            }
        }

        return (null, false);
    }

    private static bool TryReadStringElement(XElement element, ConversionSettings settings, out string value)
    {
        settings.XamlNameParser.GetClrNamespaceAndLocalName(
            element.Name,
            out string namespaceName,
            out string typeName,
            out string assemblyName);

        TypeDefinition type = settings.Inspector.GetTypeDefinition(
            namespaceName,
            typeName,
            assemblyName,
            element);

        if (type.IsString())
        {
            // This code runs after the InsertingImplicitNodes step, so a string element can only be initalized
            // from string (direct content is converted to the InitializedFromStringAttribute attribute), or be
            // empty.

            if (element.Attribute(InsertingImplicitNodes.InitializedFromStringAttribute) is XAttribute initAttribute)
            {
                value = GeneratingCode.GetAttributeValue(initAttribute);
            }
            else
            {
                value = string.Empty;
            }

            return true;
        }

        value = null;
        return false;
    }

    private static bool HasAttribute(XElement cp, string attributeName, ConversionSettings settings)
    {
        if (cp.Attribute(attributeName) is not null)
        {
            return true;
        }

        foreach (var child in cp.Elements())
        {
            int index = child.Name.LocalName.IndexOf('.');

            if (index == -1)
            {
                continue;
            }

            // First check if this is the right property.
            if (child.Name.LocalName.AsSpan(index + 1).Trim().Equals(attributeName, StringComparison.Ordinal))
            {
                (string namespaceName, string assemblyName) =
                    settings.XamlNameParser.GetClrNamespaceAndAssembly(child.Name.NamespaceName);

                TypeDefinition type = settings.Inspector.GetTypeDefinition(
                    namespaceName, child.Name.LocalName.Substring(0, index), assemblyName, child);

                if (settings.Inspector.IsContentPresenter(type))
                {
                    return true;
                }
            }
        }

        return false;
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

    private sealed class TargetTypeData
    {
        public TargetTypeData(TypeDefinition type, XNamespace xmlNamespace)
        {
            Type = type;
            XmlNamespace = xmlNamespace;
        }

        public readonly TypeDefinition Type;
        public readonly XNamespace XmlNamespace;
    }
}
