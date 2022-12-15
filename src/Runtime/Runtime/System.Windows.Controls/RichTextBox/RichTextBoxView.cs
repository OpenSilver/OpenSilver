
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
using System.Linq;
using System.Text.Json;
using System.Web;
using System.Windows.Input;
using System.Xml;
using CSHTML5.Internal;
using OpenSilver.Internal.Controls;
using OpenSilver.Internal;

#if MIGRATION
using System.Windows.Documents;
using System.Windows.Media;
#else
using Windows.Foundation;
using Windows.UI.Text;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    internal sealed class RichTextBoxView : FrameworkElement, ITextBoxView
    {
        private static bool _quillJSInitialized;

        private object _quill;

        public RichTextBoxView(RichTextBox rtb)
        {
            Host = rtb ?? throw new ArgumentNullException(nameof(rtb));
            Unloaded += (o, e) => _quill = null;
            Cursor = Cursors.IBeam;
        }

        internal RichTextBox Host { get; }

        internal override bool EnablePointerEventsCore => true;

        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            EnsureInitialized();

            domElementWhereToPlaceChildren = null;

            object div = INTERNAL_HtmlDomManager.CreateDomElementAndAppendIt("div", parentRef, this);
            object quillContainer = INTERNAL_HtmlDomManager.CreateDomElementAndAppendIt("div", div, this);

            string script = "let options = {"
                + $"readOnly: {(Host.IsReadOnly ? "true" : "false")},modules: {{clipboard: {{matchVisual:false}}}}"
                + "};"
                + "const quill = new Quill($0, options);"
                + "quill.on('selection-change', function(range, oldRange, source) { if (source === 'user' || source === 'api') { $1(); }});"
                + "quill.on('text-change', function(delta, oldDelta, source) { if (source === 'user' || source === 'api') { $2(); }});"
                + "quill;";

            _quill = OpenSilver.Interop.ExecuteJavaScript(script,
                quillContainer,
                new Action(() => Host.RaiseSelectionChanged()),
                new Action(() => Host.RaiseContentChanged()));

            return div;
        }

        protected override Size MeasureOverride(Size availableSize) => INTERNAL_GetActualWidthAndHeight();

        internal sealed override NativeEventsManager CreateEventsManager()
        {
            return new NativeEventsManager(this, this, Host, true);
        }

        internal QuillRange GetSelection()
        {
            if (_quill == null)
            {
                return null;
            }

            object range = OpenSilver.Interop.ExecuteJavaScript(@"$0.focus();
let range = $0.getSelection(true);
if (range) { JSON.stringify({ start: range.index, length: range.length }); }
else { JSON.stringify({ start: 0, length: 0 }); }", _quill);

            return JsonSerializer.Deserialize<QuillRange>(Convert.ToString(range));
        }

        internal string GetText(int start, int length)
        {
            if (_quill == null)
                return string.Empty;

            var result = OpenSilver.Interop.ExecuteJavaScript($"$0.getText({start}, {length})", _quill);
            return result.ToString();
        }

        internal string GetText()
        {
            if (_quill == null)
                return string.Empty;

            var result = OpenSilver.Interop.ExecuteJavaScript("$0.getText()", _quill);
            var content = result.ToString();
            if (content == "\n") content = string.Empty;
            return content;
        }

        internal void SetText(int start, int length, string text)
        {
            if (_quill == null)
                return;

            OpenSilver.Interop.ExecuteJavaScript(
                $"$0.deleteText({start}, {length}, 'api'); $0.insertText({start}, {HttpUtility.JavaScriptStringEncode(text, true)});",
                _quill);
        }

        internal void SetText(string text)
        {
            if (_quill == null)
                return;

            OpenSilver.Interop.ExecuteJavaScript(
                $"$0.setText({HttpUtility.JavaScriptStringEncode(text, true)}, 'api')",
                _quill);
        }

        internal void SetText(string text, IDictionary<string, object> formats)
        {
            if (_quill == null)
                return;

            OpenSilver.Interop.ExecuteJavaScript(
                $"$0.insertText($0.getLength() - 1,{HttpUtility.JavaScriptStringEncode(text, true)},{JsonSerializer.Serialize(formats)}, 'api')",
                _quill);
        }

        internal void InsertText(string text)
        {
            if (_quill == null)
                return;

            OpenSilver.Interop.ExecuteJavaScript(
                $"$0.insertText($0.getLength(), {HttpUtility.JavaScriptStringEncode(text, true)}, 'api')",
                _quill);
        }

        internal void SelectAll()
        {
            if (_quill == null)
                return;

            OpenSilver.Interop.ExecuteJavaScript(
                "$0.setSelection(0, $0.getLength());",
                _quill);
        }

        internal object GetPropertyValue(DependencyProperty prop, int start, int length)
        {
            var result = OpenSilver.Interop.ExecuteJavaScript("JSON.stringify($0.getFormat($1,$2))", _quill, start, length);
            var format = JsonSerializer.Deserialize<QuillRangeFormat>(result.ToString());

            if (prop == Control.FontWeightProperty)
            {
                return format.Bold ? FontWeights.Bold : FontWeights.Normal;
            }
            else if (prop == Control.FontStyleProperty)
            {
#if MIGRATION
                return format.Italic ? FontStyles.Italic : FontStyles.Normal;
#else
                return format.Italic ? FontStyle.Italic : FontStyle.Normal;
#endif
            }
            else if (prop == Inline.TextDecorationsProperty)
            {
#if MIGRATION
                return format.Underline ? TextDecorations.Underline : null;
#else
                return format.Underline ? TextDecorations.Underline : TextDecorations.None;
#endif
            }
            else if (prop == Control.FontFamilyProperty)
            {
                if (string.IsNullOrEmpty(format.FontName))
                    return null;

                string fontName = string.Join(" ", format.FontName.Split(new[] { '-' }));
                return new FontFamily(fontName);
            }
            else if (prop == Control.FontSizeProperty)
            {
                if (string.IsNullOrEmpty(format.FontSize))
                    return null;

                return int.Parse(format.FontSize.Replace("px", ""));
            }
            else if (prop == Control.ForegroundProperty)
            {
                if (format.Color == null)
                    return null;

                //Quill return color in 2 ways: single string , for example "#ff0000", or string array (["#ff0000","#ff00AA"])
                if (format.Color is string)
                {
                    return new SolidColorBrush(Color.INTERNAL_ConvertFromString(format.Color.ToString()));
                }
                else if (format.Color is string[] colors)
                {
                    return new SolidColorBrush(Color.INTERNAL_ConvertFromString(colors[0]));
                }

                return null;
            }
            else if (prop == Block.LineHeightProperty)
            {
                return null;//not supported yet
            }

            return null;
        }

        internal void SetPropertyValue(DependencyProperty prop, object value, int start, int length)
        {
            if (prop == Control.FontWeightProperty)
            {
                if (value is FontWeight fontWeight)
                {
                    bool isBold = fontWeight == FontWeights.Bold || fontWeight == FontWeights.SemiBold || fontWeight == FontWeights.DemiBold
                        || fontWeight == FontWeights.ExtraBold || fontWeight == FontWeights.UltraBold;
                    OpenSilver.Interop.ExecuteJavaScript("$0.formatText($1,$2,'bold',$3)", _quill, start, length, isBold);
                }
            }
            else if (prop == Control.FontStyleProperty)
            {
                if (value is FontStyle fontStyle)
                {
#if MIGRATION
                    bool isItalic = fontStyle == FontStyles.Italic;
#else
                    bool isItalic = fontStyle == FontStyle.Italic;
#endif
                    OpenSilver.Interop.ExecuteJavaScript("$0.formatText($1,$2,'italic',$3)", _quill, start, length, isItalic);
                }
            }
            else if (prop == Inline.TextDecorationsProperty)
            {
#if MIGRATION
                if (value is TextDecorationCollection textDecoration)
                {
                    string format = "";
                    if (textDecoration == TextDecorations.Underline)
                        format = "underline";
                    else if (textDecoration == TextDecorations.Strikethrough)
                        format = "strike";

                    OpenSilver.Interop.ExecuteJavaScript("$0.formatText($1,$2,$3,true)", _quill, start, length, format);
                }
                else if (value == null)
                {
                    OpenSilver.Interop.ExecuteJavaScript("$0.formatText($1,$2,'underline',false)", _quill, start, length);
                    OpenSilver.Interop.ExecuteJavaScript("$0.formatText($1,$2,'strike',false)", _quill, start, length);
                }
#endif
            }
            else if (prop == Control.FontFamilyProperty)
            {
                if (value is FontFamily fontFamily)
                {
                    OpenSilver.Interop.ExecuteJavaScript("$0.formatText($1,$2,'font',$3)", _quill, start, length, fontFamily?.ToString().Replace(" ", "-").ToLower());
                }
            }
            else if (prop == Control.FontSizeProperty)
            {
                OpenSilver.Interop.ExecuteJavaScript("$0.formatText($1,$2,'size', $3)", _quill, start, length, value + "px");
            }
            else if (prop == Control.ForegroundProperty)
            {
                if (value is SolidColorBrush brush)
                {
                    OpenSilver.Interop.ExecuteJavaScript("$0.formatText($1,$2,'color',$3)", _quill, start, length, brush.ConvertToCSSValue());
                }
            }
            else if (prop == Block.LineHeightProperty)
            {
                OpenSilver.Interop.ExecuteJavaScript("$0.formatText($1,$2,'line-height',$3)", _quill, start, length, value);
            }
        }

        internal string GetContents()
        {
            if (_quill == null)
                return null;

            var content = OpenSilver.Interop.ExecuteJavaScript("JSON.stringify($0.getContents())", _quill);
            return GetXamlContents(content.ToString());
        }

        internal string GetContents(int start, int length)
        {
            if (_quill == null)
                return null;

            var content = OpenSilver.Interop.ExecuteJavaScript($"JSON.stringify($0.getContents({start}, {length}))", _quill);
            return GetXamlContents(content.ToString());
        }

        internal void Clear()
        {
            if (_quill == null)
                return;

            OpenSilver.Interop.ExecuteJavaScript("$0.setText('','api');", _quill);
        }

        internal void SetReadOnly(bool value)
        {
            if (_quill == null)
                return;

            OpenSilver.Interop.ExecuteJavaScript(
                $"$0.root.setAttribute('contenteditable', '{(value ? "false" : "true")}')",
                _quill);
        }

        internal void SetEnable(bool value)
        {
            if (_quill == null)
                return;

            OpenSilver.Interop.ExecuteJavaScript(
                $"$0.enable({(value ? "true" : "false")})",
                _quill);
        }

        private string GetFontName(string fontName)
        {
            var names = fontName.Split('-');
            return string.Join(" ", names);
        }

        private string GetXamlContents(string content)
        {
            if (string.IsNullOrEmpty(content))
                return null;

            var deltas = JsonSerializer.Deserialize<QuillDeltas>(content.ToString());
            if (deltas == null)
                return null;

            var xaml = new XmlDocument();
            xaml.LoadXml("<Section xml:space=\"preserve\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><Paragraph/></Section>");
            var paragraph = xaml.DocumentElement.FirstChild;

            foreach (var delta in deltas.Operations)
            {
                var run = xaml.CreateElement("Run", xaml.DocumentElement.NamespaceURI);
                run.InnerText = delta.Text.TrimEnd('\n');

                if (delta.Attributes != null)
                {
                    if (delta.Attributes.Bold)
                        run.SetAttribute("FontWeight", "Bold");
                    if (delta.Attributes.Italic)
                        run.SetAttribute("FontStyle", "Italic");
                    if (delta.Attributes.Underline)
                        run.SetAttribute("TextDecorations", "Underline");
                    //if (!string.IsNullOrEmpty(delta.Attributes.FontName))
                    //    run.SetAttribute("FontFamily", GetFontName(delta.Attributes.FontName));

                    run.SetAttribute("FontFamily", !string.IsNullOrEmpty(delta.Attributes.FontName) ? GetFontName(delta.Attributes.FontName) : INTERNAL_FontsHelper.DefaultCssFontFamily);
                    if (!string.IsNullOrEmpty(delta.Attributes.FontSize))
                        run.SetAttribute("FontSize", delta.Attributes.FontSize.Replace("px", ""));
                    if (delta.Attributes.Color != null)
                    {
                        run.SetAttribute("Foreground", delta.Attributes.Color.ToString());
                    }
                }
                else
                {
                    run.SetAttribute("FontFamily", INTERNAL_FontsHelper.DefaultCssFontFamily);
                }

                paragraph.AppendChild(run);
            }

            return xaml.OuterXml;
        }

        private static void EnsureInitialized()
        {
            if (!_quillJSInitialized)
            {
                _quillJSInitialized = true;

                OpenSilver.Interop.ExecuteJavaScriptVoid(
                    $"let font = Quill.import('attributors/style/font');font.whitelist=[{GetSupportedFonts()}];Quill.register(font,true);");
                OpenSilver.Interop.ExecuteJavaScriptVoid(
                    $"let size = Quill.import('attributors/style/size');size.whitelist=[{GetSupportedFontSizes()}];Quill.register(size,true);");
            }
        }

        private static string GetSupportedFontSizes()
        {
            List<string> sizes = new List<string>();
            for (int i = 8; i <= 72; i++)
            {
                sizes.Add($"'{i}px'");
            }

            return string.Join(",", sizes);
        }

        private static string GetSupportedFonts()
        {
            //TODO: get full supported fonts?
            List<FontFamily> supportedFonts = new List<FontFamily>()
            {
                new FontFamily("Arial"),
                new FontFamily("Courier New"),
                new FontFamily("Microsoft Sans Serif"),
                new FontFamily("MS Gothic"),
                new FontFamily("Symbol"),
                new FontFamily("Tahoma"),
                new FontFamily("Times New Roman"),
                new FontFamily("Traditional Arabic"),
                new FontFamily("Verdana"),
                new FontFamily("Webdings"),
                new FontFamily("Wingdings"),
            };

            return string.Join(",", supportedFonts.Select(f => $"'{f.ToString().ToLower().Replace(" ", "-")}'"));
        }
    }
}
