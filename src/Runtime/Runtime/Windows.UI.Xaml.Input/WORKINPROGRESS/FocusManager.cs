#if WORKINPROGRESS

using System;

#if MIGRATION
namespace System.Windows.Input
#else
namespace Windows.UI.Xaml.Input
#endif
{
    [OpenSilver.NotImplemented]
	public static partial class FocusManager
	{
        [OpenSilver.NotImplemented]
		public static object GetFocusedElement()
		{
			return null;
		}

        [OpenSilver.NotImplemented]
		public static object GetFocusedElement(DependencyObject @element)
		{
			return null;
		}
	}
}
#endif