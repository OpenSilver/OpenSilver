
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

using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace OpenSilver.Compiler
{
    internal static class InsertingImplicitNodes
    {
        // Note: we use '.' to make sure this attribute is not colliding with
        // any property defined by the user.
        public static readonly XName InitializedFromStringAttribute = GeneratingCode.xNamespace.GetName("__.InitializeFromString.__");

        public static void InsertImplicitNodes(XDocument doc, ConversionSettings settings)
        {
            var indexesMapper = new Stack<List<int>>();
            TraverseNextElement(doc.Root, 0, indexesMapper, settings);
        }

        private static void TraverseNextElement(
            XElement currentElement, 
            int currentElementIndex, 
            /*Stack<Dictionary<int, int>> indexesMapper*/ Stack<List<int>> indexesMapper,
            ConversionSettings settings)
        {
            bool skipTraversalOfChildren = false;

            // Copy the children into a new array so that if we remove items from the collection,
            // it does not affect the traversal
            XElement[] children = currentElement.Elements().ToArray();

            // Check if the current element is an object (rather than a property)
            var indexesMap = new List<int>(children.Length);

            if (!XamlParser.IsMemberNode(currentElement))
            {
                //----------------------------------
                // CASE: OBJECT (e.g. <Button> or <TextBlock>)
                //----------------------------------

                // Make a list of all the child nodes that are not part of a property of the current
                // element (e.g. if the current element is a Border that contains a Button, we detect
                // "<Button>" but we ignore "<Border.Child>" and "<ToolTipService.ToolTip>" because
                // they are properties)
                List<XElement> nodesThatAreNotPropertiesOfTheObject = new List<XElement>();
                for (int i = 0; i < children.Length; i++)
                {
                    XElement child = children[i];
                    if (!XamlParser.IsMemberNode(child))
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
                    TypeDefinition type = GetTypeDefinition(currentElement.Name, currentElement, settings);
                    var contentPropertyName = settings.Inspector.GetContentPropertyName(
                        type,
                        currentElement);

                    if (contentPropertyName is null)
                    {
                        throw new XamlParseException(
                            $"Cannot add content to object of type '{settings.TypeReferenceHelper.ConvertToString(type)}'.",
                            currentElement);
                    }

                    XElement contentWrapper = currentElement;
                    
                    if (!string.IsNullOrEmpty(contentPropertyName))
                    {
                        // Wrap the child elements
                        var wrapper = new ExtendedXElement(currentElement.Name + "." + contentPropertyName);
                        wrapper.SetLineInfo(currentElement);

                        contentWrapper = wrapper;
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
                if (ContainsTextNode(currentElement, out XText directTextContent))
                {
                    bool initializeFromString = true;

                    // Get information about the element namespace and assembly
                    TypeDefinition elementType = GetTypeDefinition(currentElement.Name, currentElement, settings);
                    string elementTypeName = settings.TypeReferenceHelper.ConvertToString(elementType);
                    string assemblyName = elementType.GetAssemblyName();

                    string contentValue = directTextContent.Value;

                    if (!string.IsNullOrWhiteSpace(contentValue))
                    {
                        string contentPropertyName = settings.Inspector.GetContentPropertyName(elementType, currentElement);

                        if (!string.IsNullOrEmpty(contentPropertyName))
                        {
                            initializeFromString = false;

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

                            contentValue = CollapseWhitespaces(contentValue);

                            // Verify that the attribute is not already set
                            if (currentElement.Attribute(contentPropertyName) is not null)
                            {
                                throw new XamlParseException(
                                    $"'{elementTypeName}.{contentPropertyName}' property has already been set and can be set only once.",
                                    directTextContent);
                            }

                            // SPECIAL CASE: If we are in a TextBlock, we want to set the
                            // property "TextBlock.Text" instead of "TextBlock.Inlines"
                            if (GeneratingCode.IsTextBlock(currentElement, settings))
                            {
                                contentPropertyName = "Text";
                            }

                            // Add the Content attribute
                            var attribute = new ExtendedXAttribute(contentPropertyName, contentValue);
                            attribute.SetLineInfo(directTextContent);

                            currentElement.Add(attribute);

                            // Remove the direct text content
                            directTextContent.Remove();
                        }
                    }

                    if (initializeFromString && (
                        settings.CoreTypes.IsKnownType(elementTypeName, assemblyName) ||
                        settings.SystemTypes.IsKnownType(elementTypeName, assemblyName) ||
                        settings.Inspector.IsEnum(elementType) ||
                        settings.Inspector.HasTypeConverter(elementType)))
                    {
                        currentElement.SetAttributeValue(InitializedFromStringAttribute, contentValue.Trim());
                        directTextContent.Remove();
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
                if (ContainsTextNode(currentElement, out XText directTextContent))
                {
                    XElement parent = currentElement.Parent;

                    XName xName = currentElement.Name;
                    if (parent.Name.Namespace == currentElement.Name.Namespace)
                    {
                        int index = currentElement.Name.LocalName.IndexOf('.');
                        if (parent.Name.LocalName.Equals(currentElement.Name.LocalName.AsSpan(0, index), StringComparison.Ordinal))
                        {
                            xName = XNamespace.None.GetName(currentElement.Name.LocalName.Substring(index + 1));
                        }
                    }

                    // Replace multiple spaces (and line returns) with just one space (same behavior as in WPF): 
                    //cf. http://stackoverflow.com/questions/1279859/how-to-replace-multiple-white-spaces-with-one-white-space
                    string contentValue = CollapseWhitespaces(directTextContent.Value).Trim();

                    if (contentValue.StartsWith("{"))
                    {
                        contentValue = "{}" + contentValue;
                    }

                    // Verify that the attribute is not already set:
                    if (currentElement.Attribute(xName.LocalName) is not null)
                    {
                        throw new XamlParseException($"'{xName.LocalName}' property has already been set and can be set only once.", currentElement);
                    }

                    // Add the attribute
                    var attribute = new ExtendedXAttribute(xName, contentValue);
                    attribute.SetLineInfo(currentElement);

                    currentElement.Parent.Add(attribute);

                    // Remove the element
                    currentElement.Remove();

                    // It's useless to traverse the children because we have removed the element
                    skipTraversalOfChildren = true;
                }
                else
                {
                    for (int i = 0; i < children.Length; i++)
                    {
                        XElement child = children[i];
                        if (!XamlParser.IsMemberNode(child))
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
                    TraverseNextElement(childElements, i, indexesMapper, settings);
                    ++i;
                }
                indexesMapper.Pop();
            }
        }

        // Replace multiple spaces (and line returns) with just one space (same behavior as in WPF)
        // cf. http://stackoverflow.com/questions/1279859/how-to-replace-multiple-white-spaces-with-one-white-space
        private static string CollapseWhitespaces(string s) => Regex.Replace(s, @"\s{2,}", " ");

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

        private static TypeDefinition GetTypeDefinition(XName xName, IXmlLineInfo lineInfo, ConversionSettings settings)
        {
            settings.XamlNameParser.GetClrNamespaceAndLocalName(
                xName,
                out string namespaceName,
                out string typeName,
                out string assemblyName);

            return settings.Inspector.GetTypeDefinition(
                namespaceName,
                typeName,
                assemblyName,
                lineInfo);
        }
    }
}
