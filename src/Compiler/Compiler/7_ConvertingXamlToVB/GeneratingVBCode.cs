
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
using OpenSilver.Internal;

namespace OpenSilver.Compiler
{
    internal static partial class GeneratingVBCode
    {
        private class ComponentConnectorBuilderVB
        {
            private const string targetParam = "target";
            private const string componentIdParam = "componentId";

            private readonly List<ComponentConnectorEntry> _entries = new List<ComponentConnectorEntry>();

            public int ConnectEventHandler(string componentType, string eventName, string handlerName)
            {
                int componentId = _entries.Count;
                _entries.Add(new EventEntry(componentId, componentType, eventName, handlerName));
                return componentId;
            }

            public int ConnectAttachedEventHandler(string componentType, string ownerType, string eventName, string handlerName)
            {
                int componentId = _entries.Count;
                _entries.Add(new AttachedEventEntry(componentId, componentType, ownerType, eventName, handlerName));
                return componentId;
            }

            public int ConnectEventSetterHandler(string handlerType, string handlerName)
            {
                int componentId = _entries.Count;
                _entries.Add(new EventSetterEntry(componentId, handlerType, handlerName));
                return componentId;
            }

            public int ConnectNamedElement(string componentType, string fieldName)
            {
                int componentId = _entries.Count;
                _entries.Add(new NamedElementEntry(componentId, componentType, fieldName));
                return componentId;
            }

            public override string ToString()
            {
                var builder = new StringBuilder();

                builder.Append(' ', 4 * 2).AppendLine("<Global.System.Diagnostics.DebuggerNonUserCodeAttribute()>")
                    .Append(' ', 4 * 2).AppendLine("<Global.System.ComponentModel.EditorBrowsable(Global.System.ComponentModel.EditorBrowsableState.Never)>")
                    .Append(' ', 4 * 2).AppendLine($"Sub IComponentConnector_Connect({componentIdParam} As Integer, {targetParam} As Object) Implements {IComponentConnectorClass}.Connect");

                if (_entries.Count > 0)
                {
                    builder.Append(' ', 4 * 3).AppendLine($"Select Case ({componentIdParam})");

                    foreach (ComponentConnectorEntry entry in _entries)
                    {
                        builder.Append(' ', 4 * 4).AppendLine($"Case {entry.ComponentId}");
                        builder.Append(' ', 4 * 4).AppendLine(entry.ToString());
                        builder.Append(' ', 4 * 5).AppendLine("Return");
                    }

                    builder.Append(' ', 4 * 3).AppendLine("End Select");
                }

                builder.Append(' ', 4 * 2).AppendLine("End Sub");

                builder.AppendLine();

                builder.Append(' ', 4 * 2).AppendLine("<Global.System.Diagnostics.DebuggerNonUserCodeAttribute()>")
                    .Append(' ', 4 * 2).AppendLine("<Global.System.ComponentModel.EditorBrowsable(Global.System.ComponentModel.EditorBrowsableState.Never)>")
                    .Append(' ', 4 * 2).AppendLine($"Sub IComponentConnector_InitializeComponent() Implements {IComponentConnectorClass}.InitializeComponent")
                    .Append(' ', 4 * 3).AppendLine("Me.InitializeComponent()")
                    .Append(' ', 4 * 2).AppendLine("End Sub");

                return builder.ToString();
            }

            private abstract class ComponentConnectorEntry
            {
                protected ComponentConnectorEntry(int componentId)
                {
                    ComponentId = componentId;
                }

                public int ComponentId { get; }

                public abstract override string ToString();
            }

            private sealed class EventEntry : ComponentConnectorEntry
            {
                private readonly string _componentType;
                private readonly string _eventName;
                private readonly string _handlerName;

                public EventEntry(int componentId, string componentType, string eventName, string handlerName)
                    : base(componentId)
                {
                    _componentType = componentType;
                    _eventName = eventName;
                    _handlerName = handlerName;
                }

