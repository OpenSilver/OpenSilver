extern alias wpf;


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
using wpf::System.Windows.Controls.Primitives;

namespace DotNetForHtml5.Compiler
{
    internal static class FixingPropertiesOrder
    {
        public static void FixPropertiesOrder(XDocument doc, ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain)
        {
            FixSelectorItemsSourceOrder(doc.Root, reflectionOnSeparateAppDomain);
        }

        public static void FixSelectorItemsSourceOrder(XElement currentElement, ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain)
        {
            // Check if the current element is an object (rather than a property):
            bool isElementAnObjectRatherThanAProperty = !currentElement.Name.LocalName.Contains(".");
            if(isElementAnObjectRatherThanAProperty)
            {
                //check if the element has a attribute called ItemsSource:
                XAttribute itemsSourceAttribute = currentElement.Attribute(XName.Get("ItemsSource"));
                bool hasItemsSourceProperty = itemsSourceAttribute != null;

                //if the element has an attribute called ItemsSource, we check if it represents an object of a type that inherits from Selector:
                if(hasItemsSourceProperty)
                {
                    // Get the namespace, local name, and optional assembly that correspond to the element:
                    string namespaceName, localTypeName, assemblyNameIfAny;
                    GettingInformationAboutXamlTypes.GetClrNamespaceAndLocalName(currentElement.Name, out namespaceName, out localTypeName, out assemblyNameIfAny);
                    string selectorNamespaceName, selectorLocalTypeName, selectorAssemblyNameIfAny;
                    GettingInformationAboutXamlTypes.GetClrNamespaceAndLocalName("Selector", out selectorNamespaceName, out selectorLocalTypeName, out selectorAssemblyNameIfAny);
                    bool typeInheritsFromSelector = reflectionOnSeparateAppDomain.IsTypeAssignableFrom(namespaceName, localTypeName, assemblyNameIfAny, selectorNamespaceName, selectorLocalTypeName, selectorAssemblyNameIfAny);
                    //Type elementType = reflectionOnSeparateAppDomain.GetCSharpEquivalentOfXamlType(namespaceName, localTypeName, assemblyNameIfAny);
                    //if the type inherits from the element, we want to put the itemsSource attribute to the end:
                    if(typeInheritsFromSelector)
                    {
                        itemsSourceAttribute.Remove();
                        currentElement.Add(itemsSourceAttribute);
                    } //else we do nothing
                }//else we do nothing
            }//else we do nothing

            //we move on to the children of the element.
            if(currentElement.HasElements)
            {
                foreach(XElement child in currentElement.Elements())
                {
                    FixSelectorItemsSourceOrder(child, reflectionOnSeparateAppDomain);
                }
            }
        }

    }
}

