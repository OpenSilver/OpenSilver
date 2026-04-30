// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics;
using System.Text;

namespace OpenSilver.Compiler;

// Markup Extension Tokenizer AKA Scanner.

internal enum MeTokenType
{
    None,
    Open = '{',
    Close = '}',
    EqualSign = '=',
    Comma = ',',
    TypeName,      // String - Follows a '{' space delimited
    PropertyName,  // String - Preceeds a '='.  {},= delimited, can (but shouldn't) contain spaces.
    String,        // String - all other strings, {},= delimited can contain spaces.
    QuotedMarkupExtension // String - must be recursivly parsed as a MarkupExtension.
}

// 1) Value and (propertynames for compatibility with WPF 3.0) can also have
// escaped character with '\' to include '{' '}' ',' '=', and '\'.
// 2) Value strings can also be quoted (w/ ' or ") in their entirity to escape all
// uses of the above characters.
// 3) All strings are trimmed of whitespace front and back unless they were quoted.
// 4) Quote characters can only appear at the start and end of strings.
// 5) TypeNames cannot be quoted.

internal class MeScanner
{
    public const char Space = ' ';
    public const char OpenCurlie = '{';
    public const char CloseCurlie = '}';
    public const char Comma = ',';
    public const char EqualSign = '=';
    public const char Quote1 = '\'';
    public const char Quote2 = '\"';
    public const char Backslash = '\\';
    public const char NullChar = '\0';

    private enum StringState { Value, Type, Property };

    private readonly XamlParserContext _context;
    private readonly string _inputText;
    private readonly int _lineNumber;
    private readonly int _startPosition;
    private int _idx;
    private MeTokenType _token;
    private string _tokenXamlType;
    private string _tokenProperty;
    private string _tokenText;
    private StringState _state;
    private bool _hasTrailingWhitespace;

    public MeScanner(XamlParserContext context, string text, int lineNumber, int linePosition)
    {
        _context = context;
        _inputText = text;
        _lineNumber = lineNumber;
        _startPosition = linePosition;
        _idx = -1;
        _state = StringState.Value;
    }

    public int LineNumber
    {
        get { return _lineNumber; }
    }

    public int LinePosition
    {
        get
        {
            int offset = (_idx < 0) ? 0 : _idx;
            return _startPosition + offset;
        }
    }

    public MeTokenType Token
    {
        get { return _token; }
    }

    public string TokenType
    {
        get { return _tokenXamlType; }
    }

    public string TokenProperty
    {
        get { return _tokenProperty; }
    }

    public string TokenText
    {
        get { return _tokenText; }
    }

    public bool IsAtEndOfInput
    {
        get { return (_idx >= _inputText.Length); }
    }

    public bool HasTrailingWhitespace
    {
        get { return _hasTrailingWhitespace; }
    }

    public void Read()
    {
        bool isQuotedMarkupExtension = false;
        bool readString = false;

        _tokenText = string.Empty;
        _tokenXamlType = null;
        _tokenProperty = null;

        Advance();
        AdvanceOverWhitespace();

        if (IsAtEndOfInput)
        {
            _token = MeTokenType.None;
            return;
        }

        switch (CurrentChar)
        {
            case OpenCurlie:
                if (NextChar == CloseCurlie)    // the {} escapes the ME.  return the string.
                {
                    _token = MeTokenType.String;
                    _state = StringState.Value;
                    readString = true;          // ReadString() will strip the leading {}
                }
                else
                {
                    _token = MeTokenType.Open;
                    _state = StringState.Type;  // types follow '{'
                }

                break;

            case Quote1:
            case Quote2:
                if (NextChar == OpenCurlie)
                {
                    Advance();                    // read ahead one character
                    if (NextChar != CloseCurlie)  // check for the '}' of a {}
                    {
                        isQuotedMarkupExtension = true;
                    }

                    PushBack();                   // put back the read-ahead.
                }

                readString = true;  // read substring"
                break;

            case CloseCurlie:
                _token = MeTokenType.Close;
                _state = StringState.Value;
                break;

            case EqualSign:
                _token = MeTokenType.EqualSign;
                _state = StringState.Value;
                _context.CurrentBracketModeParseParameters.IsConstructorParsingMode = false;
                break;

            case Comma:
                _token = MeTokenType.Comma;
                _state = StringState.Value;
                if (_context.CurrentBracketModeParseParameters.IsConstructorParsingMode)
                {
                    _context.CurrentBracketModeParseParameters.IsConstructorParsingMode =
                        ++_context.CurrentBracketModeParseParameters.CurrentConstructorParam <
                        _context.CurrentBracketModeParseParameters.MaxConstructorParams;
                }

                break;

            default:
                readString = true;
                break;
        }

        if (readString)
        {
            string str = ReadString();
            _token = (isQuotedMarkupExtension) ? MeTokenType.QuotedMarkupExtension : MeTokenType.String;

            switch (_state)
            {
                case StringState.Value:
                    break;

                case StringState.Type:
                    _token = MeTokenType.TypeName;
                    ResolveTypeName(str);
                    break;

                case StringState.Property:
                    _token = MeTokenType.PropertyName;
                    ResolvePropertyName(str);
                    break;
            }

            _state = StringState.Value;
            _tokenText = RemoveEscapes(str);
        }
    }

