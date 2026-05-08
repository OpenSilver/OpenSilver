// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// Port of ExactArithmetic.h / ExactArithmetic.cpp

using System;
using System.Diagnostics;

namespace OpenSilver.Internal.Media.Geometry.Core;

/// <summary>
/// Helper class containing the exact arithmetic helper functions ported from the C++ static functions.
/// </summary>
internal static class EaHelper
{
    private const int DIGIT_BITSIZE = 8 * sizeof(uint);

    internal static uint EaNumDigits(ReadOnlySpan<uint> nn, uint nl)
    {
        Debug.Assert(nl > 0);
        int idx = (int)nl;
        while (idx > 0 && nn[idx - 1] == 0)
        {
            idx--;
        }
        return idx > 0 ? (uint)idx : 1;
    }

    private static COMPARISON EaCompareDigits(uint d1, uint d2)
    {
        return d1 > d2 ? COMPARISON.STRICTLYGREATERTHAN : (d1 == d2 ? COMPARISON.EQUAL : COMPARISON.STRICTLYLESSTHAN);
    }

    internal static COMPARISON EaCompare(ReadOnlySpan<uint> mm, int ml, ReadOnlySpan<uint> nn, int nl)
    {
        Debug.Assert(ml > 0 && nl > 0);

        COMPARISON result = COMPARISON.EQUAL;

        ml = (int)EaNumDigits(mm, (uint)ml);
        nl = (int)EaNumDigits(nn, (uint)nl);

        if (ml != nl)
        {
            result = ml > nl ? COMPARISON.STRICTLYGREATERTHAN : COMPARISON.STRICTLYLESSTHAN;
        }
        else
        {
            while (result == COMPARISON.EQUAL && ml > 0)
            {
                ml--;
                result = EaCompareDigits(mm[ml], nn[ml]);
            }
        }
        return result;
    }

    internal static void EaCopyDigitsAndZeroOverflowDigit(Span<uint> dest, ReadOnlySpan<uint> source, int nl)
    {
        Debug.Assert(nl > 0);
        source.Slice(0, nl).CopyTo(dest);
        dest[nl] = 0;
    }

    internal static void EaCopyDigits(Span<uint> dest, ReadOnlySpan<uint> source, int nl)
    {
        Debug.Assert(nl > 0);
        source.Slice(0, nl).CopyTo(dest);
    }

    internal static uint EaAddCarry(Span<uint> nn, int startIndex, int nl, uint uCarryIn)
    {
        Debug.Assert(nl > 0 && uCarryIn < 2);

        if (uCarryIn == 0)
        {
            return 0;
        }

        int i = startIndex;
        int count = nl;
        while (count > 0)
        {
            count--;
            nn[i]++;
            if (nn[i] != 0)
            {
                return 0;
            }
            i++;
        }
        return 1;
    }

    internal static uint EaSubtractBorrow(Span<uint> nn, int startIndex, int nl, uint uBorrowIn)
    {
        Debug.Assert(nl > 0 && uBorrowIn < 2);

        if (uBorrowIn == 1)
        {
            return 1;
        }

        int i = startIndex;
        int count = nl;
        while (count > 0)
        {
            count--;
            uint old = nn[i];
            nn[i]--;
            if (old != 0)
            {
                return 1;
            }
            i++;
        }
        return 0;
    }

    internal static uint EaAdd(Span<uint> mm, int mmStart, int ml, ReadOnlySpan<uint> nn, int nnStart, int nl, uint uCarryIn)
    {
        Debug.Assert(ml > 0 && nl > 0 && uCarryIn < 2 && ml >= nl);

        ulong c = uCarryIn;
        int mi = mmStart;
        int ni = nnStart;

        int remaining = ml - nl;
        int count = nl;
        while (count > 0)
        {
            c += (ulong)mm[mi] + nn[ni];
            mm[mi] = (uint)c;
            c >>= DIGIT_BITSIZE;
            mi++;
            ni++;
            count--;
        }

        if (remaining == 0)
        {
            return (uint)c;
        }
        else
        {
            return EaAddCarry(mm, mi, remaining, (uint)c);
        }
    }

