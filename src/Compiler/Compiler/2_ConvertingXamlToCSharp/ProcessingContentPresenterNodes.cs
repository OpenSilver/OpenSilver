

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



extern alias wpf;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DotNetForHtml5.Compiler
{
    internal static class ProcessingContentPresenterNodes
    {
        //------------------------------------------------------------
        // This class will process the "ContentPresenter" nodes
        // in order to transform "<ContentPresenter />" into
        // "<ContentPresenter Content="{TemplateBinding Content}"
        // ContentTemplate="{TemplateBinding ContentTemplate}" />"
        //------------------------------------------------------------

        public static void Process(XDocument doc, ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain)
        {
            TraverseNextElement(doc.Root, false, reflectionOnSeparateAppDomain);
        }
        
        static void TraverseNextElement(XElement currentElement, bool isInsideControlTemplate, ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain)
        {
            if (currentElement.Name == GeneratingCSharpCode.DefaultXamlNamespace + "ControlTemplate")
            {
                isInsideControlTemplate = true;
            }

            if (isInsideControlTemplate
                && currentElement.Name == GeneratingCSharpCode.DefaultXamlNamespace + "ContentPresenter")
            {
                if (currentElement.Attribute("Content") == null)
                {
                    if (!DoesContentPresenterContainDirectContent(currentElement))
                    {
                        currentElement.Add(new XAttribute("Content", "{TemplateBinding Content}"));
                    }
                }
                if (currentElement.Attribute("ContentTemplate") == null) //todo: also check if there is a <ContentPresenter.ContentTemplate> child node.
                    currentElement.Add(new XAttribute("ContentTemplate", "{TemplateBinding ContentTemplate}"));
            }

            // Recursion:
            foreach (var childElements in currentElement.Elements())
            {
                TraverseNextElement(childElements, isInsideControlTemplate, reflectionOnSeparateAppDomain);
            }
        }

        static bool DoesContentPresenterContainDirectContent(XElement contentPresenter)
        {
            // Check if there is direct content (note: we already added implicit nodes in a previous step):
            foreach (var child in contentPresenter.Elements())
            {
                if (child.Name == GeneratingCSharpCode.DefaultXamlNamespace + "ContentPresenter.Content")
                {
                    return true;
                }
            }
            return false;
        }
    }
}
