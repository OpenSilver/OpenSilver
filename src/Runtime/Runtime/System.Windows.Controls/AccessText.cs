
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

using OpenSilver.Internal;
using System.Globalization;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace System.Windows.Controls;

/// <summary>
/// Specifies with an underscore the character that is used as the access key.
/// </summary>
[ContentProperty(nameof(Text))]
public class AccessText : FrameworkElement
{
    private const string _accessKeyMarker = "_";
    private const string _doubleAccessKeyMarker = _accessKeyMarker + _accessKeyMarker;

    private TextBlock _textBlock;
    private Run _accessKey;
    private string _currentlyRegistered;

    static AccessText()
    {
        KeyboardNavigation.IsAccessKeyModeProperty.OverrideMetadata(
            typeof(AccessText),
            new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, FrameworkPropertyMetadataOptions.Inherits, OnIsAccessKeyModeChanged));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AccessText"/> class.
    /// </summary>
    public AccessText() { }

    /// <summary>
    /// Provides read-only access to the character that follows the first underline character.
    /// </summary>
    /// <returns>
    /// The character to return.
    /// </returns>
    public char AccessKey
    {
        get
        {
            string text = _accessKey?.Text;
            return string.IsNullOrEmpty(text) ? (char)0 : text[0];
        }
    }

