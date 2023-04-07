

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
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OpenSilver.Compiler
{
    internal static class ProcessingContentPresenterNodes
    {
        //------------------------------------------------------------
        // This class will process the "ContentPresenter" nodes
        // in order to transform "<ContentPresenter />" into
        // "<ContentPresenter Content="{TemplateBinding Content}"
        // ContentTemplate="{TemplateBinding ContentTemplate}" />"
        //------------------------------------------------------------

        public static void Process(XDocument doc, AssembliesInspector reflectionOnSeparateAppDomain)
        {
            TraverseNextElement(doc.Root, false, reflectionOnSeparateAppDomain);
        }

        static void TraverseNextElement(XElement currentElement, bool isInsideControlTemplate, AssembliesInspector reflectionOnSeparateAppDomain)
        {
            if (GeneratingCSharpCode.IsControlTemplate(currentElement))
            {
                isInsideControlTemplate = true;
            }

            if (isInsideControlTemplate && !currentElement.Name.LocalName.Contains("."))
            {
                bool isContentPresenter = reflectionOnSeparateAppDomain.IsAssignableFrom(
                    GeneratingCSharpCode.DefaultXamlNamespace,
                    "ContentPresenter",
                    currentElement.Name.NamespaceName,
                    currentElement.Name.LocalName);

                if (isContentPresenter)
                {
                    if (!HasAttribute(currentElement, "Content", reflectionOnSeparateAppDomain))
                    {
                        currentElement.Add(new XAttribute("Content", "{TemplateBinding Content}"));
                    }
                    if (!HasAttribute(currentElement, "ContentTemplate", reflectionOnSeparateAppDomain))
                    {
                        currentElement.Add(new XAttribute("ContentTemplate", "{TemplateBinding ContentTemplate}"));
                    }
                }
            }

            // Recursion:
            foreach (var childElements in currentElement.Elements())
            {
                TraverseNextElement(childElements, isInsideControlTemplate, reflectionOnSeparateAppDomain);
            }
        }

        private static bool HasAttribute(XElement cp, string attributeName, AssembliesInspector reflectionOnSeparateAppDomain)
        {
            bool found = cp.Attribute(attributeName) != null;
            if (!found)
            {
                foreach (var child in cp.Elements())
                {
                    string namespaceName = child.Name.NamespaceName;
                    string[] typeAndProperty = child.Name.LocalName.Split('.');

                    if (typeAndProperty.Length == 2)
                    {
                        // First check if this is the right property.
                        if (typeAndProperty[1].Trim() == attributeName)
                        {
                            // Then make sure this is not an attached property.
                            bool isProperty = reflectionOnSeparateAppDomain.IsAssignableFrom(
                                GeneratingCSharpCode.DefaultXamlNamespace,
                                "ContentPresenter",
                                namespaceName,
                                typeAndProperty[0]);

                            if (isProperty)
                            {
                                found = true;
                                break;
                            }
                        }
                    }
                }
            }

            return found;
        }
    }
}
