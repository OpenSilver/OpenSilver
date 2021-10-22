using System;
using System.Windows;

#if MIGRATION
namespace System.Windows.Input
#else
namespace Windows.UI.Xaml.Input
#endif
{
    [OpenSilver.NotImplemented]
	public sealed partial class TouchFrameEventArgs : EventArgs
	{
		internal TouchFrameEventArgs()
		{
		}

        [OpenSilver.NotImplemented]
		public void SuspendMousePromotionUntilTouchUp()
		{
		}

        [OpenSilver.NotImplemented]
		public TouchPointCollection GetTouchPoints(UIElement @relativeTo)
		{
			return null;
		}

        [OpenSilver.NotImplemented]
		public TouchPoint GetPrimaryTouchPoint(UIElement @relativeTo)
		{
			return null;
		}
	}
}
