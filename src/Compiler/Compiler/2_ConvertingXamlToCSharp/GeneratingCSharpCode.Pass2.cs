
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
using System.IO;
using System.Linq;
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

                    builder.AppendLine($"private static global::{ObjectType} {MethodName}({XamlContextClass} {XamlContext})")
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
                var parameters = new GeneratorContext(_sourceFile);

                TypeDefinition type = GetTypeDefinition(_reader.Document.Root.Name, _reader.Document.Root);

                parameters.GenerateFieldsForNamedElements =
                    !_settings.Inspector.IsResourceDictionary(type) &&
                    !_settings.Inspector.IsApplication(type);

                parameters.PushScope(
                    new RootScope(
                        GeneratingCode.GetUniqueName(_reader.Document.Root),
                        _settings.Inspector.IsIFrameworkElement(type)));

                // Traverse the tree in "post order" (ie. start with child elements then traverse parent elements):
                while (_reader.Read())
                {
                    switch (_reader.NodeType)
                    {
                        case XamlNodeType.StartObject:
                            if (ShouldSkipObject(_reader.ObjectData.Element))
                            {
                                _reader.SkipObject();
                                continue;
                            }

                            TryCatch(OnWriteStartObject, parameters);
                            break;

                        case XamlNodeType.EndObject:
                            TryCatch(OnWriteEndObject, parameters);
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

                string baseType = $"global::{_settings.TypeReferenceHelper.ConvertToString(type)}";

                if (hasCodeBehind)
                {
                    string connectMethod = parameters.ComponentConnector.ToString();
                    string initializeComponentMethod = CreateInitializeComponentMethod(
                        $"global::{KnownNamespaces.SystemWindows}.Application",
                        _settings.AssemblyName,
                        _fileNameWithPathRelativeToProjectRoot);

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
                        $"""
                        var component = ({componentTypeFullName})global::System.Activator.CreateInstance(typeof({componentTypeFullName}), true);
                        component.InitializeComponent();
                        return component;
                        """,
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
                        $"""
                        var {rootElementName} = new {baseType}();
                        LoadComponentImpl({rootElementName});
                        return {rootElementName};
                        """,
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
                    if (GeneratingCode.IsNullExtension(element, _settings) ||
                        GeneratingCode.IsStaticExtension(element, _settings) ||
                        GeneratingCode.IsTypeExtension(element, _settings))
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
                TypeDefinition elementTypeDefinition = GetTypeDefinition(element.Name, element);
                string elementType = _settings.TypeReferenceHelper.ConvertToString(elementTypeDefinition);
                string assemblyName = elementTypeDefinition.GetAssemblyName();

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
                if (elementType == $"{KnownNamespaces.SystemWindows}.EventSetter")
                {
                    WriteEventSetter(parameters);

                    // EventSetter only support the Event, Handler and HandledEventsToo properties. WriteEventSetter
                    // already takes care of these properties, so we just skip everything.
                    _reader.SkipObject();
                    OnWriteEndObject(parameters);
                    return;
                }

                if (IsElementTheRootElement(element))
                {
                    parameters.AppendLine($"_ = {RuntimeHelperClass}.XamlContext_WriteStartObject({parameters.CurrentXamlContext}, {elementUid});");
                }
                else
                {
                    if (element.Attribute(InsertingImplicitNodes.InitializedFromStringAttribute) is XAttribute initFromStringAttribute)
                    {
                        string stringValue = initFromStringAttribute.Value;

                        bool isKnownSystemType = _settings.SystemTypes.IsKnownType(elementType, assemblyName);
                        bool isKnownCoreType = _settings.CoreTypes.IsKnownType(elementType, assemblyName);
                        string preparedValue = ConvertFromInvariantString(stringValue, element, elementType, isKnownCoreType, isKnownSystemType);

                        using (parameters.CreateLineScope(element))
                        {
                            parameters.AppendLine(
                                $"var {elementUid} = {RuntimeHelperClass}.XamlContext_WriteStartObject({parameters.CurrentXamlContext}, {preparedValue});");
                        }
                    }
                    else
                    {
                        string value = _settings.SystemTypes.IsKnownType(elementType, assemblyName) ?
                            _settings.SystemTypes.GetDefaultValue(elementType) :
                            $"new global::{elementType}()";

                        using (parameters.CreateLineScope(element))
                        {
                            parameters.AppendLine(
                                $"var {elementUid} = {RuntimeHelperClass}.XamlContext_WriteStartObject({parameters.CurrentXamlContext}, {value});");
                        }

                        if (element.Attribute("Source") is XAttribute sourceAttribute)
                        {
                            (PropertyDefinition sourceProperty, TypeReference declaringType) =
                                _settings.Inspector.GetProperty(elementTypeDefinition, "Source", MemberFlags.Public | MemberFlags.Instance);

                            if (sourceProperty is not null &&
                                declaringType.Name == "ResourceDictionary" &&
                                declaringType.Namespace == KnownNamespaces.SystemWindows &&
                                declaringType.GetAssemblyName() == Constants.OPENSILVER_ASSEMBLY_NAME)
                            {
                                //------------------------------------------------
                                // Add the type initialization from "Source" URI:
                                //------------------------------------------------
                                string absoluteSourceUri = PathsHelper.ConvertToAbsolutePathWithComponentSyntax(
                                    sourceAttribute.Value,
                                    _fileNameWithPathRelativeToProjectRoot,
                                    _settings.AssemblyName);
                                string loadTypeFullName = XamlResourcesHelper.GenerateClassNameFromComponentUri(absoluteSourceUri);

                                parameters.AppendLine(
                                    $"(({IXamlComponentLoaderClass})new global::{loadTypeFullName}()).LoadComponent({elementUid});");
                            }
                        }
                    }
                }

                // Set templated parent if any
                if (parameters.IsInsideTemplate && _settings.Inspector.IsIFrameworkElement(elementTypeDefinition))
                {
                    parameters.AppendLine(
                        $"{RuntimeHelperClass}.XamlContext_SetTemplatedParent({parameters.CurrentXamlContext}, {elementUid});");
                }

                if (_settings.Inspector.IsIUIElement(elementTypeDefinition))
                {
                    string xamlPath = element.Attribute(GeneratingPathInXaml.PathInXamlAttribute)?.Value ?? string.Empty;
                    parameters.AppendLine($"{XamlDesignerBridgeClass}.SetPathInXaml({elementUid}, \"{xamlPath}\");");
                    parameters.AppendLine($"{XamlDesignerBridgeClass}.SetFilePath({elementUid}, @\"{_sourceFile}\");");
                }

                var deferredInitializers = new List<Action>();

                foreach (XAttribute attribute in element.Attributes())
                {
                    //-------------
                    // ATTRIBUTE
                    //-------------

                    //
                    // IMPORTANT:
                    // We need to check for x:Name first, because GeneratingCode.SkipAttribute will skip it otherwise
                    //
                    bool isXNameAttr = GeneratingCode.IsXNameAttribute(attribute);
                    if (isXNameAttr || GeneratingCode.IsNameAttribute(attribute))
                    {
                        //-------------
                        // x:Name (or "Name")
                        //-------------

                        string name = GeneratingCode.GetAttributeValue(attribute);

                        // Add the code to register the name, etc.
                        if (!parameters.IsInsideTemplate && parameters.GenerateFieldsForNamedElements)
                        {
                            string fieldModifier = "internal";
                            XAttribute attr = element.Attribute(GeneratingCode.XFieldModifierAttribute);
                            if (attr != null)
                            {
                                fieldModifier = (attr.Value ?? "").ToLower();
                            }

                            // add '@' to handle cases where x:Name is a forbidden word (for instance 'this'
                            // or any other c# keyword)
                            string fieldName = "@" + name;
                            parameters.ResultingFieldsForNamedElements.Add($"{fieldModifier} global::{elementType} {fieldName};");

                            int componentId = parameters.ComponentConnector.ConnectNamedElement(elementType, fieldName);

                            parameters.AppendLine(
                                $"{RuntimeHelperClass}.XamlContext_SetConnectionId({parameters.CurrentXamlContext}, {componentId}, {elementUid});");
                        }

                        if (isXNameAttr)
                        {
                            if (_settings.Inspector.IsDependencyObject(elementTypeDefinition))
                            {
                                parameters.AppendLine(
                                    $"{elementUid}.SetValue(global::{KnownNamespaces.SystemWindows}.FrameworkElement.NameProperty, \"{name}\");");
                            }
                        }

                        parameters.CurrentScope.RegisterName(name, elementUid);
                    }

                    if (GeneratingCode.SkipAttribute(attribute))
                    {
                        continue;
                    }

                    string attributeValue = GeneratingCode.GetAttributeValue(attribute);

                    string memberName;
                    TypeDefinition fromType;
                    bool attachedMemberOnly;
                    MemberKind memberLookupFlags;

                    int index = attribute.Name.LocalName.IndexOf('.');
                    if (index == -1)
                    {
                        memberName = attribute.Name.LocalName;
                        fromType = elementTypeDefinition;
                        attachedMemberOnly = false;
                        memberLookupFlags = MemberKind.Property |
                                            MemberKind.AttachedPropertySet |
                                            MemberKind.Event |
                                            MemberKind.AttachedEvent |
                                            MemberKind.Field;
                    }
                    else
                    {
                        memberName = attribute.Name.LocalName.Substring(index + 1);
                        string typeName = attribute.Name.LocalName.Substring(0, index);

                        XNamespace xmlns = attribute.Name.Namespace == XNamespace.None ?
                            element.GetDefaultNamespace() :
                            attribute.Name.Namespace;

                        fromType = GetTypeDefinition(xmlns.GetName(typeName), attribute);

                        if (fromType.IsAssignableFrom(elementTypeDefinition))
                        {
                            attachedMemberOnly = false;
                            memberLookupFlags = MemberKind.Property |
                                                MemberKind.AttachedPropertySet |
                                                MemberKind.Event |
                                                MemberKind.AttachedEvent |
                                                MemberKind.Field;
                        }
                        else
                        {
                            attachedMemberOnly = true;
                            memberLookupFlags = MemberKind.AttachedPropertySet |
                                                MemberKind.AttachedEvent;
                        }
                    }

                    MemberReference memberReference = _settings.Inspector.GetMemberFromType(
                        memberName,
                        fromType,
                        memberLookupFlags,
                        out TypeReference declaringType,
                        out TypeReference memberType,
                        out MemberKind memberKind);

                    switch (memberKind)
                    {
                        case MemberKind.Property:
                        case MemberKind.Field:
                            {
                                deferredInitializers.Add(() =>
                                {
                                    string value = null;

                                    if (elementType == $"{KnownNamespaces.SystemWindows}.Setter")
                                    {
                                        value = memberName switch
                                        {
                                            "Property" => GenerateCodeForSetterProperty(attribute),
                                            "Value" => GenerateCodeForSetterValue(attribute),
                                            "TargetName" => _settings.SystemTypes.ConvertToString(attributeValue),
                                            _ => throw new XamlParseException(
                                                "The '<Setter />' element cannot have attributes other than 'Property', 'Value' and 'TargetName'.",
                                                element),
                                        };
                                    }
                                    else if (elementType == $"{KnownNamespaces.SystemWindows}.Trigger")
                                    {
                                        value = memberName switch
                                        {
                                            "Property" => GenerateCodeForTriggerProperty(attribute),
                                            "Value" => GenerateCodeForTriggerValue(attribute),
                                            "SourceName" => _settings.SystemTypes.ConvertToString(attributeValue),
                                            _ => throw new XamlParseException(
                                                "The '<Trigger />' element cannot have attributes other than 'Property', 'Value' and 'SourceName'.",
                                                element),
                                        };
                                    }
                                    else if (elementType == $"{KnownNamespaces.SystemWindows}.Condition")
                                    {
                                        value = memberName switch
                                        {
                                            "Property" => GenerateCodeForConditionProperty(attribute),
                                            "Value" => GenerateCodeForConditionValue(attribute),
                                            "SourceName" => _settings.SystemTypes.ConvertToString(attributeValue),
                                            _ => throw new XamlParseException(
                                                "The '<Condition />' element cannot have attributes other than 'Property', 'Value' and 'SourceName'.",
                                                element),
                                        };
                                    }
                                    else if (elementType == $"{KnownNamespaces.SystemWindowsData}.Binding" && memberName == "Path")
                                    {
                                        if (TryResolvePathForBinding(attributeValue, element, attribute, out string resolvedPath))
                                        {
                                            string xamlPath = _settings.SystemTypes.ConvertToString(resolvedPath);
                                            parameters.AppendLine($"{elementUid}.XamlPath = {xamlPath};");
                                        }

                                        value = GenerateCodeForInstantiatingAttributeValue(
                                            memberName,
                                            memberReference,
                                            declaringType,
                                            memberType,
                                            elementTypeDefinition,
                                            attributeValue,
                                            element,
                                            attribute);
                                    }
                                    else if (elementType == $"{KnownNamespaces.SystemWindows}.TemplateBindingExtension" && memberName == "Path")
                                    {
                                        ResolvePathForTemplateBinding(attributeValue, element, out TypeDefinition ownerType, out string propertyName);
                                        parameters.AppendLine(
                                            $"{elementUid}.DependencyPropertyName = {_settings.SystemTypes.ConvertToString(propertyName)};");

                                        if (ownerType is not null)
                                        {
                                            parameters.AppendLine(
                                                $"{elementUid}.DependencyPropertyOwnerType = typeof(global::{_settings.TypeReferenceHelper.ConvertToString(ownerType)});");
                                        }

                                        value = null;
                                    }
                                    else
                                    {
                                        value = GenerateCodeForInstantiatingAttributeValue(
                                            memberName,
                                            memberReference,
                                            declaringType,
                                            memberType,
                                            elementTypeDefinition,
                                            attributeValue,
                                            element,
                                            attribute);
                                    }

                                    if (value is not null)
                                    {
                                        using (parameters.CreateLineScope(attribute))
                                        {
                                            parameters.AppendLine(
                                                $"((global::{_settings.TypeReferenceHelper.ConvertToString(declaringType)}){elementUid}).{memberName} = {value};");
                                        }
                                    }
                                });

                            }
                            break;

                        case MemberKind.AttachedPropertySet:
                            {
                                deferredInitializers.Add(() =>
                                {
                                    string ownerType = _settings.TypeReferenceHelper.ConvertToString(declaringType);
                                    string value = GenerateCodeForInstantiatingAttributeValue(
                                        memberName,
                                        memberReference,
                                        declaringType,
                                        memberType,
                                        elementTypeDefinition,
                                        attributeValue,
                                        element,
                                        attribute);

                                    using (parameters.CreateLineScope(attribute))
                                    {
                                        parameters.AppendLine(
                                            $"global::{ownerType}.Set{memberName}({elementUid}, {value});");
                                    }
                                });
                            }
                            break;

                        case MemberKind.Event:
                            {
                                int componentId = parameters.ComponentConnector.ConnectEventHandler(elementType, memberName, attributeValue);
                                parameters.AppendLine(
                                    $"{RuntimeHelperClass}.XamlContext_SetConnectionId({parameters.CurrentXamlContext}, {componentId}, {elementUid});");
                            }
                            break;

                        case MemberKind.AttachedEvent:
                            {
                                string ownerType = _settings.TypeReferenceHelper.ConvertToString(declaringType);
                                int componentId = parameters.ComponentConnector.ConnectAttachedEventHandler(elementType, ownerType, memberName, attributeValue);
                                parameters.AppendLine(
                                    $"{RuntimeHelperClass}.XamlContext_SetConnectionId({parameters.CurrentXamlContext}, {componentId}, {elementUid});");
                            }
                            break;

                        default:
                            if (attachedMemberOnly)
                            {
                                throw new XamlParseException(
                                    $"The attached property '{attribute.Name.LocalName}' is not defined on '{elementType}' or one of its base classes.",
                                    attribute);
                            }
                            else
                            {
                                throw new XamlParseException(
                                    $"The property '{memberName}' does not exist in XML namespace '{attribute.Name.NamespaceName}'.",
                                    attribute);
                            }
                    }
                }

                foreach (var initializer in deferredInitializers)
                {
                    initializer();
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
                    _settings.XamlNameParser.GetClrNamespaceAndLocalName(
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

                        _settings.XamlNameParser.GetClrNamespaceAndLocalName(
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

                EventDefinition eventDefinition = _settings.Inspector.GetEvent(
                    ownerType,
                    eventName,
                    MemberFlags.Public | MemberFlags.NonPublic | MemberFlags.Instance);

                if (eventDefinition is not null)
                {
                    handlerTypeString = _settings.TypeReferenceHelper.ConvertToString(eventDefinition.EventType);
                }
                else
                {
                    MethodDefinition addHandlerMethodDefinition = _settings.Inspector.GetMethod(
                        ownerType,
                        $"Add{eventName}Handler",
                        MemberFlags.Public | MemberFlags.NonPublic | MemberFlags.Static);
                    
                    if (addHandlerMethodDefinition is not null && addHandlerMethodDefinition.Parameters.Count == 2)
                    {
                        handlerTypeString = _settings.TypeReferenceHelper.ConvertToString(addHandlerMethodDefinition.Parameters[1].ParameterType);
                    }
                    else
                    {
                        throw new XamlParseException($"Cannot find the Style Event '{eventName}' on the type '{ownerTypeString}'.", eventSetter);
                    }
                }

                // Start generating the code
                int componentId = parameters.ComponentConnector.ConnectEventSetterHandler(handlerTypeString, GeneratingCode.GetAttributeValue(handlerAttribute));

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
                (string namespaceName, string assemblyName) = _settings.XamlNameParser.GetClrNamespaceAndAssembly(
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
                XElement targetElement = _reader.MemberData.Target;
                XElement memberElement = _reader.MemberData.Member;
                XElement valueElement = _reader.MemberData.Value;

                string targetUid = GeneratingCode.GetUniqueName(targetElement);
                string valueUid = GeneratingCode.GetUniqueName(valueElement);

                TypeDefinition targetTypeDefinition = GetTypeDefinition(targetElement.Name, targetElement);
                TypeDefinition valueTypeDefinition = GetTypeDefinition(valueElement.Name, valueElement);

                int index = memberElement.Name.LocalName.IndexOf('.');

                string memberName = memberElement.Name.LocalName.Substring(index + 1);
                string typeName = memberElement.Name.LocalName.Substring(0, index);
                TypeDefinition fromType = GetTypeDefinition(
                    memberElement.Name.Namespace.GetName(typeName), memberElement);

                MemberKind memberLookupFlags;

                // TODO: add support for events
                if (fromType.IsAssignableFrom(targetTypeDefinition))
                {
                    memberLookupFlags = MemberKind.Property | MemberKind.AttachedPropertyGet | MemberKind.AttachedPropertySet;
                }
                else
                {
                    memberLookupFlags = MemberKind.AttachedPropertyGet | MemberKind.AttachedPropertySet;
                }

                MemberReference memberReference = _settings.Inspector.GetMemberFromType(
                    memberName,
                    fromType,
                    memberLookupFlags,
                    out TypeReference declaringType,
                    out TypeReference memberType,
                    out MemberKind memberKind);

                if (memberReference is null)
                {
                    throw new XamlParseException(
                        $"The tag '{memberElement.Name.LocalName}' does not exist in XML namespace '{memberElement.Name.NamespaceName}'.",
                        memberElement);
                }

                if (_settings.Inspector.IsFrameworkTemplateTemplateProperty(memberReference))
                {
                    // TODO move call to RuntimeHelpers.SetTemplateContent(...) here
                    parameters.PopScope();
                }
                else
                {
                    bool isAttachedProperty = memberKind == MemberKind.AttachedPropertyGet ||
                                              memberKind == MemberKind.AttachedPropertySet;

                    TypeDefinition memberTypeDefinition = memberType.ResolveOrThrow();

                    bool isList = _settings.Inspector.IsIList(memberTypeDefinition);
                    bool isDictionary = _settings.Inspector.IsIDictionary(memberTypeDefinition);

                    string ownerType = _settings.TypeReferenceHelper.ConvertToString(declaringType);

                    // Check if the property is a collection, in which case we must use ".Add(...)", otherwise a simple "=" is enough:
                    if ((isList || isDictionary) && IsPropertyACollection(memberElement, memberTypeDefinition))
                    {
                        //------------------------
                        // PROPERTY TYPE IS A COLLECTION
                        //------------------------

                        string codeToAccessTheEnumerable;
                        if (isAttachedProperty)
                        {
                            codeToAccessTheEnumerable = $"global::{ownerType}.Get{memberName}({targetUid})";
                        }
                        else
                        {
                            codeToAccessTheEnumerable = $"((global::{ownerType}){targetUid}).{memberName}";
                        }

                        if (isDictionary)
                        {
                            string childKey = GetElementXKey(valueElement);
                            parameters.AppendLine($"((global::System.Collections.IDictionary){codeToAccessTheEnumerable}).Add({childKey}, {valueUid});");
                        }
                        else
                        {
                            parameters.AppendLine($"((global::System.Collections.IList){codeToAccessTheEnumerable}).Add({valueUid});");
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
                        if (!_settings.Inspector.IsElementAMarkupExtension(valueTypeDefinition) || _settings.Inspector.IsRelativeSource(valueTypeDefinition))
                        {
                            if (isAttachedProperty)
                            {
                                parameters.AppendLine(
                                    $"global::{ownerType}.Set{memberName}({targetUid}, {valueUid});");
                            }
                            else
                            {
                                parameters.AppendLine($"((global::{ownerType}){targetUid}).{memberName} = {valueUid};");
                            }
                        }
                        else
                        {
                            //------------------------------
                            // MARKUP EXTENSIONS:
                            //------------------------------

                            if (_settings.Inspector.IsStaticResourceExtension(valueTypeDefinition) ||
                                _settings.Inspector.IsThemeResourceExtension(valueTypeDefinition))
                            {
                                //------------------------------
                                // {StaticResource ...}
                                //------------------------------

                                string propertyType = _settings.TypeReferenceHelper.ConvertToString(memberType);

                                if (isAttachedProperty)
                                {
                                    parameters.AppendLine(
                                        $"global::{ownerType}.Set{memberName}({targetUid}, (global::{propertyType})({RuntimeHelperClass}.CallProvideValue({parameters.CurrentXamlContext}, {valueUid})));");
                                }
                                else
                                {
                                    parameters.AppendLine(
                                        $"((global::{ownerType}){targetUid}).{memberName} = (global::{propertyType}){RuntimeHelperClass}.CallProvideValue({parameters.CurrentXamlContext}, {valueUid});");
                                }
                            }
                            else if (_settings.Inspector.IsBinding(valueTypeDefinition) ||
                                     _settings.Inspector.IsMultiBinding(valueTypeDefinition))
                            {
                                //------------------------------
                                // {Binding ...} or MultiBinding
                                //------------------------------

                                (FieldDefinition dpDefinition, TypeReference dpDeclaringType) =
                                    _settings.Inspector.GetField(fromType, $"{memberName}Property", MemberFlags.Public | MemberFlags.Static);

                                // Check if the property is of type "Binding/MultiBinding" (or "BindingBase"), in which 
                                // case we should directly assign the value instead of calling "SetBinding"
                                if (dpDefinition is null || memberType == valueTypeDefinition || _settings.Inspector.IsBindingBase(memberType))
                                {
                                    parameters.AppendLine($"((global::{ownerType}){targetUid}).{memberName} = {valueUid};");
                                }
                                else
                                {
                                    string dpFullName = $"global::{_settings.TypeReferenceHelper.ConvertToString(dpDeclaringType)}.{dpDefinition.Name}";
                                    parameters.AppendLine(
                                        $"global::{KnownNamespaces.SystemWindowsData}.BindingOperations.SetBinding({targetUid}, {dpFullName}, {valueUid});");
                                }
                            }
                            else if (_settings.Inspector.IsDynamicResourceExtension(valueTypeDefinition) ||
                                     _settings.Inspector.IsResponsiveExtension(valueTypeDefinition))
                            {
                                //-----------------------------------
                                // {DynamicResource} or {Responsive}
                                //-----------------------------------

                                (FieldDefinition dpDefinition, TypeReference dpDeclaringType) =
                                    _settings.Inspector.GetField(fromType, $"{memberName}Property", MemberFlags.Public | MemberFlags.Static);

                                if (dpDefinition is null)
                                {
                                    if (ownerType == $"{KnownNamespaces.SystemWindows}.Setter" && memberName == "Value")
                                    {
                                        parameters.AppendLine($"((global::{ownerType}){targetUid}).{memberName} = {valueUid};");
                                    }
                                    else
                                    {
                                        throw new XamlParseException(
                                            $"A '{memberElement.Name.LocalName}' cannot be set on the '{memberName}' property of type '{ownerType}'. A '{memberElement.Name.LocalName}' can only be set on a DependencyProperty of a DependencyObject, or the Setter.Value property.",
                                            memberElement);
                                    }
                                }
                                else
                                {
                                    string markupValue = GeneratingUniqueNames.GenerateUniqueName("tmp");
                                    string dependencyPropertyName = $"global::{_settings.TypeReferenceHelper.ConvertToString(dpDeclaringType)}.{dpDefinition.Name}";
                                    string propertyType = _settings.TypeReferenceHelper.ConvertToString(memberType);

                                    parameters
                                        .AppendLine($"object {markupValue};")
                                        .AppendLine($"if (!{RuntimeHelperClass}.TrySetMarkupExtension({targetUid}, {dependencyPropertyName}, {valueUid}, out {markupValue}))")
                                        .AppendLine("{");

                                    if (isAttachedProperty)
                                    {
                                        parameters.AppendLine($"    global::{ownerType}.Set{memberName}({targetUid}, (global::{propertyType}){markupValue});");
                                    }
                                    else
                                    {
                                        parameters.AppendLine($"    ((global::{ownerType}){targetUid}).{memberName} = (global::{propertyType}){markupValue};");
                                    }

                                    parameters.AppendLine("}");
                                }
                            }
                            else if (_settings.Inspector.IsTemplateBindingExtension(valueTypeDefinition))
                            {
                                (FieldDefinition dpDefinition, TypeReference dpDeclaringType) =
                                    _settings.Inspector.GetField(fromType, $"{memberName}Property", MemberFlags.Public | MemberFlags.Static);

                                string dpName;
                                
                                if (dpDefinition is null)
                                {
                                    dpName = $"{RuntimeHelperClass}.DependencyPropertyFromName({EscapeString(memberName)}, global::{_settings.TypeReferenceHelper.ConvertToString(targetTypeDefinition)})";
                                }
                                else
                                {
                                    dpName = $"global::{_settings.TypeReferenceHelper.ConvertToString(dpDeclaringType)}.{dpDefinition.Name}";
                                }

                                parameters.AppendLine(
                                    $"{targetUid}.SetValue({dpName}, {RuntimeHelperClass}.CallProvideValue({parameters.CurrentXamlContext}, {valueUid}));");
                            }
                            else if (_settings.Inspector.IsNullExtension(valueTypeDefinition))
                            {
                                //------------------------------
                                // {x:Null}
                                //------------------------------

                                if (isAttachedProperty)
                                {
                                    parameters.AppendLine($"global::{ownerType}.Set{memberName}({targetUid}, null);");
                                }
                                else
                                {
                                    parameters.AppendLine($"((global::{ownerType}){targetUid}).{memberName} = null;");
                                }
                            }
                            else if (_settings.Inspector.IsStaticExtension(valueTypeDefinition))
                            {
                                string staticMemberName = ResolveStaticExtension(valueElement, valueElement);
                                string propertyType = _settings.TypeReferenceHelper.ConvertToString(memberType);

                                if (isAttachedProperty)
                                {
                                    parameters.AppendLine(
                                        $"global::{ownerType}.Set{memberName}({targetUid}, (global::{propertyType})(object){staticMemberName});");
                                }
                                else
                                {
                                    parameters.AppendLine(
                                        $"((global::{ownerType}){targetUid}).{memberName} = (global::{propertyType})(object){staticMemberName};");
                                }
                            }
                            else if (_settings.Inspector.IsTypeExtension(valueTypeDefinition))
                            {
                                string resolvedTypeName = ResolveTypeExtension(valueElement, valueElement);
                                string propertyType = _settings.TypeReferenceHelper.ConvertToString(memberType);

                                if (isAttachedProperty)
                                {
                                    parameters.AppendLine(
                                        $"global::{ownerType}.Set{memberName}({targetUid}, (global::{propertyType})(object)typeof(global::{resolvedTypeName}));");
                                }
                                else
                                {
                                    parameters.AppendLine(
                                        $"((global::{ownerType}){targetUid}).{memberName} = (global::{propertyType})(object)typeof(global::{resolvedTypeName});");
                                }
                            }
                            else
                            {
                                //------------------------------
                                // Other (custom MarkupExtensions)
                                //------------------------------

                                string propertyType = _settings.TypeReferenceHelper.ConvertToString(memberType);
                                (FieldDefinition dpDefinition, TypeReference dpDeclaringType) =
                                    _settings.Inspector.GetField(fromType, $"{memberName}Property", MemberFlags.Public | MemberFlags.Static);

                                if (dpDefinition is not null)
                                {
                                    string markupValue = GeneratingUniqueNames.GenerateUniqueName("tmp");
                                    string dpName = $"global::{_settings.TypeReferenceHelper.ConvertToString(dpDeclaringType)}.{dpDefinition.Name}";

                                    parameters
                                        .AppendLine($"object {markupValue};")
                                        .AppendLine($"if (!{RuntimeHelperClass}.TrySetMarkupExtension({targetUid}, {dpName}, {valueUid}, out {markupValue}))")
                                        .AppendLine("{");

                                    if (isAttachedProperty)
                                    {
                                        parameters.AppendLine($"    global::{ownerType}.Set{memberName}({targetUid}, (global::{propertyType}){markupValue});");
                                    }
                                    else
                                    {
                                        parameters.AppendLine($"    ((global::{ownerType}){targetUid}).{memberName} = (global::{propertyType}){markupValue};");
                                    }

                                    parameters.AppendLine("}");
                                }
                                else
                                {
                                    string markupExtension =
                                        $"(({IMarkupExtensionClass}){valueUid}).ProvideValue(new global::System.ServiceProvider({targetUid}, null))";

                                    if (isAttachedProperty)
                                    {
                                        parameters.AppendLine(
                                            $"global::{ownerType}.Set{memberName}({targetUid}, (global::{propertyType}){markupExtension});");
                                    }
                                    else
                                    {
                                        parameters.AppendLine($"((global::{ownerType}){targetUid}).{memberName} = (global::{propertyType}){markupExtension};");
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
                TypeDefinition targetTypeDefinition = GetTypeDefinition(target.Name, target);

                XElement child = _reader.MemberData.Value;
                string childUid = GeneratingCode.GetUniqueName(child);

                if (_settings.Inspector.IsIDictionary(targetTypeDefinition))
                {
                    string childKey = GetElementXKey(child);
                    parameters.AppendLine($"((global::System.Collections.IDictionary){targetUid}).Add({childKey}, {childUid});");
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
                return element == _reader.Document.Root;
            }

            private string GetCSharpFullTypeNameFromTargetTypeString(XElement styleElement, bool isDataType = false)
            {
                if (styleElement.Attribute(isDataType ? "DataType" : "TargetType") is not XAttribute targetTypeAttribute)
                {
                    throw new XamlParseException(
                        isDataType ? "DataTemplate must declare a DataType or have a key." : "Style must declare a TargetType.",
                        styleElement);
                }

                return GetCSharpFullTypeName(targetTypeAttribute.Value, styleElement, targetTypeAttribute);
            }

            private string GetCSharpFullTypeName(string value, XElement element, IXmlLineInfo lineInfo)
            {
                _settings.XamlNameParser.GetClrNamespaceAndLocalName(
                    value,
                    element,
                    out string namespaceName,
                    out string typeName,
                    out string assemblyName);

                TypeDefinition type = _settings.Inspector.GetTypeDefinition(namespaceName, typeName, assemblyName, lineInfo);
                return $"global::{_settings.TypeReferenceHelper.ConvertToString(type)}";
            }

            private string GetElementXKey(XElement element)
            {
                if (element.Attribute(GeneratingCode.XKeyAttribute) is XAttribute keyAttribute)
                {
                    if (!MarkupExtensionDescriptor.LooksLikeAMarkupExtension(keyAttribute.Value))
                    {
                        return EscapeString(GeneratingCode.GetAttributeValue(keyAttribute));
                    }

                    var markupExtension = MarkupExtensionDescriptor.Parse(keyAttribute.Value, keyAttribute);
                    var markupExtensionElement = InsertingMarkupNodesInXaml.GenerateExtensionElement(markupExtension, _settings, element, keyAttribute);
                    var type = GetTypeDefinition(markupExtensionElement.Name, keyAttribute);

                    if (_settings.Inspector.IsTypeExtension(type))
                    {
                        string resolvedTypeName = ResolveTypeExtension(markupExtensionElement, element);
                        return $"typeof(global::{resolvedTypeName})";
                    }
                    else if (_settings.Inspector.IsStaticExtension(type))
                    {
                        return ResolveStaticExtension(markupExtensionElement, element);
                    }
                    else
                    {
                        throw new XamlParseException(
                            $"A key for a dictionary cannot be of type '{_settings.TypeReferenceHelper.ConvertToString(type)}'. Only String, TypeExtension, and StaticExtension are supported.",
                            keyAttribute);
                    }
                }

                if (element.Attribute(GeneratingCode.XNameAttribute) is XAttribute nameAttribute)
                {
                    return EscapeString(nameAttribute.Value);
                }

                if (GeneratingCode.IsStyle(element, _settings))
                {
                    return $"typeof({GetCSharpFullTypeNameFromTargetTypeString(element)})";
                }

                if (GeneratingCode.IsDataTemplate(element, _settings) && element.Attribute("DataType") != null)
                {
                    return $"new global::{KnownNamespaces.SystemWindows}.DataTemplateKey(typeof({GetCSharpFullTypeNameFromTargetTypeString(element, isDataType: true)}))";
                }

                throw new XamlParseException(
                    $"Each dictionary entry must have an associated key. The element named '{element.Name.LocalName}' does not have a key.",
                    element);
            }

            private string GenerateCodeForInstantiatingAttributeValue(
                string memberName,
                MemberReference memberReference,
                TypeReference declaringType,
                TypeReference memberType,
                TypeReference elementType,
                string value,
                XElement element, // Only used for prefix to namespace mapping
                XObject lineInfo)
            {
                bool hasTypeConverter = _settings.Inspector.HasTypeConverter(memberReference);
                bool isEnum = _settings.TypeReferenceHelper.IsEnum(memberType.ResolveOrThrow());

                string memberTypeName = _settings.TypeReferenceHelper.ConvertToString(memberType);
                bool isKnownSystemType = _settings.SystemTypes.IsKnownType(memberTypeName, memberType.GetAssemblyName());
                bool isKnownCoreType = _settings.CoreTypes.IsKnownType(memberTypeName, memberType.GetAssemblyName());

                // Generate the code or instantiating the attribute
                if (isEnum && !isKnownSystemType && !isKnownCoreType)
                {
                    //----------------------------
                    // PROPERTY IS AN ENUM
                    //----------------------------

                    return string.Join(" | ", _settings.Inspector.GetEnumValues(
                        memberType.ResolveOrThrow(),
                        value.Trim(),
                        true,
                        true,
                        lineInfo));
                }
                else if (memberTypeName == "System.Type")
                {
                    string typeFullName = GetCSharpFullTypeName(value, element, lineInfo);

                    return $"typeof({typeFullName})";
                }
                else
                {
                    //----------------------------
                    // PROPERTY IS OF ANOTHER TYPE
                    //----------------------------

                    value = ConvertRelativeUri(value, memberName, memberTypeName, elementType, declaringType);

                    string preparedValue = ConvertFromInvariantString(
                        value, lineInfo, memberTypeName, isKnownCoreType, isKnownSystemType);

                    if (hasTypeConverter)
                    {
                        string declaringTypeName = _settings.TypeReferenceHelper.ConvertToString(declaringType);
                        return XamlContextGetPropertyValue(
                            declaringTypeName, memberName, value, memberTypeName, preparedValue);
                    }

                    return preparedValue;
                }
            }

            private string ConvertRelativeUri(
                string value,
                string memberName,
                string memberTypeName,
                TypeReference elementType,
                TypeReference declaringType)
            {
                if (GeneratingCode.ShouldConvertUri(value, memberName, memberTypeName, elementType, declaringType))
                {
                    return CreateComponentUri(value);
                }

                return value;
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

                    XNamespace xmlNamespace = xmlPrefix is null ?
                        element.GetDefaultNamespace() :
                        element.GetNamespaceOfPrefix(xmlPrefix);

                    if (GetTypeDefinition(XName.Get(typeName, xmlNamespace.NamespaceName), lineInfo, false) is not TypeDefinition type)
                    {
                        return false;
                    }

                    sb.Append('(')
                      .Append($"{_settings.TypeReferenceHelper.ConvertToString(type)}, {type.GetAssemblyName()}")
                      .Append('.')
                      .Append(propertyName)
                      .Append(')');

                    pos++;
                }
            }

            private void ResolvePathForTemplateBinding(string path, XElement element, out TypeDefinition ownerType, out string propertyName)
            {
                ownerType = null;
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

                    XNamespace xmlNamespace = xmlPrefix is null ?
                        element.GetDefaultNamespace() :
                        element.GetNamespaceOfPrefix(xmlPrefix);

                    ownerType = GetTypeDefinition(XName.Get(type, xmlNamespace.NamespaceName), element, false);
                }
            }

            private string ConvertFromInvariantString(string value, XObject context, string type, bool isKnownCoreType, bool isKnownSystemType)
            {
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
                return $"{RuntimeHelperClass}.GetPropertyValue<global::{propertyType}>(typeof(global::{propertyDeclaringType}), {EscapeString(propertyName)}, {EscapeString(value)}, () => {fallbackValue})";
            }

            private static string EscapeString(string stringValue)
            {
                return string.Concat("@\"", stringValue.Replace("\"", "\"\""), "\"");
            }

            public bool IsPropertyACollection(XElement memberElement, TypeDefinition memberType)
            {
                if (memberElement.Elements().Count() != 1)
                {
                    return true;
                }

                XElement child = memberElement.Elements().First();
                TypeDefinition childTypeDefinition = GetTypeDefinition(child.Name, child);

                return !memberType.IsAssignableFrom(childTypeDefinition) &&
                       !_settings.Inspector.IsBinding(childTypeDefinition) &&
                       !_settings.Inspector.IsStaticResourceExtension(childTypeDefinition) &&
                       !_settings.Inspector.IsTemplateBindingExtension(childTypeDefinition) &&
                       !_settings.Inspector.IsDynamicResourceExtension(childTypeDefinition);
            }

            private TypeDefinition GetTypeDefinition(XName xName, IXmlLineInfo lineInfo, bool throwIfNull = true)
            {
                _settings.XamlNameParser.GetClrNamespaceAndLocalName(
                    xName,
                    out string namespaceName,
                    out string typeName,
                    out string assemblyName);

                return _settings.Inspector.GetTypeDefinition(
                    namespaceName,
                    typeName,
                    assemblyName,
                    lineInfo,
                    throwIfNull);
            }

            private string ResolveStaticExtension(XElement element, XElement xmlnsResolver)
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
                    type = GetTypeDefinitionFromString(typeAttribute.Value, xmlnsResolver, typeAttribute);
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

                    type = GetTypeDefinitionFromString(typeString, xmlnsResolver, member);

                    // Get the member name substring.
                    fieldString = member.Value.Substring(dotIndex + 1, member.Value.Length - dotIndex - 1);
                    if (string.IsNullOrEmpty(typeString))
                    {
                        throw new XamlParseException(
                            $"'{member.Value}' StaticExtension value cannot be resolved to an enumeration, static field, or static property.",
                            member);
                    }
                }

                if (_settings.TypeReferenceHelper.IsEnum(type))
                {
                    return _settings.Inspector.GetEnumValue(type, fieldString, false, false);
                }

                FieldDefinition staticField;
                PropertyDefinition staticProperty;
                TypeReference declaringType;

                (staticField, declaringType) = _settings.Inspector.GetField(type, fieldString, MemberFlags.Public | MemberFlags.Static);

                if (staticField is not null)
                {
                    return $"global::{_settings.TypeReferenceHelper.ConvertToString(declaringType)}.{staticField.Name}";
                }

                (staticProperty, declaringType) = _settings.Inspector.GetProperty(type, fieldString, MemberFlags.Public | MemberFlags.Static);

                if (staticProperty is not null)
                {
                    if (staticProperty.GetMethod is null)
                    {
                        throw new XamlParseException(
                            $"The '{_settings.TypeReferenceHelper.ConvertToString(declaringType)}{staticProperty.Name}' property does not define a Get method.",
                            member);
                    }

                    if (!staticProperty.GetMethod.IsPublic)
                    {
                        throw new XamlParseException(
                            $"The '{_settings.TypeReferenceHelper.ConvertToString(declaringType)}{staticProperty.Name}' property Get method is not accessible.",
                            member);
                    }

                    return $"global::{_settings.TypeReferenceHelper.ConvertToString(declaringType)}.{staticProperty.Name}";
                }

                throw new XamlParseException(
                    $"'{(typeNameForError is not null ? $"{typeNameForError}.{member.Value}" : member.Value)}' StaticExtension value cannot be resolved to an enumeration, static field, or static property.",
                    member);
            }

            private string ResolveTypeExtension(XElement element, XElement xmlnsResolver)
            {
                if (element.Attribute("Type") is XAttribute typeAttribute)
                {
                    return _settings.TypeReferenceHelper.ConvertToString(GetTypeDefinitionFromString(typeAttribute.Value, xmlnsResolver, typeAttribute));
                }

                if (element.Attribute("TypeName") is not XAttribute typeNameAttribute)
                {
                    throw new XamlParseException("TypeExtension must have TypeName property set.", element);
                }

                return _settings.TypeReferenceHelper.ConvertToString(GetTypeDefinitionFromString(typeNameAttribute.Value, xmlnsResolver, typeNameAttribute));
            }

            private TypeDefinition GetTypeDefinitionFromString(string value, XElement xmlnsResolver, IXmlLineInfo lineInfo)
                => GeneratingCode.GetTypeDefinitionFromString(value, xmlnsResolver, lineInfo, _settings);

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

                (TypeDefinition fromType, string propertyName) =
                    SetterTriggerConditionHelpers.GetSetterOrTriggerOrConditionProperty(property, targetPropertyName, _settings);

                if (fromType is null)
                {
                    throw new XamlParseException($"Unable to identify the declaring type of the {typeName}'s property.", property);
                }

                MemberReference memberReference = _settings.Inspector.GetMemberFromType(
                    propertyName,
                    fromType,
                    MemberKind.Property | MemberKind.AttachedPropertyGet | MemberKind.AttachedPropertySet,
                    out TypeReference declaringType,
                    out TypeReference memberType,
                    out _);

                if (memberReference is null)
                {
                    throw new XamlParseException(
                        $"Cannot find the Style Property '{propertyName}' on the type '{_settings.TypeReferenceHelper.ConvertToString(fromType)}'.",
                        property);
                }

                return GenerateCodeForInstantiatingAttributeValue(
                    propertyName,
                    memberReference,
                    declaringType,
                    memberType,
                    fromType,
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

                return $"global::{_settings.TypeReferenceHelper.ConvertToString(declaringType)}.{propertyName}Property";
            }
        }
    }
}
