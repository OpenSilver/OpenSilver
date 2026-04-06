using System.Globalization;
using OpenSilver;

namespace System.Windows.Media;

/// <summary>
/// Provides low-level control for drawing text in Windows Presentation Foundation (WPF) applications.
/// </summary>
[NotImplemented]
public class FormattedText
{
    private readonly double _width;
    private readonly double _height;

    /// <summary>
    /// Gets the string of text to be displayed.
    /// </summary>
    /// <value>The string of text to be displayed.</value>
    public string Text { get; }

    /// <summary>
    /// Gets or sets the <see cref="FlowDirection"/> of a <see cref="FormattedText"/> object.
    /// </summary>
    /// <value>The <see cref="Windows.FlowDirection"/> of the formatted text.</value>
    public FlowDirection FlowDirection { get; set; }

    /// <summary>
    /// Get or sets the PixelsPerDip at which the text should be rendered.
    /// </summary>
    /// <value>The current <see cref="PixelsPerDip"/> value.</value>
    public double PixelsPerDip { get; set; }

    /// <summary>
    /// Gets the width between the leading and trailing alignment points of a line,
    /// excluding any trailing white-space characters.
    /// </summary>
    /// <value>
    /// The width between the leading and trailing alignment points of a line,
    /// excluding any trailing white-space characters.
    /// Provided in device-independent units (1/96th inch per unit).
    /// </value>
    public double Width => _width;

    /// <summary>
    /// Gets the distance from the top of the first line to the bottom of the last line of the <see cref="FormattedText"/> object.
    /// </summary>
    /// <value>
    /// The distance from the top of the first line to the bottom of the last line,
    /// provided in device-independent units (1/96th inch per unit).
    /// </value>
    public double Height => _height;

    /// <summary>
    /// Initializes a new instance of the <see cref="FormattedText"/> class with the specified text,
    /// culture, flow direction, typeface, font size, foreground brush, and pixelsPerDip value.
    /// </summary>
    /// <param name="textToFormat">The text to be displayed.</param>
    /// <param name="culture">The specific culture of the text.</param>
    /// <param name="flowDirection">The direction the text is read.</param>
    /// <param name="typeface">The font family, weight, style and stretch the text should be formatted with.</param>
    /// <param name="emSize">The font size for the text's em measure, provided in device-independent units (1/96th inch per unit).</param>
    /// <param name="foreground">The brush used to paint the each glyph.</param>
    /// <param name="pixelsPerDip">
    /// The Pixels Per Density Independent Pixel value, which is the equivalent of the scale factor.
    /// For example, if the DPI of a screen is 120 (or 1.25 because 120/96 = 1.25) , 1.25 pixel per density
    /// independent pixel is drawn. DIP is the unit of measurement used by WPF to be independent of device resolution and DPIs.
    /// </param>
    public FormattedText(
        string textToFormat,
        CultureInfo culture,
        FlowDirection flowDirection,
        Typeface typeface,
        double emSize,
        Brush foreground,
        double pixelsPerDip)
    {
        Text = textToFormat;
        FlowDirection = flowDirection;
        PixelsPerDip = pixelsPerDip;

        // Stub size: approximate width/height from text length and em size.
        _width = Text.Length * emSize * 0.6;
        _height = emSize * 1.2;
    }

    /// <summary>
    /// Returns a <see cref="Geometry"/> object that represents the formatted text, including all glyphs and text decorations.
    /// </summary>
    /// <param name="origin">The top-left origin of the resulting geometry.</param>
    /// <returns>The <see cref="Geometry"/> object representation of the formatted text.</returns>
    [NotImplemented]
    public Geometry BuildGeometry(Point origin) => Geometry.Empty;
}
