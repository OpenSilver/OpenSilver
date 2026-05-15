

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
using System.Xml.Linq;

namespace OpenSilver.Compiler
{
    internal static class GeneratingUniqueNames
    {
        // Note: we use '.' to make sure this attribute is not colliding with
        // any property defined by the user.
        public static readonly XName UniqueNameAttribute = GeneratingCode.xNamespace.GetName("__.UniqueName.__");

        [ThreadStatic]
        private static Dictionary<string, int> _nextCounterPerPrefix;

        public static void ProcessDocument(XDocument doc)
        {
            _nextCounterPerPrefix ??= [];
            TraverseNextElement(doc.Root);
        }

        private static void TraverseNextElement(XElement currentElement)
        {
            // If the current element is an object (rather than a property)
            if (!currentElement.Name.LocalName.Contains("."))
            {
                // Generate unique name
                string uniqueName = GenerateUniqueName(currentElement.Name.LocalName);

                // Assign unique name
                currentElement.SetAttributeValue(UniqueNameAttribute, uniqueName);
            }

            // Recursion:
            foreach (var childElements in currentElement.Elements())
            {
                TraverseNextElement(childElements);
            }
        }

        internal static string GenerateUniqueName(string str)
        {
            ReadOnlySpan<char> prefix = str.AsSpan();

            if (prefix.Length > 30)
            {
                prefix = prefix.Slice(0, 30);
            }

            // Because of f# warning, makes the first letter lower case
            string typeName = $"{char.ToLower(prefix[0])}{prefix.Slice(1)}";

            _nextCounterPerPrefix ??= [];
            _nextCounterPerPrefix.TryGetValue(typeName, out int n);
            _nextCounterPerPrefix[typeName] = ++n;

            return $"{typeName}{n}"; // Example: button1, button2, ...
        }
    }
}
