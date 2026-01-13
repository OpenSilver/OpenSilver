
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
using System.Xml;
using System.Xml.Linq;

namespace OpenSilver.Compiler
{
    internal static partial class GeneratingCSCode
    {
        private class GeneratorPass1 : ICodeGenerator
        {
            private readonly XamlReader _reader;
            private readonly ConversionSettings _settings;
            private readonly string _sourceFile;
            private readonly string _fileNameWithPathRelativeToProjectRoot;
            
            public GeneratorPass1(XDocument doc,
                string sourceFile,
                string fileNameWithPathRelativeToProjectRoot,
                ConversionSettings settings)
            {
                _reader = new XamlReader(doc);
                _settings = settings;
                _sourceFile = sourceFile;
                _fileNameWithPathRelativeToProjectRoot = fileNameWithPathRelativeToProjectRoot;
            }

            public string Generate() => GenerateImpl();

            private string GenerateImpl()
            {
                GetClassInformationFromXaml(_reader.Document, _settings.Inspector,
                    out string className, out string namespaceStringIfAny, out bool hasCodeBehind);
                string baseType = GetCSharpEquivalentOfXamlTypeAsString(_reader.Document.Root, true);

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
                            string fieldModifier = "internal";
                            XAttribute fieldModifierAttr = element.Attribute(GeneratingCode.xNamespace + "FieldModifier");
                            if (fieldModifierAttr != null)
                            {
                                fieldModifier = fieldModifierAttr.Value?.ToLower() ?? "private";
                            }

                            // add '@' to handle cases where x:Name is a forbidden word (for instance 'this'
                            // or any other c# keyword)
                            string fieldName = "@" + name;

                            // Add #line directive to map C# errors back to XAML source
                            if (element is IXmlLineInfo lineInfo && lineInfo.HasLineInfo())
                            {
                                // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-10.0/enhanced-line-directives#detailed-design
                                string lineDirective = $"#line ({lineInfo.LineNumber}, {lineInfo.LinePosition}) - ({lineInfo.LineNumber}, {lineInfo.LinePosition + 5}) 65536 \"{_sourceFile}\"";
                                resultingFieldsForNamedElements.Add(
                                    $"{lineDirective}\n{fieldModifier} {GetCSharpEquivalentOfXamlTypeAsString(element, true)} {fieldName};\n#line default");
                            }
                            else
                            {
                                resultingFieldsForNamedElements.Add(
                                    $"{fieldModifier} {GetCSharpEquivalentOfXamlTypeAsString(element, true)} {fieldName};");
                            }
                        }
                    }
                }

                if (hasCodeBehind)
                {
                    // Create the "IntializeComponent()" method:
                    string initializeComponentMethod = CreateInitializeComponentMethod(
                        $"global::{KnownNamespaces.SystemWindows}.Application",
                        _settings.AssemblyName,
                        _fileNameWithPathRelativeToProjectRoot,
                        new List<string>());

                    // Wrap everything into a partial class:
                    string partialClass = GeneratePartialClass("",
                                                               initializeComponentMethod,
                                                               new ComponentConnectorBuilderCS().ToString(),
                                                               resultingFieldsForNamedElements,
                                                               className,
                                                               namespaceStringIfAny,
                                                               baseType);

                    string componentTypeFullName = GetFullTypeName(namespaceStringIfAny, className);

                    string factoryClass = GenerateFactoryClass(
                        componentTypeFullName,
                        baseType,
                        GeneratingCode.GetUniqueName(_reader.Document.Root),
                        "throw new global::System.NotImplementedException();",
                        "throw new global::System.NotImplementedException();",
                        Enumerable.Empty<string>(),
                        $"global::{KnownNamespaces.SystemWindows}.UIElement",
                        _settings.AssemblyName,
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
                        baseType,
                        GeneratingCode.GetUniqueName(_reader.Document.Root),
                        "throw new global::System.NotImplementedException();",
                        "throw new global::System.NotImplementedException();",
                        Enumerable.Empty<string>(),
                        $"global::{KnownNamespaces.SystemWindows}.UIElement",
                        _settings.AssemblyName,
                        _fileNameWithPathRelativeToProjectRoot);

                    return finalCode;
                }
            }

            private XElement GetRootOfCurrentNamescopeForCompilation(XElement element)
            {
                while (element.Parent != null)
                {
                    XElement parent = element.Parent;
                    if (GeneratingCode.IsDataTemplate(parent, _settings.AssemblyName) ||
                        GeneratingCode.IsItemsPanelTemplate(parent, _settings.AssemblyName) ||
                        GeneratingCode.IsControlTemplate(parent, _settings.AssemblyName))
                    {
                        return parent;
                    }
                    element = parent;
                }
                return element;
            }

            private string GetCSharpEquivalentOfXamlTypeAsString(
                XElement element,
                bool ifTypeNotFoundTryGuessing,
                out string namespaceName,
                out string typeName,
                out string assemblyName)
            {
                GettingInformationAboutXamlTypes.GetClrNamespaceAndLocalName(
                    element.Name,
                    out namespaceName,
                    out typeName,
                    out assemblyName);

                return _settings.Inspector.GetCSharpEquivalentOfXamlTypeAsString(
                    namespaceName,
                    typeName,
                    assemblyName,
                    element,
                    ifTypeNotFoundTryGuessing);
            }

            private string GetCSharpEquivalentOfXamlTypeAsString(XElement element, bool ifTypeNotFoundTryGuessing = false)
                => GetCSharpEquivalentOfXamlTypeAsString(element, ifTypeNotFoundTryGuessing, out _, out _, out _);
        }
    }
}
