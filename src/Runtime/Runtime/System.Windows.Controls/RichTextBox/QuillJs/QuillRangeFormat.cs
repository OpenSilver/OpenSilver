using System.Text.Json.Serialization;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    internal class QuillRangeFormat
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
