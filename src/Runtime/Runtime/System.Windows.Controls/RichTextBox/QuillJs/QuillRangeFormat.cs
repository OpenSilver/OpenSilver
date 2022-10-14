
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

using System.Text.Json.Serialization;

namespace OpenSilver.Internal.Controls
{
    internal sealed class QuillRangeFormat
    {
        [JsonPropertyName("bold")]
        public bool Bold { get; set; }

        [JsonPropertyName("italic")]
        public bool Italic { get; set; }

        [JsonPropertyName("underline")]
        public bool Underline { get; set; }

        [JsonPropertyName("font")]
        public string FontName { get; set; }

        [JsonPropertyName("size")]
        public string FontSize { get; set; }

        [JsonPropertyName("color")]
        public object Color { get; set; }
    }
}