                public override string ToString()
                {
                    return $"AddHandler DirectCast({targetParam}, Global.{_componentType}).{_eventName}, AddressOf Me.{_handlerName}";
                }
            }

            private sealed class AttachedEventEntry : ComponentConnectorEntry
            {
                private readonly string _componentType;
                private readonly string _ownerType;
                private readonly string _eventName;
                private readonly string _handlerName;

                public AttachedEventEntry(int componentId, string componentType, string ownerType, string eventName, string handlerName)
                    : base(componentId)
                {
                    _componentType = componentType;
                    _ownerType = ownerType;
                    _eventName = eventName;
                    _handlerName = handlerName;
                }

                public override string ToString()
                {
                    return $"Global.{_ownerType}.Add{_eventName}Handler(DirectCast({targetParam}, Global.{_componentType}), AddressOf Me.{_handlerName})";
                }
            }

            private sealed class EventSetterEntry : ComponentConnectorEntry
            {
                private readonly string _handlerType;
                private readonly string _handlerName;

                public EventSetterEntry(int componentId, string handlerType, string handlerName)
                    : base(componentId)
                {
                    _handlerType = handlerType;
                    _handlerName = handlerName;
                }

                public override string ToString()
                {
                    return $"DirectCast({targetParam}, Global.System.Windows.EventSetter).Handler = New Global.{_handlerType}(AddressOf Me.{_handlerName})";
                }
            }

            private sealed class NamedElementEntry : ComponentConnectorEntry
            {
                private readonly string _componentType;
                private readonly string _fieldName;

                public NamedElementEntry(int componentId, string componentType, string fieldName)
                    : base(componentId)
                {
                    _componentType = componentType;
                    _fieldName = fieldName;
                }

                public override string ToString()
                {
                    return $"Me.{_fieldName} = DirectCast({targetParam}, Global.{_componentType})";
                }
            }
        }

        public static string GenerateCode(XDocument doc,
            string sourceFile,
            string fileNameWithPathRelativeToProjectRoot,
            string rootNamespace,
            bool isFirstPass,
            ConversionSettings settings)
        {
            ICodeGenerator generator;
            if (isFirstPass)
            {
                generator = new GeneratorPass1(doc,
                    fileNameWithPathRelativeToProjectRoot,
                    rootNamespace,
                    settings);
            }
            else
            {
                generator = new GeneratorPass2(doc,
                    sourceFile,
                    fileNameWithPathRelativeToProjectRoot,
                    rootNamespace,
                    settings);
            }

            return generator.Generate();
        }

        private static string CreateInitializeComponentMethod(
            string applicationTypeFullName,
            string assemblyNameWithoutExtension,
            string fileNameWithPathRelativeToProjectRoot)
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

            {loadComponentCall}
        End Sub
";
        }


        private static string GeneratePartialClass(
            string initializeComponentMethod,
            string connectMethod,
            List<string> fieldsForNamedElements,
            string className,
            string namespaceStringIfAny,
            string baseType)
        {
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

End Class
";

            string finalCode;
            if (!string.IsNullOrEmpty(namespaceStringIfAny))
            {
                finalCode = $@"
Namespace {namespaceStringIfAny}
{classCodeFilled}
End Namespace
";
            }
            else
            {
                finalCode = classCodeFilled;
            }

            return finalCode;
        }

