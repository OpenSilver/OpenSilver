
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

    /// <summary>
    /// Returns the value of the <see cref="LineHeight"/> attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object from which to retrieve the value of the <see cref="LineHeight"/> property.
    /// </param>
    /// <returns>
    /// The current value of the <see cref="LineHeight"/> attached property on the specified dependency object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// element is null.
    /// </exception>
    public static double GetLineHeight(DependencyObject element)
    {
        if (element is null)
        {
            throw new ArgumentNullException(nameof(element));
        }

        return (double)element.GetValue(LineHeightProperty);
    }

    /// <summary>
    /// Sets the value of the <see cref="LineHeight"/> attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object on which to set the value of the <see cref="LineHeight"/> property.
    /// </param>
    /// <param name="value">
    /// The new value to set the property to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// element is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// value is negative.
    /// </exception>
    public static void SetLineHeight(DependencyObject element, double value)
    {
        if (element is null)
        {
            throw new ArgumentNullException(nameof(element));
        }

        element.SetValueInternal(LineHeightProperty, value);
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
    /// Returns the value of the <see cref="LineStackingStrategy"/> attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object from which to retrieve the value of the <see cref="LineStackingStrategy"/> attached property.
    /// </param>
    /// <returns>
    /// The current value of the <see cref="LineStackingStrategy"/> attached property on the specified dependency object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// element is null.
    /// </exception>
    public static LineStackingStrategy GetLineStackingStrategy(DependencyObject element)
    {
        if (element is null)
        {
            throw new ArgumentNullException(nameof(element));
        }

        return (LineStackingStrategy)element.GetValue(LineStackingStrategyProperty);
    }

    /// <summary>
    /// Sets the value of the <see cref="LineStackingStrategy"/> attached property on a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object on which to set the value of the <see cref="LineStackingStrategy"/> property.
    /// </param>
    /// <param name="value">
    /// The new value to set the property to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// element is null.
    /// </exception>
    public static void SetLineStackingStrategy(DependencyObject element, LineStackingStrategy value)
    {
        if (element is null)
        {
            throw new ArgumentNullException(nameof(element));
        }

        element.SetValueInternal(LineStackingStrategyProperty, value);
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

    /// <summary>
    /// Returns the value of the <see cref="TextAlignment"/> attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object from which to retrieve the value of the <see cref="TextAlignment"/> property.
    /// </param>
    /// <returns>
    /// The current value of the <see cref="TextAlignment"/> attached property on the specified dependency object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// element is null.
    /// </exception>
    public static TextAlignment GetTextAlignment(DependencyObject element)
    {
        if (element is null)
        {
            throw new ArgumentNullException(nameof(element));
        }

        return (TextAlignment)element.GetValue(TextAlignmentProperty);
    }

    /// <summary>
    /// Sets the value of the <see cref="TextAlignment"/> attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object on which to set the value of the <see cref="TextAlignment"/> property.
    /// </param>
    /// <param name="value">
    /// The new value to set the property to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// element is null.
    /// </exception>
    public static void SetTextAlignment(DependencyObject element, TextAlignment value)
    {
        if (element is null)
        {
            throw new ArgumentNullException(nameof(element));
        }

        element.SetValueInternal(TextAlignmentProperty, value);
    }
}
