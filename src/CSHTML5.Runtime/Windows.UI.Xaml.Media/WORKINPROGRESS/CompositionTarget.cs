#if WORKINPROGRESS
using System;

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
	public static partial class CompositionTarget
	{
		public static event EventHandler Rendering;
	}
}
#endif