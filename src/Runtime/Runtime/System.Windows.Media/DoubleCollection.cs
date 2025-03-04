
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
using System.ComponentModel;
using System.Globalization;
using OpenSilver.Internal;

namespace System.Windows.Media;

/// <summary>
/// Represents an ordered collection of Double values.
/// </summary>
[TypeConverter(typeof(DoubleCollectionConverter))]
public sealed class DoubleCollection : PresentationFrameworkCollection<double>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DoubleCollection"/> class.
    /// </summary>
    public DoubleCollection() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DoubleCollection"/> class with the specified collection of 
    /// <see cref="double"/> values.
    /// </summary>
    /// <param name="collection">
    /// The collection of <see cref="double"/> values that make up the <see cref="DoubleCollection"/>.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// collection is null.
    /// </exception>
    public DoubleCollection(IEnumerable<double> collection)
        : base(collection)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DoubleCollection"/> class with the specified capacity, or the 
    /// number of <see cref="double"/> values the collection is initially capable of storing.
    /// </summary>
    /// <param name="capacity">
    /// The number of <see cref="double"/> values that the collection is initially capable of storing.
    /// </param>
    public DoubleCollection(int capacity)
        : base(capacity)
    {
    }

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