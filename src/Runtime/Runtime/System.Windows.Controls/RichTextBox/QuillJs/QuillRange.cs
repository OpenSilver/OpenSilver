using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    internal class QuillRange
    {
        [JsonPropertyName("start")]
        public int Start { get; set; }

        [JsonPropertyName("length")]
        public int Length { get; set; }

    }
}
