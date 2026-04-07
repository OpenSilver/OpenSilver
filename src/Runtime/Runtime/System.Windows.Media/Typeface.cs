
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

using OpenSilver;

namespace System.Windows.Media;

/// <summary>
/// Represents a combination of <see cref="Media.FontFamily"/>, <see cref="FontWeight"/>, <see cref="FontStyle"/>, and <see cref="FontStretch"/>.
/// </summary>
[NotImplemented]
public class Typeface
{
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

        FontFamily = fontFamily;
        Style = style;
        Weight = weight;
        Stretch = stretch;
        FallbackFontFamily = fallbackFontFamily;
    }

    internal FontFamily FallbackFontFamily { get; }

    /// <summary>
    /// Gets the name of the font family from which the typeface was constructed.
    /// </summary>
    /// <value>The <see cref="Media.FontFamily"/> from which the typeface was constructed.</value>
    public FontFamily FontFamily { get; }

    /// <summary>
    /// Gets the relative weight of the typeface.
    /// </summary>
    /// <value>A <see cref="FontWeight"/> value that represents the relative weight of the typeface.</value>
    public FontWeight Weight { get; }

    /// <summary>
    /// Gets the style of the <see cref="Typeface"/>.
    /// </summary>
    /// <value>A <see cref="FontStyle"/> value that represents the style value for the typeface.</value>
    public FontStyle Style { get; }

    /// <summary>
    /// Gets the stretch value for the <see cref="Typeface"/>.
    /// The stretch value determines whether a typeface is expanded or condensed when it is displayed.
    /// </summary>
    /// <value>A <see cref="FontStretch"/> value that represents the stretch value for the typeface.</value>
    public FontStretch Stretch { get; }

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
    public bool TryGetGlyphTypeface(out GlyphTypeface glyphTypeface)
    {
        glyphTypeface = null;
        return false;
    }

    public override int GetHashCode()
    {
        int hash = FontFamily.GetHashCode();

        if (FallbackFontFamily != null)
            hash = hash * -1521134295 + FallbackFontFamily.GetHashCode();

        hash = hash * -1521134295 + Style.GetHashCode();
        hash = hash * -1521134295 + Weight.GetHashCode();
        hash = hash * -1521134295 + Stretch.GetHashCode();
        return hash;
    }

    /// <summary>
    /// Equality check
    /// </summary>
    public override bool Equals(object o)
    {
        if (o is not Typeface t)
            return false;

        return Style == t.Style
            && Weight == t.Weight
            && Stretch == t.Stretch
            && FontFamily.Equals(t.FontFamily)
            && CompareFallbackFontFamily(t.FallbackFontFamily);
    }

    internal bool CompareFallbackFontFamily(FontFamily fallbackFontFamily)
    {
        if (fallbackFontFamily == null || FallbackFontFamily == null)
            return fallbackFontFamily == FallbackFontFamily;

        return FallbackFontFamily.Equals(fallbackFontFamily);
    }
}
