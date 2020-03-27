#if WORKINPROGRESS
using System;

#if MIGRATION
namespace System.Windows.Input
#else
namespace Windows.UI.Xaml.Input
#endif
{
	public static partial class Touch
	{
		public static event TouchFrameEventHandler FrameReported;
	}
}
#endif