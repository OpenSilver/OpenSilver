
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

using System.Windows.Markup;
using CSHTML5.Internal;
using OpenSilver.Internal;

namespace System.Windows.Documents;

/// <summary>
/// Provides a block-level content element that is used to group content into a paragraph.
/// </summary>
[ContentProperty(nameof(Inlines))]
public sealed class Paragraph : Block
{
    private InlineCollection _inlines;

    static Paragraph()
    {
        LineHeightProperty.OverrideMetadata(
            typeof(Paragraph),
            new FrameworkPropertyMetadata(
                0.0,
                FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure,
                OnPropertyChanged)
            {
                MethodToUpdateDom2 = static (d, oldValue, newValue) => ((Paragraph)d).SetLineHeight((double)newValue),
            });

        LineStackingStrategyProperty.OverrideMetadata(
            typeof(Paragraph),
            new FrameworkPropertyMetadata(
                LineStackingStrategy.MaxHeight,
                FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure)
            {
                MethodToUpdateDom2 = static (d, oldValue, newValue) => ((Paragraph)d).SetLineStackingStrategy((LineStackingStrategy)newValue),
            });

        TextAlignmentProperty.OverrideMetadata(
            typeof(Paragraph),
            new FrameworkPropertyMetadata(
                TextAlignment.Left,
                FrameworkPropertyMetadataOptions.Inherits,
                OnPropertyChanged)
            {
                MethodToUpdateDom2 = static (d, oldValue, newValue) => ((Paragraph)d).SetTextAlignment((TextAlignment)newValue),
            });
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Paragraph"/> class.
    /// </summary>
    public Paragraph()
    {
        SetValueInternal(InlinesProperty, new InlineCollection(this));
    }

    internal override string TagName => "section";

    private static readonly DependencyProperty InlinesProperty =
        DependencyProperty.Register(
            nameof(Inlines),
            typeof(InlineCollection),
            typeof(Paragraph),
            new PropertyMetadata(null, OnInlinesChanged));

    /// <summary>
    /// Gets an <see cref="InlineCollection"/> containing the top-level <see cref="Inline"/>
    /// elements that include the contents of the <see cref="Paragraph"/>.
    /// </summary>
    public InlineCollection Inlines => _inlines;

    private static void OnInlinesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((Paragraph)d)._inlines = (InlineCollection)e.NewValue;
    }

    private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((Block)d).TextContainer.OnTextContentChanged();
    }

    protected internal override void INTERNAL_OnAttachedToVisualTree()
    {
        base.INTERNAL_OnAttachedToVisualTree();
        foreach (var inline in Inlines.InternalItems)
        {
            INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(inline, this);
        }
    }

    internal sealed override bool IsModel
    {
        get => _inlines.IsModel;
        set => _inlines.IsModel = value;
    }

    internal sealed override int VisualChildrenCount => Inlines.Count;

    internal sealed override UIElement GetVisualChild(int index)
    {
        if (index >= VisualChildrenCount)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        return Inlines[index];
    }
}
