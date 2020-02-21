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
	public static partial class Fonts
	{
		public static ICollection<Typeface> SystemTypefaces
		{
			get;
			private set;
		}
	}
}
#endif