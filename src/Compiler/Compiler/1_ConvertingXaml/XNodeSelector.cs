
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

using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;

namespace OpenSilver.Compiler;

public abstract class XNodeSelector
{
    public static XNodeSelector Create(XDocument doc, XamlPreprocessorOptions options)
    {
        if (options == XamlPreprocessorOptions.Auto)
        {
            return new AutoNodeSelector(doc, options);
        }
        else if (options == XamlPreprocessorOptions.Optimize)
        {
            return new OptimizeNodeSelector(doc, options);
        }
        return new XPathNodeSelector(doc, options);
    }

    public bool IsMatch(XElement element)
    {
        if (element.Document.Root == element)
        {
            return false;
        }

        return IsMatchCore(element);
    }

    public abstract bool IsMatchCore(XElement element);

    private sealed class AutoNodeSelector : XNodeSelector
    {
        public AutoNodeSelector(XDocument doc, XamlPreprocessorOptions options) { }

        public override bool IsMatchCore(XElement element) => true;
    }

    private sealed class OptimizeNodeSelector : XNodeSelector
    {
        public OptimizeNodeSelector(XDocument doc, XamlPreprocessorOptions options) { }

        public override bool IsMatchCore(XElement element) => false;
    }

    private sealed class XPathNodeSelector : XNodeSelector
    {
        private readonly HashSet<XElement> _elements;

        public XPathNodeSelector(XDocument doc, XamlPreprocessorOptions options)
        {
            _elements = [.. doc.XPathSelectElements(options.XPath)];
        }

        public override bool IsMatchCore(XElement element) => _elements.Contains(element);
    }
}
