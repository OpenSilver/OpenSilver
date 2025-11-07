
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

using System.IO;
using System.Text.RegularExpressions;
using System.Xaml;
using System.Xml;
using System.Xml.Linq;

namespace OpenSilver.Compiler
{
    internal static class XDocumentHelper
    {
        public static XDocument Parse(string xaml, LoadOptions options)
        {
            // Remove XML processing instructions (like <?xml...?> and <?xaml-comp...?>) and any leading
            // whitespace/BOM that may appear at the beginning of the file, as they cause parsing errors
            // with the XML reader ("Data at the root level is invalid").
            xaml = RemoveXmlProcessingInstructions(xaml);

            // Load the XAML into an XDocument using the CompatibleXmlReader to handle namespaces
            XmlReader baseReader = XmlReader.Create(new StringReader(xaml), GetXmlReaderSettings(options));
            XmlReader reader = new CompatibleXmlReader(baseReader, TryGetCompatibleNamespace);
            return XDocument.Load(reader, options);
        }

        private static string RemoveXmlProcessingInstructions(string xaml)
        {
            // Remove XML processing instructions (like <?xml...?> and <?xaml-comp...?>)
            // Pattern matches: <?...?> using non-greedy matching
            string pattern = @"<\?.*?\?>";
            xaml = Regex.Replace(xaml, pattern, string.Empty);

            // Remove BOM (Byte Order Mark - character 65279 or \uFEFF) and all leading whitespace.
            // XML parsers do not accept any characters before the root element.
            // TrimStart with char array ensures we remove BOM, spaces, tabs, newlines, etc.
            // Note: This will affect line numbers in error reporting, but it's necessary for parsing.
            char[] charsToTrim = new char[] { '\uFEFF', ' ', '\t', '\r', '\n', '\v', '\f' };
            xaml = xaml.TrimStart(charsToTrim);

            return xaml;
        }

        private static XmlReaderSettings GetXmlReaderSettings(LoadOptions o)
        {
            XmlReaderSettings rs = new XmlReaderSettings();
            if ((o & LoadOptions.PreserveWhitespace) == 0) rs.IgnoreWhitespace = true;

            // DtdProcessing.Parse; Parse is not defined in the public contract
            rs.DtdProcessing = (DtdProcessing)2;
            rs.MaxCharactersFromEntities = (long)1e7;
            // rs.XmlResolver = null;
            return rs;
        }

        private static bool TryGetCompatibleNamespace(string ns, out string compatible)
        {
            if (ns == GeneratingCode.DefaultXamlNamespace ||
                ns == GeneratingCode.LegacyXamlNamespace ||
                ns == GeneratingCode.xNamespace.NamespaceName ||
                ns == "http://schemas.microsoft.com/winfx/2006/xaml/presentation/sdk" ||
                ns == "http://schemas.microsoft.com/winfx/2006/xaml/presentation/toolkit")
            {
                compatible = ns;
                return true;
            }
            compatible = null;
            return false;
        }
    }
}