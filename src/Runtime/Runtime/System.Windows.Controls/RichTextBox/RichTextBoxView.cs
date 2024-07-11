
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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Web;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using CSHTML5.Internal;
using OpenSilver.Internal.Media;

namespace OpenSilver.Internal.Controls;

internal sealed class RichTextBoxView : TextViewBase
{
    internal const string FontFamilyName = "font";
    internal const string FontWeightName = "weight";
    internal const string FontStyleName = "style";
    internal const string FontSizeName = "size";
    internal const string FontColorName = "color";
    internal const string LetterSpacingName = "spacing";
    internal const string LineHeightName = "height";
    internal const string TextAlignmentName = "align";
    internal const string TextDecorationName = "decoration";

    private static JsonSerializerOptions SerializerOptions { get; } =
        new JsonSerializerOptions
        {
            IgnoreNullValues = true,
        };

    static RichTextBoxView()
    {
        TextElement.CharacterSpacingProperty.AddOwner(
            typeof(RichTextBoxView),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure)
            {
                MethodToUpdateDom2 = static (d, oldValue, newValue) => ((RichTextBoxView)d).SetCharacterSpacing((int)newValue),
            });

        TextElement.FontFamilyProperty.AddOwner(
            typeof(RichTextBoxView),
            new FrameworkPropertyMetadata(FontFamily.Default, FrameworkPropertyMetadataOptions.Inherits, OnFontFamilyChanged)
            {
                MethodToUpdateDom2 = static (d, oldValue, newValue) => ((RichTextBoxView)d).SetFontFamily((FontFamily)newValue),
            });

        TextElement.FontSizeProperty.AddOwner(
            typeof(RichTextBoxView),
            new FrameworkPropertyMetadata(11d, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure)
            {
                MethodToUpdateDom2 = static (d, oldValue, newValue) => ((RichTextBoxView)d).SetFontSize((double)newValue),
            });

        TextElement.FontStyleProperty.AddOwner(
            typeof(RichTextBoxView),
            new FrameworkPropertyMetadata(FontStyles.Normal, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure)
            {
                MethodToUpdateDom2 = static (d, oldValue, newValue) => ((TextBoxView)d).SetFontStyle((FontStyle)newValue),
            });

        TextElement.FontWeightProperty.AddOwner(
           typeof(RichTextBoxView),
           new FrameworkPropertyMetadata(FontWeights.Normal, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure)
           {
               MethodToUpdateDom2 = static (d, oldValue, newValue) => ((RichTextBoxView)d).SetFontWeight((FontWeight)newValue),
           });

        TextElement.ForegroundProperty.AddOwner(
            typeof(RichTextBoxView),
            new FrameworkPropertyMetadata(
                TextElement.ForegroundProperty.DefaultMetadata.DefaultValue,
                FrameworkPropertyMetadataOptions.Inherits,
                OnForegroundChanged)
            {
                MethodToUpdateDom2 = static (d, oldValue, newValue) => ((RichTextBoxView)d).SetForeground(oldValue as Brush, (Brush)newValue),
            });

