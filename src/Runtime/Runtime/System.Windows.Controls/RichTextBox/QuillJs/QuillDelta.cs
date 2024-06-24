
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

namespace OpenSilver.Internal.Controls;

internal struct QuillDelta
{
    [JsonPropertyName("insert")]
    public string Text { get; set; }

    [JsonPropertyName("retain")]
    public int? Retain { get; set; }

    [JsonPropertyName("delete")]
    public int? Delete { get; set; }

    [JsonPropertyName("attributes")]
    public QuillRangeFormat? Attributes { get; set; }
}

internal struct QuillRangeFormat
{
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

    [JsonPropertyName(RichTextBoxView.LineHeightName)]
    public string LineHeight { get; set; }

    [JsonPropertyName(RichTextBoxView.TextAlignmentName)]
    public string TextAlignment { get; set; }

    [JsonPropertyName(RichTextBoxView.TextDecorationName)]
    public string TextDecorations { get; set; }
}
