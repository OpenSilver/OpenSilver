
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
using System.Text;
using System.Xml.Linq;
using System.Xml;
using OpenSilver.Internal;
using ILogger = OpenSilver.Compiler.Common.ILogger;

namespace OpenSilver.Compiler
{
    internal interface IVBCodeGenerator
    {
        string Generate();
    }

    internal static partial class GeneratingVBCode
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

                builder.Append(' ', 4 * 2).AppendLine("<Global.System.Diagnostics.DebuggerNonUserCodeAttribute()>")
                    .Append(' ', 4 * 2).AppendLine("<Global.System.ComponentModel.EditorBrowsable(Global.System.ComponentModel.EditorBrowsableState.Never)>")
                    .Append(' ', 4 * 2).AppendLine($"Sub IComponentConnector_Connect({componentIdParam} As Integer, {targetParam} As Object) Implements {IComponentConnectorClass}.Connect");

                if (_entries.Count > 0)
                {
                    builder.Append(' ', 4 * 3).AppendLine($"Select Case ({componentIdParam})");

                    for (int componentId = 0; componentId < _entries.Count; componentId++)
                    {
                        ComponentConnectorEntry eventEntry = _entries[componentId];
                        builder.Append(' ', 4 * 4).AppendLine($"Case {componentId}");
                        builder.Append(' ', 4 * 5).AppendLine($"AddHandler CType({targetParam}, {eventEntry.componentType}).{eventEntry.eventName}, AddressOf Me.{eventEntry.handlerName}");
                        builder.Append(' ', 4 * 5).AppendLine("Return");
                    }

                    builder.Append(' ', 4 * 3).AppendLine("End Select");
                }

                builder.Append(' ', 4 * 2).AppendLine("End Sub");

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

