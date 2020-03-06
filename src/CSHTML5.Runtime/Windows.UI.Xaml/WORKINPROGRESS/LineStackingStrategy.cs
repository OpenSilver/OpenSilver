#if WORKINPROGRESS

namespace System.Windows
{
	//
	// Summary:
	//     Describes the mechanism by which a line box is determined for each line.
	public enum LineStackingStrategy
	{
		//
		// Summary:
		//     The stack height is the smallest value that contains the extended block progression
		//     dimension of all the inline elements on that line when those elements are properly
		//     aligned. This is the default.
		MaxHeight = 0,
		//
		// Summary:
		//     The stack height is determined by the block element line-height property value.
		BlockLineHeight = 1,
		//
		// Summary:
		//     The stack height is determined by adding LineHeight to the baseline of the previous
		//     line.
		BaselineToBaseline = 2
	}
}

#endif