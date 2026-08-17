
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
using OpenSilver.Internal.Documents;
using System.Diagnostics;
using System.Linq;
using System.Windows.Markup;
using System.Windows.Media;

namespace System.Windows.Documents;

/// <summary>
/// A flow content element that represents a particular content item in an ordered or unordered 
/// <see cref="Documents.List"/>.
/// </summary>
[ContentProperty(nameof(Blocks))]
public class ListItem : TextElement
{
    /// <summary>
    /// Initializes a new, empty instance of the <see cref="ListItem"/> class.
    /// </summary>
    public ListItem()
    {
        Blocks = new BlockCollection(this, TextContainer);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ListItem"/> class, taking a specified <see cref="Paragraph"/> 
    /// object as the initial contents of the new <see cref="ListItem"/>.
    /// </summary>
    /// <param name="paragraph">
    /// A <see cref="Paragraph"/> object specifying the initial contents of the new <see cref="ListItem"/>.
    /// </param>
    public ListItem(Paragraph paragraph)
        : this()
    {
        ArgumentNullException.ThrowIfNull(paragraph);
        Blocks.Add(paragraph);
    }

    /// <summary>
    /// Gets a block collection that contains the top-level <see cref="Block"/> elements of the <see cref="ListItem"/>.
    /// </summary>
    /// <returns>
    /// A block collection that contains the <see cref="Block"/> elements of the <see cref="ListItem"/>.
    /// </returns>
    public BlockCollection Blocks { get; }

    /// <summary>
    /// Gets the <see cref="Documents.List"/> that contains the <see cref="ListItem"/>.
    /// </summary>
    /// <returns>
    /// The list that contains the <see cref="ListItem"/>.
    /// </returns>
    public List List => VisualTreeHelper.GetParent(this) as List;

    /// <summary>
    /// Gets a <see cref="ListItemCollection"/> that contains the <see cref="ListItem"/> elements that are siblings of the 
    /// current <see cref="ListItem"/> element.
    /// </summary>
    /// <returns>
    /// A <see cref="ListItemCollection"/> that contains the child <see cref="ListItem"/> elements that are directly hosted 
    /// by the parent of the current <see cref="ListItem"/> element, or null if the current <see cref="ListItem"/> element 
    /// has no parent.
    /// </returns>
    public ListItemCollection SiblingListItems => List?.ListItems;

    /// <summary>
    /// Gets the previous <see cref="ListItem"/> in the containing <see cref="Documents.List"/>.
    /// </summary>
    /// <returns>
    /// The previous <see cref="ListItem"/> in the <see cref="Documents.List"/>, or null if there is no
    /// previous <see cref="ListItem"/>.
    /// </returns>
    public ListItem PreviousListItem
    {
        get
        {
            if (List is List list)
            {
                int index = list.ListItems.IndexOf(this);
                if (index > 0)
                {
                    return list.ListItems[index - 1];
                }
            }
            return null;
        }
    }

    /// <summary>
    /// Gets the next <see cref="ListItem"/> in the containing <see cref="Documents.List"/>.
    /// </summary>
    /// <returns>
    /// The next <see cref="ListItem"/> in the <see cref="Documents.List"/>, or null if there is no next
    /// <see cref="ListItem"/>.
    /// </returns>
    public ListItem NextListItem
    {
        get
        {
            if (List is List list)
            {
                int index = list.ListItems.IndexOf(this);
                if (index < list.ListItems.InternalCount - 1)
                {
                    return list.ListItems[index + 1];
                }
            }
            return null;
        }
    }

    /// <summary>
    /// Identifies the <see cref="Padding"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PaddingProperty =
        Block.PaddingProperty.AddOwner(
            typeof(ListItem),
            new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsMeasure)
            {
                MethodToUpdateDom2 = static (d, oldValue, newValue) => ((ListItem)d).SetPadding((Thickness)newValue),
            });

