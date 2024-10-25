
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

namespace System.Windows.Controls;

/// <summary>
/// Defines row-specific properties that apply to <see cref="Grid"/> elements.
/// </summary>
public sealed class RowDefinition : DefinitionBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RowDefinition"/> class.
    /// </summary>
    public RowDefinition()
        : base(ThisIsRowDefinition)
    {
    }

    /// <summary>
    /// Returns a copy of this <see cref="RowDefinition"/>.
    /// </summary>
    public RowDefinition Clone() =>
        new()
        {
            MinHeight = MinHeight,
            MaxHeight = MaxHeight,
            Height = Height.Clone()
        };

    /// <summary>
    /// Gets or sets a value that represents the maximum height of a <see cref="RowDefinition"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="double"/> that represents the maximum height.
    /// </returns>
    public double MaxHeight
    {
        get => (double)GetValue(MaxHeightProperty);
        set => SetValueInternal(MaxHeightProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="MaxHeight"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MaxHeightProperty =
        DependencyProperty.Register(
            nameof(MaxHeight),
            typeof(double),
            typeof(RowDefinition),
            new PropertyMetadata(double.PositiveInfinity, OnUserMaxSizePropertyChanged),
            FrameworkElement.IsMaxWidthHeightValid);

    /// <summary>
    /// Gets or sets a value that represents the minimum allowed height of a <see cref="RowDefinition"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="double"/> that represents the minimum allowed height. The default value is 0.
    /// </returns>
    public double MinHeight
    {
        get => (double)GetValue(MinHeightProperty);
        set => SetValueInternal(MinHeightProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="MinHeight"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MinHeightProperty =
        DependencyProperty.Register(
            nameof(MinHeight),
            typeof(double),
            typeof(RowDefinition),
            new PropertyMetadata(0d, OnUserMinSizePropertyChanged),
            FrameworkElement.IsMinWidthHeightValid);

    /// <summary>
    /// Gets the calculated height of a <see cref="RowDefinition"/> element,
    /// or sets the <see cref="GridLength"/> value of a row that is defined by the <see cref="RowDefinition"/>.
    /// </summary>
    /// <returns>
    /// The <see cref="GridLength"/> that represents the height of the row. The default value is 1.0.
    /// </returns>
    public GridLength Height
    {
        get => (GridLength)GetValue(HeightProperty);
        set => SetValueInternal(HeightProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Height"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty HeightProperty =
        DependencyProperty.Register(
            nameof(Height),
            typeof(GridLength),
            typeof(RowDefinition),
            new PropertyMetadata(new GridLength(1.0, GridUnitType.Star), OnUserSizePropertyChanged));

    private static readonly DependencyPropertyKey ActualHeightPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(ActualHeight),
            typeof(double),
            typeof(RowDefinition),
            _actualHeightMetadata);

    private static readonly ReadOnlyPropertyMetadata _actualHeightMetadata = new(0.0, GetActualHeight);

    private static object GetActualHeight(DependencyObject d) => ((RowDefinition)d).ActualHeight;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly DependencyProperty ActualHeightProperty = ActualHeightPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets a value that represents the calculated height of the <see cref="RowDefinition"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="double"/> that represents the calculated height in pixels. The default value is 0.
    /// </returns>
    public double ActualHeight
    {
        get
        {
            double value = 0.0;

            if (InParentLogicalTree)
            {
                value = Parent.GetFinalRowDefinitionHeight(Index);
            }

            return value;
        }
    }

    /// <summary>
    /// Gets a value that represents the offset value of this <see cref="RowDefinition"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="double"/> that represents the offset of the row. The default value is 0.0.
    /// </returns>
    public double Offset
    {
        get
        {
            double value = 0.0;

            if (Index != 0)
            {
                value = FinalOffset;
            }

            return value;
        }
    }
}
