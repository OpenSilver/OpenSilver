#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
	public enum FontFraction
	{
		/// <summary>Default style is used.</summary>
		Normal,
		/// <summary>Stacked fraction style is used.</summary>
		Stacked,
		/// <summary>Slashed fraction style is used.</summary>
		Slashed
	}
}
