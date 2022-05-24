

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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using OpenSilver.Internal;

namespace DotNetForHtml5.Compiler
{
    internal interface ICodeGenerator
    {
        string Generate();
    }

    internal static partial class GeneratingCSharpCode
    {
        private class ComponentConnectorBuilder
        {
            private const string targetParam = "target";
            private const string componentIdParam = "componentId";

            private readonly List<ComponentConnectorEntry> _entries = new List<ComponentConnectorEntry>();

            public int Connect(string componentType, string eventName, string handlerType, string handlerName)
            {
                int componentId = _entries.Count;
                _entries.Add(new ComponentConnectorEntry
                {
                    componentType = componentType,
                    eventName = eventName,
                    handlerType = handlerType,
                    handlerName = handlerName
                });

                return componentId;
            }

            public override string ToString()
            {
                StringBuilder builder = new StringBuilder();

                builder.Append(' ', 4 * 2).AppendLine("[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]")
                    .Append(' ', 4 * 2).AppendLine("[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]")
                    .Append(' ', 4 * 2).AppendLine($"void {IComponentConnectorClass}.Connect(int {componentIdParam}, object {targetParam})")
                    .Append(' ', 4 * 2).AppendLine("{");

                if (_entries.Count > 0)
                {
                    builder.Append(' ', 4 * 3).AppendLine($"switch ({componentIdParam})")
                      .Append(' ', 4 * 3).AppendLine("{");

                    for (int componentId = 0; componentId < _entries.Count; componentId++)
                    {
                        ComponentConnectorEntry eventEntry = _entries[componentId];
                        builder.Append(' ', 4 * 4).AppendLine($"case {componentId}:");
                        builder.Append(' ', 4 * 5).AppendLine($"(({eventEntry.componentType})({targetParam})).{eventEntry.eventName} += new {eventEntry.handlerType}(this.{eventEntry.handlerName});");
                        builder.Append(' ', 4 * 5).AppendLine("return;");
                    }

                    builder.Append(' ', 4 * 3).AppendLine("}");
                }

                builder.Append(' ', 4 * 2).AppendLine("}");

                return builder.ToString();
            }

            private struct ComponentConnectorEntry
            {
                public string componentType;
                public string eventName;
                public string handlerType;
                public string handlerName;
            }
        }

        internal const string DefaultXamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        private const string LegacyXamlNamespace = "http://schemas.microsoft.com/client/2007"; // XAML namespace used for Silverlight 1.0 application
        
        private static readonly XNamespace[] DefaultXamlNamespaces = new XNamespace[2] { DefaultXamlNamespace, LegacyXamlNamespace };
        internal static readonly XNamespace xNamespace = @"http://schemas.microsoft.com/winfx/2006/xaml"; // Used for example for "x:Name" attributes and {x:Null} markup extensions.

        public static string GenerateCSharpCode(XDocument doc,
            string sourceFile,
            string fileNameWithPathRelativeToProjectRoot,
            string assemblyNameWithoutExtension,
            ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain,
            bool isFirstPass,
            bool isSLMigration,
            string codeToPutInTheInitializeComponentOfTheApplicationClass,
            ILogger logger)
        {
            ICodeGenerator generator;
            if (isFirstPass)
            {
                generator = new GeneratorPass1(doc, 
                    assemblyNameWithoutExtension, 
                    fileNameWithPathRelativeToProjectRoot, 
                    reflectionOnSeparateAppDomain, 
                    isSLMigration);
            }
            else
            {
                generator = new GeneratorPass2(doc,
                    sourceFile,
                    fileNameWithPathRelativeToProjectRoot,
                    assemblyNameWithoutExtension,
                    reflectionOnSeparateAppDomain,
                    isSLMigration,
                    codeToPutInTheInitializeComponentOfTheApplicationClass,
                    logger);
            }

            return generator.Generate();
        }

        internal static int GetLineNumber(XNode element)
        {
            // Get the line number in the original XAML file by walking up the tree until we find a node that contains line number information:

            while (element != null)
            {
                // See if the current element has line information:
                if (((IXmlLineInfo)element).HasLineInfo())
                {
                    return ((IXmlLineInfo)element).LineNumber;
                }

                // If not, go to the previous sibling node if any:
                var previousNode = element.PreviousNode;
                if (previousNode != null)
                {
                    element = previousNode;
                }
                else
                {
                    // Alternatively, walk up the tree to go to the parent node:
                    element = element.Parent;
                }
            }
            return -1;
        }

        private static bool IsAttributeTheXNameAttribute(XAttribute attribute)
        {
            bool isXName = (attribute.Name.LocalName == "Name" && attribute.Name.NamespaceName == xNamespace);
            bool isName = (attribute.Name.LocalName == "Name" && string.IsNullOrEmpty(attribute.Name.NamespaceName));
            return isXName || isName;
        }

