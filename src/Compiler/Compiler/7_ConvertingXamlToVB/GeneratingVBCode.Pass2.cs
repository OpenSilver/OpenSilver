
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Mono.Cecil;
using OpenSilver.Compiler.OtherHelpersAndHandlers.MonoCecilAssembliesInspector;
using OpenSilver.Internal;

namespace OpenSilver.Compiler
{
    internal static partial class GeneratingVBCode
    {
        private class GeneratorPass2 : ICodeGenerator
        {
            private abstract class GeneratorScope
            {
                protected GeneratorScope(string rootElement)
                {
                    Root = rootElement;
                    XamlContext = GeneratingUniqueNames.GenerateUniqueName("xamlContext");
                }

                public string Root { get; }

                public string XamlContext { get; }

                public StringBuilder StringBuilder { get; } = new StringBuilder();

                public abstract void RegisterName(string name, string scopedElement);

                protected abstract string ToStringCore();

                public sealed override string ToString() => ToStringCore();
            }

            private sealed class RootScope : GeneratorScope
            {
                private readonly Dictionary<string, string> _namescope;

                public RootScope(string rootElementName, bool createNameScope)
                    : base(rootElementName)
                {
                    StringBuilder.AppendLine($"Dim {XamlContext} = {RuntimeHelperClass}.Create_XamlContext()");
                    if (createNameScope)
                    {
                        _namescope = new Dictionary<string, string>();
                    }
                }

                public override void RegisterName(string name, string scopedElement)
                {
                    if (_namescope != null)
                    {
                        _namescope.Add(name, scopedElement);
                    }
                }

                protected override string ToStringCore()
                {
                    StringBuilder builder = new StringBuilder();

                    builder.Append(StringBuilder.ToString());
                    AppendNamescope(builder);

                    return builder.ToString();
                }

                private void AppendNamescope(StringBuilder builder)
                {
                    if (_namescope == null) return;

                    builder.AppendLine($"{RuntimeHelperClass}.InitializeNameScope({Root})");

                    foreach (var kp in _namescope)
                    {
                        builder.AppendLine($"{RuntimeHelperClass}.RegisterName({Root}, {EscapeString(kp.Key)}, {kp.Value})");
                    }
                }
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
                    throw new NotSupportedException();
                }

                protected override string ToStringCore()
                {
                    StringBuilder builder = new StringBuilder();

                    builder.AppendLine($"Private Shared Function {MethodName}({XamlContext} As {XamlContextClass}) As {ObjectType}")
                        .Append(StringBuilder.ToString());
                    builder.AppendLine($"Return {Root}")
                        .AppendLine("End Function");

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
                    StringBuilder.AppendLine($"{RuntimeHelperClass}.XamlContext_RegisterName({XamlContext}, {EscapeString(name)}, {scopedElement})");
                }

                protected override string ToStringCore()
                {
                    StringBuilder builder = new StringBuilder();

                    builder.AppendLine($"Private Shared Function {MethodName}({TemplateOwner} As Global.{KnownNamespaces.SystemWindows}.IFrameworkElement, {XamlContext} As {XamlContextClass}) As Global.{KnownNamespaces.SystemWindows}.IFrameworkElement")
                        .Append(StringBuilder.ToString());
                    builder.AppendLine($"Return {Root}")
                        .AppendLine("End Function");

                    return builder.ToString();
                }
            }

            private class GeneratorContext
            {
                private readonly Stack<GeneratorScope> _scopes = new();

                public readonly List<string> ResultingMethods = new List<string>();
                public readonly List<string> ResultingFieldsForNamedElements = new List<string>();
                public readonly List<string> ResultingFindNameCalls = new List<string>();
                public readonly ComponentConnectorBuilderVB ComponentConnector = new ComponentConnectorBuilderVB();

                public bool GenerateFieldsForNamedElements { get; set; }

                public GeneratorScope CurrentScope => _scopes.Peek();
                public StringBuilder StringBuilder => CurrentScope.StringBuilder;

                public void PushScope(GeneratorScope scope)
                {
                    _scopes.Push(scope);
                }

                public void PopScope()
                {
                    if (_scopes.Count <= 1)
                    {
                        throw new InvalidOperationException();
                    }

                    ResultingMethods.Add(_scopes.Pop().ToString());
                }

                public string CurrentXamlContext => CurrentScope.XamlContext;
            }

            private readonly XamlReader _reader;
            private readonly ConversionSettings _settings;

            private readonly string _sourceFile;
            private readonly string _fileNameWithPathRelativeToProjectRoot;
            private readonly string _rootNamespace;

            public GeneratorPass2(XDocument doc,
                string sourceFile,
                string fileNameWithPathRelativeToProjectRoot,
                string rootNamespace,
                ConversionSettings settings)
            {
                _reader = new XamlReader(doc);
                _settings = settings;
                _sourceFile = sourceFile;
                _fileNameWithPathRelativeToProjectRoot = fileNameWithPathRelativeToProjectRoot;
                _rootNamespace = rootNamespace;
            }

            public string Generate() => GenerateImpl(new GeneratorContext());

