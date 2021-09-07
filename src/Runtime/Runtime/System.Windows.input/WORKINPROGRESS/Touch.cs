using System;

#if MIGRATION
namespace System.Windows.Input
#else
namespace Windows.UI.Xaml.Input
#endif
{
    [OpenSilver.NotImplemented]
	public static partial class Touch
	{
        [OpenSilver.NotImplemented]
		public static event TouchFrameEventHandler FrameReported;
	}
}
