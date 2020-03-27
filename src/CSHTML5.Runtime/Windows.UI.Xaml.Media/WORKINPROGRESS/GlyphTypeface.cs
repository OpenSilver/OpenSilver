#if WORKINPROGRESS
#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
	//
	// Summary:
	//     Specifies a physical font face that corresponds to a font file on the disk.
	public sealed partial class GlyphTypeface
	{
		//
		// Summary:
		//     Gets or sets the font file name for the System.Windows.Media.GlyphTypeface object.
		//
		// Returns:
		//     The font file name for the System.Windows.Media.GlyphTypeface object.
		public string FontFileName
		{
			get;
			private set;
		}

		//
		// Summary:
		//     Gets the font face version interpreted from the font's 'NAME' table.
		//
		// Returns:
		//     A System.Double value that represents the version.
		public double Version
		{
			get;
			private set;
		}
	}
}
#endif