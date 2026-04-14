
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
    internal static partial class GeneratingFSCode
    {
        private class ComponentConnectorBuilderFS
        {
            private const string targetParam = "target";
            private const string componentIdParam = "componentId";

            private readonly List<ComponentConnectorEntry> _entries = new List<ComponentConnectorEntry>();

            public int ConnectEventHandler(string componentType, string eventName, string handlerType, string handlerName)
            {
                int componentId = _entries.Count;
                _entries.Add(new EventEntry(componentId, componentType, eventName, handlerType, handlerName));
                return componentId;
            }

            public int ConnectAttachedEventHandler(string componentType, string ownerType, string eventName, string handlerType, string handlerName)
            {
                int componentId = _entries.Count;
                _entries.Add(new AttachedEventEntry(componentId, componentType, ownerType, eventName, handlerType, handlerName));
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

                builder.Append(' ', 4).AppendLine($"interface {IComponentConnectorClass} with")
                    .Append(' ', 4 * 2).AppendLine("[<global.System.Diagnostics.DebuggerNonUserCode>]")
                    .Append(' ', 4 * 2).AppendLine("[<global.System.ComponentModel.EditorBrowsable(global.System.ComponentModel.EditorBrowsableState.Never)>]")
                    .Append(' ', 4 * 2).AppendLine($"member this.Connect({componentIdParam}: int, {targetParam}: obj): unit = ");

                if (_entries.Count > 0)
                {
                    builder.Append(' ', 4 * 3).AppendLine($"match {componentIdParam} with");

                    foreach (ComponentConnectorEntry entry in _entries)
                    {
                        builder.Append(' ', 4 * 4).AppendLine($"| {entry.ComponentId} ->");
                        builder.Append(' ', 4 * 5).AppendLine(entry.ToString());
                    }

                    builder.Append(' ', 4 * 4).AppendLine("| _ -> ()");
                }
                else
                {
                    builder.Append(' ', 4 * 3).AppendLine("()");
                }

                builder.Append(' ', 4 * 2).AppendLine("[<global.System.Diagnostics.DebuggerNonUserCode>]")
                    .Append(' ', 4 * 2).AppendLine("[<global.System.ComponentModel.EditorBrowsable(global.System.ComponentModel.EditorBrowsableState.Never)>]")
                    .Append(' ', 4 * 2).AppendLine("member this.InitializeComponent(): unit =")
                    .Append(' ', 4 * 3).AppendLine("this.InitializeComponent()");

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
                private readonly string _handlerType;

                public EventEntry(int componentId, string componentType, string eventName, string handlerType, string handlerName)
                    : base(componentId)
                {
                    _componentType = componentType;
                    _eventName = eventName;
                    _handlerType = handlerType;
                    _handlerName = handlerName;
                }

                public override string ToString()
                {
                    return $"({targetParam} :?> global.{_componentType}).{_eventName}.AddHandler({RuntimeHelperClass}.CreateDelegate<{_handlerType}>(\"{_handlerName}\", this))";
                }
            }

            private sealed class AttachedEventEntry : ComponentConnectorEntry
            {
                private readonly string _componentType;
                private readonly string _ownerType;
                private readonly string _eventName;
                private readonly string _handlerType;
                private readonly string _handlerName;

                public AttachedEventEntry(int componentId, string componentType, string ownerType, string eventName, string handlerType, string handlerName)
                    : base(componentId)
                {
                    _componentType = componentType;
                    _ownerType = ownerType;
                    _eventName = eventName;
                    _handlerType = handlerType;
                    _handlerName = handlerName;
                }

                public override string ToString()
                {
                    return $"global.{_ownerType}.Add{_eventName}Handler(({targetParam} :?> global.{_componentType}), {RuntimeHelperClass}.CreateDelegate<global.{_handlerType}>(\"{_handlerName}\", this))";
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
                    return $"({targetParam} :?> global.System.Windows.EventSetter).Handler <- {RuntimeHelperClass}.CreateDelegate<global.{_handlerType}>(\"{_handlerName}\", this)";
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
                    return $"this.{_fieldName} <- {targetParam} :?> global.{_componentType}";
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
                    settings);
            }
            else
            {
                generator = new GeneratorPass2(doc,
                    sourceFile,
                    fileNameWithPathRelativeToProjectRoot,
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

            // string loadComponentCall = $"{applicationTypeFullName}.LoadComponent(this, global.{XamlResourcesHelper.GenerateClassNameFromComponentUri(componentUri)}())";
            // enable this to replicate the Silverlight behavior. We use a custom variant of Application.LoadComponent that uses less reflection.
            string loadComponentCall = $"{applicationTypeFullName}.LoadComponent(this, global.System.Uri(\"{componentUri}\", global.System.UriKind.Relative))";

            return $@"
    // <summary>
    // InitializeComponent
    // </summary>
    member this.InitializeComponent() =
        if contentLoaded then
            ()

        contentLoaded <- true

        {loadComponentCall}
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

            string finalCode = $@"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by ""F#/XAML for HTML5""
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

type {className}() =
    inherit {baseType}()
    let mutable contentLoaded = false
{fieldsForNamedElementsMergedCode}

{initializeComponentMethod}

{connectMethod}

";

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

        private static string GetFullTypeName(string namespaceName, string typeName)
        {
            if (string.IsNullOrEmpty(namespaceName))
            {
                return $"global.{typeName}";
            }

            return $"global.{namespaceName}.{typeName}";
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

            // This should be same with _factoryName
            string factoryName = XamlResourcesHelper.GenerateClassNameFromComponentUri(absoluteSourceUri);

            string finalCode = $@"
#nowarn ""3391""
#nowarn ""0067""
#nowarn ""0044""

/// <summary>
/// {factoryName}
/// </summary>
[<global.System.Diagnostics.DebuggerNonUserCode>]
[<global.System.ComponentModel.EditorBrowsable(global.System.ComponentModel.EditorBrowsableState.Never)>]
type {factoryName}() =
    /// <summary>
    /// Instantiate
    /// </summary>
    [<global.System.ComponentModel.EditorBrowsable(global.System.ComponentModel.EditorBrowsableState.Never)>]
    static member public Instantiate(): obj =
        {factoryName}.CreateComponentImpl()
    interface {IXamlComponentFactoryClass}<{componentTypeFullName}> with
        member this.CreateComponent(): obj = 
            {factoryName}.CreateComponentImpl()
        member this.CreateComponent() = 
            {factoryName}.CreateComponentImpl()
    interface {IXamlComponentLoaderClass}<{baseTypeFullName}> with
        member this.LoadComponent(_component: obj): unit = 
            {factoryName}.LoadComponentImpl(_component :?> {baseTypeFullName})
        member this.LoadComponent(_component: {baseTypeFullName}): unit = 
            {factoryName}.LoadComponentImpl(_component)
    static member private LoadComponentImpl({componentParamName}: {baseTypeFullName}) =
        if (box {componentParamName} :? {uiElementFullyQualifiedTypeName}) then
            (box {componentParamName} :?> {uiElementFullyQualifiedTypeName}).XamlSourcePath <- @""{assemblyName}\{fileNameWithPathRelativeToProjectRoot}""
{loadComponentImpl}
    static member private CreateComponentImpl() : {componentTypeFullName} =
{createComponentImpl}

{string.Join(Environment.NewLine + Environment.NewLine, additionalMethods)}
";

            return finalCode;
        }

        private const string RuntimeHelperClass = "global.OpenSilver.Internal.Xaml.RuntimeHelpers";
        private const string IXamlComponentFactoryClass = "global.OpenSilver.Internal.Xaml.IXamlComponentFactory";
        private const string IXamlComponentLoaderClass = "global.OpenSilver.Internal.Xaml.IXamlComponentLoader";
        private const string IComponentConnectorClass = "global.OpenSilver.Internal.Xaml.IComponentConnector";
        private const string XamlContextClass = "global.OpenSilver.Internal.Xaml.Context.XamlContext";
        private const string IMarkupExtensionClass = "global.System.Xaml.IMarkupExtension<obj>";
        private const string XamlDesignerBridgeClass = "global.OpenSilver.Internal.Xaml.XamlDesignerBridge";
    }
}
