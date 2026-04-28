
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

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace OpenSilver.Compiler;

public sealed class MarkupExtensionDescriptor
{
    private MarkupExtensionDescriptor(
        string name,
        object contentProperty,
        List<(string Name, object Value)> properties)
    {
        Name = name;
        ContentProperty = contentProperty;
        Properties = properties;
    }

    public static bool TryParse(string value, out MarkupExtensionDescriptor markupExtension)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        if (!IsMarkupExtension(value))
        {
            markupExtension = null;
            return false;
        }

        return new MarkupExtensionParser(value).TryParse(out markupExtension);
    }

    public static MarkupExtensionDescriptor Parse(string value, IXmlLineInfo lineInfo)
    {
        if (!TryParse(value, out MarkupExtensionDescriptor descriptor))
        {
            throw new XamlParseException($"The markup extension '{value}' is invalid or malformed.", lineInfo);
        }

        return descriptor;
    }

    public static MarkupExtensionDescriptor Parse(string value) => Parse(value, null);

    public static bool IsMarkupExtension(string value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        if (value.Length == 0 || value[0] != '{')
        {
            return false;
        }

        // {} escape sequence
        if (value.Length >= 2 && value[1] == '}')
        {
            return false;
        }

        return true;
    }

    public string Name { get; }

    public object ContentProperty { get; }

    public List<(string Name, object Value)> Properties { get; }

    private struct MarkupExtensionParser
    {
        private readonly string _input;
        private int _pos;
        private StringBuilder _sb;

        public MarkupExtensionParser(string input)
        {
            _input = input;
            _pos = 0;
        }

        public bool TryParse(out MarkupExtensionDescriptor descriptor)
        {
            if (!TryParseCore(out descriptor))
            {
                return false;
            }

            if (_pos != _input.Length)
            {
                descriptor = null;
                return false;
            }

            return true;
        }

        private bool TryParseCore(out MarkupExtensionDescriptor descriptor)
        {
            descriptor = null;

            if (!Expect('{'))
            {
                return false;
            }

            SkipWhitespace();

            if (!TryReadName(out string name))
            {
                return false;
            }

            SkipWhitespace();

            object contentProperty = null;
            List<(string Name, object Value)> properties = [];
            bool hasContentProperty = false;

            while (_pos < _input.Length && _input[_pos] != '}')
            {
                if (IsPropertyAssignment())
                {
                    if (!TryReadPropertyName(out string propName))
                    {
                        return false;
                    }

                    SkipWhitespace();

                    if (!Expect('='))
                    {
                        return false;
                    }

                    SkipWhitespace();

                    if (!TryReadValue(out object propValue))
                    {
                        return false;
                    }

                    properties.Add((propName, propValue));
                }
                else
                {
                    if (hasContentProperty)
                    {
                        return false;
                    }

                    hasContentProperty = true;

                    if (!TryReadValue(out contentProperty))
                    {
                        return false;
                    }
                }

                SkipWhitespace();

                if (_pos < _input.Length && _input[_pos] == ',')
                {
                    _pos++;
                    SkipWhitespace();
                }
            }

            if (!Expect('}'))
            {
                return false;
            }

            descriptor = new MarkupExtensionDescriptor(name, contentProperty, properties);
            return true;
        }

        private bool IsPropertyAssignment()
        {
            int saved = _pos;
            int depth = 0;
            bool inQuote = false;

            while (_pos < _input.Length)
            {
                char c = _input[_pos];

                if (c == '\\' && _pos + 1 < _input.Length)
                {
                    _pos += 2;
                    continue;
                }

                if (inQuote)
                {
                    if (c == '\'')
                    {
                        inQuote = false;
                    }
                }
                else
                {
                    switch (c)
                    {
                        case '\'':
                            inQuote = true;
                            break;
                        case '{':
                            depth++;
                            break;
                        case '}' when depth == 0:
                            _pos = saved;
                            return false;
                        case '}':
                            depth--;
                            break;
                        case ',' when depth == 0:
                            _pos = saved;
                            return false;
                        case '=' when depth == 0:
                            _pos = saved;
                            return true;
                    }
                }

                _pos++;
            }

            _pos = saved;
            return false;
        }

        private bool TryReadName(out string name)
        {
            name = ReadToken(trimEnd: false, stopOnWhitespace: true, separators: "}");

            if (name.Length == 0)
            {
                name = null;
                return false;
            }

            return true;
        }

        private bool TryReadPropertyName(out string propertyName)
        {
            if (_pos < _input.Length && _input[_pos] == '\'')
            {
                _pos++; // skip opening quote
                propertyName = ReadQuotedString();
                return propertyName != null;
            }

            propertyName = ReadToken(trimEnd: false, stopOnWhitespace: true, separators: "=,}");

            if (propertyName.Length == 0)
            {
                propertyName = null;
                return false;
            }

            return true;
        }

        private bool TryReadValue(out object value)
        {
            if (_pos >= _input.Length)
            {
                value = null;
                return false;
            }

            if (_input[_pos] == '{')
            {
                // {} escape: treat everything after {} as a literal string
                if (_pos + 1 < _input.Length && _input[_pos + 1] == '}')
                {
                    _pos += 2;
                    value = ReadPlainValue();
                    return true;
                }

                // Nested markup extension
                if (TryParseCore(out MarkupExtensionDescriptor nested))
                {
                    value = nested;
                    return true;
                }

                value = null;
                return false;
            }

            if (_input[_pos] == '\'')
            {
                return TryReadQuotedValue(out value);
            }

            value = ReadPlainValue();
            return true;
        }

        private bool TryReadQuotedValue(out object value)
        {
            _pos++; // skip opening quote

            if (_pos < _input.Length && _input[_pos] == '{')
            {
                // {} escape inside quotes
                if (_pos + 1 < _input.Length && _input[_pos + 1] == '}')
                {
                    _pos += 2;
                    value = ReadQuotedString();
                    return value != null;
                }

                // Nested markup extension inside quotes
                if (TryParseCore(out MarkupExtensionDescriptor nested))
                {
                    if (_pos >= _input.Length || _input[_pos] != '\'')
                    {
                        value = null;
                        return false;
                    }

                    _pos++; // skip closing quote
                    value = nested;
                    return true;
                }

                value = null;
                return false;
            }

            value = ReadQuotedString();
            return value != null;
        }

        private string ReadQuotedString()
        {
            string result = ReadToken(trimEnd: false, stopOnWhitespace: false, separators: "'");

            if (_pos >= _input.Length)
            {
                return null;
            }

            _pos++; // skip closing quote
            return result;
        }

        private string ReadPlainValue() => ReadToken(trimEnd: true, stopOnWhitespace: false, separators: ",}");

        private string ReadToken(
            bool trimEnd,
            bool stopOnWhitespace,
            ReadOnlySpan<char> separators)
        {
            int start = _pos;

            StringBuilder sb = null;

            while (_pos < _input.Length)
            {
                char c = _input[_pos];

                if (c == '\\' && _pos + 1 < _input.Length)
                {
                    sb ??= PrepareStringBuilder(start);
                    _pos++;
                    sb.Append(_input[_pos]);
                    _pos++;
                    continue;
                }

                if ((stopOnWhitespace && char.IsWhiteSpace(c)) || separators.IndexOf(c) != -1)
                {
                    break;
                }

                sb?.Append(c);
                _pos++;
            }

            if (sb != null)
            {
                if (trimEnd)
                {
                    while (sb.Length > 0 && char.IsWhiteSpace(sb[sb.Length - 1]))
                    {
                        sb.Length--;
                    }
                }

                string value = sb.ToString();
                sb.Clear();
                return value;
            }

            string result = _input.Substring(start, _pos - start);
            return trimEnd ? result.TrimEnd() : result;
        }

        private StringBuilder PrepareStringBuilder(int start)
        {
            _sb ??= new StringBuilder();
            _sb.Append(_input, start, _pos - start);
            return _sb;
        }

        private void SkipWhitespace()
        {
            while (_pos < _input.Length && char.IsWhiteSpace(_input[_pos]))
            {
                _pos++;
            }
        }

        private bool Expect(char expected)
        {
            if (_pos >= _input.Length || _input[_pos] != expected)
            {
                return false;
            }

            _pos++;
            return true;
        }
    }
}