        private static string CreateInitializeComponentMethod(
            string applicationTypeFullName,
            string additionalCodeForApplication,
            string assemblyNameWithoutExtension,
            string fileNameWithPathRelativeToProjectRoot,
            List<string> findNameCalls)
        {
            string componentUri = $"/{assemblyNameWithoutExtension};component/{fileNameWithPathRelativeToProjectRoot.Replace('\\', '/')}";

            string loadComponentCall = $"{applicationTypeFullName}.LoadComponent(this, new global::{XamlResourcesHelper.GenerateClassNameFromComponentUri(componentUri)}());";
            //// enable this to replicate the Silverlight behavior. We use a custom variant of Application.LoadComponent that uses less reflection.
            //string loadComponentCall = $"{applicationTypeFullName}.LoadComponent(this, new global::System.Uri(\"{componentUri}\", global::System.UriKind.Relative));";

            return $@"
        private bool _contentLoaded;

        /// <summary>
        /// InitializeComponent
        /// </summary>
        public void InitializeComponent()
        {{
            if (_contentLoaded) 
            {{
                return;
            }}
            _contentLoaded = true;
            {additionalCodeForApplication}
            {loadComponentCall}
            {string.Join(Environment.NewLine + "            ", findNameCalls)}
        }}
";
        }

        private static string GetUniqueName(XElement element)
        {
            return element.Attribute(GeneratingUniqueNames.UniqueNameAttribute).Value;
        }

        private static string GeneratePartialClass(
            string initializeComponentMethod,
            string connectMethod,
            List<string> fieldsForNamedElements,
            string className,
            string namespaceStringIfAny,
            string baseType,
            bool addApplicationEntryPoint)
        {
            string applicationEntryPointIfAny = string.Empty;
            if (addApplicationEntryPoint)
            {
                applicationEntryPointIfAny = $@"
public static void Main()
{{
    new {className}();
}}";
            }

            string fieldsForNamedElementsMergedCode = string.Join(Environment.NewLine, fieldsForNamedElements);

            string classCodeFilled = $@"
public partial class {className} : {baseType}, {IComponentConnectorClass}
{{

#pragma warning disable 169, 649, 0628 // Prevents warning CS0169 ('field ... is never used'), CS0649 ('field ... is never assigned to, and will always have its default value null'), and CS0628 ('member : new protected member declared in sealed class')
{fieldsForNamedElementsMergedCode}
#pragma warning restore 169, 649, 0628

{initializeComponentMethod}

{connectMethod}

{applicationEntryPointIfAny}

}}
";
            
            string finalCode;
            if (!string.IsNullOrEmpty(namespaceStringIfAny))
            {
                finalCode = $@"
namespace {namespaceStringIfAny}
{{
{classCodeFilled}
}}
";
            }
            else
            {
                finalCode = classCodeFilled;
            }

            return finalCode;
        }

        private static void GetClassInformationFromXaml(XDocument doc, 
            ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain, 
            out string className, 
            out string namespaceStringIfAny, 
            out string baseType, 
            out bool hasCodeBehind)
        {
            // Read the "{x:Class}" attribute:
            XAttribute classAttributeIfAny = doc.Root.Attribute(xNamespace + "Class");
            if (classAttributeIfAny != null)
            {
                //-----------------
                // XAML files that have a code-behind
                //-----------------
                string classAttributeAsString = classAttributeIfAny.Value;

                // Split the content of x:Class to get the namespace and the class name:
                namespaceStringIfAny = null;
                int lastIndexOfDot = classAttributeAsString.LastIndexOf('.');
                if (lastIndexOfDot != -1)
                {
                    namespaceStringIfAny = classAttributeAsString.Substring(0, lastIndexOfDot);
                    className = classAttributeAsString.Substring(lastIndexOfDot + 1);
                }
                else
                    className = classAttributeAsString;

                hasCodeBehind = true;
            }
            else
            {
                //-----------------
                // XAML files without code-behind (such as ResourceDictionaries)
                //-----------------

                className = null;
                namespaceStringIfAny = null;
                hasCodeBehind = false;
                //todo: handle the case where there is a code-behing but the user has simply forgotten the "x:Class" attribute, in which case the user will currently get strange error messages.
            }

            // Get the base type of the control:
            string namespaceName, localTypeName, assemblyNameIfAny;
            GettingInformationAboutXamlTypes.GetClrNamespaceAndLocalName(doc.Root.Name, out namespaceName, out localTypeName, out assemblyNameIfAny);
            baseType = reflectionOnSeparateAppDomain.GetCSharpEquivalentOfXamlTypeAsString(namespaceName, localTypeName, assemblyNameIfAny, ifTypeNotFoundTryGuessing: true); // Note: we set "ifTypeNotFoundTryGuessing" to true because the type will not be found during Pass1 for example in the case that tthe root of the XAML file is: <myNamespace:MyCustumUserControlDerivedClass .../>
        }