        public static string GenerateVBCode(XDocument doc,
            string sourceFile,
            string fileNameWithPathRelativeToProjectRoot,
            string assemblyNameWithoutExtension,
            AssembliesInspector reflectionOnSeparateAppDomain,
            bool isFirstPass,
            ConversionSettingsVB settings,
            string codeToPutInTheInitializeComponentOfTheApplicationClass,
            ILogger logger)
        {
            IVBCodeGenerator generator;
            if (isFirstPass)
            {
                generator = new GeneratorPass1(doc,
                    assemblyNameWithoutExtension,
                    fileNameWithPathRelativeToProjectRoot,
                    reflectionOnSeparateAppDomain,
                    settings);
            }
            else
            {
                generator = new GeneratorPass2(doc,
                    sourceFile,
                    fileNameWithPathRelativeToProjectRoot,
                    assemblyNameWithoutExtension,
                    reflectionOnSeparateAppDomain,
                    settings,
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

        private static bool IsXNameAttribute(XAttribute attr)
            => attr.Name.LocalName == "Name" && attr.Name.NamespaceName == xNamespace;

        private static bool IsNameAttribute(XAttribute attr)
            => attr.Name.LocalName == "Name" && string.IsNullOrEmpty(attr.Name.NamespaceName);

        private static string CreateInitializeComponentMethod(
            string applicationTypeFullName,
            string additionalCodeForApplication,
            string assemblyNameWithoutExtension,
            string fileNameWithPathRelativeToProjectRoot,
            List<string> findNameCalls)
        {
            string componentUri = $"/{assemblyNameWithoutExtension};component/{fileNameWithPathRelativeToProjectRoot.Replace('\\', '/')}";

            string loadComponentCall = $"{applicationTypeFullName}.LoadComponent(Me, New {XamlResourcesHelper.GenerateClassNameFromComponentUri(componentUri)}())";
            //// enable this to replicate the Silverlight behavior. We use a custom variant of Application.LoadComponent that uses less reflection.
            //string loadComponentCall = $"{applicationTypeFullName}.LoadComponent(Me, New Global.System.Uri(\"{componentUri}\", Global.System.UriKind.Relative))";

            return $@"
        Private _contentLoaded As Boolean

        ''' <summary>
        ''' InitializeComponent
        ''' </summary>
        Public Sub InitializeComponent()
            If _contentLoaded Then
                Return
            End If
            _contentLoaded = True

            {additionalCodeForApplication}
            {loadComponentCall}
            {string.Join(Environment.NewLine + "            ", findNameCalls)}
        End Sub
";
        }

        private static string GetUniqueName(XElement element)
        {
            return element.Attribute(GeneratingUniqueNamesVB.UniqueNameAttribute).Value;
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
Public Shared Fuction Main()
    New {className}();
End Function";
            }

            string fieldsForNamedElementsMergedCode = string.Join(Environment.NewLine, fieldsForNamedElements);

            string classCodeFilled = $@"
Partial Public Class {className}
    Inherits {baseType}
    Implements {IComponentConnectorClass}

'#pragma warning disable 169, 649, 0628 // Prevents warning CS0169 ('field ... is never used'), CS0649 ('field ... is never assigned to, and will always have its default value null'), and CS0628 ('member : new protected member declared in sealed class')
{fieldsForNamedElementsMergedCode}
'#pragma warning restore 169, 649, 0628

{initializeComponentMethod}

{connectMethod}

{applicationEntryPointIfAny}

End Class
";

            return classCodeFilled;
        }

        private static void GetClassInformationFromXaml(XDocument doc,
            AssembliesInspector reflectionOnSeparateAppDomain, 
            out string className, 
            out string namespaceStringIfAny, 
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
        }

        private static string GetFullTypeName(string namespaceName, string typeName)
        {
            if (string.IsNullOrEmpty(namespaceName))
            {
                return $"Global.{typeName}";
            }

            return $"Global.{namespaceName}.{typeName}";
        }

        private static string GenerateFactoryClass(
            string componentTypeFullName,
            string componentParamName,
            string loadComponentImpl,
            string createComponentImpl,
            IEnumerable<string> additionalMethods,
            string uiElementFullyQualifiedTypeName,
            string assemblyName,
            string fileNameWithPathRelativeToProjectRoot,
            string baseType)
        {
            string absoluteSourceUri =
                    fileNameWithPathRelativeToProjectRoot.Contains(';') ?
                    fileNameWithPathRelativeToProjectRoot :
                    "/" + assemblyName + ";component/" + fileNameWithPathRelativeToProjectRoot;

            string factoryName = XamlResourcesHelper.GenerateClassNameFromComponentUri(absoluteSourceUri);

            string xamlSourcePath = "";

            if (baseType != "Global.System.Windows.Application" && baseType != "Global.System.Windows.ResourceDictionary")
            {
                xamlSourcePath = $@"If TypeOf {componentParamName} Is {uiElementFullyQualifiedTypeName} Then
            CType(CObj({componentParamName}), {uiElementFullyQualifiedTypeName}).XamlSourcePath = ""{assemblyName}\{fileNameWithPathRelativeToProjectRoot}""
        End If
";
            }

            string finalCode = $@"
'------------------------------------------------------------------------------
' <auto-generated>
'     This code was auto-generated by ""VB/XAML for HTML5""
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

<Global.System.Diagnostics.DebuggerNonUserCodeAttribute()>
<Global.System.ComponentModel.EditorBrowsable(Global.System.ComponentModel.EditorBrowsableState.Never)>
Public NotInheritable Class {factoryName}
    Implements {IXamlComponentFactoryClass}(Of {componentTypeFullName}), {IXamlComponentLoaderClass}(Of {componentTypeFullName})
    Public Shared Function Instantiate() As Object
        Return CreateComponentImpl()
    End Function

    Private Function IXamlComponentFactory_CreateComponent() As {componentTypeFullName} Implements {IXamlComponentFactoryClass}(Of {componentTypeFullName}).CreateComponent
        Return CreateComponentImpl()
    End Function

    Private Function IXamlComponentFactory_CreateComponent1() As Object Implements {IXamlComponentFactoryClass}.CreateComponent
        Return CreateComponentImpl()
    End Function

    Private Sub IXamlComponentLoader_LoadComponent(component As {componentTypeFullName}) Implements {IXamlComponentLoaderClass}(Of {componentTypeFullName}).LoadComponent
        LoadComponentImpl(component)
    End Sub

    Private Sub IXamlComponentLoader_LoadComponent1(component As Object) Implements {IXamlComponentLoaderClass}.LoadComponent
        LoadComponentImpl(CType(CObj(component), {componentTypeFullName}))
    End Sub

    Private Shared Sub LoadComponentImpl(ByVal {componentParamName} As {componentTypeFullName})
        {xamlSourcePath}

        {loadComponentImpl}
    End Sub

    Private Shared Function CreateComponentImpl() As {componentTypeFullName}
        {createComponentImpl}
    End Function

    {string.Join(Environment.NewLine + Environment.NewLine, additionalMethods)}
End Class
";

            return finalCode;
        }

        private const string RuntimeHelperClass = "Global.OpenSilver.Internal.Xaml.RuntimeHelpers";
        private const string IXamlComponentFactoryClass = "Global.OpenSilver.Internal.Xaml.IXamlComponentFactory";
        private const string IXamlComponentLoaderClass = "Global.OpenSilver.Internal.Xaml.IXamlComponentLoader";
        private const string IComponentConnectorClass = "Global.OpenSilver.Internal.Xaml.IComponentConnector";
        private const string XamlContextClass = "Global.OpenSilver.Internal.Xaml.Context.XamlContext";
        private const string IMarkupExtensionClass = "Global.System.Xaml.IMarkupExtension<object>";

        public static bool IsDataTemplate(XElement element) => IsXElementOfType(element, "DataTemplate");

        public static bool IsItemsPanelTemplate(XElement element) => IsXElementOfType(element, "ItemsPanelTemplate");

        public static bool IsControlTemplate(XElement element) => IsXElementOfType(element, "ControlTemplate");

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