    /// <summary>
    /// Identifies the <see cref="Text"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(AccessText),
            new FrameworkPropertyMetadata(
                string.Empty,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                OnTextChanged));

    /// <summary>
    /// Gets or sets the text that is displayed by the <see cref="AccessText"/> element.
    /// </summary>
    /// <returns>
    /// The text without the first underscore character. The default is an empty string.
    /// </returns>
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValueInternal(TextProperty, value);
    }

    private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((AccessText)d).UpdateText((string)e.NewValue);
    }

    /// <summary>
    /// Identifies the <see cref="FontFamily"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FontFamilyProperty = TextElement.FontFamilyProperty.AddOwner(typeof(AccessText));

    /// <summary>
    /// Gets or sets the font family to use with the <see cref="AccessText"/> element.
    /// </summary>
    /// <returns>
    /// The font family to use.
    /// </returns>
    public FontFamily FontFamily
    {
        get => (FontFamily)GetValue(FontFamilyProperty);
        set => SetValueInternal(FontFamilyProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="FontStyle"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FontStyleProperty = TextElement.FontStyleProperty.AddOwner(typeof(AccessText));

    /// <summary>
    /// Gets or sets the font style to use with the <see cref="AccessText"/> element.
    /// </summary>
    /// <returns>
    /// The font style to use; for example, normal, italic, or oblique.
    /// </returns>
    public FontStyle FontStyle
    {
        get => (FontStyle)GetValue(FontStyleProperty);
        set => SetValueInternal(FontStyleProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="FontWeight"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FontWeightProperty = TextElement.FontWeightProperty.AddOwner(typeof(AccessText));

    /// <summary>
    /// Gets or sets the font weight to use with the <see cref="AccessText"/> element.
    /// </summary>
    /// <returns>
    /// The font weight to use.
    /// </returns>
    public FontWeight FontWeight
    {
        get => (FontWeight)GetValue(FontWeightProperty);
        set => SetValueInternal(FontWeightProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="FontStretch"/> dependency property.
    /// </summary>
    [OpenSilver.NotImplemented]
    public static readonly DependencyProperty FontStretchProperty = TextElement.FontStretchProperty.AddOwner(typeof(AccessText));

    /// <summary>
    /// Gets or sets a <see cref="FontStretch"/> property that selects a normal, condensed, or 
    /// expanded font from a <see cref="FontFamily"/>.
    /// </summary>
    /// <returns>
    /// The relative degree that the font is stretched. The default is <see cref="FontStretches.Normal"/>.
    /// </returns>
    [OpenSilver.NotImplemented]
    public FontStretch FontStretch
    {
        get => (FontStretch)GetValue(FontStretchProperty);
        set => SetValueInternal(FontStretchProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="FontSize"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FontSizeProperty = TextElement.FontSizeProperty.AddOwner(typeof(AccessText));

    /// <summary>
    /// Gets or sets the font size to use with the <see cref="AccessText"/> element.
    /// </summary>
    /// <returns>
    /// The font size to use.
    /// </returns>
    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValueInternal(FontSizeProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Foreground"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ForegroundProperty = TextElement.ForegroundProperty.AddOwner(typeof(AccessText));

    /// <summary>
    /// Gets or sets the <see cref="Brush"/> that draws the text content of the element.
    /// </summary>
    /// <returns>
    /// The <see cref="Brush"/> that draws the text. The default is <see cref="Brushes.Black"/>.
    /// </returns>
    public Brush Foreground
    {
        get => (Brush)GetValue(ForegroundProperty);
        set => SetValueInternal(ForegroundProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="TextDecorations"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TextDecorationsProperty =
        Inline.TextDecorationsProperty.AddOwner(
            typeof(AccessText),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnPropertyChanged));

    /// <summary>
    /// Gets or sets the decorations that are added to the text of an <see cref="AccessText"/> element.
    /// </summary>
    /// <returns>
    /// The <see cref="TextDecorations"/> applied to the text of an <see cref="AccessText"/>. The default
    /// is null.
    /// </returns>
    public TextDecorationCollection TextDecorations
    {
        get => (TextDecorationCollection)GetValue(TextDecorationsProperty);
        set => SetValueInternal(TextDecorationsProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="LineHeight"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LineHeightProperty = Block.LineHeightProperty.AddOwner(typeof(AccessText));

    /// <summary>
    /// Gets or sets the height of each line box.
    /// </summary>
    /// <returns>
    /// A double that specifies the height of each line box. This value must be equal to or greater than 
    /// 0.0034 and equal to or less then 160000. A value of <see cref="double.NaN"/> (equivalent to an 
    /// attribute value of Auto) causes the line height to be determined automatically from the current 
    /// font characteristics. The default is <see cref="double.NaN"/>.
    /// </returns>
    public double LineHeight
    {
        get => (double)GetValue(LineHeightProperty);
        set => SetValueInternal(LineHeightProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="LineStackingStrategy"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LineStackingStrategyProperty = Block.LineStackingStrategyProperty.AddOwner(typeof(AccessText));

    /// <summary>
    /// Gets or sets how the <see cref="LineHeight"/> property is enforced.
    /// </summary>
    /// <returns>
    /// A <see cref="Windows.LineStackingStrategy"/> value that determines the behavior of the 
    /// <see cref="LineHeight"/> property.
    /// </returns>
    public LineStackingStrategy LineStackingStrategy
    {
        get => (LineStackingStrategy)GetValue(LineStackingStrategyProperty);
        set => SetValueInternal(LineStackingStrategyProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="TextAlignment"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TextAlignmentProperty = Block.TextAlignmentProperty.AddOwner(typeof(AccessText));

    /// <summary>
    /// Gets or sets the horizontal alignment of the content.
    /// </summary>
    /// <returns>
    /// The horizontal alignment of the text.
    /// </returns>
    public TextAlignment TextAlignment
    {
        get => (TextAlignment)GetValue(TextAlignmentProperty);
        set => SetValueInternal(TextAlignmentProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="TextTrimming"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TextTrimmingProperty =
        TextBlock.TextTrimmingProperty.AddOwner(
            typeof(AccessText),
            new FrameworkPropertyMetadata(
                TextTrimming.None,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                OnPropertyChanged));

    /// <summary>
    /// Gets or sets how the textual content of an <see cref="AccessText"/> element is clipped if it overflows 
    /// the line box.
    /// </summary>
    /// <returns>
    /// The trimming behavior to use. The default is <see cref="TextTrimming.None"/>.
    /// </returns>
    public TextTrimming TextTrimming
    {
        get => (TextTrimming)GetValue(TextTrimmingProperty);
        set => SetValueInternal(TextTrimmingProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="TextWrapping"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TextWrappingProperty =
        TextBlock.TextWrappingProperty.AddOwner(
            typeof(AccessText),
            new FrameworkPropertyMetadata(
                TextWrapping.NoWrap,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                OnPropertyChanged));

    /// <summary>
    /// Gets or sets whether the textual content of an <see cref="AccessText"/> element is wrapped if it 
    /// overflows the line box.
    /// </summary>
    /// <returns>
    /// The wrapping behavior to use. The default is <see cref="TextWrapping.NoWrap"/>.
    /// </returns>
    public TextWrapping TextWrapping
    {
        get => (TextWrapping)GetValue(TextWrappingProperty);
        set => SetValueInternal(TextWrappingProperty, value);
    }

    /// <summary>
    /// Remeasures the control.
    /// </summary>
    /// <param name="constraint">
    /// The maximum size limit for the control. The return value cannot exceed this size.
    /// </param>
    /// <returns>
    /// The size of the control. Cannot exceed the maximum size limit for the control.
    /// </returns>
    protected sealed override Size MeasureOverride(Size constraint)
    {
        TextBlock.Measure(constraint);
        return TextBlock.DesiredSize;
    }

    /// <summary>
    /// Arranges and sizes the content of an <see cref="AccessText"/> object.
    /// </summary>
    /// <param name="arrangeSize">
    /// The computed size that is used to arrange the content.
    /// </param>
    /// <returns>
    /// The size of the content.
    /// </returns>
    protected sealed override Size ArrangeOverride(Size arrangeSize)
    {
        TextBlock.Arrange(new Rect(arrangeSize));
        return arrangeSize;
    }

    /// <summary>
    /// Gets the number of child elements that are visual.
    /// </summary>
    /// <returns>
    /// Returns an integer that represents the number of child elements that are visible.
    /// </returns>
    protected override int VisualChildrenCount => 1;

    /// <summary>
    /// Gets the index of a visual child element.
    /// </summary>
    /// <param name="index">
    /// The index of the visual child element to return.
    /// </param>
    /// <returns>
    /// Returns an integer that represents the index of a visual child element.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    protected override Visual GetVisualChild(int index)
    {
        if (index != 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        return TextBlock;
    }

    internal static char AccessKeyMarker => _accessKeyMarker[0];

    internal TextBlock TextBlock
    {
        get
        {
            if (_textBlock is null)
            {
                CreateTextBlock();
            }
            return _textBlock;
        }
    }

    private void CreateTextBlock()
    {
        _textBlock = new TextBlock();
        AddVisualChild(_textBlock);
    }

    private void UpdateText(string text)
    {
        text ??= string.Empty;

        _accessKey = null;

        TextBlock.Inlines.Clear();

        int index = FindAccessKeyMarker(text);
        if (index != -1 && index < text.Length - 1)
        {
            string keyText = StringInfo.GetNextTextElement(text, index + 1);

            _accessKey = new Run(keyText);
            ShowKeyboardCue();

            RegisterAccessKey(keyText);

            if (index > 0)
            {
                TextBlock.Inlines.Add(new Run(text.Substring(0, index).Replace(_doubleAccessKeyMarker, _accessKeyMarker)));
            }

            TextBlock.Inlines.Add(_accessKey);

            if (index + keyText.Length < text.Length)
            {
                TextBlock.Inlines.Add(new Run(text.Substring(index + 1 + keyText.Length).Replace(_doubleAccessKeyMarker, _accessKeyMarker)));
            }
        }
        else
        {
            TextBlock.Inlines.Add(new Run(text.Replace(_doubleAccessKeyMarker, _accessKeyMarker)));
        }
    }

    private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((AccessText)d).TextBlock.SetValue(e.Property, e.NewValue);
    }

    private static void OnIsAccessKeyModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((AccessText)d).ShowKeyboardCue();
    }

    // Returns the index of _ marker.
    // _ can be escaped by double _
    private static int FindAccessKeyMarker(string text)
    {
        int lenght = text.Length;
        int startIndex = 0;
        while (startIndex < lenght)
        {
            int index = text.IndexOf(AccessKeyMarker, startIndex);
            if (index == -1)
            {
                return -1;
            }

            // If next char exist and different from _
            if (index + 1 < lenght && text[index + 1] != AccessKeyMarker)
            {
                return index;
            }

            startIndex = index + 2;
        }

        return -1;
    }

    internal static string RemoveAccessKeyMarker(string text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            int index = FindAccessKeyMarker(text);
            if (index >= 0 && index < text.Length - 1)
            {
                text = text.Remove(index, 1);
            }

            // Replace double _ with single _
            text = text.Replace(_doubleAccessKeyMarker, _accessKeyMarker);
        }
        return text;
    }

    private void RegisterAccessKey(string key)
    {
        if (_currentlyRegistered is not null)
        {
            AccessKeyManager.Unregister(_currentlyRegistered, this);
            _currentlyRegistered = null;
        }

        if (!string.IsNullOrEmpty(key))
        {
            AccessKeyManager.Register(key, this);
            _currentlyRegistered = key;
        }
    }

    private void ShowKeyboardCue()
    {
        if (_accessKey is null)
        {
            return;
        }

        if (KeyboardNavigation.GetIsAccessKeyMode(this))
        {
            _accessKey.TextDecorations = Windows.TextDecorations.Underline;
        }
        else
        {
            _accessKey.TextDecorations = null;
        }
    }
}
