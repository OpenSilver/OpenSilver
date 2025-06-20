
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

using OpenSilver.Internal;
using System.ComponentModel;

namespace System.Windows;

/// <summary>
/// Represents a set of width thresholds used by <see cref="ResponsiveExtension"/> to determine responsive 
/// breakpoints for mobile and tablet devices.
/// </summary>
[TypeConverter(typeof(ResponsiveThresholdConverter))]
public readonly struct ResponsiveThreshold : IFormattable
{
    /// <summary>
    /// Gets or sets the default responsive threshold, with mobile set to 768px and tablet set to 1024px.
    /// </summary>
    public static ResponsiveThreshold Default { get; set; } = new ResponsiveThreshold(768, 1024);

    /// <summary>
    /// Initializes a new instance of the <see cref="ResponsiveThreshold"/> struct with a single threshold 
    /// for mobile and tablet.
    /// </summary>
    /// <param name="mobile">
    /// The width threshold in pixels for both mobile and tablet devices. Must be non-negative.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="mobile"/> is negative.
    /// </exception>
    public ResponsiveThreshold(double mobile)
    {
        if (mobile < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(mobile),
                string.Format(Strings.ArgumentOutOfRange_Generic_MustBeNonNegative, nameof(mobile), mobile));
        }

        Mobile = Tablet = mobile;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResponsiveThreshold"/> struct with specified thresholds 
    /// for mobile and tablet.
    /// </summary>
    /// <param name="mobile">
    /// The width threshold in pixels for mobile devices. Must be non-negative.
    /// </param>
    /// <param name="tablet">
    /// The width threshold in pixels for tablet devices. Must be non-negative and greater than or equal to 
    /// <paramref name="mobile"/>.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="mobile"/> or <paramref name="tablet"/> is negative.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="tablet"/> is less than <paramref name="mobile"/>.
    /// </exception>
    public ResponsiveThreshold(double mobile, double tablet)
    {
        if (mobile < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(mobile),
                string.Format(Strings.ArgumentOutOfRange_Generic_MustBeNonNegative, nameof(mobile), mobile));
        }

        if (tablet < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(tablet),
                string.Format(Strings.ArgumentOutOfRange_Generic_MustBeNonNegative, nameof(tablet), tablet));
        }

        if (tablet < mobile)
        {
            throw new ArgumentException(string.Format(Strings.ResponsiveThreshold_Invalid, nameof(tablet), nameof(mobile)));
        }

        Mobile = mobile;
        Tablet = tablet;
    }

    /// <summary>
    /// Creates a new <see cref="ResponsiveThreshold"/> from the specified string representation.
    /// </summary>
    /// <param name="source">
    /// The string representation of the <see cref="ResponsiveThreshold"/>, in the form "Mobile, Tablet" or "Mobile".
    /// </param>
    /// <returns>
    /// The resulting <see cref="ResponsiveThreshold"/>.
    /// </returns>
    public static ResponsiveThreshold Parse(string source) => Parse(source, null);

    /// <summary>
    /// Creates a new <see cref="ResponsiveThreshold"/> from the specified string representation.
    /// </summary>
    /// <param name="source">
    /// The string representation of the <see cref="ResponsiveThreshold"/>, in the form "Mobile, Tablet" or "Mobile".
    /// </param>
    /// <param name="provider">
    /// An object that supplies culture-specific formatting information.
    /// </param>
    /// <returns>
    /// The resulting <see cref="ResponsiveThreshold"/>.
    /// </returns>
    public static ResponsiveThreshold Parse(string source, IFormatProvider provider)
    {
        var th = new TokenizerHelper(source, provider);

        double mobile = Convert.ToDouble(th.NextTokenRequired(), provider);
        double tablet = mobile;

        if (th.NextToken())
        {
            tablet = Convert.ToDouble(th.GetCurrentToken(), provider);
        }

        th.LastTokenRequired();

        return new ResponsiveThreshold(mobile, tablet);
    }

    /// <summary>
    /// Gets the width threshold in pixels for mobile devices.
    /// </summary>
    /// <returns>
    /// The mobile width threshold.
    /// </returns>
    public double Mobile { get; }

    /// <summary>
    /// Gets the width threshold in pixels for tablet devices.
    /// </summary>
    /// <returns>
    /// The tablet width threshold.
    /// </returns>
    public double Tablet { get; }

    /// <summary>
    /// Determines whether the specified <see cref="object"/> is a <see cref="ResponsiveThreshold"/> structure
    /// and, if it is, whether it has the same <see cref="Mobile"/> and <see cref="Tablet"/> values as this 
    /// <see cref="ResponsiveThreshold"/>.
    /// </summary>
    /// <param name="o">
    /// The <see cref="ResponsiveThreshold"/> to compare.
    /// </param>
    /// <returns>
    /// true if <paramref name="o"/> is a <see cref="ResponsiveThreshold"/> and has the same <see cref="Mobile"/> 
    /// and <see cref="Tablet"/> values as this <see cref="ResponsiveThreshold"/>; otherwise, false.
    /// </returns>
    public override bool Equals(object o) => o is ResponsiveThreshold threshold && this == threshold;

    /// <summary>
    /// Returns the hash code for this <see cref="ResponsiveThreshold"/>.
    /// </summary>
    /// <returns>
    /// The hash code for this instance.
    /// </returns>
    public override int GetHashCode() => Mobile.GetHashCode() ^ Tablet.GetHashCode();

    /// <summary>
    /// Returns the string representation of this <see cref="ResponsiveThreshold"/> structure.
    /// </summary>
    /// <returns>
    /// A string that represents the <see cref="Mobile"/> and <see cref="Tablet"/> values of this 
    /// <see cref="ResponsiveThreshold"/>.
    /// </returns>
    public override string ToString() => ConvertToString(null, null);

    /// <summary>
    /// Returns the string representation of this <see cref="ResponsiveThreshold"/> structure with the 
    /// specified formatting information.
    /// </summary>
    /// <param name="formatProvider">
    /// The culture-specific formatting information.
    /// </param>
    /// <returns>
    /// A string that represents the <see cref="Mobile"/> and <see cref="Tablet"/> values of this 
    /// <see cref="ResponsiveThreshold"/>.
    /// </returns>
    public string ToString(IFormatProvider formatProvider) => ConvertToString(null, formatProvider);

    /// <inheritdoc />
    string IFormattable.ToString(string format, IFormatProvider formatProvider) => ConvertToString(format, formatProvider);

    private string ConvertToString(string format, IFormatProvider formatProvider)
    {
        char separator = TokenizerHelper.GetNumericListSeparator(formatProvider);

        return $"{Mobile.ToString(format, formatProvider)}{separator}{Tablet.ToString(format, formatProvider)}";
    }

    /// <summary>
    /// Compares two <see cref="ResponsiveThreshold"/> for equality.
    /// </summary>
    /// <param name="threshold1">
    /// The first <see cref="ResponsiveThreshold"/> to compare.
    /// </param>
    /// <param name="threshold2">
    /// The second <see cref="ResponsiveThreshold"/> to compare.
    /// </param>
    /// <returns>
    /// true if the <see cref="Mobile"/> and <see cref="Tablet"/> components of <paramref name="threshold1"/> and 
    /// <paramref name="threshold2"/> are equal; otherwise, false.
    /// </returns>
    public static bool operator ==(ResponsiveThreshold threshold1, ResponsiveThreshold threshold2)
        => threshold1.Mobile == threshold2.Mobile && threshold1.Tablet == threshold2.Tablet;

    /// <summary>
    /// Compares two <see cref="ResponsiveThreshold"/> for inequality.
    /// </summary>
    /// <param name="threshold1">
    /// The first <see cref="ResponsiveThreshold"/> to compare.
    /// </param>
    /// <param name="threshold2">
    /// The second <see cref="ResponsiveThreshold"/> to compare.
    /// </param>
    /// <returns>
    /// true if the <see cref="Mobile"/> and <see cref="Tablet"/> components of <paramref name="threshold1"/> and 
    /// <paramref name="threshold2"/> are different; otherwise, false.
    /// </returns>
    public static bool operator !=(ResponsiveThreshold threshold1, ResponsiveThreshold threshold2) => !(threshold1 == threshold2);
}
