
/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using System;

#if MIGRATION
using System.Windows.Media;
using System.Windows.Threading;
#else
using Windows.Foundation;
using Windows.UI.Xaml.Media;
using Dispatcher = Windows.UI.Core.CoreDispatcher;
#endif

#if MIGRATION
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
    /// <summary>
    /// Defines methods that provide additional information about the layout of an element.
    /// </summary>
    public static class LayoutInformation
	{
		/// <summary>
        /// Returns the visible region of the specified element.
        /// </summary>
        /// <param name="element">
        /// The element whose layout clip geometry is desired.
        /// </param>
        /// <returns>
        /// The visible region of the clipped element, or null if the element was not clipped
        /// during layout.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// element is null.
        /// </exception>
		public static Geometry GetLayoutClip(FrameworkElement element)
		{
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (element.LayoutClip is Rect layoutClip)
            {
                return new RectangleGeometry
                {
                    Rect = layoutClip,
                };
            }

            return null;
		}

        /// <summary>
        /// Returns the element that was being processed by the layout system at the moment
        /// of an unhandled exception.
        /// </summary>
        /// <param name="dispatcher">
        /// The <see cref="Dispatcher"/> object that defines the scope of the
        /// operation. There is one dispatcher per layout engine instance.
        /// </param>
        /// <returns>
        /// The element being processed at the time of an unhandled exception.
        /// </returns>
		public static UIElement GetLayoutExceptionElement(Dispatcher dispatcher)
		{
            return LayoutManager.Current.GetLastExceptionElement();
		}

        /// <summary>
        /// Returns the layout slot, or bounding box, that contains the specified element.
        /// </summary>
        /// <param name="element">
        /// The element whose layout slot is desired.
        /// </param>
        /// <returns>
        /// The area assigned to the element for layout.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// element is null.
        /// </exception>
        public static Rect GetLayoutSlot(FrameworkElement element)
		{
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            return element.PreviousArrangeRect;
        }
	}
}
