#if WORKINPROGRESS
using System;
using System.Windows;

#if MIGRATION
namespace System.Windows.Input
#else
namespace Windows.UI.Xaml.Input
#endif
{
	public sealed partial class TouchFrameEventArgs : EventArgs
	{
		internal TouchFrameEventArgs()
		{
		}

		public void SuspendMousePromotionUntilTouchUp()
		{
		}

		public TouchPointCollection GetTouchPoints(UIElement @relativeTo)
		{
			return null;
		}

		public TouchPoint GetPrimaryTouchPoint(UIElement @relativeTo)
		{
			return null;
		}
	}
}
#endif