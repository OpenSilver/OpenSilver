#if WORKINPROGRESS
using System;
using System.Windows;

#if MIGRATION
using System.Windows.Media;
using System.Windows.Threading;
#else
using Windows.Foundation;
using Windows.UI.Xaml.Media;
using Microsoft.AspNetCore.Components;
#endif

#if MIGRATION
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
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
		public static UIElement GetLayoutExceptionElement(Dispatcher dispatcher)
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
		public static Rect GetLayoutSlot(FrameworkElement element)
		{
			return default(Rect);
		}
	}
}
#endif