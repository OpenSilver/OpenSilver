using System;

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    [OpenSilver.NotImplemented]
	public static partial class CompositionTarget
	{
        [OpenSilver.NotImplemented]
		public static event EventHandler Rendering;
	}
}
