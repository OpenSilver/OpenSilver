
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
using System.Text.RegularExpressions;
using System.Xml.Linq;
using OpenSilver.Compiler.Common;

namespace OpenSilver.Compiler
{
    internal static class InsertingMarkupNodesInXaml
    {
        //todo: support strings that contain commas, like in: {Binding Value, ConverterParameter = 'One, two, three, four, five, six', Mode = OneWay}

        internal static void InsertMarkupNodes(XDocument doc,
            ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain,
            ConversionSettings settings)
        {
            TraverseNextElement(doc.Root, doc.Root.GetDefaultNamespace(), reflectionOnSeparateAppDomain, settings);
        }

        private static void TraverseNextElement(XElement currentElement,
            XNamespace lastDefaultNamespace,
            ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain,
            ConversionSettings settings)
        {
            XNamespace currentDefaultNamespace = currentElement.GetDefaultNamespace();
            if (currentDefaultNamespace == XNamespace.None)
            {
                currentDefaultNamespace = lastDefaultNamespace;
            }

            List<XAttribute> attributesToRemove = new List<XAttribute>();
            // We use a secondary list so that when we remove an element from the XAttributes, we don't skip the next element.
            // We reverse the list because we want to preserve the order in which we set the object properties. Since XContainer
            // only allow to add a child at the start or the end, we need call XContainer.AddFirst(element) while iterating on the 
            // reverse list of attributes to do so.
            var attributes = currentElement.Attributes().Reverse().ToList(); 
            foreach (XAttribute currentAttribute in attributes)
            {
                if (IsMarkupExtension(currentAttribute))
                {
                    // Skip if the attribute has a namespace but no "dot", such as d:DataContext="{...}", so that it is in line with what we do in "GeneratingCSharpCode.cs". This is actually needed to be able to compile because something like d:DataContext="{d:ClassThatDoesNotExist ...}" where "d" is in the list of "mc:Ignorable". //todo: a better approach would be to remove all attributes that have a prefix in the list of "mc:Ignorable".
                    if (string.IsNullOrEmpty(currentAttribute.Name.NamespaceName) || currentAttribute.Name.LocalName.Contains("."))
                    {
                        string currentElementTypeName = currentElement.Name.LocalName.Split('.')[0];
                        string currentElementNamespaceName = currentElement.Name.NamespaceName;
                        string currentAttributeTypeName;
                        string currentAttributeName;
                        string currentAttributeNamespaceName = currentAttribute.Name.NamespaceName;
                        string currentAttributeValueEscaped = EscapeCommasInQuotes(currentAttribute.Value); // This will replace for example Fallback='3,3,3,3' with Fallback='3<COMMA>3<COMMA>3<COMMA>3' (to make later parsing easier)
                        if (currentAttribute.Name.LocalName.Contains(".")) // case where the type of the currentAttribute is mentionned (ex : <Border Border.Background="..." />)
                        {
                            string[] attributeSplittedLocalName = currentAttribute.Name.LocalName.Split('.');
                            currentAttributeTypeName = attributeSplittedLocalName[0];
                            currentAttributeName = attributeSplittedLocalName[1];
                        }
                        else // if the type is not mentionned, we assume the property is defined in the type of currentElement (ex : <Border Background="..." />)
                        {
                            currentAttributeNamespaceName = currentElementNamespaceName;
                            currentAttributeTypeName = currentElementTypeName;
                            currentAttributeName = currentAttribute.Name.LocalName;
                        }
                        if (string.IsNullOrEmpty(currentAttributeNamespaceName)) // if the namespace of the currentAttribute is still empty at this point, it means that currentAttribute is an attached property defined in the current default namespace.
                        {
                            currentAttributeNamespaceName = currentDefaultNamespace.NamespaceName;
                        }
                        if (currentElementNamespaceName == currentAttributeNamespaceName && currentElementTypeName == currentAttributeTypeName) // currentAttribute is a property defined in the type of currentElement (or one of his parents)
                        {
                            currentElement.AddFirst(GenerateNodeForAttribute(
                                currentElement.Name + ("." + currentAttributeName),
                                currentAttributeValueEscaped,
                                currentDefaultNamespace,
                                reflectionOnSeparateAppDomain,
                                currentElement.GetNamespaceOfPrefix,
                                settings));
                        }
                        else // currentAttribute is an attached property
                        {
                            currentElement.AddFirst(GenerateNodeForAttribute(
                                "{" + currentAttributeNamespaceName + "}" + currentAttributeTypeName + "." + currentAttributeName,
                                currentAttributeValueEscaped,
                                currentDefaultNamespace,
                                reflectionOnSeparateAppDomain,
                                currentElement.GetNamespaceOfPrefix,
                                settings));
                        }
                        currentAttribute.Remove();
                    }
                }
            }

            // Recursion:
            foreach (var childElements in currentElement.Elements())
            {
                TraverseNextElement(childElements, currentDefaultNamespace, reflectionOnSeparateAppDomain, settings);
            }
        }

