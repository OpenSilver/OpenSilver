using OpenSilver;

namespace System.Windows.Media;

/// <summary>
/// Represents a combination of <see cref="Media.FontFamily"/>, <see cref="FontWeight"/>, <see cref="FontStyle"/>, and <see cref="FontStretch"/>.
/// </summary>
[NotImplemented]
public class Typeface
{
    private readonly FontFamily _fontFamily;
    private readonly FontFamily _fallbackFontFamily;

    // these _style, _weight and _stretch are only used for storing what was passed into the constructor.
    // Since FontFamily may change these values when it includes a style name implicitly,
    private readonly FontStyle _style;
    private readonly FontWeight _weight;
    private readonly FontStretch _stretch;

    internal FontFamily FallbackFontFamily => _fallbackFontFamily;

    /// <summary>
    /// Gets the name of the font family from which the typeface was constructed.
    /// </summary>
    /// <value>The <see cref="Media.FontFamily"/> from which the typeface was constructed.</value>
    public FontFamily FontFamily => _fontFamily;

    /// <summary>
    /// Gets the relative weight of the typeface.
    /// </summary>
    /// <value>A <see cref="FontWeight"/> value that represents the relative weight of the typeface.</value>
    public FontWeight Weight => _weight;

    /// <summary>
    /// Gets the style of the <see cref="Typeface"/>.
    /// </summary>
    /// <value>A <see cref="FontStyle"/> value that represents the style value for the typeface.</value>
    public FontStyle Style => _style;

    /// <summary>
    /// Gets the stretch value for the <see cref="Typeface"/>.
    /// The stretch value determines whether a typeface is expanded or condensed when it is displayed.
    /// </summary>
    /// <value>A <see cref="FontStretch"/> value that represents the stretch value for the typeface.</value>
    public FontStretch Stretch => _stretch;

    /// <summary>
    /// Initializes a new instance of the <see cref="Typeface"/> class for the specified font family typeface name.
    /// </summary>
    /// <param name="typefaceName">The typeface name for the specified font family.</param>
    public Typeface(string typefaceName)
        : this(
            new FontFamily(typefaceName),
            FontStyles.Normal,
            FontWeights.Normal,
            FontStretches.Normal)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Typeface"/> class for the specified font family name,
    /// <see cref="Style"/>, <see cref="Weight"/>, and <see cref="Stretch"/> values.
    /// </summary>
    /// <param name="fontFamily">The font family of the typeface.</param>
    /// <param name="style">The style of the typeface.</param>
    /// <param name="weight">The relative weight of the typeface.</param>
    /// <param name="stretch">The degree to which the typeface is stretched.</param>
    public Typeface(FontFamily fontFamily, FontStyle style, FontWeight weight, FontStretch stretch)
        : this(
            fontFamily,
            style,
            weight,
            stretch,
            new FontFamily("#GLOBAL USER INTERFACE"))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Typeface"/> class for the specified font family name,
    /// <see cref="Style"/>, <see cref="Weight"/>, and <see cref="Stretch"/> values. In addition, a fallback font family is specified.
    /// </summary>
    /// <param name="fontFamily">The font family of the typeface.</param>
    /// <param name="style">The style of the typeface.</param>
    /// <param name="weight">The relative weight of the typeface.</param>
    /// <param name="stretch">The degree to which the typeface is stretched.</param>
    /// <param name="fallbackFontFamily">
    /// The font family that is used when a character is encountered that the primary font family
    /// (specified by the <paramref name="fontFamily"/> parameter) cannot display.
    /// </param>
    public Typeface(FontFamily fontFamily, FontStyle style, FontWeight weight, FontStretch stretch, FontFamily fallbackFontFamily)
    {
        ArgumentNullException.ThrowIfNull(fontFamily);

        _fontFamily = fontFamily;
        _style = style;
        _weight = weight;
        _stretch = stretch;
        _fallbackFontFamily = fallbackFontFamily;
    }

    /// <summary>
    /// Retrieves the <see cref="GlyphTypeface"/> that corresponds to the <see cref="Typeface"/>.
    /// </summary>
    /// <param name="glyphTypeface">
    /// <see cref="GlyphTypeface"/> object that corresponds to this typeface,
    /// or <see langword="null"/> if the typeface was constructed from a composite font.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the out parameter is set to a <see cref="GlyphTypeface"/> value;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    [NotImplemented]
    public bool TryGetGlyphTypeface(out GlyphTypeface glyphTypeface)
    {
        glyphTypeface = null;
        return false;
    }

    public override int GetHashCode()
    {
        int hash = _fontFamily.GetHashCode();

        if (_fallbackFontFamily != null)
            hash = hash * -1521134295 + _fallbackFontFamily.GetHashCode();

        hash = hash * -1521134295 + _style.GetHashCode();
        hash = hash * -1521134295 + _weight.GetHashCode();
        hash = hash * -1521134295 + _stretch.GetHashCode();
        return hash;
    }

    /// <summary>
    /// Equality check
    /// </summary>
    public override bool Equals(object o)
    {
        if (o is not Typeface t)
            return false;

        return _style == t._style
            && _weight == t._weight
            && _stretch == t._stretch
            && _fontFamily.Equals(t._fontFamily)
            && CompareFallbackFontFamily(t._fallbackFontFamily);
    }

    internal bool CompareFallbackFontFamily(FontFamily fallbackFontFamily)
    {
        if (fallbackFontFamily == null || _fallbackFontFamily == null)
            return fallbackFontFamily == _fallbackFontFamily;

        return _fallbackFontFamily.Equals(fallbackFontFamily);
    }
}
