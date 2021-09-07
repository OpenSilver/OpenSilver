#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
	public enum FontNumeralAlignment
	{
		/// <summary>Default numeral alignment is used.</summary>
		Normal,
		/// <summary>Proportional width alignment is used.</summary>
		Proportional,
		/// <summary>Tabular alignment is used.</summary>
		Tabular
	}
}
