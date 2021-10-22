using System;

#if MIGRATION
namespace System.Windows.Input
#else
namespace Windows.UI.Xaml.Input
#endif
{
	public delegate void TouchFrameEventHandler(object @sender, TouchFrameEventArgs @e);
}
