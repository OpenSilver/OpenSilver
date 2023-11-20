
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

namespace OpenSilver.Compiler
{
    internal static class GeneratingPathInXaml
    {
        // Note: we use '.' to make sure this attribute is not colliding with
        // any property defined by the user.
        public const string PathInXamlAttribute = "__.PathInXaml.__";
        public static void ProcessDocument(XDocument doc)
        {
            TraverseNextElement(doc.Root, "0");
        }

        private static void TraverseNextElement(XElement element, string path)
        {
            // Assign path
            element.SetAttributeValue(PathInXamlAttribute, path);
            // Recursion:
            int i = 0;
            foreach (var child in element.Elements())
            {
                TraverseNextElement(child, $"{path}.{i}");
                i++;
            }
        }
    }
}
