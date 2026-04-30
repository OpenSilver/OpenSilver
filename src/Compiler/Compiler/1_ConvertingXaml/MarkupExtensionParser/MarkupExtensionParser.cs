// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Diagnostics;

namespace OpenSilver.Compiler;

internal struct MarkupExtensionParser
{
    private readonly XamlParserContext _context;
    private string _originalText;
    private MeScanner _tokenizer;
    private string _brokenRule;

    [DebuggerDisplay("{found}")]
    private class Found
    {
        public bool found;
    }

    public MarkupExtensionParser(XamlParserContext stack)
    {
        _context = stack;
    }

    // MarkupExtension ::= '{' TYPENAME Arguments? '}'
    //    Arguments    ::= (PositionalArgs ( ',' NamedArgs)?) | NamedArgs
    //    NamedArgs    ::= NamedArg ( ',' NamedArg )*
    //    NamedArg     ::= PROPERTYNAME '=' (STRING | QUOTEDMARKUPEXTENSION | MarkupExtension)
    //  PositionalArgs ::= (Value (',' PositionalArgs)?) | NamedArg
    //       Value     ::= STRING | QUOTEDMARKUPEXTENSION |MarkupExtension

    public MarkupExtensionDescriptor Parse(string text, int lineNumber, int linePosition)
    {
        _tokenizer = new MeScanner(_context, text, lineNumber, linePosition);
        _originalText = text;
        Found f = new Found();
        NextToken();

        var descriptor = P_MarkupExtension(f);

        if (!f.found)
        {
            string brokenRule = _brokenRule;
            _brokenRule = null;
            throw new XamlParseException(_tokenizer, brokenRule);
        }

        if (_tokenizer.Token != MeTokenType.None)
        {
            throw new XamlParseException(_tokenizer, Errors.UnexpectedTokenAfterME);
        }

        if (_tokenizer.HasTrailingWhitespace)
        {
            throw new XamlParseException(_tokenizer, Errors.WhitespaceAfterME);
        }

        return descriptor;
    }

    private void SetBrokenRuleString(string ruleString)
    {
        if (string.IsNullOrEmpty(_brokenRule))
        {
            _brokenRule = string.Format(Errors.UnexpectedToken, _tokenizer.Token, ruleString, _originalText);
        }
    }

    private bool Expect(MeTokenType token, string ruleString)
    {
        if (_tokenizer.Token != token)
        {
            SetBrokenRuleString(ruleString);
            return false;
        }

        return true;
    }

    ////////////////////////////////
    // MarkupExtension ::= '{' TYPENAME Arguments? '}'
    //
    private MarkupExtensionDescriptor P_MarkupExtension(Found f)
    {
        MarkupExtensionDescriptor descriptor = null;

        // MarkupExtension ::= @'{' TYPENAME Arguments? '}'
        if (Expect(MeTokenType.Open, "MarkupExtension ::= @'{' Expr '}'"))
        {
            NextToken();

            // MarkupExtension ::= '{' @TYPENAME Arguments? '}'
            if (_tokenizer.Token == MeTokenType.TypeName)
            {
                Logic_StartElement();

                NextToken();

                // MarkupExtension ::= '{' TYPENAME @(Arguments)? '}'
                Found f2 = new Found();
                switch (_tokenizer.Token)
                {
                    // MarkupExtension ::= '{' TYPENAME (Arguments)? @'}'
                    case MeTokenType.Close:  // legal, Arguments is optional
                        descriptor = Logic_EndObject();
                        NextToken();
                        f.found = true;
                        break;

                    case MeTokenType.String:
                    case MeTokenType.QuotedMarkupExtension:
                    case MeTokenType.PropertyName:
                    case MeTokenType.Open:
                        // MarkupExtension ::= '{' TYPENAME (@Arguments)? '}'
                        foreach (var arg in P_Arguments(f2))
                        {
                            Logic_SetMember(arg.Name, arg.Value);
                        }

                        break;

                    default:
                        SetBrokenRuleString("MarkupExtension ::= '{' TYPENAME @(Arguments)? '}'");
                        break;
                }

                if (f2.found)
                {
                    if (Expect(MeTokenType.Close, "MarkupExtension ::= '{' TYPENAME (Arguments)? @'}'"))
                    {
                        descriptor = Logic_EndObject();
                        f.found = true;
                        NextToken();
                    }
                }
            }
            else
            {
                SetBrokenRuleString("MarkupExtension ::= '{' @TYPENAME (Arguments)? '}'");
            }
        }

        return descriptor;
    }

