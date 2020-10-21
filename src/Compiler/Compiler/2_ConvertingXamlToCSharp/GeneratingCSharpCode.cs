

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



#if !BRIDGE && !CSHTML5BLAZOR
extern alias custom;
#endif
extern alias wpf;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
#if !BRIDGE && !CSHTML5BLAZOR
using custom::System.Windows.Markup;
#endif
using System.IO;

namespace DotNetForHtml5.Compiler
{
    internal static class GeneratingCSharpCode
    {
        internal static readonly XNamespace DefaultXamlNamespace = @"http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        internal static readonly XNamespace xNamespace = @"http://schemas.microsoft.com/winfx/2006/xaml"; // Used for example for "x:Name" attributes and {x:Null} markup extensions.
        const string TemplateOwnerValuePlaceHolder = "TemplateOwnerValuePlaceHolder";

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
            if (isFirstPass)
            {
                return GenerateCSharpCodeForPass1(doc, fileNameWithPathRelativeToProjectRoot, assemblyNameWithoutExtension,
                    isSLMigration, reflectionOnSeparateAppDomain);
            }
            else
            {
                return GenerateCSharpCodeForPass2(doc, sourceFile, fileNameWithPathRelativeToProjectRoot, assemblyNameWithoutExtension,
                    reflectionOnSeparateAppDomain, isSLMigration, codeToPutInTheInitializeComponentOfTheApplicationClass, logger);
            }
        }

        public static string GenerateCSharpCodeForPass1(XDocument doc,
            string fileNameWithPathRelativeToProjectRoot,
            string assemblyNameWithoutExtension,
            bool isSLMigration,
            ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain)
        {
            HashSet<string> listOfAllTheTypesUsedInThisXamlFile = new HashSet<string>();
            List<string> resultingFieldsForNamedElements = new List<string>();
            List<string> resultingMethods = new List<string>();

            foreach (var element in PostOrderTreeTraversal.TraverseTreeInPostOrder(doc.Root).Where(e => !e.Name.LocalName.Contains(".")))
            {
                // Get the namespace, local name, and optional assembly that correspond to the element
                string namespaceName, localTypeName, assemblyNameIfAny;
                GettingInformationAboutXamlTypes.GetClrNamespaceAndLocalName(element.Name, out namespaceName, out localTypeName, out assemblyNameIfAny);
                string elementTypeInCSharp = reflectionOnSeparateAppDomain.GetCSharpEquivalentOfXamlTypeAsString(
                    namespaceName, localTypeName, assemblyNameIfAny, true);

                listOfAllTheTypesUsedInThisXamlFile.Add(elementTypeInCSharp);

                XAttribute xNameAttr = element.Attributes().FirstOrDefault(attr => IsAttributeTheXNameAttribute(attr));
                if (xNameAttr != null && GetRootOfCurrentNamescopeForCompilation(element).Parent == null)
                {
                    string name = xNameAttr.Value;
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        string fieldModifier = isSLMigration ? "internal" : "protected";
                        XAttribute fieldModifierAttr = element.Attribute(xNamespace + "FieldModifier");
                        if (fieldModifierAttr != null)
                        {
                            fieldModifier = fieldModifierAttr.Value?.ToLower() ?? "private";
                        }

                        resultingFieldsForNamedElements.Add(string.Format("{0} {1} {2};", fieldModifier, elementTypeInCSharp, name));
                    }
                }
            }

            // Get general information about the class:
            string className, namespaceStringIfAny, baseType;
            bool hasCodeBehind;
            GetClassInformationFromXaml(doc, fileNameWithPathRelativeToProjectRoot, assemblyNameWithoutExtension, reflectionOnSeparateAppDomain,
                out className, out namespaceStringIfAny, out baseType, out hasCodeBehind);

            // Create the "IntializeComponent()" method:
            string initializeComponentMethod = CreateInitializeComponentMethod(null, new List<string>(0), null, null,
                isSLMigration, assemblyNameWithoutExtension, fileNameWithPathRelativeToProjectRoot);

            resultingMethods.Add(initializeComponentMethod);

