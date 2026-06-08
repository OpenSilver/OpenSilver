
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

using OpenSilver.Internal.Controls;
using System.Collections;
using System.Windows.Markup;

namespace System.Windows.Controls;

/// <summary>
/// Provides a base class for elements that apply effects onto or around a single child element,
/// such as <see cref="Border"/>.
/// </summary>
[ContentProperty(nameof(Child))]
public class Decorator : FrameworkElement
{
    private UIElement _child;

    /// <summary>
    /// Initializes a new instance of the <see cref="Decorator"/> class.
    /// </summary>
    public Decorator() { }

    /// <summary>
    /// Gets or sets the single child element of a <see cref="Decorator"/>.
    /// </summary>
    /// <returns>
    /// The single child element of a <see cref="Decorator"/>.
    /// </returns>
    public virtual UIElement Child
    {
        get => _child;
        set
        {
            if (_child == value)
            {
                return;
            }

            // notify the visual layer that the old child has been removed.
            RemoveVisualChild(_child);

            //need to remove old element from logical tree
            RemoveLogicalChild(_child);

            _child = value;

            AddLogicalChild(value);
            // notify the visual layer about the new child.
            AddVisualChild(value);

            InvalidateMeasure();
        }
    }

    /// <summary>
    /// Gets an enumerator that can be used to iterate the logical child elements of a <see cref="Decorator"/>.
    /// </summary>
    /// <returns>
    /// An enumerator that can be used to iterate the logical child elements of a <see cref="Decorator"/>.
    /// </returns>
    protected internal override IEnumerator LogicalChildren
    {
        get
        {
            if (_child is null)
            {
                return EmptyEnumerator.Instance;
            }

            return new SingleChildEnumerator(_child);
        }
    }

    /// <summary>
    /// Gets a value that is equal to the number of visual child elements of this instance of
    /// <see cref="Decorator"/>.
    /// </summary>
    /// <returns>
    /// The number of visual child elements.
    /// </returns>
    protected override int VisualChildrenCount => _child is null ? 0 : 1;

    /// <summary>
    /// Gets the child <see cref="UIElement"/> element at the specified index position.
    /// </summary>
    /// <param name="index">
    /// Index position of the child element.
    /// </param>
    /// <returns>
    /// The child element at the specified index position.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// index is greater than the number of visual child elements.
    /// </exception>
    protected override UIElement GetVisualChild(int index)
    {
        if (_child is null || index != 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        return _child;
    }

    /// <summary>
    /// Measures the child element of a <see cref="Decorator"/> to prepare for arranging it during 
    /// the <see cref="ArrangeOverride(Size)"/> pass.
    /// </summary>
    /// <param name="constraint">
    /// An upper limit <see cref="Size"/> that should not be exceeded.
    /// </param>
    /// <returns>
    /// The target <see cref="Size"/> of the element.
    /// </returns>
    protected override Size MeasureOverride(Size constraint)
    {
        if (Child is UIElement child)
        {
            child.Measure(constraint);
            return child.DesiredSize;
        }
        return new Size();
    }

    /// <summary>
    /// Arranges the content of a <see cref="Decorator"/> element.
    /// </summary>
    /// <param name="arrangeSize">
    /// The <see cref="Size"/> this element uses to arrange its child content.
    /// </param>
    /// <returns>
    /// The <see cref="Size"/> that represents the arranged size of this <see cref="Decorator"/> 
    /// element and its child.
    /// </returns>
    protected override Size ArrangeOverride(Size arrangeSize)
    {
        UIElement child = Child;
        child?.Arrange(new Rect(arrangeSize));
        return arrangeSize;
    }
}