            private string GenerateImpl(GeneratorContext parameters)
            {
                parameters.GenerateFieldsForNamedElements =
                    !_settings.Inspector.IsAssignableFrom(
                        KnownNamespaces.SystemWindows, "ResourceDictionary",
                        _reader.Document.Root.Name.NamespaceName, _reader.Document.Root.Name.LocalName)
                    &&
                    !_settings.Inspector.IsAssignableFrom(
                        KnownNamespaces.SystemWindows, "Application",
                        _reader.Document.Root.Name.NamespaceName, _reader.Document.Root.Name.LocalName);

                parameters.PushScope(
                    new RootScope(GeneratingCode.GetUniqueName(_reader.Document.Root),
                        _settings.Inspector.IsAssignableFrom(
                            KnownNamespaces.SystemWindows, "IFrameworkElement",
                            _reader.Document.Root.Name.NamespaceName, _reader.Document.Root.Name.LocalName)));

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

                (string namespaceDeclaration, string namespaceName) = GetNamespace(namespaceStringIfAny, _rootNamespace);

                string baseType = GetCSharpEquivalentOfXamlTypeAsString(_reader.Document.Root.Name, true);

                if (hasCodeBehind)
                {
                    bool isApp = IsClassTheApplicationClass(baseType);

                    string connectMethod = parameters.ComponentConnector.ToString();
                    string initializeComponentMethod = CreateInitializeComponentMethod(
                        $"Global.{KnownNamespaces.SystemWindows}.Application",
                        _settings.AssemblyName,
                        _fileNameWithPathRelativeToProjectRoot,
                        parameters.ResultingFindNameCalls);

                    string additionalConstructors = isApp ?
                        @"Private Sub New(stub as Global.OpenSilver.XamlDesignerConstructorStub)
    InitializeComponent()
End Sub
" : string.Empty;

                    // Wrap everything into a partial class:
                    string partialClass = GeneratePartialClass(additionalConstructors,
                                                               initializeComponentMethod,
                                                               connectMethod,
                                                               parameters.ResultingFieldsForNamedElements,
                                                               className,
                                                               namespaceDeclaration,
                                                               baseType);

                    string componentTypeFullName = GetFullTypeName(namespaceName, className);

                    string factoryClass = GenerateFactoryClass(
                        componentTypeFullName,
                        baseType,
                        GeneratingCode.GetUniqueName(_reader.Document.Root),
                        parameters.CurrentScope.ToString(),
                        $"Return CType(Global.CSHTML5.Internal.TypeInstantiationHelper.Instantiate(GetType({componentTypeFullName})), {componentTypeFullName})",
                        parameters.ResultingMethods,
                        $"Global.{KnownNamespaces.SystemWindows}.UIElement",
                        _settings.AssemblyName,
                        _fileNameWithPathRelativeToProjectRoot);

                    string finalCode = $@"
{factoryClass}
{partialClass}";

                    return finalCode;
                }
                else
                {
                    string rootElementName = GeneratingCode.GetUniqueName(_reader.Document.Root);

                    string finalCode = GenerateFactoryClass(
                        baseType,
                        baseType,
                        rootElementName,
                        parameters.CurrentScope.ToString(),
                        string.Join(Environment.NewLine, $"Dim {rootElementName} = New {baseType}()", $"LoadComponentImpl({rootElementName})", $"Return {rootElementName}"),
                        parameters.ResultingMethods,
                        $"Global.{KnownNamespaces.SystemWindows}.UIElement",
                        _settings.AssemblyName,
                        _fileNameWithPathRelativeToProjectRoot);

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
                    element.Name,
                    out string namespaceName,
                    out string localTypeName,
                    out string assemblyNameIfAny);

                // Some special cases
                if (elementType == $"Global.{KnownNamespaces.SystemWindows}.EventSetter")
                {
                    WriteEventSetter(parameters);

                    // EventSetter only support the Event, Handler and HandledEventsToo properties. WriteEventSetter
                    // already takes care of these properties, so we just skip everything.
                    while (_reader.Read())
                    {
                        if (_reader.NodeType == XamlNodeType.EndObject && _reader.ObjectData.Element == element)
                        {
                            break;
                        }
                    }

                    return;
                }

                // Get information about which element holds the namescope of the current element. For example, if the current element is inside a DataTemplate, the DataTemplate is the root of the namescope of the current element. If the element is not inside a DataTemplate or ControlTemplate, the root of the XAML is the root of the namescope of the current element.
                bool isElementInRootNamescope = GetRootOfCurrentNamescopeForRuntime(element).Parent == null;

                bool isRootElement = IsElementTheRootElement(element);
                bool isKnownSystemType = _settings.SystemTypes.IsKnownType(
                    elementType.Substring("Global.".Length), assemblyNameIfAny);
                bool isInitializeTypeFromString =
                    element.Attribute(InsertingImplicitNodes.InitializedFromStringAttribute) != null;

                // Add the constructor (in case of object) or a direct initialization (in case
                // of system type or "isInitializeFromString" or referenced ResourceDictionary)
                // (unless this is the root element)
                string elementUid = GeneratingCode.GetUniqueName(element);

                bool isInNewScope = false;
                GeneratorScope rootScope = parameters.CurrentScope;

                if (isRootElement)
                {
                    parameters.StringBuilder.AppendLine($"{RuntimeHelperClass}.XamlContext_WriteStartObject({parameters.CurrentXamlContext}, {elementUid})");
                }
                else
                {
                    if (isKnownSystemType)
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

                        string preparedValue = _settings.SystemTypes.ConvertKnownType(directContent, elementType.Substring("Global.".Length));
                        parameters.StringBuilder.AppendLine(
                            $"Dim {elementUid} = {RuntimeHelperClass}.XamlContext_WriteStartObject({parameters.CurrentXamlContext}, {preparedValue})");
                    }
                    else if (isInitializeTypeFromString)
                    {
                        //------------------------------------------------
                        // Add the type initialization from string:
                        //------------------------------------------------

                        string stringValue = element.Attribute(InsertingImplicitNodes.InitializedFromStringAttribute).Value;

                        bool isKnownCoreType = _settings.CoreTypes.IsKnownType(
                            elementType.Substring("Global.".Length), assemblyNameIfAny);

                        string preparedValue = ConvertFromInvariantString(
                            stringValue, element, elementType, isKnownCoreType, isKnownSystemType);

                        parameters.StringBuilder.AppendLine(
                            $"Dim {elementUid} = {RuntimeHelperClass}.XamlContext_WriteStartObject({parameters.CurrentXamlContext}, {preparedValue})");
                    }
                    else
                    {
                        if (_settings.Options == XamlPreprocessorOptions.Auto)
                        {
                            isInNewScope = true;

                            var objectScope = new NewObjectScope(elementUid, elementType);

                            parameters.StringBuilder.AppendLine(
                                $"Dim {elementUid} = {objectScope.MethodName}({parameters.CurrentXamlContext})");

                            parameters.PushScope(objectScope);
                        }

                        parameters.StringBuilder.AppendLine(
                            $"Dim {elementUid} = {RuntimeHelperClass}.XamlContext_WriteStartObject({parameters.CurrentXamlContext}, New {elementType}())");

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

                            parameters.StringBuilder.AppendLine(
                                $"CType(New {loadTypeFullName}(), {IXamlComponentLoaderClass}).LoadComponent({elementUid})");
                        }
                    }
                }

                // Set templated parent if any
                if (rootScope is FrameworkTemplateScope &&
                    _settings.Inspector.IsAssignableFrom(KnownNamespaces.SystemWindows, "IFrameworkElement", element.Name.NamespaceName, element.Name.LocalName))
                {
                    parameters.StringBuilder.AppendLine(
                        $"{RuntimeHelperClass}.XamlContext_SetTemplatedParent({parameters.CurrentXamlContext}, {elementUid})");
                }

                if (_settings.Inspector.IsAssignableFrom(KnownNamespaces.SystemWindowsMediaAnimation, "Timeline", element.Name.NamespaceName, element.Name.LocalName))
                {
                    parameters.StringBuilder.AppendLine($"{RuntimeHelperClass}.XamlContext_SetAnimationContext({parameters.CurrentXamlContext}, {elementUid})");
                }

                if (_settings.Inspector.IsAssignableFrom(KnownNamespaces.SystemWindows, "IUIElement", element.Name.NamespaceName, element.Name.LocalName))
                {
                    string xamlPath = element.Attribute(GeneratingPathInXaml.PathInXamlAttribute)?.Value ?? string.Empty;
                    parameters.StringBuilder.AppendLine($"{XamlDesignerBridgeClass}.SetPathInXaml({elementUid}, \"{xamlPath}\")");
                    parameters.StringBuilder.AppendLine($"{XamlDesignerBridgeClass}.SetFilePath({elementUid}, \"{_sourceFile}\")");
                }

