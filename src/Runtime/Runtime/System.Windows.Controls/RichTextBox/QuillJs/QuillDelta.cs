
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

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenSilver.Internal.Controls;

[JsonConverter(typeof(QuillDeltaJsonConverter))]
internal struct QuillDelta
{
    public string Text { get; set; }

    public QuillImage? Image { get; set; }

    public int? Retain { get; set; }

    public int? Delete { get; set; }

    public QuillRangeFormat? Attributes { get; set; }
}

internal struct QuillImage
{
    [JsonPropertyName("image")]
    public string ImageData { get; set; }
}

internal struct QuillRangeFormat
{
    // Images properties

    [JsonPropertyName("width")]
    public string Width { get; set; }

    [JsonPropertyName("height")]
    public string Height { get; set; }

    [JsonPropertyName("data-originalsrc")]
    public string OriginalSource { get; set; }

    [JsonPropertyName("data-objectfit")]
    public string ObjectFit { get; set; }

    // Text Properties

    [JsonPropertyName(RichTextBoxView.FontFamilyName)]
    public string FontFamily { get; set; }

    [JsonPropertyName(RichTextBoxView.FontWeightName)]
    public string FontWeight { get; set; }

    [JsonPropertyName(RichTextBoxView.FontStyleName)]
    public string FontStyle { get; set; }

    [JsonPropertyName(RichTextBoxView.FontSizeName)]
    public string FontSize { get; set; }

    [JsonPropertyName(RichTextBoxView.FontColorName)]
    public string Foreground { get; set; }

    [JsonPropertyName(RichTextBoxView.LetterSpacingName)]
    public string CharacterSpacing { get; set; }

    [JsonPropertyName(RichTextBoxView.TextDecorationName)]
    public string TextDecorations { get; set; }

    // Block properties

    [JsonPropertyName(RichTextBoxView.LineHeightName)]
    public string LineHeight { get; set; }

    [JsonPropertyName(RichTextBoxView.TextAlignmentName)]
    public string TextAlignment { get; set; }
}

internal sealed class QuillDeltaJsonConverter : JsonConverter<QuillDelta>
{
    private const string Insert = "insert";
    private const string Retain = "retain";
    private const string Delete = "delete";
    private const string Attributes = "attributes";

    public override QuillDelta Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var delta = new QuillDelta();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString();
                reader.Read();

                switch (propertyName)
                {
                    case Insert:
                        if (reader.TokenType == JsonTokenType.String)
                        {
                            delta.Text = reader.GetString();
                        }
                        else if (reader.TokenType == JsonTokenType.StartObject)
                        {
                            delta.Image = JsonSerializer.Deserialize<QuillImage>(ref reader, options);
                        }
                        break;

                    case Retain:
                        delta.Retain = reader.GetInt32();
                        break;

                    case Delete:
                        delta.Delete = reader.GetInt32();
                        break;

                    case Attributes:
                        delta.Attributes = JsonSerializer.Deserialize<QuillRangeFormat>(ref reader, options);
                        break;

                    default:
                        reader.Skip();
                        break;
                }
            }
        }

        return delta;
    }

    public override void Write(Utf8JsonWriter writer, QuillDelta value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        if (value.Text != null)
        {
            writer.WritePropertyName(Insert);
            writer.WriteStringValue(value.Text);
        }
        else if (value.Image.HasValue)
        {
            writer.WritePropertyName(Insert);
            JsonSerializer.Serialize(writer, value.Image.Value, options);
        }
        if (value.Retain.HasValue)
        {
            writer.WritePropertyName(Retain);
            writer.WriteNumberValue(value.Retain.Value);
        }
        else if (value.Delete.HasValue)
        {
            writer.WritePropertyName(Delete);
            writer.WriteNumberValue(value.Delete.Value);
        }

        if (value.Attributes.HasValue)
        {
            writer.WritePropertyName(Attributes);
            JsonSerializer.Serialize(writer, value.Attributes.Value, options);
        }

        writer.WriteEndObject();
    }
}
