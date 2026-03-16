
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

using System.ComponentModel;
using System.Diagnostics;
using OpenSilver.Internal;

namespace System.Windows;

/// <summary>
/// Describes the degree to which a font has been stretched, compared to the normal
/// aspect ratio of that font.
/// </summary>
[TypeConverter(typeof(FontStretchConverter))]
public readonly struct FontStretch : IFormattable
{
    private readonly int _stretch;

    internal FontStretch(int stretch)
    {
        Debug.Assert(1 <= stretch && stretch <= 9);

        // We want the default zero value of new FontStretch() to correspond to FontStretches.Normal.
        // Therefore, the _stretch value is shifted by 5 relative to the OpenType stretch value.
        _stretch = stretch - 5;
    }

    /// <summary>
    /// Creates a new instance of <see cref="FontStretch"/> that corresponds to the OpenType usStretchClass value.
    /// </summary>
    /// <param name="stretchValue">
    /// An integer value between one and nine that corresponds to the usStretchValue definition in the OpenType 
    /// specification.
    /// </param>
    /// <returns>
    /// A new instance of <see cref="FontStretch"/>.
    /// </returns>
    public static FontStretch FromOpenTypeStretch(int stretchValue)
    {
        if (stretchValue < 1 || stretchValue > 9)
        {
            throw new ArgumentOutOfRangeException(nameof(stretchValue), string.Format(Strings.ParameterMustBeBetween, 1, 9));
        }

        return new FontStretch(stretchValue);
    }

    /// <summary>
    /// Returns a value that represents the OpenType usStretchClass for this <see cref="FontStretch"/> object.
    /// </summary>
    /// <returns>
    /// An integer value between 1 and 999 that corresponds to the usStretchClass definition in the OpenType 
    /// specification.
    /// </returns>
    public int ToOpenTypeStretch()
    {
        Debug.Assert(1 <= RealStretch && RealStretch <= 9);
        return RealStretch;
    }

    /// <summary>
    /// Compares two instances of <see cref="FontStretch"/> objects.
    /// </summary>
    /// <param name="left">
    /// The first <see cref="FontStretch"/> object to compare.
    /// </param>
    /// <param name="right">
    /// The second <see cref="FontStretch"/> object to compare.
    /// </param>
    /// <returns>
    /// An <see cref="int"/> value that represents the relationship between the two instances of <see cref="FontStretch"/>.
    /// </returns>
    public static int Compare(FontStretch left, FontStretch right) => left._stretch - right._stretch;

    /// <summary>
    /// Compares a <see cref="FontStretch"/> object with the current <see cref="FontStretch"/> object.
    /// </summary>
    /// <param name="obj">
    /// The instance of the <see cref="FontStretch"/> object to compare for equality.
    /// </param>
    /// <returns>
    /// true if two instances are equal; otherwise, false.
    /// </returns>
    public bool Equals(FontStretch obj) => this == obj;

    /// <summary>
    /// Compares an object with the current <see cref="FontStretch"/> object.
    /// </summary>
    /// <param name="obj">
    /// The instance of the object to compare for equality.
    /// </param>
    /// <returns>
    /// true if two instances are equal; otherwise, false.
    /// </returns>
    public override bool Equals(object obj) => obj is FontStretch fontStretch && this == fontStretch;

    /// <summary>
    /// Retrieves the hash code for this object.
    /// </summary>
    /// <returns>
    /// An integer hash value.
    /// </returns>
    public override int GetHashCode() => RealStretch;

    /// <summary>
    /// Creates a <see cref="string"/> representation of the current <see cref="FontStretch"/>
    /// object.
    /// </summary>
    /// <returns>
    /// A <see cref="string"/> value representation of the object.
    /// </returns>
    public override string ToString() => ConvertToString(null, null);

    string IFormattable.ToString(string format, IFormatProvider formatProvider)
        => ConvertToString(format, formatProvider);

    /// <summary>
    /// Compares two instances of <see cref="FontStretch"/> for equality.
    /// </summary>
    /// <param name="left">
    /// First instance of <see cref="FontStretch"/> to compare.
    /// </param>
    /// <param name="right">
    /// Second instance of <see cref="FontStretch"/> to compare.
    /// </param>
    /// <returns>
    /// true when the specified <see cref="FontStretch"/> objects are equal; otherwise, false.
    /// </returns>
    public static bool operator ==(FontStretch left, FontStretch right) => left._stretch == right._stretch;

    /// <summary>
    /// Evaluates two instances of <see cref="FontStretch"/> to determine inequality.
    /// </summary>
    /// <param name="left">
    /// The first instance of <see cref="FontStretch"/> to compare.
    /// </param>
    /// <param name="right">
    /// The second instance of <see cref="FontStretch"/> to compare.
    /// </param>
    /// <returns>
    /// false if left is equal to right; otherwise, true.
    /// </returns>
    public static bool operator !=(FontStretch left, FontStretch right) => !(left == right);

    /// <summary>
    /// Evaluates two instances of <see cref="FontStretch"/> to determine whether one instance is 
    /// less than the other.
    /// </summary>
    /// <param name="left">
    /// The first instance of <see cref="FontStretch" /> to compare.
    /// </param>
    /// <param name="right">
    /// The second instance of <see cref="FontStretch" /> to compare.
    /// </param>
    /// <returns>
    /// true if left is less than right; otherwise, false.
    /// </returns>
    public static bool operator <(FontStretch left, FontStretch right) => Compare(left, right) < 0;

    /// <summary>
    /// Evaluates two instances of <see cref="FontStretch"/> to determine if one instance is 
    /// greater than the other.
    /// </summary>
    /// <param name="left">
    /// First instance of <see cref="FontStretch"/> to compare.
    /// </param>
    /// <param name="right">
    /// Second instance of <see cref="FontStretch"/> to compare.
    /// </param>
    /// <returns>
    /// true if left is greater than right; otherwise, false.
    /// </returns>
    public static bool operator >(FontStretch left, FontStretch right) => Compare(left, right) > 0;

    /// <summary>
    /// Evaluates two instances of <see cref="FontStretch"/> to determine whether one instance is 
    /// less than or equal to the other.
    /// </summary>
    /// <param name="left">
    /// The first instance of <see cref="FontStretch"/> to compare.
    /// </param>
    /// <param name="right">
    /// The second instance of <see cref="FontStretch"/> to compare.
    /// </param>
    /// <returns>
    /// true if left is less than or equal to right; otherwise, false.
    /// </returns>
    public static bool operator <=(FontStretch left, FontStretch right) => Compare(left, right) <= 0;

    /// <summary>
    /// Evaluates two instances of <see cref="FontStretch"/> to determine whether one instance is 
    /// greater than or equal to the other.
    /// </summary>
    /// <param name="left">
    /// The first instance of <see cref="FontStretch"/> to compare.
    /// </param>
    /// <param name="right">
    /// The second instance of <see cref="FontStretch"/> to compare.
    /// </param>
    /// <returns>
    /// true if left is greater than or equal to right; otherwise, false.
    /// </returns>
    public static bool operator >=(FontStretch left, FontStretch right) => Compare(left, right) >= 0;

    /// <summary>
    /// Creates a string representation of this object based on the format string 
    /// and IFormatProvider passed in.  
    /// If the provider is null, the CurrentCulture is used.
    /// See the documentation for IFormattable for more information.
    /// </summary>
    /// <returns>
    /// A string representation of this object.
    /// </returns>
    private string ConvertToString(string format, IFormatProvider provider)
    {
        FontStretches.FontStretchToString(RealStretch, out string convertedValue);
        return convertedValue;
    }

    /// <summary>
    /// We want the default zero value of new FontStretch() to correspond to <see cref="FontStretches.Normal"/>.
    /// Therefore, _stretch value is shifted by 5 relative to the OpenType stretch value.
    /// </summary>
    private int RealStretch => _stretch + 5;
}
