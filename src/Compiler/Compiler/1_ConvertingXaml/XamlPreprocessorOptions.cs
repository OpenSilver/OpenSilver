
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

using System;
using System.Xml.XPath;

namespace OpenSilver.Compiler;

public readonly struct XamlPreprocessorOptions
{
    public static readonly XamlPreprocessorOptions Auto = new("[Auto]");
    public static readonly XamlPreprocessorOptions Optimize = new("[Optimize]");

    public XamlPreprocessorOptions(string xPath)
    {
        XPath = xPath ?? throw new ArgumentNullException(nameof(xPath));
    }

    public string XPath { get; }

    public override int GetHashCode() => XPath?.GetHashCode() ?? 0;

    public override bool Equals(object obj) => obj is XamlPreprocessorOptions other && this == other;

    public static bool operator ==(XamlPreprocessorOptions left, XamlPreprocessorOptions right) => left.XPath == right.XPath;

    public static bool operator !=(XamlPreprocessorOptions left, XamlPreprocessorOptions right) => !(left == right);
}

internal static class XamlPreprocessorOptionsHelpers
{
    public static bool TryParse(string str, out XamlPreprocessorOptions options)
    {
        if (string.IsNullOrEmpty(str) || string.Equals(str, "Auto", StringComparison.OrdinalIgnoreCase))
        {
            options = XamlPreprocessorOptions.Auto;
            return true;
        }
        else if (string.Equals(str, "Optimize", StringComparison.OrdinalIgnoreCase))
        {
            options = XamlPreprocessorOptions.Optimize;
            return true;
        }
        else if (str.StartsWith("XPath:", StringComparison.OrdinalIgnoreCase))
        {
            string xPath = str.Substring("XPath:".Length);

            try
            {
                if (XPathExpression.Compile(xPath).ReturnType == XPathResultType.NodeSet)
                {
                    options = new XamlPreprocessorOptions(xPath);
                    return true;
                }
            }
            catch { }
        }

        options = XamlPreprocessorOptions.Auto;
        return false;
    }
}
