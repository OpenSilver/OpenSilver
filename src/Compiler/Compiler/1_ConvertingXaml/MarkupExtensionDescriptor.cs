
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

using System.Collections.Generic;
using System.Xml;

namespace OpenSilver.Compiler;

public sealed class MarkupExtensionDescriptor
{
    public string Name { get; set; }

    public List<object> ConstructorArguments { get; } = [];

    public List<(string Name, object Value)> Properties { get; } = [];

    public static bool LooksLikeAMarkupExtension(string attrValue)
    {
        if (attrValue.Length < 2) return false;
        if (attrValue[0] != '{') return false;
        if (attrValue[1] == '}') return false;
        return true;
    }

    public static MarkupExtensionDescriptor Parse(string value, IXmlLineInfo lineInfo)
    {
        var parser = new MarkupExtensionParser(new XamlParserContext());
        var descriptor = parser.Parse(value, lineInfo.LineNumber, lineInfo.LinePosition);

        if (!HasValidPositionalArguments(descriptor))
        {
            throw new XamlParseException(Errors.TooManyPositionalArguments, lineInfo);
        }

        return descriptor;
    }

    private static bool HasValidPositionalArguments(MarkupExtensionDescriptor descriptor)
    {
        if (descriptor.ConstructorArguments.Count > 1)
        {
            return false;
        }

        foreach (var arg in descriptor.ConstructorArguments)
        {
            if (arg is MarkupExtensionDescriptor d && !HasValidPositionalArguments(d))
            {
                return false;
            }
        }

        foreach (var arg in descriptor.Properties)
        {
            if (arg.Value is MarkupExtensionDescriptor d && !HasValidPositionalArguments(d))
            {
                return false;
            }
        }

        return true;
    }
}