    internal static uint EaSubtract(Span<uint> mm, int mmStart, int ml, ReadOnlySpan<uint> nn, int nnStart, int nl, uint uBorrowIn)
    {
        Debug.Assert(ml > 0 && nl > 0 && uBorrowIn < 2 && ml >= nl);

        ulong c = uBorrowIn;
        int mi = mmStart;
        int ni = nnStart;

        int remaining = ml - nl;
        int count = nl;
        while (count > 0)
        {
            uint invn = nn[ni] ^ 0xFFFFFFFF;
            c += (ulong)mm[mi] + invn;
            mm[mi] = (uint)c;
            c >>= DIGIT_BITSIZE;
            mi++;
            ni++;
            count--;
        }

        if (remaining == 0)
        {
            return (uint)c;
        }
        else
        {
            return EaSubtractBorrow(mm, mi, remaining, (uint)c);
        }
    }

    internal static uint EaMultiplyDigit(Span<uint> pp, int ppStart, int pl, ReadOnlySpan<uint> mm, int mmStart, int ml, uint d)
    {
        Debug.Assert(pl > ml && ml > 0);

        if (d == 0)
        {
            return 0;
        }
        else if (d == 1)
        {
            return EaAdd(pp, ppStart, pl, mm, mmStart, ml, 0);
        }
        else
        {
            ulong c = 0;
            int pi = ppStart;
            int mi = mmStart;

            int plRemaining = pl - ml;
            int mlCount = ml;
            while (mlCount > 0)
            {
                c += pp[pi] + ((ulong)d * mm[mi]);
                pp[pi] = (uint)c;
                c >>= DIGIT_BITSIZE;
                pi++;
                mi++;
                mlCount--;
            }
            while (plRemaining > 0)
            {
                c += pp[pi];
                pp[pi] = (uint)c;
                c >>= DIGIT_BITSIZE;
                pi++;
                plRemaining--;
            }
            return (uint)c;
        }
    }

    internal static uint EaMultiply(Span<uint> pp, int ppStart, int pl, ReadOnlySpan<uint> mm, int mmStart, int ml, ReadOnlySpan<uint> nn, int nnStart, int nl)
    {
        Debug.Assert(pl > 0 && ml > 0 && nl > 0);
        Debug.Assert(pl >= ml + nl);

        uint c = 0;
        int pi = ppStart;
        int ni = nnStart;
        int plCurrent = pl;

        for (int i = 0; i < nl; i++)
        {
            c += EaMultiplyDigit(pp, pi, plCurrent, mm, mmStart, ml, nn[ni]);
            pi++;
            ni++;
            plCurrent--;
        }
        return c;
    }
}

/// <summary>
/// A signed integer in the range [-2^64 + 1, 2^64 - 1].
/// </summary>
internal struct CZ64
{
    public const int Size = 3;

    private uint _digit0;
    private uint _digit1;
    private uint _digit2;
    private SIGNINDICATOR _sign;

    public CZ64(double value)
    {
        Debug.Assert(IntegerConstants.IsValidInteger31(value));

        _sign = SIGNINDICATOR.ZERO;

        if (value > 0)
        {
            SetSign(SIGNINDICATOR.STRICTLY_POSITIVE);
            _digit0 = (uint)value;
        }
        else if (value < 0)
        {
            SetSign(SIGNINDICATOR.STRICTLY_NEGATIVE);
            _digit0 = (uint)(-value);
        }
        else
        {
            SetSign(SIGNINDICATOR.ZERO);
        }
    }

    public readonly COMPARISON Compare(CZ64 other)
    {
        SIGNINDICATOR eOtherSI = other.GetSign();
        SIGNINDICATOR eThisSI = GetSign();
        COMPARISON c = COMPARISON.EQUAL;

        if ((int)eThisSI > (int)eOtherSI)
        {
            c = COMPARISON.STRICTLYGREATERTHAN;
        }
        else if ((int)eThisSI < (int)eOtherSI)
        {
            c = COMPARISON.STRICTLYLESSTHAN;
        }
        else if ((int)eThisSI > 0)
        {
            c = EaHelper.EaCompare(GetDigits(stackalloc uint[Size]), Size, other.GetDigits(stackalloc uint[Size]), Size);
        }
        else if ((int)eThisSI < 0)
        {
            c = EaHelper.EaCompare(other.GetDigits(stackalloc uint[Size]), Size, GetDigits(stackalloc uint[Size]), Size);
        }
        return c;
    }

