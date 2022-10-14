
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
    internal sealed class QuillDelta
    {
        [JsonPropertyName("insert")]
        public string Text { get; set; }

        [JsonPropertyName("attributes")]
        public QuillRangeFormat Attributes { get; set; }
    }

    internal sealed class QuillDeltas
    {
        [JsonPropertyName("ops")]
        public QuillDelta[] Operations { get; set; }
    }
}
