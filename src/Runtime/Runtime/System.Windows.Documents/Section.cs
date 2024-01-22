
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

namespace System.Windows.Documents;

/// <summary>
/// A block-level element used for grouping other <see cref="Block"/>
/// elements.
/// </summary>
[ContentProperty(nameof(Blocks))]
[OpenSilver.NotImplemented]
public sealed class Section : Block
{
    public Section()
    {
        SetValueInternal(BlocksProperty, new BlockCollection(this));
    }

    internal override string TagName => "section";

    private static readonly DependencyProperty BlocksProperty =
        DependencyProperty.Register(
            nameof(Blocks),
            typeof(BlockCollection),
            typeof(Section),
            null);

    /// <summary>
    /// Gets a <see cref="BlockCollection"/> containing the top-level <see cref="Block"/>
    /// elements that comprise the contents of the <see cref="Section"/>.
    /// This property has no default value.
    /// </summary>
    public BlockCollection Blocks => (BlockCollection)GetValue(BlocksProperty);

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

    protected internal override void INTERNAL_OnAttachedToVisualTree()
    {
        base.INTERNAL_OnAttachedToVisualTree();
        foreach (var block in Blocks)
        {
            INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(block, this);
        }
    }
}
