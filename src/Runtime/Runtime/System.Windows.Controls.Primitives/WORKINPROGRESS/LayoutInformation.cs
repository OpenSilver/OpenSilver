using System;
using System.Windows;

#if MIGRATION
using System.Windows.Media;
using System.Windows.Threading;
#else
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml.Media;
#endif

#if MIGRATION
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
    [OpenSilver.NotImplemented]
	public static partial class LayoutInformation
	{
		//
		// Summary:
		//     Returns the visible region of the specified element.
		//
		// Parameters:
		//   element:
		//     The element whose layout clip geometry is desired.
		//
		// Returns:
		//     The visible region of the clipped element, or null if the element was not clipped
		//     during layout.
		//
		// Exceptions:
		//   T:System.ArgumentNullException:
		//     element is null.
        [OpenSilver.NotImplemented]
		public static Geometry GetLayoutClip(FrameworkElement element)
		{
			return default(Geometry);
		}

		//
		// Summary:
		//     Returns the element that was being processed by the layout system at the moment
		//     of an unhandled exception.
		//
		// Parameters:
		//   dispatcher:
		//     The System.Windows.Threading.Dispatcher object that defines the scope of the
		//     operation. There is one dispatcher per layout engine instance.
		//
		// Returns:
		//     The element being processed at the time of an unhandled exception.
        [OpenSilver.NotImplemented]
#if MIGRATION
		public static UIElement GetLayoutExceptionElement(Dispatcher dispatcher)
#else
		public static UIElement GetLayoutExceptionElement(CoreDispatcher dispatcher)
#endif
		{
			return default(UIElement);
		}

		//
		// Summary:
		//     Returns the layout slot, or bounding box, that contains the specified element.
		//
		// Parameters:
		//   element:
		//     The element whose layout slot is desired.
		//
		// Returns:
		//     The area assigned to the element for layout.
        [OpenSilver.NotImplemented]
		public static Rect GetLayoutSlot(FrameworkElement element)
		{
			return default(Rect);
		}
	}
}
