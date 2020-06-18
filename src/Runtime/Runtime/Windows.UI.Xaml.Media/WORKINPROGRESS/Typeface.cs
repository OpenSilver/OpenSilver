#if WORKINPROGRESS
using System;
using System.Collections.Generic;
using System.Text;

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
	public partial class Typeface
	{
		public bool TryGetGlyphTypeface(out GlyphTypeface glyphTypeface)
		{
			glyphTypeface = null;
			return false;
		}
	}
}
#endif