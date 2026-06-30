
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
using System.Windows.Media;

namespace System.Windows.Controls;

/// <summary>
/// Defines an area where you can arrange child elements either horizontally or vertically, relative to each other.
/// </summary>
public class DockPanel : Panel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DockPanel" /> class.
    /// </summary>
    public DockPanel() { }

    /// <summary>
    /// Identifies the <see cref="LastChildFill"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LastChildFillProperty =
        DependencyProperty.Register(
            nameof(LastChildFill),
            typeof(bool),
            typeof(DockPanel),
            new FrameworkPropertyMetadata(BooleanBoxes.TrueBox, FrameworkPropertyMetadataOptions.AffectsArrange));

    /// <summary>
    /// Gets or sets a value that indicates whether the last child element within a <see cref="DockPanel"/> stretches 
    /// to fill the remaining available space.
    /// </summary>
    /// <value>
    /// true if the last child element stretches to fill the remaining space; otherwise false. The default value is true.
    /// </value>
    public bool LastChildFill
    {
        get => (bool)GetValue(LastChildFillProperty);
        set => SetValueInternal(LastChildFillProperty, value);
    }

    /// <summary>
    /// Identifies the <b>DockPanel.Dock</b> attached property.
    /// </summary>
    public static readonly DependencyProperty DockProperty =
        DependencyProperty.RegisterAttached(
            "Dock",
            typeof(Dock),
            typeof(DockPanel),
            new FrameworkPropertyMetadata(Dock.Left, OnDockChanged),
            IsValidDock);

    /// <summary>
    /// Gets the value of the <b>DockPanel.Dock</b> attached property for a specified <see cref="UIElement"/>.
    /// </summary>
    /// <param name="element">
    /// The element from which the property value is read.
    /// </param>
    /// <returns>
    /// The <b>DockPanel.Dock</b> property value for the element.
    /// </returns>
    [AttachedPropertyBrowsableForChildren]
    public static Dock GetDock(UIElement element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (Dock)element.GetValue(DockProperty);
    }

    /// <summary>
    /// Sets the value of the <b>DockPanel.Dock</b> attached property to a specified element.
    /// </summary>
    /// <param name="element">
    /// The element to which the attached property is written.
    /// </param>
    /// <param name="dock">
    /// The needed <see cref="Dock"/> value.
    /// </param>
    public static void SetDock(UIElement element, Dock dock)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(DockProperty, dock);
    }

    private static void OnDockChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        UIElement element = (UIElement)d;

        // Cause the DockPanel to update its layout when a child changes
        if (VisualTreeHelper.GetParent(element) is DockPanel panel)
        {
            panel.InvalidateMeasure();
        }
    }

    private static bool IsValidDock(object o)
    {
        Dock dock = (Dock)o;
        return dock == Dock.Left || dock == Dock.Top || dock == Dock.Right || dock == Dock.Bottom;
    }

    /// <summary>
    /// Measures the child elements of a <see cref="DockPanel"/> prior to arranging them during the 
    /// <see cref="ArrangeOverride(Size)"/> pass.
    /// </summary>
    /// <param name="constraint">
    /// A maximum <see cref="Size"/> to not exceed.
    /// </param>
    /// <returns>
    /// A <see cref="Size"/> that represents the element size you want.
    /// </returns>
    protected override Size MeasureOverride(Size constraint)
    {
        double usedWidth = 0.0;
        double usedHeight = 0.0;
        double maximumWidth = 0.0;
        double maximumHeight = 0.0;

        // Measure each of the Children
        foreach (UIElement element in UnsafeGetChildren())
        {
            // Get the child's desired size
            Size remainingSize = new Size(
                Math.Max(0.0, constraint.Width - usedWidth),
                Math.Max(0.0, constraint.Height - usedHeight));
            element.Measure(remainingSize);
            Size desiredSize = element.DesiredSize;

            // Decrease the remaining space for the rest of the children
            switch (GetDock(element))
            {
                case Dock.Left:
                case Dock.Right:
                    maximumHeight = Math.Max(maximumHeight, usedHeight + desiredSize.Height);
                    usedWidth += desiredSize.Width;
                    break;
                case Dock.Top:
                case Dock.Bottom:
                    maximumWidth = Math.Max(maximumWidth, usedWidth + desiredSize.Width);
                    usedHeight += desiredSize.Height;
                    break;
            }
        }

        maximumWidth = Math.Max(maximumWidth, usedWidth);
        maximumHeight = Math.Max(maximumHeight, usedHeight);
        return new Size(maximumWidth, maximumHeight);
    }

    /// <summary>
    /// Arranges the content (child elements) of a <see cref="DockPanel"/> element.
    /// </summary>
    /// <param name="arrangeSize">
    /// The <see cref="Size"/> this element uses to arrange its child elements.
    /// </param>
    /// <returns>
    /// The <see cref="Size"/> that represents the arranged size of this <see cref="DockPanel"/> element.
    /// </returns>
    protected override Size ArrangeOverride(Size arrangeSize)
    {
        double left = 0.0;
        double top = 0.0;
        double right = 0.0;
        double bottom = 0.0;

        // Arrange each of the Children
        List<UIElement> children = UnsafeGetChildren();
        int dockedCount = children.Count - (LastChildFill ? 1 : 0);
        int index = 0;
        foreach (UIElement element in children)
        {
            // Determine the remaining space left to arrange the element
            Rect remainingRect = new Rect(
                left,
                top,
                Math.Max(0.0, arrangeSize.Width - left - right),
                Math.Max(0.0, arrangeSize.Height - top - bottom));

            // Trim the remaining Rect to the docked size of the element
            // (unless the element should fill the remaining space because
            // of LastChildFill)
            if (index < dockedCount)
            {
                Size desiredSize = element.DesiredSize;
                switch (GetDock(element))
                {
                    case Dock.Left:
                        left += desiredSize.Width;
                        remainingRect.Width = desiredSize.Width;
                        break;
                    case Dock.Top:
                        top += desiredSize.Height;
                        remainingRect.Height = desiredSize.Height;
                        break;
                    case Dock.Right:
                        right += desiredSize.Width;
                        remainingRect.X = Math.Max(0.0, arrangeSize.Width - right);
                        remainingRect.Width = desiredSize.Width;
                        break;
                    case Dock.Bottom:
                        bottom += desiredSize.Height;
                        remainingRect.Y = Math.Max(0.0, arrangeSize.Height - bottom);
                        remainingRect.Height = desiredSize.Height;
                        break;
                }
            }

            element.Arrange(remainingRect);
            index++;
        }

        return arrangeSize;
    }
}