    public void Multiply(CZ64 other)
    {
        Span<uint> digits = GetDigits(stackalloc uint[Size]);
        Span<uint> otherDigits = other.GetDigits(stackalloc uint[Size]);

        Debug.Assert(GetDigitCount(digits) == 1 && GetDigitCount(otherDigits) == 1);

        Span<uint> uNewDigits = stackalloc uint[2];

        EaHelper.EaMultiply(uNewDigits, 0, 2, digits, 0, 1, otherDigits, 0, 1);
        ReplaceDigitsButKeepSign(digits, uNewDigits);
        SetSign((SIGNINDICATOR)((int)GetSign() * (int)other.GetSign()));

        SetDigits(digits);
    }

    public readonly SIGNINDICATOR GetSign() => _sign;

    public void Negate() => SetSign(GetOppositeSign());

    public void SetSign(SIGNINDICATOR sign) => _sign = sign;

    public readonly SIGNINDICATOR GetOppositeSign() => (SIGNINDICATOR)(-(int)_sign);

    private readonly Span<uint> GetDigits(Span<uint> digits)
    {
        Debug.Assert(digits.Length >= Size);
        digits[0] = _digit0;
        digits[1] = _digit1;
        digits[2] = _digit2;
        return digits;
    }

    private void SetDigits(ReadOnlySpan<uint> digits)
    {
        Debug.Assert(digits.Length >= Size);
        _digit0 = digits[0];
        _digit1 = digits[1];
        _digit2 = digits[2];
    }

    private static uint GetDigitCount(ReadOnlySpan<uint> digits) => EaHelper.EaNumDigits(digits, Size);

    private static void ReplaceDigitsButKeepSign(Span<uint> digits, ReadOnlySpan<uint> source)
    {
        Debug.Assert(source.Length > 0 && source.Length <= Size && digits.Length >= Size);
        source.CopyTo(digits);
    }
}

/// <summary>
/// A signed integer in the range [-2^128 + 1, 2^128 - 1].
/// </summary>
internal struct CZ128
{
    public const int Size = 5;

    private uint _digit0;
    private uint _digit1;
    private uint _digit2;
    private uint _digit3;
    private uint _digit4;
    private SIGNINDICATOR _sign;

    public CZ128(double value)
    {
        Debug.Assert(IntegerConstants.IsValidInteger53(value));

        _sign = SIGNINDICATOR.ZERO;

        ulong tab;

        if (value > 0.0)
        {
            tab = (ulong)value;
            SetSign(SIGNINDICATOR.STRICTLY_POSITIVE);
        }
        else if (value < 0.0)
        {
            tab = (ulong)(-value);
            SetSign(SIGNINDICATOR.STRICTLY_NEGATIVE);
        }
        else
        {
            tab = 0;
            SetSign(SIGNINDICATOR.ZERO);
        }

        _digit0 = (uint)tab;
        _digit1 = (uint)(tab >> 32);
    }

    public readonly COMPARISON Compare(CZ128 other)
    {
        SIGNINDICATOR eOtherSI = other.GetSign();
        SIGNINDICATOR eThisSI = GetSign();
        COMPARISON c = COMPARISON.EQUAL;

        if ((int)eThisSI > (int)eOtherSI)
        {
            c = COMPARISON.STRICTLYGREATERTHAN;
        }
        else if ((int)eThisSI < (int)eOtherSI)
        {
            c = COMPARISON.STRICTLYLESSTHAN;
        }
        else if ((int)eThisSI > 0)
        {
            c = EaHelper.EaCompare(GetDigits(stackalloc uint[Size]), Size, other.GetDigits(stackalloc uint[Size]), Size);
        }
        else if ((int)eThisSI < 0)
        {
            c = EaHelper.EaCompare(other.GetDigits(stackalloc uint[Size]), Size, GetDigits(stackalloc uint[Size]), Size);
        }
        return c;
    }

    public void Multiply(CZ128 other)
    {
        Span<uint> digits = GetDigits(stackalloc uint[Size]);
        Span<uint> otherDigits = other.GetDigits(stackalloc uint[Size]);

        uint yl = GetDigitCount(digits);
        uint zl = GetDigitCount(otherDigits);

        Debug.Assert(yl > 0 && yl < Size - 2 && zl > 0 && zl < Size - 2);

        Span<uint> uNewDigits = stackalloc uint[5 - 1];
        EaHelper.EaMultiply(uNewDigits, 0, (int)(yl + zl), digits, 0, (int)yl, otherDigits, 0, (int)zl);
        ReplaceDigitsButKeepSign(digits, uNewDigits.Slice(0, (int)(yl + zl)));
        SetSign((SIGNINDICATOR)((int)GetSign() * (int)other.GetSign()));

        SetDigits(digits);
    }

