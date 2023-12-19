
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

using System.Collections;

namespace System.Windows.Documents;

/// <summary>
/// Represents a collection of <see cref="Inline"/> elements.
/// </summary>
public class InlineCollection : TextElementCollection<Inline>, IList
{
    internal InlineCollection(UIElement owner)
        : base(owner)
    {
    }

    public void Add(string text)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        Add(new Run { Text = text });
    }

    int IList.Add(object value)
    {
        Inline inline = value switch
        {
            string text => new Run { Text = text ?? string.Empty },
            Inline i => i,
            _ => null,
        };

        Add(inline);
        return Count;
    }
}