        private static string GetFullTypeName(string namespaceName, string typeName)
        {
            if (string.IsNullOrEmpty(namespaceName))
            {
                return $"global::{typeName}";
            }

            return $"global::{namespaceName}.{typeName}";
        }

        private static string GenerateFactoryClass(
            string componentTypeFullName,
            string componentParamName,
            string loadComponentImpl,
            string createComponentImpl,
            IEnumerable<string> additionalMethods,
            string uiElementFullyQualifiedTypeName,
            string assemblyName,
            string fileNameWithPathRelativeToProjectRoot)
        {
            string absoluteSourceUri =
                    fileNameWithPathRelativeToProjectRoot.Contains(';') ?
                    fileNameWithPathRelativeToProjectRoot :
                    "/" + assemblyName + ";component/" + fileNameWithPathRelativeToProjectRoot;

            string factoryName = XamlResourcesHelper.GenerateClassNameFromComponentUri(absoluteSourceUri);

            string finalCode = $@"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by ""C#/XAML for HTML5""
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
public sealed class {factoryName} : {IXamlComponentFactoryClass}<{componentTypeFullName}>, {IXamlComponentLoaderClass}<{componentTypeFullName}>
{{
    public static object Instantiate()
    {{
        return CreateComponentImpl();
    }}

    {componentTypeFullName} {IXamlComponentFactoryClass}<{componentTypeFullName}>.CreateComponent()
    {{
        return CreateComponentImpl();
    }}

    object {IXamlComponentFactoryClass}.CreateComponent()
    {{
        return CreateComponentImpl();
    }}

    void {IXamlComponentLoaderClass}<{componentTypeFullName}>.LoadComponent({componentTypeFullName} component)
    {{
        LoadComponentImpl(component);
    }}

    void {IXamlComponentLoaderClass}.LoadComponent(object component)
    {{
        LoadComponentImpl(({componentTypeFullName})component);
    }}

    private static void LoadComponentImpl({componentTypeFullName} {componentParamName})
    {{
#pragma warning disable 0184 // Prevents warning CS0184 ('The given expression is never of the provided ('type') type')
        if ({componentParamName} is {uiElementFullyQualifiedTypeName})
        {{
            (({uiElementFullyQualifiedTypeName})(object){componentParamName}).XamlSourcePath = @""{assemblyName}\{fileNameWithPathRelativeToProjectRoot}"";
        }}
#pragma warning restore 0184

        {loadComponentImpl}
    }}

    private static {componentTypeFullName} CreateComponentImpl()
    {{
        {createComponentImpl}
    }}

    {string.Join(Environment.NewLine + Environment.NewLine, additionalMethods)}
}}
";

            return finalCode;
        }

        private const string RuntimeHelperClass = "global::OpenSilver.Internal.Xaml.RuntimeHelpers";
        private const string IXamlComponentFactoryClass = "global::OpenSilver.Internal.Xaml.IXamlComponentFactory";
        private const string IXamlComponentLoaderClass = "global::OpenSilver.Internal.Xaml.IXamlComponentLoader";
        private const string IComponentConnectorClass = "global::OpenSilver.Internal.Xaml.IComponentConnector";
        private const string XamlContextClass = "global::OpenSilver.Internal.Xaml.Context.XamlContext";

        public static bool IsDataTemplate(XElement element) => IsXElementOfType(element, "DataTemplate");

        public static bool IsItemsPanelTemplate(XElement element) => IsXElementOfType(element, "ItemsPanelTemplate");

        public static bool IsControlTemplate(XElement element) => IsXElementOfType(element, "ControlTemplate");

        public static bool IsResourceDictionary(XElement element) => IsXElementOfType(element, "ResourceDictionary");

        public static bool IsBinding(XElement element) => IsXElementOfType(element, "Binding");

        public static bool IsStyle(XElement element) => IsXElementOfType(element, "Style");

        public static bool IsTextBlock(XElement element) => IsXElementOfType(element, "TextBlock");

        public static bool IsRun(XElement element) => IsXElementOfType(element, "Run");

        public static bool IsSpan(XElement element) => IsXElementOfType(element, "Span");

        public static bool IsItalic(XElement element) => IsXElementOfType(element, "Italic");
        
        public static bool IsUnderline(XElement element) => IsXElementOfType(element, "Underline");

        public static bool IsBold(XElement element) => IsXElementOfType(element, "Bold");

        public static bool IsHyperlink(XElement element) => IsXElementOfType(element, "Hyperlink");

        public static bool IsParagraph(XElement element) => IsXElementOfType(element, "Paragraph");

        public static bool IsColorAnimation(XElement element) => IsXElementOfType(element, "ColorAnimation");

        private static bool IsXElementOfType(XElement element, string typeName)
        {
            XName name = element.Name;
            if (name.LocalName == typeName)
            {
                for (int i = 0; i < DefaultXamlNamespaces.Length; i++)
                {
                    if (name.Namespace == DefaultXamlNamespaces[i])
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