    private static string RemoveEscapes(string value)
    {
        if (value.StartsWith("{}", StringComparison.OrdinalIgnoreCase))
        {
            value = value.Substring(2);
        }

        if (value.IndexOf(Backslash) == -1)
        {
            return value;
        }

        StringBuilder builder = new StringBuilder(value.Length);
        int start = 0;
        int idx;
        do
        {
            idx = value.IndexOf(Backslash, start);
            if (idx < 0)
            {
                builder.Append(value, start, value.Length - start);
                break;
            }
            else
            {
                int clearTextLength = idx - start;

                // Copy Clear Text
                builder.Append(value, start, clearTextLength);

                // Add the character after the backslash
                if (idx + 1 < value.Length)
                {
                    builder.Append(value[idx + 1]);
                }

                // pick up again after that
                start = idx + 2;
            }
        }
        while (start < value.Length);
        string result = builder.ToString();
        return result;
    }

    private void ResolveTypeName(string longName)
    {
        _tokenXamlType = longName;
    }

    private void ResolvePropertyName(string longName)
    {
        _tokenProperty = longName;
    }

    private string ReadString()
    {
        bool escaped = false;
        char quoteChar = NullChar;
        bool atStart = true;
        bool wasQuoted = false;
        uint braceCount = 0;    // To be compat with v3 which allowed balanced {} inside of strings

        StringBuilder sb = new StringBuilder();
        char ch;

        while (!IsAtEndOfInput)
        {
            ch = CurrentChar;

            // handle escaping and quoting first.
            if (escaped)
            {
                sb.Append(Backslash);
                sb.Append(ch);
                escaped = false;
            }
            else if (quoteChar != NullChar)
            {
                if (ch == Backslash)
                {
                    escaped = true;
                }
                else if (ch != quoteChar)
                {
                    sb.Append(ch);
                }
                else
                {
                    ch = CurrentChar;
                    quoteChar = NullChar;
                    break;  // we are done.
                }
            }
            else
            {
                bool done = false;
                switch (ch)
                {
                    case Space:
                        if (_state == StringState.Type)
                        {
                            done = true;  // we are done.
                            break;
                        }

                        sb.Append(ch);
                        break;

                    case OpenCurlie:
                        braceCount++;
                        sb.Append(ch);
                        break;
                    case CloseCurlie:
                        if (braceCount == 0)
                        {
                            done = true;
                        }
                        else
                        {
                            braceCount--;
                            sb.Append(ch);
                        }

                        break;
                    case Comma:
                        done = true;  // we are done.
                        break;

                    case EqualSign:
                        _state = StringState.Property;
                        done = true;  // we are done.
                        break;

                    case Backslash:
                        escaped = true;
                        break;

                    case Quote1:
                    case Quote2:
                        if (!atStart)
                        {
                            throw new XamlParseException(this, Errors.QuoteCharactersOutOfPlace);
                        }

                        quoteChar = ch;
                        wasQuoted = true;
                        break;

                    default:  // All other character (including whitespace)
                        sb.Append(ch);
                        break;
                }

                if (done)
                {
                    if (braceCount > 0)
                    {
                        throw new XamlParseException(this, Errors.UnexpectedTokenAfterME);
                    }

                    PushBack();
                    break;  // we are done.
                }
            }

            atStart = false;
            Advance();
        }

        if (quoteChar != NullChar)
        {
            throw new XamlParseException(this, Errors.UnclosedQuote);
        }

        string result = sb.ToString();
        if (!wasQuoted)
        {
            result = result.TrimEnd(KnownStrings.WhitespaceChars);
            result = result.TrimStart(KnownStrings.WhitespaceChars);
        }

        return result;
    }

    private char CurrentChar
    {
        get { return _inputText[_idx]; }
    }

    private char NextChar
    {
        get
        {
            if (_idx + 1 < _inputText.Length)
            {
                return _inputText[_idx + 1];
            }

            return NullChar;
        }
    }

    private bool Advance()
    {
        ++_idx;
        if (IsAtEndOfInput)
        {
            _idx = _inputText.Length;
            return false;
        }

        return true;
    }

    private static bool IsWhitespaceChar(char ch)
    {
        Debug.Assert(KnownStrings.WhitespaceChars.Length == 5);

        if (ch == KnownStrings.WhitespaceChars[0] ||
            ch == KnownStrings.WhitespaceChars[1] ||
            ch == KnownStrings.WhitespaceChars[2] ||
            ch == KnownStrings.WhitespaceChars[3] ||
            ch == KnownStrings.WhitespaceChars[4])
        {
            return true;
        }

        return false;
    }

    private void AdvanceOverWhitespace()
    {
        bool sawWhitespace = false;

        while (!IsAtEndOfInput && IsWhitespaceChar(CurrentChar))
        {
            sawWhitespace = true;
            Advance();
        }

        // WFP 3.0 errors on trailing whitespace.
        // [note: very first compat workaround in the new XAML parser]
        // Noticing trailing whitespace is not very natural in this parser.
        // so this extra code is here to implement this error.
        if (IsAtEndOfInput && sawWhitespace)
        {
            _hasTrailingWhitespace = true;
        }
    }

    private void PushBack()
    {
        _idx -= 1;
    }
}

internal class BracketModeParseParameters
{
    internal BracketModeParseParameters(XamlParserContext context)
    {
        CurrentConstructorParam = 0;
        IsConstructorParsingMode = true;
        MaxConstructorParams = 1;
    }

    internal int CurrentConstructorParam { get; set; }
    internal int MaxConstructorParams { get; set; }
    internal bool IsConstructorParsingMode { get; set; }
}
