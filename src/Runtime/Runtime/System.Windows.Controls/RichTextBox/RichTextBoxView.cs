
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

using CSHTML5.Internal;
using OpenSilver.Internal.Media;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;

namespace OpenSilver.Internal.Controls;

internal sealed class RichTextBoxView : TextViewBase
{
    internal const string FontFamilyName = "font";
    internal const string FontWeightName = "weight";
    internal const string FontStyleName = "style";
    internal const string FontSizeName = "size";
    internal const string FontColorName = "color";
    internal const string LetterSpacingName = "spacing";
    internal const string LineHeightName = "lineheight";
    internal const string TextAlignmentName = "align";
    internal const string TextDecorationName = "decoration";

    private static readonly string[] _newLineSeparators = ["\r\n", "\n", "\r"];

    private static JsonSerializerOptions SerializerOptions { get; } =
        new JsonSerializerOptions
        {
            IgnoreNullValues = true,
        };

    static RichTextBoxView()
    {
        IsEnabledProperty.OverrideMetadata(typeof(RichTextBoxView), new PropertyMetadata(OnIsEnabledChanged));

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
    }

    private DispatcherOperation _refreshOp;

    public RichTextBoxView(RichTextBox rtb)
        : base(rtb)
    {
    }

    internal new RichTextBox Host => (RichTextBox)base.Host;

    internal bool IsReadOnly => !IsEnabled || Host.IsReadOnly;

    /// <inheritdoc />
    protected internal override HtmlElementReference CreateDomElement(HtmlElementReference parent)
    {
        return INTERNAL_HtmlDomManager.CreateRichTextBoxViewDomElementAndAppendIt(parent, this);
    }

    protected internal sealed override void INTERNAL_OnAttachedToVisualTree()
    {
        base.INTERNAL_OnAttachedToVisualTree();

        SetProperties();

        if (Host.IsKeyboardFocused)
        {
            InputManager.SetFocusNative(OuterDiv);
        }
    }

    protected internal sealed override void INTERNAL_OnDetachedFromVisualTree()
    {
        base.INTERNAL_OnDetachedFromVisualTree();

        Host.Synchronize();

        Interop.ExecuteJavaScriptVoidAsync($"osjs.richTextViewManager.deleteView('{OuterDiv.Uid}')");
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
        OuterDiv.SetAttribute("spellcheck", host.IsSpellCheckEnabled);

        SetContentsFromBlocks();
    }

    protected override Size MeasureContent(Size constraint)
    {
        double maxWidth = double.IsPositiveInfinity(constraint.Width) ? -1 : constraint.Width;
        double maxHeight = double.IsPositiveInfinity(constraint.Height) ? -1 : constraint.Height;
        string size = Interop.ExecuteJavaScriptString(
            $"osjs.richTextViewManager.measureView('{OuterDiv.Uid}', {maxWidth.ToInvariantString()}, {maxHeight.ToInvariantString()})");

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
        if (!OuterDiv.IsConnected)
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
        if (!OuterDiv.IsConnected)
        {
            return string.Empty;
        }

        return Interop.ExecuteJavaScriptString($"osjs.richTextViewManager.getSelectedText('{OuterDiv.Uid}')");
    }

    internal int GetContentLength()
    {
        if (!OuterDiv.IsConnected)
        {
            return 0;
        }

        return Interop.ExecuteJavaScriptInt32($"osjs.richTextViewManager.getContentLength('{OuterDiv.Uid}')");
    }

    internal void SetSelectedText(string text)
    {
        if (!OuterDiv.IsConnected)
        {
            return;
        }

        Interop.ExecuteJavaScriptVoid(
            $"osjs.richTextViewManager.setSelectedText('{OuterDiv.Uid}', {HttpUtility.JavaScriptStringEncode(text, true)})");

        OnContentChanged(true);
    }

    internal void SelectAll()
    {
        if (!OuterDiv.IsConnected)
        {
            return;
        }

        Interop.ExecuteJavaScriptVoid($"osjs.richTextViewManager.selectAll('{OuterDiv.Uid}')");
    }

    internal void Select(int start, int length)
    {
        if (!OuterDiv.IsConnected)
        {
            return;
        }

        Interop.ExecuteJavaScriptVoid(
            $"osjs.richTextViewManager.select('{OuterDiv.Uid}', {start.ToInvariantString()}, {length.ToInvariantString()})");
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
                "none" => null,
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
        if (!OuterDiv.IsConnected)
        {
            return null;
        }

        return Interop.ExecuteJavaScriptString(
            $"osjs.richTextViewManager.getFormat('{OuterDiv.Uid}', '{propertyName}')");
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
        if (!OuterDiv.IsConnected)
        {
            return;
        }

        Interop.ExecuteJavaScriptVoid(
            $"osjs.richTextViewManager.format('{OuterDiv.Uid}', '{property}', {HttpUtility.JavaScriptStringEncode(value, true)})");

        OnContentChanged(true);
    }

    internal string GetXaml() => GetXaml(GetContents());

    internal string GetXaml(int start, int length) => GetXaml(GetContents(start, length));

