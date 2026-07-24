
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
using OpenSilver.Internal.Documents;
using System.Diagnostics;
using System.Linq;
using System.Windows.Markup;
using System.Windows.Media;

namespace System.Windows.Documents;

/// <summary>
/// A block-level element used for grouping other <see cref="Block"/>
/// elements.
/// </summary>
[ContentProperty(nameof(Blocks))]
public sealed class Section : Block
{
    private BlockCollection _blocks;

    public Section()
    {
        SetValueInternal(BlocksProperty, new BlockCollection(this, TextContainer));
    }

    internal override string TagName => "section";

    private static readonly DependencyProperty BlocksProperty =
        DependencyProperty.Register(
            nameof(Blocks),
            typeof(BlockCollection),
            typeof(Section),
            new PropertyMetadata(null, OnBlocksChanged));

    /// <summary>
    /// Gets a <see cref="BlockCollection"/> containing the top-level <see cref="Block"/>
    /// elements that comprise the contents of the <see cref="Section"/>.
    /// This property has no default value.
    /// </summary>
    public BlockCollection Blocks => _blocks;

    private static void OnBlocksChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((Section)d)._blocks = (BlockCollection)e.NewValue;
    }

    /// <summary>
    /// Gets or sets a value that indicates whether a trailing paragraph break should
    /// be inserted after the last paragraph when copying the contents of a root <see cref="Section"/>
    /// element to the clipboard.
    /// </summary>
    /// <returns>
    /// true if a trailing paragraph break should be included; otherwise false.
    /// </returns>
    [OpenSilver.NotImplemented]
    public bool HasTrailingParagraphBreakOnPaste { get; set; }

    internal sealed override void AttachVisualChildren()
    {
        foreach (var block in Blocks.InternalItems)
        {
            INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(block, this);
        }
    }

    internal override ITextContainer OnCreateTextContainer() => new TextContainerSection(this);

    internal sealed override bool IsModel
    {
        get => _blocks.IsModel;
        set => _blocks.IsModel = value;
    }

    protected sealed override int VisualChildrenCount => Blocks.InternalCount;

    protected sealed override UIElement GetVisualChild(int index)
    {
        if (index >= VisualChildrenCount)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        return Blocks.InternalItems[index];
    }

    private sealed class TextContainerSection : ITextContainer
    {
        private readonly Section _section;

        public TextContainerSection(Section section)
        {
            Debug.Assert(section is not null);
            _section = section;
        }

        public string Text => string.Join("\n", _section.Blocks.InternalItems.Select(b => b.TextContainer.Text));

        public void OnTextContentChanged()
        {
            if (TextContainersHelper.Get(VisualTreeHelper.GetParent(_section)) is ITextContainer parent)
            {
                parent.OnTextContentChanged();
            }
        }
    }
}
