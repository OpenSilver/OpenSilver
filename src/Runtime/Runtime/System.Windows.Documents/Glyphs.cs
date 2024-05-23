
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

using System.Windows.Media;

namespace System.Windows.Documents;

/// <summary>
/// Provides a visual representation of letters, characters, or symbols, in a specific font and style.
/// </summary>
[OpenSilver.NotImplemented]
public sealed class Glyphs : FrameworkElement
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Glyphs"/> class.
    /// </summary>
    public Glyphs() { }

    /// <summary>
    /// Identifies the <see cref="Fill"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FillProperty =
        DependencyProperty.Register(
            nameof(Fill),
            typeof(Brush),
            typeof(Glyphs),
            new PropertyMetadata((object)null));

    /// <summary>
    /// Gets or sets the <see cref="Brush"/> that is used to render the glyphs.
    /// </summary>
    /// <returns>
    /// The brush to use to render the glyphs. The default is null, which is evaluated
    /// as <see cref="Colors.Transparent"/> for rendering.
    /// </returns>
    public Brush Fill
    {
        get => (Brush)GetValue(FillProperty);
        set => SetValue(FillProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="FontRenderingEmSize"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FontRenderingEmSizeProperty =
        DependencyProperty.Register(
            nameof(FontRenderingEmSize),
            typeof(double),
            typeof(Glyphs),
            new PropertyMetadata(0.0));

    /// <summary>
    /// Gets or sets the font size used for rendering the glyphs.
    /// </summary>
    /// <returns>
    /// The font size used for rendering the glyphs. The default is 0.
    /// </returns>
    public double FontRenderingEmSize
    {
        get => (double)GetValue(FontRenderingEmSizeProperty);
        set => SetValue(FontRenderingEmSizeProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="FontUri"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FontUriProperty =
        DependencyProperty.Register(
            nameof(FontUri),
            typeof(Uri),
            typeof(Glyphs),
            new PropertyMetadata(new Uri(string.Empty, UriKind.Relative)));

    /// <summary>
    /// Gets or sets the location of the font used for rendering the glyphs.
    /// </summary>
    /// <returns>
    /// The URI source of the font. The default is null.
    /// </returns>
    public Uri FontUri
    {
        get => (Uri)GetValue(FontUriProperty);
        set => SetValue(FontUriProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Indices"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IndicesProperty =
        DependencyProperty.Register(
            nameof(Indices),
            typeof(string),
            typeof(Glyphs),
            new PropertyMetadata(string.Empty));

    /// <summary>
    /// Gets or sets the glyph indices for the glyphs.
    /// </summary>
    /// <returns>
    /// A string that defines glyph indices as well as other glyph specifics in a string
    /// mini-language. The default is null.
    /// </returns>
    public string Indices
    {
        get => (string)GetValue(IndicesProperty);
        set => SetValue(IndicesProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="OriginX"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty OriginXProperty =
        DependencyProperty.Register(
            nameof(OriginX),
            typeof(double),
            typeof(Glyphs),
            new PropertyMetadata((double)float.MinValue));

    /// <summary>
    /// Gets or sets the x origin for the glyphs.
    /// </summary>
    /// <returns>
    /// The x origin of the <see cref="Glyphs"/>, in pixels. The default is 0.
    /// </returns>
    public double OriginX
    {
        get => (double)GetValue(OriginXProperty);
        set => SetValue(OriginXProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="OriginY"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty OriginYProperty =
        DependencyProperty.Register(
            nameof(OriginY),
            typeof(double),
            typeof(Glyphs),
            new PropertyMetadata((double)float.MinValue));

    /// <summary>
    /// Gets or sets the y origin for the glyphs.
    /// </summary>
    /// <returs>
    /// The y origin of the <see cref="Glyphs"/>, in pixels. The default is 0.
    /// </returs>
    public double OriginY
    {
        get => (double)GetValue(OriginYProperty);
        set => SetValue(OriginYProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="StyleSimulations"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty StyleSimulationsProperty =
        DependencyProperty.Register(
            nameof(StyleSimulations),
            typeof(StyleSimulations),
            typeof(Glyphs),
            new PropertyMetadata(StyleSimulations.None));

    /// <summary>
    /// Gets or sets the style simulations applied to the glyphs.
    /// </summary>
    /// <returns>
    /// One of the enumeration values that specifies the style simulations to apply to
    /// the glyphs. The default is None.
    /// </returns>
    public StyleSimulations StyleSimulations
    {
        get => (StyleSimulations)GetValue(StyleSimulationsProperty);
        set => SetValue(StyleSimulationsProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="UnicodeString"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty UnicodeStringProperty =
        DependencyProperty.Register(
            nameof(UnicodeString),
            typeof(string),
            typeof(Glyphs),
            new PropertyMetadata(string.Empty));

    /// <summary>
    /// Gets or sets the Unicode string to render in glyphs.
    /// </summary>
    /// <returns>
    /// A Unicode string with XAML-compatible encoding. The default is an empty string.
    /// </returns>
    public string UnicodeString
    {
        get => (string)GetValue(UnicodeStringProperty);
        set => SetValue(UnicodeStringProperty, value);
    }

    /// <summary>
    /// Gets or sets the font source that is applied to the <see cref="Glyphs"/>
    /// for rendering content.
    /// </summary>
    /// <returns>
    /// The font source used to render content in the <see cref="Glyphs"/>.
    /// The default is a null reference.
    /// </returns>
    public FontSource FontSource { get; set; }
}
