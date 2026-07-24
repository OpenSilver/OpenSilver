
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

using OpenSilver.Internal.Documents;
using System.Collections;

namespace System.Windows.Documents;

/// <summary>
/// Represents a collection of <see cref="Inline"/> elements.
/// </summary>
public class InlineCollection : TextElementCollection<Inline>, IList
{
    internal InlineCollection(UIElement owner, ITextContainer textContainer)
        : base(owner, textContainer)
    {
    }

    /// <summary>
    /// Gets the first <see cref="Inline"/> element within this instance of <see cref="InlineCollection"/>.
    /// </summary>
    /// <returns>
    /// The first <see cref="Inline"/> element within this instance of <see cref="InlineCollection"/>.
    /// </returns>
    public Inline FirstInline => InternalCount > 0 ? InternalItems[0] : null;

    /// <summary>
    /// Gets the last <see cref="Inline"/> element within this instance of <see cref="InlineCollection"/>.
    /// </summary>
    /// <returns>
    /// The last <see cref="Inline"/> element within this instance of <see cref="InlineCollection"/>.
    /// </returns>
    public Inline LastInline => InternalCount > 0 ? InternalItems[InternalCount - 1] : null;

    /// <summary>
    /// Adds an implicit <see cref="Run"/> element with the given text, supplied as a <see cref="string"/>.
    /// </summary>
    /// <param name="text">
    /// Text set as the <see cref="Run.Text"/> property for the implicit <see cref="Run"/>.
    /// </param>
    public void Add(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        Add(new Run(text));
    }

    int IList.Add(object value)
    {
        Inline inline = value switch
        {
            string text => new Run(text ?? string.Empty),
            Inline i => i,
            UIElement element => new InlineUIContainer(element),
            _ => null,
        };

        Add(inline);
        return InternalCount;
    }
}
