#if !MIGRATION
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
	public partial interface IScrollInfo
	{
		Rect MakeVisible(UIElement visual, Rect rectangle);
	}
}
