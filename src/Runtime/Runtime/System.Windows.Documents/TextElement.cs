
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

using System.Text;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xaml.Markup;
using CSHTML5.Internal;
using OpenSilver.Internal.Documents;
using OpenSilver.Internal;

namespace System.Windows.Documents;

/// <summary>
/// An abstract class used as the base class for the abstract <see cref="Block"/>
/// and <see cref="Inline"/> classes.
/// </summary>
[RuntimeNameProperty(nameof(Name))]
public abstract class TextElement : UIElement
{
    private ITextContainer _textContainer;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextElement"/> class.
    /// </summary>
    protected TextElement() { }

    internal ITextContainer TextContainer => _textContainer ??= TextContainersHelper.Create(this);

    internal virtual bool IsModel { get; set; }

    /// <summary>
    /// Identifies the <see cref="CharacterSpacing"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CharacterSpacingProperty =
        DependencyProperty.RegisterAttached(
            nameof(CharacterSpacing),
            typeof(int),
            typeof(TextElement),
            new PropertyMetadata(0) { Inherits = true, });

    /// <summary>
    /// Gets or sets the distance between characters of text in the control measured
    /// in 1000ths of the font size.
    /// </summary>
    /// <returns>
    /// The distance between characters of text in the control measured in 1000ths of
    /// the font size. The default is 0.
    /// </returns>
    public int CharacterSpacing
    {
        get => (int)GetValue(CharacterSpacingProperty);
        set => SetValueInternal(CharacterSpacingProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="FontFamily"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FontFamilyProperty =
        DependencyProperty.RegisterAttached(
            nameof(FontFamily),
            typeof(FontFamily),
            typeof(TextElement),
            new PropertyMetadata(FontFamily.Default) { Inherits = true, },
            IsValidFontFamily);

    /// <summary>
    /// Gets or sets the preferred top-level font family for the content of the element.
    /// </summary>
    /// <returns>
    /// The preferred font family, or a primary preferred font family with one or more
    /// fallback font families. See <see cref="Media.FontFamily"/> for default information.
    /// </returns>
    public FontFamily FontFamily
    {
        get => (FontFamily)GetValue(FontFamilyProperty);
        set => SetValueInternal(FontFamilyProperty, value);
    }

    private static bool IsValidFontFamily(object o) => o is FontFamily;

    /// <summary>
    /// Identifies the <see cref="FontSize"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FontSizeProperty =
        DependencyProperty.RegisterAttached(
            nameof(FontSize),
            typeof(double),
            typeof(TextElement),
            new PropertyMetadata(11d) { Inherits = true, });

    /// <summary>
    /// Gets or sets the font size for the content of the element.
    /// </summary>
    /// <returns>
    /// The desired font size in pixels. The default is 11 pixels.
    /// </returns>
    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValueInternal(FontSizeProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="FontStretch"/> dependency property.
    /// </summary>
    [OpenSilver.NotImplemented]
    public static readonly DependencyProperty FontStretchProperty =
        DependencyProperty.RegisterAttached(
            nameof(FontStretch),
            typeof(FontStretch),
            typeof(TextElement),
            new PropertyMetadata(FontStretches.Normal));

    /// <summary>
    /// Gets or sets the glyph width of the font in a family to select.
    /// </summary>
    /// <returns>
    /// One of the <see cref="FontStretches"/> property values, specifying the desired
    /// font stretch. The default is <see cref="FontStretches.Normal"/>.
    /// </returns>
    [OpenSilver.NotImplemented]
    public FontStretch FontStretch
    {
        get => (FontStretch)GetValue(FontStretchProperty);
        set => SetValueInternal(FontStretchProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="FontStyle"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FontStyleProperty =
        DependencyProperty.RegisterAttached(
            nameof(FontStyle),
            typeof(FontStyle),
            typeof(TextElement),
            new PropertyMetadata(FontStyles.Normal) { Inherits = true, });

    /// <summary>
    /// Gets or sets the font style for the content in this element.
    /// </summary>
    /// <returns>
    /// One of the <see cref="FontStyles"/> property values, specifying the font style.
    /// The default is <see cref="FontStyles.Normal"/>.
    /// </returns>
    public FontStyle FontStyle
    {
        get => (FontStyle)GetValue(FontStyleProperty);
        set => SetValueInternal(FontStyleProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="FontWeight"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FontWeightProperty =
        DependencyProperty.RegisterAttached(
            nameof(FontWeight),
            typeof(FontWeight),
            typeof(TextElement),
            new PropertyMetadata(FontWeights.Normal) { Inherits = true, });

    /// <summary>
    /// Gets or sets the top-level font weight to select from the font family for the
    /// content in this element.
    /// </summary>
    /// <returns>
    /// One of the <see cref="FontWeights"/> property values, specifying the font weight.
    /// The default is <see cref="FontWeights.Normal"/>.
    /// </returns>
    public FontWeight FontWeight
    {
        get => (FontWeight)GetValue(FontWeightProperty);
        set => SetValueInternal(FontWeightProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Foreground"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ForegroundProperty =
        DependencyProperty.RegisterAttached(
            nameof(Foreground),
            typeof(Brush),
            typeof(TextElement),
            new PropertyMetadata(new SolidColorBrush(Colors.Black)) { Inherits = true, });

    /// <summary>
    /// Gets or sets the <see cref="Brush"/> to apply to the content in this element.
    /// </summary>
    /// <returns>
    /// The brush that is applied to the text contents. The default is a <see cref="SolidColorBrush"/>
    /// with <see cref="SolidColorBrush.Color"/> value <see cref="Colors.Black"/>.
    /// </returns>
    public Brush Foreground
    {
        get => (Brush)GetValue(ForegroundProperty);
        set => SetValueInternal(ForegroundProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Language"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LanguageProperty =
        FrameworkElement.LanguageProperty.AddOwner(
            typeof(TextElement),
            new FrameworkPropertyMetadata(XmlLanguage.GetLanguage("en-US"), FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>
    /// Gets or sets the language of the content within an <see cref="TextElement"/> element.
    /// </summary>
    /// <returns>
    /// A value of type <see cref="XmlLanguage"/>. The default is a <see cref="XmlLanguage"/>
    /// value created with the string en-US (<see cref="XmlLanguage.IetfLanguageTag"/> is "en-US").
    /// </returns>
    public XmlLanguage Language
    {
        get => (XmlLanguage)GetValue(LanguageProperty);
        set => SetValueInternal(LanguageProperty, value);
    }

    /// <summary>
    /// Gets or sets a unique identification for the object.
    /// </summary>
    /// <returns>
    /// The unique identifier for the object.
    /// </returns>
    public string Name
    {
        get => (string)GetValue(FrameworkElement.NameProperty);
        set => SetValueInternal(FrameworkElement.NameProperty, value);
    }

    /// <summary>
    /// Gets an object in the Silverlight object model by referencing the object's x:Name
    /// or <see cref="Name"/> attribute value.
    /// </summary>
    /// <param name="name">
    /// The name of the object to retrieve.
    /// </param>
    /// <returns>
    /// The object that has the specified name, or null if no object is retrieved.
    /// </returns>
    public object FindName(string name)
    {
        DependencyObject parent = VisualTreeHelper.GetParent(this);
        while (parent is not null)
        {
            if (parent is FrameworkElement fe)
            {
                return fe.FindName(name);
            }

            parent = VisualTreeHelper.GetParent(parent);
        }

        return null;
    }

    /// <summary>
    /// Gets a <see cref="TextPointer"/> that represents the end of the content
    /// in the element.
    /// </summary>
    /// <returns>
    /// A <see cref="TextPointer"/> that represents the end of the content
    /// in the <see cref="TextElement"/>.
    /// </returns>
    [OpenSilver.NotImplemented]
    public TextPointer ContentEnd { get; }

    /// <summary>
    /// Gets a <see cref="TextPointer"/> that represents the start of content
    /// in the element.
    /// </summary>
    /// <returns>
    /// A <see cref="TextPointer"/> that represents the start of the content
    /// in the <see cref="TextElement"/>.
    /// </returns>
    [OpenSilver.NotImplemented]
    public TextPointer ContentStart { get; }

    /// <summary>
    /// Gets a <see cref="TextPointer"/> that represents the position just
    /// after the end of the element.
    /// </summary>
    /// <returns>
    /// A <see cref="TextPointer"/> that represents the position just after
    /// the end of the <see cref="TextElement"/>.
    /// </returns>
    [OpenSilver.NotImplemented]
    public TextPointer ElementEnd { get; }

    /// <summary>
    /// Gets a <see cref="TextPointer"/> that represents the position just
    /// before the start of the element.
    /// </summary>
    /// <returns>
    /// A <see cref="TextPointer"/> that represents the position just before
    /// the start of the <see cref="TextElement"/>.
    /// </returns>
    [OpenSilver.NotImplemented]
    public TextPointer ElementStart { get; }

    internal virtual string TagName => "span";

    internal virtual void AppendHtml(StringBuilder builder) { }

    internal sealed override UIElement MouseTarget => null;

    internal sealed override UIElement KeyboardTarget => null;

    internal override sealed void SetPointerEvents(bool hitTestable) { }

    internal override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.Metadata is FrameworkPropertyMetadata fMetadata)
        {
            if (fMetadata.AffectsMeasure && e.OperationType != OperationType.Inherit)
            {
                UIElementHelpers.InvalidateMeasure(this);
            }
        }

        if (e.Metadata.Inherits)
        {
            if (e.OperationType != OperationType.Inherit)
            {
                TreeWalkHelper.InvalidateOnInheritablePropertyChange(
                    this,
                    new InheritablePropertyChangeInfo(this, e.Property, e.OldValue, e.NewValue),
                    true);
            }
        }
    }

    internal override void OnVisualParentChanged(DependencyObject oldParent)
    {
        InvalidateInheritedProperties(this, VisualTreeHelper.GetParent(this));

        base.OnVisualParentChanged(oldParent);
    }

    public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
    {
        domElementWhereToPlaceChildren = null;
        return INTERNAL_HtmlDomManager.CreateTextElementDomElementAndAppendIt(parentRef, this);
    }
}
