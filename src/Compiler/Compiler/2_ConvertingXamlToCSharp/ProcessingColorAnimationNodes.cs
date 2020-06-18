using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DotNetForHtml5.Compiler
{
    internal static class ProcessingColorAnimationNodes
    {
        //------------------------------------------------------------
        // this class will process the "ColorAnimation" nodes
        // in order to transform "Storyboard.TargetProperty="XXX""
        // into "Storyboard.TargetProperty="XXX.Color""
        // if XXX doesn't end on ".Color" or ".Color)"
        //------------------------------------------------------------

        public static void Process(XDocument doc, ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain)
        {
            TraverseNextElement(doc.Root, reflectionOnSeparateAppDomain);
        }

        static void TraverseNextElement(XElement currentElement, ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain)
        {
           
            if (currentElement.Name == GeneratingCSharpCode.DefaultXamlNamespace + "ColorAnimation")
            {
                //Storyboard.TargetProperty="Background.Color"
                string value = currentElement.Attribute("Storyboard.TargetProperty").Value;
                if (!string.IsNullOrWhiteSpace(value) 
                    && !(value.EndsWith(".Color") 
                        || value.EndsWith(".Color)")))
                {
                    value += ".Color";
                    currentElement.Attribute("Storyboard.TargetProperty").SetValue(value);
                }
            }

            // Recursion:
            foreach (var childElements in currentElement.Elements())
            {
                TraverseNextElement(childElements, reflectionOnSeparateAppDomain);
            }
        }
    }
}
