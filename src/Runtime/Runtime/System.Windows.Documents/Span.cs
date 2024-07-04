
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

using System.Text;
using System.Windows.Markup;
using CSHTML5.Internal;

namespace System.Windows.Documents;

/// <summary>
/// Groups other <see cref="Inline"/> content elements.
/// </summary>
[ContentProperty(nameof(Inlines))]
public class Span : Inline
{
    private InlineCollection _inlines;

    /// <summary>
    /// Initializes a new instance of the <see cref="Span"/> class.
    /// </summary>
    public Span()
    {
        SetValueInternal(InlinesProperty, new InlineCollection(this));
    }

    private static readonly DependencyProperty InlinesProperty =
        DependencyProperty.Register(
            nameof(Inlines),
            typeof(InlineCollection),
            typeof(Span),
            new PropertyMetadata(null, OnInlinesChanged));

    /// <summary>
    /// Gets an <see cref="InlineCollection"/> containing the top-level inline
    /// elements that include the contents of <see cref="Span"/>.
    /// </summary>
    /// <returns>
    /// An <see cref="InlineCollection"/> containing the inline elements that
    /// include the contents of the <see cref="Span"/>. This property has
    /// no default value.
    /// </returns>
    public InlineCollection Inlines => _inlines;

    private static void OnInlinesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((Span)d)._inlines = (InlineCollection)e.NewValue;
    }

    protected internal override void INTERNAL_OnAttachedToVisualTree()
    {
        base.INTERNAL_OnAttachedToVisualTree();

        foreach (Inline child in Inlines.InternalItems)
        {
            INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(child, this);
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

        return Inlines.InternalItems[index];
    }

    internal sealed override void AppendHtml(StringBuilder builder)
    {
        builder.Append($"<{TagName} class=\"opensilver-textelement\">");
        foreach (Inline inline in Inlines.InternalItems)
        {
            inline.AppendHtml(builder);
        }
        builder.Append($"</{TagName}>");
    }
}
