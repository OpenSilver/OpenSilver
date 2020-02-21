using System;
#if WORKINPROGRESS

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
	public sealed partial class ScrollContentPresenter : ContentPresenter, IScrollInfo
	{
		public Rect MakeVisible(UIElement visual, Rect rectangle)
		{
			return default(Rect);
		}
	}
}
#endif