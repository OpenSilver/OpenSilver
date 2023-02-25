

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
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OpenSilver.Compiler
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
           
            if (GeneratingCSharpCode.IsColorAnimation(currentElement))
            {
                //Storyboard.TargetProperty="Background.Color"
                XAttribute targetPropertyAttr = currentElement.Attribute("Storyboard.TargetProperty");
                if (targetPropertyAttr != null)
                {
                    string value = targetPropertyAttr.Value;
                    if (!string.IsNullOrWhiteSpace(value)
                        && !(value.TrimStart('(').TrimEnd(')') == "Color")
                        && !value.TrimEnd(')').EndsWith(".Color"))
                    {
                        value += ".Color";
                        currentElement.Attribute("Storyboard.TargetProperty").SetValue(value);
                    }
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
