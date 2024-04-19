
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
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace OpenSilver.Compiler
{
    internal static partial class GeneratingVBCode
    {
        private class GeneratorPass1 : ICodeGenerator
        {
            private readonly XamlReader _reader;
            private readonly ConversionSettings _settings;
            private readonly string _fileNameWithPathRelativeToProjectRoot;
            private readonly string _assemblyNameWithoutExtension;
            private readonly string _rootNamespace;
            private readonly AssembliesInspector _reflectionOnSeparateAppDomain;
            
            public GeneratorPass1(XDocument doc,
                string assemblyNameWithoutExtension,
                string fileNameWithPathRelativeToProjectRoot,
                string rootNamespace,
                AssembliesInspector reflectionOnSeparateAppDomain,
                ConversionSettings settings)
            {
                _reader = new XamlReader(doc);
                _settings = settings;
                _assemblyNameWithoutExtension = assemblyNameWithoutExtension;
                _fileNameWithPathRelativeToProjectRoot = fileNameWithPathRelativeToProjectRoot;
                _rootNamespace = rootNamespace;
                _reflectionOnSeparateAppDomain = reflectionOnSeparateAppDomain;
            }

            public string Generate() => GenerateImpl();

            private string GenerateImpl()
            {
                GetClassInformationFromXaml(_reader.Document, _reflectionOnSeparateAppDomain,
                    out string className, out string namespaceStringIfAny, out bool hasCodeBehind);

                (string namespaceDeclaration, string namespaceName) = GetNamespace(namespaceStringIfAny, _rootNamespace);

                string baseType = GetCSharpEquivalentOfXamlTypeAsString(_reader.Document.Root.Name, true);

                List<string> resultingFieldsForNamedElements = new List<string>();
                List<string> resultingMethods = new List<string>();

                while (_reader.Read())
                {
                    if (_reader.NodeType != XamlNodeType.StartObject)
                        continue;

                    if (!hasCodeBehind)
                    {
                        // No code behind, no need to create fields for elementd with an x:Name
                        continue;
                    }

                    XElement element = _reader.ObjectData.Element;
                    XAttribute xNameAttr = element.Attributes()
                        .FirstOrDefault(attr => GeneratingCode.IsXNameAttribute(attr) || GeneratingCode.IsNameAttribute(attr));

                    if (xNameAttr != null && GetRootOfCurrentNamescopeForCompilation(element).Parent == null)
                    {
                        string name = xNameAttr.Value;
                        if (!string.IsNullOrWhiteSpace(name))
                        {
                            string fieldModifier = _settings.Metadata.FieldModifier;
                            XAttribute fieldModifierAttr = element.Attribute(GeneratingCode.xNamespace + "FieldModifier");
                            if (fieldModifierAttr != null)
                            {
                                fieldModifier = fieldModifierAttr.Value?.ToLower() ?? "private";
                            }

                            // add '[]' to handle cases where x:Name is a forbidden word (for instance 'Me'
                            // or any other VB keyword)
                            string fieldName = $"[{name}]";
                            resultingFieldsForNamedElements.Add(
                                string.Format(
                                    "    {0} WithEvents {1} As {2}",
                                    fieldModifier,
                                    fieldName,
                                    GetCSharpEquivalentOfXamlTypeAsString(element.Name, true)));
                        }
                    }
                }

                if (hasCodeBehind)
                {
                    // Create the "IntializeComponent()" method:
                    string initializeComponentMethod = CreateInitializeComponentMethod(
                        $"Global.{_settings.Metadata.SystemWindowsNS}.Application",
                        string.Empty,
                        _assemblyNameWithoutExtension,
                        _fileNameWithPathRelativeToProjectRoot,
                        new List<string>());

                    // Wrap everything into a partial class:
                    string partialClass = GeneratePartialClass("",
                                                               initializeComponentMethod,
                                                               new ComponentConnectorBuilderVB().ToString(),
                                                               resultingFieldsForNamedElements,
                                                               className,
                                                               namespaceDeclaration,
                                                               baseType);

                    string componentTypeFullName = GetFullTypeName(namespaceName, className);

                    string factoryClass = GenerateFactoryClass(
                        componentTypeFullName,
                        GeneratingCode.GetUniqueName(_reader.Document.Root),
                        "Throw New Global.System.NotImplementedException()",
                        "Throw New Global.System.NotImplementedException()",
                        Enumerable.Empty<string>(),
                        $"Global.{_settings.Metadata.SystemWindowsNS}.UIElement",
                        _assemblyNameWithoutExtension,
                        _fileNameWithPathRelativeToProjectRoot);

                    string finalCode = $@"
{factoryClass}
{partialClass}";

                    return finalCode;
                }
                else
                {
                    string finalCode = GenerateFactoryClass(
                        baseType,
                        GeneratingCode.GetUniqueName(_reader.Document.Root),
                        "Throw New Global.System.NotImplementedException()",
                        "Throw New Global.System.NotImplementedException()",
                        Enumerable.Empty<string>(),
                        $"Global.{_settings.Metadata.SystemWindowsNS}.UIElement",
                        _assemblyNameWithoutExtension,
                        _fileNameWithPathRelativeToProjectRoot);

                    return finalCode;
                }
            }

            private XElement GetRootOfCurrentNamescopeForCompilation(XElement element)
            {
                while (element.Parent != null)
                {
                    XElement parent = element.Parent;
                    if (GeneratingCode.IsDataTemplate(parent, _settings) ||
                        GeneratingCode.IsItemsPanelTemplate(parent, _settings) ||
                        GeneratingCode.IsControlTemplate(parent, _settings))
                    {
                        return parent;
                    }
                    element = parent;
                }
                return element;
            }

            private string GetCSharpEquivalentOfXamlTypeAsString(
                XName xName,
                bool ifTypeNotFoundTryGuessing,
                out string namespaceName,
                out string typeName,
                out string assemblyName)
            {
                GettingInformationAboutXamlTypes.GetClrNamespaceAndLocalName(
                    xName,
                    _settings.EnableImplicitAssemblyRedirection,
                    out namespaceName,
                    out typeName,
                    out assemblyName);

                return _reflectionOnSeparateAppDomain.GetCSharpEquivalentOfXamlTypeAsString(
                    namespaceName,
                    typeName,
                    assemblyName,
                    ifTypeNotFoundTryGuessing);
            }

            private string GetCSharpEquivalentOfXamlTypeAsString(XName xName, bool ifTypeNotFoundTryGuessing = false)
                => GetCSharpEquivalentOfXamlTypeAsString(xName, ifTypeNotFoundTryGuessing, out _, out _, out _);
        }
    }
}
