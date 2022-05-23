
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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using OpenSilver.Internal;

namespace DotNetForHtml5.Compiler
{
    internal static partial class GeneratingCSharpCode
    {
        private class GeneratorPass2 : ICodeGenerator
        {
            private abstract class GeneratorScope
            {
                private readonly Dictionary<string, string> _namescope;

                protected GeneratorScope(string rootElement, bool createNameScope)
                {
                    Root = rootElement;
                    XamlContext = GeneratingUniqueNames.GenerateUniqueNameFromString("xamlContext");
                    
                    if (createNameScope)
                    {
                        _namescope = new Dictionary<string, string>();
                    }
                }

                public string Root { get; }

                public string XamlContext { get; }

                public StringBuilder StringBuilder { get; } = new StringBuilder();

                protected abstract string ToStringCore();

                public void RegisterName(string name, string scopedElement)
                {
                    if (_namescope == null) return;
                    
                    _namescope.Add(name, scopedElement);
                }

                public sealed override string ToString() => ToStringCore();

                private protected void AppendNamescope(StringBuilder builder)
                {
                    if (_namescope == null) return;

                    builder.AppendLine($"{RuntimeHelperClass}.InitializeNameScope({Root});");

                    foreach (var kp in _namescope)
                    {
                        builder.AppendLine($"{RuntimeHelperClass}.RegisterName({Root}, {EscapeString(kp.Key)}, {kp.Value});");
                    }
                }
            }

            private class RootScope : GeneratorScope
            {
                public RootScope(string rootElementName, bool createNameScope) 
                    : base(rootElementName, createNameScope) 
                {
                    StringBuilder.AppendLine($"var {XamlContext} = {RuntimeHelperClass}.Create_XamlContext();");
                }

                protected override string ToStringCore()
                {
                    StringBuilder builder = new StringBuilder();

                    builder.Append(StringBuilder.ToString());
                    AppendNamescope(builder);

                    return builder.ToString();
                }
            }

            private class FrameworkTemplateScope : GeneratorScope
            {
                private readonly IMetadata _metadata;

                public FrameworkTemplateScope(string templateName, string templateRoot, IMetadata metadata)
                    : base(templateRoot, true)
                {
                    _metadata = metadata;
                    Name = templateName;
                    TemplateOwner = $"templateOwner_{templateName}";
                    MethodName = $"Create_{templateName}";
                }

                public string Name { get; }

                public string TemplateOwner { get; }

                public string MethodName { get; }

                protected override string ToStringCore()
                {
                    StringBuilder builder = new StringBuilder();

                    builder.AppendLine($"private static global::{_metadata.SystemWindowsNS}.FrameworkElement {MethodName}(global::{_metadata.SystemWindowsNS}.FrameworkElement {TemplateOwner}, {XamlContextClass} {XamlContext})")
                        .AppendLine("{")
                        .Append(StringBuilder.ToString());
                    AppendNamescope(builder);
                    builder.AppendLine($"return {Root};")
                        .AppendLine("}");

                    return builder.ToString();
                }
            }

            private class GeneratorContext
            {
                private readonly Stack<GeneratorScope> _scopes = new Stack<GeneratorScope>();

                public readonly List<string> ResultingMethods = new List<string>();
                public readonly List<string> ResultingFieldsForNamedElements = new List<string>();
                public readonly List<string> ResultingFindNameCalls = new List<string>();
                public readonly ComponentConnectorBuilder ComponentConnector = new ComponentConnectorBuilder();

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

            private const string TemplateOwnerValuePlaceHolder = "TemplateOwnerValuePlaceHolder";

            private readonly XamlReader _reader;
            private readonly IMetadata _metadata;

            private readonly string _sourceFile;
            private readonly string _fileNameWithPathRelativeToProjectRoot;
            private readonly string _assemblyNameWithoutExtension;
            private readonly ReflectionOnSeparateAppDomainHandler _reflectionOnSeparateAppDomain;
            private readonly bool _isSLMigration;
            private readonly string _codeToPutInTheInitializeComponentOfTheApplicationClass;
            private readonly ILogger _logger;

            public GeneratorPass2(XDocument doc,
                string sourceFile,
                string fileNameWithPathRelativeToProjectRoot,
                string assemblyNameWithoutExtension,
                ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain,
                bool isSLMigration,
                string codeToPutInTheInitializeComponentOfTheApplicationClass,
                ILogger logger)
            {
                _reader = new XamlReader(doc);
                _metadata = isSLMigration ? Metadata.Silverlight : Metadata.UWP;
                _sourceFile = sourceFile;
                _fileNameWithPathRelativeToProjectRoot = fileNameWithPathRelativeToProjectRoot;
                _assemblyNameWithoutExtension = assemblyNameWithoutExtension;
                _reflectionOnSeparateAppDomain = reflectionOnSeparateAppDomain;
                _isSLMigration = isSLMigration;
                _codeToPutInTheInitializeComponentOfTheApplicationClass = codeToPutInTheInitializeComponentOfTheApplicationClass;
                _logger = logger;
            }

            public string Generate() => GenerateImpl(new GeneratorContext());