    /// <summary>
    /// Gets or sets the padding thickness for the element. 
    /// </summary>
    /// <returns>
    /// A <see cref="Thickness"/> structure that specifies the amount of padding to apply, in device independent pixels.
    /// The default is a uniform thickness of zero (0.0).
    /// </returns>
    public Thickness Padding
    {
        get => (Thickness)GetValue(PaddingProperty);
        set => SetValueInternal(PaddingProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Margin"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MarginProperty =
         Block.MarginProperty.AddOwner(
             typeof(ListItem),
             new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsMeasure)
             {
                 MethodToUpdateDom2 = static (d, oldValue, newValue) => ((ListItem)d).SetMargin((Thickness)newValue),
             });

    /// <summary>
    /// A <see cref="Thickness"/> structure that specifies the amount of margin to apply, in device independent pixels.
    /// The default is a uniform thickness of zero (0.0).
    /// </summary>
    /// <returns>
    /// Gets or sets the margin thickness for the element.
    /// </returns>
    public Thickness Margin
    {
        get => (Thickness)GetValue(MarginProperty);
        set => SetValueInternal(MarginProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="LineStackingStrategy"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LineStackingStrategyProperty =
        Block.LineStackingStrategyProperty.AddOwner(typeof(ListItem));

    /// <summary>
    /// Gets or sets the mechanism by which a line box is determined for each line of text within the 
    /// <see cref="ListItem"/>.
    /// </summary>
    /// <returns>
    /// One of the <see cref="Windows.LineStackingStrategy"/> values that specifies the mechanism by which 
    /// a line box is determined for each line of text within the <see cref="ListItem"/>. The default is 
    /// <see cref="LineStackingStrategy.MaxHeight"/>.
    /// </returns>
    public LineStackingStrategy LineStackingStrategy
    {
        get => (LineStackingStrategy)GetValue(LineStackingStrategyProperty);
        set => SetValueInternal(LineStackingStrategyProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="BorderThickness"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BorderThicknessProperty =
        Block.BorderThicknessProperty.AddOwner(
            typeof(ListItem),
            new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender)
            {
                MethodToUpdateDom2 = static (d, oldValue, newValue) => ((ListItem)d).SetBorderWidth((Thickness)newValue),
            });

    /// <summary>
    /// Gets or sets the border thickness for the element.
    /// </summary>
    /// <returns>
    /// A <see cref="Thickness"/> structure that specifies the amount of border to apply, in device independent pixels.
    /// The default is a uniform thickness of zero (0.0).
    /// </returns>
    public Thickness BorderThickness
    {
        get => (Thickness)GetValue(BorderThicknessProperty);
        set => SetValueInternal(BorderThicknessProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="BorderBrush"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BorderBrushProperty =
         Block.BorderBrushProperty.AddOwner(
             typeof(ListItem),
             new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender)
             {
                 MethodToUpdateDom2 = static (d, oldValue, newValue) => ((ListItem)d).SetBorderColor(oldValue as Brush, (Brush)newValue),
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

    /// <summary>
    /// Identifies the <see cref="LineHeight"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LineHeightProperty =
        Block.LineHeightProperty.AddOwner(typeof(ListItem));

    /// <summary>
    /// Gets or sets the height of each line of content.
    /// </summary>
    /// <returns>
    /// The height of each line in device independent pixels with a value range of 0.0034 to 160000. A value of 
    /// <see cref="double.NaN"/> (equivalent to an attribute value of "Auto") causes the line height to be 
    /// determined automatically from the current font characteristics. The default is <see cref="double.NaN"/>.
    /// </returns>
    public double LineHeight
    {
        get => (double)GetValue(LineHeightProperty);
        set => SetValueInternal(LineHeightProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="TextAlignment"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TextAlignmentProperty =
        Block.TextAlignmentProperty.AddOwner(typeof(ListItem));

    /// <summary>
    /// Gets or sets a value that indicates the horizontal alignment of text content.
    /// </summary>
    /// <returns>
    /// One of the <see cref="Windows.TextAlignment"/> values that specifies the desired alignment. The default 
    /// is <see cref="TextAlignment.Left"/>.
    /// </returns>
    public TextAlignment TextAlignment
    {
        get => (TextAlignment)GetValue(TextAlignmentProperty);
        set => SetValueInternal(TextAlignmentProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="FlowDirection"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FlowDirectionProperty =
        FrameworkElement.FlowDirectionProperty.AddOwner(typeof(ListItem));

    /// <summary>
    /// Gets or sets the relative direction for flow of content within a <see cref="ListItem"/> element.
    /// </summary>
    /// <returns>
    /// One of the <see cref="Windows.FlowDirection"/> values that specifies the relative flow direction.
    /// The default is <see cref="FlowDirection.LeftToRight"/>.
    /// </returns>
    public FlowDirection FlowDirection
    {
        get => (FlowDirection)GetValue(FlowDirectionProperty);
        set => SetValueInternal(FlowDirectionProperty, value);
    }

    /// <inheritdoc />
    protected sealed override int VisualChildrenCount => Blocks.InternalCount;

    /// <inheritdoc />
    protected sealed override UIElement GetVisualChild(int index)
    {
        if (index >= VisualChildrenCount)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        return Blocks.InternalItems[index];
    }

    internal override bool IsModel
    {
        get => Blocks.IsModel;
        set => Blocks.IsModel = value;
    }

    internal override string TagName => "li";

    /// <inheritdoc />
    protected internal override HtmlElementReference CreateDomElement(HtmlElementReference parent)
    {
        return INTERNAL_HtmlDomManager.CreateListItemDomElementAndAppendIt(parent, this);
    }

    internal sealed override void AttachVisualChildren()
    {
        foreach (var block in Blocks.InternalItems)
        {
            INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(block, this);
        }
    }

    internal sealed override ITextContainer OnCreateTextContainer() => new TextContainerListItem(this);

    private sealed class TextContainerListItem : ITextContainer
    {
        private readonly ListItem _listItem;

        public TextContainerListItem(ListItem listItem)
        {
            Debug.Assert(listItem is not null);
            _listItem = listItem;
        }

        public string Text => string.Join("\n", _listItem.Blocks.Select(block => block.TextContainer.Text));

        public void OnTextContentChanged()
        {
            if (TextContainersHelper.Get(VisualTreeHelper.GetParent(_listItem)) is ITextContainer parent)
            {
                parent.OnTextContentChanged();
            }
        }
    }

}
