#if MIGRATION

namespace System.Windows.Media
{
	//
	// Summary:
	//     Defines the supported rendering modes for text.
	public enum TextRenderingMode
	{
		//
		// Summary:
		//     Text is rendered with the most appropriate rendering algorithm based on the layout
		//     mode that was used to format the text.
		Auto = 0,
		//
		// Summary:
		//     Text is rendered with bilevel anti-aliasing.
		Aliased = 1,
		//
		// Summary:
		//     Text is rendered with grayscale anti-aliasing.
		Grayscale = 2,
		//
		// Summary:
		//     Text is rendered with the most appropriate ClearType rendering algorithm based
		//     on the layout mode that was used to format the text.
		ClearType = 3
	}
}

#endif
