
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

using System.Xml;
using System.Xml.Linq;

namespace OpenSilver.Compiler;

internal sealed class ExtendedXElement : XElement, IXmlLineInfo
{
    private LineInfo _lineInfo;

    public ExtendedXElement(XName name)
        : base(name)
    {
    }

    public ExtendedXElement(XName name, object content)
        : base(name, content)
    {
    }

    public ExtendedXElement(XName name, params object[] content)
        : base(name, content)
    {
    }

    public void SetLineInfo(IXmlLineInfo lineInfo)
    {
        if (lineInfo is not null && lineInfo.HasLineInfo())
        {
            _lineInfo = new LineInfo(lineInfo.LineNumber, lineInfo.LinePosition);
        }
    }

    int IXmlLineInfo.LineNumber => _lineInfo?.LineNumber ?? 0;

    int IXmlLineInfo.LinePosition => _lineInfo?.LinePosition ?? 0;

    bool IXmlLineInfo.HasLineInfo() => _lineInfo is not null;

    private sealed class LineInfo
    {
        public LineInfo(int lineNumber, int linePosition)
        {
            LineNumber = lineNumber;
            LinePosition = linePosition;
        }

        public readonly int LineNumber;
        public readonly int LinePosition;
    }
}
