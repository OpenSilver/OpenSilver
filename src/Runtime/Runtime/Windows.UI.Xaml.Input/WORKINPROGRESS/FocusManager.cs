#if WORKINPROGRESS

using System;

#if MIGRATION
namespace System.Windows.Input
#else
namespace Windows.UI.Xaml.Input
#endif
{
	public static partial class FocusManager
	{
		public static object GetFocusedElement()
		{
			return null;
		}

		public static object GetFocusedElement(DependencyObject @element)
		{
			return null;
		}
	}
}
#endif