            private string GenerateImpl(GeneratorContext parameters)
            {
                parameters.GenerateFieldsForNamedElements = 
                    !_reflectionOnSeparateAppDomain.IsAssignableFrom(
                        _metadata.SystemWindowsNS, "ResourceDictionary",
                        _reader.Document.Root.Name.NamespaceName, _reader.Document.Root.Name.LocalName) 
                    &&
                    !_reflectionOnSeparateAppDomain.IsAssignableFrom(
                        _metadata.SystemWindowsNS, "Application",
                        _reader.Document.Root.Name.NamespaceName, _reader.Document.Root.Name.LocalName);

                parameters.PushScope(
                    new RootScope(GetUniqueName(_reader.Document.Root),
                        _reflectionOnSeparateAppDomain.IsAssignableFrom(
                            _metadata.SystemWindowsNS, "FrameworkElement",
                            _reader.Document.Root.Name.NamespaceName, _reader.Document.Root.Name.LocalName)
                    )
                );

                // Traverse the tree in "post order" (ie. start with child elements then traverse parent elements):
                while (_reader.Read())
                {
                    switch (_reader.NodeType)
                    {
                        case XamlNodeType.StartObject:
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
                string className, namespaceStringIfAny, baseType;
                bool hasCodeBehind;
                GetClassInformationFromXaml(_reader.Document, _reflectionOnSeparateAppDomain,
                    out className, out namespaceStringIfAny, out baseType, out hasCodeBehind);

                if (hasCodeBehind)
                {
                    string connectMethod = parameters.ComponentConnector.ToString();
                    string initializeComponentMethod = CreateInitializeComponentMethod(
                        $"global::{_metadata.SystemWindowsNS}.Application",
                        IsClassTheApplicationClass(baseType) ? _codeToPutInTheInitializeComponentOfTheApplicationClass : string.Empty,
                        _assemblyNameWithoutExtension,
                        _fileNameWithPathRelativeToProjectRoot,
                        parameters.ResultingFindNameCalls);

                    // Wrap everything into a partial class:
                    string partialClass = GeneratePartialClass(initializeComponentMethod,
                                                               connectMethod,
                                                               parameters.ResultingFieldsForNamedElements,
                                                               className,
                                                               namespaceStringIfAny,
                                                               baseType,
#if BRIDGE
                                                           addApplicationEntryPoint: IsClassTheApplicationClass(baseType)
#else
                                                           addApplicationEntryPoint: false
#endif
);

                    string componentTypeFullName = GetFullTypeName(namespaceStringIfAny, className);

                    string factoryClass = GenerateFactoryClass(
                        componentTypeFullName,
                        GetUniqueName(_reader.Document.Root),
                        parameters.CurrentScope.ToString(),
                        $"return ({componentTypeFullName})global::CSHTML5.Internal.TypeInstantiationHelper.Instantiate(typeof({componentTypeFullName}));",
                        parameters.ResultingMethods,
                        $"global::{_metadata.SystemWindowsNS}.UIElement",
                        _assemblyNameWithoutExtension,
                        _fileNameWithPathRelativeToProjectRoot);

                    string finalCode = $@"
{factoryClass}
{partialClass}";

                    return finalCode;
                }
                else
                {
                    string rootElementName = GetUniqueName(_reader.Document.Root);

                    string finalCode = GenerateFactoryClass(
                        baseType,
                        rootElementName,
                        parameters.CurrentScope.ToString(),
                        string.Join(Environment.NewLine, $"var {rootElementName} = new {baseType}();", $"LoadComponentImpl({rootElementName});", $"return {rootElementName};"),
                        parameters.ResultingMethods,
                        $"global::{_metadata.SystemWindowsNS}.UIElement",
                        _assemblyNameWithoutExtension,
                        _fileNameWithPathRelativeToProjectRoot);

                    return finalCode;
                }
            }

            private void OnWriteStartObject(GeneratorContext parameters)
            {
                XElement element = _reader.ObjectData.Element;

                // Get the namespace, local name, and optional assembly that correspond to the element:
                string namespaceName, localTypeName, assemblyNameIfAny;
                GettingInformationAboutXamlTypes.GetClrNamespaceAndLocalName(element.Name, out namespaceName, out localTypeName, out assemblyNameIfAny);

                // Get information about which element holds the namescope of the current element. For example, if the current element is inside a DataTemplate, the DataTemplate is the root of the namescope of the current element. If the element is not inside a DataTemplate or ControlTemplate, the root of the XAML is the root of the namescope of the current element.
                XElement elementThatIsRootOfTheCurrentNamescope = GetRootOfCurrentNamescopeForRuntime(element);
                bool isElementInRootNamescope = (elementThatIsRootOfTheCurrentNamescope.Parent == null);

                // Check if the element is the root element:
                string elementTypeInCSharp =
                    _reflectionOnSeparateAppDomain.GetCSharpEquivalentOfXamlTypeAsString(
                        namespaceName, localTypeName, assemblyNameIfAny
                    );

                bool isRootElement = IsElementTheRootElement(element);
                bool isKnownSystemType = SystemTypesHelper.IsSupportedSystemType(
                    elementTypeInCSharp.Substring("global::".Length), assemblyNameIfAny
                );
                bool isInitializeTypeFromString =
                    element.Attribute(InsertingImplicitNodes.InitializedFromStringAttribute) != null;
                bool isResourceDictionary = IsResourceDictionary(element);
                bool isResourceDictionaryReferencedBySourceURI =
                    isResourceDictionary && element.Attribute("Source") != null;

                // Add the constructor (in case of object) or a direct initialization (in case
                // of system type or "isInitializeFromString" or referenced ResourceDictionary)
                // (unless this is the root element)
                string elementUniqueNameOrThisKeyword = GetUniqueName(element);

                bool flag = false;
                if (!isRootElement)
                {
                    flag = true;
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
                            directContent = GetDefaultValueOfTypeAsString(
                                namespaceName, localTypeName, isKnownSystemType, _reflectionOnSeparateAppDomain, assemblyNameIfAny
                            );
                        }

                        parameters.StringBuilder.AppendLine(
                            string.Format(
                                "{1} {0} = {3}.XamlContext_PushScope({4}, {2});",
                                elementUniqueNameOrThisKeyword,
                                elementTypeInCSharp,
                                SystemTypesHelper.ConvertFromInvariantString(directContent, elementTypeInCSharp.Substring("global::".Length)),
                                RuntimeHelperClass,
                                parameters.CurrentXamlContext
                            )
                        );
                    }
                    else if (isInitializeTypeFromString)
                    {
                        //------------------------------------------------
                        // Add the type initialization from string:
                        //------------------------------------------------

                        string stringValue = element.Attribute(InsertingImplicitNodes.InitializedFromStringAttribute).Value;

                        bool isKnownCoreType = CoreTypesHelper.IsSupportedCoreType(
                            elementTypeInCSharp.Substring("global::".Length), assemblyNameIfAny
                        );

                        string preparedValue = ConvertFromInvariantString(
                            stringValue, elementTypeInCSharp, isKnownCoreType, isKnownSystemType
                        );

                        parameters.StringBuilder.AppendLine(
                            string.Format("var {0} = {2}.XamlContext_PushScope({3}, {1});", 
                                elementUniqueNameOrThisKeyword, 
                                preparedValue,
                                RuntimeHelperClass,
                                parameters.CurrentXamlContext
                            )
                        );

                    }
                    else if (isResourceDictionaryReferencedBySourceURI)
                    {
                        //------------------------------------------------
                        // Add the type initialization from "Source" URI:
                        //------------------------------------------------
                        string sourceUri = element.Attribute("Source").Value; // Note: this attribute exists because we have checked earlier.
                        string absoluteSourceUri = PathsHelper.ConvertToAbsolutePathWithComponentSyntax(
                            sourceUri,
                            _fileNameWithPathRelativeToProjectRoot,
                            _assemblyNameWithoutExtension);

                        parameters.StringBuilder.AppendLine(
                            string.Format(
                                "var {0} = {3}.XamlContext_PushScope({4}, (({1})new {2}()).CreateComponent());",
                                elementUniqueNameOrThisKeyword,
                                $"{IXamlComponentFactoryClass}<global::{_metadata.SystemWindowsNS}.ResourceDictionary>",
                                XamlResourcesHelper.GenerateClassNameFromComponentUri(absoluteSourceUri),
                                RuntimeHelperClass,
                                parameters.CurrentXamlContext
                            )
                        );
                    }
                    else
                    {
                        //------------------------------------------------
                        // Add the type constructor:
                        //------------------------------------------------
                        parameters.StringBuilder.AppendLine(string.Format("var {0} = {2}.XamlContext_PushScope({3}, new {1}());", 
                            elementUniqueNameOrThisKeyword, 
                            elementTypeInCSharp,
                            RuntimeHelperClass,
                            parameters.CurrentXamlContext));
                    }
                }
                
                if (!flag)
                {
                    parameters.StringBuilder.AppendLine($"_ = {RuntimeHelperClass}.XamlContext_PushScope({parameters.CurrentXamlContext}, {elementUniqueNameOrThisKeyword});");
                }

                // Set templated parent if any
                if (parameters.CurrentScope is FrameworkTemplateScope scope)
                {
                    if (_reflectionOnSeparateAppDomain.IsAssignableFrom(_metadata.SystemWindowsNS, "FrameworkElement", element.Name.NamespaceName, element.Name.LocalName))
                    {
                        parameters.StringBuilder.AppendLine($"{RuntimeHelperClass}.SetTemplatedParent({elementUniqueNameOrThisKeyword}, {scope.TemplateOwner});");
                    }
                }

                // Add the attributes:
                foreach (XAttribute attribute in element.Attributes())
                {
                    //-------------
                    // ATTRIBUTE
                    //-------------

                    string attributeValue = attribute.Value;
                    string attributeLocalName = attribute.Name.LocalName;

                    // Skip the attributes "GeneratingUniqueNames.UniqueNameAttribute" and "InitializedFromStringAttribute":
                    if (!IsReservedAttribute(attributeLocalName)
                        && !attribute.IsNamespaceDeclaration)
                    {
                        // Verify that the attribute is not an attached property:
                        //todo: This test does not work 100% of the time. For example if we have <Grid Column="1" ..../> the compiler thinks that Column is a normal property whereas it actually is an attached property.
                        bool isAttachedProperty = attributeLocalName.Contains(".");
                        if (!isAttachedProperty)
                        {
                            if (IsAttributeTheXNameAttribute(attribute))
                            {
                                //-------------
                                // x:Name (or "Name")
                                //-------------

                                string name = attributeValue;

                                // Add the code to register the name, etc.
                                if (isElementInRootNamescope && parameters.GenerateFieldsForNamedElements)
                                {
                                    string fieldModifier = _metadata.FieldModifier;
                                    XAttribute attr = element.Attribute(xNamespace + "FieldModifier");
                                    if (attr != null)
                                    {
                                        fieldModifier = (attr.Value ?? "").ToLower();
                                    }

                                    // add '@' to handle cases where x:Name is a forbidden word (for instance 'this'
                                    // or any other c# keyword)
                                    string fieldName = "@" + name;
                                    parameters.ResultingFieldsForNamedElements.Add(string.Format("{0} {1} {2};", fieldModifier, elementTypeInCSharp, fieldName));
                                    parameters.ResultingFindNameCalls.Add($"this.{fieldName} = (({elementTypeInCSharp})(this.FindName(\"{name}\")));");
                                }

                                // We also set the Name property on the object itself, if the XAML was "Name=..." or (if the XAML was x:Name=... AND the Name property exists in the object).    (Note: setting the Name property on the object is useful for example in <VisualStateGroup Name="Pressed"/> where the parent control looks at the name of its direct children:
                                bool isNamePropertyRatherThanXColonNameProperty = string.IsNullOrEmpty(attribute.Name.NamespaceName); // This is used to distinguish between "Name" and "x:Name"
                                if (isNamePropertyRatherThanXColonNameProperty || _reflectionOnSeparateAppDomain.DoesTypeContainNameMemberOfTypeString(namespaceName, localTypeName, assemblyNameIfAny))
                                {
                                    parameters.StringBuilder.AppendLine(string.Format("{0}.Name = \"{1}\";", elementUniqueNameOrThisKeyword, name));
                                }

                                parameters.CurrentScope.RegisterName(name, elementUniqueNameOrThisKeyword);
                                //todo: throw an exception when both "x:Name" and "Name" are specified in the XAML.

                            }
                            else if (IsEventTriggerRoutedEventProperty(element.Name, attributeLocalName))
                            {
                                // TODO Check that 'attributeLocalName' is effectively the LoadedEvent routed event.
                                // Silverlight only allows the FrameworkElement.LoadedEvent as value for the EventTrigger.RoutedEvent
                                // property, so for now we assume the xaml is always valid.
                                parameters.StringBuilder.AppendLine($"{elementUniqueNameOrThisKeyword}.RoutedEvent = global::{_metadata.SystemWindowsNS}.FrameworkElement.LoadedEvent;");
                            }
                            else if (string.IsNullOrEmpty(attribute.Name.NamespaceName))
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
                                    MemberTypes memberType = _reflectionOnSeparateAppDomain.GetMemberType(memberName, namespaceName, localTypeName, assemblyNameIfAny);
                                    switch (memberType)
                                    {
                                        case MemberTypes.Event:

                                            //------------
                                            // C# EVENT
                                            //------------

                                            string eventHandlerType = _reflectionOnSeparateAppDomain.GetEventHandlerType(
                                                memberName, namespaceName, localTypeName, assemblyNameIfAny
                                            );

                                            parameters.StringBuilder.AppendLine(
                                                string.Format("{0}.XamlContext_SetConnectionId({1}, {2}, {3});",
                                                    RuntimeHelperClass,
                                                    parameters.CurrentXamlContext,
                                                    parameters.ComponentConnector.Connect(elementTypeInCSharp, attributeLocalName, eventHandlerType, attributeValue),
                                                    elementUniqueNameOrThisKeyword)
                                            );

                                            break;
                                        case MemberTypes.Field:
                                        case MemberTypes.Property:

                                            //------------
                                            // C# PROPERTY
                                            //------------

                                            // Generate the code for instantiating the attribute value:
                                            string codeForInstantiatingTheAttributeValue;
                                            if (elementTypeInCSharp == $"global::{_metadata.SystemWindowsNS}.Setter")
                                            {
                                                //we get the parent Style node (since there is a Style.Setters node that is added, the parent style node is )
                                                if (element.Parent != null && element.Parent.Parent != null && element.Parent.Parent.Name.LocalName == "Style")
                                                {

                                                    if (attributeLocalName == "Property")
                                                    {
                                                        // Style setter property:
                                                        codeForInstantiatingTheAttributeValue = GenerateCodeForSetterProperty(element.Parent.Parent, attributeValue); //todo: support attached properties used in a Setter
                                                    }
                                                    else if (attributeLocalName == "Value")
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
                                                            codeForInstantiatingTheAttributeValue = GenerateCodeForInstantiatingAttributeValue(name,
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
                                            else if (elementTypeInCSharp == $"global::{_metadata.SystemWindowsDataNS}.Binding"
                                                && memberName == "Path")
                                            {
                                                if (TryResolvePathForBinding(attributeValue, element, out string resolvedPath))
                                                {
                                                    parameters.StringBuilder.AppendLine(
                                                        string.Format("{0}.XamlPath = {1};",
                                                            elementUniqueNameOrThisKeyword,
                                                            SystemTypesHelper.ConvertFromInvariantString(resolvedPath, "System.String")
                                                        )
                                                    );
                                                }

                                                XName typeName = element.Name;
                                                string propertyName = attribute.Name.LocalName;

                                                codeForInstantiatingTheAttributeValue =
                                                    GenerateCodeForInstantiatingAttributeValue(
                                                        typeName,
                                                        propertyName,
                                                        isAttachedProperty,
                                                        attributeValue,
                                                        element
                                                    );
                                            }
                                            else
                                            {
                                                //------------
                                                // NORMAL C# PROPERTY
                                                //------------

                                                XName typeName = element.Name;
                                                string propertyName = attribute.Name.LocalName;

                                                codeForInstantiatingTheAttributeValue =
                                                    GenerateCodeForInstantiatingAttributeValue(
                                                        typeName,
                                                        propertyName,
                                                        isAttachedProperty,
                                                        attributeValue,
                                                        element
                                                    );
                                            }

                                            // Append the statement:
                                            if (codeForInstantiatingTheAttributeValue != null)
                                            {
                                                parameters.StringBuilder.AppendLine(
                                                    string.Format(
                                                        "{0}.{1} = {2};",
                                                        elementUniqueNameOrThisKeyword, attributeLocalName, codeForInstantiatingTheAttributeValue
                                                    )
                                                );
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
                            // ATTACHED PROPERTY
                            //-------------

                            // Split the attribute name:
                            string[] splitted = attribute.Name.LocalName.Split('.');
                            string classLocalNameForAttachedProperty = splitted[0];
                            XName elementNameForAttachedProperty = attribute.Name.Namespace + classLocalNameForAttachedProperty;
                            string attachedPropertyTypeNamespaceName, attachedPropertyTypeLocalName, attachedPropertyTypeAssemblyNameIfAny;
                            GettingInformationAboutXamlTypes.GetClrNamespaceAndLocalName(elementNameForAttachedProperty, out attachedPropertyTypeNamespaceName, out attachedPropertyTypeLocalName, out attachedPropertyTypeAssemblyNameIfAny);
                            string classFullNameForAttachedProperty = _reflectionOnSeparateAppDomain.GetCSharpEquivalentOfXamlTypeAsString(attachedPropertyTypeNamespaceName, attachedPropertyTypeLocalName, attachedPropertyTypeAssemblyNameIfAny);
                            string propertyName = splitted[1];

                                // Generate the code for instantiating the attribute value:
                            string codeForInstantiatingTheAttributeValue = GenerateCodeForInstantiatingAttributeValue(
                                elementNameForAttachedProperty,
                                propertyName,
                                isAttachedProperty,
                                attributeValue,
                                element);

                            // Append the statement:
                            parameters.StringBuilder.AppendLine(string.Format("{0}.Set{1}({2},{3});", classFullNameForAttachedProperty, propertyName, elementUniqueNameOrThisKeyword, codeForInstantiatingTheAttributeValue));
                        }
                    }
                }
            }

            private void OnWriteEndObject(GeneratorContext parameters)
            {
                parameters.StringBuilder.AppendLine($"{RuntimeHelperClass}.XamlContext_PopScope({parameters.CurrentXamlContext});");
            }

            private void OnWriteStartMember(GeneratorContext parameters)
            {
                XElement element = _reader.MemberData.Target;
                XElement member = _reader.MemberData.Member;

                int idx = member.Name.LocalName.IndexOf('.');
                string typeName = member.Name.LocalName.Substring(0, idx);
                string propertyName = member.Name.LocalName.Substring(idx + 1);

                if (propertyName == "ContentPropertyUsefulOnlyDuringTheCompilation" &&
                    _reflectionOnSeparateAppDomain.IsAssignableFrom(_metadata.SystemWindowsNS, "FrameworkTemplate", member.Name.NamespaceName, typeName))
                {
                    if (member.Elements().Count() > 1)
                    {
                        throw new XamlParseException("A FrameworkTemplate cannot have more than one child.", element);
                    }

                    string frameworkTemplateName = GetUniqueName(element);

                    FrameworkTemplateScope scope = new FrameworkTemplateScope(
                        frameworkTemplateName,
                        GetUniqueName(member.Elements().First()),
                        _metadata);

                    parameters.StringBuilder.AppendLine($"{RuntimeHelperClass}.SetTemplateContent({frameworkTemplateName}, {parameters.CurrentXamlContext}, {scope.MethodName});");

                    parameters.PushScope(scope);
                }
            }

            private void OnWriteEndMember(GeneratorContext parameters)
            {
                XElement element = _reader.MemberData.Member;

                // Get the namespace, local name, and optional assembly that correspond to the element:
                string namespaceName, localTypeName, assemblyNameIfAny;
                GettingInformationAboutXamlTypes.GetClrNamespaceAndLocalName(element.Name, out namespaceName, out localTypeName, out assemblyNameIfAny);

                // Get information about the parent element (to which the property applies) and the element itself:
                var parentElement = element.Parent;
                string parentElementUniqueNameOrThisKeyword = GetUniqueName(parentElement);
                string typeName = element.Name.LocalName.Split('.')[0];
                string propertyName = element.Name.LocalName.Split('.')[1];
                XName elementName = element.Name.Namespace + typeName; // eg. if the element is <VisualStateManager.VisualStateGroups>, this will be "DefaultNamespace+VisualStateManager"

                bool isFrameworkTemplateContentProperty = propertyName == "ContentPropertyUsefulOnlyDuringTheCompilation" &&
                   _reflectionOnSeparateAppDomain.IsAssignableFrom(_metadata.SystemWindowsNS, "FrameworkTemplate", element.Name.NamespaceName, typeName);

                if (isFrameworkTemplateContentProperty)
                {
                    // TODO move call to FrameworkTemplate.SetMethodToInstantiateFrameworkTemplate(...) here
                    parameters.PopScope();
                }
                else
                {
                    XElement child = _reader.MemberData.Value;

                    bool isAttachedProperty = GettingInformationAboutXamlTypes.IsPropertyAttached(element, _reflectionOnSeparateAppDomain); //(parentElement.Name != elementName) && !GettingInformationAboutXamlTypes.IsTypeAssignableFrom(parentElement.Name, elementName, reflectionOnSeparateAppDomain); // Note: the comparison includes the namespace. // eg. <Grid><VisualStateManager.VisualStateGroups>...</VisualStateManager.VisualStateGroups></Grid> should return "true", while <n:MyUserControl><UserControl.Resources>...</n:MyUserControl></UserControl.Resources> should return "false".

                    // Check if the property is a collection, in which case we must use ".Add(...)", otherwise a simple "=" is enough:
                    if (GettingInformationAboutXamlTypes.IsPropertyOrFieldACollection(element, _reflectionOnSeparateAppDomain, isAttachedProperty)
                        && (element.Elements().Count() != 1
                        || (!GettingInformationAboutXamlTypes.IsTypeAssignableFrom(element.Elements().First().Name, element.Name, _reflectionOnSeparateAppDomain, isAttached: isAttachedProperty)) // To handle the case where the user explicitly declares the collection element. Example: <Application.Resources><ResourceDictionary><Child x:Key="test"/></ResourceDictionary></Application.Resources> (rather than <Application.Resources><Child x:Key="test"/></Application.Resources>), in which case we need to do "=" instead pf "Add()"
                        && !IsBinding(element.Elements().First())
                        && element.Elements().First().Name.LocalName != "StaticResourceExtension"
                        && element.Elements().First().Name.LocalName != "StaticResource"
                        && element.Elements().First().Name.LocalName != "TemplateBinding"
                        && element.Elements().First().Name.LocalName != "TemplateBindingExtension"))
                    {
                        //------------------------
                        // PROPERTY TYPE IS A COLLECTION
                        //------------------------

                        string codeToAccessTheEnumerable;
                        if (isAttachedProperty)
                        {
                            string elementTypeInCSharp = _reflectionOnSeparateAppDomain.GetCSharpEquivalentOfXamlTypeAsString(
                                elementName.Namespace.NamespaceName,
                                elementName.LocalName,
                                assemblyNameIfAny);

                            codeToAccessTheEnumerable = string.Format(
                                "{0}.Get{1}({2})",
                                elementTypeInCSharp,
                                propertyName,
                                parentElementUniqueNameOrThisKeyword);
                        }
                        else
                        {
                            codeToAccessTheEnumerable = parentElementUniqueNameOrThisKeyword + "." + propertyName;
                        }

                        if (GettingInformationAboutXamlTypes.IsPropertyOrFieldADictionary(element, _reflectionOnSeparateAppDomain, isAttachedProperty))
                        {
                            bool isImplicitStyle;
                            bool isImplicitDataTemplate;
                            string childKey = GetElementXKey(child, out isImplicitStyle, out isImplicitDataTemplate);
                            if (isImplicitStyle)
                            {
                                parameters.StringBuilder.AppendLine($"((global::System.Collections.IDictionary){codeToAccessTheEnumerable}).Add(typeof({childKey}), {GetUniqueName(child)});");
                            }
                            else if (isImplicitDataTemplate)
                            {
                                string key = $"new global::{_metadata.SystemWindowsNS}.DataTemplateKey(typeof({childKey}))";

                                parameters.StringBuilder.AppendLine($"((global::System.Collections.IDictionary){codeToAccessTheEnumerable}).Add({key}, {GetUniqueName(child)});");
                            }
                            else
                            {
                                parameters.StringBuilder.AppendLine($"((global::System.Collections.IDictionary){codeToAccessTheEnumerable}).Add(\"{childKey}\", {GetUniqueName(child)});");
                            }
                        }
                        else
                        {
                            parameters.StringBuilder.AppendLine($"((global::System.Collections.IList){codeToAccessTheEnumerable}).Add({GetUniqueName(child)});");
                        }
                    }
                    else
                    {
                        //------------------------
                        // PROPERTY TYPE IS NOT A COLLECTION
                        //------------------------

                        string childUniqueName = GetUniqueName(child);
                        if (!GettingInformationAboutXamlTypes.IsElementAMarkupExtension(child, _reflectionOnSeparateAppDomain)
                            || (child.Name.LocalName == "RelativeSource")) // Note about "RelativeSource": even though it inherits from "MarkupExtension", we do not was to consider "RelativeSource" as a markup extension for the compilation because it is only meant to be used WITHIN another markup extension (sort of a "nested" markup extension), such as in: "{Binding Background, RelativeSource={RelativeSource Mode=TemplatedParent}}"
                        {
                            if (isAttachedProperty)
                            {
                                string elementTypeInCSharp = _reflectionOnSeparateAppDomain.GetCSharpEquivalentOfXamlTypeAsString(elementName.Namespace.NamespaceName, elementName.LocalName, assemblyNameIfAny);
                                parameters.StringBuilder.AppendLine(string.Format("{0}.Set{1}({2}, {3});", elementTypeInCSharp, propertyName, parentElementUniqueNameOrThisKeyword, childUniqueName)); // eg. MyCustomGridClass.SetRow(grid32877267T6, int45628789434);
                            }
                            else
                            {
                                parameters.StringBuilder.AppendLine(string.Format("{0}.{1} = {2};", parentElementUniqueNameOrThisKeyword, propertyName, childUniqueName));
                            }
                        }
                        else
                        {
                            //------------------------------
                            // MARKUP EXTENSIONS:
                            //------------------------------

                            XElement parent = element.Parent;

                            if (child.Name.LocalName == "StaticResource" || child.Name.LocalName == "StaticResourceExtension" || child.Name.LocalName == "ThemeResourceExtension") //todo: see if there are other elements than StaticResource that need the parents //todo: check namespace as well?
                            {
                                //------------------------------
                                // {StaticResource ...}
                                //------------------------------

                                string[] splittedLocalName = element.Name.LocalName.Split('.');
                                string propertyKey = GettingInformationAboutXamlTypes.GetKeyNameOfProperty(
                                    parent, splittedLocalName[1], _reflectionOnSeparateAppDomain
                                );
                                string propertyKeyString = propertyKey ?? "null";
                                string elementTypeInCSharp;
                                string propertyNamespaceName, propertyLocalTypeName, propertyAssemblyName;
                                bool isTypeString, isTypeEnum;

                                // Attached property
                                if (isAttachedProperty)
                                {
                                    elementTypeInCSharp = _reflectionOnSeparateAppDomain.GetCSharpEquivalentOfXamlTypeAsString(
                                        element.Name.NamespaceName, splittedLocalName[0], assemblyNameIfAny
                                    );

                                    _reflectionOnSeparateAppDomain.GetPropertyOrFieldTypeInfo(
                                        propertyName,
                                        element.Name.NamespaceName,
                                        splittedLocalName[0],
                                        out propertyNamespaceName,
                                        out propertyLocalTypeName,
                                        out propertyAssemblyName,
                                        out isTypeString,
                                        out isTypeEnum,
                                        assemblyNameIfAny,
                                        isAttached: true
                                    );

                                    parameters.StringBuilder.AppendLine(
                                        string.Format(
                                            "{0}.Set{1}({2}, ({3})({4}.CallProvideValue({5}, {6})));",
                                            elementTypeInCSharp,
                                            propertyName,
                                            GetUniqueName(parent),
                                            "global::" + (!string.IsNullOrEmpty(propertyNamespaceName) ? propertyNamespaceName + "." : "") + propertyLocalTypeName,
                                            RuntimeHelperClass,
                                            parameters.CurrentXamlContext,
                                            childUniqueName
                                        )
                                    );
                                }
                                else
                                {
                                    elementTypeInCSharp = _reflectionOnSeparateAppDomain.GetCSharpEquivalentOfXamlTypeAsString(
                                        parent.Name.NamespaceName, parent.Name.LocalName, assemblyNameIfAny
                                    );

                                    _reflectionOnSeparateAppDomain.GetPropertyOrFieldTypeInfo(
                                        propertyName,
                                        parent.Name.Namespace.NamespaceName,
                                        parent.Name.LocalName,
                                        out propertyNamespaceName,
                                        out propertyLocalTypeName,
                                        out propertyAssemblyName,
                                        out isTypeString,
                                        out isTypeEnum,
                                        assemblyNameIfAny,
                                        isAttached: false
                                    );

                                    parameters.StringBuilder.AppendLine(
                                        string.Format(
                                            "{0}.{1} = ({2}){3}.CallProvideValue({4}, {5});",
                                            GetUniqueName(parent),
                                            propertyName,
                                            "global::" + (!string.IsNullOrEmpty(propertyNamespaceName) ? propertyNamespaceName + "." : "") + propertyLocalTypeName,
                                            RuntimeHelperClass,
                                            parameters.CurrentXamlContext,
                                            childUniqueName
                                        )
                                    );
                                }
                            }
                            else if (child.Name.LocalName == "Binding") //todo: verify that the namespace is the one that we used when we added the Binding to the XAML tree?
                            {
                                //------------------------------
                                // {Binding ...}
                                //------------------------------

                                bool isDependencyProperty =
                                    _reflectionOnSeparateAppDomain.GetField(
                                        propertyName + "Property",
                                        isAttachedProperty ? elementName.Namespace.NamespaceName : parent.Name.Namespace.NamespaceName,
                                        isAttachedProperty ? elementName.LocalName : parent.Name.LocalName,
                                        _assemblyNameWithoutExtension) != null;

                                string propertyDeclaringTypeName;
                                string propertyTypeNamespace;
                                string propertyTypeName;
                                bool isTypeString;
                                bool isTypeEnum;
                                if (!isAttachedProperty)
                                {
                                    _reflectionOnSeparateAppDomain.GetPropertyOrFieldInfo(propertyName,
                                                                                         parent.Name.Namespace.NamespaceName,
                                                                                         parent.Name.LocalName,
                                                                                         out propertyDeclaringTypeName,
                                                                                         out propertyTypeNamespace,
                                                                                         out propertyTypeName,
                                                                                         out isTypeString,
                                                                                         out isTypeEnum,
                                                                                         assemblyNameIfAny,
                                                                                         false);
                                }
                                else
                                {
                                    _reflectionOnSeparateAppDomain.GetAttachedPropertyGetMethodInfo("Get" + propertyName,
                                        elementName.Namespace.NamespaceName,
                                        elementName.LocalName,
                                        out propertyDeclaringTypeName,
                                        out propertyTypeNamespace,
                                        out propertyTypeName,
                                        out isTypeString,
                                        out isTypeEnum,
                                        assemblyNameIfAny);
                                }
                                string propertyTypeFullName = (!string.IsNullOrEmpty(propertyTypeNamespace) ? propertyTypeNamespace + "." : "") + propertyTypeName;

                                // Check if the property is of type "Binding" (or "BindingBase"), in which 
                                // case we should directly assign the value instead of calling "SetBinding"
                                bool isPropertyOfTypeBinding = propertyTypeFullName == $"global::{_metadata.SystemWindowsDataNS}.Binding" ||
                                    propertyTypeFullName == $"global::{_metadata.SystemWindowsDataNS}.BindingBase";

                                if (isPropertyOfTypeBinding || !isDependencyProperty)
                                {
                                    parameters.StringBuilder.AppendLine(string.Format("{0}.{1} = {2};", parentElementUniqueNameOrThisKeyword, propertyName, GetUniqueName(child)));
                                }
                                else
                                {
                                    parameters.StringBuilder.AppendLine(string.Format("global::{3}.BindingOperations.SetBinding({0}, {1}, {2});",
                                        parentElementUniqueNameOrThisKeyword, propertyDeclaringTypeName + "." + propertyName + "Property", GetUniqueName(child), _metadata.SystemWindowsDataNS)); //we add the container itself since we couldn't add it inside the while
                                }
                            }
                            else if (child.Name.LocalName == "TemplateBindingExtension")
                            {
                                var dependencyPropertyName =
                                    _reflectionOnSeparateAppDomain.GetField(
                                        propertyName + "Property",
                                        isAttachedProperty ? elementName.Namespace.NamespaceName : parent.Name.Namespace.NamespaceName,
                                        isAttachedProperty ? elementName.LocalName : parent.Name.LocalName,
                                        _assemblyNameWithoutExtension);

                                parameters.StringBuilder.AppendLine(string.Format(
                                    "{0}.SetValue({1}, {2}.ProvideValue(new global::System.ServiceProvider({3}, null)));",
                                    parentElementUniqueNameOrThisKeyword,
                                    dependencyPropertyName,
                                    GetUniqueName(child),
                                    parameters.CurrentScope is FrameworkTemplateScope scope ? scope.TemplateOwner : TemplateOwnerValuePlaceHolder));
                            }
                            else if (child.Name == xNamespace + "NullExtension")
                            {
                                //------------------------------
                                // {x:Null}
                                //------------------------------

                                if (isAttachedProperty)
                                {
                                    string elementTypeInCSharp = _reflectionOnSeparateAppDomain.GetCSharpEquivalentOfXamlTypeAsString(elementName.Namespace.NamespaceName, elementName.LocalName, assemblyNameIfAny);
                                    parameters.StringBuilder.AppendLine(string.Format("{0}.Set{1}({2}, null);", elementTypeInCSharp, propertyName, parentElementUniqueNameOrThisKeyword));
                                }
                                else
                                {
                                    parameters.StringBuilder.AppendLine(string.Format("{0}.{1} = null;", parentElementUniqueNameOrThisKeyword, propertyName));
                                }
                                //todo-perfs: avoid generating the line "var NullExtension_cfb65e0262594ddb87d60d8e776ce142 = new global::System.Windows.Markup.NullExtension();", which is never used. Such a line is generated when the user code contains a {x:Null} markup extension.
                            }
                            else
                            {
                                //------------------------------
                                // Other (custom MarkupExtensions)
                                //------------------------------

                                string propertyKey = GettingInformationAboutXamlTypes.GetKeyNameOfProperty(
                                    parent, element.Name.LocalName.Split('.')[1], _reflectionOnSeparateAppDomain
                                );
                                string propertyKeyString = propertyKey ?? "null";

                                if (isAttachedProperty)
                                {
                                    string elementTypeInCSharp = _reflectionOnSeparateAppDomain.GetCSharpEquivalentOfXamlTypeAsString(
                                        elementName.Namespace.NamespaceName, elementName.LocalName, assemblyNameIfAny
                                    );

                                    string[] splittedLocalName = element.Name.LocalName.Split('.');

                                    string propertyNamespaceName, propertyLocalTypeName, propertyAssemblyName;
                                    bool isTypeString, isTypeEnum;
                                    _reflectionOnSeparateAppDomain.GetPropertyOrFieldTypeInfo(
                                        propertyName,
                                        element.Name.NamespaceName,
                                        splittedLocalName[0],
                                        out propertyNamespaceName,
                                        out propertyLocalTypeName,
                                        out propertyAssemblyName,
                                        out isTypeString,
                                        out isTypeEnum,
                                        assemblyNameIfAny,
                                        true
                                    );

                                    string propertyType = string.Format(
                                        "global::{0}{1}{2}",
                                        propertyNamespaceName,
                                        string.IsNullOrEmpty(propertyNamespaceName) ? string.Empty : ".",
                                        propertyLocalTypeName
                                    );

                                    string markupExtension = string.Format(
                                        "{0}.ProvideValue(new global::System.ServiceProvider({1}, {2}))",
                                        childUniqueName, GetUniqueName(parent), propertyKeyString
                                    );

                                    parameters.StringBuilder.AppendLine(
                                        string.Format("{0}.Set{1}({2}, ({3}){4});",
                                                      elementTypeInCSharp,
                                                      propertyName,
                                                      parentElementUniqueNameOrThisKeyword,
                                                      propertyType,
                                                      markupExtension
                                        )
                                    );
                                }
                                else
                                {
                                    string propertyNamespaceName, propertyLocalTypeName, propertyAssemblyName;
                                    bool isTypeString, isTypeEnum;

                                    // Todo: remove what is irrelevant below:
                                    // Note: the code was copy-pasted from the Binding section from here.
                                    // It is because we need to call SetBinding if a Custom marckup
                                    // expression returns a Binding.
                                    string propertyDeclaringTypeName;
                                    string propertyTypeNamespace;
                                    string propertyTypeName;
                                    _reflectionOnSeparateAppDomain.GetPropertyOrFieldInfo(
                                        propertyName,
                                        parent.Name.Namespace.NamespaceName,
                                        parent.Name.LocalName,
                                        out propertyDeclaringTypeName,
                                        out propertyTypeNamespace,
                                        out propertyTypeName,
                                        out isTypeString,
                                        out isTypeEnum,
                                        assemblyNameIfAny,
                                        false
                                    );

                                    _reflectionOnSeparateAppDomain.GetPropertyOrFieldTypeInfo(
                                        propertyName,
                                        parent.Name.Namespace.NamespaceName,
                                        parent.Name.LocalName,
                                        out propertyNamespaceName,
                                        out propertyLocalTypeName,
                                        out propertyAssemblyName,
                                        out isTypeString,
                                        out isTypeEnum,
                                        assemblyNameIfAny
                                    );


                                    string customMarkupValueName = "customMarkupValue_" + Guid.NewGuid().ToString("N");

                                    bool isDependencyProperty = _reflectionOnSeparateAppDomain.GetField(
                                        propertyName + "Property",
                                        isAttachedProperty ? elementName.Namespace.NamespaceName : parent.Name.Namespace.NamespaceName,
                                        isAttachedProperty ? elementName.LocalName : parent.Name.LocalName,
                                        _assemblyNameWithoutExtension) != null;

                                    if (isDependencyProperty)
                                    {
                                        string bindingBaseTypeString = $"{_metadata.SystemWindowsDataNS}.Binding";

                                        //todo: make this more readable by cutting it into parts ?
                                        parameters.StringBuilder.AppendLine(
                                            string.Format(@"var {0} = {1}.ProvideValue(new global::System.ServiceProvider({2}, {3}));
if({0} is {4})
{{
    global::{9}.BindingOperations.SetBinding({7}, {8}, ({4}){0});
}}
else
{{
    {2}.{5} = ({6}){0};
}}",
                                                          customMarkupValueName, //0
                                                          childUniqueName,//1
                                                          GetUniqueName(parent),//2
                                                          propertyKeyString,//3
                                                          bindingBaseTypeString,//4
                                                          propertyName,//5
                                                          "global::" + (!string.IsNullOrEmpty(propertyNamespaceName) ? propertyNamespaceName + "." : "") + propertyLocalTypeName,//6
                                                          parentElementUniqueNameOrThisKeyword,//7
                                                          propertyDeclaringTypeName + "." + propertyName + "Property", //8
                                                          _metadata.SystemWindowsDataNS//9
                                                          ));
                                    }
                                    else
                                    {
                                        parameters.StringBuilder.AppendLine(
                                           string.Format(@"var {0} = {1}.ProvideValue(new global::System.ServiceProvider({2}, {3})); {2}.{4} = ({5}){0};",
                                                    customMarkupValueName, //0
                                                    childUniqueName,//1
                                                    GetUniqueName(parent),//2
                                                    propertyKeyString,//3
                                                    propertyName,//4
                                                    "global::" + (!string.IsNullOrEmpty(propertyNamespaceName) ? propertyNamespaceName + "." : "") + propertyLocalTypeName//5
                                           ));
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
                XElement child = _reader.MemberData.Value;

                string targetUniqueName = GetUniqueName(target);

                if (GettingInformationAboutXamlTypes.IsElementADictionary(target, _reflectionOnSeparateAppDomain))
                {
                    bool isImplicitStyle;
                    bool isImplicitDataTemplate;
                    string childKey = GetElementXKey(child, out isImplicitStyle, out isImplicitDataTemplate);
                    if (isImplicitStyle)
                    {
                        parameters.StringBuilder.AppendLine($"((global::System.Collections.IDictionary){targetUniqueName}).Add(typeof({childKey}), {GetUniqueName(child)});");
                    }
                    else if (isImplicitDataTemplate)
                    {
                        string key = $"new global::{_metadata.SystemWindowsNS}.DataTemplateKey(typeof({childKey}))";

                        parameters.StringBuilder.AppendLine($"((global::System.Collections.IDictionary){targetUniqueName}).Add({key}, {GetUniqueName(child)});");
                    }
                    else
                    {
                        parameters.StringBuilder.AppendLine($"((global::System.Collections.IDictionary){targetUniqueName}).Add(\"{childKey}\", {GetUniqueName(child)});");
                    }
                }
                else
                {
                    parameters.StringBuilder.AppendLine($"((global::System.Collections.IList){targetUniqueName}).Add({GetUniqueName(child)});");
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

                        if (propertyName == "ContentPropertyUsefulOnlyDuringTheCompilation" &&
                            _reflectionOnSeparateAppDomain.IsAssignableFrom(_metadata.SystemWindowsNS, "FrameworkTemplate",
                            namespaceName, typeName))
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
                return className == $"global::{_metadata.SystemWindowsNS}.Application";
            }

            private string GenerateCodeForSetterProperty(XElement styleElement, string attributeValue)
            {
                bool isAttachedProperty = attributeValue.Contains(".");
                string elementTypeInCSharp, dependencyPropertyName;
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
                    string elementNamespaceName, elementLocalTypeName, assemblyIfAny;
                    GettingInformationAboutXamlTypes.GetClrNamespaceAndLocalName(propertyFullXamlTypeName, styleElement, out elementNamespaceName, out elementLocalTypeName, out assemblyIfAny);
                    elementTypeInCSharp = _reflectionOnSeparateAppDomain.GetCSharpEquivalentOfXamlTypeAsString(elementNamespaceName, elementLocalTypeName);
                    dependencyPropertyName = splittedAttachedProperty[1] + "Property";
                }
                else
                {
                    elementTypeInCSharp = GetCSharpFullTypeNameFromTargetTypeString(styleElement);
                    dependencyPropertyName = attributeValue + "Property"; //todo: handle the case where the DependencyProperty name is not the name of the property followed by "Property" (at least improve the error message)
                }
                return string.Format("{0}.{1}", elementTypeInCSharp, dependencyPropertyName);
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

                GettingInformationAboutXamlTypes.GetClrNamespaceAndLocalName(attributeTypeString, currentXElement, out namespaceName, out localTypeName, out assemblyNameIfAny);
                return _reflectionOnSeparateAppDomain.GetCSharpEquivalentOfXamlTypeAsXName(namespaceName, localTypeName, assemblyNameIfAny, ifTypeNotFoundTryGuessing: false);
            }

            private string GetCSharpFullTypeNameFromTargetTypeString(XElement styleElement, bool isDataType = false)
            {
                string namespaceName;
                string localTypeName;
                string assemblyNameIfAny;
                var targetTypeAttribute = styleElement.Attribute(isDataType ? "DataType" : "TargetType");
                if (targetTypeAttribute == null)
                    throw new XamlParseException(isDataType ? "DataTemplate must declare a DataType or have a key." : "Style must declare a TargetType.");
                string targetTypeString = targetTypeAttribute.Value;
                GettingInformationAboutXamlTypes.GetClrNamespaceAndLocalName(targetTypeString, styleElement, out namespaceName, out localTypeName, out assemblyNameIfAny);
                string elementTypeInCSharp = _reflectionOnSeparateAppDomain.GetCSharpEquivalentOfXamlTypeAsString(namespaceName, localTypeName, assemblyNameIfAny, ifTypeNotFoundTryGuessing: false);
                return elementTypeInCSharp;
            }

            private string GetCSharpFullTypeName(string typeString, XElement elementWhereTheTypeIsUsed)
            {
                string namespaceName;
                string localTypeName;
                string assemblyNameIfAny;
                GettingInformationAboutXamlTypes.GetClrNamespaceAndLocalName(typeString, elementWhereTheTypeIsUsed, out namespaceName, out localTypeName, out assemblyNameIfAny);
                string elementTypeInCSharp = _reflectionOnSeparateAppDomain.GetCSharpEquivalentOfXamlTypeAsString(namespaceName, localTypeName, assemblyNameIfAny, ifTypeNotFoundTryGuessing: false);
                return elementTypeInCSharp;
            }

            private string GetElementXKey(XElement element,
                out bool isImplicitStyle,
                out bool isImplicitDataTemplate)
            {
                isImplicitStyle = false;
                isImplicitDataTemplate = false;

                if (element.Attribute(xNamespace + "Key") != null)
                {
                    return element.Attribute(xNamespace + "Key").Value;
                }
                else if (element.Attribute(xNamespace + "Name") != null)
                {
                    return element.Attribute(xNamespace + "Name").Value;
                }
                else if (IsStyle(element))
                {
                    isImplicitStyle = true;
                    return GetCSharpFullTypeNameFromTargetTypeString(element);
                }
                else if (IsDataTemplate(element) && element.Attribute("DataType") != null)
                {
                    isImplicitDataTemplate = true;
                    return GetCSharpFullTypeNameFromTargetTypeString(element, isDataType: true);
                }
                else
                {
                    throw new XamlParseException("Each dictionary entry must have an associated key. The element named '" + element.Name.LocalName + "' does not have a key.");
                }
            }

            private static string GetDefaultValueOfTypeAsString(string namespaceName,
                string localTypeName,
                bool isSystemType,
                ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain,
                string assemblyIfAny = null)
            {
                if (isSystemType)
                {
                    return SystemTypesHelper.GetDefaultValue(namespaceName, localTypeName, assemblyIfAny);
                }
                else
                {
                    Type type = reflectionOnSeparateAppDomain.GetCSharpEquivalentOfXamlType(namespaceName, localTypeName, assemblyIfAny, true);
                    if (type == null)
                    {
                        return null;
                    }
                    else
                    {
                        if (type.IsValueType)
                        {
                            return Activator.CreateInstance(type).ToString();
                        }
                        else
                        {
                            return "";
                        }
                    }
                }
            }

            private string GenerateCodeForInstantiatingAttributeValue(
                XName xName,
                string propertyName,
                bool isAttachedProperty,
                string value,
                XElement elementWhereTheTypeIsUsed)
            {
                string namespaceName, localTypeName, assemblyNameIfAny;
                GettingInformationAboutXamlTypes.GetClrNamespaceAndLocalName(
                    xName, out namespaceName, out localTypeName, out assemblyNameIfAny
                );

                string valueNamespaceName, valueLocalTypeName, valueAssemblyName;
                bool isValueString, isValueEnum;

                if (isAttachedProperty)
                {
                    _reflectionOnSeparateAppDomain.GetMethodReturnValueTypeInfo(
                        "Get" + propertyName,
                        namespaceName,
                        localTypeName,
                        out valueNamespaceName,
                        out valueLocalTypeName,
                        out valueAssemblyName,
                        out isValueString,
                        out isValueEnum,
                        assemblyNameIfAny);
                }
                else
                {
                    _reflectionOnSeparateAppDomain.GetPropertyOrFieldTypeInfo(
                        propertyName,
                        namespaceName,
                        localTypeName,
                        out valueNamespaceName,
                        out valueLocalTypeName,
                        out valueAssemblyName,
                        out isValueString,
                        out isValueEnum,
                        assemblyNameIfAny);
                }

                string valueTypeFullName = string.Format(
                    "global::{0}{1}{2}",
                    valueNamespaceName,
                    string.IsNullOrEmpty(valueNamespaceName) ? string.Empty : ".",
                    valueLocalTypeName
                );

                // Generate the code or instantiating the attribute
                if (isValueString)
                {
                    //----------------------------
                    // PROPERTY IS OF TYPE STRING
                    //----------------------------
                    return ConvertingStringToValue.PrepareStringForString(value);
                }
                else if (isValueEnum)
                {
                    //----------------------------
                    // PROPERTY IS AN ENUM
                    //----------------------------
                    if (value.IndexOf(',') != -1)
                    {
                        string[] values = value.Split(new char[] { ',' })
                            .Select(v =>
                                string.Format(
                                    "{0}.{1}",
                                    valueTypeFullName,
                                    _reflectionOnSeparateAppDomain.GetFieldName(v.Trim(), valueNamespaceName, valueLocalTypeName, null)
                                )
                            ).ToArray();

                        return string.Join(" | ", values);
                    }
                    else
                    {
                        return string.Format(
                            "{0}.{1}",
                            valueTypeFullName,
                            _reflectionOnSeparateAppDomain.GetFieldName(value, valueNamespaceName, valueLocalTypeName, null)
                        );
                    }
                }
                else if (valueTypeFullName == "global::System.Type")
                {
                    string typeFullName = GetCSharpFullTypeName(value, elementWhereTheTypeIsUsed);

                    return string.Format("typeof({0})", typeFullName);
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

                    bool isKnownSystemType = SystemTypesHelper.IsSupportedSystemType(
                        valueTypeFullName.Substring("global::".Length), valueAssemblyName
                    );

                    bool isKnownCoreType = CoreTypesHelper.IsSupportedCoreType(
                        valueTypeFullName.Substring("global::".Length), valueAssemblyName
                    );

                    string declaringTypeName = _reflectionOnSeparateAppDomain.GetCSharpEquivalentOfXamlTypeAsString(
                        namespaceName, localTypeName, assemblyNameIfAny
                    );

                    if (isAttachedProperty)
                    {
                        return ConvertFromInvariantString(
                            value, valueTypeFullName, isKnownCoreType, isKnownSystemType
                        );
                    }
                    else
                    {
                        return ConvertFromInvariantString(
                            declaringTypeName, propertyName, value, valueTypeFullName, isKnownCoreType, isKnownSystemType
                        );
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
                if ((valueTypeFullName == $"global::{_metadata.SystemWindowsMediaNS}.ImageSource"
                    || valueTypeFullName == "global::System.Uri"
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
                        path = "/" + _assemblyNameWithoutExtension + ";component/" + pathRelativeToProjectRoot;
                    }
                }
            }

            private static bool IsUriAbsolute(string path)
            {
                if (path.Contains(":"))
                {
                    // cf. https://stackoverflow.com/questions/1737575/are-colons-allowed-in-urls
                    string textBeforeColon = path.Substring(0, path.IndexOf(":"));
                    if (!textBeforeColon.Contains(@"\") && !textBeforeColon.Contains(@"/"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
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
                    GettingInformationAboutXamlTypes.GetClrNamespaceAndLocalName(
                        XName.Get(typeName, xmlNamespace.NamespaceName),
                        out string namespaceName, out string localTypeName, out string assemblyName
                    );

                    string assemblyQualifiedName = _reflectionOnSeparateAppDomain.GetAssemblyQualifiedNameOfXamlType(
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

            private static string ConvertFromInvariantString(string value, string type, bool isKnownCoreType, bool isKnownSystemType)
            {
                string preparedValue;

                if (isKnownCoreType)
                {
                    preparedValue = CoreTypesHelper.ConvertFromInvariantString(
                        value, type.Substring("global::".Length)
                    );
                }
                else if (isKnownSystemType)
                {
                    preparedValue = SystemTypesHelper.ConvertFromInvariantString(
                        value, type.Substring("global::".Length)
                    );
                }
                else
                {
                    preparedValue = ConvertingStringToValue.ConvertFromInvariantString(type, value);
                }

                return preparedValue;
            }

            private static string ConvertFromInvariantString(
                string propertyDeclaringType,
                string propertyName,
                string value,
                string propertyType,
                bool isKnownCoreType,
                bool isKnownSystemType)
            {
                return string.Format(
                    "{0}.GetPropertyValue<{1}>(typeof({2}), {3}, {4}, () => {5})",
                    RuntimeHelperClass,
                    propertyType,
                    propertyDeclaringType,
                    EscapeString(propertyName),
                    EscapeString(value),
                    ConvertFromInvariantString(value, propertyType, isKnownCoreType, isKnownSystemType)
                );
            }

            private bool IsEventTriggerRoutedEventProperty(XName xName, string propertyName)
            {
                if (xName.LocalName != "EventTrigger" || propertyName != "RoutedEvent")
                {
                    return false;
                }

                if (xName.NamespaceName.StartsWith("clr-namespace:"))
                {
                    GettingInformationAboutXamlTypes.ParseClrNamespaceDeclaration(xName.NamespaceName, out string ns, out string assembly);
#if MIGRATION
                    return ns == _metadata.SystemWindowsNS && (assembly == _metadata.SystemWindowsDLL || assembly == GetCurrentCoreAssemblyName());
#else
                    return ns == _metadata.SystemWindowsNS && assembly == GetCurrentCoreAssemblyName();
#endif
                }

                return DefaultXamlNamespaces.Contains(xName.Namespace);
            }

            private static string GetCurrentCoreAssemblyName()
            {
#if MIGRATION

#if CSHTML5BLAZOR
                return Constants.NAME_OF_CORE_ASSEMBLY_SLMIGRATION_USING_BLAZOR;
#elif BRIDGE
                return Constants.NAME_OF_CORE_ASSEMBLY_SLMIGRATION_USING_BRIDGE;
#endif

#else

#if CSHTML5BLAZOR
                return Constants.NAME_OF_CORE_ASSEMBLY_USING_BLAZOR;
#elif BRIDGE
                return Constants.NAME_OF_CORE_ASSEMBLY_USING_BRIDGE;
#endif

#endif
            }

            private static bool IsReservedAttribute(string attributeName)
            {
                if (attributeName == GeneratingUniqueNames.UniqueNameAttribute ||
                    attributeName == InsertingImplicitNodes.InitializedFromStringAttribute)
                {
                    return true;
                }

                return false;
            }

            private static string EscapeString(string stringValue)
            {
                return string.Concat("@\"", stringValue.Replace("\"", "\"\""), "\"");
            }
        }
    }
}
