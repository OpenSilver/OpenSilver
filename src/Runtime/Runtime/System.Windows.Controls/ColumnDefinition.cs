
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
/// Defines column-specific properties that apply to <see cref="Grid"/> objects.
/// </summary>
public sealed class ColumnDefinition : DefinitionBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnDefinition"/> class.
    /// </summary>
    public ColumnDefinition()
        : base(ThisIsColumnDefinition)
    {
    }

    /// <summary>
    /// Returns a copy of the current <see cref="ColumnDefinition"/>.
    /// </summary>
    public ColumnDefinition Clone() =>
        new()
        {
            MinWidth = MinWidth,
            MaxWidth = MaxWidth,
            Width = Width.Clone()
        };

    /// <summary>
    /// Gets or sets a value that represents the maximum width of a <see cref="ColumnDefinition"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="double"/> that represents the maximum width in pixels. The default is 
    /// <see cref="double.PositiveInfinity"/>.
    /// </returns>
    public double MaxWidth
    {
        get => (double)GetValue(MaxWidthProperty);
        set => SetValueInternal(MaxWidthProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="MaxWidth"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MaxWidthProperty =
        DependencyProperty.Register(
            nameof(MaxWidth),
            typeof(double),
            typeof(ColumnDefinition),
            new PropertyMetadata(double.PositiveInfinity, OnUserMaxSizePropertyChanged),
            FrameworkElement.IsMaxWidthHeightValid);

    /// <summary>
    /// Gets or sets a value that represents the minimum width of a <see cref="ColumnDefinition"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="double"/> that represents the minimum width in pixels. The default is 0.
    /// </returns>
    public double MinWidth
    {
        get => (double)GetValue(MinWidthProperty);
        set => SetValueInternal(MinWidthProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="MinWidth"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MinWidthProperty =
        DependencyProperty.Register(
            nameof(MinWidth),
            typeof(double),
            typeof(ColumnDefinition),
            new PropertyMetadata(0d, OnUserMinSizePropertyChanged),
            FrameworkElement.IsMinWidthHeightValid);

    /// <summary>
    /// Gets the calculated width of a <see cref="ColumnDefinition"/> element,
    /// or sets the <see cref="GridLength"/> value of a column that is defined by the
    /// <see cref="ColumnDefinition"/>.
    /// </summary>
    /// <returns>
    /// The <see cref="GridLength"/> that represents the width of the column. The default
    /// value is 1.0.
    /// </returns>
    public GridLength Width
    {
        get => (GridLength)GetValue(WidthProperty);
        set => SetValueInternal(WidthProperty, value);
    }

    /// <summary>
    /// Identifies the Width dependency property.
    /// </summary>
    public static readonly DependencyProperty WidthProperty =
        DependencyProperty.Register(
            nameof(Width),
            typeof(GridLength),
            typeof(ColumnDefinition),
            new PropertyMetadata(new GridLength(1.0, GridUnitType.Star), OnUserSizePropertyChanged));

    private static readonly DependencyPropertyKey ActualWidthPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(ActualWidth),
            typeof(double),
            typeof(ColumnDefinition),
            _actualWidthMetadata);

    private static readonly ReadOnlyPropertyMetadata _actualWidthMetadata = new(0.0, GetActualWidth);

    private static object GetActualWidth(DependencyObject d) => ((ColumnDefinition)d).ActualWidth;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly DependencyProperty ActualWidthProperty = ActualWidthPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets a value that represents the actual calculated width of a <see cref="ColumnDefinition"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="double"/> that represents the actual calculated width in pixels. The default is 0.
    /// </returns>
    public double ActualWidth
    {
        get
        {
            double value = 0.0;

            if (InParentLogicalTree)
            {
                value = Parent.GetFinalColumnDefinitionWidth(Index);
            }

            return value;
        }
    }

    /// <summary>
    /// Gets a value that represents the offset value of this <see cref="ColumnDefinition"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="double"/> that represents the offset of the column. The default value is 0.0.
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
