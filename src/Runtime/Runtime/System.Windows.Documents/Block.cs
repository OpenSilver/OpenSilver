
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

namespace System.Windows.Documents;

/// <summary>
/// An abstract class that provides a base for all block-level content elements.
/// </summary>
public abstract class Block : TextElement
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Block" /> class. 
    /// </summary>
    protected Block() { }

    /// <summary>
    /// Identifies the <see cref="LineHeight" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty LineHeightProperty =
        DependencyProperty.RegisterAttached(
            nameof(LineHeight),
            typeof(double),
            typeof(Block),
            new PropertyMetadata(0.0) { Inherits = true, },
            IsValidLineHeight);

    /// <summary>
    /// Gets or sets the height of each line of content.
    /// </summary>
    /// <returns>
    /// The height of each line in pixels. A value of 0 indicates that the line
    /// height is determined automatically from the current font characteristics. 
    /// The default is 0.
    /// </returns>
    public double LineHeight
    {
        get => (double)GetValue(LineHeightProperty);
        set => SetValueInternal(LineHeightProperty, value);
    }

    private static bool IsValidLineHeight(object o)
    {
        double d = (double)o;
        return !double.IsNaN(d) && d >= 0;
    }

    /// <summary>
    /// Identifies the <see cref="LineStackingStrategy" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty LineStackingStrategyProperty =
        DependencyProperty.RegisterAttached(
            nameof(LineStackingStrategy),
            typeof(LineStackingStrategy),
            typeof(Block),
            new PropertyMetadata(LineStackingStrategy.MaxHeight) { Inherits = true, });

    /// <summary>
    /// Gets or sets a value that indicates how a line box is determined for each 
    /// line of text in a <see cref="Block" />.
    /// The default is <see cref="LineStackingStrategy.MaxHeight" />.
    /// </summary>
    public LineStackingStrategy LineStackingStrategy
    {
        get => (LineStackingStrategy)GetValue(LineStackingStrategyProperty);
        set => SetValueInternal(LineStackingStrategyProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="TextAlignment" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty TextAlignmentProperty =
        DependencyProperty.RegisterAttached(
            nameof(TextAlignment),
            typeof(TextAlignment),
            typeof(Block),
            new PropertyMetadata(TextAlignment.Left) { Inherits = true, });

    /// <summary>
    /// Gets or sets the horizontal alignment of the text content. 
    /// The default is <see cref="TextAlignment.Left" />.
    /// </summary>
    public TextAlignment TextAlignment
    {
        get => (TextAlignment)GetValue(TextAlignmentProperty);
        set => SetValueInternal(TextAlignmentProperty, value);
    }
}
