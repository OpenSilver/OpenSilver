
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

using System.Collections.Generic;
using System.Globalization;

namespace OpenSilver.Internal.Data;

/// <summary>
/// Creates a comparer class that takes in a CultureInfo as a parameter,
/// which it will use when comparing strings.
/// </summary>
internal sealed class CultureSensitiveComparer : IComparer<object>
{
    private readonly CultureInfo _culture;

    private CultureSensitiveComparer(CultureInfo culture)
    {
        _culture = culture ?? CultureInfo.InvariantCulture;
    }

    /// <summary>
    /// Creates a comparer which will respect the CultureInfo
    /// that is passed in when comparing strings.
    /// </summary>
    /// <param name="culture">The CultureInfo to use in string comparisons</param>
    public static CultureSensitiveComparer GetComparer(CultureInfo culture)
    {
        if (culture is null || culture == CultureInfo.InvariantCulture)
        {
            return Invariant;
        }

        if (culture == CultureInfo.CurrentCulture)
        {
            return Default;
        }

        return new CultureSensitiveComparer(culture);
    }

    public static CultureSensitiveComparer Default { get; } = new(CultureInfo.CurrentCulture);

    public static CultureSensitiveComparer Invariant { get; } = new(CultureInfo.InvariantCulture);

    /// <summary>
    /// Compares two objects and returns a value indicating whether one is less than, equal to or greater than the other.
    /// </summary>
    /// <param name="x">first item to compare</param>
    /// <param name="y">second item to compare</param>
    /// <returns>Negative number if x is less than y, zero if equal, and a positive number if x is greater than y</returns>
    /// <remarks>
    /// Compares the 2 items using the specified CultureInfo for string and using the default object comparer for all other objects.
    /// </remarks>
    public int Compare(object x, object y)
    {
        if (x is null)
        {
            if (y is not null)
            {
                return -1;
            }
            return 0;
        }
        if (y is null)
        {
            return 1;
        }

        // at this point x and y are not null
        if (x.GetType() == typeof(string) && y.GetType() == typeof(string))
        {
            return _culture.CompareInfo.Compare((string)x, (string)y);
        }
        else
        {
            return Comparer<object>.Default.Compare(x, y);
        }
    }
}
