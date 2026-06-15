
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

namespace System.Windows.Media;

/// <summary>
/// Represents a collection of guide lines that can assist in adjusting rendered figures to a 
/// device pixel grid.
/// </summary>
[OpenSilver.NotImplemented]
public sealed class GuidelineSet : DependencyObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GuidelineSet"/> class.
    /// </summary>
    public GuidelineSet() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GuidelineSet"/> class with the specified 
    /// <see cref="GuidelinesX"/> and <see cref="GuidelinesY"/> values.
    /// </summary>
    /// <param name="guidelinesX">
    /// The value of the <see cref="GuidelinesX"/> property.
    /// </param>
    /// <param name="guidelinesY">
    /// The value of the <see cref="GuidelinesY"/> property.
    /// </param>
    public GuidelineSet(double[] guidelinesX, double[] guidelinesY)
    {
        if (guidelinesX is not null)
        {
            GuidelinesX = [.. guidelinesX];
        }

        if (guidelinesY is not null)
        {
            GuidelinesY = [.. guidelinesY];
        }
    }

    /// <summary>
    /// Identifies the <see cref="GuidelinesX"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty GuidelinesXProperty =
        DependencyProperty.Register(
            nameof(GuidelinesX),
            typeof(DoubleCollection),
            typeof(GuidelineSet),
            new PropertyMetadata(
                new PFCDefaultValueFactory<double>(
                    static () => new DoubleCollection(),
                    static (d, dp) => new DoubleCollection()),
                null,
                CoerceGuidelines));

    /// <summary>
    /// Gets or sets a series of coordinate values that represent guide lines on the X-axis.
    /// </summary>
    /// <returns>
    /// A <see cref="DoubleCollection"/> of values that represent guide lines on the X-axis.
    /// </returns>
    public DoubleCollection GuidelinesX
    {
        get => (DoubleCollection)GetValue(GuidelinesXProperty);
        set => SetValueInternal(GuidelinesXProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="GuidelinesY"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty GuidelinesYProperty =
        DependencyProperty.Register(
            nameof(GuidelinesY),
            typeof(DoubleCollection),
            typeof(GuidelineSet),
            new PropertyMetadata(
                new PFCDefaultValueFactory<double>(
                    static () => new DoubleCollection(),
                    static (d, dp) => new DoubleCollection()),
                null,
                CoerceGuidelines));

    /// <summary>
    /// Gets or sets a series of coordinate values that represent guide lines on the Y-axis.
    /// </summary>
    /// <returns>
    /// A <see cref="DoubleCollection"/> of values that represent guide lines on the Y-axis.
    /// </returns>
    public DoubleCollection GuidelinesY
    {
        get => (DoubleCollection)GetValue(GuidelinesYProperty);
        set => SetValueInternal(GuidelinesYProperty, value);
    }

    private static object CoerceGuidelines(DependencyObject d, object baseValue)
    {
        return baseValue ?? new DoubleCollection();
    }
}
