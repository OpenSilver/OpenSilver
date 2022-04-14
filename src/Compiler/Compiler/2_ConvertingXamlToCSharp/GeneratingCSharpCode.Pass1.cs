
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

namespace DotNetForHtml5.Compiler
{
    internal static partial class GeneratingCSharpCode
    {
        private class GeneratorPass1 : ICodeGenerator
        {
            private readonly XamlReader _reader;
            private readonly string _fileNameWithPathRelativeToProjectRoot;
            private readonly string _assemblyNameWithoutExtension;
            private readonly ReflectionOnSeparateAppDomainHandler _reflectionOnSeparateAppDomain;
            private readonly bool _isSLMigration;

            public GeneratorPass1(XDocument doc,
                string assemblyNameWithoutExtension,
                string fileNameWithPathRelativeToProjectRoot,
                ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain,
                bool isSLMigration)
            {
                _reader = new XamlReader(doc);
                _assemblyNameWithoutExtension = assemblyNameWithoutExtension;
                _fileNameWithPathRelativeToProjectRoot = fileNameWithPathRelativeToProjectRoot;
                _isSLMigration = isSLMigration;
                _reflectionOnSeparateAppDomain = reflectionOnSeparateAppDomain;
            }

            public string Generate() => GenerateImpl();

            private string GenerateImpl()
            {
                // Get general information about the class:
                string className, namespaceStringIfAny, baseType;
                bool hasCodeBehind;
                GetClassInformationFromXaml(_reader.Document, _reflectionOnSeparateAppDomain,
                    out className, out namespaceStringIfAny, out baseType, out hasCodeBehind);

                HashSet<string> listOfAllTheTypesUsedInThisXamlFile = new HashSet<string>();
                List<string> resultingFieldsForNamedElements = new List<string>();
                List<string> resultingMethods = new List<string>();

                while (_reader.Read())
                {
                    if (_reader.NodeType != XamlNodeType.StartObject)
                        continue;

                    XElement element = _reader.ObjectData.Element;

                    // Get the namespace, local name, and optional assembly that correspond to the element
                    string namespaceName, localTypeName, assemblyNameIfAny;
                    GettingInformationAboutXamlTypes.GetClrNamespaceAndLocalName(element.Name, out namespaceName, out localTypeName, out assemblyNameIfAny);
                    string elementTypeInCSharp = _reflectionOnSeparateAppDomain.GetCSharpEquivalentOfXamlTypeAsString(
                        namespaceName, localTypeName, assemblyNameIfAny, true);

                    if (hasCodeBehind ||
                        elementTypeInCSharp.StartsWith("global::"))
                    {
                        // if hasCodeBehing is true, we have a namespace so we can assume that 
                        // the type is either fully specified or that it is in the current
                        // namespace.
                        // Otherwise, we only take fully specified types (because we are in the
                        // global namespace).
                        listOfAllTheTypesUsedInThisXamlFile.Add(elementTypeInCSharp);
                    }

                    if (!hasCodeBehind)
                    {
                        // No code behind, no need to create fields for elementd with an x:Name
                        continue;
                    }

                    XAttribute xNameAttr = element.Attributes().FirstOrDefault(attr => IsAttributeTheXNameAttribute(attr));
                    if (xNameAttr != null && GetRootOfCurrentNamescopeForCompilation(element).Parent == null)
                    {
                        string name = xNameAttr.Value;
                        if (!string.IsNullOrWhiteSpace(name))
                        {
                            string fieldModifier = _isSLMigration ? "internal" : "protected";
                            XAttribute fieldModifierAttr = element.Attribute(xNamespace + "FieldModifier");
                            if (fieldModifierAttr != null)
                            {
                                fieldModifier = fieldModifierAttr.Value?.ToLower() ?? "private";
                            }

                            // add '@' to handle cases where x:Name is a forbidden word (for instance 'this'
                            // or any other c# keyword)
                            string fieldName = "@" + name;
                            resultingFieldsForNamedElements.Add(string.Format("{0} {1} {2};", fieldModifier, elementTypeInCSharp, fieldName));
                        }
                    }
                }

                if (hasCodeBehind)
                {
                    // Create the "IntializeComponent()" method:
                    string initializeComponentMethod = CreateInitializeComponentMethod(GetUniqueName(_reader.Document.Root), null, new List<string>(0), null, null, null,
                        _isSLMigration, _assemblyNameWithoutExtension, _fileNameWithPathRelativeToProjectRoot);

                    resultingMethods.Add(initializeComponentMethod);

                    // Wrap everything into a partial class:
                    string partialClass = GeneratePartialClass(resultingMethods,
                                                               resultingFieldsForNamedElements,
                                                               className,
                                                               namespaceStringIfAny,
                                                               baseType,
                                                               _fileNameWithPathRelativeToProjectRoot,
                                                               _assemblyNameWithoutExtension,
                                                               listOfAllTheTypesUsedInThisXamlFile,
                                                               hasCodeBehind,
#if BRIDGE
                                                           addApplicationEntryPoint: IsClassTheApplicationClass(baseType)
#else
                                                           addApplicationEntryPoint: false
#endif
);

                    string componentTypeFullName = GetFullTypeName(namespaceStringIfAny, className);

                    string factoryClass = GenerateFactoryClass(
                        componentTypeFullName,
                        CreateFactoryMethod(componentTypeFullName),
                        Enumerable.Empty<string>(),
                        _assemblyNameWithoutExtension,
                        _fileNameWithPathRelativeToProjectRoot);

                    string finalCode = $@"
{factoryClass}
{partialClass}";

                    return finalCode;
                }
                else
                {
                    string factoryImpl = CreateFactoryMethod(
                        GetUniqueName(_reader.Document.Root),
                        baseType,
                        null,
                        new List<string>(0),
                        null,
                        null,
                        null,
                        _isSLMigration,
                        _assemblyNameWithoutExtension,
                        _fileNameWithPathRelativeToProjectRoot);

                    string finalCode = GenerateFactoryClass(
                        baseType,
                        factoryImpl,
                        Enumerable.Empty<string>(),
                        _assemblyNameWithoutExtension,
                        _fileNameWithPathRelativeToProjectRoot);

                    return finalCode;
                }
            }

            private static XElement GetRootOfCurrentNamescopeForCompilation(XElement element)
            {
                while (element.Parent != null)
                {
                    if (element.Name.Namespace == DefaultXamlNamespace && DoesClassInheritFromFrameworkTemplate(element.Name.LocalName))
                    {
                        return element;
                    }
                    element = element.Parent;
                }
                return element;
            }

            private static bool DoesClassInheritFromFrameworkTemplate(string classLocalName) //todo: add support for namespace for more precision
            {
                return classLocalName == "DataTemplate" || classLocalName == "ItemsPanelTemplate" || classLocalName == "ControlTemplate";
            }
        }
    }
}