    private string GetXaml(QuillDelta[] deltas)
    {
        var parser = new QuillContentParser(deltas);

        if (!parser.MoveToNextBlock())
        {
            return string.Empty;
        }

        var xaml = new XmlDocument();
        xaml.LoadXml("<Section xml:space=\"preserve\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"></Section>");

        bool done;
        bool isEmpty = true;

        do
        {
            var p = CreateParagraph(xaml, parser.BlockFormat, parser.Inlines);

            done = !parser.MoveToNextBlock();

            if (!done || !p.IsEmpty)
            {
                // SL drops the last paragraph if it is empty.
                xaml.DocumentElement.AppendChild(p.Paragraph);
                isEmpty = false;
            }
        }
        while (!done);

        return isEmpty ? string.Empty : xaml.OuterXml;
    }

    private (XmlElement Paragraph, bool IsEmpty) CreateParagraph(XmlDocument document, QuillRangeFormat format, IEnumerable<QuillDelta> deltas)
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

        bool isEmpty = true;

        foreach (QuillDelta d in deltas)
        {
            if (!string.IsNullOrEmpty(d.Text))
            {
                isEmpty = false;
                paragraph.AppendChild(CreateRun(document, d));
            }
        }

        return (paragraph, isEmpty);
    }

    private XmlElement CreateRun(XmlDocument document, QuillDelta delta)
    {
        var run = document.CreateElement(nameof(Run), document.DocumentElement.NamespaceURI);

        run.SetAttribute(nameof(Run.Text), delta.Text);

        QuillRangeFormat format = delta.Attributes ?? default;
        run.SetAttribute(nameof(TextElement.FontFamily), format.FontFamily switch
        {
            null or "" => ((FontFamily)GetValue(TextElement.FontFamilyProperty)).Source,
            string s when s == FontFace.DefaultCssFontFamily => FontFamily.Default.Source,
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
            string[] rgb = cssColor.Substring(4, cssColor.Length - 5).Split([','], StringSplitOptions.RemoveEmptyEntries);
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
            string[] rgba = cssColor.Substring(5, cssColor.Length - 6).Split([','], StringSplitOptions.RemoveEmptyEntries);
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
        if (!OuterDiv.IsConnected)
        {
            return [];
        }

        return Interop.ExecuteJavaScriptString($"osjs.richTextViewManager.getContents('{OuterDiv.Uid}')") switch
        {
            "" or null => [],
            string contents => JsonSerializer.Deserialize<QuillDelta[]>(contents, SerializerOptions),
        };
    }

    private QuillDelta[] GetContents(int start, int length)
    {
        if (!OuterDiv.IsConnected)
        {
            return [];
        }

        return Interop.ExecuteJavaScriptString($"osjs.richTextViewManager.getContents('{OuterDiv.Uid}', {start.ToInvariantString()}, {length.ToInvariantString()})") switch
        {
            "" or null => [],
            string contents => JsonSerializer.Deserialize<QuillDelta[]>(contents, SerializerOptions),
        };
    }

    internal void SetEnable(bool value)
    {
        if (!OuterDiv.IsConnected)
        {
            return;
        }

        Interop.ExecuteJavaScriptVoid(
            $"osjs.richTextViewManager.enable('{OuterDiv.Uid}', {(value ? "true" : "false")})");
    }

    internal void SetContentsFromBlocks()
    {
        if (!OuterDiv.IsConnected)
        {
            return;
        }

        var deltas = new QuillDeltaBuilder()
            .AddBlocks(Host.InternalBlocks)
            .GetDeltas();

        Interop.ExecuteJavaScriptVoid(
            $"osjs.richTextViewManager.setContents('{OuterDiv.Uid}', {JsonSerializer.Serialize(deltas, SerializerOptions)})");

        OnContentChanged(false);
    }

    internal void UpdateContentsFromTextElement(TextElement element, int start, int length)
    {
        if (!OuterDiv.IsConnected)
        {
            return;
        }

        var deltas = new QuillDeltaBuilder()
            .Retain(start)
            .Delete(length)
            .Add(element)
            .GetDeltas();

        Interop.ExecuteJavaScriptVoid(
            $"osjs.richTextViewManager.updateContents('{OuterDiv.Uid}', {JsonSerializer.Serialize(deltas, SerializerOptions)})");

        OnContentChanged(true);
    }

    internal void ProcessKeyDown(KeyEventArgs e)
    {
        if (!OuterDiv.IsConnected) return;

        if (RichTextViewManager.OnKeyDown(this, e))
        {
            e.Handled = true;
            e.Cancellable = false;
        }
    }

    internal void OnIsReadOnlyChanged() => SetEnable(!IsReadOnly);

    private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var richTextBoxView = (RichTextBoxView)d;
        richTextBoxView.SetEnable(!richTextBoxView.IsReadOnly);
    }

    internal void OnIsSpellCheckEnabledChanged(bool isSpellCheckEnabled)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && OuterDiv.IsConnected)
        {
            OuterDiv.SetAttribute("spellcheck", isSpellCheckEnabled);
        }
    }

    internal void OnTextWrappingChanged(TextWrapping textWrapping)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && OuterDiv.IsConnected)
        {
            this.SetTextWrapping(textWrapping);
        }
    }

    internal void SetCaretBrush(Brush brush)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && OuterDiv.IsConnected)
        {
            this.SetCaretColor(brush);
        }
    }

    internal void OnAcceptsReturnChanged(bool acceptsReturn)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && OuterDiv.IsConnected)
        {
            SetAcceptsReturn(acceptsReturn);
        }
    }

    private void SetAcceptsReturn(bool value) =>
        Interop.ExecuteJavaScriptVoidAsync($"osjs.richTextViewManager.setAcceptsReturn('{OuterDiv.Uid}', '{(value ? "true" : "false")}')");

    internal void OnAcceptsTabChanged(bool acceptsTab)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && OuterDiv.IsConnected)
        {
            SetAcceptsTab(acceptsTab);
        }
    }

    private void SetAcceptsTab(bool value) =>
        Interop.ExecuteJavaScriptVoidAsync($"osjs.richTextViewManager.setAcceptsTab('{OuterDiv.Uid}', '{(value ? "true" : "false")}')");

    private void OnContentChanged(bool invalidateModel)
    {
        InvalidateMeasure();
        if (invalidateModel)
        {
            Host.InvalidateModel();
        }
        Host.OnContentChanged();
    }

    private ref struct QuillDeltaBuilder
    {
        private readonly List<QuillDelta> _builder;
        private QuillRangeFormat? _format;

        public QuillDeltaBuilder()
        {
            _builder = [];
        }

        public readonly List<QuillDelta> GetDeltas() => _builder;

        public QuillDeltaBuilder Retain(int offset)
        {
            _builder.Add(new QuillDelta { Retain = offset });
            return this;
        }

        public QuillDeltaBuilder Delete(int length)
        {
            if (length > 0)
            {
                _builder.Add(new QuillDelta { Delete = length });
            }
            return this;
        }

        public QuillDeltaBuilder Add(TextElement element)
        {
            switch (element)
            {
                case Inline inline:
                    _format = GetBlockFormat(inline);
                    AddInline(inline);
                    break;

                case Block block:
                    _format = GetBlockFormat(block);
                    AddBlock(block);
                    break;
            }

            return this;
        }

        public QuillDeltaBuilder AddBlocks(BlockCollection blocks)
        {
            foreach (Block block in blocks.InternalItems)
            {
                (QuillRangeFormat? oldFormat, _format) = (_format, GetBlockFormat(block));
                AddBlock(block);
                _format = oldFormat;
            }

            return this;
        }

        private void AddBlock(Block block)
        {
            switch (block)
            {
                case Section section:
                    if (section.Blocks.InternalCount > 0)
                    {
                        AddBlocks(section.Blocks);
                        break;
                    }
                    _builder.Add(EndOfParagraph());
                    break;

                case Paragraph paragraph:
                    AddInlines(paragraph.Inlines);
                    _builder.Add(EndOfParagraph());
                    break;
            }
        }

        private void AddInlines(InlineCollection inlines)
        {
            foreach (Inline inline in inlines.InternalItems)
            {
                AddInline(inline);
            }
        }

        private void AddInline(Inline inline)
        {
            switch (inline)
            {
                case Run run when !string.IsNullOrEmpty(run.Text):
                    string[] lines = run.Text.Split(_newLineSeparators, StringSplitOptions.None);

                    if (!string.IsNullOrEmpty(lines[0]))
                    {
                        _builder.Add(GetTextDelta(lines[0], run));
                    }

                    for (int i = 1; i < lines.Length; i++)
                    {
                        _builder.Add(EndOfParagraph());

                        if (!string.IsNullOrEmpty(lines[i]))
                        {
                            _builder.Add(GetTextDelta(lines[i], run));
                        }
                    }
                    break;

                case Span span:
                    AddInlines(span.Inlines);
                    break;

                case LineBreak:
                    _builder.Add(EndOfParagraph());
                    break;

                case InlineImageContainer image:
                    _builder.Add(new QuillDelta
                    {
                        Image = new QuillImage { ImageData = image.GetImageData() },
                        Attributes = new QuillRangeFormat
                        {
                            Width = double.IsNaN(image.Width) ? string.Empty : image.Width.ToInvariantString(),
                            Height = double.IsNaN(image.Height) ? string.Empty : image.Height.ToInvariantString(),
                            OriginalSource = image.GetOriginalSource(),
                            ObjectFit = InlineImageContainer.ConvertStretch(image.Stretch),
                        },
                    });
                    break;
            }
        }

        private QuillDelta EndOfParagraph()
        {
            return new QuillDelta
            {
                Text = "\n",
                Attributes = _format,
            };
        }

        private static QuillRangeFormat? GetBlockFormat(TextElement textElement)
        {
            return new QuillRangeFormat
            {
                TextAlignment = FontProperties.ToCssTextAlignment(Block.GetTextAlignment(textElement)),
                LineHeight = FontProperties.ToCssLineHeight(Block.GetLineHeight(textElement)),
            };
        }

        private static QuillDelta GetTextDelta(string text, Run run)
        {
            return new QuillDelta
            {
                Text = text,
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
        }
    }
}
