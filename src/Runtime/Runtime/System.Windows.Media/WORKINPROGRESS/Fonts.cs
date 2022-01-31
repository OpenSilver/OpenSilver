using System;
using System.Collections.Generic;
using System.Text;

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    [OpenSilver.NotImplemented]
	public static partial class Fonts
	{
		[OpenSilver.NotImplemented]
		public static ICollection<Typeface> SystemTypefaces => new List<Typeface>();
	}
}
