
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

#if !MIGRATION
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
    /// <summary>
    /// Represents the main scrollable region inside a <see cref="ScrollViewer"/>
    /// control.
    /// </summary>
    public interface IScrollInfo
    {
        #region Public Methods

        /// <summary>
        /// Scrolls down within the content by one logical unit.
        /// </summary>
        void LineDown();

        /// <summary>
        /// Scrolls left within the content by one logical unit.
        /// </summary>
        void LineLeft();

        /// <summary>
        /// Scrolls right within the content by one logical unit.
        /// </summary>
        void LineRight();

        /// <summary>
        /// Scrolls up within the content by one logical unit.
        /// </summary>
        void LineUp();

        /// <summary>
        /// Scrolls down within the content after the user clicks the wheel button on a mouse.
        /// </summary>
        void MouseWheelDown();

        /// <summary>
        /// Scrolls left within the content after the user clicks the wheel button on a mouse.
        /// </summary>
        void MouseWheelLeft();

        /// <summary>
        /// Scrolls right within the content after the user clicks the wheel button on a
        /// mouse.
        /// </summary>
        void MouseWheelRight();

        /// <summary>
        /// Scrolls up within the content after the user clicks the wheel button on a mouse.
        /// </summary>
        void MouseWheelUp();

        /// <summary>
        /// Scrolls down within the content by one page.
        /// </summary>
        void PageDown();

        /// <summary>
        /// Scrolls left within the content by one page.
        /// </summary>
        void PageLeft();

        /// <summary>
        /// Scrolls right within the content by one page.
        /// </summary>
        void PageRight();

        /// <summary>
        /// Scrolls up within the content by one page.
        /// </summary>
        void PageUp();

        /// <summary>
        /// Forces content to scroll until the coordinate space of a visual object is visible.
        /// </summary>
        /// <param name="visual">
        /// A <see cref="UIElement"/> that becomes visible.
        /// </param>
        /// <param name="rectangle">
        /// A bounding rectangle that identifies the coordinate space to make visible.
        /// </param>
        /// <returns>
        /// A <see cref="Rect"/> that is visible.
        /// </returns>
        Rect MakeVisible(UIElement visual, Rect rectangle);

        /// <summary>
        /// Sets the amount of horizontal offset.
        /// </summary>
        /// <param name="offset">
        /// The amount that content is horizontally offset from the containing viewport.
        /// </param>
        void SetHorizontalOffset(double offset);

        /// <summary>
        /// Sets the amount of vertical offset.
        /// </summary>
        /// <param name="offset">
        /// The amount that content is vertically offset from the containing viewport.
        /// </param>
        void SetVerticalOffset(double offset);

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value that indicates whether scrolling on the vertical axis is
        /// possible.
        /// </summary>
        /// <returns>
        /// true if scrolling is possible; otherwise false. This property has no default
        /// value.
        /// </returns>
        bool CanVerticallyScroll { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether scrolling on the horizontal axis
        /// is possible.
        /// </summary>
        /// <returns>
        /// true if scrolling is possible; otherwise false. This property has no default
        /// value.
        /// </returns>
        bool CanHorizontallyScroll { get; set; }

        /// <summary>
        /// Gets the horizontal size of the extent.
        /// </summary>
        /// <returns>
        /// A <see cref="double"/> that represents, in device independent pixels, the horizontal
        /// size of the extent. This property has no default value.
        /// </returns>
        double ExtentWidth { get; }

        /// <summary>
        /// Gets the vertical size of the extent.
        /// </summary>
        /// <returns>
        /// A <see cref="double"/> that represents, in device independent pixels, the vertical size
        /// of the extent. This property has no default value.
        /// </returns>
        double ExtentHeight { get; }

        /// <summary>
        /// Gets the horizontal size of the viewport for this content.
        /// </summary>
        /// <returns>
        /// A <see cref="double"/> that represents, in device independent pixels, the horizontal
        /// size of the viewport for this content. This property has no default value.
        /// </returns>
        double ViewportWidth { get; }

        /// <summary>
        /// Gets the vertical size of the viewport for this content.
        /// </summary>
        /// <returns>
        /// A <see cref="double"/> that represents, in device independent pixels, the vertical size
        /// of the viewport for this content. This property has no default value.
        /// </returns>
        double ViewportHeight { get; }

        /// <summary>
        /// Gets the horizontal offset of the scrolled content.
        /// </summary>
        /// <returns>
        /// A <see cref="double"/> that represents, in device independent pixels, the horizontal
        /// offset. Valid values are between zero and the <see cref="ExtentWidth"/>
        /// minus the <see cref="ViewportWidth"/>. This property has no default value.
        /// </returns>
        double HorizontalOffset { get; }

        /// <summary>
        /// Gets the vertical offset of the scrolled content.
        /// </summary>
        /// <returns>
        /// A <see cref="double"/> that represents, in device independent pixels, the vertical offset
        /// of the scrolled content. Valid values are between zero and the <see cref="ExtentHeight"/>
        /// minus the <see cref="ViewportHeight"/>. This property has no default value.
        /// </returns>
        double VerticalOffset { get; }

        /// <summary>
        /// Gets or sets a <see cref="ScrollViewer"/> element that controls scrolling
        /// behavior.
        /// </summary>
        /// <returns>
        /// A <see cref="ScrollViewer"/> element that controls scrolling behavior.
        /// This property has no default value.
        /// </returns>
        ScrollViewer ScrollOwner { get; set; }

        #endregion
    }
}
