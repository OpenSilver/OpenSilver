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
                    currentElement.Add(new XAttribute("Content", "{TemplateBinding Content}"));
                if (currentElement.Attribute("ContentTemplate") == null)
                    currentElement.Add(new XAttribute("ContentTemplate", "{TemplateBinding ContentTemplate}"));
            }

            // Recursion:
            foreach (var childElements in currentElement.Elements())
            {
                TraverseNextElement(childElements, isInsideControlTemplate, reflectionOnSeparateAppDomain);
            }
        }
    }
}
