
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
using System.Xml;

namespace OpenSilver.Compiler;

[Serializable]
public class XamlParseException : Exception
{
    private readonly LineInfo _lineInfo;

    internal XamlParseException(MeScanner meScanner, string message)
        : base(message)
    {
        _lineInfo = new LineInfo(meScanner.LineNumber, meScanner.LinePosition);
    }

    public XamlParseException(string message)
        : this(message, null, null)
    {
    }

    public XamlParseException(string message, IXmlLineInfo lineInfo)
        : this(message, lineInfo, null)
    {
    }

    public XamlParseException(string message, IXmlLineInfo lineInfo, Exception innerException)
        : base(message, innerException)
    {
        if (lineInfo is not null && lineInfo.HasLineInfo())
        {
            _lineInfo = new LineInfo(lineInfo.LineNumber, lineInfo.LinePosition);
        }
    }

    /// <summary>
    /// LineNumber that the exception occured on.
    /// </summary>
    public int LineNumber => _lineInfo?.LineNumber ?? 0;

    /// <summary>
    /// LinePosition that the exception occured on.
    /// </summary>
    public int LinePosition => _lineInfo?.LinePosition ?? 0;

    /// <inheritdoc />
    public override string Message
    {
        get
        {
            if (HasLineInfo())
            {
                return $"{base.Message} Line {LineNumber} Position {LinePosition}.";
            }

            return base.Message;
        }
    }

    internal bool HasLineInfo() => _lineInfo is not null;

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