
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

using System.Windows.Media;

namespace System.Windows.Controls;

/// <summary>
/// Defines an area within which you can explicitly position child elements by using coordinates 
/// that are relative to the <see cref="Canvas"/> area.
/// </summary>
public class Canvas : Panel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Canvas"/> class.
    /// </summary>
    public Canvas() { }

    /// <summary>
    /// Identifies the Canvas.Left attached property.
    /// </summary>
    public static readonly DependencyProperty LeftProperty =
        DependencyProperty.RegisterAttached(
            "Left",
            typeof(double),
            typeof(Canvas),
            new FrameworkPropertyMetadata(double.NaN, OnPositioningChanged),
            IsDoubleFiniteOrNaN);

    /// <summary>
    /// Gets the value of the Canvas.Left attached property for a given dependency object.
    /// </summary>
    /// <param name="element">
    /// The element from which the property value is read.
    /// </param>
    /// <returns>
    /// The Canvas.Left coordinate of the specified element.
    /// </returns>
    [AttachedPropertyBrowsableForChildren]
    public static double GetLeft(UIElement element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (double)element.GetValue(LeftProperty);
    }

    /// <summary>
    /// Sets the value of the Canvas.Left attached property for a given dependency object.
    /// </summary>
    /// <param name="element">
    /// The element to which the property value is written.
    /// </param>
    /// <param name="value">
    /// Sets the Canvas.Left coordinate of the specified element.
    /// </param>
    public static void SetLeft(UIElement element, double value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(LeftProperty, value);
    }

    /// <summary>
    /// Identifies the Canvas.Top attached property.
    /// </summary>
    public static readonly DependencyProperty TopProperty =
        DependencyProperty.RegisterAttached(
            "Top",
            typeof(double),
            typeof(Canvas),
            new FrameworkPropertyMetadata(double.NaN, OnPositioningChanged),
            IsDoubleFiniteOrNaN);

    /// <summary>
    /// Gets the value of the Canvas.Top attached property for a given dependency object.
    /// </summary>
    /// <param name="element">
    /// The element from which the property value is read.
    /// </param>
    /// <returns>
    /// The Canvas.Top coordinate of the specified element.
    /// </returns>
    [AttachedPropertyBrowsableForChildren]
    public static double GetTop(UIElement element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (double)element.GetValue(TopProperty);
    }

    /// <summary>
    /// Sets the value of the Canvas.Top attached property for a given dependency object.
    /// </summary>
    /// <param name="element">
    /// The element to which the property value is written.
    /// </param>
    /// <param name="value">
    /// Sets the Canvas.Top coordinate of the specified element.
    /// </param>
    public static void SetTop(UIElement element, double value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(TopProperty, value);
    }

    /// <summary>
    /// Identifies the Canvas.Right attached property.
    /// </summary>
    public static readonly DependencyProperty RightProperty =
        DependencyProperty.RegisterAttached(
            "Right",
            typeof(double),
            typeof(Canvas),
            new FrameworkPropertyMetadata(double.NaN, OnPositioningChanged),
            IsDoubleFiniteOrNaN);

    /// <summary>
    /// Gets the value of the Canvas.Right attached property for a given dependency object.
    /// </summary>
    /// <param name="element">
    /// The element from which the property value is read.
    /// </param>
    /// <returns>
    /// The Canvas.Right coordinate of the specified element.
    /// </returns>
    [AttachedPropertyBrowsableForChildren]
    public static double GetRight(UIElement element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (double)element.GetValue(RightProperty);
    }

    /// <summary>
    /// Sets the value of the Canvas.Right attached property for a given dependency object.
    /// </summary>
    /// <param name="element">
    /// The element to which the property value is written.
    /// </param>
    /// <param name="length">
    /// Sets the Canvas.Right coordinate of the specified element.
    /// </param>
    public static void SetRight(UIElement element, double length)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(RightProperty, length);
    }

    /// <summary>
    /// Identifies the Canvas.Bottom attached property.
    /// </summary>
    public static readonly DependencyProperty BottomProperty =
        DependencyProperty.RegisterAttached(
            "Bottom",
            typeof(double),
            typeof(Canvas),
            new FrameworkPropertyMetadata(double.NaN, OnPositioningChanged),
            IsDoubleFiniteOrNaN);

    /// <summary>
    /// Returns the value of the Canvas.Bottom attached property for a given dependency object.
    /// </summary>
    /// <param name="element">
    /// The element from which the property value is read.
    /// </param>
    /// <returns>
    /// The Canvas.Bottom coordinate of the specified element.
    /// </returns>
    [AttachedPropertyBrowsableForChildren]
    public static double GetBottom(UIElement element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (double)element.GetValue(BottomProperty);
    }

    /// <summary>
    /// Sets the value of the Canvas.Bottom attached property for a given dependency object.
    /// </summary>
    /// <param name="element">
    /// The element to which the property value is written.
    /// </param>
    /// <param name="length">
    /// Sets the Canvas.Bottom coordinate of the specified element.
    /// </param>
    public static void SetBottom(UIElement element, double length)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(BottomProperty, length);
    }

    private static bool IsDoubleFiniteOrNaN(object o) => !double.IsInfinity((double)o);

    private static void OnPositioningChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is UIElement uie && VisualTreeHelper.GetParent(uie) is Canvas p)
        {
            p.InvalidateArrange();
        }
    }

    /// <summary>
    /// Measures the child elements of a <see cref="Canvas"/> in anticipation of arranging them during the 
    /// <see cref="ArrangeOverride(Size)"/> pass.
    /// </summary>
    /// <param name="constraint">
    /// An upper limit <see cref="Size"/> that should not be exceeded.
    /// </param>
    /// <returns>
    /// A <see cref="Size"/> that represents the size that is required to arrange child content.
    /// </returns>
    protected override Size MeasureOverride(Size constraint)
    {
        var childConstraint = new Size(double.PositiveInfinity, double.PositiveInfinity);

        foreach (UIElement child in InternalChildren)
        {
            child.Measure(childConstraint);
        }

        return new Size();
    }

    /// <summary>
    /// Arranges the content of a <see cref="Canvas"/> element.
    /// </summary>
    /// <param name="arrangeSize">
    /// The size that this <see cref="Canvas"/> element should use to arrange its child elements.
    /// </param>
    /// <returns>
    /// A <see cref="Size"/> that represents the arranged size of this <see cref="Canvas"/> element and its descendants.
    /// </returns>
    protected override Size ArrangeOverride(Size arrangeSize)
    {
        foreach (UIElement child in InternalChildren)
        {
            double x = 0;
            double y = 0;

            // Compute offset of the child:
            // If Left is specified, then Right is ignored
            // If Left is not specified, then Right is used
            // If both are not there, then 0
            double left = GetLeft(child);
            if (!double.IsNaN(left))
            {
                x = left;
            }
            else
            {
                double right = GetRight(child);

                if (!double.IsNaN(right))
                {
                    x = arrangeSize.Width - child.DesiredSize.Width - right;
                }
            }

            double top = GetTop(child);
            if (!double.IsNaN(top))
            {
                y = top;
            }
            else
            {
                double bottom = GetBottom(child);

                if (!double.IsNaN(bottom))
                {
                    y = arrangeSize.Height - child.DesiredSize.Height - bottom;
                }
            }

            child.Arrange(new Rect(new Point(x, y), child.DesiredSize));
        }

        return arrangeSize;
    }

    internal override Rect? GetLayoutClip(Size layoutSlotSize)
    {
        if (ClipToBounds)
        {
            return new Rect(RenderSize);
        }
        else
        {
            return null;
        }
    }
}
