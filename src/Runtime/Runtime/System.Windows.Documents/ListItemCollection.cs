
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

namespace System.Windows.Documents;

/// <summary>
/// Represents a collection of <see cref="ListItem"/> elements. <see cref="ListItemCollection"/>
/// defines the allowable child content of a <see cref="List"/> element.
/// </summary>
public class ListItemCollection : TextElementCollection<ListItem>
{
    internal ListItemCollection(List owner, ITextContainer textContainer)
        : base(owner, textContainer)
    {
    }

    /// <summary>
    /// Gets the first <see cref="ListItem"/> element within this instance of <see cref="ListItemCollection"/>.
    /// </summary>
    /// <returns>
    /// The first <see cref="ListItem"/> element within this instance of <see cref="ListItemCollection"/>.
    /// </returns>
    public ListItem FirstListItem => InternalCount > 0 ? InternalItems[0] : null;

    /// <summary>
    /// Gets the last <see cref="ListItem"/> element within this instance of <see cref="ListItemCollection"/>.
    /// </summary>
    /// <returns>
    /// The last <see cref="ListItem"/> element within this instance of <see cref="ListItemCollection"/>.
    /// </returns>
    public ListItem LastListItem => InternalCount > 0 ? InternalItems[InternalCount - 1] : null;
}
