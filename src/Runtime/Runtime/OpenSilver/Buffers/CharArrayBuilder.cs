
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
using System.Buffers;
using System.Diagnostics;

namespace OpenSilver.Buffers;

internal sealed class CharArrayBuilder
{
    private const int DefaultCapacity = 2 * 1024 * 1024;
    private const int MaxArrayLength = 0X7FFFFFC7;
    private static readonly string NewLine = Features.Interop.UseNewLineSeparator ? ";\n" : ";";

    private char[] _buffer;
    private int _length;

    public CharArrayBuilder(int initialCapacity = DefaultCapacity)
    {
        _buffer = ArrayPool<char>.Shared.Rent(initialCapacity);
        _length = 0;
    }

    public char[] Buffer => _buffer;

    public int Length => _length;

    public int Capacity => _buffer.Length;

    public void AppendLine() => Append(NewLine);

    public void AppendLine(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            AppendLine();
            return;
        }

        int newLength = _length + value.Length + NewLine.Length;
        if (newLength > Capacity)
        {
            EnsureCapacity(newLength);
        }

        value.AsSpan().CopyTo(_buffer.AsSpan(_length));
        NewLine.AsSpan().CopyTo(_buffer.AsSpan(_length + value.Length));

        _length = newLength;
    }

    public void Append(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return;
        }

        int newLength = _length + value.Length;
        if (newLength > Capacity)
        {
            EnsureCapacity(newLength);
        }

        value.AsSpan().CopyTo(_buffer.AsSpan(_length));

        _length = newLength;
    }

    public void Reset() => _length = 0;

    public string ToStringAndClear()
    {
        (int length, _length) = (_length, 0);
        return _buffer.AsSpan(0, length).ToString();
    }

    private void EnsureCapacity(int minimum)
    {
        Debug.Assert(minimum > Capacity);

        int capacity = Capacity;
        int nextCapacity = capacity == 0 ? DefaultCapacity : 2 * capacity;

        if ((uint)nextCapacity > (uint)MaxArrayLength)
        {
            nextCapacity = Math.Max(capacity + 1, MaxArrayLength);
        }

        nextCapacity = Math.Max(nextCapacity, minimum);

        char[] newBuffer = ArrayPool<char>.Shared.Rent(nextCapacity);
        Array.Copy(_buffer, newBuffer, _length);

        ArrayPool<char>.Shared.Return(_buffer);
        _buffer = newBuffer;
    }
}
