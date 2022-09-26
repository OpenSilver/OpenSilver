using CSHTML5.Internal;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Web;
using System.Xml;
#if MIGRATION
using System.Windows.Documents;
using System.Windows.Media;
namespace System.Windows.Controls
#else
using Windows.UI.Text;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
namespace Windows.UI.Xaml.Controls
#endif
{
    internal class QuillJsPresenter : IRichTextBoxPresenter
    {
        private RichTextBox _parent;
        private object _instance;

        public QuillJsPresenter(RichTextBox parent)
        {
            _parent = parent;
        }

        public TextSelection Selection
        {
            get
            {
                if (_instance == null)
                    return null;

                string script = @"$0.focus();let range = $0.getSelection(true);
                                 if(range){
                                    JSON.stringify({
                                        start: range.index,
                                        length: range.length
                                    });
                                 }
                                else{
                                        JSON.stringify({
                                        start: 0,
                                        length: 0
                                    });}";
                var range = OpenSilver.Interop.ExecuteJavaScript(script, _instance);
                if(range != null && !OpenSilver.Interop.IsNull(range) && !OpenSilver.Interop.IsUndefined(range))
                {
                    var quillRange = JsonSerializer.Deserialize<QuillRange>(range.ToString());
                    var selection = new TextSelection(this, quillRange.Start, quillRange.Length);

                    return selection;
                }

                return null;
            }
        }

        public void Init()
        {
            LoadExtensions();
            CreateInstance();
            RegisterEvents();
        }

        public string GetText(int start, int length)
        {
            if (_instance == null)
                return string.Empty;

            string script = $"$0.getText({start}, {length})";
            var result = OpenSilver.Interop.ExecuteJavaScript(script, _instance);
            return result?.ToString();
        }

        public string GetText()
        {
            if (_instance == null)
                return string.Empty;

            string script = "$0.getText()";
            var result = OpenSilver.Interop.ExecuteJavaScript(script, _instance);
            var content = result?.ToString();
            if (content == "\n") content = string.Empty;
            return content;

        }

        public void SetText(int start, int length, string text)
        {
            if (_instance == null)
                return;

            string script = $"$0.deleteText({start}, {length}, 'api');"
                + $"$0.insertText({start}, {HttpUtility.JavaScriptStringEncode(text, true)});";
            OpenSilver.Interop.ExecuteJavaScript(script, _instance);
        }

        public void SetText(string text)
        {
            if (_instance == null)
                return;

            string script = $"$0.setText({HttpUtility.JavaScriptStringEncode(text, true)}, 'api')";
            OpenSilver.Interop.ExecuteJavaScript(script, _instance);
        }

        public void SetText(string text, IDictionary<string, object> formats)
        {
            if (_instance == null)
                return;

            string script = $"$0.insertText($0.getLength() - 1,{HttpUtility.JavaScriptStringEncode(text, true)},{JsonSerializer.Serialize(formats)}, 'api')";
            OpenSilver.Interop.ExecuteJavaScript(script, _instance);
        }
        
        public void InsertText(string text)
        {
            if (_instance == null)
                return;

            string script = $"$0.insertText($0.getLength(), {HttpUtility.JavaScriptStringEncode(text, true)}, 'api')";
            OpenSilver.Interop.ExecuteJavaScript(script, _instance);
        }

        public void SelectAll()
        {
            if (_instance == null)
                return;

            string script = "$0.setSelection(0, $0.getLength());";
            OpenSilver.Interop.ExecuteJavaScript(script, _instance);
        }

        public object GetPropertyValue(DependencyProperty prop, int start, int length)
        {
            var result = OpenSilver.Interop.ExecuteJavaScript("JSON.stringify($0.getFormat($1,$2))", _instance, start, length);
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
                    return new SolidColorBrush((Color)Color.INTERNAL_ConvertFromString(format.Color.ToString()));
                }
                else if (format.Color is string[] colors)
                {
                    return new SolidColorBrush((Color)Color.INTERNAL_ConvertFromString(colors[0]));
                }

                return null;
            }
            else if (prop == Block.LineHeightProperty)
            {
                return null;//not supported yet
            }

            return null;
        }

        public void SetPropertyValue(DependencyProperty prop, object value, int start, int length)
        {
            if (prop == Control.FontWeightProperty)
            {
                if (value is FontWeight fontWeight)
                {
                    bool isBold = fontWeight == FontWeights.Bold || fontWeight == FontWeights.SemiBold || fontWeight == FontWeights.DemiBold
                        || fontWeight == FontWeights.ExtraBold || fontWeight == FontWeights.UltraBold;
                    OpenSilver.Interop.ExecuteJavaScript("$0.formatText($1,$2,'bold',$3)", _instance, start, length, isBold);
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
                    OpenSilver.Interop.ExecuteJavaScript("$0.formatText($1,$2,'italic',$3)", _instance, start, length, isItalic);
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

                    OpenSilver.Interop.ExecuteJavaScript("$0.formatText($1,$2,$3,true)", _instance, start, length, format);
                }
                else if (value == null)
                {
                    OpenSilver.Interop.ExecuteJavaScript("$0.formatText($1,$2,'underline',false)", _instance, start, length);
                    OpenSilver.Interop.ExecuteJavaScript("$0.formatText($1,$2,'strike',false)", _instance, start, length);
                }
#endif
            }
            else if (prop == Control.FontFamilyProperty)
            {
                if (value is FontFamily fontFamily)
                {
                    OpenSilver.Interop.ExecuteJavaScript("$0.formatText($1,$2,'font',$3)", _instance, start, length, fontFamily?.ToString().Replace(" ", "-").ToLower());
                }
            }
            else if (prop == Control.FontSizeProperty)
            {
                OpenSilver.Interop.ExecuteJavaScript("$0.formatText($1,$2,'size', $3)", _instance, start, length, value + "px");
            }
            else if (prop == Control.ForegroundProperty)
            {
                if (value is SolidColorBrush brush)
                {
                    OpenSilver.Interop.ExecuteJavaScript("$0.formatText($1,$2,'color',$3)", _instance, start, length, brush.ConvertToCSSValue());
                }
            }
            else if (prop == Block.LineHeightProperty)
            {
                OpenSilver.Interop.ExecuteJavaScript("$0.formatText($1,$2,'line-height',$3)", _instance, start, length, value);
            }
        }

        public string GetContents()
        {
            if (_instance != null)
            {
                var content = OpenSilver.Interop.ExecuteJavaScript("JSON.stringify($0.getContents())", _instance);
                return GetXamlContents(content?.ToString());
            }
            return null;
        }

        public string GetContents(int start, int length)
        {
            var content = OpenSilver.Interop.ExecuteJavaScript($"JSON.stringify($0.getContents({start}, {length}))", _instance);
            return GetXamlContents(content?.ToString());
        }

        public void Clear()
        {
            if (_instance == null)
                return;
            string script = "$0.setText('','api');";
            OpenSilver.Interop.ExecuteJavaScript(script, _instance);
        }

        public void SetReadOnly(bool value)
        {
            if (_instance == null)
                return;

            string val = value ? "false" : "true";
            string script = $"$0.root.setAttribute('contenteditable', '{val}')";
            OpenSilver.Interop.ExecuteJavaScript(script, _instance);
        }

        public void SetEnable(bool value)
        {
            if (_instance == null)
                return;

            string val = value ? "true" : "false";
            string script = $"$0.enable({val})";
            OpenSilver.Interop.ExecuteJavaScript(script, _instance);
        }

        private void RegisterEvents()
        {
            RegisterSelectionChangeEvent();
            RegisterTextChangeEvent();
        }

        private void RegisterSelectionChangeEvent()
        {
#if MIGRATION
            Action onSelectionChange = () =>
            {
                _parent.RaiseSelectionChanged();
            };

            OpenSilver.Interop.ExecuteJavaScript("$0.on('selection-change', function(range, oldRange, source){ if(source === 'user' || source === 'api'){$1();}})", _instance, onSelectionChange);
#endif
        }

        private void RegisterTextChangeEvent()
        {
#if MIGRATION
            Action onTextChange = () =>
            {
                _parent.RaiseContentChanged();
            };

            OpenSilver.Interop.ExecuteJavaScript("$0.on('text-change', function(delta, oldDelta, source){ if(source === 'user' || source === 'api'){$1();}})", _instance, onTextChange);
#endif
        }

        private void LoadExtensions()
        {
            OpenSilver.Interop.ExecuteJavaScript($"let font = Quill.import('attributors/style/font');font.whitelist=[{GetSupportedFonts()}];Quill.register(font,true);");
            OpenSilver.Interop.ExecuteJavaScript($"let size = Quill.import('attributors/style/size');size.whitelist=[{GetSupportedFontSizes()}];Quill.register(size,true);");
        }

        private string GetSupportedFontSizes()
        {
            List<string> sizes = new List<string>();
            for (int i = 8; i <= 72; i++)
            {
                sizes.Add($"'{i}px'");
            }

            return string.Join(",", sizes);
        }

        private string GetSupportedFonts()
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
                else {
                    run.SetAttribute("FontFamily", INTERNAL_FontsHelper.DefaultCssFontFamily);
                }

                paragraph.AppendChild(run);
            }

            return xaml.OuterXml;
        }

        private void CreateInstance()
        {
            string script = "let options = {"
                + $"readOnly: {(_parent.IsReadOnly ? "true" : "false")},modules: {{clipboard: {{matchVisual:false}}}}"
                + "};"
                + "new Quill($0, options);";
            _instance = OpenSilver.Interop.ExecuteJavaScript(script, "#" + _parent.GetEditorId());
        }

       
    }
}