        private static void GetClassInformationFromXaml(XDocument doc,
            AssembliesInspector reflectionOnSeparateAppDomain,
            out string className,
            out string namespaceStringIfAny,
            out bool hasCodeBehind)
        {
            // Read the "{x:Class}" attribute:
            XAttribute classAttributeIfAny = doc.Root.Attribute(GeneratingCode.xNamespace + "Class");
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

        private static (string NamespaceDeclaration, string NamespaceName) GetNamespace(string ns, string rootNamespace)
        {
            if (string.IsNullOrEmpty(ns))
            {
                return (string.Empty, rootNamespace);
            }

            if (string.IsNullOrEmpty(rootNamespace))
            {
                return (ns, ns);
            }

            if (ns.StartsWith(rootNamespace))
            {
                if (ns.Length == rootNamespace.Length)
                {
                    return (string.Empty, ns);
                }
                else if (ns[rootNamespace.Length] == '.')
                {
                    return (ns.Substring(rootNamespace.Length + 1), ns);
                }
            }

            return (ns, $"{rootNamespace}.{ns}");
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
            string baseTypeFullName,
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
'------------------------------------------------------------------------------
' <auto-generated>
'     This code was auto-generated by ""VB/XAML for HTML5""
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Namespace Global
    ''' <summary>
    ''' {factoryName}
    ''' </summary>
    <Global.System.Diagnostics.DebuggerNonUserCodeAttribute()>
    <Global.System.ComponentModel.EditorBrowsable(Global.System.ComponentModel.EditorBrowsableState.Never)>
    Public NotInheritable Class {factoryName}
        Implements {IXamlComponentFactoryClass}(Of {componentTypeFullName}), {IXamlComponentLoaderClass}(Of {baseTypeFullName})
        ''' <summary>
        ''' Instantiate
        ''' </summary>
        <Global.System.ComponentModel.EditorBrowsable(Global.System.ComponentModel.EditorBrowsableState.Never)>
        Public Shared Function Instantiate() As Object
            Return CreateComponentImpl()
        End Function
    
        Private Function IXamlComponentFactory_CreateComponent() As {componentTypeFullName} Implements {IXamlComponentFactoryClass}(Of {componentTypeFullName}).CreateComponent
            Return CreateComponentImpl()
        End Function
    
        Private Function IXamlComponentFactory_CreateComponent1() As Object Implements {IXamlComponentFactoryClass}.CreateComponent
            Return CreateComponentImpl()
        End Function
    
        Private Sub IXamlComponentLoader_LoadComponent(component As {baseTypeFullName}) Implements {IXamlComponentLoaderClass}(Of {baseTypeFullName}).LoadComponent
            LoadComponentImpl(component)
        End Sub
    
        Private Sub IXamlComponentLoader_LoadComponent1(component As Object) Implements {IXamlComponentLoaderClass}.LoadComponent
            LoadComponentImpl(CType(component, {baseTypeFullName}))
        End Sub
    
        Private Shared Sub LoadComponentImpl(ByVal {componentParamName} As {baseTypeFullName})
            If TypeOf CObj({componentParamName}) Is {uiElementFullyQualifiedTypeName} Then
                CType(CObj({componentParamName}), {uiElementFullyQualifiedTypeName}).XamlSourcePath = ""{assemblyName}\{fileNameWithPathRelativeToProjectRoot}""
            End If
    
            {loadComponentImpl}
        End Sub
    
        Private Shared Function CreateComponentImpl() As {componentTypeFullName}
            {createComponentImpl}
        End Function
    
        {string.Join(Environment.NewLine + Environment.NewLine, additionalMethods)}
    End Class
End Namespace
";

            return finalCode;
        }

        private const string RuntimeHelperClass = "Global.OpenSilver.Internal.Xaml.RuntimeHelpers";
        private const string IXamlComponentFactoryClass = "Global.OpenSilver.Internal.Xaml.IXamlComponentFactory";
        private const string IXamlComponentLoaderClass = "Global.OpenSilver.Internal.Xaml.IXamlComponentLoader";
        private const string IComponentConnectorClass = "Global.OpenSilver.Internal.Xaml.IComponentConnector";
        private const string XamlContextClass = "Global.OpenSilver.Internal.Xaml.Context.XamlContext";
        private const string IMarkupExtensionClass = "Global.System.Xaml.IMarkupExtension(Of Object)";
        private const string XamlDesignerBridgeClass = "Global.OpenSilver.Internal.Xaml.XamlDesignerBridge";
    }
}
