

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
        internal static readonly XNamespace DefaultXamlNamespace = @"http://schemas.microsoft.com/winfx/2006/xaml/presentation";
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
            string rootElementName,
            string codeToWorkWithTheRootElement, 
            List<string> findNameCalls, 
            string codeToPlaceAtTheBeginningOfInitializeComponent, 
            string codeToPlaceAtTheEndOfInitializeComponent, 
            string nameScope,
            bool isSLMigration, 
            string assemblyNameWithoutExtension, 
            string fileNameWithPathRelativeToProjectRoot)
        {
            string uiElementFullyQualifiedTypeName = isSLMigration ? "global::System.Windows.UIElement" : "global::Windows.UI.Xaml.UIElement";

            string body = CreateMethodBody(
                codeToWorkWithTheRootElement,
                findNameCalls,
                codeToPlaceAtTheBeginningOfInitializeComponent,
                codeToPlaceAtTheEndOfInitializeComponent,
                nameScope);

            return $@"
        private bool _contentLoaded;
        public void InitializeComponent()
        {{
            if (_contentLoaded)
                return;
            _contentLoaded = true;

#pragma warning disable 0184 // Prevents warning CS0184 ('The given expression is never of the provided ('type') type')
            if (this is {uiElementFullyQualifiedTypeName})
            {{
                (({uiElementFullyQualifiedTypeName})(object)this).XamlSourcePath = @""{assemblyNameWithoutExtension}\{fileNameWithPathRelativeToProjectRoot}"";
            }}
#pragma warning restore 0184

            var {rootElementName} = this;
            {body}
        }}
";
        }

        private static string GetUniqueName(XElement element)
        {
            return element.Attribute(GeneratingUniqueNames.UniqueNameAttribute).Value;
        }

        private static string GeneratePartialClass(
            List<string> methods,
            List<string> fieldsForNamedElements,
            string className,
            string namespaceStringIfAny,
            string baseType,
            string fileNameWithPathRelativeToProjectRoot,
            string assemblyNameWithoutExtension,
            HashSet<string> listOfAllTheTypesUsedInThisXamlFile,
            bool hasCodeBehind,
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

            string absoluteSourceUri = 
                fileNameWithPathRelativeToProjectRoot.Contains(';') ? 
                fileNameWithPathRelativeToProjectRoot : 
                "/" + assemblyNameWithoutExtension + ";component/" + fileNameWithPathRelativeToProjectRoot;
            
            string classToInstantiateName = XamlResourcesHelper.GenerateClassNameFromComponentUri(absoluteSourceUri);

            string methodsMergedCode = string.Join(Environment.NewLine + Environment.NewLine, methods);
            
            string fieldsForNamedElementsMergedCode = string.Join(Environment.NewLine, fieldsForNamedElements);

            // Note: This is useful because we need to generate some c# code for every type used in the XAML
            // file because otherwise the types risk not being found at "Pass2" of the compilation. In fact,
            // Visual Studio automatically removes project references that are not referenced from C#, so if
            // a type is present only in XAML and not in C#, its DLL risks not being referenced.
            string fieldsToEnsureThatAllTypesReferencedInTheXamlFileAreReferenced = 
                string.Join(
                    Environment.NewLine, 
                    listOfAllTheTypesUsedInThisXamlFile.Select(
                        x => $"private {x} {GeneratingUniqueNames.GenerateUniqueNameFromString("Unused")};"
                    )
                );

            string classAccessModifier = hasCodeBehind ? "" : "public ";

            string classCodeFilled = $@"
public partial class {className} : {baseType}
{{

#pragma warning disable 169, 649, 0628 // Prevents warning CS0169 ('field ... is never used'), CS0649 ('field ... is never assigned to, and will always have its default value null'), and CS0628 ('member : new protected member declared in sealed class')
{fieldsForNamedElementsMergedCode}

{fieldsToEnsureThatAllTypesReferencedInTheXamlFileAreReferenced}
#pragma warning restore 169, 649, 0628

{methodsMergedCode}

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
            string factoryImpl,
            IEnumerable<string> additionalMethods,
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

[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
public sealed class {factoryName} : {IXamlComponentFactoryClass}<{componentTypeFullName}>
{{
    public static object Instantiate()
    {{
        return new {factoryName}().CreateComponentImpl();
    }}

    {componentTypeFullName} {IXamlComponentFactoryClass}<{componentTypeFullName}>.CreateComponent()
    {{
        return CreateComponentImpl();
    }}

    object {IXamlComponentFactoryClass}.CreateComponent()
    {{
        return CreateComponentImpl();
    }}
    {factoryImpl}

    {string.Join(Environment.NewLine + Environment.NewLine, additionalMethods)}
}}
";

            return finalCode;
        }

        private static string CreateFactoryMethod(
            string rootElementName,
            string typeFullName,
            string codeToWorkWithTheRootElement,
            List<string> findNameCalls,
            string codeToPlaceAtTheBeginningOfInitializeComponent,
            string codeToPlaceAtTheEndOfInitializeComponent,
            string nameScope,
            bool isSLMigration,
            string assemblyNameWithoutExtension,
            string fileNameWithPathRelativeToProjectRoot)
        {
            string body = CreateMethodBody(
                codeToWorkWithTheRootElement,
                findNameCalls,
                codeToPlaceAtTheBeginningOfInitializeComponent,
                codeToPlaceAtTheEndOfInitializeComponent,
                nameScope);
            string uiElementFullName = isSLMigration ? "global::System.Windows.UIElement" : "global::Windows.UI.Xaml.UIElement";

            return $@"
    private {typeFullName} CreateComponentImpl()
    {{
        var {rootElementName} = new {typeFullName}();
    
#pragma warning disable 0184 // Prevents warning CS0184 ('The given expression is never of the provided ('type') type')
        if ({rootElementName} is {uiElementFullName})
        {{
            (({uiElementFullName})(object){rootElementName}).XamlSourcePath = @""{assemblyNameWithoutExtension}\{fileNameWithPathRelativeToProjectRoot}"";
        }}
#pragma warning restore 0184
    
        {body}
    
        return {rootElementName};
    }}";
        }

        private static string CreateFactoryMethod(string componentType)
        {
            return $@"
    private {componentType} CreateComponentImpl()
    {{
        return ({componentType})global::CSHTML5.Internal.TypeInstantiationHelper.Instantiate(typeof({componentType}));
    }}
";
        }

        private static string CreateMethodBody(
            string codeToWorkWithTheRootElement,
            List<string> findNameCalls,
            string codeToPlaceAtTheBeginningOfInitializeComponent,
            string codeToPlaceAtTheEndOfInitializeComponent,
            string nameScope)
        {
            return $@"
{codeToPlaceAtTheBeginningOfInitializeComponent}
{codeToWorkWithTheRootElement}
{string.Join("\r\n", findNameCalls)}
{nameScope}
{codeToPlaceAtTheEndOfInitializeComponent}    
";
        }

        private const string RuntimeHelperClass = "global::OpenSilver.Internal.Xaml.RuntimeHelpers";
        private const string IXamlComponentFactoryClass = "global::OpenSilver.Internal.Xaml.IXamlComponentFactory";
    }
}