    ////////////////////////////////
    // Arguments ::= (PositionalArgs ( ',' NamedArgs)?) | NamedArgs
    //
    private IEnumerable<(string Name, object Value)> P_Arguments(Found f)
    {
        Found f2 = new Found();
        // Arguments ::= @ (PositionalArgs ( ',' NamedArgs)?) | NamedArgs
        switch (_tokenizer.Token)
        {
            case MeTokenType.Close:  // not found
                break;

            // Arguments ::= (@ PositionalArgs ( ',' NamedArgs)?) | NamedArgs
            case MeTokenType.String:
            case MeTokenType.QuotedMarkupExtension:
            case MeTokenType.Open:
                foreach (var arg in P_PositionalArgs(f2))
                {
                    yield return arg;
                }

                f.found = f2.found;
                if (f.found)
                {
                    if (_context.CurrentArgCount > 0)
                    {
                        Logic_EndPositionalParameters();
                    }
                }

                // Arguments ::= (PositionalArgs @ ( ',' NamedArgs)?) | NamedArgs
                while (_tokenizer.Token == MeTokenType.Comma)
                {
                    // Arguments ::= (PositionalArgs ( @ ',' NamedArgs)?) | NamedArgs
                    NextToken();

                    // Arguments ::= (PositionalArgs ( ',' @ NamedArgs)?) | NamedArgs
                    foreach (var arg in P_NamedArgs(f2))
                    {
                        yield return arg;
                    }
                }

                break;

            // Arguments ::= (PositionalArgs ( ',' NamedArgs)?) | @ NamedArgs
            case MeTokenType.PropertyName:
                foreach (var arg in P_NamedArgs(f2))
                {
                    yield return arg;
                }

                f.found = f2.found;
                break;

            default:
                SetBrokenRuleString("Arguments ::= @ (PositionalArgs ( ',' NamedArgs)?) | NamedArgs");
                break;
        }
    }

    ////////////////////////////////
    //  PositionalArgs ::= (Value (',' PositionalArgs)?) | NamedArg
    //
    private IEnumerable<(string Name, object Value)> P_PositionalArgs(Found f)
    {
        Found f2 = new Found();

        // PositionalArgs ::= @ (Value (',' PositionalArgs)?) | NamedArg
        switch (_tokenizer.Token)
        {
            // PositionalArgs ::= ( @ Value (',' PositionalArgs)?) | NamedArg
            case MeTokenType.String:
            case MeTokenType.QuotedMarkupExtension:
            case MeTokenType.Open:
                if (_context.CurrentArgCount++ == 0)
                {
                    Logic_StartPositionalParameters();
                }

                yield return (null, P_Value(f2));

                if (!f2.found)
                {
                    SetBrokenRuleString("PositionalArgs ::= (NamedArg | (@Value (',' PositionalArgs)?)");
                    break;
                }

                f.found = f2.found;

                // PositionalArgs ::= (Value @ (',' PositionalArgs)?) | NamedArg
                if (_tokenizer.Token == MeTokenType.Comma)
                {
                    Found f3 = new Found();

                    // PositionalArgs ::= (Value ( @ ',' PositionalArgs)?) | NamedArg
                    NextToken();

                    // PositionalArgs ::= (Value (',' @ PositionalArgs)?) | NamedArg
                    foreach (var arg in P_PositionalArgs(f3))
                    {
                        yield return arg;
                    }

                    if (!f3.found)
                    {
                        SetBrokenRuleString("PositionalArgs ::= (Value (',' @ PositionalArgs)?) | NamedArg");
                        break;
                    }

                    // no f.found this is optional
                }

                break;

            // PositionalArgs ::= (Value (',' PositionalArgs)?) | @ NamedArg
            case MeTokenType.PropertyName:
                if (_context.CurrentArgCount > 0)
                {
                    Logic_EndPositionalParameters();
                }

                yield return P_NamedArg(f2);

                if (!f2.found)
                {
                    SetBrokenRuleString("PositionalArgs ::= (Value (',' PositionalArgs)?) | @ NamedArg");
                }

                f.found = f2.found;
                break;

            default:
                SetBrokenRuleString("PositionalArgs ::= @ (Value (',' PositionalArgs)?) | NamedArg");
                break;
        }
    }

    ////////////////////////////////
    // NamedArgs ::= NamedArg ( ',' NamedArg )*
    //
    private IEnumerable<(string Name, object Value)> P_NamedArgs(Found f)
    {
        Found f2 = new Found();

        // NamedArgs ::= @NamedArg ( ',' NamedArg )*
        switch (_tokenizer.Token)
        {
            case MeTokenType.PropertyName:
                yield return P_NamedArg(f2);

                f.found = f2.found;

                // NamedArgs ::= NamedArg @( ',' NamedArg )*
                while (_tokenizer.Token == MeTokenType.Comma)
                {
                    // NamedArgs ::= NamedArg ( @',' NamedArg )*
                    NextToken();

                    // NamedArgs ::= NamedArg ( ',' @NamedArg )*
                    yield return P_NamedArg(f2);
                }

                break;

            default:
                SetBrokenRuleString("NamedArgs ::= @NamedArg ( ',' NamedArg )*");
                break;
        }
    }

    ////////////////////////////////
    //   Value   ::= (STRING | QUOTEDMARKUPEXTENSION | MarkupExtension)
    //
    private object P_Value(Found f)
    {
        Found f2 = new Found();

