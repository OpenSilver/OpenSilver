
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

            public int Connect(string componentType, string eventName, string handlerName)
            {
                int componentId = _entries.Count;
                _entries.Add(new ComponentConnectorEntry
                {
                    componentType = componentType,
                    eventName = eventName,
                    handlerName = handlerName
                });

                return componentId;
            }

            public override string ToString()
            {
                StringBuilder builder = new StringBuilder();

                builder.Append(' ', 4).AppendLine("interface global.OpenSilver.Internal.Xaml.IComponentConnector with")
                    .Append(' ', 4 * 2).AppendLine("[<global.System.Diagnostics.DebuggerNonUserCode>]")
                    .Append(' ', 4 * 2).AppendLine("[<global.System.ComponentModel.EditorBrowsable(global.System.ComponentModel.EditorBrowsableState.Never)>]")
                    .Append(' ', 4 * 2 ).AppendLine($"member this.Connect({componentIdParam}: int, {targetParam}: obj): unit = ");

                if (_entries.Count > 0)
                {
                    builder.Append(' ', 4 * 3).AppendLine($"match {componentIdParam} with");

                    for (int componentId = 0; componentId < _entries.Count; componentId++)
                    {
                        ComponentConnectorEntry eventEntry = _entries[componentId];
                        builder.Append(' ', 4 * 4).AppendLine($"| {componentId} ->");
                        builder.Append(' ', 4 * 5).AppendLine($"{RuntimeHelperClass}.RegisterEventHandler(\"{eventEntry.handlerName}\", \"{eventEntry.eventName}\", {targetParam}, this)");
                    }

                    builder.Append(' ', 4 * 4).AppendLine("| _ -> ()");
                }
                else
                {
                    builder.Append(' ', 4 * 3).AppendLine("()");
                }

                return builder.ToString();
            }

            private struct ComponentConnectorEntry
            {
                public string componentType;
                public string eventName;
                public string handlerName;
            }
        }

        public static string GenerateCode(XDocument doc,
            string sourceFile,
            string fileNameWithPathRelativeToProjectRoot,
            string assemblyNameWithoutExtension,
            string rootNamespace,
            AssembliesInspector reflectionOnSeparateAppDomain,
            bool isFirstPass,
            ConversionSettings settings,
            string codeToPutInTheInitializeComponentOfTheApplicationClass)
        {
            ICodeGenerator generator;
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
                    codeToPutInTheInitializeComponentOfTheApplicationClass);
            }

            return generator.Generate();
        }

        private static string CreateInitializeComponentMethod(
            string applicationTypeFullName,
            string additionalCodeForApplication,
            string assemblyNameWithoutExtension,
            string fileNameWithPathRelativeToProjectRoot,
            List<string> findNameCalls)
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

        {additionalCodeForApplication}
        {loadComponentCall}
        {string.Join(Environment.NewLine + "        ", findNameCalls)}
";
        }


        private static string GeneratePartialClass(
            string additionalConstructors,
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

{additionalConstructors}

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

[<global.System.Diagnostics.DebuggerNonUserCode>]
[<global.System.ComponentModel.EditorBrowsable(global.System.ComponentModel.EditorBrowsableState.Never)>]
type {factoryName}() =
    static member public Instantiate(): obj =
        {factoryName}.CreateComponentImpl()
    interface {IXamlComponentFactoryClass}<{componentTypeFullName}> with
        member this.CreateComponent(): obj = 
            {factoryName}.CreateComponentImpl()
        member this.CreateComponent() = 
            {factoryName}.CreateComponentImpl()
    interface {IXamlComponentLoaderClass}<{componentTypeFullName}> with
        member this.LoadComponent(_component: obj): unit = 
            {factoryName}.LoadComponentImpl(_component :?> {componentTypeFullName})
        member this.LoadComponent(_component: {componentTypeFullName}): unit = 
            {factoryName}.LoadComponentImpl(_component)
    static member private LoadComponentImpl({componentParamName}: {componentTypeFullName}) =
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
