
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

#if MIGRATION

namespace System.Windows
{
    public sealed class TextDecorationCollection
    {
        internal TextDecorationCollection() { }

        internal TextDecoration Decoration { get; set; }

        internal string ToHtmlString()
        {
            if (Decoration != null)
            {
                switch (Decoration.Location)
                {
                    case TextDecorationLocation.Underline:
                        return "underline";
                    case TextDecorationLocation.Strikethrough:
                        return "line-through";
                    case TextDecorationLocation.OverLine:
                        return "overline";
                    //case TextDecorationLocation.Baseline:
                    //    break;
                }
            }

            return string.Empty;
        }
    }

    internal sealed class TextDecoration
    {
        internal TextDecoration(TextDecorationLocation location)
        {
            this.Location = location;
        }

        internal TextDecorationLocation Location { get; private set; }
    }
}

#endif