        Block.LineHeightProperty.AddOwner(
            typeof(RichTextBoxView),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure)
            {
                MethodToUpdateDom2 = static (d, oldValue, newValue) => ((RichTextBoxView)d).SetLineHeight((double)newValue),
            });

        Block.TextAlignmentProperty.AddOwner(
            typeof(RichTextBoxView),
            new FrameworkPropertyMetadata(TextAlignment.Left, FrameworkPropertyMetadataOptions.Inherits)
            {
                MethodToUpdateDom2 = static (d, oldValue, newValue) => ((RichTextBoxView)d).SetTextAlignment((TextAlignment)newValue),
            });
        IsHitTestableProperty.OverrideMetadata(typeof(RichTextBoxView), new PropertyMetadata(BooleanBoxes.TrueBox));
    }

    private DispatcherOperation _refreshOp;
    private WeakEventListener<RichTextBoxView, Brush, EventArgs> _foregroundChangedListener;

    public RichTextBoxView(RichTextBox rtb)
        : base(rtb)
    {
        IsEnabledChanged += (o, e) => SetEnable(!IsReadOnly);
    }

    internal new RichTextBox Host => (RichTextBox)base.Host;

    internal bool IsReadOnly => !IsEnabled || Host.IsReadOnly;

    public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
    {
        domElementWhereToPlaceChildren = null;
        return INTERNAL_HtmlDomManager.CreateRichTextBoxViewDomElementAndAppendIt((INTERNAL_HtmlDomElementReference)parentRef, this);
    }

    protected sealed internal override void INTERNAL_OnAttachedToVisualTree()
    {
        base.INTERNAL_OnAttachedToVisualTree();

        SetProperties();

        if (FocusManager.GetFocusedElement() == Host)
        {
            InputManager.SetFocusNative(OuterDiv);
        }
    }

    private void SetProperties()
    {
        RichTextBox host = Host;

        this.SetTextDecorations(host.TextDecorations);
        this.SetTextWrapping(host.TextWrapping);
        this.SetCaretColor(host.CaretBrush);

        if (IsReadOnly)
        {
            SetEnable(false);
        }

        SetAcceptsReturn(host.AcceptsReturn);
        SetAcceptsTab(host.AcceptsTab);
        INTERNAL_HtmlDomManager.SetDomElementAttribute(OuterDiv, "spellcheck", host.IsSpellCheckEnabled);

        SetContentsFromBlocks();
    }

    protected override Size MeasureContent(Size constraint)
    {
        double maxWidth = double.IsPositiveInfinity(constraint.Width) ? -1 : constraint.Width;
        double maxHeight = double.IsPositiveInfinity(constraint.Height) ? -1 : constraint.Height;
        string size = Interop.ExecuteJavaScriptString(
            $"document.richTextViewManager.measureView('{OuterDiv.UniqueIdentifier}', {maxWidth.ToInvariantString()}, {maxHeight.ToInvariantString()})");

        int i = size.IndexOf('|');
        string w = size.Substring(0, i);
        string h = size.Substring(i + 1);
        if (double.TryParse(w, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out double width) &&
            double.TryParse(h, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out double height))
        {
            return new Size(width, height);
        }

        return new Size();
    }

    protected internal override void OnInput() => OnContentChanged(true);

    internal void InvalidateUI()
    {
        if (OuterDiv is null)
        {
            return;
        }

        _refreshOp ??= Dispatcher.InvokeAsync(() =>
        {
            _refreshOp = null;
            SetContentsFromBlocks();
        },
        DispatcherPriority.Background);
    }

    internal string GetSelectedText()
    {
        if (OuterDiv is null)
        {
            return string.Empty;
        }

        return Interop.ExecuteJavaScriptString($"document.richTextViewManager.getSelectedText('{OuterDiv.UniqueIdentifier}')");
    }

    internal int GetContentLength()
    {
        if (OuterDiv is null)
        {
            return 0;
        }

        return Interop.ExecuteJavaScriptInt32($"document.richTextViewManager.getContentLength('{OuterDiv.UniqueIdentifier}')");
    }

    internal void SetSelectedText(string text)
    {
        if (OuterDiv is null)
        {
            return;
        }

        Interop.ExecuteJavaScriptVoid(
            $"document.richTextViewManager.setSelectedText('{OuterDiv.UniqueIdentifier}', {HttpUtility.JavaScriptStringEncode(text, true)})");

        OnContentChanged(true);
    }

    internal void SelectAll()
    {
        if (OuterDiv is null)
        {
            return;
        }

        Interop.ExecuteJavaScriptVoid($"document.richTextViewManager.selectAll('{OuterDiv.UniqueIdentifier}')");
    }

    internal void Select(int start, int length)
    {
        if (OuterDiv is null)
        {
            return;
        }

        Interop.ExecuteJavaScriptVoid(
            $"document.richTextViewManager.select('{OuterDiv.UniqueIdentifier}', {start.ToInvariantString()}, {length.ToInvariantString()})");
    }

    internal object GetFormat(DependencyProperty dp)
    {
        if (dp == TextElement.FontFamilyProperty)
        {
            string format = GetFormatNative(FontFamilyName);
            return format switch
            {
                { Length: > 0 } => new FontFamily(format),
                _ => GetValue(TextElement.FontFamilyProperty),
            };
        }
        else if (dp == TextElement.FontWeightProperty)
        {
            string format = GetFormatNative(FontWeightName);
            return format switch
            {
                _ when int.TryParse(format, NumberStyles.Integer, CultureInfo.InvariantCulture, out int weight) => new FontWeight(weight),
                _ => GetValue(TextElement.FontWeightProperty),
            };
        }
        else if (dp == TextElement.FontStyleProperty)
        {
            string format = GetFormatNative(FontStyleName);
            return format switch
            {
                "normal" => FontStyles.Normal,
                "italic" => FontStyles.Italic,
                "oblique" => FontStyles.Oblique,
                _ => GetValue(TextElement.FontStyleProperty),
            };
        }
        else if (dp == Inline.TextDecorationsProperty)
        {
            string format = GetFormatNative(TextDecorationName);
            return format switch
            {
                "underline" => TextDecorations.Underline,
                "line-through" => TextDecorations.Strikethrough,
                "overline" => TextDecorations.OverLine,
                _ => GetValue(Inline.TextDecorationsProperty),
            };
        }
        else if (dp == TextElement.FontSizeProperty)
        {
            string format = GetFormatNative(FontSizeName);
            return format switch
            {
                { Length: > 2 } when double.TryParse(
                    format.Substring(0, format.Length - 2), // Remove 'px'
                    NumberStyles.Float | NumberStyles.AllowThousands,
                    CultureInfo.InvariantCulture,
                    out double fontSize) => fontSize,
                _ => GetValue(TextElement.FontSizeProperty),
            };
        }
        else if (dp == TextElement.ForegroundProperty)
        {
            string format = GetFormatNative(FontColorName);
            return format switch
            {
                _ when TryParseCssColor(format, out Color color) => new SolidColorBrush(color),
                _ => GetValue(TextElement.ForegroundProperty),
            };
        }
        else if (dp == TextElement.CharacterSpacingProperty)
        {
            string format = GetFormatNative(LetterSpacingName);
            return format switch
            {
                "normal" => 0,
                { Length: > 2 } when double.TryParse(
                    format.Substring(0, format.Length - 2), // Remove 'em'
                    NumberStyles.Float | NumberStyles.AllowThousands,
                    CultureInfo.InvariantCulture,
                    out double cSpacing) => (int)(1000 * cSpacing),
                _ => GetValue(TextElement.CharacterSpacingProperty),
            };
        }
        else if (dp == Block.LineHeightProperty)
        {
            string format = GetFormatNative(LetterSpacingName);
            return format switch
            {
                "normal" => 0.0,
                { Length: > 2 } when double.TryParse(
                    format.Substring(0, format.Length - 2), // Remove 'px'
                    NumberStyles.Float | NumberStyles.AllowThousands,
                    CultureInfo.InvariantCulture,
                    out double lineHeight) => lineHeight,
                _ => GetValue(Block.LineHeightProperty),
            };
        }
        else if (dp == Block.TextAlignmentProperty)
        {
            string format = GetFormatNative(TextAlignmentName);
            return format switch
            {
                "start" => TextAlignment.Left,
                "center" => TextAlignment.Center,
                "end" => TextAlignment.Right,
                "justify" => TextAlignment.Justify,
                _ => GetValue(Block.TextAlignmentProperty),
            };
        }
        else if (dp == TextElement.FontStretchProperty)
        {
            // Not implemented
            return Host.FontStretch;
        }

        return DependencyProperty.UnsetValue;
    }

    private string GetFormatNative(string propertyName)
    {
        if (OuterDiv is null)
        {
            return null;
        }

        return Interop.ExecuteJavaScriptString(
            $"document.richTextViewManager.getFormat('{OuterDiv.UniqueIdentifier}', '{propertyName}')");
    }

    internal void Format(DependencyProperty dp, object value)
    {
        if (dp == TextElement.FontFamilyProperty)
        {
            if (value is FontFamily fontFamily)
            {
                FormatNative(FontFamilyName, FontProperties.ToCssFontFamily(fontFamily));
            }
        }
        else if (dp == TextElement.FontWeightProperty)
        {
            if (value is FontWeight fontWeight)
            {
                FormatNative(FontWeightName, FontProperties.ToCssFontWeight(fontWeight));
            }
        }
        else if (dp == TextElement.FontStyleProperty)
        {
            if (value is FontStyle fontStyle)
            {
                FormatNative(FontStyleName, FontProperties.ToCssFontStyle(fontStyle));
            }
        }
        else if (dp == Inline.TextDecorationsProperty)
        {
            var textDecoration = value as TextDecorationCollection;
            FormatNative(TextDecorationName, FontProperties.ToCssTextDecoration(textDecoration));
        }
        else if (dp == TextElement.FontSizeProperty)
        {
            FormatNative(FontSizeName, FontProperties.ToCssPxFontSize(Convert.ToDouble(value, CultureInfo.InvariantCulture)));
        }
        else if (dp == TextElement.ForegroundProperty)
        {
            string cssColor = value switch
            {
                Color color => color.ToHtmlString(1.0),
                SolidColorBrush scb => scb.ToHtmlString(),
                string sColor => Color.Parse(sColor).ToHtmlString(1.0),
                _ => string.Empty,
            };
            FormatNative(FontColorName, cssColor);
        }
        else if (dp == TextElement.CharacterSpacingProperty)
        {
            FormatNative(LetterSpacingName, FontProperties.ToCssLetterSpacing(Convert.ToInt32(value, CultureInfo.InvariantCulture)));
        }
        else if (dp == Block.LineHeightProperty)
        {
            FormatNative(LineHeightName, FontProperties.ToCssLineHeight(Convert.ToDouble(value, CultureInfo.InvariantCulture)));
        }
        else if (dp == Block.TextAlignmentProperty)
        {
            if (value is TextAlignment textAlignment)
            {
                FormatNative(TextAlignmentName, FontProperties.ToCssTextAlignment(textAlignment));
            }
        }
        else if (dp == TextElement.FontStretchProperty)
        {
            // Not implemented
        }
    }

    private void FormatNative(string property, string value)
    {
        if (OuterDiv is null)
        {
            return;
        }

        Interop.ExecuteJavaScriptVoid(
            $"document.richTextViewManager.format('{OuterDiv.UniqueIdentifier}', '{property}', {HttpUtility.JavaScriptStringEncode(value, true)})");

        OnContentChanged(true);
    }

    internal string GetXaml() => GetXaml(GetContents());

    internal string GetXaml(int start, int length) => GetXaml(GetContents(start, length));

    private string GetXaml(QuillDelta[] contents)
    {
        var deltas = RemoveTrailingLineBreak(contents);

        if (deltas.Length == 0)
        {
            return string.Empty;
        }

        var xaml = new XmlDocument();
        xaml.LoadXml("<Section xml:space=\"preserve\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"></Section>");

        var parser = new QuillContentParser(deltas);

        while (parser.MoveToNextBlock())
        {
            xaml.DocumentElement.AppendChild(CreateParagraph(xaml, parser.BlockFormat, parser.Inlines));
        }

        return xaml.OuterXml;

        static Span<QuillDelta> RemoveTrailingLineBreak(Span<QuillDelta> deltas)
        {
            if (deltas.Length > 0)
            {
                ref QuillDelta delta = ref deltas[deltas.Length - 1];

                if (delta.Text == "\n" && !delta.Attributes.HasValue)
                {
                    return deltas.Slice(0, deltas.Length - 1);
                }

                if (delta.Text.EndsWith("\n"))
                {
                    delta.Text = delta.Text.Substring(0, delta.Text.Length - 1);
                }
            }

            return deltas;
        }
    }

    private XmlElement CreateParagraph(XmlDocument document, QuillRangeFormat format, IEnumerable<QuillDelta> deltas)
    {
        var paragraph = document.CreateElement(nameof(Paragraph), document.DocumentElement.NamespaceURI);
        paragraph.SetAttribute(nameof(Block.TextAlignment), format.TextAlignment switch
        {
            "center" => nameof(TextAlignment.Center),
            "end" => nameof(TextAlignment.Right),
            "justify" => nameof(TextAlignment.Justify),
            _ => nameof(TextAlignment.Left),
        });
        paragraph.SetAttribute(nameof(Block.LineHeight), format.LineHeight switch
        {
            "normal" or "" or null => "0",
            _ => format.LineHeight.Substring(0, format.LineHeight.Length - 2), // Remove 'px'
        });

        foreach (QuillDelta d in deltas)
        {
            if (!string.IsNullOrEmpty(d.Text))
            {
                paragraph.AppendChild(CreateRun(document, d));
            }
        }

        return paragraph;
    }

    private XmlElement CreateRun(XmlDocument document, QuillDelta delta)
    {
        var run = document.CreateElement(nameof(Run), document.DocumentElement.NamespaceURI);

        run.SetAttribute(nameof(Run.Text), delta.Text);

        QuillRangeFormat format = delta.Attributes ?? default;
        run.SetAttribute(nameof(TextElement.FontFamily), format.FontFamily switch
        {
            null or "" => ((FontFamily)GetValue(TextElement.FontFamilyProperty)).Source,
            _ => format.FontFamily,
        });

        run.SetAttribute(nameof(TextElement.FontWeight), format.FontWeight switch
        {
            _ when int.TryParse(
                format.FontWeight,
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out int weight) && FontWeights.FontWeightToString(weight, out string fontWeight) => fontWeight,
            _ => ((FontWeight)GetValue(TextElement.FontWeightProperty)).ToString(),
        });

        run.SetAttribute(nameof(TextElement.FontStyle), format.FontStyle switch
        {
            "normal" => nameof(FontStyles.Normal),
            "italic" => nameof(FontStyles.Italic),
            "oblique" => nameof(FontStyles.Oblique),
            _ => ((FontStyle)GetValue(TextElement.FontStyleProperty)).ToString(),
        });

        run.SetAttribute(nameof(TextElement.FontSize), format.FontSize switch
        {
            null or "" => ((double)GetValue(TextElement.FontSizeProperty)).ToInvariantString(),
            _ => format.FontSize.Substring(0, format.FontSize.Length - 2), // Remove 'px'
        });

        run.SetAttribute(nameof(TextElement.Foreground), format.Foreground switch
        {
            string when TryParseCssColor(format.Foreground, out Color color) => color.ToString(CultureInfo.InvariantCulture),
            _ => (Brush)GetValue(TextElement.ForegroundProperty) switch
            {
                SolidColorBrush scb => scb.GetColorWithOpacity().ToString(CultureInfo.InvariantCulture),
                _ => "Black",
            },
        });

        run.SetAttribute(nameof(TextElement.CharacterSpacing), format.CharacterSpacing switch
        {
            "normal" => "0",
            { Length: > 2 } when double.TryParse(
                format.CharacterSpacing.Substring(0, format.CharacterSpacing.Length - 2), // Remove 'em'
                NumberStyles.Float | NumberStyles.AllowThousands,
                CultureInfo.InvariantCulture,
                out double cSpacing) => ((int)(1000 * cSpacing)).ToInvariantString(),
            _ => ((int)GetValue(TextElement.CharacterSpacingProperty)).ToInvariantString(),
        });

        run.SetAttribute(nameof(Inline.TextDecorations), format.TextDecorations switch
        {
            "underline" => nameof(TextDecorations.Underline),
            "line-through" => nameof(TextDecorations.Strikethrough),
            "overline" => nameof(TextDecorations.OverLine),
            "none" => "None",
            _ => (TextDecorationCollection)GetValue(Inline.TextDecorationsProperty) switch
            {
                null => "None",
                TextDecorationCollection decoration => decoration.Location switch
                {
                    TextDecorationLocation.Underline => nameof(TextDecorations.Underline),
                    TextDecorationLocation.Strikethrough => nameof(TextDecorations.Strikethrough),
                    TextDecorationLocation.OverLine => nameof(TextDecorations.OverLine),
                    _ => "None",
                },
            }
        });

        return run;
    }

    internal static bool TryParseCssColor(string cssColor, out Color color)
    {
        if (cssColor.StartsWith("rgb("))
        {
            string[] rgb = cssColor.Substring(4, cssColor.Length - 5).Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (rgb.Length == 3)
            {
                color = Color.FromRgb(
                    byte.Parse(rgb[0], CultureInfo.InvariantCulture),
                    byte.Parse(rgb[1], CultureInfo.InvariantCulture),
                    byte.Parse(rgb[2], CultureInfo.InvariantCulture));
                return true;
            }
        }
        else if (cssColor.StartsWith("rgba("))
        {
            string[] rgba = cssColor.Substring(5, cssColor.Length - 6).Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (rgba.Length == 4)
            {
                color = Color.FromArgb(
                    (byte)(255 * double.Parse(rgba[3], CultureInfo.InvariantCulture)),
                    byte.Parse(rgba[0], CultureInfo.InvariantCulture),
                    byte.Parse(rgba[1], CultureInfo.InvariantCulture),
                    byte.Parse(rgba[2], CultureInfo.InvariantCulture));
                return true;
            }
        }

        color = default;
        return false;
    }

    internal QuillDelta[] GetContents()
    {
        if (OuterDiv is null)
        {
            return Array.Empty<QuillDelta>();
        }

        return Interop.ExecuteJavaScriptString($"document.richTextViewManager.getContents('{OuterDiv.UniqueIdentifier}')") switch
        {
            "" or null => Array.Empty<QuillDelta>(),
            string contents => JsonSerializer.Deserialize<QuillDelta[]>(contents, SerializerOptions),
        };
    }

    private QuillDelta[] GetContents(int start, int length)
    {
        if (OuterDiv is null)
        {
            return Array.Empty<QuillDelta>();
        }

        return Interop.ExecuteJavaScriptString($"document.richTextViewManager.getContents('{OuterDiv.UniqueIdentifier}', {start.ToInvariantString()}, {length.ToInvariantString()})") switch
        {
            "" or null => Array.Empty<QuillDelta>(),
            string contents => JsonSerializer.Deserialize<QuillDelta[]>(contents, SerializerOptions),
        };
    }

    internal void SetEnable(bool value)
    {
        if (OuterDiv is null)
        {
            return;
        }

        Interop.ExecuteJavaScriptVoid(
            $"document.richTextViewManager.enable('{OuterDiv.UniqueIdentifier}', {(value ? "true" : "false")})");
    }

    internal void SetContentsFromBlocks()
    {
        if (OuterDiv is null)
        {
            return;
        }

        QuillDelta[] deltas = GetDeltas(Host.GetBlocksCache()).ToArray();

        Interop.ExecuteJavaScriptVoid(
            $"document.richTextViewManager.setContents('{OuterDiv.UniqueIdentifier}', {JsonSerializer.Serialize(deltas, SerializerOptions)})");

        OnContentChanged(false);
    }

    internal void UpdateContentsFromTextElement(TextElement element, int start, int length)
    {
        if (OuterDiv is null)
        {
            return;
        }

        QuillDelta[] deltas = GetDeltas(start, length, element).ToArray();

        Interop.ExecuteJavaScriptVoid(
            $"document.richTextViewManager.updateContents('{OuterDiv.UniqueIdentifier}', {JsonSerializer.Serialize(deltas, SerializerOptions)})");

        OnContentChanged(true);

        static IEnumerable<QuillDelta> GetDeltas(int start, int length, TextElement element)
        {
            yield return new QuillDelta { Retain = start, };
            if (length > 0)
            {
                yield return new QuillDelta { Delete = length, };
            }

            switch (element)
            {
                case Inline inline:
                    foreach (QuillDelta delta in RichTextBoxView.GetDeltas(inline))
                    {
                        yield return delta;
                    }
                    break;

                case Block block:
                    foreach (QuillDelta delta in RichTextBoxView.GetDeltas(block))
                    {
                        yield return delta;
                    }
                    break;
            }
        }
    }

    private static IEnumerable<QuillDelta> GetDeltas(BlockCollection blocks)
    {
        foreach (Block block in blocks.InternalItems)
        {
            foreach (QuillDelta delta in GetDeltas(block))
            {
                yield return delta;
            }
        }
    }

    private static IEnumerable<QuillDelta> GetDeltas(InlineCollection inlines)
    {
        foreach (Inline inline in inlines.InternalItems)
        {
            foreach (QuillDelta delta in GetDeltas(inline))
            {
                yield return delta;
            }
        }
    }

    private static IEnumerable<QuillDelta> GetDeltas(Block block)
    {
        return block switch
        {
            Section section when section.Blocks.Count > 0 => GetDeltas(section.Blocks),
            Section section => GetDeltas(section.Blocks).Append(CloseBlock(section)),
            Paragraph paragraph => GetDeltas(paragraph.Inlines).Append(CloseBlock(paragraph)),
            _ => Enumerable.Empty<QuillDelta>(),
        };

        static QuillDelta CloseBlock(Block block)
        {
            return new QuillDelta
            {
                Text = "\n",
                Attributes = new QuillRangeFormat
                {
                    TextAlignment = FontProperties.ToCssTextAlignment(block.TextAlignment),
                    LineHeight = FontProperties.ToCssLineHeight(block.LineHeight),
                },
            };
        }
    }

    private static IEnumerable<QuillDelta> GetDeltas(Inline inline)
    {
        switch (inline)
        {
            case Run run when !string.IsNullOrEmpty(run.Text):
                yield return new QuillDelta
                {
                    Text = run.Text,
                    Attributes = new QuillRangeFormat
                    {
                        FontFamily = FontProperties.ToCssFontFamily(run.FontFamily),
                        FontWeight = FontProperties.ToCssFontWeight(run.FontWeight),
                        FontStyle = FontProperties.ToCssFontStyle(run.FontStyle),
                        FontSize = FontProperties.ToCssPxFontSize(run.FontSize),
                        Foreground = (run.Foreground as SolidColorBrush)?.ToHtmlString(),
                        CharacterSpacing = FontProperties.ToCssLetterSpacing(run.CharacterSpacing),
                        TextDecorations = FontProperties.ToCssTextDecoration(run.TextDecorations),
                    },
                };
                break;

            case Span span:
                foreach (QuillDelta delta in GetDeltas(span.Inlines))
                {
                    yield return delta;
                }
                break;

            case LineBreak:
                yield return new QuillDelta { Text = "\n" };
                break;
        }
    }

    internal void ProcessKeyDown(KeyEventArgs e)
    {
        if (OuterDiv is null) return;

        if (RichTextViewManager.Instance.OnKeyDown(this, e))
        {
            e.Handled = true;
            e.Cancellable = false;
        }
    }

    internal void OnIsReadOnlyChanged() => SetEnable(!IsReadOnly);

    internal void OnIsSpellCheckEnabledChanged(bool isSpellCheckEnabled)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && OuterDiv is not null)
        {
            INTERNAL_HtmlDomManager.SetDomElementAttribute(OuterDiv, "spellcheck", isSpellCheckEnabled);
        }
    }

    internal void OnTextWrappingChanged(TextWrapping textWrapping)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && OuterDiv is not null)
        {
            this.SetTextWrapping(textWrapping);
        }
    }

    internal void SetCaretBrush(Brush brush)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && OuterDiv is not null)
        {
            this.SetCaretColor(brush);
        }
    }

    internal void OnAcceptsReturnChanged(bool acceptsReturn)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && OuterDiv is not null)
        {
            SetAcceptsReturn(acceptsReturn);
        }
    }

    private void SetAcceptsReturn(bool value) =>
        Interop.ExecuteJavaScriptVoidAsync($"document.richTextViewManager.setAcceptsReturn('{OuterDiv.UniqueIdentifier}', '{(value ? "true" : "false")}')");

    internal void OnAcceptsTabChanged(bool acceptsTab)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && OuterDiv is not null)
        {
            SetAcceptsTab(acceptsTab);
        }
    }

    private void SetAcceptsTab(bool value) =>
        Interop.ExecuteJavaScriptVoidAsync($"document.richTextViewManager.setAcceptsTab('{OuterDiv.UniqueIdentifier}', '{(value ? "true" : "false")}')");

    private static void OnFontFamilyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        UIElementHelpers.InvalidateMeasureOnFontFamilyChanged((RichTextBoxView)d, (FontFamily)e.NewValue);
    }

    private static void OnForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var view = (RichTextBoxView)d;

        if (view._foregroundChangedListener != null)
        {
            view._foregroundChangedListener.Detach();
            view._foregroundChangedListener = null;
        }

        if (e.NewValue is Brush newBrush)
        {
            view._foregroundChangedListener = new(view, newBrush)
            {
                OnEventAction = static (instance, sender, args) => instance.OnForegroundChanged(sender, args),
                OnDetachAction = static (listener, source) => source.Changed -= listener.OnEvent,
            };
            newBrush.Changed += view._foregroundChangedListener.OnEvent;
        }
    }

    private void OnForegroundChanged(object sender, EventArgs e)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
        {
            var foreground = (Brush)sender;
            this.SetForeground(foreground, foreground);
        }
    }

    private void OnContentChanged(bool invalidateModel)
    {
        InvalidateMeasure();
        if (invalidateModel)
        {
            Host.InvalidateModel();
        }
        Host.OnContentChanged();
    }
}
