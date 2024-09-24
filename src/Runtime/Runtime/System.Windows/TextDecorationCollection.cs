
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

namespace System.Windows
{
    public sealed class TextDecorationCollection
    {
        internal TextDecorationCollection(TextDecorationLocation location)
        {
            Location = location;
        }

        internal TextDecorationLocation Location { get; }

        internal string ToHtmlString()
        {
            return Location switch
            {
                TextDecorationLocation.Underline => "underline",
                TextDecorationLocation.Strikethrough => "line-through",
                TextDecorationLocation.Overline => "overline",
                _ => "none",
            };
        }

        internal static string ToString(TextDecorationCollection textDecoration)
        {
            if (textDecoration is null)
            {
                return "None";
            }

            return textDecoration.Location switch
            {
                TextDecorationLocation.Underline => nameof(TextDecorationLocation.Underline),
                TextDecorationLocation.Strikethrough => nameof(TextDecorationLocation.Strikethrough),
                TextDecorationLocation.Overline => nameof(TextDecorationLocation.Overline),
                _ => string.Empty,
            };
        }
    }
}