    public readonly SIGNINDICATOR GetSign() => _sign;

    public void Negate() => SetSign(GetOppositeSign());

    public void SetSign(SIGNINDICATOR sign) => _sign = sign;

    public readonly SIGNINDICATOR GetOppositeSign() => (SIGNINDICATOR)(-(int)_sign);

    private readonly Span<uint> GetDigits(Span<uint> digits)
    {
        Debug.Assert(digits.Length >= Size);
        digits[0] = _digit0;
        digits[1] = _digit1;
        digits[2] = _digit2;
        digits[3] = _digit3;
        digits[4] = _digit4;
        return digits;
    }

    private void SetDigits(ReadOnlySpan<uint> digits)
    {
        Debug.Assert(digits.Length >= Size);
        _digit0 = digits[0];
        _digit1 = digits[1];
        _digit2 = digits[2];
        _digit3 = digits[3];
        _digit4 = digits[4];
    }

    private static uint GetDigitCount(ReadOnlySpan<uint> digits) => EaHelper.EaNumDigits(digits, Size);

    private static void ReplaceDigitsButKeepSign(Span<uint> digits, ReadOnlySpan<uint> source)
    {
        Debug.Assert(source.Length > 0 && source.Length <= Size && digits.Length >= Size);
        source.CopyTo(digits);
    }
}

/// <summary>
/// A signed integer in the range [-2^192 + 1, 2^192 - 1].
/// </summary>
internal struct CZ192
{
    public const int Size = 7;

    private uint _digit0;
    private uint _digit1;
    private uint _digit2;
    private uint _digit3;
    private uint _digit4;
    private uint _digit5;
    private uint _digit6;
    private SIGNINDICATOR _sign;

    public CZ192(double value)
    {
        Debug.Assert(IntegerConstants.IsValidInteger33(value));

        _sign = SIGNINDICATOR.ZERO;

        ulong tab;

        if (value > 0.0)
        {
            tab = (ulong)value;
            SetSign(SIGNINDICATOR.STRICTLY_POSITIVE);
        }
        else if (value < 0.0)
        {
            tab = (ulong)(-value);
            SetSign(SIGNINDICATOR.STRICTLY_NEGATIVE);
        }
        else
        {
            tab = 0;
            SetSign(SIGNINDICATOR.ZERO);
        }

        _digit0 = (uint)tab;
        _digit1 = (uint)(tab >> 32);
    }

    public readonly COMPARISON Compare(CZ192 other)
    {
        SIGNINDICATOR eOtherSI = other.GetSign();
        SIGNINDICATOR eThisSI = GetSign();
        COMPARISON c = COMPARISON.EQUAL;

        if ((int)eThisSI > (int)eOtherSI)
        {
            c = COMPARISON.STRICTLYGREATERTHAN;
        }
        else if ((int)eThisSI < (int)eOtherSI)
        {
            c = COMPARISON.STRICTLYLESSTHAN;
        }
        else if ((int)eThisSI > 0)
        {
            c = EaHelper.EaCompare(GetDigits(stackalloc uint[Size]), Size, other.GetDigits(stackalloc uint[Size]), Size);
        }
        else if ((int)eThisSI < 0)
        {
            c = EaHelper.EaCompare(other.GetDigits(stackalloc uint[Size]), Size, GetDigits(stackalloc uint[Size]), Size);
        }
        return c;
    }

