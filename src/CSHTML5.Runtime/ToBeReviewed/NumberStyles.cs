
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
//
//===============================================================================


#region Assembly mscorlib.dll, v4.0.0.0
// C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\mscorlib.dll
#endregion

using System;
using System.Runtime.InteropServices;

namespace System.Globalization
{
    // Summary:
    //     Determines the styles permitted in numeric string arguments that are passed
    //     to the Parse and TryParse methods of the integral and floating-point numeric
    //     types.
    [Serializable]
    [Flags]
    public enum NumberStyles
    {
        // Summary:
        //     Indicates that no style elements, such as leading or trailing white space,
        //     thousands separators, or a decimal separator, can be present in the parsed
        //     string. The string to be parsed must consist of integral decimal digits only.
        None = 0,
        //
        // Summary:
        //     Indicates that leading white-space characters can be present in the parsed
        //     string. Valid white-space characters have the Unicode values U+0009, U+000A,
        //     U+000B, U+000C, U+000D, and U+0020. Note that this is a subset of the characters
        //     for which the System.Char.IsWhiteSpace(System.Char) method returns true.
        AllowLeadingWhite = 1,
        //
        // Summary:
        //     Indicates that trailing white-space characters can be present in the parsed
        //     string. Valid white-space characters have the Unicode values U+0009, U+000A,
        //     U+000B, U+000C, U+000D, and U+0020. Note that this is a subset of the characters
        //     for which the System.Char.IsWhiteSpace(System.Char) method returns true.
        AllowTrailingWhite = 2,
        //
        // Summary:
        //     Indicates that the numeric string can have a leading sign. Valid leading
        //     sign characters are determined by the System.Globalization.NumberFormatInfo.PositiveSign
        //     and System.Globalization.NumberFormatInfo.NegativeSign properties.
        AllowLeadingSign = 4,
        //
        // Summary:
        //     Indicates that the System.Globalization.NumberStyles.AllowLeadingWhite, System.Globalization.NumberStyles.AllowTrailingWhite,
        //     and System.Globalization.NumberStyles.AllowLeadingSign styles are used. This
        //     is a composite number style.
        Integer = 7,
        //
        // Summary:
        //     Indicates that the numeric string can have a trailing sign. Valid trailing
        //     sign characters are determined by the System.Globalization.NumberFormatInfo.PositiveSign
        //     and System.Globalization.NumberFormatInfo.NegativeSign properties.
        AllowTrailingSign = 8,
        //
        // Summary:
        //     Indicates that the numeric string can have one pair of parentheses enclosing
        //     the number. The parentheses indicate that the string to be parsed represents
        //     a negative number.
        AllowParentheses = 16,
        //
        // Summary:
        //     Indicates that the numeric string can have a decimal point. If the System.Globalization.NumberStyles
        //     value includes the System.Globalization.NumberStyles.AllowCurrencySymbol
        //     flag and the parsed string includes a currency symbol, the decimal separator
        //     character is determined by the System.Globalization.NumberFormatInfo.CurrencyDecimalSeparator
        //     property. Otherwise, the decimal separator character is determined by the
        //     System.Globalization.NumberFormatInfo.NumberDecimalSeparator property.
        AllowDecimalPoint = 32,
        //
        // Summary:
        //     Indicates that the numeric string can have group separators, such as symbols
        //     that separate hundreds from thousands. If the System.Globalization.NumberStyles
        //     value includes the System.Globalization.NumberStyles.AllowCurrencySymbol
        //     flag and the string to be parsed includes a currency symbol, the valid group
        //     separator character is determined by the System.Globalization.NumberFormatInfo.CurrencyGroupSeparator
        //     property, and the number of digits in each group is determined by the System.Globalization.NumberFormatInfo.CurrencyGroupSizes
        //     property. Otherwise, the valid group separator character is determined by
        //     the System.Globalization.NumberFormatInfo.NumberGroupSeparator property,
        //     and the number of digits in each group is determined by the System.Globalization.NumberFormatInfo.NumberGroupSizes
        //     property.
        AllowThousands = 64,
        //
        // Summary:
        //     Indicates that the System.Globalization.NumberStyles.AllowLeadingWhite, System.Globalization.NumberStyles.AllowTrailingWhite,
        //     System.Globalization.NumberStyles.AllowLeadingSign, System.Globalization.NumberStyles.AllowTrailingSign,
        //     System.Globalization.NumberStyles.AllowDecimalPoint, and System.Globalization.NumberStyles.AllowThousands
        //     styles are used. This is a composite number style.
        Number = 111,
        //
        // Summary:
        //     Indicates that the numeric string can be in exponential notation. The System.Globalization.NumberStyles.AllowExponent
        //     flag allows the parsed string to contain an exponent that begins with the
        //     "E" or "e" character and that is followed by an optional positive or negative
        //     sign and an integer. In other words, it successfully parses strings in the
        //     form nnnExx, nnnE+xx, and nnnE-xx. It does not allow a decimal separator
        //     or sign in the significand or mantissa; to allow these elements in the string
        //     to be parsed, use the System.Globalization.NumberStyles.AllowDecimalPoint
        //     and System.Globalization.NumberStyles.AllowLeadingSign flags, or use a composite
        //     style that includes these individual flags.
        AllowExponent = 128,
        //
        // Summary:
        //     Indicates that the System.Globalization.NumberStyles.AllowLeadingWhite, System.Globalization.NumberStyles.AllowTrailingWhite,
        //     System.Globalization.NumberStyles.AllowLeadingSign, System.Globalization.NumberStyles.AllowDecimalPoint,
        //     and System.Globalization.NumberStyles.AllowExponent styles are used. This
        //     is a composite number style.
        Float = 167,
        //
        // Summary:
        //     Indicates that the numeric string can contain a currency symbol. Valid currency
        //     symbols are determined by the System.Globalization.NumberFormatInfo.CurrencySymbol
        //     property.
        AllowCurrencySymbol = 256,
        //
        // Summary:
        //     Indicates that all styles except System.Globalization.NumberStyles.AllowExponent
        //     and System.Globalization.NumberStyles.AllowHexSpecifier are used. This is
        //     a composite number style.
        Currency = 383,
        //
        // Summary:
        //     Indicates that all styles except System.Globalization.NumberStyles.AllowHexSpecifier
        //     are used. This is a composite number style.
        Any = 511,
        //
        // Summary:
        //     Indicates that the numeric string represents a hexadecimal value. Valid hexadecimal
        //     values include the numeric digits 0-9 and the hexadecimal digits A-F and
        //     a-f. Strings that are parsed using this style cannot be prefixed with "0x"
        //     or "&h". A string that is parsed with the System.Globalization.NumberStyles.AllowHexSpecifier
        //     style will always be interpreted as a hexadecimal value. The only flags that
        //     can be combined with System.Globalization.NumberStyles.AllowHexSpecifier
        //     are System.Globalization.NumberStyles.AllowLeadingWhite and System.Globalization.NumberStyles.AllowTrailingWhite.
        //     The System.Globalization.NumberStyles enumeration includes a composite style,
        //     System.Globalization.NumberStyles.HexNumber, that consists of these three
        //     flags.
        AllowHexSpecifier = 512,
        //
        // Summary:
        //     Indicates that the System.Globalization.NumberStyles.AllowLeadingWhite, System.Globalization.NumberStyles.AllowTrailingWhite,
        //     and System.Globalization.NumberStyles.AllowHexSpecifier styles are used.
        //     This is a composite number style.
        HexNumber = 515,
    }
}