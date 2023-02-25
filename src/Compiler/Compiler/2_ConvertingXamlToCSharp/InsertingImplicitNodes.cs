
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

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using OpenSilver.Compiler.Common;

namespace OpenSilver.Compiler
{
    internal static class InsertingImplicitNodes
    {
        // Note: we use '.' to make sure this attribute is not colliding with
        // any property defined by the user.
        public const string InitializedFromStringAttribute = "__.InitializeFromString.__";

        public static void InsertImplicitNodes(XDocument doc,
            ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain,
            ConversionSettings settings)
        {
            var indexesMapper = new Stack<List<int>>();
            TraverseNextElement(doc.Root, 0, indexesMapper, reflectionOnSeparateAppDomain, settings);
        }

        private static void TraverseNextElement(
            XElement currentElement, 
            int currentElementIndex, 
            /*Stack<Dictionary<int, int>> indexesMapper*/ Stack<List<int>> indexesMapper, 
            ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain,
            ConversionSettings settings)
        {
            bool skipTraversalOfChildren = false;

            // Copy the children into a new array so that if we remove items from the collection,
            // it does not affect the traversal
            XElement[] children = currentElement.Elements().ToArray();

            // Check if the current element is an object (rather than a property)
            bool isElementAnObject = !currentElement.Name.LocalName.Contains(".");
            var indexesMap = new List<int>(children.Length);

            if (isElementAnObject)
            {
                //----------------------------------
                // CASE: OBJECT (e.g. <Button> or <TextBlock>)
                //----------------------------------

                // Make a list of all the child nodes that are not part of a property of the current
                // element (e.g. if the current element is a Border that contains a Button, we detect
                // "<Button>" but we ignore "<Border.Child>" and "<ToolTipService.ToolTip>" because
                // they are properties)
                List<XElement> nodesThatAreNotPropertiesOfTheObject = new List<XElement>();
                XElement child;
                for (int i = 0; i < children.Length; i++)
                {
                    child = children[i];
                    if (!child.Name.LocalName.Contains("."))
                    {
                        nodesThatAreNotPropertiesOfTheObject.Add(child);
                        indexesMap.Add(i);
                    }
                }

                //-------------------------------------------------------------
                // Explicitly add the "ContentProperty" to the XAML. For example,
                //   <Border>
                //     <TextBlock/>
                //   </Border>
                // becomes
                //   <Border>
                //     <Border.Child>
                //       <TextBlock/>
                //     </Border.Child>
                //   </Border>
                //-------------------------------------------------------------
                // If that list is not empty, put those child elements into a group, which name is
                // the default children property (aka "ContentProperty") of the parent
                if (nodesThatAreNotPropertiesOfTheObject.Count > 0)
                {
                    // Find out the name of the default children property (aka "ContentProperty") of the current element:
                    GettingInformationAboutXamlTypes.GetClrNamespaceAndLocalName(
                        currentElement.Name,
                        settings.EnableImplicitAssemblyRedirection,
                        out string namespaceName,
                        out string localName,
                        out string assemblyNameIfAny);
                    var contentPropertyName = reflectionOnSeparateAppDomain.GetContentPropertyName(namespaceName, localName, assemblyNameIfAny);
                    XElement contentWrapper = currentElement;
                    
                    if (contentPropertyName != null)
                    {
                        // Wrap the child elements
                        contentWrapper = new XElement(currentElement.Name + "." + contentPropertyName);
                    }
                    
                    foreach (var childElement in nodesThatAreNotPropertiesOfTheObject)
                    {
                        childElement.Remove();
                    }

                    contentWrapper.Add(nodesThatAreNotPropertiesOfTheObject.ToArray<object>());
                    
                    if (contentWrapper != currentElement)
                    {
                        currentElement.Add(contentWrapper);
                    }
                }

                //-------------------------------------------------------------
                // If there is some direct text content (such as <Button>content</Button), convert
                // the text into an attribute (such as <Button Content="content"></Button>) if the
                // element has the "[ContentProperty]" attribute.
                // Note: if the type is a system type (such as <sys:Double>50</sys:Double>), we
                // ignore it because later its value will be directly assigned.
                // Similarly, if the type is an Enum or a known type from the OpenSilver runtime
                // (for instance Thickness, Rect, Size...), we also ignore it because later it will
                // be transformed into a call to the "TypeFromStringConverters" class.
                //-------------------------------------------------------------
                XText directTextContent;
                if (ContainsTextNode(currentElement, out directTextContent))
                {
                    // Read the content
                    string contentValue = directTextContent.Value;

                    // Get information about the element namespace and assembly
                    GettingInformationAboutXamlTypes.GetClrNamespaceAndLocalName(
                        currentElement.Name,
                        settings.EnableImplicitAssemblyRedirection,
                        out string namespaceName,
                        out string localName,
                        out string assemblyNameIfAny);

                    string elementTypeInCSharp = reflectionOnSeparateAppDomain.GetCSharpEquivalentOfXamlTypeAsString(
                            namespaceName, localName, assemblyNameIfAny, false
                    );
                                    

                    // Distinguish system types (string, double, etc.) to other types
                    if (SystemTypesHelper.IsSupportedSystemType(elementTypeInCSharp.Substring("global::".Length), assemblyNameIfAny))
                    {
                        // In this case we do nothing because system types are handled
                        // later in the process. Example: "<sys:Double>50</sys:Double>"
                        // becomes "Double x = 50;"
                    }
                    else if (reflectionOnSeparateAppDomain.IsTypeAnEnum(namespaceName, localName, assemblyNameIfAny) ||
                             settings.CoreTypesConverter.IsSupportedCoreType(elementTypeInCSharp.Substring("global::".Length), assemblyNameIfAny))
                    {
                        // Add the attribute that will tell the compiler to later
                        // intialize the type by converting from the string using the
                        // "TypeFromStringConverters" class
                        currentElement.SetAttributeValue(InitializedFromStringAttribute, contentValue);
                        
                        // Remove the direct text content
                        directTextContent.Remove();
                    }
                    else
                    {
                        // Ensure the content is not empty
                        if (!string.IsNullOrWhiteSpace(contentValue)) //todo: do we really need this?
                        {
                            List<int> siblings = indexesMapper.Peek();

                            // If it is the first child, we want to trim the start of the string. (Silverlight behavior)
                            if (currentElementIndex == siblings[0]) //at least of size 1 (it contains currentElement)
                            {
                                contentValue = contentValue.TrimStart();
                            }

                            // If it is the last child, we want to trim the end of the string. (Silverlight behavior)
                            if (currentElementIndex == siblings[siblings.Count - 1])
                            {
                                contentValue = contentValue.TrimEnd();
                            }

                            // Replace multiple spaces (and line returns) with just one space (same behavior as in WPF)
                            // cf. http://stackoverflow.com/questions/1279859/how-to-replace-multiple-white-spaces-with-one-white-space
                            contentValue = Regex.Replace(contentValue, @"\s{2,}", " ");

                            string contentPropertyName = 
                                reflectionOnSeparateAppDomain.GetContentPropertyName(
                                    namespaceName, localName, assemblyNameIfAny
                                );

                            if (!string.IsNullOrEmpty(contentPropertyName))
                            {
                                // Verify that the attribute is not already set
                                if (currentElement.Attribute(contentPropertyName) != null)
                                {
                                    throw new XamlParseException(
                                        string.Format("The property '{0}' is set more than once.", contentPropertyName)
                                    );
                                }

                                // SPECIAL CASE: If we are in a TextBlock, we want to set the
                                // property "TextBlock.Text" instead of "TextBlock.Inlines"
                                if (GeneratingCSharpCode.IsTextBlock(currentElement) || GeneratingCSharpCode.IsRun(currentElement))
                                {
                                    contentPropertyName = "Text";
                                }

                                // Add the Content attribute
                                XAttribute attribute = new XAttribute(contentPropertyName, contentValue);
                                currentElement.Add(attribute);

                                // Remove the direct text content
                                directTextContent.Remove();
                            }
                            else
                            {
                                throw new XamlParseException(
                                    string.Format("The element '{0}' does not support direct content.", currentElement.Name)
                                );
                            }
                        }
                    }
                }
            }
            else
            {
                //----------------------------------
                // CASE: PROPERTY (e.g. <Button.Visibility> or <TextBlock.Text> or <ToolTipService.ToolTip>)
                //----------------------------------

                // If there is some direct text content (such as <Button.Visibility>Collapsed</Button.Visibility>
                // or <ToolTipService.ToolTip>Test</ToolTipService.ToolTip>), we convert the text into an attribute
                // (such as <Button Visibility="Collapsed"></Button> or <Button ToolTipService.ToolTip="Test></Button>)
                XText directTextContent;
                if (ContainsTextNode(currentElement, out directTextContent))
                {
                    // Check if we are on a direct object property (such as <Button.Visibility>) or an attached property
                    // (such as <ToolTipService.ToolTip>). For example, if the current element is <TextBlock.Text> and
                    // the parent is <TextBlock>, the result is true. Conversely, if the current element is
                    // <ToolTipService.ToolTip> and the parent is <Border>, the result is false.
                    bool isAttachedProperty = 
                        currentElement.Parent.Name != (currentElement.Name.Namespace + currentElement.Name.LocalName.Substring(0, currentElement.Name.LocalName.IndexOf(".")));

                    // Read the content
                    string contentValue = directTextContent.Value;

                    // Get the property name
                    string contentPropertyName = isAttachedProperty ? 
                        currentElement.Name.LocalName : 
                        currentElement.Name.LocalName.Substring(currentElement.Name.LocalName.IndexOf(".") + 1);

                    // Replace multiple spaces (and line returns) with just one space (same behavior as in WPF): 
                    //cf. http://stackoverflow.com/questions/1279859/how-to-replace-multiple-white-spaces-with-one-white-space
                    contentValue = Regex.Replace(contentValue, @"\s{2,}", " ").Trim();
                    if (!string.IsNullOrEmpty(contentValue))
                    {
                        contentValue = contentValue[0] == '{' ? "{}" + contentValue : contentValue;
                    }

                    // Verify that the attribute is not already set:
                    if (currentElement.Attribute(contentPropertyName) != null)
                    {
                        throw new XamlParseException(
                            string.Format("The property '{0}' is set more than once.", contentPropertyName)
                        );
                    }

                    // Add the attribute
                    currentElement.Parent.Add(
                        new XAttribute(contentPropertyName, contentValue)
                    );

                    // Remove the element
                    currentElement.Remove();

                    // It's useless to traverse the children because we have removed the element
                    skipTraversalOfChildren = true;
                }
                else
                {
                    XElement child;
                    for (int i = 0; i < children.Length; i++)
                    {
                        child = children[i];
                        if (!child.Name.LocalName.Contains("."))
                        {
                            indexesMap.Add(i);
                        }
                    }
                }
            }

            // Recursion
            if (!skipTraversalOfChildren)
            {
                indexesMapper.Push(indexesMap);
                int i = 0;
                foreach (var childElements in children)
                {
                    TraverseNextElement(childElements, i, indexesMapper, reflectionOnSeparateAppDomain, settings);
                    ++i;
                }
                indexesMapper.Pop();
            }
        }

        private static bool ContainsTextNode(XElement element, out XText textNode)
        {
            foreach (var child in element.Nodes())
            {
                if (child is XText xText)
                {
                    textNode = xText;
                    return true;
                }
            }

            textNode = null;
            return false;
        }
    }
}
