
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

using System.Globalization;
using OpenSilver.Internal;

namespace System.Windows.Media;

/// <summary>
/// Represents an ordered collection of Double values.
/// </summary>
public sealed class DoubleCollection : PresentationFrameworkCollection<double>
{
    public DoubleCollection() { }

    /// <summary>
    /// Converts a <see cref="string"/> representation of a collection of doubles into an equivalent <see cref="DoubleCollection"/>.
    /// </summary>
    /// <param name="source">
    /// The <see cref="string"/> representation of the collection of doubles.
    /// </param>
    /// <returns>
    /// Returns the equivalent <see cref="DoubleCollection"/>.
    /// </returns>
    public static DoubleCollection Parse(string source)
    {
        IFormatProvider formatProvider = CultureInfo.InvariantCulture;

        var th = new TokenizerHelper(source, formatProvider);

        var collection = new DoubleCollection();

        while (th.NextToken())
        {
            collection.Add(Convert.ToDouble(th.GetCurrentToken(), formatProvider));
        }

        return collection;
    }

    internal override void AddOverride(double value) => AddInternal(value);

    internal override void ClearOverride() => ClearInternal();

    internal override void InsertOverride(int index, double value) => InsertInternal(index, value);

    internal override void RemoveAtOverride(int index) => RemoveAtInternal(index);

    internal override double GetItemOverride(int index) => GetItemInternal(index);

    internal override void SetItemOverride(int index, double value) => SetItemInternal(index, value);
}