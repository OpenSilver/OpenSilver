
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
using System.Collections.Generic;
using System.Linq;

namespace OpenSilver.Internal.Controls;

internal ref struct QuillContentParser
{
    private readonly Span<QuillDelta> _deltas;
    private readonly List<QuillDelta> _inlines;

    private int _pos = -1;
    private int _nextParagraphPosition;
    private bool _endReached;

    public QuillContentParser(Span<QuillDelta> deltas)
    {
        _deltas = deltas;
        _inlines = new();
    }

    public QuillRangeFormat BlockFormat { get; private set; }

    public IEnumerable<QuillDelta> Inlines { get; private set; }

    public bool MoveToNextBlock()
    {
        if (_endReached)
        {
            return false;
        }

        if (_nextParagraphPosition > 0)
        {
            QuillDelta delta = _deltas[_pos];
            BlockFormat = delta.Attributes.Value;
            Inlines = _inlines.Skip(_nextParagraphPosition);
            string text = delta.Text.EndsWith("\n") ? delta.Text.Substring(0, delta.Text.Length - 1) : delta.Text;
            if (!string.IsNullOrEmpty(text))
            {
                Inlines = Inlines.Append(new QuillDelta { Text = text });
            }
            _nextParagraphPosition = 0;
            return true;
        }

        _inlines.Clear();
        _pos++;

        while (_pos < _deltas.Length)
        {
            QuillDelta delta = _deltas[_pos];

            if (!IsEndOfParagraph(delta))
            {
                _pos++;
                _inlines.Add(delta);
                continue;
            }

            _nextParagraphPosition = FindStartOfParagraph();

            if (_nextParagraphPosition > 0)
            {
                BlockFormat = default;
                Inlines = _inlines.Take(_nextParagraphPosition);
            }
            else
            {
                BlockFormat = delta.Attributes.Value;
                Inlines = _inlines;
                string text = delta.Text.EndsWith("\n") ? delta.Text.Substring(0, delta.Text.Length - 1) : delta.Text;
                if (!string.IsNullOrEmpty(text))
                {
                    Inlines = Inlines.Append(new QuillDelta { Text = text });
                }
            }

            return true;
        }

        _endReached = true;

        if (_inlines.Count > 0)
        {
            BlockFormat = default;
            Inlines = _inlines;
            return true;
        }

        return false;
    }

    private static bool IsEndOfParagraph(QuillDelta delta)
    {
        return delta.Attributes is QuillRangeFormat format &&
            (!string.IsNullOrEmpty(format.TextAlignment) || !string.IsNullOrEmpty(format.LineHeight));
    }

    private int FindStartOfParagraph()
    {
        for (int i = _inlines.Count - 1; i >= 0; i--)
        {
            QuillDelta delta = _inlines[i];
            int index = delta.Text.LastIndexOf('\n');
            if (index != -1)
            {
                _inlines[i] = new QuillDelta
                {
                    Text = delta.Text.Substring(0, index),
                    Attributes = delta.Attributes,
                };
                _inlines.Insert(i + 1, new QuillDelta
                {
                    Text = delta.Text.Substring(index + 1),
                    Attributes = delta.Attributes,
                });

                return i + 1;
            }
        }

        return 0;
    }
}
