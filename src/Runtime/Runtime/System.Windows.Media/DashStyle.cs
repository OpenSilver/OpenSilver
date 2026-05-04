
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
using System.Collections.Generic;

namespace System.Windows.Media;

/// <summary>
/// Represents the sequence of dashes and gaps that will be applied by a <see cref="Pen"/>.
/// </summary>
[OpenSilver.NotImplemented]
public class DashStyle : DependencyObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DashStyle"/> class.
    /// </summary>
    public DashStyle() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DashStyle"/> class with the specified <see cref="Dashes"/>
    /// and <see cref="Offset"/>.
    /// </summary>
    /// <param name="dashes">
    /// The <see cref="Dashes"/> of the <see cref="DashStyle"/>.
    /// </param>
    /// <param name="offset">
    /// The <see cref="Offset"/> of the <see cref="DashStyle"/>.
    /// </param>
    public DashStyle(IEnumerable<double> dashes, double offset)
    {
        Offset = offset;

        if (dashes is not null)
        {
            Dashes = [.. dashes];
        }
    }

    /// <summary>
    /// Identifies the <see cref="Dashes"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DashesProperty =
        DependencyProperty.Register(
            nameof(Dashes),
            typeof(DoubleCollection),
            typeof(DashStyle),
            new PropertyMetadata(new PFCDefaultValueFactory<double>(
                static () => new DoubleCollection(),
                static (d, dp) => new DoubleCollection())));

    /// <summary>
    /// Gets or sets the collection of dashes and gaps in this <see cref="DashStyle"/>.
    /// </summary>
    /// <returns>
    /// The collection of dashes and gaps. The default is an empty <see cref="DoubleCollection"/>.
    /// </returns>
    public DoubleCollection Dashes
    {
        get => (DoubleCollection)GetValue(DashesProperty);
        set => SetValueInternal(DashesProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Offset"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty OffsetProperty =
        DependencyProperty.Register(
            nameof(Offset),
            typeof(double),
            typeof(DashStyle),
            new PropertyMetadata(0.0));

    /// <summary>
    /// Gets or sets how far in the dash sequence the stroke will start.
    /// </summary>
    /// <returns>
    /// The offset for the dash sequence. The default is 0.
    /// </returns>
    public double Offset
    {
        get => (double)GetValue(OffsetProperty);
        set => SetValueInternal(OffsetProperty, value);
    }
}