                // Add the attributes:
                foreach (XAttribute attribute in element.Attributes())
                {
                    //-------------
                    // ATTRIBUTE
                    //-------------

                    string attributeValue = GetAttributeValue(attribute);
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
                                if (isElementInRootNamescope && parameters.GenerateFieldsForNamedElements)
                                {
                                    string fieldModifier = "Friend";
                                    XAttribute attr = element.Attribute(GeneratingCode.xNamespace + "FieldModifier");
                                    if (attr != null)
                                    {
                                        fieldModifier = (attr.Value ?? "").ToLower();
                                    }

                                    // add '[]' to handle cases where x:Name is a forbidden word (for instance 'Me'
                                    // or any other VB keyword)
                                    string fieldName = $"[{name}]";
                                    parameters.ResultingFieldsForNamedElements.Add($"{fieldModifier} WithEvents {fieldName} As {elementType}");
                                    parameters.ResultingFindNameCalls.Add($"Me.{fieldName} = (CType(Me.FindName(\"{name}\"), {elementType}))");
                                }

                                if (isXNameAttr)
                                {
                                    if (_settings.Inspector.IsAssignableFrom(KnownNamespaces.SystemWindows, "DependencyObject",
                                        element.Name.NamespaceName, element.Name.LocalName))
                                    {
                                        parameters.StringBuilder.AppendLine(
                                            $"{elementUid}.SetValue(Global.{KnownNamespaces.SystemWindows}.FrameworkElement.NameProperty, \"{name}\")");
                                    }
                                }
                                else
                                {
                                    if (_settings.Inspector.DoesTypeContainNameMemberOfTypeString(namespaceName, localTypeName, assemblyNameIfAny))
                                    {
                                        parameters.StringBuilder.AppendLine($"{elementUid}.Name = \"{name}\"");
                                    }
                                }

                                rootScope.RegisterName(name, elementUid);
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
                                    MemberTypes memberType = _settings.Inspector.GetMemberType(memberName, namespaceName, localTypeName, assemblyNameIfAny);
                                    switch (memberType)
                                    {
                                        case MemberTypes.Event:

                                            //------------
                                            // C# EVENT
                                            //------------

                                            int componentId = parameters.ComponentConnector.ConnectEventHandler(elementType, attributeName, attributeValue);
                                            parameters.StringBuilder.AppendLine(
                                                $"{RuntimeHelperClass}.XamlContext_SetConnectionId({parameters.CurrentXamlContext}, {componentId}, {elementUid})");

                                            break;
                                        case MemberTypes.Field:
                                        case MemberTypes.Property:

                                            //------------
                                            // C# PROPERTY
                                            //------------

                                            // Generate the code for instantiating the attribute value:
                                            string value;
                                            if (elementType == $"Global.{KnownNamespaces.SystemWindows}.Setter")
                                            {
                                                //we get the parent Style node (since there is a Style.Setters node that is added, the parent style node is )
                                                if (element.Parent != null && element.Parent.Parent != null && element.Parent.Parent.Name.LocalName == "Style")
                                                {

                                                    if (attributeName == "Property")
                                                    {
                                                        // Style setter property:
                                                        value = GenerateCodeForSetterProperty(element.Parent.Parent, attributeValue); //todo: support attached properties used in a Setter
                                                    }
                                                    else if (attributeName == "Value")
                                                    {
                                                        var property = element.Attribute("Property");
                                                        if (property != null)
                                                        {
                                                            bool isSetterForAttachedProperty = property.Value.Contains('.');
                                                            XName name = GetCSharpXNameFromTargetTypeOrAttachedPropertyString(element, isSetterForAttachedProperty);
                                                            //string str = GetCSharpFullTypeNameFromTargetTypeString(styleNode, reflectionOnSeparateAppDomain);
                                                            //string[] s = {"::"};
                                                            //string[] splittedStr = str.Split(s, StringSplitOptions.RemoveEmptyEntries);
                                                            //string[] splittedTypeName = splittedStr[splittedStr.Length - 1].Split('.');
                                                            //XName typeName = XName.Get(splittedTypeName[splittedTypeName.Length - 1], splittedStr[0]); 
                                                            string propertyName = isSetterForAttachedProperty ? property.Value.Split('.')[1] : property.Value;
                                                            value = GenerateCodeForInstantiatingAttributeValue(name,
                                                                propertyName,
                                                                isSetterForAttachedProperty,
                                                                attributeValue,
                                                                element);
                                                        }
                                                        else
                                                            throw new XamlParseException(@"The <Setter> element must declare a ""Property"" attribute.");
                                                    }
                                                    else
                                                        throw new XamlParseException(@"The <Setter> element cannot have attributes other than ""Property"" and ""Value"".");
                                                }
                                                else
                                                    throw new XamlParseException(@"""<Setter/>"" tags can only be declared inside a <Style/>.");
                                            }
                                            else if (elementType == $"Global.{KnownNamespaces.SystemWindowsData}.Binding"
                                                && memberName == "Path")
                                            {
                                                if (TryResolvePathForBinding(attributeValue, element, out string resolvedPath))
                                                {
                                                    string xamlPath = _settings.SystemTypes.ConvertToString(resolvedPath);
                                                    parameters.StringBuilder.AppendLine($"{elementUid}.XamlPath = {xamlPath}");
                                                }

                                                XName typeName = element.Name;
                                                string propertyName = attribute.Name.LocalName;

                                                value = GenerateCodeForInstantiatingAttributeValue(
                                                    typeName,
                                                    propertyName,
                                                    isAttachedMember,
                                                    attributeValue,
                                                    element);
                                            }
                                            else if (elementType == $"Global.{KnownNamespaces.SystemWindows}.TemplateBindingExtension"
                                                && memberName == "Path")
                                            {
                                                ResolvePathForTemplateBinding(attributeValue, element, out string typeName, out string propertyName);
                                                parameters.StringBuilder.AppendLine(
                                                    $"{elementUid}.DependencyPropertyName = {_settings.SystemTypes.ConvertToString(propertyName)}");
                                                if (typeName != null)
                                                {
                                                    parameters.StringBuilder.AppendLine(
                                                        $"{elementUid}.DependencyPropertyOwnerType = GetType({typeName})");
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

                                                value =
                                                    GenerateCodeForInstantiatingAttributeValue(
                                                        typeName,
                                                        propertyName,
                                                        isAttachedMember,
                                                        attributeValue,
                                                        element
                                                    );
                                            }

                                            // Append the statement:
                                            if (value != null)
                                            {
                                                parameters.StringBuilder.AppendLine($"{elementUid}.{attributeName} = {value}");
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

                            XName ownerTypeXName = attribute.Name.Namespace + split[0];
                            string memberName = split[1];

                            GettingInformationAboutXamlTypes.GetClrNamespaceAndLocalName(
                                ownerTypeXName,
                                out string ownerTypeNamespace,
                                out string ownerTypeName,
                                out string ownerTypeAssemblyName);

                            (MemberTypes memberType, MethodDefinition method, TypeReference declaringType) =
                                _settings.Inspector.GetAttachedMemberType(
                                    memberName, ownerTypeNamespace, ownerTypeName, ownerTypeAssemblyName);

                            switch (memberType)
                            {
                                case MemberTypes.Property:
                                    {
                                        string ownerType = $"Global.{declaringType.ConvertToString(SupportedLanguage.VBNet)}";
                                        string value = GenerateCodeForInstantiatingAttributeValue(
                                            ownerTypeXName,
                                            memberName,
                                            isAttachedMember,
                                            attributeValue,
                                            element);

                                        parameters.StringBuilder.AppendLine(
                                            $"{ownerType}.Set{memberName}({elementUid}, {value})");
                                    }
                                    break;

                                case MemberTypes.Event:
                                    {
                                        string ownerType = $"Global.{declaringType.ConvertToString(SupportedLanguage.VBNet)}";

                                        int componentId = parameters.ComponentConnector.ConnectAttachedEventHandler(elementType, ownerType, memberName, attributeValue);
                                        parameters.StringBuilder.AppendLine(
                                            $"{RuntimeHelperClass}.XamlContext_SetConnectionId({parameters.CurrentXamlContext}, {componentId}, {elementUid})");
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

                if (isInNewScope)
                {
                    parameters.PopScope();
                }
            }

            private void WriteEventSetter(GeneratorContext parameters)
            {
                XElement eventSetter = _reader.ObjectData.Element;

                // we get the parent Style node (since there is a Style.Setters node that is added, the parent style node is )
                if (eventSetter.Parent is null || eventSetter.Parent.Parent is null || eventSetter.Parent.Parent.Name.LocalName != "Style")
                {
                    throw new XamlParseException("\"<EventSetter/>\" tags can only be declared inside a <Style/>.");
                }

                XElement style = eventSetter.Parent.Parent;
                XAttribute eventAttribute = eventSetter.Attribute("Event"); // required
                XAttribute handlerAttribute = eventSetter.Attribute("Handler"); // required
                XAttribute handledEventsTooAttribute = eventSetter.Attribute("HandledEventsToo");

                if (eventAttribute is null || handlerAttribute is null)
                {
                    throw new XamlParseException("\"EventSetter\" must declare an \"Event\" and a \"Handler\".", eventSetter);
                }

                // First, find the event
                string eventName, namespaceName, typeName, assemblyName;

                string eventAttributeValue = GetAttributeValue(eventAttribute);

                int index = eventAttributeValue.IndexOf('.');
                if (index >= 0)
                {
                    GetClrNamespaceAndLocalName(eventAttributeValue.Substring(0, index), eventSetter, out namespaceName, out typeName, out assemblyName);
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
                        GetClrNamespaceAndLocalName(targetType.Value, style, out namespaceName, out typeName, out assemblyName);
                    }
                    else
                    {
                        namespaceName = KnownNamespaces.SystemWindows;
                        typeName = "FrameworkElement";
                        assemblyName = "OpenSilver";
                    }
                }

                string handlerTypeString;

                TypeDefinition ownerType = _settings.Inspector.GetTypeDefinition(namespaceName, typeName, assemblyName);
                string ownerTypeString = ownerType.ConvertToString(SupportedLanguage.VBNet);

                if (_settings.Inspector.GetEvent(ownerType, eventName, false) is EventDefinition eventDefinition)
                {
                    handlerTypeString = eventDefinition.EventType.ConvertToString(SupportedLanguage.VBNet);
                }
                else if (_settings.Inspector.GetMethod(ownerType, $"Add{eventName}Handler", false, true) is MethodDefinition addHandlerMethodDefinition &&
                    addHandlerMethodDefinition.Parameters.Count == 2)
                {
                    handlerTypeString = addHandlerMethodDefinition.Parameters[1].ParameterType.ConvertToString(SupportedLanguage.VBNet);
                }
                else
                {
                    throw new XamlParseException($"Cannot find the Style Event '{eventName}' on the type '{ownerTypeString}'.", eventSetter);
                }

                // Start generating the code
                int componentId = parameters.ComponentConnector.ConnectEventSetterHandler("Global." + handlerTypeString, GetAttributeValue(handlerAttribute));

                string eventSetterName = GeneratingCode.GetUniqueName(eventSetter);

                parameters.StringBuilder.AppendLine(
                    $"Dim {eventSetterName} = {RuntimeHelperClass}.XamlContext_WriteStartObject({parameters.CurrentXamlContext}, New Global.{KnownNamespaces.SystemWindows}.EventSetter())");

                parameters.StringBuilder.AppendLine(
                    $"{eventSetterName}.Event = {RuntimeHelperClass}.RoutedEventFromName(\"{eventName}\", GetType(Global.{ownerTypeString}))");

                parameters.StringBuilder.AppendLine(
                    $"{RuntimeHelperClass}.XamlContext_SetConnectionId({parameters.CurrentXamlContext}, {componentId}, {eventSetterName})");

                if (handledEventsTooAttribute is not null)
                {
                    string value = _settings.SystemTypes.ConvertToBoolean(GetAttributeValue(handledEventsTooAttribute));
                    parameters.StringBuilder.AppendLine($"{eventSetterName}.HandledEventsToo = {value}");
                }

                parameters.StringBuilder.AppendLine($"{RuntimeHelperClass}.XamlContext_WriteEndObject({parameters.CurrentXamlContext})");
            }

            private void OnWriteEndObject(GeneratorContext parameters)
            {
                parameters.StringBuilder.AppendLine($"{RuntimeHelperClass}.XamlContext_WriteEndObject({parameters.CurrentXamlContext})");
            }

            private void OnWriteStartMember(GeneratorContext parameters)
            {
                XElement element = _reader.MemberData.Target;
                XElement member = _reader.MemberData.Member;

                int idx = member.Name.LocalName.IndexOf('.');
                string typeName = member.Name.LocalName.Substring(0, idx);
                string propertyName = member.Name.LocalName.Substring(idx + 1);

                if (_settings.Inspector.IsFrameworkTemplateTemplateProperty(propertyName, member.Name.NamespaceName, typeName))
                {
                    if (member.Elements().Count() > 1)
                    {
                        throw new XamlParseException("A FrameworkTemplate cannot have more than one child.", element);
                    }

                    string frameworkTemplateName = GeneratingCode.GetUniqueName(element);

                    var scope = new FrameworkTemplateScope(frameworkTemplateName, GeneratingCode.GetUniqueName(member.Elements().First()));

                    parameters.StringBuilder.AppendLine($"{RuntimeHelperClass}.SetTemplateContent({frameworkTemplateName}, {parameters.CurrentXamlContext}, AddressOf {scope.MethodName})");

                    parameters.PushScope(scope);
                }
            }

            private void OnWriteEndMember(GeneratorContext parameters)
            {
                XElement element = _reader.MemberData.Member;

                // Get the namespace, local name, and optional assembly that correspond to the element:
                GetClrNamespaceAndLocalName(element.Name, out _, out _, out string assemblyNameIfAny);

                // Get information about the parent element (to which the property applies) and the element itself:
                XElement parent = element.Parent;
                string parentUid = GeneratingCode.GetUniqueName(parent);
                string typeName = element.Name.LocalName.Split('.')[0];
                string propertyName = element.Name.LocalName.Split('.')[1];
                XName elementName = element.Name.Namespace + typeName; // eg. if the element is <VisualStateManager.VisualStateGroups>, this will be "DefaultNamespace+VisualStateManager"

                if (_settings.Inspector.IsFrameworkTemplateTemplateProperty(propertyName, element.Name.NamespaceName, typeName))
                {
                    // TODO move call to RuntimeHelpers.SetTemplateContent(...) here
                    parameters.PopScope();
                }
                else
                {
                    XElement child = _reader.MemberData.Value;
                    string childUid = GeneratingCode.GetUniqueName(child);

                    bool isAttachedProperty = IsPropertyAttached(element);

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
                                assemblyNameIfAny);

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
                                parameters.StringBuilder.AppendLine($"CType({codeToAccessTheEnumerable}, Global.System.Collections.IDictionary).Add(GetType({childKey}), {childUid})");
                            }
                            else if (isImplicitDataTemplate)
                            {
                                string key = $"New Global.{KnownNamespaces.SystemWindows}.DataTemplateKey(GetType({childKey}))";

                                parameters.StringBuilder.AppendLine($"CType({codeToAccessTheEnumerable}, Global.System.Collections.IDictionary).Add({key}, {childUid})");
                            }
                            else
                            {
                                parameters.StringBuilder.AppendLine($"CType({codeToAccessTheEnumerable}, Global.System.Collections.IDictionary).Add(\"{childKey}\", {childUid})");
                            }
                        }
                        else
                        {
                            parameters.StringBuilder.AppendLine($"CType({codeToAccessTheEnumerable}, Global.System.Collections.IList).Add({childUid})");
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
                                string elementType = _settings.Inspector.GetCSharpEquivalentOfXamlTypeAsString(elementName.Namespace.NamespaceName, elementName.LocalName, assemblyNameIfAny);
                                parameters.StringBuilder.AppendLine(
                                    $"{elementType}.Set{propertyName}({parentUid}, {childUid})"); // eg. MyCustomGridClass.SetRow(grid32877267T6, int45628789434);
                            }
                            else
                            {
                                parameters.StringBuilder.AppendLine($"{parentUid}.{propertyName} = {childUid}");
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
                                        element.Name.NamespaceName, splittedLocalName[0], assemblyNameIfAny);

                                    _settings.Inspector.GetPropertyOrFieldTypeInfo(
                                        propertyName,
                                        element.Name.NamespaceName,
                                        splittedLocalName[0],
                                        out propertyNamespaceName,
                                        out propertyLocalTypeName,
                                        out _,
                                        out _,
                                        assemblyNameIfAny,
                                        isAttached: true
                                    );

                                    string propertyType = GetFullTypeName(propertyNamespaceName, propertyLocalTypeName);
                                    parameters.StringBuilder.AppendLine(
                                        $"{elementType}.Set{propertyName}({parentUid}, CType({RuntimeHelperClass}.CallProvideValue({parameters.CurrentXamlContext}, {childUid}), {propertyType}))");
                                }
                                else
                                {
                                    _settings.Inspector.GetPropertyOrFieldTypeInfo(
                                        propertyName,
                                        parent.Name.Namespace.NamespaceName,
                                        parent.Name.LocalName,
                                        out propertyNamespaceName,
                                        out propertyLocalTypeName,
                                        out _,
                                        out _,
                                        assemblyNameIfAny,
                                        isAttached: false
                                    );

                                    string propertyType = GetFullTypeName(propertyNamespaceName, propertyLocalTypeName);
                                    parameters.StringBuilder.AppendLine(
                                        $"{parentUid}.{propertyName} = CType({RuntimeHelperClass}.CallProvideValue({parameters.CurrentXamlContext}, {childUid}), {propertyType})");
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
                                        _settings.AssemblyName) != null;

                                string propertyDeclaringTypeName;
                                string propertyTypeNamespace;
                                string propertyTypeName;
                                if (!isAttachedProperty)
                                {
                                    _settings.Inspector.GetPropertyOrFieldInfo(propertyName,
                                                                                         parent.Name.Namespace.NamespaceName,
                                                                                         parent.Name.LocalName,
                                                                                         out propertyDeclaringTypeName,
                                                                                         out propertyTypeNamespace,
                                                                                         out propertyTypeName,
                                                                                         assemblyNameIfAny,
                                                                                         false);
                                }
                                else
                                {
                                    _settings.Inspector.GetAttachedPropertyGetMethodInfo("Get" + propertyName,
                                        elementName.Namespace.NamespaceName,
                                        elementName.LocalName,
                                        out propertyDeclaringTypeName,
                                        out propertyTypeNamespace,
                                        out propertyTypeName,
                                        assemblyNameIfAny);
                                }
                                string propertyTypeFullName = (!string.IsNullOrEmpty(propertyTypeNamespace) ? propertyTypeNamespace + "." : "") + propertyTypeName;

                                // Check if the property is of type "Binding/MultiBinding" (or "BindingBase"), in which 
                                // case we should directly assign the value instead of calling "SetBinding"
                                bool isPropertyOfTypeBinding =
                                    propertyTypeFullName == $"Global.{KnownNamespaces.SystemWindowsData}.{child.Name.LocalName}" ||
                                    propertyTypeFullName == $"Global.{KnownNamespaces.SystemWindowsData}.BindingBase";

                                if (isPropertyOfTypeBinding || !isDependencyProperty)
                                {
                                    parameters.StringBuilder.AppendLine($"{parentUid}.{propertyName} = {childUid}");
                                }
                                else
                                {
                                    string dpFullName = $"{propertyDeclaringTypeName}.{propertyName}Property";
                                    parameters.StringBuilder.AppendLine(
                                        $"Global.{KnownNamespaces.SystemWindowsData}.BindingOperations.SetBinding({parentUid}, {dpFullName}, {childUid})");
                                }
                            }
                            else if (GeneratingCode.IsDynamicResourceExtension(child))
                            {
                                //------------------------------
                                // {DynamicResource}
                                //------------------------------

                                string dependencyPropertyName =
                                    _settings.Inspector.GetField(
                                        propertyName + "Property",
                                        isAttachedProperty ? elementName.Namespace.NamespaceName : parent.Name.Namespace.NamespaceName,
                                        isAttachedProperty ? elementName.LocalName : parent.Name.LocalName,
                                        _settings.AssemblyName);

                                string propertyDeclaringTypeName;
                                string propertyTypeNamespace;
                                string propertyTypeName;
                                if (!isAttachedProperty)
                                {
                                    _settings.Inspector.GetPropertyOrFieldInfo(propertyName,
                                        parent.Name.Namespace.NamespaceName,
                                        parent.Name.LocalName,
                                        out propertyDeclaringTypeName,
                                        out propertyTypeNamespace,
                                        out propertyTypeName,
                                        assemblyNameIfAny,
                                        false);
                                }
                                else
                                {
                                    _settings.Inspector.GetAttachedPropertyGetMethodInfo("Get" + propertyName,
                                        elementName.Namespace.NamespaceName,
                                        elementName.LocalName,
                                        out propertyDeclaringTypeName,
                                        out propertyTypeNamespace,
                                        out propertyTypeName,
                                        assemblyNameIfAny);
                                }

                                if (dependencyPropertyName is null)
                                {
                                    string elementType = _settings.Inspector.GetCSharpEquivalentOfXamlTypeAsString(
                                        elementName.Namespace.NamespaceName,
                                        elementName.LocalName,
                                        assemblyNameIfAny);

                                    if (elementType == $"Global.{KnownNamespaces.SystemWindows}.Setter" && propertyName == "Value")
                                    {
                                        parameters.StringBuilder.AppendLine(
                                            $"{parentUid}.{propertyName} = {childUid}");
                                    }
                                    else
                                    {
                                        throw new XamlParseException(
                                            $"A 'DynamicResourceExtension' cannot be set on the '{propertyName}' property of type '{elementType.Substring("Global.".Length)}'. A 'DynamicResourceExtension' can only be set on a DependencyProperty of a DependencyObject, or the Setter.Value property.",
                                            element);
                                    }
                                }
                                else
                                {
                                    string markupValue = GeneratingUniqueNames.GenerateUniqueName("tmp");
                                    string propertyTypeFullName = string.IsNullOrEmpty(propertyTypeNamespace) ?
                                        $"Global.{propertyTypeName}" :
                                        $"Global.{propertyTypeNamespace}.{propertyTypeName}";

                                    parameters.StringBuilder
                                        .AppendLine($"Dim {markupValue} As Object = Nothing")
                                        .AppendLine($"If Not {RuntimeHelperClass}.TrySetMarkupExtension({parentUid}, {dependencyPropertyName}, {childUid}, {markupValue})");

                                    if (!isAttachedProperty)
                                    {
                                        parameters.StringBuilder
                                            .AppendLine($"    {parentUid}.{propertyName} = CType({markupValue}, {propertyTypeFullName})");
                                    }
                                    else
                                    {
                                        parameters.StringBuilder
                                            .AppendLine($"    {propertyDeclaringTypeName}.Set{propertyName}({parentUid}, CType({markupValue}, {propertyTypeFullName}))");
                                    }

                                    parameters.StringBuilder.AppendLine("End If");
                                }
                            }
                            else if (child.Name.LocalName == "TemplateBindingExtension")
                            {
                                var dpName =
                                    _settings.Inspector.GetField(
                                        propertyName + "Property",
                                        isAttachedProperty ? elementName.Namespace.NamespaceName : parent.Name.Namespace.NamespaceName,
                                        isAttachedProperty ? elementName.LocalName : parent.Name.LocalName,
                                        _settings.AssemblyName);

                                parameters.StringBuilder.AppendLine(
                                    $"{parentUid}.SetValue({dpName}, {RuntimeHelperClass}.CallProvideValue({parameters.CurrentXamlContext}, {childUid}))");
                            }
                            else if (GeneratingCode.IsNullExtension(child))
                            {
                                //------------------------------
                                // {x:Null}
                                //------------------------------

                                if (isAttachedProperty)
                                {
                                    string elementType = _settings.Inspector.GetCSharpEquivalentOfXamlTypeAsString(elementName.Namespace.NamespaceName, elementName.LocalName, assemblyNameIfAny);
                                    parameters.StringBuilder.AppendLine($"{elementType}.Set{propertyName}({parentUid}, Nothing)");
                                }
                                else
                                {
                                    parameters.StringBuilder.AppendLine($"{parentUid}.{propertyName} = Nothing");
                                }
                                //todo-perfs: avoid generating the line "var NullExtension_cfb65e0262594ddb87d60d8e776ce142 = new Global.System.Windows.Markup.NullExtension();", which is never used. Such a line is generated when the user code contains a {x:Null} markup extension.
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
                                        out string propertyTypeNS,
                                        out string propertyTypeName,
                                        out _,
                                        out _,
                                        assemblyNameIfAny,
                                        isAttached: true);

                                    string type = _settings.Inspector.GetCSharpEquivalentOfXamlTypeAsString(
                                        elementName.Namespace.NamespaceName,
                                        elementName.LocalName,
                                        assemblyNameIfAny);

                                    parameters.StringBuilder.AppendLine(
                                        $"{type}.Set{propertyName}({parentUid}, CType(CType({staticMemberName}, Object), {GetFullTypeName(propertyTypeNS, propertyTypeName)}))");
                                }
                                else
                                {
                                    _settings.Inspector.GetPropertyOrFieldTypeInfo(
                                        propertyName,
                                        parent.Name.NamespaceName,
                                        parent.Name.LocalName,
                                        out string propertyTypeNS,
                                        out string propertyTypeName,
                                        out _,
                                        out _,
                                        assemblyNameIfAny,
                                        isAttached: false);

                                    parameters.StringBuilder.AppendLine(
                                        $"{parentUid}.{propertyName} = CType(CType({staticMemberName}, Object), {GetFullTypeName(propertyTypeNS, propertyTypeName)})");
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
                                        out string propertyTypeNS,
                                        out string propertyTypeName,
                                        out _,
                                        out _,
                                        assemblyNameIfAny,
                                        isAttached: true);

                                    string type = _settings.Inspector.GetCSharpEquivalentOfXamlTypeAsString(
                                        elementName.Namespace.NamespaceName,
                                        elementName.LocalName,
                                        assemblyNameIfAny);

                                    parameters.StringBuilder.AppendLine(
                                        $"{type}.Set{propertyName}({parentUid}, CType(CType(GetType(Global.{resolvedTypeName}), Object), {GetFullTypeName(propertyTypeNS, propertyTypeName)}))");
                                }
                                else
                                {
                                    _settings.Inspector.GetPropertyOrFieldTypeInfo(
                                        propertyName,
                                        parent.Name.NamespaceName,
                                        parent.Name.LocalName,
                                        out string propertyTypeNS,
                                        out string propertyTypeName,
                                        out _,
                                        out _,
                                        assemblyNameIfAny,
                                        isAttached: false);

                                    parameters.StringBuilder.AppendLine(
                                        $"{parentUid}.{propertyName} = CType(CType(GetType(Global.{resolvedTypeName}), Object), {GetFullTypeName(propertyTypeNS, propertyTypeName)})");
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
                                    out string propertyTypeNS,
                                    out string propertyTypeName,
                                    out _,
                                    out _,
                                    assemblyNameIfAny,
                                    isAttachedProperty);

                                string dpName = _settings.Inspector.GetField(
                                    propertyName + "Property",
                                    propertyOwnerTypeNS,
                                    propertyOwnerTypeName,
                                    _settings.AssemblyName);

                                if (dpName != null)
                                {
                                    string markupValue = GeneratingUniqueNames.GenerateUniqueName("tmp");
                                    string propertyTypeFullName = GetFullTypeName(propertyTypeNS, propertyTypeName);

                                    parameters.StringBuilder
                                        .AppendLine($"Dim {markupValue} As Object = Nothing")
                                        .AppendLine($"If Not {RuntimeHelperClass}.TrySetMarkupExtension({parentUid}, {dpName}, {childUid}, {markupValue})");

                                    if (isAttachedProperty)
                                    {
                                        string elementType = _settings.Inspector.GetCSharpEquivalentOfXamlTypeAsString(
                                            propertyOwnerTypeNS, propertyOwnerTypeName, assemblyNameIfAny);

                                        parameters.StringBuilder
                                            .AppendLine($"    {elementType}.Set{propertyName}({parentUid}, CType({markupValue}, {propertyTypeFullName}))");
                                    }
                                    else
                                    {
                                        parameters.StringBuilder
                                            .AppendLine($"    {parentUid}.{propertyName} = CType({markupValue}, {propertyTypeFullName})");
                                    }

                                    parameters.StringBuilder.AppendLine("End If");
                                }
                                else
                                {
                                    if (isAttachedProperty)
                                    {
                                        string elementType = _settings.Inspector.GetCSharpEquivalentOfXamlTypeAsString(
                                            propertyOwnerTypeNS, propertyOwnerTypeName, assemblyNameIfAny);

                                        string markupExtension = 
                                            $"CType({childUid},{IMarkupExtensionClass}).ProvideValue(New Global.System.ServiceProvider({parentUid}, Nothing))";

                                        parameters.StringBuilder.AppendLine(
                                            $"{elementType}.Set{propertyName}({parentUid}, CType({markupExtension},{GetFullTypeName(propertyTypeNS, propertyTypeName)})");
                                    }
                                    else
                                    {
                                        parameters.StringBuilder.AppendLine(
                                            $"{parentUid}.{propertyName} = CType((CType({childUid}, {IMarkupExtensionClass}).ProvideValue(New Global.System.ServiceProvider({parentUid}, Nothing)), {GetFullTypeName(propertyTypeNS, propertyTypeName)})");
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
                        parameters.StringBuilder.AppendLine($"CType({targetUid}, Global.System.Collections.IDictionary).Add(GetType({childKey}), {childUid})");
                    }
                    else if (isImplicitDataTemplate)
                    {
                        string key = $"New Global.{KnownNamespaces.SystemWindows}.DataTemplateKey(GetType({childKey}))";

                        parameters.StringBuilder.AppendLine($"CType({targetUid}, Global.System.Collections.IDictionary).Add({key}, {childUid})");
                    }
                    else
                    {
                        parameters.StringBuilder.AppendLine($"CType({targetUid}, Global.System.Collections.IDictionary).Add(\"{childKey}\", {childUid})");
                    }
                }
                else
                {
                    parameters.StringBuilder.AppendLine($"CType({targetUid}, Global.System.Collections.IList).Add({childUid})");
                }
            }

            private void TryCatch(Action<GeneratorContext> method, GeneratorContext parameters)
            {
                try
                {
                    method(parameters);
                }
                catch (XamlParseException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    IXmlLineInfo info = _reader.ObjectData?.Element ?? _reader.MemberData?.Member;

                    throw new XamlParseException(
                        "An unexpected error occurred, see inner exception for more details.",
                        info,
                        ex);
                }
            }

            private XElement GetRootOfCurrentNamescopeForRuntime(XElement element)
            {
                XElement currentElement = element;
                while (currentElement.Parent != null)
                {
                    int index = currentElement.Parent.Name.LocalName.IndexOf(".");
                    if (index > -1)
                    {
                        string namespaceName = currentElement.Parent.Name.NamespaceName;
                        string typeName = currentElement.Parent.Name.LocalName.Substring(0, index);
                        string propertyName = currentElement.Parent.Name.LocalName.Substring(index + 1);

                        if (_settings.Inspector.IsFrameworkTemplateTemplateProperty(propertyName, namespaceName, typeName))
                        {
                            return currentElement;
                        }
                    }

                    currentElement = currentElement.Parent;
                }

                return currentElement;
            }

            private bool IsElementTheRootElement(XElement element)
            {
                return (element == _reader.Document.Root);
            }

            private bool IsClassTheApplicationClass(string className)
            {
                return className == $"Global.{KnownNamespaces.SystemWindows}.Application";
            }

            private bool IsResourceDictionaryCreatedFromSource(XElement element)
            {
                if (element.Attribute("Source") != null)
                {
                    return _settings.Inspector.IsResourceDictionarySourcePropertyVisible(
                        element.Name.NamespaceName, element.Name.LocalName);
                }

                return false;
            }

            private string GenerateCodeForSetterProperty(XElement styleElement, string attributeValue)
            {
                bool isAttachedProperty = attributeValue.Contains(".");
                string elementType, dependencyPropertyName;
                bool hasNamespace;
                string namespaceName, propertyName;
                // Check for namespace/prefix
                if (attributeValue.Contains(':'))
                {
                    hasNamespace = true;
                    string[] splittedAttributeValue = attributeValue.Split(':');
                    namespaceName = splittedAttributeValue[0];
                    propertyName = splittedAttributeValue[1];
                }
                else
                {
                    hasNamespace = false;
                    namespaceName = "";
                    propertyName = attributeValue;
                }

                if (isAttachedProperty)
                {
                    string[] splittedAttachedProperty = propertyName.Split('.');
                    string propertyFullXamlTypeName = namespaceName + (hasNamespace ? ":" : "") + splittedAttachedProperty[0];
                    GetClrNamespaceAndLocalName(propertyFullXamlTypeName,
                        styleElement,
                        out string elementNamespaceName,
                        out string elementLocalTypeName,
                        out string _);
                    elementType = _settings.Inspector.GetCSharpEquivalentOfXamlTypeAsString(
                        elementNamespaceName,
                        elementLocalTypeName);

                    dependencyPropertyName = splittedAttachedProperty[1] + "Property";
                }
                else
                {
                    elementType = GetCSharpFullTypeNameFromTargetTypeString(styleElement);
                    dependencyPropertyName = attributeValue + "Property"; //todo: handle the case where the DependencyProperty name is not the name of the property followed by "Property" (at least improve the error message)
                }
                return $"{elementType}.{dependencyPropertyName}";
            }

            private XName GetCSharpXNameFromTargetTypeOrAttachedPropertyString(XElement setterElement, bool isAttachedProperty)
            {
                string namespaceName;
                string localTypeName;
                string assemblyNameIfAny;
                XAttribute attributeToLookAt;
                XElement currentXElement;
                if (isAttachedProperty)
                {
                    currentXElement = setterElement;
                    attributeToLookAt = currentXElement.Attribute("Property");
                    if (attributeToLookAt == null)
                        throw new XamlParseException("Setter must declare a Property.");
                }
                else
                {
                    currentXElement = setterElement.Parent.Parent;
                    attributeToLookAt = currentXElement.Attribute("TargetType");
                    if (attributeToLookAt == null)
                        throw new XamlParseException("Style must declare a TargetType.");
                }

                string attributeTypeString;
                // attribute has a namespace or a prefix
                if (attributeToLookAt.Value.Contains(':'))
                {
                    string[] splittedValue = attributeToLookAt.Value.Split(':');

                    if (isAttachedProperty)
                    {
                        if (splittedValue[1].Contains('.'))
                        {
                            attributeTypeString = splittedValue[0] + ":" + splittedValue[1].Split('.')[0];
                        }
                        else
                        {
                            throw new XamlParseException(@"Namespaces or prefixes must be followed by a type.");
                        }
                    }
                    else
                    {
                        attributeTypeString = attributeToLookAt.Value;
                    }
                }
                else
                {
                    attributeTypeString = attributeToLookAt.Value.Split('.')[0];
                }

                GetClrNamespaceAndLocalName(attributeTypeString, currentXElement, out namespaceName, out localTypeName, out assemblyNameIfAny);
                return _settings.Inspector.GetCSharpEquivalentOfXamlTypeAsXName(namespaceName, localTypeName, assemblyNameIfAny);
            }

            private string GetCSharpFullTypeNameFromTargetTypeString(XElement styleElement, bool isDataType = false)
            {
                var targetTypeAttribute = styleElement.Attribute(isDataType ? "DataType" : "TargetType");
                if (targetTypeAttribute == null)
                    throw new XamlParseException(isDataType ? "DataTemplate must declare a DataType or have a key." : "Style must declare a TargetType.");

                GetClrNamespaceAndLocalName(targetTypeAttribute.Value,
                    styleElement,
                    out string namespaceName,
                    out string localTypeName,
                    out string assemblyNameIfAny);
                string elementType = _settings.Inspector.GetCSharpEquivalentOfXamlTypeAsString(
                    namespaceName,
                    localTypeName,
                    assemblyNameIfAny,
                    ifTypeNotFoundTryGuessing: false);

                return elementType;
            }

            private string GetCSharpFullTypeName(string typeString, XElement elementWhereTheTypeIsUsed)
            {
                GetClrNamespaceAndLocalName(typeString,
                    elementWhereTheTypeIsUsed,
                    out string namespaceName,
                    out string localTypeName,
                    out string assemblyNameIfAny);
                string elementType = _settings.Inspector.GetCSharpEquivalentOfXamlTypeAsString(
                    namespaceName,
                    localTypeName,
                    assemblyNameIfAny,
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
                    throw new XamlParseException("Each dictionary entry must have an associated key. The element named '" + element.Name.LocalName + "' does not have a key.");
                }
            }

            private string GenerateCodeForInstantiatingAttributeValue(
                XName xName,
                string propertyName,
                bool isAttachedProperty,
                string value,
                XElement elementWhereTheTypeIsUsed)
            {
                GetClrNamespaceAndLocalName(
                    xName,
                    out string namespaceName,
                    out string localTypeName,
                    out string assemblyNameIfAny);

                string valueNamespaceName, valueLocalTypeName, valueAssemblyName;
                bool isValueEnum;

                if (isAttachedProperty)
                {
                    _settings.Inspector.GetMethodReturnValueTypeInfo(
                        "Get" + propertyName,
                        namespaceName,
                        localTypeName,
                        out valueNamespaceName,
                        out valueLocalTypeName,
                        out valueAssemblyName,
                        out isValueEnum,
                        assemblyNameIfAny);
                }
                else
                {
                    _settings.Inspector.GetPropertyOrFieldTypeInfo(
                        propertyName,
                        namespaceName,
                        localTypeName,
                        out valueNamespaceName,
                        out valueLocalTypeName,
                        out valueAssemblyName,
                        out isValueEnum,
                        assemblyNameIfAny);
                }

                string valueTypeFullName = GetFullTypeName(valueNamespaceName, valueLocalTypeName);
                bool isKnownSystemType = _settings.SystemTypes.IsKnownType(
                        valueTypeFullName.Substring("Global.".Length), valueAssemblyName);
                bool isKnownCoreType = _settings.CoreTypes.IsKnownType(
                    valueTypeFullName.Substring("Global.".Length), valueAssemblyName);

                // Generate the code or instantiating the attribute
                if (isValueEnum && !isKnownSystemType && !isKnownCoreType)
                {
                    //----------------------------
                    // PROPERTY IS AN ENUM
                    //----------------------------

                    TypeDefinition enumType = _settings.Inspector.GetTypeDefinition(
                        valueNamespaceName,
                        valueLocalTypeName,
                        valueAssemblyName);

                    return string.Join(" | ", _settings.Inspector.GetEnumValues(
                        enumType,
                        value.Trim(),
                        true,
                        true));
                }
                else if (valueTypeFullName == "Global.System.Type")
                {
                    string typeFullName = GetCSharpFullTypeName(value, elementWhereTheTypeIsUsed);

                    return $"GetType({typeFullName})";
                }
                else
                {
                    //----------------------------
                    // PROPERTY IS OF ANOTHER TYPE
                    //----------------------------

                    ChangeRelativePathIntoAbsolutePathIfNecessary(
                        ref value,
                        valueTypeFullName,
                        propertyName,
                        xName);

                    if (isAttachedProperty)
                    {
                        return ConvertFromInvariantString(
                            value, elementWhereTheTypeIsUsed, valueTypeFullName, isKnownCoreType, isKnownSystemType);
                    }
                    else
                    {
                        string declaringTypeName = _settings.Inspector.GetCSharpEquivalentOfXamlTypeAsString(
                            namespaceName, localTypeName, assemblyNameIfAny);
                        string fallbackValue = ConvertFromInvariantString(
                            value, elementWhereTheTypeIsUsed, valueTypeFullName, isKnownCoreType, isKnownSystemType);

                        return XamlContextGetPropertyValue(
                            declaringTypeName, propertyName, value, valueTypeFullName, fallbackValue);
                    }
                }
            }

            private void ChangeRelativePathIntoAbsolutePathIfNecessary(ref string path,
                string valueTypeFullName,
                string propertyName,
                XName parentXName)
            {
                // In the case of the "Frame" control, a relative URI to a ".xaml" file (used for navigation) should not be changed into an absolute URI, because it is relative to the Startup assembly, not to the current assembly where the value is defined:
                bool IsFrameOrUriMappingSpecialCase =
                    parentXName.LocalName == "UriMapping"
                    || parentXName.LocalName == "Frame"
                    || parentXName.LocalName == "HyperlinkButton";

                // We change relative paths into absolute paths in case of <Image> controls and other controls that have the "Source" property:
                if ((valueTypeFullName == $"Global.{KnownNamespaces.SystemWindowsMedia}.ImageSource"
                    || valueTypeFullName == "Global.System.Uri"
                    || (propertyName == "FontFamily" && path.Contains('.')))
                    && !IsFrameOrUriMappingSpecialCase
                    && !path.ToLower().EndsWith(".xaml")) // Note: this is to avoid messing with Frame controls, which paths are always relative to the startup assembly (in SL).
                {
                    if (!IsUriAbsolute(path) // This lines checks if the URI is in the form "ms-appx://" or "http://" or "https://" or "mailto:..." etc.
                        && !path.ToLower().Contains(@";component/")) // This line checks if the URI is in the form "/assemblyName;component/FolderName/FileName.xaml"
                    {
                        // Get the relative path of the current XAML file:
                        string relativePathOfTheCurrentFile = Path.GetDirectoryName(_fileNameWithPathRelativeToProjectRoot.Replace('\\', '/'));

                        // Combine the relative path of the current file with the path specified by the user:
                        string pathRelativeToProjectRoot = Path.Combine(relativePathOfTheCurrentFile.Replace('\\', '/'), path.Replace('\\', '/')).Replace('\\', '/');

                        // Surround the path with the assembly name to make it an absolute path in the form: "/assemblyName;component/FolderName/FileName.xaml"
                        path = $"/{_settings.AssemblyName};component/{pathRelativeToProjectRoot}";
                    }
                }
            }

            private static bool IsUriAbsolute(string path)
            {
                if (path.StartsWith("~"))
                {
                    return true;
                }

                int index = path.IndexOf(':');
                if (index >= 0)
                {
                    string scheme = path.Substring(0, index);
                    return Uri.CheckSchemeName(scheme);
                }

                return false;
            }

            private bool TryResolvePathForBinding(string path, XElement element, out string resolvedPath)
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
                        namespaceName, localTypeName, assemblyName
                    );

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
                        typeName = GetCSharpEquivalentOfXamlTypeAsString(XName.Get(type, xmlNamespace.NamespaceName));
                    }
                    catch { }
                }
            }

            private string ConvertFromInvariantString(string value, XElement context, string type, bool isKnownCoreType, bool isKnownSystemType)
            {
                type = type.Substring("Global.".Length);

                if (_settings.SystemTypes.IsNullableType(type, null, out string underlyingType))
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        return "Nothing";
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
                return $"{RuntimeHelperClass}.GetPropertyValue(Of {propertyType})(GetType({propertyDeclaringType}), {EscapeString(propertyName)}, {EscapeString(value)}, Function() {fallbackValue})";
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
                return string.Concat("\"", stringValue.Replace("\"", "\"\""), "\"");
            }

            private static string GetAttributeValue(XAttribute attribute)
            {
                string value = attribute.Value;

                if (value is not null && value.StartsWith("{}"))
                {
                    return value.Substring(2);
                }

                return value;
            }

            private bool IsPropertyAttached(XElement propertyElement)
            {
                GetClrNamespaceAndLocalName(propertyElement.Name, out string namespaceName, out string localName, out string assemblyNameIfAny);
                if (localName.Contains("."))
                {
                    var split = localName.Split('.');
                    var typeLocalName = split[0];
                    var propertyOrFieldName = split[1];
                    GetClrNamespaceAndLocalName(propertyElement.Parent.Name, out string parentNamespaceName, out string parentLocalTypeName, out string parentAssemblyIfAny);
                    return _settings.Inspector.IsPropertyAttached(propertyOrFieldName, namespaceName, typeLocalName, parentNamespaceName, parentLocalTypeName, assemblyNameIfAny);
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
                    return _settings.Inspector.DoesMethodReturnACollection(methodName, namespaceName, localName, assemblyNameIfAny);
                }
                else
                {
                    var propertyOrFieldName = propertyElement.Name.LocalName.Split('.')[1];
                    var parentElement = propertyElement.Parent;
                    GetClrNamespaceAndLocalName(parentElement.Name, out string parentNamespaceName, out string parentLocalName, out string parentAssemblyNameIfAny);
                    return _settings.Inspector.IsPropertyOrFieldACollection(propertyOrFieldName, parentNamespaceName, parentLocalName, parentAssemblyNameIfAny);
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

                return !IsTypeAssignableFrom(child.Name, element.Name, isAttachedProperty) &&
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
                    return _settings.Inspector.DoesMethodReturnADictionary(methodName, namespaceName, localName, assemblyNameIfAny);
                }
                else
                {
                    var propertyOrFieldName = propertyElement.Name.LocalName.Split('.')[1];
                    var parentElement = propertyElement.Parent;
                    GetClrNamespaceAndLocalName(parentElement.Name, out string parentNamespaceName, out string parentLocalName, out string parentAssemblyNameIfAny);
                    return _settings.Inspector.IsPropertyOrFieldADictionary(propertyOrFieldName, parentNamespaceName, parentLocalName, parentAssemblyNameIfAny);
                }
            }

            private bool IsElementADictionary(XElement element)
            {
                GetClrNamespaceAndLocalName(element.Name, out string elementNameSpace, out string elementLocalName, out string assemblyNameIfAny);
                return _settings.Inspector.IsElementADictionary(elementNameSpace, elementLocalName, assemblyNameIfAny);
            }

            private bool IsElementAMarkupExtension(XElement element)
            {
                GetClrNamespaceAndLocalName(element.Name, out string elementNameSpace, out string elementLocalName, out string assemblyNameIfAny);
                return _settings.Inspector.IsElementAMarkupExtension(elementNameSpace, elementLocalName, assemblyNameIfAny);
            }

            private bool IsTypeAssignableFrom(XName elementOfTypeToAssignFrom, XName elementOfTypeToAssignTo, bool isAttached = false)
            {
                GetClrNamespaceAndLocalName(elementOfTypeToAssignFrom, out string nameSpaceOfTypeToAssignFrom, out string nameOfTypeToAssignFrom, out string assemblyNameOfTypeToAssignFrom);
                GetClrNamespaceAndLocalName(elementOfTypeToAssignTo, out string nameSpaceOfTypeToAssignTo, out string nameOfTypeToAssignTo, out string assemblyNameOfTypeToAssignTo);
                return _settings.Inspector.IsTypeAssignableFrom(nameSpaceOfTypeToAssignFrom, nameOfTypeToAssignFrom, assemblyNameOfTypeToAssignFrom, nameSpaceOfTypeToAssignTo, nameOfTypeToAssignTo, assemblyNameOfTypeToAssignTo, isAttached);
            }

            private static void GetClrNamespaceAndLocalName(
                string typeAsString,
                XElement element,
                out string namespaceName,
                out string localName,
                out string assemblyNameIfAny)
            {
                GettingInformationAboutXamlTypes.GetClrNamespaceAndLocalName(
                    typeAsString,
                    element,
                    out namespaceName,
                    out localName,
                    out assemblyNameIfAny);
            }

            private static void GetClrNamespaceAndLocalName(XName xName, out string namespaceName, out string localName, out string assemblyNameIfAny)
                => GettingInformationAboutXamlTypes.GetClrNamespaceAndLocalName(
                    xName,
                    out namespaceName,
                    out localName,
                    out assemblyNameIfAny);

            private string GetCSharpEquivalentOfXamlTypeAsString(
                XName xName,
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
                    ifTypeNotFoundTryGuessing);
            }

            private string GetCSharpEquivalentOfXamlTypeAsString(
                XName xName,
                out string namespaceName,
                out string typeName,
                out string assemblyName)
                => GetCSharpEquivalentOfXamlTypeAsString(xName, false, out namespaceName, out typeName, out assemblyName);

            private string GetCSharpEquivalentOfXamlTypeAsString(XName xName, bool ifTypeNotFoundTryGuessing = false)
                => GetCSharpEquivalentOfXamlTypeAsString(xName, ifTypeNotFoundTryGuessing, out _, out _, out _);

            private string ResolveStaticExtension(XElement element)
            {
                if (element.Attribute("Member") is not XAttribute member)
                {
                    throw new XamlParseException("StaticExtension must have Member property set.");
                }

                string fieldString;
                string typeNameForError = null;
                TypeDefinition type;

                if (element.Attribute("MemberType") is XAttribute typeAttribute)
                {
                    type = GetTypeDefinitionFromString(element, typeAttribute.Value);
                    fieldString = member.Value;
                    typeNameForError = type.ConvertToString(SupportedLanguage.VBNet);
                }
                else
                {
                    int dotIndex = member.Value.IndexOf('.');
                    if (dotIndex < 0)
                    {
                        throw new XamlParseException($"'{member.Value}' StaticExtension value cannot be resolved to an enumeration, static field, or static property");
                    }

                    // Pull out the type substring (this will include any XML prefix, e.g. "av:Button")
                    string typeString = member.Value.Substring(0, dotIndex);
                    if (string.IsNullOrEmpty(typeString))
                    {
                        throw new XamlParseException($"'{member.Value}' StaticExtension value cannot be resolved to an enumeration, static field, or static property");
                    }

                    type = GetTypeDefinitionFromString(element, typeString);

                    // Get the member name substring.
                    fieldString = member.Value.Substring(dotIndex + 1, member.Value.Length - dotIndex - 1);
                    if (string.IsNullOrEmpty(typeString))
                    {
                        throw new XamlParseException($"'{member.Value}' StaticExtension value cannot be resolved to an enumeration, static field, or static property");
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
                    return $"Global.{declaringType.ConvertToString(SupportedLanguage.VBNet)}.{staticField.Name}";
                }

                (staticProperty, declaringType) = _settings.Inspector.GetProperty(type, fieldString, true, true);

                if (staticProperty is not null)
                {
                    return $"Global.{declaringType.ConvertToString(SupportedLanguage.VBNet)}.{staticProperty.Name}";
                }

                throw new XamlParseException(
                    $"'{(typeNameForError is not null ? $"{typeNameForError}.{member.Value}" : member.Value)}' StaticExtension value cannot be resolved to an enumeration, static field, or static property");
            }

            private string ResolveTypeExtension(XElement element)
            {
                if (element.Attribute("Type") is XAttribute typeAttribute)
                {
                    return GetTypeDefinitionFromString(element, typeAttribute.Value).ConvertToString(SupportedLanguage.VBNet);
                }

                if (element.Attribute("TypeName") is not XAttribute typeNameAttribute)
                {
                    throw new XamlParseException("TypeExtension must have TypeName property set.");
                }

                return GetTypeDefinitionFromString(element, typeNameAttribute.Value).ConvertToString(SupportedLanguage.VBNet);
            }

            private TypeDefinition GetTypeDefinitionFromString(XElement element, string value)
            {
                Debug.Assert(value is not null);

                GetClrNamespaceAndLocalName(value, element, out string namespaceName, out string typeName, out string assemblyName);

                return _settings.Inspector.GetTypeDefinition(namespaceName, typeName, assemblyName);
            }
        }
    }
}
