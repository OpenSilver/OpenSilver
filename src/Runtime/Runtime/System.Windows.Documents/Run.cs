
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
using OpenSilver.Internal;
using OpenSilver.Internal.Documents;
using OpenSilver.Internal.Media;
using System.Diagnostics;
using System.Text;
using System.Web;
using System.Windows.Markup;
using System.Windows.Media;

namespace System.Windows.Documents;

/// <summary>
/// Represents a discrete section of formatted or unformatted text.
/// </summary>
[ContentProperty(nameof(Text))]
public sealed class Run : Inline
{
    static Run()
    {
        CharacterSpacingProperty.OverrideMetadata(
            typeof(Run),
            new FrameworkPropertyMetadata(
                0,
                FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure,
                OnPropertyChanged)
            {
                MethodToUpdateDom2 = static (d, oldValue, newValue) => ((Run)d).SetCharacterSpacing((int)newValue),
            });

        FontFamilyProperty.OverrideMetadata(
            typeof(Run),
            new FrameworkPropertyMetadata(
                FontFamily.Default,
                FrameworkPropertyMetadataOptions.Inherits,
                OnFontFamilyChanged)
            {
                MethodToUpdateDom2 = static (d, oldValue, newValue) => ((Run)d).SetFontFamily((FontFamily)newValue),
            });

        FontSizeProperty.OverrideMetadata(
            typeof(Run),
            new FrameworkPropertyMetadata(
                11d,
                FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure,
                OnPropertyChanged)
            {
                MethodToUpdateDom2 = static (d, oldValue, newValue) => ((Run)d).SetFontSize((double)newValue),
            });

        FontStyleProperty.OverrideMetadata(
            typeof(Run),
            new FrameworkPropertyMetadata(
                FontStyles.Normal,
                FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure,
                OnPropertyChanged)
            {
                MethodToUpdateDom2 = static (d, oldValue, newValue) => ((Run)d).SetFontStyle((FontStyle)newValue),
            });

        FontWeightProperty.OverrideMetadata(
            typeof(Run),
            new FrameworkPropertyMetadata(
                FontWeights.Normal,
                FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure,
                OnPropertyChanged)
            {
                MethodToUpdateDom2 = static (d, oldValue, newValue) => ((Run)d).SetFontWeight((FontWeight)newValue),
            });

        ForegroundProperty.OverrideMetadata(
            typeof(Run),
            new FrameworkPropertyMetadata(
                ForegroundProperty.DefaultMetadata.DefaultValue,
                FrameworkPropertyMetadataOptions.Inherits,
                OnForegroundChanged)
            {
                MethodToUpdateDom2 = static (d, oldValue, newValue) => ((Run)d).SetForeground(oldValue as Brush, (Brush)newValue),
            });

        TextDecorationsProperty.OverrideMetadata(
            typeof(Run),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.Inherits,
                OnPropertyChanged)
            {
                MethodToUpdateDom2 = static (d, oldValue, newValue) => ((Run)d).SetTextDecorations((TextDecorationCollection)newValue),
            });

        FlowDirectionProperty.OverrideMetadata(
            typeof(Run),
            new FrameworkPropertyMetadata(FlowDirection.LeftToRight, FrameworkPropertyMetadataOptions.Inherits)
            {
                MethodToUpdateDom2 = static (d, oldValue, newValue) => ((Run)d).SetDirection((FlowDirection)newValue),
            });
    }

    private WeakEventToken _weakEventToken;

    /// <summary>
    /// Initializes a new, default instance of the <see cref="Run"/> class.
    /// </summary>
    public Run() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Run"/> class, taking a specified string 
    /// as the initial contents of the text run.
    /// </summary>
    /// <param name="text">
    /// A string specifying the initial contents of the <see cref="Run"/> object.
    /// </param>
    public Run(string text)
    {
        Text = text;
    }

    /// <summary>
    /// Identifies the <see cref="Text"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(Run),
            new PropertyMetadata(string.Empty, OnTextChanged, CoerceText)
            {
                MethodToUpdateDom2 = static (d, oldValue, newValue) => ((Run)d).SetInnerText((string)newValue),
            });

    /// <summary>
    /// A string that specifies the text contents of the <see cref="Run"/>.
    /// The default is <see cref="string.Empty"/>.
    /// </summary>
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValueInternal(TextProperty, value);
    }

    private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((Run)d).TextContainer.OnTextContentChanged();
    }

    private static object CoerceText(DependencyObject d, object value)
    {
        return (string)value ?? string.Empty;
    }

    /// <summary>
    /// Identifies the <see cref="FlowDirection"/> dependency property.
    /// </summary>
    public new static readonly DependencyProperty FlowDirectionProperty = Inline.FlowDirectionProperty;

    /// <summary>
    /// Gets or sets the direction that text and other user interface elements flow within
    /// the <see cref="Run"/> element that controls their layout.
    /// </summary>
    /// <returns>
    /// The direction that text and other UI elements flow within the <see cref="Run"/>
    /// element. The default value is <see cref="FlowDirection.LeftToRight"/>.
    /// </returns>
    public new FlowDirection FlowDirection
    {
        get => base.FlowDirection;
        set => base.FlowDirection = value;
    }

    private static void OnFontFamilyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var run = (Run)d;
        UIElementHelpers.InvalidateMeasureOnFontFamilyChanged(run, (FontFamily)e.NewValue);
        run.OnPropertyChanged();
    }

    private static void OnForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var run = (Run)d;

        if (run._weakEventToken != null)
        {
            run._weakEventToken.Dispose();
            run._weakEventToken = null;
        }

        if (e.NewValue is Brush newBrush && !newBrush.IsSealed)
        {
            run._weakEventToken = WeakEvent.Subscribe<Run, Brush, EventArgs>(
                run,
                newBrush,
                static (instance, sender, args) => instance.OnForegroundChanged(sender, args),
                static (handler, source) => source.Changed -= new EventHandler(handler),
                static (handler, source) => source.Changed += new EventHandler(handler));
        }

        run.OnPropertyChanged();
    }

    private void OnForegroundChanged(object sender, EventArgs e)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
        {
            var foreground = (Brush)sender;
            this.SetForeground(foreground, foreground);
        }
    }

    private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((Run)d).OnPropertyChanged();
    }

    private void OnPropertyChanged()
    {
        if (IsModel)
        {
            TextContainer.OnTextContentChanged();
        }
    }

    internal override ITextContainer OnCreateTextContainer() => new TextContainerRun(this);

    internal override void AppendHtml(StringBuilder builder)
    {
        builder.Append("<span class=\"opensilver-inline\" style=\"font: ");
        FontProperties.AppendCssFontAsHtml(builder, FontStyle, FontWeight, FontSize, 0.0, FontFamily);
        builder.Append($"; letter-spacing: {FontProperties.ToCssLetterSpacing(CharacterSpacing)}\">")
               .Append(HttpUtility.HtmlEncode(Text))
               .Append("</span>");
    }

    private sealed class TextContainerRun : ITextContainer
    {
        private readonly Run _run;

        public TextContainerRun(Run run)
        {
            Debug.Assert(run is not null);
            _run = run;
        }

        public string Text => _run.Text;

        public void OnTextContentChanged()
        {
            if (TextContainersHelper.Get(VisualTreeHelper.GetParent(_run)) is ITextContainer parent)
            {
                parent.OnTextContentChanged();
            }
        }
    }
}
