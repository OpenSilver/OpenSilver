
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

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace OpenSilver.Compiler
{
    internal static partial class GeneratingFSCode
    {
        private class GeneratorPass1 : ICodeGenerator
        {
            private readonly XamlReader _reader;
            private readonly ConversionSettings _settings;
            private readonly string _fileNameWithPathRelativeToProjectRoot;
            
            public GeneratorPass1(XDocument doc,
                string fileNameWithPathRelativeToProjectRoot,
                ConversionSettings settings)
            {
                _reader = new XamlReader(doc);
                _settings = settings;
                _fileNameWithPathRelativeToProjectRoot = fileNameWithPathRelativeToProjectRoot;
            }

            public string Generate() => GenerateImpl();

            private string GenerateImpl()
            {
                GetClassInformationFromXaml(_reader.Document, _settings.Inspector,
                    out string className, out string namespaceStringIfAny, out bool hasCodeBehind);

                string baseType = GeneratingCode.GetCSharpEquivalentOfXamlTypeAsString(_reader.Document.Root, _settings);

                List<string> resultingFieldsForNamedElements = new List<string>();
                List<string> resultingMembersForNamedElements = new List<string>();
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
                            string fieldModifier = "let mutable";
                            XAttribute fieldModifierAttr = element.Attribute(GeneratingCode.xNamespace + "FieldModifier");
                            if (fieldModifierAttr != null)
                            {
                                fieldModifier = fieldModifierAttr.Value?.ToLower() ?? "let mutable";
                            }

                            // add '[]' to handle cases where x:Name is a forbidden word (for instance 'Me'
                            // or any other FS keyword)
                            string fieldName = name;
                            string fieldNameLocal = name + "_local";
                            resultingFieldsForNamedElements.Add($@"
    {fieldModifier} {fieldNameLocal} = Unchecked.defaultof<{GeneratingCode.GetCSharpEquivalentOfXamlTypeAsString(element, _settings)}>");
                            resultingMembersForNamedElements.Add($@"
    member this.{fieldName}
        with get() = {fieldNameLocal}
        and set(value) = {fieldNameLocal} <- value
");
                        }
                    }
                }

                if (hasCodeBehind)
                {
                    // Create the "IntializeComponent()" method:
                    string initializeComponentMethod = CreateInitializeComponentMethod(
                        $"global.{KnownNamespaces.SystemWindows}.Application",
                        _settings.AssemblyName,
                        _fileNameWithPathRelativeToProjectRoot);

                    // combine local variables and members
                    resultingFieldsForNamedElements.AddRange(resultingMembersForNamedElements);

                    string classNameXaml = className + "Xaml"; // As F# doesn't support partial class, at the codebehind it will inherit []Xaml class

                    // Wrap everything into a partial class:
                    string partialClass = GeneratePartialClass(initializeComponentMethod,
                                                               new ComponentConnectorBuilderFS().ToString(),
                                                               resultingFieldsForNamedElements,
                                                               classNameXaml,
                                                               namespaceStringIfAny,
                                                               baseType);

                    string componentTypeFullName = GetFullTypeName(namespaceStringIfAny, classNameXaml);

                    string factoryClass = GenerateFactoryClass(
                        componentTypeFullName,
                        baseType,
                        GeneratingCode.GetUniqueName(_reader.Document.Root),
                        "        raise (global.System.NotImplementedException())",
                        "        raise (global.System.NotImplementedException())",
                        Enumerable.Empty<string>(),
                        $"global.{KnownNamespaces.SystemWindows}.UIElement",
                        _settings.AssemblyName,
                        _fileNameWithPathRelativeToProjectRoot);

                    string finalCode;
                    if (!string.IsNullOrEmpty(namespaceStringIfAny))
                    {
                        finalCode = $@"
namespace {namespaceStringIfAny}
{partialClass}

namespace global
{factoryClass}
";
                    }
                    else
                    {
                        finalCode = $@"
{partialClass}
{factoryClass}";
                    }

                    return finalCode;
                }
                else
                {
                    string finalCode = GenerateFactoryClass(
                        baseType,
                        baseType,
                        GeneratingCode.GetUniqueName(_reader.Document.Root),
                        "        raise (global.System.NotImplementedException())",
                        "        raise (global.System.NotImplementedException())",
                        Enumerable.Empty<string>(),
                        $"global.{KnownNamespaces.SystemWindows}.UIElement",
                        _settings.AssemblyName,
                        _fileNameWithPathRelativeToProjectRoot);

                    if (!string.IsNullOrEmpty(namespaceStringIfAny))
                    {
                        finalCode = $@"
namespace global
{finalCode}
";
                    } else {
                        finalCode = $@"
namespace GlobalResource
{finalCode}";
                    }
                    
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
        }
    }
}