        // Value   ::= @(STRING | QUOTEDMARKUPEXTENSION | MarkupExtension)
        switch (_tokenizer.Token)
        {
            // Value   ::= (@STRING | QUOTEDMARKUPEXTENSION | MarkupExtension)
            case MeTokenType.String:
                string text = Logic_Text();
                f.found = true;
                NextToken();
                return text;

            // Value   ::= (STRING | @QUOTEDMARKUPEXTENSION | MarkupExtension)
            case MeTokenType.QuotedMarkupExtension:
                var nestedParser = new MarkupExtensionParser(_context);
                var descriptor1 = nestedParser.Parse(_tokenizer.TokenText, LineNumber, LinePosition);
                f.found = true;
                NextToken();
                return descriptor1;

            // Value   ::= (STRING | QUOTEDMARKUPEXTENSION | @MarkupExtension)
            case MeTokenType.Open:
                var descriptor2 = P_MarkupExtension(f2);
                f.found = f2.found;
                return descriptor2;

            default:
                return null;
        }
    }

    ////////////////////////////////
    // NamedArg ::= PROPERTYNAME '=' (STRING | QUOTEDMARKUPEXTENSION | MarkupExtension)
    //
    private (string Name, object Value) P_NamedArg(Found f)
    {
        Found f2 = new Found();

        // NamedArg ::= @PROPERTYNAME '=' (STRING | QUOTEDMARKUPEXTENSION | MarkupExtension)
        if (_tokenizer.Token == MeTokenType.PropertyName)
        {
            Logic_StartMember();
            NextToken();

            // NamedArg ::= PROPERTYNAME @'=' (STRING | QUOTEDMARKUPEXTENSION | MarkupExtension)
            Expect(MeTokenType.EqualSign, "NamedArg ::= PROPERTYNAME @'=' Value");
            NextToken();

            string name = _context.CurrentMember;
            object value = null;

            // NamedArg ::= PROPERTYNAME '=' @(STRING | QUOTEDMARKUPEXTENSION | MarkupExtension)
            switch (_tokenizer.Token)
            {
                // NamedArg ::= PROPERTYNAME '=' (@STRING | QUOTEDMARKUPEXTENSION | MarkupExtension)
                case MeTokenType.String:
                    value = Logic_Text();
                    f.found = true;
                    NextToken();
                    break;

                // NamedArg ::= PROPERTYNAME '=' (STRING | @QUOTEDMARKUPEXTENSION | MarkupExtension)
                case MeTokenType.QuotedMarkupExtension:
                    var nestedParser = new MarkupExtensionParser(_context);
                    value = nestedParser.Parse(_tokenizer.TokenText, LineNumber, LinePosition);
                    f.found = true;
                    NextToken();
                    break;

                // NamedArg ::= PROPERTYNAME '=' (STRING | QUOTEDMARKUPEXTENSION | @MarkupExtension)
                case MeTokenType.Open:
                    value = P_Value(f2);
                    f.found = f2.found;
                    break;

                case MeTokenType.PropertyName:
                    {
                        string error;
                        if (_context.CurrentMember is null)
                        {
                            error = string.Format(Errors.MissingComma1, _tokenizer.TokenText);
                        }
                        else
                        {
                            error = string.Format(Errors.MissingComma2, _context.CurrentMember, _tokenizer.TokenText);
                        }

                        throw new XamlParseException(_tokenizer, error);
                    }

                default:
                    SetBrokenRuleString("NamedArg ::= PROPERTYNAME '=' @(STRING | QUOTEDMARKUPEXTENSION | MarkupExtension)");
                    break;
            }

            Logic_EndMember();

            return (name, value);
        }

        return (null, null);
    }

    // ================================================

    private void NextToken()
    {
        _tokenizer.Read();
    }

    private int LineNumber
    {
        get { return _tokenizer.LineNumber; }
    }

    private int LinePosition
    {
        get { return _tokenizer.LinePosition; }
    }

    // ================================================

    private void Logic_StartElement()
    {
        _context.PushScope();
        _context.CurrentType = _tokenizer.TokenType;
        _context.CurrentBracketModeParseParameters = new BracketModeParseParameters(_context);
    }

    private MarkupExtensionDescriptor Logic_EndObject()
    {
        return _context.PopScope();
    }

    private void Logic_StartMember()
    {
        string member = _tokenizer.TokenProperty;
        _context.CurrentMember = member;
    }

    private void Logic_EndMember()
    {
        _context.CurrentMember = null;
    }

    private void Logic_StartPositionalParameters()
    {
        _context.CurrentMember = null;
    }

    private void Logic_EndPositionalParameters()
    {
        // the Ctor args were pushed onto the Builder (XamlWriter)
        // stack, but were not pushed onto the parser stack, so the
        // ME is still the CurrentType for us.

        _context.CurrentArgCount = 0;
        _context.CurrentMember = null;
    }

    private string Logic_Text()
    {
        return _tokenizer.TokenText;
    }

    private void Logic_SetMember(string name, object value)
    {
        if (name is null)
        {
            _context.CurrentDescriptor.ConstructorArguments.Add(value);
        }
        else
        {
            _context.CurrentDescriptor.Properties.Add((name, value));
        }
    }
}