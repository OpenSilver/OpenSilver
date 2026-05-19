
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

using System.Diagnostics;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Media;

namespace OpenSilver.Internal.Documents;

internal sealed class TextContainerList : ITextContainer
{
    private readonly List _list;

    public TextContainerList(List list)
    {
        Debug.Assert(list is not null);
        _list = list;
    }

    public string Text => string.Join("\n", _list.ListItems.Select(li => li.TextContainer.Text));

    public void OnTextContentChanged()
    {
        if (TextContainersHelper.Get(VisualTreeHelper.GetParent(_list)) is ITextContainer parent)
        {
            parent.OnTextContentChanged();
        }
    }
}

internal sealed class TextContainerListItem : ITextContainer
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
