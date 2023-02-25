

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
    internal static class GeneratingUniqueNames
    {
        // Note: we use '.' to make sure this attribute is not colliding with
        // any property defined by the user.
        public const string UniqueNameAttribute = "__.UniqueName.__";

        public static void ProcessDocument(XDocument doc)
        {
            TraverseNextElement(doc.Root);
        }

        private static void TraverseNextElement(XElement currentElement)
        {
            // If the current element is an object (rather than a property)
            if (!currentElement.Name.LocalName.Contains("."))
            {
                // Generate unique name
                string uniqueName = GenerateUniqueName(currentElement);

                // Assign unique name
                currentElement.SetAttributeValue(UniqueNameAttribute, uniqueName);
            }

            // Recursion:
            foreach (var childElements in currentElement.Elements())
            {
                TraverseNextElement(childElements);
            }
        }

        private static string GenerateUniqueName(XElement element)
        {
            string guidAsString = Guid.NewGuid().ToString("N"); // Example: 00000000000000000000000000000000
            string prefix = element.Name.LocalName;
            
            if (prefix.Length > 30)
            {
                prefix = prefix.Substring(0, 30);
            }
            
            return $"{prefix}_{guidAsString}"; // Example: Button_4541C363579C48A981219C392BF8ACD5
        }

        internal static string GenerateUniqueNameFromString(string str)
        {
            Guid guid = Guid.NewGuid();
            string guidAsString = guid.ToString("N"); // Example: 00000000000000000000000000000000
            string prefix = str;
            if (prefix.Length > 30)
                prefix = prefix.Substring(0, 30);
            return prefix + "_" + guidAsString; // Example: Button_4541C363579C48A981219C392BF8ACD5
        }
    }
}
