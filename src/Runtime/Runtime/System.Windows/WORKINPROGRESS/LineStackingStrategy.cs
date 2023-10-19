namespace System.Windows
{
	/// <summary>
	/// Describes the mechanism by which a line box is determined for each line.
	/// </summary>
	public enum LineStackingStrategy
	{
		/// <summary>
		/// The stack height is the smallest value that contains the extended block progression
		/// dimension of all the inline elements on that line when those elements are properly
		/// aligned. This is the default.
		/// </summary>
		MaxHeight = 0,
		/// <summary>
		/// The stack height is determined by the block element line-height property value.
		/// </summary>
		BlockLineHeight = 1,
		/// <summary>
		/// The stack height is determined by adding LineHeight to the baseline of the previous
		/// line.
		/// </summary>
		BaselineToBaseline = 2
	}
}
