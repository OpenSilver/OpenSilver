
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

using CSHTML5.Internal;
using OpenSilver;
using OpenSilver.Internal;
using System.Windows.Media;

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
    /// Identifies the FlowDirection dependency property.
    /// </summary>
    public new static readonly DependencyProperty FlowDirectionProperty =
        FrameworkElement.FlowDirectionProperty.AddOwner(typeof(Block));

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
        ArgumentNullException.ThrowIfNull(element);

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
        ArgumentNullException.ThrowIfNull(element);

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
        ArgumentNullException.ThrowIfNull(element);

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
        ArgumentNullException.ThrowIfNull(element);

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
        ArgumentNullException.ThrowIfNull(element);

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
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(TextAlignmentProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Margin"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MarginProperty =
        DependencyProperty.Register(
            nameof(Margin),
            typeof(Thickness),
            typeof(Block),
            new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsMeasure)
            {
                MethodToUpdateDom2 = static (d, oldValue, newValue) => ((Block)d).SetMargin((Thickness)newValue)
            },
            IsValidMargin);

    /// <summary>
    /// Gets or sets the margin thickness for the element.
    /// </summary>
    /// <returns>
    /// A <see cref="Thickness"/> structure that specifies the amount of margin to apply, in device independent 
    /// pixels. The default is a uniform thickness of zero (0.0).
    /// </returns>
    public Thickness Margin
    {
        get => (Thickness)GetValue(MarginProperty);
        set => SetValueInternal(MarginProperty, value);
    }

    private static bool IsValidMargin(object o) => IsValidThickness((Thickness)o, true);

    /// <summary>
    /// Identifies the <see cref="Padding"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PaddingProperty =
        DependencyProperty.Register(
            nameof(Padding),
            typeof(Thickness),
            typeof(Block),
            new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsMeasure)
            {
                MethodToUpdateDom2 = static (d, oldValue, newValue) => ((Block)d).SetPadding((Thickness)newValue),
            },
            IsValidPadding);

    /// <summary>
    /// Gets or sets the padding thickness for the element.
    /// </summary>
    /// <returns>
    /// A <see cref="Thickness"/> structure that specifies the amount of padding to apply, in device independent
    /// pixels. The default is a uniform thickness of zero (0.0).
    /// </returns>
    public Thickness Padding
    {
        get => (Thickness)GetValue(PaddingProperty);
        set => SetValueInternal(PaddingProperty, value);
    }

    private static bool IsValidPadding(object o) => IsValidThickness((Thickness)o, true);

    /// <summary>
    /// Identifies the <see cref="BorderThickness"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BorderThicknessProperty =
        DependencyProperty.Register(
            nameof(BorderThickness),
            typeof(Thickness),
            typeof(Block),
            new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsMeasure)
            {
                MethodToUpdateDom2 = static (d, oldValue, newValue) => ((Block)d).SetBorderWidth((Thickness)newValue),
            },
            IsValidBorderThickness);

    /// <summary>
    /// Gets or sets the border thickness for the element.
    /// </summary>
    /// <returns>
    /// A <see cref="Thickness"/> structure specifying the amount of border to apply, in device independent
    /// pixels. The default is a uniform thickness of zero (0.0).
    /// </returns>
    public Thickness BorderThickness
    {
        get => (Thickness)GetValue(BorderThicknessProperty);
        set => SetValueInternal(BorderThicknessProperty, value);
    }

    private static bool IsValidBorderThickness(object o) => IsValidThickness((Thickness) o, false);

    /// <summary>
    /// Identifies the <see cref="BorderBrush"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BorderBrushProperty =
        DependencyProperty.Register(
            nameof(BorderBrush),
            typeof(Brush),
            typeof(Block),
            new FrameworkPropertyMetadata((object)null)
            {
                MethodToUpdateDom2 = static (d, oldValue, newValue) => ((Block)d).SetBorderColor(oldValue as Brush, (Brush)newValue),
            });

    /// <summary>
    /// Gets or sets a <see cref="Brush"/> to use when painting the element's border.
    /// </summary>
    /// <returns>
    /// The brush used to apply to the element's border. The default is null.
    /// </returns>
    public Brush BorderBrush
    {
        get => (Brush)GetValue(BorderBrushProperty);
        set => SetValueInternal(BorderBrushProperty, value);
    }

    private static bool IsValidThickness(Thickness t, bool allowNaN)
    {
        const double maxThickness = 1000000;

        if (!allowNaN)
        {
            if (double.IsNaN(t.Left) || double.IsNaN(t.Right) || double.IsNaN(t.Top) || double.IsNaN(t.Bottom))
            {
                return false;
            }
        }
        if (!double.IsNaN(t.Left) && (t.Left < 0 || t.Left > maxThickness))
        {
            return false;
        }
        if (!double.IsNaN(t.Right) && (t.Right < 0 || t.Right > maxThickness))
        {
            return false;
        }
        if (!double.IsNaN(t.Top) && (t.Top < 0 || t.Top > maxThickness))
        {
            return false;
        }
        if (!double.IsNaN(t.Bottom) && (t.Bottom < 0 || t.Bottom > maxThickness))
        {
            return false;
        }
        return true;
    }

    /// <inheritdoc />
    protected internal override HtmlElementReference CreateDomElement(HtmlElementReference parent)
    {
        return INTERNAL_HtmlDomManager.CreateBlockDomElementAndAppendIt(parent, this);
    }
}