    public void Add(CZ192 other)
    {
        Span<uint> digits = GetDigits(stackalloc uint[Size]);
        Span<uint> otherDigits = other.GetDigits(stackalloc uint[Size]);

        uint yl = GetDigitCount(digits);
        uint zl = GetDigitCount(otherDigits);
        Debug.Assert(yl < Size && zl < Size);

        Span<uint> uNewDigits = stackalloc uint[7];

        if (GetSign() == other.GetSign())
        {
            switch (EaHelper.EaCompare(digits, (int)yl, otherDigits, (int)zl))
            {
                case COMPARISON.EQUAL:
                case COMPARISON.STRICTLYGREATERTHAN:
                    EaHelper.EaCopyDigitsAndZeroOverflowDigit(uNewDigits, digits, (int)yl);
                    EaHelper.EaAdd(uNewDigits, 0, (int)yl + 1, otherDigits, 0, (int)zl, 0);
                    ReplaceDigitsButKeepSign(digits, uNewDigits.Slice(0, (int)(yl + 1)));
                    break;

                case COMPARISON.STRICTLYLESSTHAN:
                    EaHelper.EaCopyDigitsAndZeroOverflowDigit(uNewDigits, otherDigits, (int)zl);
                    EaHelper.EaAdd(uNewDigits, 0, (int)zl + 1, digits, 0, (int)yl, 0);
                    ReplaceDigitsButKeepSign(digits, uNewDigits.Slice(0, (int)(zl + 1)));
                    break;

                default:
                    Debug.Assert(false);
                    break;
            }
        }
        else
        {
            switch (EaHelper.EaCompare(digits, (int)yl, otherDigits, (int)zl))
            {
                case COMPARISON.EQUAL:
                    SetSign(SIGNINDICATOR.ZERO);
                    SetToZero(digits);
                    break;

                case COMPARISON.STRICTLYGREATERTHAN:
                    EaHelper.EaSubtract(digits, 0, (int)yl, otherDigits, 0, (int)zl, 1);
                    break;

                case COMPARISON.STRICTLYLESSTHAN:
                    EaHelper.EaCopyDigits(uNewDigits, otherDigits, (int)zl);
                    EaHelper.EaSubtract(uNewDigits, 0, (int)zl, digits, 0, (int)yl, 1);
                    ReplaceDigitsButKeepSign(digits, uNewDigits.Slice(0, (int)zl));
                    SetSign(other.GetSign());
                    break;

                default:
                    Debug.Assert(false);
                    break;
            }
        }

        SetDigits(digits);
    }

    public void Subtract(CZ192 other)
    {
        other.Negate();
        Add(other);
    }

    public void Multiply(CZ192 other)
    {
        Span<uint> digits = GetDigits(stackalloc uint[Size]);
        Span<uint> otherDigits = other.GetDigits(stackalloc uint[Size]);

        uint yl = GetDigitCount(digits);
        uint zl = GetDigitCount(otherDigits);
        Debug.Assert(yl > 0 && zl > 0 && (yl + zl < Size));

        Span<uint> uNewDigits = stackalloc uint[7 - 1];

        EaHelper.EaMultiply(uNewDigits, 0, (int)(yl + zl), digits, 0, (int)yl, otherDigits, 0, (int)zl);
        ReplaceDigitsButKeepSign(digits, uNewDigits.Slice(0, (int)(yl + zl)));
        SetSign((SIGNINDICATOR)((int)GetSign() * (int)other.GetSign()));

        SetDigits(digits);
    }

    public readonly SIGNINDICATOR GetSign() => _sign;

    public void Negate() => SetSign(GetOppositeSign());

    public void SetSign(SIGNINDICATOR sign) => _sign = sign;

    public readonly SIGNINDICATOR GetOppositeSign() => (SIGNINDICATOR)(-(int)_sign);

    private readonly Span<uint> GetDigits(Span<uint> digits)
    {
        Debug.Assert(digits.Length >= Size);
        digits[0] = _digit0;
        digits[1] = _digit1;
        digits[2] = _digit2;
        digits[3] = _digit3;
        digits[4] = _digit4;
        digits[5] = _digit5;
        digits[6] = _digit6;
        return digits;
    }

    private void SetDigits(ReadOnlySpan<uint> digits)
    {
        Debug.Assert(digits.Length >= Size);
        _digit0 = digits[0];
        _digit1 = digits[1];
        _digit2 = digits[2];
        _digit3 = digits[3];
        _digit4 = digits[4];
        _digit5 = digits[5];
        _digit6 = digits[6];
    }

    private static void SetToZero(Span<uint> digits) => digits.Clear();

    private static uint GetDigitCount(ReadOnlySpan<uint> digits) => EaHelper.EaNumDigits(digits, Size);

    private static void ReplaceDigitsButKeepSign(Span<uint> digits, ReadOnlySpan<uint> source)
    {
        Debug.Assert(source.Length > 0 && source.Length <= Size && digits.Length >= Size);
        source.CopyTo(digits);
    }
}