        internal static bool IsMarkupExtension(XAttribute attribute)
        {
            if (attribute != null)
            {
                string value = attribute.Value;
                if (value.StartsWith("{"))
                {
                    int indexOfClosingBracket = value.IndexOf('}');
                    if (indexOfClosingBracket < 0)
                    {
                        throw new XamlParseException(string.Format("Invalid value for attribute {0}. Use \"{{}}\" to escape '{{'.", attribute.Name), GeneratingCSharpCode.GetLineNumber(attribute.Parent), -1);
                    }
                    string contentBetweenBrackets = value.Substring(1, indexOfClosingBracket - 1);
                    if (string.IsNullOrEmpty(contentBetweenBrackets)) //handle special case where '{' is escaped with "{}"
                    {
                        return false;
                    }
                    else
                    {
                        string trimmedValue = value.Trim();
                        if (trimmedValue != string.Empty)
                        {
                            char c = trimmedValue[0];
                            return !(c >= '0' && c <= '9'); //We check whether the first character is a Number because of StringFormat (example: "{Binding ... StringFormat={0:N4}}" the "{0:N4}" part is not a MarkupExtension).
                        }
                        else
                        {
                            throw new XamlParseException(string.Format("Invalid value for attribute {0}. Use {} to escape '{'.", attribute.Name), GeneratingCSharpCode.GetLineNumber(attribute.Parent), -1);
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Generates a XElement with the name as given as a parameter and which contains the elements defined in the attributeValue parameter
        /// </summary>
        /// <param name="nodeName">the name Of the XElement to create (for example: Border.Background)</param>
        /// <param name="attributeValue">a string containing the whole attribute's definition (for example: "{Binding Toto, Mode = TwoWay, Converter = {StaticResource Myconverter}}")</param>
        /// <param name="reflectionOnSeparateAppDomain"></param>
        /// <returns></returns>
        private static XElement GenerateNodeForAttribute(
            XName nodeName,
            string attributeValue,
            XNamespace lastDefaultNamespace,
            ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain,
            Func<string, XNamespace> getNamespaceOfPrefix,
            ConversionSettings settings)
        {
            Dictionary<string, string> listOfSubAttributes = GenerateListOfAttributesFromString(attributeValue);
            List<XElement> elementsToAdd = new List<XElement>();
            List<XAttribute> attributesToAdd = new List<XAttribute>();

            foreach (string keyString in listOfSubAttributes.Keys)
            {
                string currentAttribute = listOfSubAttributes[keyString];
                bool isMarkupExtension = currentAttribute.StartsWith("{");
                if (isMarkupExtension)
                {
                    int indexOfNextClosingBracket = currentAttribute.IndexOf("}");
                    string contentBetweenBrackets = currentAttribute.Substring(1, indexOfNextClosingBracket - 1);
                    isMarkupExtension = (!string.IsNullOrWhiteSpace(contentBetweenBrackets));
                    if (isMarkupExtension)
                    {
                        //We check whether the first character is a Number because of StringFormat (example: "{Binding ... StringFormat={0:N4}}" the "{0:N4}" part is not a MarkupExtension).
                        string tempCurrentAttribute = contentBetweenBrackets.Trim();
                        char c = tempCurrentAttribute[0];
                        isMarkupExtension = !(c >= '0' && c <= '9');
                    }
                }
                if (isMarkupExtension) //example: {Binding Toto, Mode = TwoWay, Converter = {StaticResource Myconverter}}
                {
                    //-------------------------
                    // Markup Extension
                    //-------------------------

                    try
                    {
                        string content = currentAttribute.TrimStart('{').TrimStart();
                        int indexOfCharacterAfterClassName = content.IndexOf(' ');
                        if (indexOfCharacterAfterClassName < 0)
                            indexOfCharacterAfterClassName = content.IndexOf('}');

                        string currentSubAttributeWithoutUselessPart = content;
                        string nextClassName = content.Substring(0, indexOfCharacterAfterClassName); //nextClassName = "Binding"

                        currentSubAttributeWithoutUselessPart = currentSubAttributeWithoutUselessPart.Remove(0, indexOfCharacterAfterClassName).Trim();
                        currentSubAttributeWithoutUselessPart = currentSubAttributeWithoutUselessPart.Remove(currentSubAttributeWithoutUselessPart.Length - 1, 1); //to remove the '}' at the end

                        // We add the suffix "Extension" to the markup extension name (unless it is a Binding or RelativeSource). For example, "StaticResource" becomes "StaticResourceExtension":
                        if (nextClassName != "Binding" && nextClassName != "RelativeSource" && !nextClassName.EndsWith("Extension"))
                        {
                            // this is a trick, we need to check if :
                            // - type named 'MyCurrentMarkupExtensionName' exist.
                            // - if the previous does not exist, look for 'MyCurrentMarkupExtensionNameExtension'.
                            // - if none exist throw an Exception
                            // note: currently, this implementation can lead to a crash if a MarkupExtension is named "xxxxxExtensionExtension" and is used with "xxxxxExtension" in some xaml code.
                            nextClassName += "Extension";
                        }

                        // Determine the namespace and local name:
                        XNamespace ns;
                        string localName;
                        if (!TryGetNamespaceFromNameThatMayHaveAPrefix(nextClassName, getNamespaceOfPrefix, out ns, out localName))
                        {
                            ns = lastDefaultNamespace;
                            localName = nextClassName;
                        }

                        // Generate the elements:
                        XElement subXElement = GenerateNodeForAttribute(
                            ns + localName,
                            currentSubAttributeWithoutUselessPart,
                            lastDefaultNamespace,
                            reflectionOnSeparateAppDomain,
                            getNamespaceOfPrefix,
                            settings);
                        XElement subXElement1 = subXElement;
                        if (!nodeName.LocalName.Contains('.'))
                        {
                            subXElement1 = new XElement(nodeName + "." + keyString, subXElement);
                        }
                        elementsToAdd.Add(subXElement1);
                    }
                    catch (Exception ex)
                    {
                        throw new XamlParseException("Error in the following markup extension: \"" + currentAttribute + "\". " + ex.Message);
                    }
                }
                else //it can be directly set as an attribute because it is not a markupExtension:
                {
                    //-------------------------
                    // Not a markup extension
                    //-------------------------

                    string keyStringAfterPlaceHolderReplacement = keyString;
                    if (keyString == "_placeHolderForDefaultValue") //this test is to replace the name of the attribute (which is in keyString) if it was a placeholder
                    {
                        GettingInformationAboutXamlTypes.GetClrNamespaceAndLocalName(
                            nodeName,
                            settings.EnableImplicitAssemblyRedirection,
                            out string namespaceName,
                            out string localName,
                            out string assemblyNameIfAny);
                        keyStringAfterPlaceHolderReplacement = reflectionOnSeparateAppDomain.GetContentPropertyName(namespaceName, localName, assemblyNameIfAny);
                    }
                    else if (keyStringAfterPlaceHolderReplacement.StartsWith("{")) //if we enter this if, it means that keyString is of the form "{Binding ElementName" so we want to remove "{Binding "
                    {
                        int indexOfFirstSpace = keyStringAfterPlaceHolderReplacement.IndexOf(' ');
                        keyStringAfterPlaceHolderReplacement = keyStringAfterPlaceHolderReplacement.Remove(0, indexOfFirstSpace + 1); //+1 because we want to remove the space
                    }
                    if (currentAttribute.EndsWith("}"))
                    {
                        int i = 0;
                        foreach (char c in currentAttribute)
                        {
                            if (c == '{')
                                ++i;
                            if (c == '}')
                                --i;
                        }
                        if (i < 0) // We do not want to remove the last '}' if it comes from something like "StringFormat=This is {0}" (in Bindings here).
                        {
                            currentAttribute = currentAttribute.Remove(currentAttribute.Length - 1);
                        }
                    }
                    currentAttribute = currentAttribute.Replace("<COMMA>", ","); // Unescape (cf. code where <COMMA> is added)
                    XAttribute attribute = new XAttribute(keyStringAfterPlaceHolderReplacement, currentAttribute);
                    attributesToAdd.Add(attribute);
                }
            }

            XNamespace xNamespace = @"http://schemas.microsoft.com/winfx/2006/xaml/presentation"; //todo: support markup extensions that use custom namespaces.
            string actualNodeName = nodeName.LocalName;
            string nodeNamespace = "{" + nodeName.NamespaceName + "}";
            string[] splittedNodeName = actualNodeName.Split('.');
            if (splittedNodeName.Length == 3) //case where we have an attached property (ex: <Border Canvas.Left={Binding...})
            {
                actualNodeName = splittedNodeName[1] + "." + splittedNodeName[2];
            }
            XElement xElement = new XElement(nodeNamespace + actualNodeName, attributesToAdd, elementsToAdd);
            return xElement;
        }

        static bool TryGetNamespaceFromNameThatMayHaveAPrefix(string nameThatMayHaveAPrefix, Func<string, XNamespace> getNamespaceOfPrefix, out XNamespace ns, out string localName)
        {
            int indexOfColons = nameThatMayHaveAPrefix.IndexOf(':');
            if (indexOfColons != -1)
            {
                string prefix = nameThatMayHaveAPrefix.Substring(0, indexOfColons);
                if (!string.IsNullOrEmpty(prefix))
                {
                    localName = nameThatMayHaveAPrefix.Substring(indexOfColons + 1);
                    ns = getNamespaceOfPrefix(prefix);
                    if (ns != null)
                    {
                        return true;
                    }
                    else
                    {
                        // Unknown prefix.
                        throw new XamlParseException(string.Format("Unknown prefix '{0}' in '{1}'", prefix, nameThatMayHaveAPrefix));
                    }
                }
                else
                {
                    // Empty prefix.
                    throw new XamlParseException(string.Format("Empty prefix in '{1}'", prefix, nameThatMayHaveAPrefix));
                }
            }
            else
            {
                // No prefix.
                ns = null;
                localName = nameThatMayHaveAPrefix;
                return false;
            }
        }

        static Dictionary<string, string> GenerateListOfAttributesFromString(string attributeValue)
        {
            //example of attributeValue:
            //"Toto, Mode = TwoWay, Converter = {StaticResource MyConverter}
            //(Note: Before having that as an attributeValue, we entered this method with "{Binding Toto, Mode = TwoWay, Converter = {StaticResource MyConverter}}")

            Dictionary<string, string> returnValue = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(attributeValue))
            {
                //we want: 
                //  _placeHolderForDefaultValue --> Toto
                //  Mode --> TwoWay
                //  Converter --> {StaticResource MyConverter}
                string str = attributeValue;

                //we want right now : "_placeHolderForDefaultValue = Toto,  Mode = TwoWay, Converter = {StaticResource MyConverter}
                int indexOfFirstBracket = str.IndexOf('{'); //this is for the case where we have a MarkupExtension (which can contain equals and commas) as default value
                int indexOfFirstComma = str.IndexOf(',');
                int indexOfFirstEqualSign = str.IndexOf('=');

                //we get rid of the spaces around the first equal sign:
                if (indexOfFirstEqualSign != -1)
                {
                    string stringBeforeFirstEqualSign = str.Substring(0, indexOfFirstEqualSign);
                    string stringAfterFirstEqualSign = str.Substring(indexOfFirstEqualSign + 1);
                    str = stringBeforeFirstEqualSign.Trim() + "=" + stringAfterFirstEqualSign.Trim();
                }
                int indexOfFirstSpace = str.IndexOf(' '); //this one is to know if there is a space before the 

                bool commaIsBeforeFirstEqualSign = indexOfFirstComma != -1 && indexOfFirstComma <= indexOfFirstEqualSign; //we want a placeHolder for when it is the case (ex: toto, Converter=MyConverter)
                bool bracketIsBeforeFirstCommaAndFirstEqualSign = (indexOfFirstBracket != -1 &&
                        (indexOfFirstComma != -1 && indexOfFirstBracket < indexOfFirstComma)
                        && (indexOfFirstEqualSign != -1 && indexOfFirstBracket < indexOfFirstEqualSign)); //same as above (ex: {Binding toto, Converter=MyConverter}
                bool spaceIsBeforeFirstEqualSign = (indexOfFirstSpace != -1 && indexOfFirstSpace < indexOfFirstEqualSign); //Note: checking the position of the first equal sign compared to the first space might be irrelevant ? (it should not lead to a wrong result anyway)

                if (indexOfFirstEqualSign == -1 || commaIsBeforeFirstEqualSign || bracketIsBeforeFirstCommaAndFirstEqualSign || spaceIsBeforeFirstEqualSign) //if this test is true, it means that we have to find the name of the property (inferior or equal for the case where there is only the default parameter --> both equal -1)
                {
                    str = "_placeHolderForDefaultValue = " + str;
                }

                //string subAttributeKey = str.Remove(indexOfFirstEqualSign); 
                //subAttributeKey = subAttributeKey.Trim();
                //str = str.Remove(0, indexOfFirstEqualSign + 1);
                //str = str.Trim();
                char[] splitElements = { ',' };
                string[] splittedAttributes = str.Split(splitElements, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < splittedAttributes.Length; ++i)
                {
                    string currentSplit = splittedAttributes[i]; ;
                    string subAttributeKey = null;
                    string subAttributeValue = null;
                    //++i;
                    indexOfFirstEqualSign = currentSplit.IndexOf('=');

                    if (indexOfFirstEqualSign != -1)
                    {
                        subAttributeKey = currentSplit.Remove(indexOfFirstEqualSign); //we know that everything before the fisrt equal is the name of an attribute because if it was not, there would still be the placeHolder (and after a comma, the attribute name is mandatory).
                        subAttributeValue = currentSplit.Remove(0, indexOfFirstEqualSign + 1); //we know that at least the rest is the value (since we need a comma to split the attributes). "At least" because it could be more because of MarkupExtensions --> '{'
                    }

                    //we need to make sure the subAttributeValue is not split (for example if the subAttributeValue is in the form: {Binding bobo, Mode = TwoWay}):
                    //we count the brackets:
                    if (subAttributeValue.Contains('{'))
                    {
                        int j = 0;
                        j += CountBrackets(subAttributeValue);
                        while (j != 0)
                        {
                            ++i;
                            string s = splittedAttributes[i];
                            j += CountBrackets(s);
                            subAttributeValue += ", " + s;
                        }
                    }

                    subAttributeKey = subAttributeKey.Trim();

                    // trim empty characters first.
                    subAttributeValue = subAttributeValue.Trim();
                    // Check if string value start with a quote
                    // if so :
                    // 1 - trim the quotes (we should probably verify that the
                    // string ends with a quote)
                    // 2 - if the first character is a {, add {} at the
                    // beginning of the string to ensure that the { is still
                    // escaped.
                    if (subAttributeValue.StartsWith("\'"))
                    {
                        subAttributeValue = subAttributeValue.Trim('\'');
                        subAttributeValue = UnescapeString(subAttributeValue);
                        if (subAttributeValue.StartsWith("{") && 
                            !subAttributeValue.StartsWith("{}"))
                        {
                            subAttributeValue = "{}" + subAttributeValue;
                        }
                    }

                    //now we have both our key and our value ready to be added:
                    returnValue.Add(subAttributeKey, subAttributeValue);
                }
            }
            return returnValue;
        }

        private static string UnescapeString(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            var sb = new StringBuilder();
            bool isEscaped = false;
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                
                if (c == '\\' && !isEscaped)
                {
                    isEscaped = true;
                    continue;
                }

                sb.Append(c);
                isEscaped = false;
            }

            return sb.ToString();
        }

        static int CountBrackets(string str)
        {
            int j = 0;
            for (int k = 0; k < str.Length; ++k)
            {
                if (str[k] == '{')
                {
                    ++j;
                }
                else if (str[k] == '}')
                {
                    --j;
                }
            }
            return j;
        }

        /// <summary>
        /// This will replace for example Fallback='3,3,3,3' with Fallback='3<COMMA>3<COMMA>3<COMMA>3' (to make later parsing easier)
        /// </summary>
        static string EscapeCommasInQuotes(string markupExtension)
        {
            return Regex.Replace(markupExtension, @"='[^']+'", delegate (Match match)
            {
                string value = match.ToString();
                return value.Replace(",", "<COMMA>");
            });
        }
    }
}
