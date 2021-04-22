#if WORKINPROGRESS

#if !MIGRATION
using System;
#endif

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI
#endif
{
	public partial struct Color : IFormattable
	{
		//
		// Summary:
		//     Creates a string representation of the color by using the ARGB channels and the
		//     specified format provider.
		//
		// Parameters:
		//   provider:
		//     Culture-specific formatting information.
		//
		// Returns:
		//     The string representation of the color.
		[OpenSilver.NotImplemented]
		public string ToString(IFormatProvider provider)
		{
			return default(string);
		}

		string IFormattable.ToString(string format, IFormatProvider formatProvider)
		{
			return default(string);
		}
	}
}
#endif