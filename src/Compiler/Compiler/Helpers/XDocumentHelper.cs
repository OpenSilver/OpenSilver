
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
using System.Xaml;
using System.Xml;
using System.Xml.Linq;

namespace OpenSilver.Compiler
{
    internal static class XDocumentHelper
    {
        public static XDocument Parse(string xaml, LoadOptions options)
        {
            XmlReader baseReader = XmlReader.Create(new StringReader(xaml), GetXmlReaderSettings(options));
            XmlReader reader = new CompatibleXmlReader(baseReader, TryGetCompatibleNamespace);
            return XDocument.Load(reader, options);
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