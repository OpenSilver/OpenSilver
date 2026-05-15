
/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using System.Xml.Linq;

namespace OpenSilver.Compiler;

internal static class GeneratingUniqueNames
{
    // Note: we use '.' to make sure this attribute is not colliding with
    // any property defined by the user.
    public static readonly XName UniqueNameAttribute = GeneratingCode.xNamespace.GetName("__.UniqueName.__");

    public static void ProcessDocument(XDocument doc, ConversionSettings settings)
    {
        TraverseNextElement(doc.Root, settings);
    }

    private static void TraverseNextElement(XElement element, ConversionSettings settings)
    {
        // If the current element is an object (rather than a property)
        if (!XamlParser.IsMemberNode(element))
        {
            // Generate unique name
            string uniqueName = settings.NameProvider.GetName(element.Name.LocalName);

            // Assign unique name
            element.SetAttributeValue(UniqueNameAttribute, uniqueName);
        }

        foreach (var childElements in element.Elements())
        {
            TraverseNextElement(childElements, settings);
        }
    }
}
