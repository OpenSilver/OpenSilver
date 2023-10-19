namespace System.Windows.Media
{
    /// <summary>
    /// Defines the supported rendering modes for text.
    /// </summary>
    public enum TextRenderingMode
	{
        /// <summary>
        /// Text is rendered with the most appropriate rendering algorithm based on the layout
        /// mode that was used to format the text.
        /// </summary>
        Auto = 0,
        /// <summary>
        /// Text is rendered with bilevel anti-aliasing.
        /// </summary>
        Aliased = 1,
        /// <summary>
        /// Text is rendered with grayscale anti-aliasing.
        /// </summary>
        Grayscale = 2,
        /// <summary>
        /// Text is rendered with the most appropriate ClearType rendering algorithm based
        /// on the layout mode that was used to format the text.
		/// </summary>
        ClearType = 3
	}
}
