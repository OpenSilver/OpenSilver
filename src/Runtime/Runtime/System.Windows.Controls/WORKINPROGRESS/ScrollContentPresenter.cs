using System;

#if MIGRATION
using System.Windows.Controls.Primitives;
#else
using Windows.UI.Xaml.Controls.Primitives;
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
	[OpenSilver.NotImplemented]
	public sealed partial class ScrollContentPresenter : ContentPresenter, IScrollInfo
	{
        [OpenSilver.NotImplemented]
		public Rect MakeVisible(UIElement visual, Rect rectangle)
		{
			return default(Rect);
		}
	}
}
