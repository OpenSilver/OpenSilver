
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
using OpenSilver.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace OpenSilver.Compiler
{
    internal static partial class GeneratingCSCode
    {
        private class GeneratorPass2 : ICodeGenerator
        {
            private abstract class GeneratorScope
            {
                private readonly StringBuilder _stringBuilder = new();

                protected GeneratorScope(string rootElement)
                {
                    Root = rootElement;
                    XamlContext = GeneratingUniqueNames.GenerateUniqueName("xamlContext");
                }

                public string Root { get; }

                public string XamlContext { get; }

                public void AppendLine(string value) => _stringBuilder.AppendLine(value);

                public abstract void RegisterName(string name, string scopedElement);

                public string Build() => BuildCore(_stringBuilder);

                protected abstract string BuildCore(StringBuilder stringBuilder);
            }

            private sealed class RootScope : GeneratorScope
            {
                private readonly bool _buildNamescope;

                public RootScope(string rootElementName, bool createNameScope)
                    : base(rootElementName)
                {
                    _buildNamescope = createNameScope;

                    AppendLine($"var {XamlContext} = {RuntimeHelperClass}.Create_XamlContext();");
                    if (createNameScope)
                    {
                        AppendLine($"{RuntimeHelperClass}.XamlContext_InitializeNameScope({XamlContext}, {rootElementName});");
                    }
                }

                public override void RegisterName(string name, string scopedElement)
                {
                    if (_buildNamescope)
                    {
                        AppendLine($"{RuntimeHelperClass}.XamlContext_RegisterName({XamlContext}, {EscapeString(name)}, {scopedElement});");
                    }
                }

                protected override string BuildCore(StringBuilder stringBuilder) => stringBuilder.ToString();
            }

            private sealed class NewObjectScope : GeneratorScope
            {
                public NewObjectScope(string objectName, string objectType)
                    : base(objectName)
                {
                    ObjectType = objectType;
                    MethodName = $"New_{objectName}";
                }

                public string MethodName { get; }

                public string ObjectType { get; }

                public override void RegisterName(string name, string scopedElement)
                {
                    AppendLine($"{RuntimeHelperClass}.XamlContext_RegisterName({XamlContext}, {EscapeString(name)}, {scopedElement});");
                }

                protected override string BuildCore(StringBuilder stringBuilder)
                {
                    StringBuilder builder = new StringBuilder();

                    builder.AppendLine($"private static {ObjectType} {MethodName}({XamlContextClass} {XamlContext})")
                        .AppendLine("{")
                        .Append(stringBuilder.ToString());
                    builder.AppendLine($"return {Root};")
                        .AppendLine("}");

                    return builder.ToString();
                }
            }

            private sealed class FrameworkTemplateScope : GeneratorScope
            {
                public FrameworkTemplateScope(string templateName, string templateRoot)
                    : base(templateRoot)
                {
                    Name = templateName;
                    TemplateOwner = $"templateOwner_{templateName}";
                    MethodName = $"Create_{templateName}";
                }

                public string Name { get; }

                public string TemplateOwner { get; }

                public string MethodName { get; }

                public override void RegisterName(string name, string scopedElement)
                {
                    AppendLine($"{RuntimeHelperClass}.XamlContext_RegisterName({XamlContext}, {EscapeString(name)}, {scopedElement});");
                }

                protected override string BuildCore(StringBuilder stringBuilder)
                {
                    StringBuilder builder = new StringBuilder();

                    builder.AppendLine($"private static global::{KnownNamespaces.SystemWindows}.IFrameworkElement {MethodName}(global::{KnownNamespaces.SystemWindows}.IFrameworkElement {TemplateOwner}, {XamlContextClass} {XamlContext})")
                        .AppendLine("{")
                        .Append(stringBuilder.ToString());
                    builder.AppendLine($"return {Root};")
                        .AppendLine("}");

                    return builder.ToString();
                }
            }

            private sealed class GeneratorContext
            {
                private readonly Stack<GeneratorScope> _scopes = new();

                public readonly List<string> ResultingMethods = new List<string>();
                public readonly List<string> ResultingFieldsForNamedElements = new List<string>();
                public readonly List<string> ResultingFindNameCalls = new List<string>();
                public readonly ComponentConnectorBuilderCS ComponentConnector = new ComponentConnectorBuilderCS();
                private readonly string _sourceFile;
                private int _frameworkTemplateCount = 0;

                public GeneratorContext(string sourceFile)
                {
                    _sourceFile = sourceFile;
                }

                public bool GenerateFieldsForNamedElements { get; set; }
                public bool IsInsideTemplate => _frameworkTemplateCount > 0;
                public GeneratorScope CurrentScope => _scopes.Peek();
                public string CurrentXamlContext => CurrentScope.XamlContext;

                public void PushScope(GeneratorScope scope)
                {
                    _scopes.Push(scope);

                    if (scope is FrameworkTemplateScope)
                    {
                        _frameworkTemplateCount++;
                    }
                }

                public void PopScope()
                {
                    if (_scopes.Count <= 1)
                    {
                        throw new InvalidOperationException();
                    }

                    GeneratorScope scope = _scopes.Pop();

                    if (scope is FrameworkTemplateScope)
                    {
                        _frameworkTemplateCount--;
                    }

                    ResultingMethods.Add(scope.Build());
                }

                public GeneratorContext AppendLine(string value)
                {
                    CurrentScope.AppendLine(value);
                    return this;
                }

                public IDisposable CreateLineScope(IXmlLineInfo lineInfo)
                {
                    if (lineInfo is not null && lineInfo.HasLineInfo())
                    {
                        return new LineDirectiveScope(CurrentScope, lineInfo, _sourceFile);
                    }

                    return null;
                }

                private struct LineDirectiveScope : IDisposable
                {
                    private GeneratorScope _scope;

                    public LineDirectiveScope(GeneratorScope scope, IXmlLineInfo lineInfo, string sourceFile)
                    {
                        _scope = scope;

                        // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-10.0/enhanced-line-directives#detailed-design
                        scope.AppendLine($"#line ({lineInfo.LineNumber}, {lineInfo.LinePosition}) - ({lineInfo.LineNumber}, {lineInfo.LinePosition}) 65536 \"{sourceFile}\"");
                    }

                    public void Dispose()
                    {
                        if (_scope is GeneratorScope scope)
                        {
                            _scope = null;
                            scope.AppendLine("#line default");
                        }
                    }
                }
            }

            private readonly XamlReader _reader;
            private readonly ConversionSettings _settings;
            private readonly XNodeSelector _nodeSelector;

            private readonly string _sourceFile;
            private readonly string _fileNameWithPathRelativeToProjectRoot;

            public GeneratorPass2(XDocument doc,
                string sourceFile,
                string fileNameWithPathRelativeToProjectRoot,
                ConversionSettings settings)
            {
                _reader = new XamlReader(doc);
                _settings = settings;
                _nodeSelector = XNodeSelector.Create(doc, settings.Options);
                _sourceFile = sourceFile;
                _fileNameWithPathRelativeToProjectRoot = fileNameWithPathRelativeToProjectRoot;
            }

            public string Generate()
            {
                GettingInformationAboutXamlTypes.GetClrNamespaceAndLocalName(_reader.Document.Root.Name,
                    out string namespaceName, out string typeName, out string assemblyName);

                var context = new GeneratorContext(_sourceFile);

                context.GenerateFieldsForNamedElements =
                    !_settings.Inspector.IsResourceDictionary(namespaceName, typeName, assemblyName, _reader.Document.Root) &&
                    !_settings.Inspector.IsApplication(namespaceName, typeName, assemblyName, _reader.Document.Root);

                context.PushScope(
                    new RootScope(GeneratingCode.GetUniqueName(_reader.Document.Root),
                        _settings.Inspector.IsIFrameworkElement(namespaceName, typeName, assemblyName, _reader.Document.Root)));

                return GenerateImpl(context);
            }

            private string GenerateImpl(GeneratorContext parameters)
            {
                // Traverse the tree in "post order" (ie. start with child elements then traverse parent elements):
                while (_reader.Read())
                {
                    switch (_reader.NodeType)
                    {
                        case XamlNodeType.StartObject:
                            if (!ShouldSkipObject(_reader.ObjectData.Element))
                            {
                                TryCatch(OnWriteStartObject, parameters);
                            }
                            break;

                        case XamlNodeType.EndObject:
                            if (!ShouldSkipObject(_reader.ObjectData.Element))
                            {
                                TryCatch(OnWriteEndObject, parameters);
                            }
                            break;

                        case XamlNodeType.StartMember:
                            TryCatch(OnWriteStartMember, parameters);
                            break;

                        case XamlNodeType.EndMember:
                            if (_reader.MemberData.Member != null)
                            {
                                TryCatch(OnWriteEndMember, parameters);
                            }
                            else
                            {
                                TryCatch(OnWriteEndMemberCollection, parameters);
                            }
                            break;
                    }
                }

                // Get general information about the class:
                GetClassInformationFromXaml(_reader.Document, _settings.Inspector,
                    out string className, out string namespaceStringIfAny, out bool hasCodeBehind);

                string baseType = GetCSharpEquivalentOfXamlTypeAsString(_reader.Document.Root.Name, _reader.Document.Root, true);

                if (hasCodeBehind)
                {
                    string connectMethod = parameters.ComponentConnector.ToString();
                    string initializeComponentMethod = CreateInitializeComponentMethod(
                        $"global::{KnownNamespaces.SystemWindows}.Application",
                        _settings.AssemblyName,
                        _fileNameWithPathRelativeToProjectRoot,
                        parameters.ResultingFindNameCalls);

                    // Wrap everything into a partial class:
                    string partialClass = GeneratePartialClass(_reader.Document.Root,
                                                               initializeComponentMethod,
                                                               connectMethod,
                                                               parameters.ResultingFieldsForNamedElements,
                                                               className,
                                                               namespaceStringIfAny,
                                                               baseType,
                                                               _sourceFile);

                    string componentTypeFullName = GetFullTypeName(namespaceStringIfAny, className);

                    string factoryClass = GenerateFactoryClass(
                        _reader.Document.Root,
                        componentTypeFullName,
                        baseType,
                        GeneratingCode.GetUniqueName(_reader.Document.Root),
                        parameters.CurrentScope.Build(),
                        $"return ({componentTypeFullName})global::System.Activator.CreateInstance(typeof({componentTypeFullName}), true);",
                        parameters.ResultingMethods,
                        $"global::{KnownNamespaces.SystemWindows}.UIElement",
                        _settings.AssemblyName,
                        _fileNameWithPathRelativeToProjectRoot,
                        _sourceFile);

                    string finalCode = $@"
{factoryClass}
{partialClass}";

                    return finalCode;
                }
                else
                {
                    string rootElementName = GeneratingCode.GetUniqueName(_reader.Document.Root);

                    string finalCode = GenerateFactoryClass(
                        _reader.Document.Root,
                        baseType,
                        baseType,
                        rootElementName,
                        parameters.CurrentScope.Build(),
                        string.Join(Environment.NewLine, $"var {rootElementName} = new {baseType}();", $"LoadComponentImpl({rootElementName});", $"return {rootElementName};"),
                        parameters.ResultingMethods,
                        $"global::{KnownNamespaces.SystemWindows}.UIElement",
                        _settings.AssemblyName,
                        _fileNameWithPathRelativeToProjectRoot,
                        _sourceFile);

                    return finalCode;
                }
            }

            private bool ShouldSkipObject(XElement element)
            {
                if (element.Attribute(InsertingMarkupNodesInXaml.GeneratedMarkupExtensionAttribute) is not null)
                {
                    // For these markup extensions, we resolve the value at compile time, so we don't need to
                    // instantiate the markup extension.
                    if (GeneratingCode.IsNullExtension(element) ||
                        GeneratingCode.IsStaticExtension(element) ||
                        GeneratingCode.IsTypeExtension(element))
                    {
                        return true;
                    }
                }

                return false;
            }

            private void OnWriteStartObject(GeneratorContext parameters)
            {
                XElement element = _reader.ObjectData.Element;

                // Check if the element is the root element:
                string elementType = GetCSharpEquivalentOfXamlTypeAsString(
                    element,
                    out string namespaceName,
                    out string localTypeName,
                    out string assemblyNameIfAny);

                // Add the constructor (in case of object) or a direct initialization (in case
                // of system type or "isInitializeFromString" or referenced ResourceDictionary)
                // (unless this is the root element)
                string elementUid = GeneratingCode.GetUniqueName(element);

                if (_nodeSelector.IsMatch(element))
                {
                    var objectScope = new NewObjectScope(elementUid, elementType);

                    parameters.AppendLine(
                        $"var {elementUid} = {objectScope.MethodName}({parameters.CurrentXamlContext});");

                    parameters.PushScope(objectScope);
                }

                // Some special cases
                if (elementType == $"global::{KnownNamespaces.SystemWindows}.EventSetter")
                {
                    WriteEventSetter(parameters);

                    // EventSetter only support the Event, Handler and HandledEventsToo properties. WriteEventSetter
                    // already takes care of these properties, so we just skip everything.
                    while (_reader.Read())
                    {
                        if (_reader.NodeType == XamlNodeType.EndObject && _reader.ObjectData.Element == element)
                        {
                            OnWriteEndObject(parameters);
                            break;
                        }
                    }

                    return;
                }

                if (IsElementTheRootElement(element))
                {
                    parameters.AppendLine($"_ = {RuntimeHelperClass}.XamlContext_WriteStartObject({parameters.CurrentXamlContext}, {elementUid});");
                }
                else
                {
                    if (_settings.SystemTypes.IsKnownType(elementType.Substring("global::".Length), assemblyNameIfAny))
                    {
                        //------------------------------------------------
                        // Add the type initialization from literal value:
                        //------------------------------------------------
                        string directContent;
                        if (element.FirstNode is XText xText)
                        {
                            directContent = xText.Value;
                        }
                        else
                        {
                            // If the direct content is not specified, we use the type's
                            // default value (ex: <sys:String></sys:String>)
                            directContent = _settings.SystemTypes.GetDefaultValue(namespaceName, localTypeName, assemblyNameIfAny);
                        }

                        string preparedValue = _settings.SystemTypes.ConvertKnownType(directContent, elementType.Substring("global::".Length));

                        using (parameters.CreateLineScope(element))
                        {
                            parameters.AppendLine(
                                $"var {elementUid} = {RuntimeHelperClass}.XamlContext_WriteStartObject({parameters.CurrentXamlContext}, {preparedValue});");
                        }
                    }
                    else if (element.Attribute(InsertingImplicitNodes.InitializedFromStringAttribute) != null)
                    {
                        //------------------------------------------------
                        // Add the type initialization from string:
                        //------------------------------------------------

                        string stringValue = element.Attribute(InsertingImplicitNodes.InitializedFromStringAttribute).Value;

                        bool isKnownCoreType = _settings.CoreTypes.IsKnownType(
                            elementType.Substring("global::".Length), assemblyNameIfAny);

                        string preparedValue = ConvertFromInvariantString(
                            stringValue, element, elementType, isKnownCoreType, false);

                        using (parameters.CreateLineScope(element))
                        {
                            parameters.AppendLine(
                                $"var {elementUid} = {RuntimeHelperClass}.XamlContext_WriteStartObject({parameters.CurrentXamlContext}, {preparedValue});");
                        }
                    }
                    else
                    {
                        using (parameters.CreateLineScope(element))
                        {
                            parameters.AppendLine(
                                $"var {elementUid} = {RuntimeHelperClass}.XamlContext_WriteStartObject({parameters.CurrentXamlContext}, new {elementType}());");
                        }

                        if (IsResourceDictionaryCreatedFromSource(element))
                        {
                            //------------------------------------------------
                            // Add the type initialization from "Source" URI:
                            //------------------------------------------------
                            string absoluteSourceUri = PathsHelper.ConvertToAbsolutePathWithComponentSyntax(
                                element.Attribute("Source").Value,
                                _fileNameWithPathRelativeToProjectRoot,
                                _settings.AssemblyName);
                            string loadTypeFullName = XamlResourcesHelper.GenerateClassNameFromComponentUri(absoluteSourceUri);

                            parameters.AppendLine(
                                $"(({IXamlComponentLoaderClass})new {loadTypeFullName}()).LoadComponent({elementUid});");
                        }
                    }
                }

                // Set templated parent if any
                if (parameters.IsInsideTemplate &&
                    _settings.Inspector.IsIFrameworkElement(namespaceName, localTypeName, assemblyNameIfAny, element))
                {
                    parameters.AppendLine(
                        $"{RuntimeHelperClass}.XamlContext_SetTemplatedParent({parameters.CurrentXamlContext}, {elementUid});");
                }

                if (_settings.Inspector.IsIUIElement(namespaceName, localTypeName, assemblyNameIfAny, element))
                {
                    string xamlPath = element.Attribute(GeneratingPathInXaml.PathInXamlAttribute)?.Value ?? string.Empty;
                    parameters.AppendLine($"{XamlDesignerBridgeClass}.SetPathInXaml({elementUid}, \"{xamlPath}\");");
                    parameters.AppendLine($"{XamlDesignerBridgeClass}.SetFilePath({elementUid}, @\"{_sourceFile}\");");
                }

                // Add the attributes:
                foreach (XAttribute attribute in element.Attributes())
                {
                    //-------------
                    // ATTRIBUTE
                    //-------------

                    string attributeValue = GeneratingCode.GetAttributeValue(attribute);
                    string attributeName = attribute.Name.LocalName;

                    // Skip the utility attributes:
                    if (!IsReservedAttribute(attributeName)
                        && !attribute.IsNamespaceDeclaration)
                    {
                        // Verify that the attribute is not an attached property:
                        //todo: This test does not work 100% of the time. For example if we have <Grid Column="1" ..../> the compiler thinks that Column is a normal property whereas it actually is an attached property.
                        bool isAttachedMember = attributeName.Contains(".");
                        if (!isAttachedMember)
                        {
                            bool isXNameAttr = GeneratingCode.IsXNameAttribute(attribute);
                            if (isXNameAttr || GeneratingCode.IsNameAttribute(attribute))
                            {
                                //-------------
                                // x:Name (or "Name")
                                //-------------

                                string name = attributeValue;

                                // Add the code to register the name, etc.
                                if (!parameters.IsInsideTemplate && parameters.GenerateFieldsForNamedElements)
                                {
                                    string fieldModifier = "internal";
                                    XAttribute attr = element.Attribute(GeneratingCode.xNamespace + "FieldModifier");
                                    if (attr != null)
                                    {
                                        fieldModifier = (attr.Value ?? "").ToLower();
                                    }

                                    // add '@' to handle cases where x:Name is a forbidden word (for instance 'this'
                                    // or any other c# keyword)
                                    string fieldName = "@" + name;
                                    parameters.ResultingFieldsForNamedElements.Add($"{fieldModifier} {elementType} {fieldName};");
                                    parameters.ResultingFindNameCalls.Add($"this.{fieldName} = (({elementType})(this.FindName(\"{name}\")));");
                                }

                                if (isXNameAttr)
                                {
                                    if (_settings.Inspector.IsDependencyObject(namespaceName, localTypeName, assemblyNameIfAny, element))
                                    {
                                        parameters.AppendLine(
                                            $"{elementUid}.SetValue(global::{KnownNamespaces.SystemWindows}.FrameworkElement.NameProperty, \"{name}\");");
                                    }
                                }
                                else
                                {
                                    if (_settings.Inspector.DoesTypeContainNameMemberOfTypeString(namespaceName, localTypeName, assemblyNameIfAny, attribute))
                                    {
                                        parameters.AppendLine($"{elementUid}.Name = \"{name}\";");
                                    }
                                }

                                parameters.CurrentScope.RegisterName(name, elementUid);
                            }
                            else if (string.IsNullOrEmpty(attribute.Name.NamespaceName) || attribute.Name.NamespaceName == element.Name.NamespaceName)
                            {
                                //-------------
                                // Attributes without namespace
                                //-------------

                                // Verify that there are no markups (they are supposed to have been replaced by XML nodes before entering this method - cf. InsertingMarkupNodesInXaml.InsertMarkupNodes(..)):
                                //if (!attributeValue.StartsWith("{"))
                                if (!InsertingMarkupNodesInXaml.IsMarkupExtension(attribute))
                                {
                                    // Check if the attribute corresponds to a Property, an Event, etc.:
                                    string memberName = attribute.Name.LocalName;
                                    MemberTypes memberType = _settings.Inspector.GetMemberType(memberName, namespaceName, localTypeName, assemblyNameIfAny, attribute);
                                    switch (memberType)
                                    {
                                        case MemberTypes.Event:

                                            //------------
                                            // C# EVENT
                                            //------------

                                            int componentId = parameters.ComponentConnector.ConnectEventHandler(elementType, attributeName, attributeValue);
                                            parameters.AppendLine(
                                                $"{RuntimeHelperClass}.XamlContext_SetConnectionId({parameters.CurrentXamlContext}, {componentId}, {elementUid});");

                                            break;
                                        case MemberTypes.Field:
                                        case MemberTypes.Property:

                                            //------------
                                            // C# PROPERTY
                                            //------------

                                            // Generate the code for instantiating the attribute value:
                                            string value;
                                            if (elementType == $"global::{KnownNamespaces.SystemWindows}.Setter")
                                            {
                                                value = attributeName switch
                                                {
                                                    "Property" => GenerateCodeForSetterProperty(attribute),
                                                    "Value" => GenerateCodeForSetterValue(attribute),
                                                    "TargetName" => _settings.SystemTypes.ConvertToString(attributeValue),
                                                    _ => throw new XamlParseException(
                                                        "The '<Setter />' element cannot have attributes other than 'Property', 'Value' and 'TargetName'.",
                                                        element),
                                                };
                                            }
                                            else if (elementType == $"global::{KnownNamespaces.SystemWindows}.Trigger")
                                            {
                                                value = attributeName switch
                                                {
                                                    "Property" => GenerateCodeForTriggerProperty(attribute),
                                                    "Value" => GenerateCodeForTriggerValue(attribute),
                                                    "SourceName" => _settings.SystemTypes.ConvertToString(attributeValue),
                                                    _ => throw new XamlParseException(
                                                        "The '<Trigger />' element cannot have attributes other than 'Property', 'Value' and 'SourceName'.",
                                                        element),
                                                };
                                            }
                                            else if (elementType == $"global::{KnownNamespaces.SystemWindows}.Condition")
                                            {
                                                value = attributeName switch
                                                {
                                                    "Property" => GenerateCodeForConditionProperty(attribute),
                                                    "Value" => GenerateCodeForConditionValue(attribute),
                                                    "SourceName" => _settings.SystemTypes.ConvertToString(attributeValue),
                                                    _ => throw new XamlParseException(
                                                        "The '<Condition />' element cannot have attributes other than 'Property', 'Value' and 'SourceName'.",
                                                        element),
                                                };
                                            }
                                            else if (elementType == $"global::{KnownNamespaces.SystemWindowsData}.Binding"
                                                && memberName == "Path")
                                            {
                                                if (TryResolvePathForBinding(attributeValue, element, attribute, out string resolvedPath))
                                                {
                                                    string xamlPath = _settings.SystemTypes.ConvertToString(resolvedPath);
                                                    parameters.AppendLine($"{elementUid}.XamlPath = {xamlPath};");
                                                }

                                                XName typeName = element.Name;
                                                string propertyName = attribute.Name.LocalName;

                                                value = GenerateCodeForInstantiatingAttributeValue(
                                                    typeName,
                                                    propertyName,
                                                    isAttachedMember,
                                                    attributeValue,
                                                    element,
                                                    attribute);
                                            }
                                            else if (elementType == $"global::{KnownNamespaces.SystemWindows}.TemplateBindingExtension" && memberName == "Path")
                                            {
                                                ResolvePathForTemplateBinding(attributeValue, element, out string typeName, out string propertyName);
                                                parameters.AppendLine(
                                                    $"{elementUid}.DependencyPropertyName = {_settings.SystemTypes.ConvertToString(propertyName)};");

                                                if (typeName != null)
                                                {
                                                    parameters.AppendLine(
                                                        $"{elementUid}.DependencyPropertyOwnerType = typeof({typeName});");
                                                }

                                                value = null;
                                            }
                                            else
                                            {
                                                //------------
                                                // NORMAL C# PROPERTY
                                                //------------

                                                XName typeName = element.Name;
                                                string propertyName = attribute.Name.LocalName;

                                                value = GenerateCodeForInstantiatingAttributeValue(
                                                    typeName,
                                                    propertyName,
                                                    isAttachedMember,
                                                    attributeValue,
                                                    element,
                                                    attribute);
                                            }

                                            // Append the statement:
                                            if (value != null)
                                            {
                                                using (parameters.CreateLineScope(attribute))
                                                {
                                                    parameters.AppendLine($"{elementUid}.{attributeName} = {value};");
                                                }
                                            }

                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            //-------------
                            // ATTACHED PROPERTY OR EVENT
                            //-------------

                            string[] split = attribute.Name.LocalName.Split('.');

                            XNamespace attributeNS = attribute.Name.Namespace == XNamespace.None ?
                                element.GetDefaultNamespace() :
                                attribute.Name.Namespace;
                            XName ownerTypeXName = attributeNS + split[0];
                            string memberName = split[1];

                            GettingInformationAboutXamlTypes.GetClrNamespaceAndLocalName(
                                ownerTypeXName,
                                out string ownerTypeNamespace,
                                out string ownerTypeName,
                                out string ownerTypeAssemblyName);

                            (MemberTypes memberType, MethodDefinition method, TypeReference declaringType) =
                                _settings.Inspector.GetAttachedMemberType(
                                    memberName, ownerTypeNamespace, ownerTypeName, ownerTypeAssemblyName, attribute);

                            switch (memberType)
                            {
                                case MemberTypes.Property:
                                    {
                                        string ownerType = $"global::{_settings.TypeReferenceHelper.ConvertToString(declaringType)}";
                                        string value = GenerateCodeForInstantiatingAttributeValue(
                                            ownerTypeXName,
                                            memberName,
                                            isAttachedMember,
                                            attributeValue,
                                            element,
                                            attribute);

                                        using (parameters.CreateLineScope(attribute))
                                        {
                                            parameters.AppendLine(
                                                $"{ownerType}.Set{memberName}({elementUid}, {value});");
                                        }
                                    }
                                    break;

                                case MemberTypes.Event:
                                    {
                                        string ownerType = $"global::{_settings.TypeReferenceHelper.ConvertToString(declaringType)}";
                                        int componentId = parameters.ComponentConnector.ConnectAttachedEventHandler(elementType, ownerType, memberName, attributeValue);

                                        parameters.AppendLine(
                                            $"{RuntimeHelperClass}.XamlContext_SetConnectionId({parameters.CurrentXamlContext}, {componentId}, {elementUid});");
                                    }
                                    break;

                                default:
                                    throw new XamlParseException(
                                        $"The property '{attribute.Name.LocalName}' does not exist in XML namespace '{attribute.Name.NamespaceName}'.",
                                        attribute);
                            }
                        }
                    }
                }
            }

            private void WriteEventSetter(GeneratorContext parameters)
            {
                XElement eventSetter = _reader.ObjectData.Element;

                // we get the parent Style node (since there is a Style.Setters node that is added, the parent style node is )
                if (eventSetter.Parent is null || eventSetter.Parent.Parent is null || eventSetter.Parent.Parent.Name.LocalName != "Style")
                {
                    throw new XamlParseException("'<EventSetter />' tags can only be declared inside a '<Style />'.", eventSetter);
                }

                XElement style = eventSetter.Parent.Parent;
                XAttribute eventAttribute = eventSetter.Attribute("Event"); // required
                XAttribute handlerAttribute = eventSetter.Attribute("Handler"); // required
                XAttribute handledEventsTooAttribute = eventSetter.Attribute("HandledEventsToo");
                IXmlLineInfo lineInfo = eventSetter;

                if (eventAttribute is null || handlerAttribute is null)
                {
                    throw new XamlParseException("'EventSetter' must declare an 'Event' and a 'Handler'.", eventSetter);
                }

                // First, find the event
                string eventName, namespaceName, typeName, assemblyName;

                string eventAttributeValue = GeneratingCode.GetAttributeValue(eventAttribute);

                int index = eventAttributeValue.IndexOf('.');
                if (index >= 0)
                {
                    GettingInformationAboutXamlTypes.GetClrNamespaceAndLocalName(
                        eventAttributeValue.Substring(0, index), eventSetter, out namespaceName, out typeName, out assemblyName);
                    eventName = eventAttributeValue.Substring(index + 1);
                }
                else
                {
                    index = eventAttributeValue.IndexOf(':');
                    if (index >= 0)
                    {
                        // WPF ignore everything before the ':'
                        eventName = eventAttributeValue.Substring(index + 1);
                    }
                    else
                    {
                        eventName = eventAttributeValue;
                    }

                    if (style.Attribute("TargetType") is XAttribute targetType)
                    {
                        lineInfo = targetType;

                        GettingInformationAboutXamlTypes.GetClrNamespaceAndLocalName(
                            targetType.Value, style, out namespaceName, out typeName, out assemblyName);
                    }
                    else
                    {
                        namespaceName = KnownNamespaces.SystemWindows;
                        typeName = "FrameworkElement";
                        assemblyName = "OpenSilver";
                    }
                }

                string handlerTypeString;

                TypeDefinition ownerType = _settings.Inspector.GetTypeDefinition(namespaceName, typeName, assemblyName, lineInfo);
                string ownerTypeString = _settings.TypeReferenceHelper.ConvertToString(ownerType);

                if (_settings.Inspector.GetEvent(ownerType, eventName, false) is EventDefinition eventDefinition)
                {
                    handlerTypeString = _settings.TypeReferenceHelper.ConvertToString(eventDefinition.EventType);
                }
                else if (_settings.Inspector.GetMethod(ownerType, $"Add{eventName}Handler", false, true) is MethodDefinition addHandlerMethodDefinition &&
                    addHandlerMethodDefinition.Parameters.Count == 2)
                {
                    handlerTypeString = _settings.TypeReferenceHelper.ConvertToString(addHandlerMethodDefinition.Parameters[1].ParameterType);
                }
                else
                {
                    throw new XamlParseException($"Cannot find the Style Event '{eventName}' on the type '{ownerTypeString}'.", eventSetter);
                }

                // Start generating the code
                int componentId = parameters.ComponentConnector.ConnectEventSetterHandler("global::" + handlerTypeString, GeneratingCode.GetAttributeValue(handlerAttribute));

                string eventSetterName = GeneratingCode.GetUniqueName(eventSetter);

                parameters.AppendLine(
                    $"global::{KnownNamespaces.SystemWindows}.EventSetter {eventSetterName} = {RuntimeHelperClass}.XamlContext_WriteStartObject({parameters.CurrentXamlContext}, new global::{KnownNamespaces.SystemWindows}.EventSetter());");

                parameters.AppendLine(
                    $"{eventSetterName}.Event = {RuntimeHelperClass}.RoutedEventFromName(\"{eventName}\", typeof(global::{ownerTypeString}));");

                parameters.AppendLine(
                    $"{RuntimeHelperClass}.XamlContext_SetConnectionId({parameters.CurrentXamlContext}, {componentId}, {eventSetterName});");

                if (handledEventsTooAttribute is not null)
                {
                    string value = _settings.SystemTypes.ConvertToBoolean(GeneratingCode.GetAttributeValue(handledEventsTooAttribute));
                    parameters.AppendLine($"{eventSetterName}.HandledEventsToo = {value};");
                }
            }

            private void OnWriteEndObject(GeneratorContext parameters)
            {
                parameters.AppendLine($"{RuntimeHelperClass}.XamlContext_WriteEndObject({parameters.CurrentXamlContext});");

                if (_nodeSelector.IsMatch(_reader.ObjectData.Element))
                {
                    parameters.PopScope();
                }
            }

            private void OnWriteStartMember(GeneratorContext parameters)
            {
                XElement element = _reader.MemberData.Target;
                XElement member = _reader.MemberData.Member;

                int idx = member.Name.LocalName.IndexOf('.');

                string typeName = member.Name.LocalName.Substring(0, idx);
                (string namespaceName, string assemblyName) = GettingInformationAboutXamlTypes.GetClrNamespaceAndAssembly(
                    member.Name.NamespaceName);

                string propertyName = member.Name.LocalName.Substring(idx + 1);

                if (_settings.Inspector.IsFrameworkTemplateTemplateProperty(propertyName, namespaceName, typeName, assemblyName, member))
                {
                    if (member.Elements().Count() > 1)
                    {
                        throw new XamlParseException("A FrameworkTemplate cannot have more than one child.", element);
                    }

                    string frameworkTemplateName = GeneratingCode.GetUniqueName(element);

                    var scope = new FrameworkTemplateScope(frameworkTemplateName, GeneratingCode.GetUniqueName(member.Elements().First()));

                    parameters.AppendLine($"{RuntimeHelperClass}.SetTemplateContent({frameworkTemplateName}, {parameters.CurrentXamlContext}, {scope.MethodName});");

                    parameters.PushScope(scope);
                }
            }

            private void OnWriteEndMember(GeneratorContext parameters)
            {
                XElement element = _reader.MemberData.Member;

                // Get information about the parent element (to which the property applies) and the element itself:
                XElement parent = element.Parent;
                string parentUid = GeneratingCode.GetUniqueName(parent);

                int idx = element.Name.LocalName.IndexOf('.');

                string typeName = element.Name.LocalName.Substring(0, idx);
                (string namespaceName, string assemblyName) = GettingInformationAboutXamlTypes.GetClrNamespaceAndAssembly(
                    element.Name.NamespaceName);

                string propertyName = element.Name.LocalName.Substring(idx + 1);

                if (_settings.Inspector.IsFrameworkTemplateTemplateProperty(propertyName, namespaceName, typeName, assemblyName, element))
                {
                    // TODO move call to RuntimeHelpers.SetTemplateContent(...) here
                    parameters.PopScope();
                }
                else
                {
                    XElement child = _reader.MemberData.Value;
                    string childUid = GeneratingCode.GetUniqueName(child);

                    bool isAttachedProperty = IsPropertyAttached(element);
                    XName elementName = element.Name.Namespace + typeName;

                    // Check if the property is a collection, in which case we must use ".Add(...)", otherwise a simple "=" is enough:
                    if (IsPropertyACollection(element, isAttachedProperty))
                    {
                        //------------------------
                        // PROPERTY TYPE IS A COLLECTION
                        //------------------------

                        string codeToAccessTheEnumerable;
                        if (isAttachedProperty)
                        {
                            string elementType = _settings.Inspector.GetCSharpEquivalentOfXamlTypeAsString(
                                elementName.Namespace.NamespaceName,
                                elementName.LocalName,
                                assemblyName,
                                element);

                            codeToAccessTheEnumerable = $"{elementType}.Get{propertyName}({parentUid})";
                        }
                        else
                        {
                            codeToAccessTheEnumerable = $"{parentUid}.{propertyName}";
                        }

                        if (IsPropertyOrFieldADictionary(element, isAttachedProperty))
                        {
                            string childKey = GetElementXKey(child, out bool isImplicitStyle, out bool isImplicitDataTemplate);
                            if (isImplicitStyle)
                            {
                                parameters.AppendLine($"((global::System.Collections.IDictionary){codeToAccessTheEnumerable}).Add(typeof({childKey}), {childUid});");
                            }
                            else if (isImplicitDataTemplate)
                            {
                                string key = $"new global::{KnownNamespaces.SystemWindows}.DataTemplateKey(typeof({childKey}))";

                                parameters.AppendLine($"((global::System.Collections.IDictionary){codeToAccessTheEnumerable}).Add({key}, {childUid});");
                            }
                            else
                            {
                                parameters.AppendLine($"((global::System.Collections.IDictionary){codeToAccessTheEnumerable}).Add(\"{childKey}\", {childUid});");
                            }
                        }
                        else
                        {
                            parameters.AppendLine($"((global::System.Collections.IList){codeToAccessTheEnumerable}).Add({childUid});");
                        }
                    }
                    else
                    {
                        //------------------------
                        // PROPERTY TYPE IS NOT A COLLECTION
                        //------------------------

                        // Note about "RelativeSource": even though it inherits from "MarkupExtension", we do not was
                        // to consider "RelativeSource" as a markup extension for the compilation because it is only
                        // meant to be used WITHIN another markup extension (sort of a "nested" markup extension),
                        // such as in: "{Binding Background, RelativeSource={RelativeSource Mode=TemplatedParent}}"
                        if (!IsElementAMarkupExtension(child) || (child.Name.LocalName == "RelativeSource"))
                        {
                            if (isAttachedProperty)
                            {
                                string elementType = _settings.Inspector.GetCSharpEquivalentOfXamlTypeAsString(
                                    elementName.Namespace.NamespaceName, elementName.LocalName, assemblyName, element);
                                parameters.AppendLine(
                                    $"{elementType}.Set{propertyName}({parentUid}, {childUid});"); // eg. MyCustomGridClass.SetRow(grid32877267T6, int45628789434);
                            }
                            else
                            {
                                parameters.AppendLine($"{parentUid}.{propertyName} = {childUid};");
                            }
                        }
                        else
                        {
                            //------------------------------
                            // MARKUP EXTENSIONS:
                            //------------------------------

                            if (child.Name.LocalName == "StaticResource" || child.Name.LocalName == "StaticResourceExtension" || child.Name.LocalName == "ThemeResourceExtension") //todo: see if there are other elements than StaticResource that need the parents //todo: check namespace as well?
                            {
                                //------------------------------
                                // {StaticResource ...}
                                //------------------------------

                                string[] splittedLocalName = element.Name.LocalName.Split('.');
                                string propertyNamespaceName, propertyLocalTypeName;

                                // Attached property
                                if (isAttachedProperty)
                                {
                                    string elementType = _settings.Inspector.GetCSharpEquivalentOfXamlTypeAsString(
                                        element.Name.NamespaceName, splittedLocalName[0], assemblyName, element);

                                    _settings.Inspector.GetPropertyOrFieldTypeInfo(
                                        propertyName,
                                        element.Name.NamespaceName,
                                        splittedLocalName[0],
                                        assemblyName,
                                        element,
                                        out propertyNamespaceName,
                                        out propertyLocalTypeName,
                                        out _,
                                        out _,
                                        isAttached: true);

                                    string propertyType = GetFullTypeName(propertyNamespaceName, propertyLocalTypeName);
                                    parameters.AppendLine(
                                        $"{elementType}.Set{propertyName}({parentUid}, ({propertyType})({RuntimeHelperClass}.CallProvideValue({parameters.CurrentXamlContext}, {childUid})));");
                                }
                                else
                                {
                                    _settings.Inspector.GetPropertyOrFieldTypeInfo(
                                        propertyName,
                                        parent.Name.Namespace.NamespaceName,
                                        parent.Name.LocalName,
                                        assemblyName,
                                        element,
                                        out propertyNamespaceName,
                                        out propertyLocalTypeName,
                                        out _,
                                        out _,
                                        isAttached: false);

                                    string propertyType = GetFullTypeName(propertyNamespaceName, propertyLocalTypeName);
                                    parameters.AppendLine(
                                        $"{parentUid}.{propertyName} = ({propertyType}){RuntimeHelperClass}.CallProvideValue({parameters.CurrentXamlContext}, {childUid});");
                                }
                            }
                            else if (child.Name.LocalName == "Binding" || child.Name.LocalName == "MultiBinding")
                            {
                                //------------------------------
                                // {Binding ...} or MultiBinding
                                //------------------------------

                                bool isDependencyProperty =
                                    _settings.Inspector.GetField(
                                        propertyName + "Property",
                                        isAttachedProperty ? elementName.Namespace.NamespaceName : parent.Name.Namespace.NamespaceName,
                                        isAttachedProperty ? elementName.LocalName : parent.Name.LocalName,
                                        _settings.AssemblyName,
                                        isAttachedProperty ? element : parent) != null;

                                string propertyDeclaringTypeName;
                                string propertyTypeNamespace;
                                string propertyTypeName;
                                if (!isAttachedProperty)
                                {
                                    _settings.Inspector.GetPropertyOrFieldInfo(propertyName,
                                        parent.Name.Namespace.NamespaceName,
                                        parent.Name.LocalName,
                                        assemblyName,
                                        element,
                                        out propertyDeclaringTypeName,
                                        out propertyTypeNamespace,
                                        out propertyTypeName);
                                }
                                else
                                {
                                    _settings.Inspector.GetAttachedPropertyGetMethodInfo("Get" + propertyName,
                                        elementName.Namespace.NamespaceName,
                                        elementName.LocalName,
                                        assemblyName,
                                        element,
                                        out propertyDeclaringTypeName,
                                        out propertyTypeNamespace,
                                        out propertyTypeName);
                                }
                                string propertyTypeFullName = (!string.IsNullOrEmpty(propertyTypeNamespace) ? propertyTypeNamespace + "." : "") + propertyTypeName;

                                // Check if the property is of type "Binding/MultiBinding" (or "BindingBase"), in which 
                                // case we should directly assign the value instead of calling "SetBinding"
                                bool isPropertyOfTypeBinding =
                                    propertyTypeFullName == $"global::{KnownNamespaces.SystemWindowsData}.{child.Name.LocalName}" ||
                                    propertyTypeFullName == $"global::{KnownNamespaces.SystemWindowsData}.BindingBase";

                                if (isPropertyOfTypeBinding || !isDependencyProperty)
                                {
                                    parameters.AppendLine($"{parentUid}.{propertyName} = {childUid};");
                                }
                                else
                                {
                                    string dpFullName = $"{propertyDeclaringTypeName}.{propertyName}Property";
                                    parameters.AppendLine(
                                        $"global::{KnownNamespaces.SystemWindowsData}.BindingOperations.SetBinding({parentUid}, {dpFullName}, {childUid});");
                                }
                            }
                            else if (GeneratingCode.IsDynamicResourceExtension(child) || GeneratingCode.IsResponsiveExtension(child))
                            {
                                //-----------------------------------
                                // {DynamicResource} or {Responsive}
                                //-----------------------------------

                                string dependencyPropertyName =
                                    _settings.Inspector.GetField(
                                        propertyName + "Property",
                                        isAttachedProperty ? elementName.Namespace.NamespaceName : parent.Name.Namespace.NamespaceName,
                                        isAttachedProperty ? elementName.LocalName : parent.Name.LocalName,
                                        _settings.AssemblyName,
                                        isAttachedProperty ? element : parent);

                                string propertyDeclaringTypeName;
                                string propertyTypeNamespace;
                                string propertyTypeName;
                                if (!isAttachedProperty)
                                {
                                    _settings.Inspector.GetPropertyOrFieldInfo(propertyName,
                                        parent.Name.Namespace.NamespaceName,
                                        parent.Name.LocalName,
                                        assemblyName,
                                        element,
                                        out propertyDeclaringTypeName,
                                        out propertyTypeNamespace,
                                        out propertyTypeName);
                                }
                                else
                                {
                                    _settings.Inspector.GetAttachedPropertyGetMethodInfo("Get" + propertyName,
                                        elementName.Namespace.NamespaceName,
                                        elementName.LocalName,
                                        assemblyName,
                                        element,
                                        out propertyDeclaringTypeName,
                                        out propertyTypeNamespace,
                                        out propertyTypeName);
                                }

                                if (dependencyPropertyName is null)
                                {
                                    string elementType = _settings.Inspector.GetCSharpEquivalentOfXamlTypeAsString(
                                        elementName.Namespace.NamespaceName,
                                        elementName.LocalName,
                                        assemblyName,
                                        element);

                                    if (elementType == $"global::{KnownNamespaces.SystemWindows}.Setter" && propertyName == "Value")
                                    {
                                        parameters.AppendLine($"{parentUid}.{propertyName} = {childUid};");
                                    }
                                    else
                                    {
                                        throw new XamlParseException(
                                            $"A '{element.Name.LocalName}' cannot be set on the '{propertyName}' property of type '{elementType.Substring("global::".Length)}'. A '{element.Name.LocalName}' can only be set on a DependencyProperty of a DependencyObject, or the Setter.Value property.",
                                            element);
                                    }
                                }
                                else
                                {
                                    string markupValue = GeneratingUniqueNames.GenerateUniqueName("tmp");
                                    string propertyTypeFullName = string.IsNullOrEmpty(propertyTypeNamespace) ?
                                        $"global::{propertyTypeName}" :
                                        $"global::{propertyTypeNamespace}.{propertyTypeName}";

                                    parameters
                                        .AppendLine($"object {markupValue};")
                                        .AppendLine($"if (!{RuntimeHelperClass}.TrySetMarkupExtension({parentUid}, {dependencyPropertyName}, {childUid}, out {markupValue}))")
                                        .AppendLine("{");

                                    if (!isAttachedProperty)
                                    {
                                        parameters.AppendLine($"    {parentUid}.{propertyName} = ({propertyTypeFullName}){markupValue};");
                                    }
                                    else
                                    {
                                        parameters.AppendLine($"    {propertyDeclaringTypeName}.Set{propertyName}({parentUid}, ({propertyTypeFullName}){markupValue});");
                                    }

                                    parameters.AppendLine("}");
                                }
                            }
                            else if (child.Name.LocalName == "TemplateBindingExtension")
                            {
                                var dpName =
                                    _settings.Inspector.GetField(
                                        propertyName + "Property",
                                        isAttachedProperty ? elementName.Namespace.NamespaceName : parent.Name.Namespace.NamespaceName,
                                        isAttachedProperty ? elementName.LocalName : parent.Name.LocalName,
                                        _settings.AssemblyName,
                                        isAttachedProperty ? element : parent);

                                parameters.AppendLine(
                                    $"{parentUid}.SetValue({dpName}, {RuntimeHelperClass}.CallProvideValue({parameters.CurrentXamlContext}, {childUid}));");
                            }
                            else if (GeneratingCode.IsNullExtension(child))
                            {
                                //------------------------------
                                // {x:Null}
                                //------------------------------

                                if (isAttachedProperty)
                                {
                                    string elementType = _settings.Inspector.GetCSharpEquivalentOfXamlTypeAsString(
                                        elementName.Namespace.NamespaceName, elementName.LocalName, assemblyName, element);
                                    parameters.AppendLine($"{elementType}.Set{propertyName}({parentUid}, null);");
                                }
                                else
                                {
                                    parameters.AppendLine($"{parentUid}.{propertyName} = null;");
                                }
                                //todo-perfs: avoid generating the line "var NullExtension_cfb65e0262594ddb87d60d8e776ce142 = new global::System.Windows.Markup.NullExtension();", which is never used. Such a line is generated when the user code contains a {x:Null} markup extension.
                            }
                            else if (GeneratingCode.IsStaticExtension(child))
                            {
                                string staticMemberName = ResolveStaticExtension(child);

                                if (isAttachedProperty)
                                {
                                    _settings.Inspector.GetPropertyOrFieldTypeInfo(
                                        propertyName,
                                        element.Name.NamespaceName,
                                        typeName,
                                        assemblyName,
                                        element,
                                        out string propertyTypeNS,
                                        out string propertyTypeName,
                                        out _,
                                        out _,
                                        isAttached: true);

                                    string type = _settings.Inspector.GetCSharpEquivalentOfXamlTypeAsString(
                                        elementName.Namespace.NamespaceName,
                                        elementName.LocalName,
                                        assemblyName,
                                        element);

                                    parameters.AppendLine(
                                        $"{type}.Set{propertyName}({parentUid}, ({GetFullTypeName(propertyTypeNS, propertyTypeName)})(object){staticMemberName});");
                                }
                                else
                                {
                                    _settings.Inspector.GetPropertyOrFieldTypeInfo(
                                        propertyName,
                                        parent.Name.NamespaceName,
                                        parent.Name.LocalName,
                                        assemblyName,
                                        element,
                                        out string propertyTypeNS,
                                        out string propertyTypeName,
                                        out _,
                                        out _,
                                        isAttached: false);

                                    parameters.AppendLine(
                                        $"{parentUid}.{propertyName} = ({GetFullTypeName(propertyTypeNS, propertyTypeName)})(object){staticMemberName};");
                                }
                            }
                            else if (GeneratingCode.IsTypeExtension(child))
                            {
                                string resolvedTypeName = ResolveTypeExtension(child);

                                if (isAttachedProperty)
                                {
                                    _settings.Inspector.GetPropertyOrFieldTypeInfo(
                                        propertyName,
                                        element.Name.NamespaceName,
                                        typeName,
                                        assemblyName,
                                        element,
                                        out string propertyTypeNS,
                                        out string propertyTypeName,
                                        out _,
                                        out _,
                                        isAttached: true);

                                    string type = _settings.Inspector.GetCSharpEquivalentOfXamlTypeAsString(
                                        elementName.Namespace.NamespaceName,
                                        elementName.LocalName,
                                        assemblyName,
                                        element);

                                    parameters.AppendLine(
                                        $"{type}.Set{propertyName}({parentUid}, ({GetFullTypeName(propertyTypeNS, propertyTypeName)})(object)typeof(global::{resolvedTypeName}));");
                                }
                                else
                                {
                                    _settings.Inspector.GetPropertyOrFieldTypeInfo(
                                        propertyName,
                                        parent.Name.NamespaceName,
                                        parent.Name.LocalName,
                                        assemblyName,
                                        element,
                                        out string propertyTypeNS,
                                        out string propertyTypeName,
                                        out _,
                                        out _,
                                        isAttached: false);

                                    parameters.AppendLine(
                                        $"{parentUid}.{propertyName} = ({GetFullTypeName(propertyTypeNS, propertyTypeName)})(object)typeof(global::{resolvedTypeName});");
                                }
                            }
                            else
                            {
                                //------------------------------
                                // Other (custom MarkupExtensions)
                                //------------------------------

                                string propertyOwnerTypeNS, propertyOwnerTypeName;
                                if (isAttachedProperty)
                                {
                                    propertyOwnerTypeNS = element.Name.NamespaceName;
                                    propertyOwnerTypeName = element.Name.LocalName.Split('.')[0];
                                }
                                else
                                {
                                    propertyOwnerTypeNS = parent.Name.Namespace.NamespaceName;
                                    propertyOwnerTypeName = parent.Name.LocalName;
                                }

                                _settings.Inspector.GetPropertyOrFieldTypeInfo(
                                    propertyName,
                                    propertyOwnerTypeNS,
                                    propertyOwnerTypeName,
                                    assemblyName,
                                    element,
                                    out string propertyTypeNS,
                                    out string propertyTypeName,
                                    out _,
                                    out _,
                                    isAttachedProperty);

                                string dpName = _settings.Inspector.GetField(
                                    propertyName + "Property",
                                    propertyOwnerTypeNS,
                                    propertyOwnerTypeName,
                                    _settings.AssemblyName,
                                    isAttachedProperty ? element : parent);

                                if (dpName != null)
                                {
                                    string markupValue = GeneratingUniqueNames.GenerateUniqueName("tmp");
                                    string propertyTypeFullName = GetFullTypeName(propertyTypeNS, propertyTypeName);

                                    parameters
                                        .AppendLine($"object {markupValue};")
                                        .AppendLine($"if (!{RuntimeHelperClass}.TrySetMarkupExtension({parentUid}, {dpName}, {childUid}, out {markupValue}))")
                                        .AppendLine("{");

                                    if (isAttachedProperty)
                                    {
                                        string elementType = _settings.Inspector.GetCSharpEquivalentOfXamlTypeAsString(
                                            propertyOwnerTypeNS, propertyOwnerTypeName, assemblyName, element);

                                        parameters.AppendLine($"    {elementType}.Set{propertyName}({parentUid}, ({propertyTypeFullName}){markupValue});");
                                    }
                                    else
                                    {
                                        parameters.AppendLine($"    {parentUid}.{propertyName} = ({propertyTypeFullName}){markupValue};");
                                    }

                                    parameters.AppendLine("}");
                                }
                                else
                                {
                                    if (isAttachedProperty)
                                    {
                                        string elementType = _settings.Inspector.GetCSharpEquivalentOfXamlTypeAsString(
                                            propertyOwnerTypeNS, propertyOwnerTypeName, assemblyName, element);

                                        string markupExtension =
                                            $"(({IMarkupExtensionClass}){childUid}).ProvideValue(new global::System.ServiceProvider({parentUid}, null))";

                                        parameters.AppendLine(
                                            $"{elementType}.Set{propertyName}({parentUid}, ({GetFullTypeName(propertyTypeNS, propertyTypeName)}){markupExtension});");
                                    }
                                    else
                                    {
                                        parameters.AppendLine(
                                            $"{parentUid}.{propertyName} = ({GetFullTypeName(propertyTypeNS, propertyTypeName)})(({IMarkupExtensionClass}){childUid}).ProvideValue(new global::System.ServiceProvider({parentUid}, null));");
                                    }
                                }
                            }
                        }
                    }
                }
            }

            private void OnWriteEndMemberCollection(GeneratorContext parameters)
            {
                XElement target = _reader.MemberData.Target;
                string targetUid = GeneratingCode.GetUniqueName(target);

                XElement child = _reader.MemberData.Value;
                string childUid = GeneratingCode.GetUniqueName(child);

                if (IsElementADictionary(target))
                {
                    string childKey = GetElementXKey(child, out bool isImplicitStyle, out bool isImplicitDataTemplate);
                    if (isImplicitStyle)
                    {
                        parameters.AppendLine($"((global::System.Collections.IDictionary){targetUid}).Add(typeof({childKey}), {childUid});");
                    }
                    else if (isImplicitDataTemplate)
                    {
                        string key = $"new global::{KnownNamespaces.SystemWindows}.DataTemplateKey(typeof({childKey}))";

                        parameters.AppendLine($"((global::System.Collections.IDictionary){targetUid}).Add({key}, {childUid});");
                    }
                    else
                    {
                        parameters.AppendLine($"((global::System.Collections.IDictionary){targetUid}).Add(\"{childKey}\", {childUid});");
                    }
                }
                else
                {
                    parameters.AppendLine($"((global::System.Collections.IList){targetUid}).Add({childUid});");
                }
            }

            private void TryCatch(Action<GeneratorContext> method, GeneratorContext parameters)
            {
                try
                {
                    method(parameters);
                }
                catch (XamlParseException ex) when (ex.HasLineInfo())
                {
                    throw;
                }
                catch (Exception ex)
                {
                    IXmlLineInfo info = _reader.ObjectData?.Element ?? _reader.MemberData?.Member;

                    throw new XamlParseException($"An unexpected error occurred: {ex.Message}", info, ex);
                }
            }

            private bool IsElementTheRootElement(XElement element)
            {
                return (element == _reader.Document.Root);
            }

            private bool IsResourceDictionaryCreatedFromSource(XElement element)
            {
                if (element.Attribute("Source") != null)
                {
                    return _settings.Inspector.IsResourceDictionarySourcePropertyVisible(
                        element.Name.NamespaceName, element.Name.LocalName, element);
                }

                return false;
            }

            private string GetCSharpFullTypeNameFromTargetTypeString(XElement styleElement, bool isDataType = false)
            {
                if (styleElement.Attribute(isDataType ? "DataType" : "TargetType") is not XAttribute targetTypeAttribute)
                {
                    throw new XamlParseException(
                        isDataType ? "DataTemplate must declare a DataType or have a key." : "Style must declare a TargetType.",
                        styleElement);
                }

                GettingInformationAboutXamlTypes.GetClrNamespaceAndLocalName(
                    targetTypeAttribute.Value,
                    styleElement,
                    out string namespaceName,
                    out string localTypeName,
                    out string assemblyNameIfAny);
                string elementType = _settings.Inspector.GetCSharpEquivalentOfXamlTypeAsString(
                    namespaceName,
                    localTypeName,
                    assemblyNameIfAny,
                    targetTypeAttribute,
                    ifTypeNotFoundTryGuessing: false);

                return elementType;
            }

            private string GetCSharpFullTypeName(string typeString, XElement elementWhereTheTypeIsUsed, IXmlLineInfo lineInfo)
            {
                GettingInformationAboutXamlTypes.GetClrNamespaceAndLocalName(
                    typeString,
                    elementWhereTheTypeIsUsed,
                    out string namespaceName,
                    out string localTypeName,
                    out string assemblyNameIfAny);
                string elementType = _settings.Inspector.GetCSharpEquivalentOfXamlTypeAsString(
                    namespaceName,
                    localTypeName,
                    assemblyNameIfAny,
                    lineInfo,
                    ifTypeNotFoundTryGuessing: false);

                return elementType;
            }

            private string GetElementXKey(XElement element,
                out bool isImplicitStyle,
                out bool isImplicitDataTemplate)
            {
                isImplicitStyle = false;
                isImplicitDataTemplate = false;

                if (element.Attribute(GeneratingCode.xNamespace + "Key") != null)
                {
                    return element.Attribute(GeneratingCode.xNamespace + "Key").Value;
                }
                else if (element.Attribute(GeneratingCode.xNamespace + "Name") != null)
                {
                    return element.Attribute(GeneratingCode.xNamespace + "Name").Value;
                }
                else if (GeneratingCode.IsStyle(element, _settings.AssemblyName))
                {
                    isImplicitStyle = true;
                    return GetCSharpFullTypeNameFromTargetTypeString(element);
                }
                else if (GeneratingCode.IsDataTemplate(element, _settings.AssemblyName) && element.Attribute("DataType") != null)
                {
                    isImplicitDataTemplate = true;
                    return GetCSharpFullTypeNameFromTargetTypeString(element, isDataType: true);
                }
                else
                {
                    throw new XamlParseException(
                        $"Each dictionary entry must have an associated key. The element named '{element.Name.LocalName}' does not have a key.",
                        element);
                }
            }

            private string GenerateCodeForInstantiatingAttributeValue(
                XName xName,
                string propertyName,
                bool isAttachedProperty,
                string value,
                XElement elementWhereTheTypeIsUsed,
                XObject lineInfo)
            {
                GetClrNamespaceAndLocalName(
                    xName,
                    out string namespaceName,
                    out string localTypeName,
                    out string assemblyNameIfAny);

                string valueNamespaceName, valueLocalTypeName, valueAssemblyName;
                bool isValueEnum, hasTypeConverter;

                if (isAttachedProperty)
                {
                    _settings.Inspector.GetMethodReturnValueTypeInfo(
                        "Get" + propertyName,
                        namespaceName,
                        localTypeName,
                        assemblyNameIfAny,
                        lineInfo,
                        out valueNamespaceName,
                        out valueLocalTypeName,
                        out valueAssemblyName,
                        out isValueEnum);

                    hasTypeConverter = false;
                }
                else
                {
                    _settings.Inspector.GetPropertyOrFieldTypeInfo(
                        propertyName,
                        namespaceName,
                        localTypeName,
                        assemblyNameIfAny,
                        lineInfo,
                        out valueNamespaceName,
                        out valueLocalTypeName,
                        out valueAssemblyName,
                        out isValueEnum,
                        out hasTypeConverter);
                }

                string valueTypeFullName = GetFullTypeName(valueNamespaceName, valueLocalTypeName);
                bool isKnownSystemType = _settings.SystemTypes.IsKnownType(
                        valueTypeFullName.Substring("global::".Length), valueAssemblyName);
                bool isKnownCoreType = _settings.CoreTypes.IsKnownType(
                    valueTypeFullName.Substring("global::".Length), valueAssemblyName);

                // Generate the code or instantiating the attribute
                if (isValueEnum && !isKnownSystemType && !isKnownCoreType)
                {
                    //----------------------------
                    // PROPERTY IS AN ENUM
                    //----------------------------

                    TypeDefinition enumType = _settings.Inspector.GetTypeDefinition(
                        valueNamespaceName,
                        valueLocalTypeName,
                        valueAssemblyName,
                        lineInfo);

                    return string.Join(" | ", _settings.Inspector.GetEnumValues(
                        enumType,
                        value.Trim(),
                        true,
                        true,
                        lineInfo));
                }
                else if (valueTypeFullName == "global::System.Type")
                {
                    string typeFullName = GetCSharpFullTypeName(value, elementWhereTheTypeIsUsed, lineInfo);

                    return $"typeof({typeFullName})";
                }
                else
                {
                    //----------------------------
                    // PROPERTY IS OF ANOTHER TYPE
                    //----------------------------

                    value = ConvertRelativeUri(value, propertyName, valueTypeFullName, namespaceName, localTypeName, assemblyNameIfAny);

                    string preparedValue = ConvertFromInvariantString(
                        value, lineInfo, valueTypeFullName, isKnownCoreType, isKnownSystemType);

                    if (!isAttachedProperty && hasTypeConverter)
                    {
                        string declaringTypeName = _settings.Inspector.GetCSharpEquivalentOfXamlTypeAsString(
                            namespaceName, localTypeName, assemblyNameIfAny, lineInfo);

                        return XamlContextGetPropertyValue(
                            declaringTypeName, propertyName, value, valueTypeFullName, preparedValue);
                    }

                    return preparedValue;
                }
            }

            private string ConvertRelativeUri(
                string value,
                string propertyName,
                string propertyType,
                string declaringTypeNS,
                string declaringTypeName,
                string declaringTypeAssembly)
            {
                if (GeneratingCode.IsUriMapping(declaringTypeNS, declaringTypeName, declaringTypeAssembly, _settings.AssemblyName) ||
                    GeneratingCode.IsFrame(declaringTypeNS, declaringTypeName, declaringTypeAssembly, _settings.AssemblyName) ||
                    GeneratingCode.IsHyperlinkButton(declaringTypeNS, declaringTypeName, declaringTypeAssembly, _settings.AssemblyName) ||
                    GeneratingCode.IsHyperlink(declaringTypeNS, declaringTypeName, declaringTypeAssembly, _settings.AssemblyName))
                {
                    return value;
                }

                if (GeneratingCode.IsUriAbsolute(value))
                {
                    return value;
                }

                if (GeneratingCode.IsComponentUri(value))
                {
                    return value;
                }

                if (GeneratingCode.IsApplicationStartupUriProperty(propertyName, declaringTypeNS, declaringTypeName, declaringTypeAssembly, _settings.AssemblyName))
                {
                    return CreateComponentUri(value);
                }

                if (value.EndsWith(".xaml", StringComparison.OrdinalIgnoreCase))
                {
                    return value;
                }

                if (propertyType != "global::System.Uri" &&
                    propertyType != $"global::{KnownNamespaces.SystemWindowsMedia}.ImageSource" &&
                    (propertyName != "FontFamily" || !value.Contains(".")))
                {
                    return value;
                }

                return CreateComponentUri(value);
            }

            private string CreateComponentUri(string relativePath)
            {
                // Get the relative path of the current XAML file:
                string relativePathOfTheCurrentFile = Path.GetDirectoryName(
                    _fileNameWithPathRelativeToProjectRoot.Replace('\\', '/'));

                // Combine the relative path of the current file with the path specified by the user:
                string pathRelativeToProjectRoot = Path.Combine(
                    relativePathOfTheCurrentFile.Replace('\\', '/'),
                    relativePath.Replace('\\', '/')).Replace('\\', '/');

                return $"/{_settings.AssemblyName};component/{pathRelativeToProjectRoot}";
            }

            private bool TryResolvePathForBinding(string path, XElement element, IXmlLineInfo lineInfo, out string resolvedPath)
            {
                if (path == "" || path == ".")
                {
                    resolvedPath = path;
                    return true;
                }

                resolvedPath = null;
                StringBuilder sb = new StringBuilder();

                int pos = 0;
                char c;

                while (true)
                {
                    while (pos < path.Length)
                    {
                        c = path[pos];

                        if (c == '(')
                        {
                            break;
                        }

                        sb.Append(c);
                        pos++;
                    }

                    if (pos == path.Length)
                    {
                        resolvedPath = sb.ToString();
                        return true;
                    }

                    pos++;
                    if (pos == path.Length)
                    {
                        return false;
                    }

                    while (pos < path.Length)
                    {
                        c = path[pos];
                        if (!char.IsWhiteSpace(c))
                        {
                            break;
                        }

                        pos++;
                    }

                    if (pos == path.Length)
                    {
                        return false;
                    }

                    int start = pos;
                    string xmlPrefix = null;
                    string typeName = null;
                    string propertyName = null;
                    while (pos < path.Length)
                    {
                        c = path[pos];
                        if (c == ':')
                        {
                            xmlPrefix = path.Substring(start, pos - start);
                            start = pos + 1;
                        }
                        else if (c == '.')
                        {
                            typeName = path.Substring(start, pos - start);
                            start = pos + 1;
                        }
                        else if (c == ')')
                        {
                            propertyName = path.Substring(start, pos - start);
                            break;
                        }

                        pos++;
                    }

                    if (pos == path.Length)
                    {
                        return false;
                    }

                    if (string.IsNullOrWhiteSpace(typeName) || string.IsNullOrWhiteSpace(propertyName))
                    {
                        return false;
                    }

                    XNamespace xmlNamespace = xmlPrefix == null ? element.GetDefaultNamespace() : element.GetNamespaceOfPrefix(xmlPrefix);
                    GetClrNamespaceAndLocalName(
                        XName.Get(typeName, xmlNamespace.NamespaceName),
                        out string namespaceName,
                        out string localTypeName,
                        out string assemblyName);

                    string assemblyQualifiedName = _settings.Inspector.GetAssemblyQualifiedNameOfXamlType(
                        namespaceName, localTypeName, assemblyName, lineInfo);

                    if (assemblyQualifiedName == null)
                    {
                        return false;
                    }

                    sb.Append('(')
                      .Append(assemblyQualifiedName)
                      .Append('.')
                      .Append(propertyName)
                      .Append(')');

                    pos++;
                }
            }

            private void ResolvePathForTemplateBinding(string path, XElement element, out string typeName, out string propertyName)
            {
                typeName = null;
                propertyName = path;

                int idx1 = path.IndexOf('.');
                if (idx1 > 0 && idx1 < path.Length - 1)
                {
                    string xmlPrefix, type;
                    propertyName = path.Substring(idx1 + 1);

                    int idx2 = path.IndexOf(':', 0, idx1);
                    if (idx2 > -1)
                    {
                        xmlPrefix = path.Substring(0, idx2);
                        type = path.Substring(idx2 + 1, idx1 - idx2 - 1);
                    }
                    else
                    {
                        xmlPrefix = null;
                        type = path.Substring(0, idx1);
                    }

                    XNamespace xmlNamespace = xmlPrefix == null ? element.GetDefaultNamespace() : element.GetNamespaceOfPrefix(xmlPrefix);
                    try
                    {
                        typeName = GetCSharpEquivalentOfXamlTypeAsString(XName.Get(type, xmlNamespace.NamespaceName), element);
                    }
                    catch { }
                }
            }

            private string ConvertFromInvariantString(string value, XObject context, string type, bool isKnownCoreType, bool isKnownSystemType)
            {
                type = type.Substring("global::".Length);

                if (_settings.SystemTypes.IsNullableType(type, null, out string underlyingType))
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        return "null";
                    }

                    type = underlyingType;
                    isKnownCoreType = _settings.CoreTypes.IsKnownType(type, null);
                    isKnownSystemType = _settings.SystemTypes.IsKnownType(type, null);
                }

                if (isKnownCoreType)
                {
                    return _settings.CoreTypes.ConvertKnownType(value, type, context);
                }
                else if (isKnownSystemType)
                {
                    return _settings.SystemTypes.ConvertKnownType(value, type);
                }

                return _settings.CoreTypes.ConvertFromInvariantString(value, type);
            }

            private string XamlContextGetPropertyValue(
                string propertyDeclaringType,
                string propertyName,
                string value,
                string propertyType,
                string fallbackValue)
            {
                return $"{RuntimeHelperClass}.GetPropertyValue<{propertyType}>(typeof({propertyDeclaringType}), {EscapeString(propertyName)}, {EscapeString(value)}, () => {fallbackValue})";
            }

            private static bool IsReservedAttribute(string attributeName)
            {
                return attributeName == GeneratingUniqueNames.UniqueNameAttribute ||
                       attributeName == InsertingImplicitNodes.InitializedFromStringAttribute ||
                       attributeName == InsertingMarkupNodesInXaml.GeneratedMarkupExtensionAttribute ||
                       attributeName == GeneratingPathInXaml.PathInXamlAttribute;
            }

            private static string EscapeString(string stringValue)
            {
                return string.Concat("@\"", stringValue.Replace("\"", "\"\""), "\"");
            }

            private bool IsPropertyAttached(XElement propertyElement)
            {
                GetClrNamespaceAndLocalName(propertyElement.Name, out string namespaceName, out string localName, out string assemblyName);
                if (localName.Contains("."))
                {
                    var split = localName.Split('.');
                    var typeName = split[0];
                    var propertyOrFieldName = split[1];
                    return _settings.Inspector.IsPropertyAttached(
                        propertyOrFieldName, namespaceName, typeName, assemblyName, propertyElement);
                }

                return false;
            }

            private bool IsPropertyOrFieldACollection(XElement propertyElement, bool isAttachedProperty)
            {
                if (isAttachedProperty)
                {
                    string methodName = "Get" + propertyElement.Name.LocalName.Split('.')[1]; // In case of attached property, we check the return type of the method "GetPROPERTYNAME()". For example, in case of "Grid.Row", we check the return type of the method "Grid.GetRow()".
                    XName elementName = propertyElement.Name.Namespace + propertyElement.Name.LocalName.Split('.')[0]; // eg. if the propertyElement is <VisualStateManager.VisualStateGroups>, this will be "DefaultNamespace+VisualStateManager"
                    GetClrNamespaceAndLocalName(elementName, out string namespaceName, out string localName, out string assemblyNameIfAny);
                    return _settings.Inspector.DoesMethodReturnACollection(
                        methodName, namespaceName, localName, assemblyNameIfAny, propertyElement);
                }
                else
                {
                    var propertyOrFieldName = propertyElement.Name.LocalName.Split('.')[1];
                    var parentElement = propertyElement.Parent;
                    GetClrNamespaceAndLocalName(parentElement.Name, out string parentNamespaceName, out string parentLocalName, out string parentAssemblyNameIfAny);
                    return _settings.Inspector.IsPropertyOrFieldACollection(
                        propertyOrFieldName, parentNamespaceName, parentLocalName, parentAssemblyNameIfAny, propertyElement);
                }
            }

            private bool IsPropertyACollection(XElement element, bool isAttachedProperty)
            {
                if (!IsPropertyOrFieldACollection(element, isAttachedProperty))
                {
                    return false;
                }

                if (element.Elements().Count() != 1)
                {
                    return true;
                }

                XElement child = element.Elements().First();

                return !IsTypeAssignableFrom(child, element, isAttachedProperty) &&
                    !GeneratingCode.IsBinding(child, _settings.AssemblyName) &&
                    child.Name.LocalName != "StaticResource" &&
                    child.Name.LocalName != "StaticResourceExtension" &&
                    child.Name.LocalName != "TemplateBinding" &&
                    child.Name.LocalName != "TemplateBindingExtension" &&
                    child.Name.LocalName != "DynamicResource" &&
                    child.Name.LocalName != "DynamicResourceExtension";
            }

            private bool IsPropertyOrFieldADictionary(
                XElement propertyElement,
                bool isAttachedProperty)
            {
                if (isAttachedProperty)
                {
                    string methodName = "Get" + propertyElement.Name.LocalName.Split('.')[1]; // In case of attached property, we check the return type of the method "GetPROPERTYNAME()". For example, in case of "Grid.Row", we check the return type of the method "Grid.GetRow()".
                    XName elementName = propertyElement.Name.Namespace + propertyElement.Name.LocalName.Split('.')[0]; // eg. if the propertyElement is <VisualStateManager.VisualStateGroups>, this will be "DefaultNamespace+VisualStateManager"
                    GetClrNamespaceAndLocalName(elementName, out string namespaceName, out string localName, out string assemblyNameIfAny);
                    return _settings.Inspector.DoesMethodReturnADictionary(
                        methodName, namespaceName, localName, assemblyNameIfAny, propertyElement);
                }
                else
                {
                    var propertyOrFieldName = propertyElement.Name.LocalName.Split('.')[1];
                    var parentElement = propertyElement.Parent;
                    GetClrNamespaceAndLocalName(parentElement.Name, out string parentNamespaceName, out string parentLocalName, out string parentAssemblyNameIfAny);
                    return _settings.Inspector.IsPropertyOrFieldADictionary(
                        propertyOrFieldName, parentNamespaceName, parentLocalName, parentAssemblyNameIfAny, propertyElement);
                }
            }

            private bool IsElementADictionary(XElement element)
            {
                GetClrNamespaceAndLocalName(element.Name, out string elementNameSpace, out string elementLocalName, out string assemblyNameIfAny);
                return _settings.Inspector.IsElementADictionary(elementNameSpace, elementLocalName, assemblyNameIfAny, element);
            }

            private bool IsElementAMarkupExtension(XElement element)
            {
                GetClrNamespaceAndLocalName(element.Name, out string elementNameSpace, out string elementLocalName, out string assemblyNameIfAny);
                return _settings.Inspector.IsElementAMarkupExtension(elementNameSpace, elementLocalName, assemblyNameIfAny, element);
            }

            private bool IsTypeAssignableFrom(XElement from, XElement to, bool isAttached = false)
            {
                GetClrNamespaceAndLocalName(from.Name,
                    out string nameSpaceOfTypeToAssignFrom, out string nameOfTypeToAssignFrom, out string assemblyNameOfTypeToAssignFrom);
                GetClrNamespaceAndLocalName(to.Name,
                    out string nameSpaceOfTypeToAssignTo, out string nameOfTypeToAssignTo, out string assemblyNameOfTypeToAssignTo);

                return _settings.Inspector.IsTypeAssignableFrom(
                    nameSpaceOfTypeToAssignFrom, nameOfTypeToAssignFrom, assemblyNameOfTypeToAssignFrom,
                    nameSpaceOfTypeToAssignTo, nameOfTypeToAssignTo, assemblyNameOfTypeToAssignTo,
                    to, isAttached);
            }

            private static void GetClrNamespaceAndLocalName(XName xName, out string namespaceName, out string localName, out string assemblyNameIfAny)
                => GettingInformationAboutXamlTypes.GetClrNamespaceAndLocalName(
                    xName,
                    out namespaceName,
                    out localName,
                    out assemblyNameIfAny);

            private string GetCSharpEquivalentOfXamlTypeAsString(
                XName xName,
                IXmlLineInfo lineInfo,
                bool ifTypeNotFoundTryGuessing,
                out string namespaceName,
                out string typeName,
                out string assemblyName)
            {
                GettingInformationAboutXamlTypes.GetClrNamespaceAndLocalName(
                    xName,
                    out namespaceName,
                    out typeName,
                    out assemblyName);

                return _settings.Inspector.GetCSharpEquivalentOfXamlTypeAsString(
                    namespaceName,
                    typeName,
                    assemblyName,
                    lineInfo,
                    ifTypeNotFoundTryGuessing);
            }

            private string GetCSharpEquivalentOfXamlTypeAsString(
                XElement element,
                out string namespaceName,
                out string typeName,
                out string assemblyName)
                => GetCSharpEquivalentOfXamlTypeAsString(element.Name, element, false, out namespaceName, out typeName, out assemblyName);

            private string GetCSharpEquivalentOfXamlTypeAsString(XName xName, IXmlLineInfo lineInfo, bool ifTypeNotFoundTryGuessing = false)
                => GetCSharpEquivalentOfXamlTypeAsString(xName, lineInfo, ifTypeNotFoundTryGuessing, out _, out _, out _);

            private string ResolveStaticExtension(XElement element)
            {
                if (element.Attribute("Member") is not XAttribute member)
                {
                    throw new XamlParseException("StaticExtension must have Member property set.", element);
                }

                string fieldString;
                string typeNameForError = null;
                TypeDefinition type;

                if (element.Attribute("MemberType") is XAttribute typeAttribute)
                {
                    type = GetTypeDefinitionFromString(element, typeAttribute.Value, typeAttribute);
                    fieldString = member.Value;
                    typeNameForError = _settings.TypeReferenceHelper.ConvertToString(type);
                }
                else
                {
                    int dotIndex = member.Value.IndexOf('.');
                    if (dotIndex < 0)
                    {
                        throw new XamlParseException(
                            $"'{member.Value}' StaticExtension value cannot be resolved to an enumeration, static field, or static property.",
                            member);
                    }

                    // Pull out the type substring (this will include any XML prefix, e.g. "av:Button")
                    string typeString = member.Value.Substring(0, dotIndex);
                    if (string.IsNullOrEmpty(typeString))
                    {
                        throw new XamlParseException(
                            $"'{member.Value}' StaticExtension value cannot be resolved to an enumeration, static field, or static property.",
                            member);
                    }

                    type = GetTypeDefinitionFromString(element, typeString, member);

                    // Get the member name substring.
                    fieldString = member.Value.Substring(dotIndex + 1, member.Value.Length - dotIndex - 1);
                    if (string.IsNullOrEmpty(typeString))
                    {
                        throw new XamlParseException(
                            $"'{member.Value}' StaticExtension value cannot be resolved to an enumeration, static field, or static property.",
                            member);
                    }
                }

                if (type.IsEnum)
                {
                    return _settings.Inspector.GetEnumValue(type, fieldString, false, false);
                }

                FieldDefinition staticField;
                PropertyDefinition staticProperty;
                TypeReference declaringType;

                (staticField, declaringType) = _settings.Inspector.GetField(type, fieldString, true, true);

                if (staticField is not null)
                {
                    return $"global::{_settings.TypeReferenceHelper.ConvertToString(declaringType)}.{staticField.Name}";
                }

                (staticProperty, declaringType) = _settings.Inspector.GetProperty(type, fieldString, true, true);

                if (staticProperty is not null)
                {
                    return $"global::{_settings.TypeReferenceHelper.ConvertToString(declaringType)}.{staticProperty.Name}";
                }

                throw new XamlParseException(
                    $"'{(typeNameForError is not null ? $"{typeNameForError}.{member.Value}" : member.Value)}' StaticExtension value cannot be resolved to an enumeration, static field, or static property.",
                    member);
            }

            private string ResolveTypeExtension(XElement element)
            {
                if (element.Attribute("Type") is XAttribute typeAttribute)
                {
                    return _settings.TypeReferenceHelper.ConvertToString(GetTypeDefinitionFromString(element, typeAttribute.Value, typeAttribute));
                }

                if (element.Attribute("TypeName") is not XAttribute typeNameAttribute)
                {
                    throw new XamlParseException("TypeExtension must have TypeName property set.", element);
                }

                return _settings.TypeReferenceHelper.ConvertToString(GetTypeDefinitionFromString(element, typeNameAttribute.Value, typeNameAttribute));
            }

            private TypeDefinition GetTypeDefinitionFromString(XElement element, string value, IXmlLineInfo lineInfo)
            {
                Debug.Assert(value is not null);

                GettingInformationAboutXamlTypes.GetClrNamespaceAndLocalName(
                    value, element, out string namespaceName, out string typeName, out string assemblyName);

                return _settings.Inspector.GetTypeDefinition(namespaceName, typeName, assemblyName, lineInfo);
            }

            private string GenerateCodeForSetterValue(XAttribute value) => GenerateCodeForSetterOrTriggerOrConditionValue(value, "Setter", "TargetName");

            private string GenerateCodeForTriggerValue(XAttribute value) => GenerateCodeForSetterOrTriggerOrConditionValue(value, "Trigger", "SourceName");

            private string GenerateCodeForConditionValue(XAttribute value) => GenerateCodeForSetterOrTriggerOrConditionValue(value, "Condition", "SourceName");

            private string GenerateCodeForSetterOrTriggerOrConditionValue(XAttribute value, string typeName, string targetPropertyName)
            {
                XElement element = value.Parent;

                if (element.Attribute("Property") is not XAttribute property)
                {
                    // For Condition, it is possible to have a Binding instead of a Property for DataTrigger and MultiDataTrigger.
                    // In this case we don't convert the value and just return a string instead.
                    if (typeName == "Condition")
                    {
                        return _settings.SystemTypes.ConvertToString(GeneratingCode.GetAttributeValue(value));
                    }

                    throw new XamlParseException($"The '<{typeName} />' element must declare a 'Property' attribute.", element);
                }

                (TypeDefinition declaringType, string propertyName) =
                    SetterTriggerConditionHelpers.GetSetterOrTriggerOrConditionProperty(property, targetPropertyName, _settings);

                if (declaringType is null)
                {
                    throw new XamlParseException($"Unable to identify the declaring type of the {typeName}'s property.", property);
                }

                return GenerateCodeForInstantiatingAttributeValue(
                    XName.Get(declaringType.Name, declaringType.Namespace),
                    propertyName,
                    property.Value.Contains('.'),
                    GeneratingCode.GetAttributeValue(value),
                    element,
                    property);
            }

            private string GenerateCodeForSetterProperty(XAttribute property) => GenerateCodeForSetterOrTriggerOrConditionProperty(property, "Setter", "TargetName");

            private string GenerateCodeForTriggerProperty(XAttribute property) => GenerateCodeForSetterOrTriggerOrConditionProperty(property, "Trigger", "SourceName");

            private string GenerateCodeForConditionProperty(XAttribute property) => GenerateCodeForSetterOrTriggerOrConditionProperty(property, "Condition", "SourceName");

            private string GenerateCodeForSetterOrTriggerOrConditionProperty(XAttribute property, string typeName, string targetPropertyName)
            {
                (TypeDefinition declaringType, string propertyName) =
                    SetterTriggerConditionHelpers.GetSetterOrTriggerOrConditionProperty(property, targetPropertyName, _settings);

                if (declaringType is null)
                {
                    throw new XamlParseException($"Unable to identify the declaring type of the {typeName}'s property.", property);
                }

                return $"{_settings.TypeReferenceHelper.ConvertToString(declaringType)}.{propertyName}Property";
            }
        }
    }
}
