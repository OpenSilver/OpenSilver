using System.Text.Json.Serialization;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    internal class QuillDelta
    {
        [JsonPropertyName("insert")]
        public string Text { get; set; }

        [JsonPropertyName("attributes")]
        public QuillRangeFormat Attributes { get; set; }
    }

    internal class QuillDeltas
    {
        [JsonPropertyName("ops")]
        public QuillDelta[] Operations { get; set; }
    }
}