            // Add a contructor if there is no code behind:
            if (!hasCodeBehind)
            {
                string uiElementFullyQualifiedTypeName = isSLMigration ? "global::System.Windows.UIElement" : "global::Windows.UI.Xaml.UIElement";
                resultingMethods.Add(string.Format(@"
        public {0}()
        {{
            this.InitializeComponent();
#pragma warning disable 0184 // Prevents warning CS0184 ('The given expression is never of the provided ('type') type')
            if (this is {1})
            {{
                (({1})(object)this).XamlSourcePath = @""{2}\{3}"";
            }}
#pragma warning restore 0184
        }}
", className, uiElementFullyQualifiedTypeName, assemblyNameWithoutExtension, fileNameWithPathRelativeToProjectRoot));
            }

            // Wrap everything into a partial class:
            string finalCode = GeneratePartialClass(resultingMethods,
                                                    resultingFieldsForNamedElements,
                                                    className,
                                                    namespaceStringIfAny,
                                                    baseType,
                                                    fileNameWithPathRelativeToProjectRoot,
                                                    assemblyNameWithoutExtension,
                                                    listOfAllTheTypesUsedInThisXamlFile,
                                                    hasCodeBehind,
#if BRIDGE
                                                    addApplicationEntryPoint: IsClassTheApplicationClass(baseType)
#else
                                                    addApplicationEntryPoint: false
#endif
);

            return finalCode;
        }

        public static string GenerateCSharpCodeForPass2(XDocument doc,
            string sourceFile,
            string fileNameWithPathRelativeToProjectRoot,
            string assemblyNameWithoutExtension,
            ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain,
            bool isSLMigration,
            string codeToPutInTheInitializeComponentOfTheApplicationClass,
            ILogger logger)
        {
            List<string> resultingMethods = new List<string>();
            List<string> resultingFieldsForNamedElements = new List<string>(); // Such as: "protected TextBox MyTextBox1;"
            List<string> resultingFindNameCalls = new List<string>();

            Dictionary<XElement, Dictionary<string, string>> namescopeRootToNameToUniqueNameDictionary = new Dictionary<XElement, Dictionary<string, string>>(); // This contains the association between an element name (defined with x:Name="..." or Name="") and its "INTERNAL_UniqueName" (that is also the local C# code name of the variable that holds the reference to the element).
            HashSet<string> listOfAllTheTypesUsedInThisXamlFile = new HashSet<string>(); // This list contains all the types referenced by this XAML file. It is useful because we need to generate some c# code for every type (basically a simple useless field declaration for each type) because otherwise the types risk not being found at "Pass2" of the compilation. In fact, Visual Studio automatically removes project references that are not referenced from C#, so if a type is present only in XAML and not in C#, its DLL risks not being referenced.
            Stack<string> codeStack = new Stack<string>();
            StringBuilder stringBuilder = new StringBuilder();

            string namespaceSystemWindows = (isSLMigration ? "global::System.Windows" : "global::Windows.UI.Xaml");
            string namespaceSystemWindowsControls = (isSLMigration ? "global::System.Windows.Controls" : "global::Windows.UI.Xaml.Controls");
            string namespaceSystemWindowsData = (isSLMigration ? "global::System.Windows.Data" : "global::Windows.UI.Xaml.Data");
            string namespaceSystemWindowsMediaAnimation = (isSLMigration ? "global::System.Windows.Media.Animation" : "global::Windows.UI.Xaml.Media.Animation");

            // The code contained in the following dictionaries will be placed at the end of the "InitializeComponent" method if they were defined in the root namescope, or at the end of the FrameworkTemplate code if they were defined inside an "InstantiateFrameworkTemplate" method.
            // Note: "namescopeRoot" is the XElement that is the root of a namescope (for example, a DataTemplate, a ControlTemplate, or the root of the whole XAML).
            Dictionary<XElement, List<string>> namescopeRootToMarkupExtensionsAdditionalCode = new Dictionary<XElement, List<string>>();
            Dictionary<XElement, List<string>> namescopeRootToStoryboardsAdditionalCode = new Dictionary<XElement, List<string>>();

            // Populate the dictionary that associates names to unique names. This needs to be done before the main parsing, 
            // because elements positioned early in the XAML file may reference other elements that are positioned later in the XAML file:
            Dictionary<XElement, Dictionary<string, string>> namescopeRootToElementsUniqueNameToInstantiatedObjects = new Dictionary<XElement, Dictionary<string, string>>();
            PopulateDictionaryThatAssociatesNamesToUniqueNames(doc, namescopeRootToNameToUniqueNameDictionary, namescopeRootToElementsUniqueNameToInstantiatedObjects, reflectionOnSeparateAppDomain);

            // Traverse the tree in "post order" (ie. start with child elements then traverse parent elements):
            foreach (var element in PostOrderTreeTraversal.TraverseTreeInPostOrder(doc.Root))
            {
                try
                {
                    // Get the namespace, local name, and optional assembly that correspond to the element:
                    string namespaceName, localTypeName, assemblyNameIfAny;
                    GettingInformationAboutXamlTypes.GetClrNamespaceAndLocalName(element.Name, out namespaceName, out localTypeName, out assemblyNameIfAny);

                    // Get information about which element holds the namescope of the current element. For example, if the current element is inside a DataTemplate, the DataTemplate is the root of the namescope of the current element. If the element is not inside a DataTemplate or ControlTemplate, the root of the XAML is the root of the namescope of the current element.
                    XElement elementThatIsRootOfTheCurrentNamescope = GetRootOfCurrentNamescopeForRuntime(element, reflectionOnSeparateAppDomain);
                    bool isElementInRootNamescope = (elementThatIsRootOfTheCurrentNamescope.Parent == null); // Check if the root of the current namescope is also the root of the XAML (note: to be the root of the XAML means that the parent is null).

                    bool isAProperty = element.Name.LocalName.Contains('.');

                    #region CASE: the element is a Property
                    if (isAProperty)
                    // Check if the element is a property:
                    {
                        //------------------------------
                        // IF THE ELEMENT IS A PROPERTY:
                        //------------------------------

                        // Get information about the parent element (to which the property applies) and the element itself:
                        var parentElement = element.Parent;
                        string parentElementUniqueNameOrThisKeyword = GetUniqueName(parentElement);
                        string typeName = element.Name.LocalName.Split('.')[0];
                        string propertyName = element.Name.LocalName.Split('.')[1];
                        XName elementName = element.Name.Namespace + typeName; // eg. if the element is <VisualStateManager.VisualStateGroups>, this will be "DefaultNamespace+VisualStateManager"

                        stringBuilder.Clear();

                        //Special case: MergedDictionaries. We need to handle these differently because we cannot create all the dictionaries then create them because they may need the resources from one another so we need to add them one by one.
                        if (elementName.Namespace == DefaultXamlNamespace && elementName.LocalName == "ResourceDictionary" && propertyName == "MergedDictionaries")
                        {
                            // Add the code of the children elements:
                            var childrenElements = element.Elements();
                            int currentChildIndex = 0;
                            int childrenCount = childrenElements.Count(); //todo-performance: find a more performant way to count the children?
                            foreach (var childCode in PopElementsFromStackAndReadThemInReverseOrder(codeStack, childrenCount)) // Note: this is supposed to not raise OutOfIndex because child nodes are supposed to have added code to the stack.
                            {
                                stringBuilder.AppendLine(childCode);
                                string childUniqueName = GetUniqueName(childrenElements.ElementAt(currentChildIndex));
                                stringBuilder.AppendLine(string.Format("{0}.{1}.Add({2});", parentElementUniqueNameOrThisKeyword, propertyName, childUniqueName));
                                ++currentChildIndex;
                            }
                        }
                        else
                        {
                            bool isFrameworkTemplateContentProperty = propertyName == "ContentPropertyUsefulOnlyDuringTheCompilation" &&
                               reflectionOnSeparateAppDomain.IsAssignableFrom(namespaceSystemWindows, "FrameworkTemplate", element.Name.NamespaceName, typeName);

                            if (isFrameworkTemplateContentProperty)
                            {
                                //------------------------------
                                // IF THE ELEMENT IS A DATATEMPLATE PROPERTY:
                                //------------------------------

                                if (codeStack.Count == 0 || element.Elements().FirstOrDefault() == null)
                                    throw new wpf::System.Windows.Markup.XamlParseException(string.Format("A {0} cannot be empty.", typeName));
                                else if (element.Elements().Count() > 1)
                                    throw new wpf::System.Windows.Markup.XamlParseException(string.Format("A {0} cannot contain more than one child element.", typeName));

                                // Create the method to instantiate the DataTemplate:
                                //we create a frameworkTemplate element to allow Binding with RelativeSource = TemplatedParent:
                                string templateInstanceUniqueName = GeneratingUniqueNames.GenerateUniqueNameFromString("templateInstance");

                                string codeToInstantiateTheDataTemplate = codeStack.Pop();

                                //we replace the Placeholder that was put for the template name:
                                codeToInstantiateTheDataTemplate = codeToInstantiateTheDataTemplate.Replace(GeneratingCSharpCode.TemplateOwnerValuePlaceHolder, templateInstanceUniqueName);

                                XElement frameworkTemplateRoot = element.Parent;
                                string frameworkTemplateUniqueName = GetUniqueName(frameworkTemplateRoot);
                                string childUniqueName = GetUniqueName(element.Elements().First());
                                string objectsToInstantiateAtTheBeginningOfTheDataTemplate = string.Join("\r\n",
                                   GetNameToUniqueNameDictionary(frameworkTemplateRoot,
                                       namescopeRootToElementsUniqueNameToInstantiatedObjects).Select(x => x.Value));
                                string markupExtensionsAdditionalCode = string.Join("\r\n",
                                    GetListThatContainsAdditionalCodeFromDictionary(frameworkTemplateRoot,
                                        namescopeRootToMarkupExtensionsAdditionalCode));
                                string storyboardsAdditionalCode = string.Join("\r\n",
                                    GetListThatContainsAdditionalCodeFromDictionary(frameworkTemplateRoot,
                                        namescopeRootToStoryboardsAdditionalCode));
                                string additionalCodeToPlaceAtTheEndOfTheMethod = markupExtensionsAdditionalCode + Environment.NewLine + storyboardsAdditionalCode;

                                string dataTemplateMethod = CreateDataTemplateLambda(codeToInstantiateTheDataTemplate,
                                    frameworkTemplateUniqueName,
                                    childUniqueName,
                                    templateInstanceUniqueName,
                                    objectsToInstantiateAtTheBeginningOfTheDataTemplate,
                                    additionalCodeToPlaceAtTheEndOfTheMethod,
                                    namespaceSystemWindows);
                                // Create the code that sets the "MethodToInstantiateDataTemplate":
                                string codeToSetTheMethod = string.Format("{0}.SetMethodToInstantiateFrameworkTemplate({1});", frameworkTemplateUniqueName, dataTemplateMethod);

                                stringBuilder.AppendLine(codeToSetTheMethod);
                            }
                            else
                            {
                                bool isAttachedProperty = GettingInformationAboutXamlTypes.IsPropertyAttached(element, reflectionOnSeparateAppDomain); //(parentElement.Name != elementName) && !GettingInformationAboutXamlTypes.IsTypeAssignableFrom(parentElement.Name, elementName, reflectionOnSeparateAppDomain); // Note: the comparison includes the namespace. // eg. <Grid><VisualStateManager.VisualStateGroups>...</VisualStateManager.VisualStateGroups></Grid> should return "true", while <n:MyUserControl><UserControl.Resources>...</n:MyUserControl></UserControl.Resources> should return "false".

                                // Check if the property is a collection, in which case we must use ".Add(...)", otherwise a simple "=" is enough:
                                if (GettingInformationAboutXamlTypes.IsPropertyOrFieldACollection(element, reflectionOnSeparateAppDomain, isAttachedProperty)
                                    && (element.Elements().Count() != 1
                                    || (!GettingInformationAboutXamlTypes.IsTypeAssignableFrom(element.Elements().First().Name, element.Name, reflectionOnSeparateAppDomain, isAttached: isAttachedProperty)) // To handle the case where the user explicitly declares the collection element. Example: <Application.Resources><ResourceDictionary><Child x:Key="test"/></ResourceDictionary></Application.Resources> (rather than <Application.Resources><Child x:Key="test"/></Application.Resources>), in which case we need to do "=" instead pf "Add()"
                                    && element.Elements().First().Name != DefaultXamlNamespace + "Binding" 
                                    && element.Elements().First().Name.LocalName != "StaticResourceExtension" 
                                    && element.Elements().First().Name.LocalName != "StaticResource"
                                    && element.Elements().First().Name.LocalName != "TemplateBinding"
                                    && element.Elements().First().Name.LocalName != "TemplateBindingExtension"))
                                {
                                    //------------------------
                                    // PROPERTY TYPE IS A COLLECTION
                                    //------------------------

                                    // Determine if the property is a collection or a dictionary:
                                    EnumerableType enumerableType;
                                    if (GettingInformationAboutXamlTypes.IsPropertyOrFieldADictionary(element, reflectionOnSeparateAppDomain, isAttachedProperty))
                                        enumerableType = EnumerableType.Dictionary;
                                    else
                                        enumerableType = EnumerableType.Collection;

                                    string codeToAccessTheEnumerable;
                                    if (isAttachedProperty)
                                    {
                                        string elementTypeInCSharp = reflectionOnSeparateAppDomain.GetCSharpEquivalentOfXamlTypeAsString(
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

                                    // Add the children to the collection/dictionary:
                                    GenerateCodeForAddingChildrenToCollectionOrDictionary(
                                        codeStack: codeStack,
                                        stringBuilder: stringBuilder,
                                        enumerableType: enumerableType,
                                        codeToAccessTheEnumerable: codeToAccessTheEnumerable,
                                        elementThatContainsTheChildrenToAdd: element,
                                        reflectionOnSeparateAppDomain: reflectionOnSeparateAppDomain);
                                }
                                else
                                {
                                    //------------------------
                                    // PROPERTY TYPE IS NOT A COLLECTION
                                    //------------------------

                                    // Add the code of the children elements:
                                    int childrenCount = element.Elements().Count(); //todo-performance: find a more performant way to count the children?
                                    foreach (var childCode in PopElementsFromStackAndReadThemInReverseOrder<string>(codeStack, childrenCount)) // Note: this is supposed to not raise OutOfIndex because child nodes are supposed to have added code to the stack.
                                    {
                                        stringBuilder.AppendLine(childCode);
                                    }

                                    bool first = true;
                                    foreach (XElement child in element.Elements())
                                    {
                                        if (!first)
                                        {
                                            //TODO: check wether WPF & UWP also allow that silently
                                            //throw new wpf::System.Windows.Markup.XamlParseException(string.Format("The property '{0}' is set more than once.", propertyName));
                                            logger.WriteWarning($"The property \"{propertyName}\" is set more than once.", sourceFile, GetLineNumber(element));
                                        }

                                        string childUniqueName = GetUniqueName(child);
                                        if (!GettingInformationAboutXamlTypes.IsElementAMarkupExtension(child, reflectionOnSeparateAppDomain)
                                            || (child.Name.LocalName == "RelativeSource")) // Note about "RelativeSource": even though it inherits from "MarkupExtension", we do not was to consider "RelativeSource" as a markup extension for the compilation because it is only meant to be used WITHIN another markup extension (sort of a "nested" markup extension), such as in: "{Binding Background, RelativeSource={RelativeSource Mode=TemplatedParent}}"
                                        {
                                            if (isAttachedProperty)
                                            {
                                                string elementTypeInCSharp = reflectionOnSeparateAppDomain.GetCSharpEquivalentOfXamlTypeAsString(elementName.Namespace.NamespaceName, elementName.LocalName, assemblyNameIfAny);
                                                stringBuilder.AppendLine(string.Format("{0}.Set{1}({2}, {3});", elementTypeInCSharp, propertyName, parentElementUniqueNameOrThisKeyword, childUniqueName)); // eg. MyCustomGridClass.SetRow(grid32877267T6, int45628789434);
                                            }
                                            else
                                            {
                                                stringBuilder.AppendLine(string.Format("{0}.{1} = {2};", parentElementUniqueNameOrThisKeyword, propertyName, childUniqueName));
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

                                                //we generate a list of the parents of the element so that we can search their resources if needed
                                                XElement elementForSearch = element.Parent;
                                                string nameForParentsCollection = GeneratingUniqueNames.GenerateUniqueNameFromString("parents"); // Example: parents_4541C363579C48A981219C392BF8ACD5
                                                stringBuilder.AppendLine(string.Format("var {0} = new global::System.Collections.Generic.List<global::System.Object>();",
                                                    nameForParentsCollection));
                                                while (elementForSearch != null && elementForSearch.Parent != null) //we check for the parent because the last parent will be the container(for example a Page) and the generated code doesn't create an instance for itself.
                                                {
                                                    if (elementForSearch != elementThatIsRootOfTheCurrentNamescope)
                                                    {
                                                        if (!elementForSearch.Name.LocalName.Contains('.'))
                                                        {
                                                            if (!GettingInformationAboutXamlTypes.IsElementAMarkupExtension(elementForSearch, reflectionOnSeparateAppDomain)) //we don't want to add the MarkupExtensions in the list of the parents (A MarkupExtension is not a DependencyObject)
                                                            {
                                                                stringBuilder.AppendLine(string.Format("{0}.Add({1});",
                                                                    nameForParentsCollection, GetUniqueName(elementForSearch)));
                                                            }
                                                        }
                                                    }
                                                    elementForSearch = elementForSearch.Parent;
                                                }
                                                stringBuilder.AppendLine(string.Format("{0}.Add(this);", nameForParentsCollection));

                                                string[] splittedLocalName = element.Name.LocalName.Split('.');
                                                string propertyKey = GettingInformationAboutXamlTypes.GetKeyNameOfProperty(parent, splittedLocalName[1], reflectionOnSeparateAppDomain);
                                                string propertyKeyString = propertyKey != null ? propertyKey : "null";
                                                string elementTypeInCSharp;
                                                string propertyNamespaceName, propertyLocalTypeName;
                                                bool isTypeString, isTypeEnum;
                                                // Attached property
                                                if (isAttachedProperty)
                                                {
                                                    elementTypeInCSharp = reflectionOnSeparateAppDomain.GetCSharpEquivalentOfXamlTypeAsString(element.Name.NamespaceName, splittedLocalName[0], assemblyNameIfAny);
                                                    reflectionOnSeparateAppDomain.GetPropertyOrFieldTypeInfo(propertyName, element.Name.NamespaceName, splittedLocalName[0], out propertyNamespaceName, out propertyLocalTypeName, out isTypeString, out isTypeEnum, assemblyNameIfAny, isAttached: true);
                                                    stringBuilder.AppendLine(string.Format("{0}.Set{1}({2},({3})({4}.ProvideValue(new global::System.ServiceProvider({2}, {5}, {6}))));",
                                                        elementTypeInCSharp,
                                                        propertyName,
                                                        GetUniqueName(parent),
                                                        "global::" + (!string.IsNullOrEmpty(propertyNamespaceName) ? propertyNamespaceName + "." : "") + propertyLocalTypeName,
                                                        childUniqueName,
                                                        propertyKeyString,
                                                        nameForParentsCollection));
                                                }
                                                else
                                                {
                                                    elementTypeInCSharp = reflectionOnSeparateAppDomain.GetCSharpEquivalentOfXamlTypeAsString(parent.Name.NamespaceName, parent.Name.LocalName, assemblyNameIfAny);
                                                    reflectionOnSeparateAppDomain.GetPropertyOrFieldTypeInfo(propertyName, parent.Name.Namespace.NamespaceName, parent.Name.LocalName, out propertyNamespaceName, out propertyLocalTypeName, out isTypeString, out isTypeEnum, assemblyNameIfAny, isAttached: false);
                                                    stringBuilder.AppendLine(string.Format("{0}.{1} = ({2})({3}.ProvideValue(new global::System.ServiceProvider({0}, {4}, {5})));",
                                                        GetUniqueName(parent),
                                                        propertyName,
                                                        "global::" + (!string.IsNullOrEmpty(propertyNamespaceName) ? propertyNamespaceName + "." : "") + propertyLocalTypeName,
                                                        childUniqueName,
                                                        propertyKeyString,
                                                        nameForParentsCollection));
                                                }
                                            }
                                            else if (child.Name.LocalName == "Binding") //todo: verify that the namespace is the one that we used when we added the Binding to the XAML tree?
                                            {
                                                //------------------------------
                                                // {Binding ...}
                                                //------------------------------

                                                // Get a reference to the list to which we add the generated markup extensions code
                                                List<string> markupExtensionsAdditionalCode = GetListThatContainsAdditionalCodeFromDictionary(
                                                    elementThatIsRootOfTheCurrentNamescope, namescopeRootToMarkupExtensionsAdditionalCode);
                                                
                                                bool isDependencyProperty =
                                                    reflectionOnSeparateAppDomain.GetField(
                                                        propertyName + "Property", 
                                                        isAttachedProperty ? elementName.Namespace.NamespaceName : parent.Name.Namespace.NamespaceName,
                                                        isAttachedProperty ? elementName.LocalName : parent.Name.LocalName,
                                                        assemblyNameWithoutExtension) != null;

                                                string propertyDeclaringTypeName;
                                                string propertyTypeNamespace;
                                                string propertyTypeName;
                                                bool isTypeString;
                                                bool isTypeEnum;
                                                if (!isAttachedProperty)
                                                {
                                                    reflectionOnSeparateAppDomain.GetPropertyOrFieldInfo(propertyName,
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
                                                    reflectionOnSeparateAppDomain.GetMethodInfo("Get" + propertyName,
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

                                                if (BindingRelativeSourceIsTemplatedParent(child))
                                                {
                                                    stringBuilder.AppendLine(string.Format("{0}.TemplateOwner = {1};", GetUniqueName(child), GeneratingCSharpCode.TemplateOwnerValuePlaceHolder));
                                                }

                                                // Check if the property is of type "Binding" (or "BindingBase"), in which 
                                                // case we should directly assign the value instead of calling "SetBinding"
                                                bool isPropertyOfTypeBinding = (
                                                    (!isSLMigration && propertyTypeFullName == "Windows.UI.Xaml.Data.Binding")
                                                    || (!isSLMigration && propertyTypeFullName == "Windows.UI.Xaml.Data.BindingBase")
                                                    || (isSLMigration && propertyTypeFullName == "System.Windows.Data.Binding")
                                                    || (isSLMigration && propertyTypeFullName == "System.Windows.Data.BindingBase"));
                                                if (isPropertyOfTypeBinding || !isDependencyProperty)
                                                {
                                                    stringBuilder.AppendLine(string.Format("{0}.{1} = {2};", parentElementUniqueNameOrThisKeyword, propertyName, GetUniqueName(child)));
                                                }
                                                else
                                                {
                                                    markupExtensionsAdditionalCode.Add(string.Format("{3}.BindingOperations.SetBinding({0}, {1}, {2});", parentElementUniqueNameOrThisKeyword, propertyDeclaringTypeName + "." + propertyName + "Property", GetUniqueName(child), namespaceSystemWindowsData)); //we add the container itself since we couldn't add it inside the while
                                                }
                                            }
                                            else if (child.Name.LocalName == "TemplateBindingExtension")
                                            {
                                                var dependencyPropertyName =
                                                    "global::" + reflectionOnSeparateAppDomain.GetField(
                                                        propertyName + "Property",
                                                        isAttachedProperty ? elementName.Namespace.NamespaceName : parent.Name.Namespace.NamespaceName,
                                                        isAttachedProperty ? elementName.LocalName : parent.Name.LocalName,
                                                        assemblyNameWithoutExtension);

                                                stringBuilder.AppendLine(string.Format(
                                                    "{0}.SetValue({1}, {2}.ProvideValue(new global::System.ServiceProvider({3}.TemplateOwner, null)));",
                                                    parentElementUniqueNameOrThisKeyword,
                                                    dependencyPropertyName,
                                                    GetUniqueName(child),
                                                    TemplateOwnerValuePlaceHolder));
                                            }
                                            else if (child.Name == xNamespace + "NullExtension")
                                            {
                                                //------------------------------
                                                // {x:Null}
                                                //------------------------------

                                                if (isAttachedProperty)
                                                {
                                                    string elementTypeInCSharp = reflectionOnSeparateAppDomain.GetCSharpEquivalentOfXamlTypeAsString(elementName.Namespace.NamespaceName, elementName.LocalName, assemblyNameIfAny);
                                                    stringBuilder.AppendLine(string.Format("{0}.Set{1}({2}, null);", elementTypeInCSharp, propertyName, parentElementUniqueNameOrThisKeyword));
                                                }
                                                else
                                                {
                                                    stringBuilder.AppendLine(string.Format("{0}.{1} = null;", parentElementUniqueNameOrThisKeyword, propertyName));
                                                }
                                                //todo-perfs: avoid generating the line "var NullExtension_cfb65e0262594ddb87d60d8e776ce142 = new global::System.Windows.Markup.NullExtension();", which is never used. Such a line is generated when the user code contains a {x:Null} markup extension.
                                            }
                                            else
                                            {
                                                //------------------------------
                                                // Other (custom MarkupExtensions)
                                                //------------------------------

                                                string propertyKey = GettingInformationAboutXamlTypes.GetKeyNameOfProperty(parent, element.Name.LocalName.Split('.')[1], reflectionOnSeparateAppDomain);
                                                string propertyKeyString = propertyKey != null ? propertyKey : "null";

                                                if (isAttachedProperty)
                                                {
                                                    string elementTypeInCSharp = reflectionOnSeparateAppDomain.GetCSharpEquivalentOfXamlTypeAsString(
                                                        elementName.Namespace.NamespaceName,
                                                        elementName.LocalName,
                                                        assemblyNameIfAny);

                                                    string[] splittedLocalName = element.Name.LocalName.Split('.');

                                                    string propertyNamespaceName, propertyLocalTypeName;
                                                    bool isTypeString, isTypeEnum;
                                                    reflectionOnSeparateAppDomain.GetPropertyOrFieldTypeInfo(
                                                        propertyName,
                                                        element.Name.NamespaceName,
                                                        splittedLocalName[0],
                                                        out propertyNamespaceName,
                                                        out propertyLocalTypeName,
                                                        out isTypeString,
                                                        out isTypeEnum,
                                                        assemblyNameIfAny);

                                                    string propertyType = "global::" + (!string.IsNullOrEmpty(propertyNamespaceName) ? propertyNamespaceName + "." : "") + propertyLocalTypeName;

                                                    string markupExtension = string.Format("{0}.ProvideValue(new global::System.ServiceProvider({1}, {2}))",
                                                                                           childUniqueName,
                                                                                           GetUniqueName(parent),
                                                                                           propertyKeyString);

                                                    stringBuilder.AppendLine(
                                                        string.Format("{0}.Set{1}({2}, ({3}){4});",
                                                                      elementTypeInCSharp,
                                                                      propertyName,
                                                                      parentElementUniqueNameOrThisKeyword,
                                                                      propertyType,
                                                                      markupExtension));
                                                }
                                                else
                                                {
                                                    string propertyNamespaceName, propertyLocalTypeName;
                                                    bool isTypeString, isTypeEnum;

                                                    //Todo: remove what is irrelevant below:
                                                    //Note: the code was copy-pasted from the Binding section from here. It is because we need to call SetBinding if a Custom marckup expression returns a Binding.
                                                    string propertyDeclaringTypeName;
                                                    string propertyTypeNamespace;
                                                    string propertyTypeName;
                                                    reflectionOnSeparateAppDomain.GetPropertyOrFieldInfo(propertyName,
                                                                                                         parent.Name.Namespace.NamespaceName,
                                                                                                         parent.Name.LocalName,
                                                                                                         out propertyDeclaringTypeName,
                                                                                                         out propertyTypeNamespace,
                                                                                                         out propertyTypeName,
                                                                                                         out isTypeString,
                                                                                                         out isTypeEnum,
                                                                                                         assemblyNameIfAny,
                                                                                                         false);
                                    
                                                    reflectionOnSeparateAppDomain.GetPropertyOrFieldTypeInfo(
                                                        propertyName,
                                                        parent.Name.Namespace.NamespaceName,
                                                        parent.Name.LocalName,
                                                        out propertyNamespaceName,
                                                        out propertyLocalTypeName,
                                                        out isTypeString,
                                                        out isTypeEnum,
                                                        assemblyNameIfAny);


                                                    string customMarkupValueName = "customMarkupValue_" + Guid.NewGuid().ToString("N");

                                                    bool isDependencyProperty = reflectionOnSeparateAppDomain.GetField(
                                                        propertyName + "Property",
                                                        isAttachedProperty ? elementName.Namespace.NamespaceName : parent.Name.Namespace.NamespaceName,
                                                        isAttachedProperty ? elementName.LocalName : parent.Name.LocalName,
                                                        assemblyNameWithoutExtension) != null;

                                                    if (isDependencyProperty)
                                                    {
                                                        string bindingBaseTypeString = isSLMigration ? "System.Windows.Data.Binding" : "Windows.UI.Xaml.Data.Binding";

                                                        //todo: make this more readable by cutting it into parts ?
                                                        stringBuilder.AppendLine(
                                                            string.Format(@"var {0} = {1}.ProvideValue(new global::System.ServiceProvider({2}, {3}));
if({0} is {4})
{{
    {9}.BindingOperations.SetBinding({7}, {8}, ({4}){0});
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
                                                                          namespaceSystemWindowsData//9
                                                                          ));
                                                    }
                                                    else
                                                    {
                                                        stringBuilder.AppendLine(
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
                                        first = false;
                                    }
                                }
                            }
                        }
                        // Put the whole code into the stack:
                        var resultingForTheElementAndAllItsChildren = stringBuilder.ToString();
                        codeStack.Push(resultingForTheElementAndAllItsChildren);
                    }
                    #endregion
                    #region CASE: the element is an Object
                    else
                    {
                        //-----------------------------
                        // IF THE ELEMENT IS AN OBJECT:
                        //-----------------------------

                        // Check if the element is the root element:
                        bool isRootElement = IsElementTheRootElement(element, doc);
                        bool isSystemType = SystemTypesHelper.IsSupportedSystemType(namespaceName, localTypeName, assemblyNameIfAny);
                        bool isInitializeTypeFromString = (element.Attribute(InsertingImplicitNodesAndNoDirectTextContent.AttributeNameForTypesToBeInitializedFromString) != null);
                        bool isResourceDictionary = element.Name == DefaultXamlNamespace + "ResourceDictionary";
                        bool isResourceDictionaryReferencedBySourceURI = (isResourceDictionary && element.Attribute("Source") != null);

                        // Add the constructor (in case of object) or a direct initialization (in case of system type or "isInitializeFromString" or referenced ResourceDictionary) (unless this is the root element):
                        string elementUniqueNameOrThisKeyword = GetUniqueName(element);
                        string elementTypeInCSharp = reflectionOnSeparateAppDomain.GetCSharpEquivalentOfXamlTypeAsString(namespaceName, localTypeName, assemblyNameIfAny);
                        stringBuilder.Clear();
                        if (!isRootElement)
                        {
                            // Instantiate the object if it has not been done yet in the 'PopulateDictionaryThatAssociatesNamesToUniqueNames()' method.
                            Dictionary<string, string> uniqueNameToObjectsMap = null;
                            if (!namescopeRootToElementsUniqueNameToInstantiatedObjects.TryGetValue(elementThatIsRootOfTheCurrentNamescope, out uniqueNameToObjectsMap) ||
                                !uniqueNameToObjectsMap.ContainsKey(elementUniqueNameOrThisKeyword))
                            {
                                if (isSystemType)
                                {
                                    //------------------------------------------------
                                    // Add the type initialization from literal value:
                                    //------------------------------------------------
                                    string directContent;
                                    if (element.FirstNode is XText)
                                    {
                                        directContent = ((XText)element.FirstNode).Value;
                                    }
                                    else
                                    {
                                        //If the direct content is not specified, we use the type's default value (ex: <sys:String></sys:String>)
                                        directContent = GetDefaultValueOfTypeAsString(namespaceName, localTypeName, isSystemType, reflectionOnSeparateAppDomain, assemblyNameIfAny);
                                        //throw new wpf::System.Windows.Markup.XamlParseException(string.Format(@"Direct content is expected for the element {0}. Example of direct content: <sys:Double>13</sys:Double>", elementTypeInCSharp));
                                    }
                                    stringBuilder.AppendLine(string.Format("{1} {0} = {2};", elementUniqueNameOrThisKeyword, elementTypeInCSharp, SystemTypesHelper.ConvertSytemTypeFromXamlValueToCSharp(directContent, namespaceName, localTypeName, assemblyNameIfAny)));
                                }
                                else if (isInitializeTypeFromString)
                                {
                                    //------------------------------------------------
                                    // Add the type initialization from string:
                                    //------------------------------------------------
                                    string stringToInitializeTypeFrom = element.Attribute(InsertingImplicitNodesAndNoDirectTextContent.AttributeNameForTypesToBeInitializedFromString).Value; //Note: this exists because we have checked above.
                                    string preparedValue = ConvertingStringToValue.ConvertStringToValue(elementTypeInCSharp, stringToInitializeTypeFrom);
                                    stringBuilder.AppendLine(string.Format("var {0} = {1};", elementUniqueNameOrThisKeyword, preparedValue));

                                }
                                else if (isResourceDictionaryReferencedBySourceURI)
                                {
                                    //------------------------------------------------
                                    // Add the type initialization from "Source" URI:
                                    //------------------------------------------------
                                    string sourceUri = element.Attribute("Source").Value; // Note: this attribute exists because we have checked earlier.
                                    string absoluteSourceUri = PathsHelper.ConvertToAbsolutePathWithComponentSyntax(sourceUri, fileNameWithPathRelativeToProjectRoot, assemblyNameWithoutExtension);

                                    stringBuilder.AppendLine(string.Format("var {0} = new global::{1}();", elementUniqueNameOrThisKeyword, XamlFilesWithoutCodeBehindHelper.GenerateClassNameFromAbsoluteUri(absoluteSourceUri)));
                                }
                                else
                                {
                                    //------------------------------------------------
                                    // Add the type constructor:
                                    //------------------------------------------------
                                    stringBuilder.AppendLine(string.Format("var {0} = new {1}();", elementUniqueNameOrThisKeyword, elementTypeInCSharp));
                                }
                            }

                            //special case: it is a ResourceDictionary in a <XXX.Resources> tag: we want to create the dictionary and immediately set the parent's Resources to the Dictionary.
                            //              this is to let the MergedDictionaries' resources be added to the Application.Resources as they are added, so that they can use each other's resources without needing to add a MergedDictionary in them.
                            //              at this point, we have already added the line to create the ResourceDictionary, so we only need to set the paren't Resources property to this Dictionary.
                            if (isResourceDictionary)
                            {
                                //we check whether it is in the parent's Resources' property:
                                XElement parent = element.Parent;
                                string parentLocalName = parent.Name.LocalName;
                                string[] splittedParentName = parentLocalName.Split('.');
                                if (splittedParentName.Length == 2 && splittedParentName[1] == "Resources")
                                {
                                    //add the element.Resources = this ResourceDictionary:
                                    stringBuilder.AppendLine(string.Format("{0}.Resources = {1};", GetUniqueName(parent.Parent), elementUniqueNameOrThisKeyword));

                                }
                                //todo: add a check in the case "the element is a Property" whether the property's name is "Resources", in which case we do not set it because it shoul dbe done here.
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

                            // Skip the attributes "INTERNAL_UniqueName" and "INTERNAL_InitializeFromString":
                            if (attributeLocalName != GeneratingUniqueNames.AttributeNameForUniqueName
                                && attributeLocalName != InsertingImplicitNodesAndNoDirectTextContent.AttributeNameForTypesToBeInitializedFromString
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
                                        if (isElementInRootNamescope && !reflectionOnSeparateAppDomain.IsAssignableFrom(
                                                namespaceSystemWindows,
                                                "ResourceDictionary",
                                                elementThatIsRootOfTheCurrentNamescope.Name.NamespaceName,
                                                elementThatIsRootOfTheCurrentNamescope.Name.LocalName))
                                        {
                                            string fieldModifier = (isSLMigration ? "internal" : "protected");
                                            XAttribute attr = element.Attribute(xNamespace + "FieldModifier");
                                            if (attr != null)
                                            {
                                                fieldModifier = (attr.Value ?? "").ToLower();
                                            }
                                            resultingFieldsForNamedElements.Add(string.Format("{0} {1} {2};", fieldModifier, elementTypeInCSharp, name));
                                            //resultingFindNameCalls.Add(string.Format("{0} = ({1})this.FindName(\"{2}\");", name, elementTypeInCSharp, name));
                                            resultingFindNameCalls.Add(string.Format("{0} = {1};", name, elementUniqueNameOrThisKeyword));
                                            stringBuilder.AppendLine(string.Format("this.RegisterName(\"{0}\", {1});", name, elementUniqueNameOrThisKeyword));
                                        }
                                        else if (elementThatIsRootOfTheCurrentNamescope.Name == DefaultXamlNamespace + "ControlTemplate")
                                        {
                                            stringBuilder.AppendLine(string.Format("templateOwner_{0}.RegisterName(\"{1}\", {2});", GetUniqueName(elementThatIsRootOfTheCurrentNamescope), name, elementUniqueNameOrThisKeyword));
                                            //resultingFindNameCalls.Add(string.Format("{0} = {1};", name, elementUniqueNameOrThisKeyword));
                                        }

                                        // We also set the Name property on the object itself, if the XAML was "Name=..." or (if the XAML was x:Name=... AND the Name property exists in the object).    (Note: setting the Name property on the object is useful for example in <VisualStateGroup Name="Pressed"/> where the parent control looks at the name of its direct children:
                                        bool isNamePropertyRatherThanXColonNameProperty = string.IsNullOrEmpty(attribute.Name.NamespaceName); // This is used to distinguish between "Name" and "x:Name"
                                        if (isNamePropertyRatherThanXColonNameProperty || reflectionOnSeparateAppDomain.DoesTypeContainNameMemberOfTypeString(namespaceName, localTypeName, assemblyNameIfAny))
                                        {
                                            stringBuilder.AppendLine(string.Format("{0}.Name = \"{1}\";", elementUniqueNameOrThisKeyword, name));
                                        }
                                        //todo: throw an exception when both "x:Name" and "Name" are specified in the XAML.

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
                                            MemberTypes memberType = reflectionOnSeparateAppDomain.GetMemberType(memberName, namespaceName, localTypeName, assemblyNameIfAny);
                                            switch (memberType)
                                            {
                                                case MemberTypes.Event:

                                                    //------------
                                                    // C# EVENT
                                                    //------------

                                                    // Append the statement:
                                                    stringBuilder.AppendLine(string.Format("{0}.{1} += {2};", elementUniqueNameOrThisKeyword, attributeLocalName, attributeValue));

                                                    break;
                                                case MemberTypes.Field:
                                                case MemberTypes.Property:

                                                    //------------
                                                    // C# PROPERTY
                                                    //------------

                                                    // Generate the code for instantiating the attribute value:
                                                    string codeForInstantiatingTheAttributeValue;
                                                    if (elementTypeInCSharp == namespaceSystemWindows + ".Setter")
                                                    {
                                                        //we get the parent Style node (since there is a Style.Setters node that is added, the parent style node is )
                                                        if (element.Parent != null && element.Parent.Parent != null && element.Parent.Parent.Name.LocalName == "Style")
                                                        {

                                                            if (attributeLocalName == "Property")
                                                            {
                                                                // Style setter property:
                                                                codeForInstantiatingTheAttributeValue = GenerateCodeForSetterProperty(element.Parent.Parent, attributeValue, reflectionOnSeparateAppDomain); //todo: support attached properties used in a Setter
                                                            }
                                                            else if (attributeLocalName == "Value")
                                                            {
                                                                var property = element.Attribute("Property");
                                                                if (property != null)
                                                                {
                                                                    bool isSetterForAttachedProperty = property.Value.Contains('.');
                                                                    XName name = GetCSharpXNameFromTargetTypeOrAttachedPropertyString(element, isSetterForAttachedProperty, reflectionOnSeparateAppDomain);
                                                                    //string str = GetCSharpFullTypeNameFromTargetTypeString(styleNode, reflectionOnSeparateAppDomain);
                                                                    //string[] s = {"::"};
                                                                    //string[] splittedStr = str.Split(s, StringSplitOptions.RemoveEmptyEntries);
                                                                    //string[] splittedTypeName = splittedStr[splittedStr.Length - 1].Split('.');
                                                                    //XName typeName = XName.Get(splittedTypeName[splittedTypeName.Length - 1], splittedStr[0]); 
                                                                    string propertyName = isSetterForAttachedProperty ? property.Value.Split('.')[1] : property.Value;
                                                                    codeForInstantiatingTheAttributeValue = GenerateCodeForInstantiatingAttributeValue(name, propertyName, isSetterForAttachedProperty, attributeValue, element, fileNameWithPathRelativeToProjectRoot, assemblyNameWithoutExtension, reflectionOnSeparateAppDomain);
                                                                }
                                                                else
                                                                    throw new wpf::System.Windows.Markup.XamlParseException(@"The <Setter> element must declare a ""Property"" attribute.");
                                                            }
                                                            else
                                                                throw new wpf::System.Windows.Markup.XamlParseException(@"The <Setter> element cannot have attributes other than ""Property"" and ""Value"".");
                                                        }
                                                        else
                                                            throw new wpf::System.Windows.Markup.XamlParseException(@"""<Setter/>"" tags can only be declared inside a <Style/>.");
                                                    }
                                                    else if (elementTypeInCSharp == namespaceSystemWindowsData + ".Binding"
                                                        && memberName == "ElementName")
                                                    {
                                                        // Verify that the user has not already set a "Source" for the binding, otherwise his source prevails over the "ElementName" property (ie. if the suer sets both Source and ElementName, we should only use Source):
                                                        if (element.Element("Binding.Source") == null && element.Attribute("Source") == null) //todo: test this...
                                                        {
                                                            // Replace "ElementName" with a direct reference to the instance:
                                                            // Note: We need to put the code at the end of the method because "FindName" only works after all the names in the current namescope have been registered.
                                                            List<string> markupExtensionsAdditionalCode = GetListThatContainsAdditionalCodeFromDictionary(elementThatIsRootOfTheCurrentNamescope, namescopeRootToMarkupExtensionsAdditionalCode);
                                                            string uniqueNameOfSource = GetUniqueNameFromElementName(attributeValue, elementThatIsRootOfTheCurrentNamescope, namescopeRootToNameToUniqueNameDictionary);
                                                            if (uniqueNameOfSource != null)
                                                            {
                                                                markupExtensionsAdditionalCode.Add(string.Format("{0}.Source = {1};", elementUniqueNameOrThisKeyword, uniqueNameOfSource));
                                                            }
                                                            else
                                                            {
                                                                //TODO: check wether WPF & UWP also allow that silently
                                                                // throw new wpf::System.Windows.Markup.XamlParseException("The \"ElementName\" specified in the Binding was not found: " + attributeValue);
                                                                logger.WriteWarning($"The \"ElementName\" specified in the Binding was not found: {attributeValue}", sourceFile, GetLineNumber(element));
                                                            }
                                                        }
                                                        codeForInstantiatingTheAttributeValue = null; // null means that we skip this attribute here.
                                                    }
                                                    else
                                                    {
                                                        //------------
                                                        // NORMAL C# PROPERTY
                                                        //------------

                                                        XName typeName = element.Name;
                                                        string propertyName = attribute.Name.LocalName;
                                                        codeForInstantiatingTheAttributeValue = GenerateCodeForInstantiatingAttributeValue(typeName, propertyName, isAttachedProperty, attributeValue, element, fileNameWithPathRelativeToProjectRoot, assemblyNameWithoutExtension, reflectionOnSeparateAppDomain);
                                                    }

                                                    // Append the statement:
                                                    if (codeForInstantiatingTheAttributeValue != null)
                                                        stringBuilder.AppendLine(string.Format("{0}.{1} = {2};", elementUniqueNameOrThisKeyword, attributeLocalName, codeForInstantiatingTheAttributeValue));

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
                                    string classFullNameForAttachedProperty = reflectionOnSeparateAppDomain.GetCSharpEquivalentOfXamlTypeAsString(attachedPropertyTypeNamespaceName, attachedPropertyTypeLocalName, attachedPropertyTypeAssemblyNameIfAny);
                                    string propertyName = splitted[1];

                                    if (classLocalNameForAttachedProperty != "Storyboard" || propertyName == "TargetName")
                                    {
                                        // Generate the code for instantiating the attribute value:
                                        string codeForInstantiatingTheAttributeValue = GenerateCodeForInstantiatingAttributeValue(elementNameForAttachedProperty, propertyName, isAttachedProperty, attributeValue, element, fileNameWithPathRelativeToProjectRoot, assemblyNameWithoutExtension, reflectionOnSeparateAppDomain);

                                        // Append the statement:
                                        stringBuilder.AppendLine(string.Format("{0}.Set{1}({2},{3});", classFullNameForAttachedProperty, propertyName, elementUniqueNameOrThisKeyword, codeForInstantiatingTheAttributeValue));
                                    }
                                    else
                                    {
                                        if (classLocalNameForAttachedProperty == "Storyboard" && propertyName == "TargetProperty")
                                        {
                                            // Look for a "TargetName" at the animation level (eg. <DoubleAnimation Storyboard.TargetName="border1"/>)
                                            var targetNameAttributeAtTheAnimationLevel = element.Attribute("Storyboard.TargetName");
                                            string targetElementUniqueName = null;
                                            if (targetNameAttributeAtTheAnimationLevel != null)
                                            {
                                                if (namescopeRootToNameToUniqueNameDictionary[elementThatIsRootOfTheCurrentNamescope].TryGetValue(targetNameAttributeAtTheAnimationLevel.Value, out targetElementUniqueName))
                                                {
#if LOG_TARGET_ELEMENTS_NOT_FOUND
                                                        logger.WriteWarning(string.Format("Could not find an element with name \"{0}\".", targetNameAttributeAtTheAnimationLevel.Value));
#endif
                                                }
                                            }
                                            else
                                            {
                                                // If no "TargetName" was found at the animation level, look at the Storyboard level (eg. <Storyboard Storyboard.TargetName="border1"/>)
                                                var targetNameAttributeAtTheStoryboardLevel = element.Parent.Parent.Attribute("Storyboard.TargetName"); // Note: here there is ".Parent.Parent" because the first parent is "<Storyboard.Children>", while the second parent is "<Storyboard>".
                                                if (targetNameAttributeAtTheStoryboardLevel != null)
                                                {
                                                    if (!namescopeRootToNameToUniqueNameDictionary[elementThatIsRootOfTheCurrentNamescope].TryGetValue(targetNameAttributeAtTheStoryboardLevel.Value, out targetElementUniqueName))
                                                    {
#if LOG_TARGET_ELEMENTS_NOT_FOUND
                                                            logger.WriteWarning(string.Format("Could not find an element with name \"{0}\".", targetNameAttributeAtTheStoryboardLevel.Value));
#endif
                                                    }
                                                }
                                            }
                                            // Note: if none is found, it is not a problem, because the user can specify a target via the method Storyboard.Begin(target).

                                            string accessorsUniqueNamePart = GeneratingUniqueNames.GenerateUniqueNameFromString("VisualStateProperty");
                                            //todo:
                                            // 1) generate the code to access the property from the target root (defined by the "TargetName" or "Target" property) and put it in a Function<DependencyObject, DependencyObject>
                                            // 2) generate the codes to set and get the property accessed in the previous code and put it in an Action<DependencyObject, object> and a Function<DependencyObject, object> respectively.
                                            GenerateCodeForStoryboardAccessToPropertyFromTargetRoot(namespaceName, assemblyNameIfAny, attributeValue, accessorsUniqueNamePart, resultingMethods, element, reflectionOnSeparateAppDomain, namespaceSystemWindows);

                                            string[] splittedPropertyPath = attributeValue.Split('.');
                                            string dependencyPropertyName = splittedPropertyPath[splittedPropertyPath.Length - 1];
                                            if (dependencyPropertyName.EndsWith(")"))
                                            {
                                                dependencyPropertyName = dependencyPropertyName.Substring(0, dependencyPropertyName.Length - 1);
                                            }
                                            if (dependencyPropertyName.EndsWith("]"))
                                            {
                                                throw new NotSupportedException("PropertyPaths that end with brackets are not supported yet.");
                                            }
                                            string dependencyPropertyPath = GeneratePropertyPathFromAttributeValue(attributeValue, element, reflectionOnSeparateAppDomain);
                                            //todo: if needed, uncomment the folling line then change the way it is accessed in the TimeLine.Apply overrides (ex: ObjectAnimationUsingKeyFrames).
                                            //dependencyPropertyName += "Property";



                                            // 3) Add this (roughly):
                                            //      Storyboard.SetTargetProperty(colorAnimation7637478638468367843,
                                            //          new PropertyPath(
                                            //              accessVisualStateProperty65675669834683448390,
                                            //              setVisualStateProperty65675669834683448390,
                                            //              getVisualStateProperty65675669834683448390));
                                            //      Storyboard.SetTarget(colorAnimation7637478638468367843, canvas67567345673874893);

                                            string findAName = string.Format(@"
{1}.Storyboard.SetTargetProperty({2},
    new {0}.PropertyPath(
        ""{6}"",
        ""{5}"",
        access{3},
        set{3},
        setAnimation{3},                                     
        setLocal{3},
        get{3}));
{1}.Storyboard.SetTarget({2}, {4});
", namespaceSystemWindows, namespaceSystemWindowsMediaAnimation, element.Attribute("INTERNAL_UniqueName").Value, accessorsUniqueNamePart, targetElementUniqueName != null ? targetElementUniqueName : "null", dependencyPropertyName, dependencyPropertyPath);

                                            // 4) put all the code generated above in the Dictionary of the code to add to the end of the storyboard :
                                            List<string> storyboardsAdditionalCode = GetListThatContainsAdditionalCodeFromDictionary(elementThatIsRootOfTheCurrentNamescope, namescopeRootToStoryboardsAdditionalCode);
                                            //storyboardsAdditionalCode.Add(codeForStoryboardAccessToProperty);
                                            storyboardsAdditionalCode.Add(findAName);
                                            //}
                                            //else
                                            //{
                                            //    throw new wpf::System.Windows.Markup.XamlParseException(@"An element with a ""Storyboard.TargetProperty"" attribute must declare a ""Storyboard.TargetName"" attribute.");
                                            //}
                                        }
                                    }
                                }
                            }
                        }

                        // Determine if the element is a collection or a dictionary:
                        EnumerableType enumerableType = EnumerableType.None;
                        if (GettingInformationAboutXamlTypes.IsElementADictionary(element, reflectionOnSeparateAppDomain))
                            enumerableType = EnumerableType.Dictionary;
                        else if (GettingInformationAboutXamlTypes.IsElementACollection(element, reflectionOnSeparateAppDomain))
                            enumerableType = EnumerableType.Collection;

                        if (enumerableType == EnumerableType.Collection || enumerableType == EnumerableType.Dictionary)
                        {
                            // Add the children to the collection/dictionary:
                            GenerateCodeForAddingChildrenToCollectionOrDictionary(
                                codeStack: codeStack,
                                stringBuilder: stringBuilder,
                                enumerableType: enumerableType,
                                codeToAccessTheEnumerable: elementUniqueNameOrThisKeyword,
                                elementThatContainsTheChildrenToAdd: element,
                                reflectionOnSeparateAppDomain: reflectionOnSeparateAppDomain);
                        }
                        else
                        {
                            // Add the code of the children elements:
                            int childrenCount = element.Elements().Count(); //todo-performance: find a more performant way to count the children?
                            foreach (var childCode in PopElementsFromStackAndReadThemInReverseOrder<string>(codeStack, childrenCount)) // Note: this is supposed to not raise OutOfIndex because child nodes are supposed to have added code to the stack.
                            {
                                stringBuilder.AppendLine(childCode);
                            }
                        }

                        // Put the whole code into the stack:
                        var resultingForTheElementAndAllItsChildren = stringBuilder.ToString();
                        codeStack.Push(resultingForTheElementAndAllItsChildren);
                    }
                    #endregion
                }
                catch (wpf::System.Windows.Markup.XamlParseException xamlParseException)
                {
                    // We create and throw a copy of the "XamlParseException" in order to add the line number information:
                    int lineNumber = GetLineNumber(element);
                    var newXamlParseException =
                        new wpf::System.Windows.Markup.XamlParseException(
                            xamlParseException.Message,
                            lineNumber,
                            -1,
                            xamlParseException);
                    throw newXamlParseException;
                }
            }

            // Process the code that remains in the stack:
            if (codeStack.Count != 1)
                throw new wpf::System.Windows.Markup.XamlParseException("At the end of the parsing, the code stack should contain exactly one item.");
            string codeToWorkWithTheRootElement = codeStack.Pop();

            // Get general information about the class:
            string className, namespaceStringIfAny, baseType;
            bool hasCodeBehind;
            GetClassInformationFromXaml(doc, fileNameWithPathRelativeToProjectRoot, assemblyNameWithoutExtension, reflectionOnSeparateAppDomain, out className, out namespaceStringIfAny, out baseType, out hasCodeBehind);

            // Create the "IntializeComponent()" method:
            string objectsToInstantiateEarly = string.Join("\r\n", GetNameToUniqueNameDictionary(doc.Root, namescopeRootToElementsUniqueNameToInstantiatedObjects).Select(x => x.Value));
            string markupExtensionsAdditionalCodeForElementsInRootNamescope = string.Join("\r\n", GetListThatContainsAdditionalCodeFromDictionary(doc.Root, namescopeRootToMarkupExtensionsAdditionalCode));
            string storyboardsAdditionalCodeForElementsInRootNamescope = string.Join("\r\n", GetListThatContainsAdditionalCodeFromDictionary(doc.Root, namescopeRootToStoryboardsAdditionalCode));
            bool isClassTheApplicationClass = IsClassTheApplicationClass(baseType);
            string additionalCodeToPlaceAtTheBeginningOfInitializeComponent = (isClassTheApplicationClass ? codeToPutInTheInitializeComponentOfTheApplicationClass : "") + objectsToInstantiateEarly;
            string additionalCodeToPlaceAtTheEndOfInitializeComponent = markupExtensionsAdditionalCodeForElementsInRootNamescope + Environment.NewLine + storyboardsAdditionalCodeForElementsInRootNamescope;

            string initializeComponentMethod = CreateInitializeComponentMethod(codeToWorkWithTheRootElement, resultingFindNameCalls, additionalCodeToPlaceAtTheBeginningOfInitializeComponent, additionalCodeToPlaceAtTheEndOfInitializeComponent, isSLMigration, assemblyNameWithoutExtension, fileNameWithPathRelativeToProjectRoot);
            resultingMethods.Insert(0, initializeComponentMethod);

            // Add a contructor if there is no code behind:
            if (!hasCodeBehind)
            {
                string uiElementFullyQualifiedTypeName = isSLMigration ? "global::System.Windows.UIElement" : "global::Windows.UI.Xaml.UIElement";
                resultingMethods.Add(string.Format(@"
        public {0}()
        {{
            this.InitializeComponent();
#pragma warning disable 0184 // Prevents warning CS0184 ('The given expression is never of the provided ('type') type')
            if (this is {1})
            {{
                (({1})(object)this).XamlSourcePath = @""{2}\{3}"";
            }}
#pragma warning restore 0184
        }}
", className, uiElementFullyQualifiedTypeName, assemblyNameWithoutExtension, fileNameWithPathRelativeToProjectRoot));
            }

            // Wrap everything into a partial class:
            string finalCode = GeneratePartialClass(resultingMethods,
                                                    resultingFieldsForNamedElements,
                                                    className,
                                                    namespaceStringIfAny,
                                                    baseType,
                                                    fileNameWithPathRelativeToProjectRoot,
                                                    assemblyNameWithoutExtension,
                                                    listOfAllTheTypesUsedInThisXamlFile,
                                                    hasCodeBehind,
#if BRIDGE
                                                    addApplicationEntryPoint: isClassTheApplicationClass
#else
 addApplicationEntryPoint: false
#endif
);

            return finalCode;
        }

        /// <summary>
        /// Generate the property path from the Storyboard.TargetProperty property.
        /// </summary>
        /// <param name="attributeValue"></param>
        /// <param name="element"></param>
        /// <param name="reflectionOnSeparateAppDomain"></param>
        /// <returns></returns>
        private static string GeneratePropertyPathFromAttributeValue(string attributeValue, XElement element, ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain)
        {
            string propertyPath = "";
            List<string> propertyPathSplittedInBlocks = SplitStoryboardTargetPropertyInBlocks(attributeValue, keepParenthesis: true);
            bool isPropertyPathEmpty = true;
            foreach (string block in propertyPathSplittedInBlocks)
            {
                if (!isPropertyPathEmpty)
                {
                    if (!block.StartsWith("["))
                    {
                        propertyPath += ".";
                    }
                }
                else
                {
                    isPropertyPathEmpty = false;
                }

                if (block.StartsWith("["))
                {
                    propertyPath += block;
                }
                else
                {
                    string propertyName;
                    if (block.Contains("."))
                    {
                        string namespaceName, localName, assemblyIfAny;
                        GettingInformationAboutXamlTypes.GetClrNamespaceAndLocalName(block.TrimStart('(').TrimEnd(')'), element, out namespaceName, out localName, out assemblyIfAny);
                        string[] splittedLocalName = localName.Split('.');
                        string chsarpTypeName = reflectionOnSeparateAppDomain.GetCSharpEquivalentOfXamlTypeAsString(namespaceName, splittedLocalName[0], assemblyIfAny);
                        propertyName = chsarpTypeName;
                        if (splittedLocalName.Length == 2)
                        {
                            propertyName += "." + splittedLocalName[1];
                        }
                        propertyName = "(" + propertyName + ")";
                    }
                    else
                    {
                        propertyName = block;
                    }
                    propertyPath += propertyName;
                }
            }
            return propertyPath;
        }

        private static List<string> SplitStoryboardTargetPropertyInBlocks(string attributeValue, bool keepParenthesis)
        {
            string[] attributeValueSplittedAtParenthesis = attributeValue.Trim().Split('(');
            List<string> propertyPathSplittedInBlocks = new List<string>();
            foreach (string step in attributeValueSplittedAtParenthesis)
            {
                string nonHandledPartOfStep = step;
                int index = step.IndexOf(')'); //example in "Canvas.Background)." --> index = 17
                if (index != -1)
                {
                    //no need to check if the string is empty since we know there is a parenthesis
                    if (keepParenthesis)
                    {
                        propertyPathSplittedInBlocks.Add("(" + step.Substring(0, index + 1)); //example: "Canvas.Background)." ==> "(Canvas.Background)"
                    }
                    else
                    {
                        propertyPathSplittedInBlocks.Add(step.Substring(0, index)); //example: "Canvas.Background)." ==> "Canvas.Background"
                    }
                    nonHandledPartOfStep = step.Substring(index + 1);
                }
                //now we know there are only elements that are outside of parenthesis, so the only remaining block separators are '.' and the brackets:
                string[] splittedRest = nonHandledPartOfStep.Split('.');
                foreach (string subStep in splittedRest)
                {
                    index = subStep.IndexOf('[');
                    if (index != -1)
                    {
                        string subStepFirstPart = subStep.Substring(0, index);
                        if (!string.IsNullOrWhiteSpace(subStepFirstPart))
                        {
                            propertyPathSplittedInBlocks.Add(subStepFirstPart); //example: "Canvas.Background)." ==> "Canvas.Background"
                        }
                        propertyPathSplittedInBlocks.Add(subStep.Substring(index)); //here we know there is [X] so no need to check if it is an empty string.
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(subStep))
                        {
                            propertyPathSplittedInBlocks.Add(subStep);
                        }
                    }
                }
            }
            return propertyPathSplittedInBlocks;
        }

        /// <summary>
        /// Generates the code to access the property that will be set from the Storyboard.TargetProperty property.
        /// </summary>
        /// <param name="namespaceName"></param>
        /// <param name="assemblyNameIfAny"></param>
        /// <param name="propertyPath"></param>
        /// <param name="targetPropertyPathRootElement">The uniqe name of the element whose name has been set in TargetName</param>
        /// <param name="reflectionOnSeparateAppDomain"></param>
        /// <returns></returns>
        private static void GenerateCodeForStoryboardAccessToPropertyFromTargetRoot(string namespaceName, string assemblyNameIfAny, string propertyPath, string accessorsUniqueNamePart, List<string> resultingMethods, XElement element, ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain, string namespaceSystemWindows)
        {
            if (propertyPath == null)
            {
                throw new Exception();//todo: make this more explicit (maybe using MissingMemberException)
            }
            string generatedCode = string.Empty;
            string rootTargetObjectInstance = "rootTargetObjectInstance";

            //we go through the propertyPath string to read the path to the property:
            propertyPath = propertyPath.Trim();

            List<string> propertyPathSplittedInBlocks = SplitStoryboardTargetPropertyInBlocks(propertyPath, keepParenthesis: false);

            string previousVarName = rootTargetObjectInstance;
            //now we have generated the list of blocks for which we will now need to generate the code.
            for (int blockIndex = 0; blockIndex < propertyPathSplittedInBlocks.Count - 1; ++blockIndex)
            {
                string currentBlock = propertyPathSplittedInBlocks.ElementAt(blockIndex);
                if (!string.IsNullOrWhiteSpace(currentBlock))
                {
                    string currentVarName = "x" + blockIndex;
                    string dependencyPropertyVarName = "prop" + blockIndex;
                    string indexName = "index" + blockIndex;
                    //todo: in GenerateStoryboardAccessToPropertyBlockCode, make a thing to return a type if we have a thing like (Type.Property) somewhere, to allow the reflection to be made at compilation instead of at runtime starting from this element.
                    generatedCode += Environment.NewLine;
                    generatedCode += namespaceSystemWindows + ".DependencyProperty " + dependencyPropertyVarName + " = null;";
                    generatedCode += Environment.NewLine;
                    generatedCode += GenerateStoryboardAccessToPropertyBlockCode(namespaceName, assemblyNameIfAny, currentBlock, previousVarName, currentVarName, blockIndex, dependencyPropertyVarName, indexName, reflectionOnSeparateAppDomain, namespaceSystemWindows);
                    generatedCode += Environment.NewLine;
                    generatedCode += "yield return new global::System.Tuple<" + namespaceSystemWindows + ".DependencyObject, " + namespaceSystemWindows + ".DependencyProperty, int?> ((" + namespaceSystemWindows + ".DependencyObject)" + currentVarName + ", " + dependencyPropertyVarName + ", " + indexName + ");";
                    previousVarName = currentVarName;
                }
            }
            generatedCode += Environment.NewLine;
            generatedCode += "yield break;";
            //generatedCode += string.Format("return {0};", previousVarName);

            string lastPropertyOwnerType = string.Empty;
            string lastPropertyInPath = propertyPathSplittedInBlocks[propertyPathSplittedInBlocks.Count - 1];
            string[] splittedLastPropertyInPath = lastPropertyInPath.Split('.');
            string lastPropertyFullPath = null;
            string lastPropertyPreparation = string.Empty;
            if (splittedLastPropertyInPath.Length > 1)
            {
                string propertyTypeNamespaceName;
                string propertyTypeName;
                string propertyTypeAssemblyIfAny;
                GettingInformationAboutXamlTypes.GetClrNamespaceAndLocalName(splittedLastPropertyInPath[0], element, out propertyTypeNamespaceName, out propertyTypeName, out propertyTypeAssemblyIfAny);
                lastPropertyOwnerType = reflectionOnSeparateAppDomain.GetCSharpEquivalentOfXamlTypeAsString(propertyTypeNamespaceName, propertyTypeName, assemblyNameIfAny);
                lastPropertyFullPath = lastPropertyOwnerType + "." + splittedLastPropertyInPath[1] + "Property"; //todo: we currently consider that the DependencyProperty for a Property is named with the same name + Property. Change this line once we won't be doing this anymore.
            }
            else
            {
                //note: below, we consider that the dependencyProperty name is the one o the property + Property. If we change this state of mind, apply changes here. 
                lastPropertyPreparation = string.Format(@"
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty(""{0}"").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField(""{0}Property"");
{1}.DependencyProperty property = ({1}.DependencyProperty)propertyField.GetValue(null);
", lastPropertyInPath, namespaceSystemWindows);
                lastPropertyFullPath = "property"; //note: it is the same as the one defined in lastPropertyPreparation
            }
            resultingMethods.Add(string.Format(@"
public global::System.Collections.Generic.IEnumerable<global::System.Tuple<{3}.DependencyObject, {3}.DependencyProperty, int?>> access{0} ({3}.DependencyObject {1})
{{
  {2}
}}", accessorsUniqueNamePart, rootTargetObjectInstance, generatedCode, namespaceSystemWindows));
            resultingMethods.Add(string.Format(@"
public void set{0} ({3}.DependencyObject finalTargetInstance, object value)
{{
  {1}
  (finalTargetInstance).SetVisualStateValue({2}, value);
}}", accessorsUniqueNamePart, lastPropertyPreparation, lastPropertyFullPath, namespaceSystemWindows));
            resultingMethods.Add(string.Format(@"
public void setAnimation{0} ({3}.DependencyObject finalTargetInstance, object value)
{{
  {1}
  (finalTargetInstance).SetAnimationValue({2}, value);
}}", accessorsUniqueNamePart, lastPropertyPreparation, lastPropertyFullPath, namespaceSystemWindows));
            resultingMethods.Add(string.Format(@"
public void setLocal{0} ({3}.DependencyObject finalTargetInstance, object value)
{{
  {1}
  (finalTargetInstance).SetCurrentValue({2}, value);
}}", accessorsUniqueNamePart, lastPropertyPreparation, lastPropertyFullPath, namespaceSystemWindows));
            resultingMethods.Add(string.Format(@"
public global::System.Object get{0} ({3}.DependencyObject finalTargetInstance)
{{
  {1}
  return finalTargetInstance.GetVisualStateValue({2});
}}", accessorsUniqueNamePart, lastPropertyPreparation, lastPropertyFullPath, namespaceSystemWindows));
        }

        static string GenerateStoryboardAccessToPropertyBlockCode(string namespaceName, string assemblyNameIfAny, string block, string blockRootElement, string currentVarName, int blockIndex, string dependencyPropertyVarName, string indexName, ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain, string namespaceSystemWindows)
        {
            string generatedCode = null;
            string[] splittedBlock = block.Split('.');
            string lastPartOfBlock = splittedBlock[splittedBlock.Length - 1];
            if (splittedBlock.Length > 1)
            {
                //the block is in the format: "Type.Property"
                string typeAsString = splittedBlock[0];
                typeAsString = reflectionOnSeparateAppDomain.GetCSharpEquivalentOfXamlTypeAsString(namespaceName, typeAsString, assemblyNameIfAny); //todo: check if these parameters are good
                generatedCode = string.Format("var {0} = (({1}){2}).{3};", currentVarName, typeAsString, blockRootElement, lastPartOfBlock);
                generatedCode += string.Format(@"
global::System.Type {0} = (({2}){3}).GetType().GetProperty(""{4}"").DeclaringType;
    {1} = ({5}.DependencyProperty){0}.GetField(""{4}Property"").GetValue(null);", "propertyDeclaringType" + blockIndex, dependencyPropertyVarName, typeAsString, blockRootElement, lastPartOfBlock, namespaceSystemWindows);
                generatedCode += Environment.NewLine;
                generatedCode += "int? " + indexName + " = null" + ";";
            }

            if (string.IsNullOrWhiteSpace(generatedCode))
            {
                if (!lastPartOfBlock.StartsWith("["))
                {
                    //We need to use reflection at runtime:
                    generatedCode = string.Format(@"
global::System.Type {0} = {1}.GetType();
System.Reflection.PropertyInfo {2} = {0}.GetProperty(""{3}"");
var {4} = {2}.GetValue({1});
{5} = ({6}.DependencyProperty){0}.GetField(""{3}Property"", System.Reflection.BindingFlags.FlattenHierarchy | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public).GetValue(null);", GeneratingUniqueNames.GenerateUniqueNameFromString("parentElementType"), blockRootElement, GeneratingUniqueNames.GenerateUniqueNameFromString("propertyInfo"), lastPartOfBlock, currentVarName, dependencyPropertyVarName, namespaceSystemWindows);
                    generatedCode += Environment.NewLine;
                    generatedCode += "int? " + indexName + " = null" + ";";
                }
                else
                {
                    //todo: this
                    string index = lastPartOfBlock.TrimEnd(']').TrimStart('[');
                    generatedCode = string.Format("var {0} = ((dynamic){1})[{2}];", currentVarName, blockRootElement, index);
                    generatedCode += Environment.NewLine;
                    generatedCode += "int? " + indexName + " = " + index + ";";
                }
            }
            return generatedCode;
        }

        private static XElement GetRootOfCurrentNamescopeForCompilation(XElement element)
        {
            while (element.Parent != null)
            {
                if (element.Name.Namespace == DefaultXamlNamespace && DoesClassInheritFromFrameworkTemplate(element.Name.LocalName))
                {
                    return element;
                }
                element = element.Parent;
            }
            return element;
        }

        private static XElement GetRootOfCurrentNamescopeForRuntime(XElement element, ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain)
        {
            XElement currentElement = element;
            bool skipTemplateNode = true;
            while (currentElement.Parent != null)
            {
                if (!currentElement.Name.LocalName.Contains("."))
                {
                    if (reflectionOnSeparateAppDomain.IsAssignableFrom(DefaultXamlNamespace.NamespaceName, "FrameworkTemplate",
                        currentElement.Name.NamespaceName, currentElement.Name.LocalName))
                    {
                        if (!skipTemplateNode)
                        {
                            return currentElement;
                        }
                    }
                    skipTemplateNode = false;
                }

                currentElement = currentElement.Parent;
            }
            return currentElement;
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

        private static string GenerateCodeToInstantiateXElement(XElement element, ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain)
        {
            string namespaceName, localTypeName, assemblyNameIfAny;
            GettingInformationAboutXamlTypes.GetClrNamespaceAndLocalName(element.Name, out namespaceName, out localTypeName, out assemblyNameIfAny);
            bool isSystemType = SystemTypesHelper.IsSupportedSystemType(namespaceName, localTypeName, assemblyNameIfAny);
            bool isInitializeTypeFromString = (element.Attribute(InsertingImplicitNodesAndNoDirectTextContent.AttributeNameForTypesToBeInitializedFromString) != null);

            // Add the constructor (in case of object) or a direct initialization (in case of system type or "isInitializeFromString" or referenced ResourceDictionary) (unless this is the root element):
            string elementUniqueNameOrThisKeyword = GetUniqueName(element);
            string elementTypeInCSharp;
            elementTypeInCSharp = reflectionOnSeparateAppDomain.GetCSharpEquivalentOfXamlTypeAsString(namespaceName, localTypeName, assemblyNameIfAny, false);
            if (isSystemType)
            {
                //------------------------------------------------
                // Add the type initialization from literal value:
                //------------------------------------------------
                string directContent;
                if (element.FirstNode is XText)
                {
                    directContent = ((XText)element.FirstNode).Value;
                }
                else
                {
                    //If the direct content is not specified, we use the type's default value (ex: <sys:String></sys:String>)
                    directContent = GetDefaultValueOfTypeAsString(namespaceName, localTypeName, isSystemType, reflectionOnSeparateAppDomain, assemblyNameIfAny);
                    //throw new wpf::System.Windows.Markup.XamlParseException(string.Format(@"Direct content is expected for the element {0}. Example of direct content: <sys:Double>13</sys:Double>", elementTypeInCSharp));
                }
                return string.Format("{1} {0} = {2};", elementUniqueNameOrThisKeyword, elementTypeInCSharp, SystemTypesHelper.ConvertSytemTypeFromXamlValueToCSharp(directContent, namespaceName, localTypeName, assemblyNameIfAny));
            }
            else if (isInitializeTypeFromString)
            {
                //------------------------------------------------
                // Add the type initialization from string:
                //------------------------------------------------
                string stringToInitializeTypeFrom = element.Attribute(InsertingImplicitNodesAndNoDirectTextContent.AttributeNameForTypesToBeInitializedFromString).Value; //Note: this exists because we have checked above.
                string preparedValue = ConvertingStringToValue.ConvertStringToValue(elementTypeInCSharp, stringToInitializeTypeFrom);
                return string.Format("var {0} = {1};", elementUniqueNameOrThisKeyword, preparedValue);

            }
            else
            {
                //------------------------------------------------
                // Add the type constructor:
                //------------------------------------------------
                return string.Format("var {0} = new {1}();", elementUniqueNameOrThisKeyword, elementTypeInCSharp);
            }
        }

        private static void PopulateDictionaryThatAssociatesNamesToUniqueNames(XDocument doc,
            Dictionary<XElement, Dictionary<string, string>> namescopeRootToNameToUniqueNameDictionary,
            Dictionary<XElement, Dictionary<string, string>> namescopeRootToElementsUniqueNameToInstantiatedObjects,
            ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain)
        {
            foreach (var element in PostOrderTreeTraversal.TraverseTreeInPostOrder(doc.Root)) // Note: any order is fine here.
            {
                // Get information about which element holds the namescope of the current element. For example, if the current element is inside a DataTemplate, the DataTemplate is the root of the namescope of the current element. If the element is not inside a DataTemplate or ControlTemplate, the root of the XAML is the root of the namescope of the current element.
                XElement elementThatIsRootOfTheCurrentNamescope = GetRootOfCurrentNamescopeForRuntime(element, reflectionOnSeparateAppDomain);

                foreach (XAttribute attribute in element.Attributes())
                {
                    if (IsAttributeTheXNameAttribute(attribute))
                    {
                        string elementUniqueNameOrThisKeyword = GetUniqueName(element);
                        string name = attribute.Value;

                        // Remember the "Name to UniqueName" association:
                        Dictionary<string, string> nameToUniqueNameDictionary = GetNameToUniqueNameDictionary(elementThatIsRootOfTheCurrentNamescope, namescopeRootToNameToUniqueNameDictionary);
                        if (nameToUniqueNameDictionary.ContainsKey(name))
                            throw new wpf::System.Windows.Markup.XamlParseException("The name already exists in the tree: " + name);
                        nameToUniqueNameDictionary.Add(name, elementUniqueNameOrThisKeyword);
                        if (element != elementThatIsRootOfTheCurrentNamescope)
                        {
                            Dictionary<string, string> uniqueNameToInstantiatedObjectDictionary = GetNameToUniqueNameDictionary(elementThatIsRootOfTheCurrentNamescope, namescopeRootToElementsUniqueNameToInstantiatedObjects);
                            uniqueNameToInstantiatedObjectDictionary.Add(elementUniqueNameOrThisKeyword, GenerateCodeToInstantiateXElement(element, reflectionOnSeparateAppDomain));
                        }
                    }
                }
            }
        }

        private static bool ShouldGenerateFieldForXNameAttribute(XElement element)
        {
            if (element.Document.Root.Name.NamespaceName == DefaultXamlNamespace
                && element.Document.Root.Name.LocalName == "ResourceDictionary")
            {
                return false;
            }
            return true;
        }

        static bool IsAttributeTheXNameAttribute(XAttribute attribute)
        {
            bool isXName = (attribute.Name.LocalName == "Name" && attribute.Name.NamespaceName == xNamespace);
            bool isName = (attribute.Name.LocalName == "Name" && string.IsNullOrEmpty(attribute.Name.NamespaceName));
            return isXName || isName;
        }

        static bool IsElementTheRootElement(XElement element, XDocument doc)
        {
            return (element == doc.Root);
        }

        static bool IsClassTheApplicationClass(string className)
        {
            return (className == "global::Windows.UI.Xaml.Application" || className == "global::System.Windows.Application");
        }

        static bool DoesClassInheritFromFrameworkTemplate(string classLocalName) //todo: add support for namespace for more precision
        {
            return classLocalName == "DataTemplate" || classLocalName == "ItemsPanelTemplate" || classLocalName == "ControlTemplate";
        }

        static List<string> GetListThatContainsAdditionalCodeFromDictionary(XElement elementThatIsRootOfTheCurrentNamescope, Dictionary<XElement, List<string>> namescopeRootToListOfAdditionalCode)
        {
            if (namescopeRootToListOfAdditionalCode.ContainsKey(elementThatIsRootOfTheCurrentNamescope))
            {
                List<string> listThatContainsAdditionalCode = namescopeRootToListOfAdditionalCode[elementThatIsRootOfTheCurrentNamescope];
                return listThatContainsAdditionalCode;
            }
            else
            {
                List<string> listThatContainsAdditionalCode = new List<string>();
                namescopeRootToListOfAdditionalCode.Add(elementThatIsRootOfTheCurrentNamescope, listThatContainsAdditionalCode);
                return listThatContainsAdditionalCode;
            }
        }

        private static string GetUniqueNameFromElementName(string elementName, XElement rootOfTheCurrentNamescope, Dictionary<XElement, Dictionary<string, string>> namescopeRootToNameToUniqueNameDictionary)
        {
            if (rootOfTheCurrentNamescope == null)
            {
                return null;
            }
            if (namescopeRootToNameToUniqueNameDictionary.ContainsKey(rootOfTheCurrentNamescope))
            {
                if (namescopeRootToNameToUniqueNameDictionary[rootOfTheCurrentNamescope].ContainsKey(elementName))
                {
                    return namescopeRootToNameToUniqueNameDictionary[rootOfTheCurrentNamescope][elementName];
                }
            }
            return GetUniqueNameFromElementName(elementName, rootOfTheCurrentNamescope.Parent, namescopeRootToNameToUniqueNameDictionary);
        }

        static Dictionary<string, string> GetNameToUniqueNameDictionary(XElement elementThatIsRootOfTheCurrentNamescope, Dictionary<XElement, Dictionary<string, string>> namescopeRootToNameToUniqueNameDictionary)
        {
            if (namescopeRootToNameToUniqueNameDictionary.ContainsKey(elementThatIsRootOfTheCurrentNamescope))
            {
                Dictionary<string, string> nameToUniqueNameDictionary = namescopeRootToNameToUniqueNameDictionary[elementThatIsRootOfTheCurrentNamescope];
                return nameToUniqueNameDictionary;
            }
            else
            {
                Dictionary<string, string> nameToUniqueNameDictionary = new Dictionary<string, string>();
                namescopeRootToNameToUniqueNameDictionary.Add(elementThatIsRootOfTheCurrentNamescope, nameToUniqueNameDictionary);
                return nameToUniqueNameDictionary;
            }
        }

        static IEnumerable<T> PopElementsFromStackAndReadThemInReverseOrder<T>(Stack<T> stack, int count)
        {
            // Note: this method is used for example to change the order of the child codes so that they are added in the same order as the in the XAML.

            Stack<T> stackToInvertOrder = new Stack<T>();
            for (int i = 0; i < count; i++)
            {
                var element = stack.Pop();
                stackToInvertOrder.Push(element);
            }
            while (stackToInvertOrder.Count > 0)
            {
                var element = stackToInvertOrder.Pop();
                yield return element;
            }
        }

        enum EnumerableType
        {
            None, Collection, Dictionary
        }

        static void GenerateCodeForAddingChildrenToCollectionOrDictionary(Stack<string> codeStack, StringBuilder stringBuilder, EnumerableType enumerableType, string codeToAccessTheEnumerable, XElement elementThatContainsTheChildrenToAdd, ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain)
        {
            List<XElement> children = elementThatContainsTheChildrenToAdd.Elements().ToList();
            List<string> childrenCode = PopElementsFromStackAndReadThemInReverseOrder(codeStack, children.Count).ToList();

            switch (enumerableType)
            {
                // If it is a simple Collection, we can Add elements without keys:
                case EnumerableType.Collection:
                    for (int i = 0; i < children.Count; i++)
                    {
                        XElement child = children[i];
                        stringBuilder.AppendLine(childrenCode[i]);
                        bool isChildAProperty = child.Name.LocalName.Contains('.');
                        if (!isChildAProperty) // This will skip for example "<ResourceDictionary.MergedDictionaries>". In fact, if we are inside a <ResourceDictionary></ResourceDictionary>, we want to add all the directly-defined children but not the property ".MergedDictionaries".
                        {
                            string childUniqueName = GetUniqueName(child);
                            stringBuilder.AppendLine(string.Format("{0}.Add({1});", codeToAccessTheEnumerable, childUniqueName));
                        }
                    }
                    break;
                // If it is a Dictionary (such as <ResourceDictionary></ResourceDictionary>), we need a key to add the element:
                case EnumerableType.Dictionary:
                    for (int i = 0; i < children.Count; i++)
                    {
                        XElement child = children[i];
                        stringBuilder.AppendLine(childrenCode[i]);
                        bool isChildAProperty = child.Name.LocalName.Contains('.');
                        if (!isChildAProperty) // This will skip for example "<ResourceDictionary.MergedDictionaries>". In fact, if we are inside a <ResourceDictionary></ResourceDictionary>, we want to add all the directly-defined children but not the property ".MergedDictionaries".
                        {
                            string childUniqueName = GetUniqueName(child);
                            bool isImplicitStyle;
                            bool isImplicitDataTemplate;
                            string childKey = GetElementXKey(child, reflectionOnSeparateAppDomain, out isImplicitStyle, out isImplicitDataTemplate);
                            if (isImplicitStyle || isImplicitDataTemplate)
                            {
                                stringBuilder.AppendLine(string.Format("{0}[typeof({1})] = {2};", codeToAccessTheEnumerable, childKey, childUniqueName));
                            }
                            else
                            {
                                stringBuilder.AppendLine(string.Format("{0}[\"{1}\"] = {2};", codeToAccessTheEnumerable, childKey, childUniqueName));
                            }
                        }
                    }
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        static string GenerateCodeForSetterProperty(XElement styleElement, string attributeValue, ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain)
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
                elementTypeInCSharp = reflectionOnSeparateAppDomain.GetCSharpEquivalentOfXamlTypeAsString(elementNamespaceName, elementLocalTypeName);
                dependencyPropertyName = splittedAttachedProperty[1] + "Property";
            }
            else
            {
                elementTypeInCSharp = GetCSharpFullTypeNameFromTargetTypeString(styleElement, reflectionOnSeparateAppDomain);
                dependencyPropertyName = attributeValue + "Property"; //todo: handle the case where the DependencyProperty name is not the name of the property followed by "Property" (at least improve the error message)
            }
            return string.Format("{0}.{1}", elementTypeInCSharp, dependencyPropertyName);
        }

        private static XName GetCSharpXNameFromTargetTypeOrAttachedPropertyString(XElement setterElement, bool isAttachedProperty, ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain)
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
                    throw new wpf::System.Windows.Markup.XamlParseException("Setter must declare a Property.");
            }
            else
            {
                currentXElement = setterElement.Parent.Parent;
                attributeToLookAt = currentXElement.Attribute("TargetType");
                if (attributeToLookAt == null)
                    throw new wpf::System.Windows.Markup.XamlParseException("Style must declare a TargetType.");
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
                        throw new wpf::System.Windows.Markup.XamlParseException(@"Namespaces or prefixes must be followed by a type.");
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
            return reflectionOnSeparateAppDomain.GetCSharpEquivalentOfXamlTypeAsXName(namespaceName, localTypeName, assemblyNameIfAny, ifTypeNotFoundTryGuessing: false);
        }

        static string GetCSharpFullTypeNameFromTargetTypeString(XElement styleElement, ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain, bool isDataType = false)
        {
            string namespaceName;
            string localTypeName;
            string assemblyNameIfAny;
            var targetTypeAttribute = styleElement.Attribute(isDataType ? "DataType" : "TargetType");
            if (targetTypeAttribute == null)
                throw new wpf::System.Windows.Markup.XamlParseException(isDataType ? "DataTemplate must declare a DataType or have a key." : "Style must declare a TargetType.");
            string targetTypeString = targetTypeAttribute.Value;
            GettingInformationAboutXamlTypes.GetClrNamespaceAndLocalName(targetTypeString, styleElement, out namespaceName, out localTypeName, out assemblyNameIfAny);
            string elementTypeInCSharp = reflectionOnSeparateAppDomain.GetCSharpEquivalentOfXamlTypeAsString(namespaceName, localTypeName, assemblyNameIfAny, ifTypeNotFoundTryGuessing: false);
            return elementTypeInCSharp;
        }

        static string GetCSharpFullTypeName(string typeString, XElement elementWhereTheTypeIsUsed, ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain)
        {
            string namespaceName;
            string localTypeName;
            string assemblyNameIfAny;
            GettingInformationAboutXamlTypes.GetClrNamespaceAndLocalName(typeString, elementWhereTheTypeIsUsed, out namespaceName, out localTypeName, out assemblyNameIfAny);
            string elementTypeInCSharp = reflectionOnSeparateAppDomain.GetCSharpEquivalentOfXamlTypeAsString(namespaceName, localTypeName, assemblyNameIfAny, ifTypeNotFoundTryGuessing: false);
            return elementTypeInCSharp;
        }

        static string CreateInitializeComponentMethod(string codeToWorkWithTheRootElement, List<string> findNameCalls, string codeToPlaceAtTheBeginningOfInitializeComponent, string codeToPlaceAtTheEndOfInitializeComponent, bool isSLMigration, string assemblyNameWithoutExtension, string fileNameWithPathRelativeToProjectRoot)
        {
            string uiElementFullyQualifiedTypeName = isSLMigration ? "global::System.Windows.UIElement" : "global::Windows.UI.Xaml.UIElement";
            string method = @"
        private bool _contentLoaded;
        public void InitializeComponent()
        {{
            if (_contentLoaded)
                return;
            _contentLoaded = true;

#pragma warning disable 0184 // Prevents warning CS0184 ('The given expression is never of the provided ('type') type')
            if (this is {4})
            {{
                (({4})(object)this).XamlSourcePath = @""{5}\{6}"";
            }}
#pragma warning restore 0184

{0}

{1}

{2}

{3}    
        }}
";
            string findNameCallsMerged = string.Join("\r\n", findNameCalls);

            return string.Format(method, codeToPlaceAtTheBeginningOfInitializeComponent, codeToWorkWithTheRootElement, findNameCallsMerged, codeToPlaceAtTheEndOfInitializeComponent, uiElementFullyQualifiedTypeName, assemblyNameWithoutExtension, fileNameWithPathRelativeToProjectRoot);
        }

        private static string CreateDataTemplateLambda(string codeToInstantiateTheDataTemplate, string dataTemplateUniqueName, string childUniqueName, string templateInstanceUniqueName, string codeToPlaceAtTheBeginningOfTheMethod, string additionalCodeToPlaceAtTheEndOfTheMethod, string namespaceSystemWindows)
        {
            string lambda = @"templateOwner_{1} => 
{{
var {2} = new {0}.TemplateInstance();
{2}.TemplateOwner = templateOwner_{1};
{6}
{3}
{4}
{2}.TemplateContent = {5};
return {2};
}}";

            /*
                        @"templateOwner_{"dataTemplateUniqueName"} => 
                        {
                            var {"dataTemplateUniqueName"} = new {"namespaceSystemWindows"}.TemplateInstance();
                            {"templateInstanceUniqueName"}.TemplateOwner = templateOwner_{"dataTemplateUniqueName"};
                            {"codeToPlaceAtTheBeginningOfTheMethod"}
                            {"codeToInstantiateTheDataTemplate"}
                            {"additionalCodeToPlaceAtTheEndOfTheMethod"}
                            {"templateInstanceUniqueName"}.TemplateContent = {"childUniqueName"};
                            return {"templateInstanceUniqueName"};
                        }";
            */

            return string.Format(lambda,
                                 namespaceSystemWindows,
                                 dataTemplateUniqueName,
                                 templateInstanceUniqueName,
                                 codeToInstantiateTheDataTemplate,
                                 additionalCodeToPlaceAtTheEndOfTheMethod,
                                 childUniqueName,
                                 codeToPlaceAtTheBeginningOfTheMethod);
        }

        //        static string CreateDataTemplateMethod(string codeToInstantiateTheDataTemplate, string dataTemplateUniqueName, string childUniqueName, string templateInstanceUniqueName, string additionalCodeToPlaceAtTheEndOfTheMethod, string namespaceSystemWindows, string namespaceSystemWindowsControls)
        //        {
        //            string method = @"
        //        private {0}.TemplateInstance Instantiate_{1}({6}.Control templateOwner)
        //        {{
        //var {2} = new {0}.TemplateInstance();
        //{2}.TemplateOwner = templateOwner;
        //{3}

        //{4}
        //{2}.TemplateContent = {5};
        //return {2};
        //        }}
        //";
        //            return string.Format(
        //                method,
        //                namespaceSystemWindows,
        //                dataTemplateUniqueName,
        //                templateInstanceUniqueName,
        //                codeToInstantiateTheDataTemplate,
        //                additionalCodeToPlaceAtTheEndOfTheMethod,
        //                childUniqueName,
        //                namespaceSystemWindowsControls);
        //        }

        static string GetUniqueName(XElement element)
        {
            if (element == element.Document.Root)
            {
                return "this";
            }
            else
            {
                return element.Attribute(GeneratingUniqueNames.AttributeNameForUniqueName).Value; // Note: this is supposed to exist because we have added it in a prior code.
            }
        }

        static string GetElementXKey(XElement element, ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain, out bool isImplicitStyle, out bool isImplicitDataTemplate)
        {
            isImplicitStyle = false;
            isImplicitDataTemplate = false;

            if (element.Attribute(xNamespace + "Key") != null)
                return element.Attribute(xNamespace + "Key").Value;
            else if (element.Attribute(xNamespace + "Name") != null)
                return element.Attribute(xNamespace + "Name").Value;
            else if (element.Name == DefaultXamlNamespace + "Style")
            {
                isImplicitStyle = true;
                return GetCSharpFullTypeNameFromTargetTypeString(element, reflectionOnSeparateAppDomain);
            }
            else if (element.Name == DefaultXamlNamespace + "DataTemplate"
                && element.Attribute("DataType") != null)
            {
                isImplicitDataTemplate = true;
                return GetCSharpFullTypeNameFromTargetTypeString(element, reflectionOnSeparateAppDomain, isDataType: true);
            }
            else
                throw new wpf::System.Windows.Markup.XamlParseException("Each dictionary entry must have an associated key. The element named '" + element.Name.LocalName + "' does not have a key.");
        }

        static string GeneratePartialClass(
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
            string classFactoryCode = @"
public static class {1}
{{
    public static object Instantiate()
    {{
        global::System.Type type = typeof({0});
        return global::CSHTML5.Internal.TypeInstantiationHelper.Instantiate(type);
    }}
}}
";

            string classCode = @"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by ""C#/XAML for HTML5""
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------



{5}partial class {0} : {1}
{{

#pragma warning disable 169, 649, 0628 // Prevents warning CS0169 ('field ... is never used'), CS0649 ('field ... is never assigned to, and will always have its default value null'), and CS0628 ('member : new protected member declared in sealed class')
{3}

{4}
#pragma warning restore 169, 649, 0628

{2}

{6}

}}
";
            string namespaceWrapperIfNecessary = @"
{2}
namespace {0}
{{

{1}

}}
";
            string applicationEntryPointIfAny =
                (addApplicationEntryPoint ?
                string.Format(@"
public static void Main()
{{
    new {0}();
}}", className) : "");
            string absoluteSourceUri = (fileNameWithPathRelativeToProjectRoot.Contains(';') ? fileNameWithPathRelativeToProjectRoot : "/" + assemblyNameWithoutExtension + ";component/" + fileNameWithPathRelativeToProjectRoot); // This line ensures that the Uri is in the absolute form: "/assemblyName;component/RelativePath/FileName.xaml"
            string classToInstantiateName = XamlFilesWithoutCodeBehindHelper.GenerateClassFactoryNameFromAbsoluteUri_ForRuntimeAccess(absoluteSourceUri);
            string methodsMergedCode = string.Join("\r\n\r\n", methods);
            string fieldsForNamedElementsMergedCode = string.Join("\r\n", fieldsForNamedElements);
            string fieldsToEnsureThatAllTypesReferencedInTheXamlFileAreReferenced = string.Join("\r\n", listOfAllTheTypesUsedInThisXamlFile.Select<string, string>(x => string.Format("private {0} Unused_{1};", x, Guid.NewGuid().ToString("N")))); // Note: This is useful because we need to generate some c# code for every type used in the XAML file because otherwise the types risk not being found at "Pass2" of the compilation. In fact, Visual Studio automatically removes project references that are not referenced from C#, so if a type is present only in XAML and not in C#, its DLL risks not being referenced.
            string classAccessModifier = (hasCodeBehind ? "" : "public "); // Note: if the class has code-behind, we do not specify an access modifier so that the user can specify it by setting an access modifier in the partial class of the code-behind. If there is no code-behind (such as for resource dictionaries), we set it to "public".
            string classCodeFilled = string.Format(classCode, className, baseType, methodsMergedCode, fieldsForNamedElementsMergedCode, fieldsToEnsureThatAllTypesReferencedInTheXamlFileAreReferenced, classAccessModifier, applicationEntryPointIfAny);
            string finalCode;
            if (!string.IsNullOrEmpty(namespaceStringIfAny))
            {
                classFactoryCode = string.Format(classFactoryCode, namespaceStringIfAny + "." + className, classToInstantiateName);
                finalCode = string.Format(namespaceWrapperIfNecessary, namespaceStringIfAny, classCodeFilled, classFactoryCode);
            }
            else
            {
                classFactoryCode = string.Format(classFactoryCode, className, classToInstantiateName);
                finalCode = classFactoryCode + classCodeFilled;
            }
            return finalCode;
        }

        static void GetClassInformationFromXaml(XDocument doc, string fileNameWithPathRelativeToProjectRoot, string assemblyNameWithoutExtension, ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain, out string className, out string namespaceStringIfAny, out string baseType, out bool hasCodeBehind)
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

                className = XamlFilesWithoutCodeBehindHelper.GenerateClassNameFromAssemblyAndPath(fileNameWithPathRelativeToProjectRoot, assemblyNameWithoutExtension);
                namespaceStringIfAny = null;
                hasCodeBehind = false;
                //todo: handle the case where there is a code-behing but the user has simply forgotten the "x:Class" attribute, in which case the user will currently get strange error messages.
            }

            // Get the base type of the control:
            string namespaceName, localTypeName, assemblyNameIfAny;
            GettingInformationAboutXamlTypes.GetClrNamespaceAndLocalName(doc.Root.Name, out namespaceName, out localTypeName, out assemblyNameIfAny);
            baseType = reflectionOnSeparateAppDomain.GetCSharpEquivalentOfXamlTypeAsString(namespaceName, localTypeName, assemblyNameIfAny, ifTypeNotFoundTryGuessing: true); // Note: we set "ifTypeNotFoundTryGuessing" to true because the type will not be found during Pass1 for example in the case that tthe root of the XAML file is: <myNamespace:MyCustumUserControlDerivedClass .../>
        }

        static string GetDefaultValueOfTypeAsString(string namespaceName, string localTypeName, bool isSystemType, ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain, string assemblyIfAny = null)
        {
            if (isSystemType)
            {
                return SystemTypesHelper.GetDefaultValueOfSystemTypeAsString(localTypeName);
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

        static string GenerateCodeForInstantiatingAttributeValue(XName xName, string propertyName, bool isAttachedProperty, string valueAsString, XElement elementWhereTheTypeIsUsed, string fileNameWithPathRelativeToProjectRoot, string assemblyNameWithoutExtension, ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain)
        {
            //// Check the type of the attribute:
            //Type valueType;
            //if (isAttachedProperty)
            //    valueType = GettingInformationAboutXamlTypes.GetMethodReturnValueType(xname, "Get" + propertyOrFieldName, reflectionOnSeparateAppDomain);
            //else
            //    valueType = GettingInformationAboutXamlTypes.GetPropertyOrFieldType(xname, propertyOrFieldName, reflectionOnSeparateAppDomain);
            //string valueTypeFullName = "global::" + valueType.FullName;

            string namespaceName, localTypeName, assemblyNameIfAny;
            GettingInformationAboutXamlTypes.GetClrNamespaceAndLocalName(xName, out namespaceName, out localTypeName, out assemblyNameIfAny);

            string valueNamespaceName, valueLocalTypeName;
            bool isValueString, isValueEnum;
            if (isAttachedProperty)
                reflectionOnSeparateAppDomain.GetMethodReturnValueTypeInfo("Get" + propertyName, namespaceName, localTypeName, out valueNamespaceName, out valueLocalTypeName, out isValueString, out isValueEnum, assemblyNameIfAny);
            else
                reflectionOnSeparateAppDomain.GetPropertyOrFieldTypeInfo(propertyName, namespaceName, localTypeName, out valueNamespaceName, out valueLocalTypeName, out isValueString, out isValueEnum, assemblyNameIfAny);

            string valueTypeFullName = "global::" + (!string.IsNullOrEmpty(valueNamespaceName) ? valueNamespaceName + "." : "") + valueLocalTypeName;
            //string generatedCSharpCode;

            //if (reflectionOnSeparateAppDomain.TryGenerateCodeForInstantiatingAttributeValue(valueAsString, out generatedCSharpCode, valueNamespaceName, valueLocalTypeName, assemblyNameIfAny))
            //{
            //    return generatedCSharpCode;
            //}


            // Generate the code or instantiating the attribute:
            if (isValueString)
            {
                //----------------------------
                // PROPERTY IS OF TYPE STRING
                //----------------------------
                return ConvertingStringToValue.PrepareStringForString(null, valueAsString);
            }
            else if (isValueEnum)
            {
                //----------------------------
                // PROPERTY IS AN ENUM
                //----------------------------
                if (valueAsString.IndexOf(',') != -1)
                {
                    string[] values = valueAsString.Split(new char[] { ',' })
                        .Select(v =>
                        {
                            return string.Format("{0}.{1}", valueTypeFullName,
                                reflectionOnSeparateAppDomain.GetFieldName(v.Trim(), valueNamespaceName, valueLocalTypeName, null));
                        }).ToArray();
                    return string.Join(" | ", values);
                }
                else
                {
                    return string.Format("{0}.{1}", valueTypeFullName,
                        reflectionOnSeparateAppDomain.GetFieldName(valueAsString, valueNamespaceName, valueLocalTypeName, null));
                }
            }
            else if (valueTypeFullName == "global::System.Type")
            {
                string typeFullName = GetCSharpFullTypeName(valueAsString, elementWhereTheTypeIsUsed, reflectionOnSeparateAppDomain);
                return string.Format("typeof({0})", typeFullName);
            }
            else
            {
                //----------------------------
                // PROPERTY IS OF ANOTHER TYPE
                //----------------------------
                ChangeRelativePathIntoAbsolutePathIfNecessary(ref valueAsString, valueTypeFullName, propertyName, fileNameWithPathRelativeToProjectRoot, assemblyNameWithoutExtension, xName);
                return ConvertingStringToValue.ConvertStringToValue(valueTypeFullName, valueAsString);
            }
        }

        static bool BindingRelativeSourceIsTemplatedParent(XElement child)
        {
            foreach (XElement bindingProperty in child.Elements())
            {
                if (bindingProperty.Name == GeneratingCSharpCode.DefaultXamlNamespace + "Binding.RelativeSource")
                {
                    foreach (XElement relativeSource in bindingProperty.Elements())
                    {
                        if (relativeSource.Name == GeneratingCSharpCode.DefaultXamlNamespace + "RelativeSource")
                        {
                            foreach (XAttribute attribute in relativeSource.Attributes())
                            {
                                if (attribute.Name.LocalName == "Mode" && attribute.Value == "TemplatedParent")
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        static void ChangeRelativePathIntoAbsolutePathIfNecessary(ref string path, string valueTypeFullName, string propertyName, string fileNameWithPathRelativeToProjectRoot, string assemblyNameWithoutExtension, XName parentXName)
        {
            // In the case of the "Frame" control, a relative URI to a ".xaml" file (used for navigation) should not be changed into an absolute URI, because it is relative to the Startup assembly, not to the current assembly where the value is defined:
            bool IsFrameOrUriMappingSpecialCase =
                parentXName.LocalName == "UriMapping"
                || parentXName.LocalName == "Frame"
                || parentXName.LocalName == "HyperlinkButton";

            // We change relative paths into absolute paths in case of <Image> controls and other controls that have the "Source" property:
            if ((valueTypeFullName == "global::Windows.UI.Xaml.Media.ImageSource"
                || valueTypeFullName == "global::System.Windows.Media.ImageSource"
                || valueTypeFullName == "global::System.Uri"
                || (propertyName == "FontFamily" && path.Contains('.')))
                && !IsFrameOrUriMappingSpecialCase
                && !path.ToLower().EndsWith(".xaml")) // Note: this is to avoid messing with Frame controls, which paths are always relative to the startup assembly (in SL).
            {
                if (!IsUriAbsolute(path) // This lines checks if the URI is in the form "ms-appx://" or "http://" or "https://" or "mailto:..." etc.
                    && !path.ToLower().Contains(@";component/")) // This line checks if the URI is in the form "/assemblyName;component/FolderName/FileName.xaml"
                {
                    // Get the relative path of the current XAML file:
                    string relativePathOfTheCurrentFile = Path.GetDirectoryName(fileNameWithPathRelativeToProjectRoot.Replace('\\', '/'));

                    // Combine the relative path of the current file with the path specified by the user:
                    string pathRelativeToProjectRoot = Path.Combine(relativePathOfTheCurrentFile.Replace('\\', '/'), path.Replace('\\', '/')).Replace('\\', '/');

                    // Surround the path with the assembly name to make it an absolute path in the form: "/assemblyName;component/FolderName/FileName.xaml"
                    path = "/" + assemblyNameWithoutExtension + ";component/" + pathRelativeToProjectRoot;
                }
            }
        }

        static bool IsUriAbsolute(string path)
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
    }
}
