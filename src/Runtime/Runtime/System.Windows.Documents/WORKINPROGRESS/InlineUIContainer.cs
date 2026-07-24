
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
using System.Windows.Markup;
using System.Windows.Media;

namespace System.Windows.Documents;

// This class is not finished because there is an issue displaying the element "inline" instead of on a new line.
/// <summary>
/// Provides an inline content element that enables UIElement types to be embedded in the content of a (Rich)TextBlock.
/// </summary>
[ContentProperty(nameof(Child))]
public sealed class InlineUIContainer : Inline
{
    private UIElement _child;

    /// <summary>
    /// Initializes a new, empty instance of the <see cref="InlineUIContainer"/> class.
    /// </summary>
    public InlineUIContainer()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InlineUIContainer"/> class, taking a specified 
    /// <see cref="UIElement"/> object as the initial contents of the new <see cref="InlineUIContainer"/>.
    /// </summary>
    /// <param name="child">
    /// An <see cref="UIElement"/> object specifying the initial contents of the new <see cref="InlineUIContainer"/>.
    /// </param>
    public InlineUIContainer(UIElement child)
    {
        Child = child;
    }

    /// <summary>
    /// Gets or sets the <see cref="UIElement"/> hosted by the <see cref="InlineUIContainer"/>.
    /// </summary>
    /// <returns>
    /// The <see cref="UIElement"/> hosted by the <see cref="InlineUIContainer"/>.
    /// </returns>
    public UIElement Child
    {
        get
        {
            return _child;
        }
        set
        {
            if (IsLoadedCache)
            {
                INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(_child, this);
                INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(value, this);
            }
            _child = value;
        }
    }

    internal sealed override void AttachVisualChildren() => INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(_child, this);

    internal override ITextContainer OnCreateTextContainer() => new TextContainerInlineUIContainer(this);

    private sealed class TextContainerInlineUIContainer : ITextContainer
    {
        private readonly InlineUIContainer _uiContainer;

        internal TextContainerInlineUIContainer(InlineUIContainer uiContainer)
        {
            _uiContainer = uiContainer;
        }

        public string Text => string.Empty;

        public void OnTextContentChanged()
        {
            if (TextContainersHelper.Get(VisualTreeHelper.GetParent(_uiContainer)) is ITextContainer parent)
            {
                parent.OnTextContentChanged();
            }
        }
    }
}
