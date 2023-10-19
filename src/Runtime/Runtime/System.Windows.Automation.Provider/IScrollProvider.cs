
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

namespace System.Windows.Automation.Provider
{
    /// <summary>
    /// Exposes methods and properties to support access by a UI automation client to a 
    /// control that acts as a scrollable container for a collection of child objects. 
    /// The children of this control must implement <see cref="IScrollItemProvider" />.
    /// </summary>
    public interface IScrollProvider
    {
        /// <summary>
        /// Gets a value that indicates whether the control can scroll horizontally.
        /// </summary>
        /// <returns>
        /// true if the control can scroll horizontally; otherwise, false.
        /// </returns>
        bool HorizontallyScrollable { get; }

        /// <summary>
        /// Gets the current horizontal scroll position.
        /// </summary>
        /// <returns>
        /// The horizontal scroll position as a percentage of the total content area 
        /// within the control.
        /// </returns>
        double HorizontalScrollPercent { get; }

        /// <summary>
        /// Gets the current horizontal view size.
        /// </summary>
        /// <returns>
        /// The horizontal size of the viewable region as a percentage of the total 
        /// content area within the control.
        /// </returns>
        double HorizontalViewSize { get; }

        /// <summary>
        /// Scrolls the visible region of the content area horizontally, vertically, or both.
        /// </summary>
        /// <param name="horizontalAmount">
        /// The horizontal increment that is specific to the control. Pass 
        /// <see cref="ScrollPatternIdentifiers.NoScroll" /> if the control cannot 
        /// be scrolled in this direction.
        /// </param>
        /// <param name="verticalAmount">
        /// The vertical increment that is specific to the control. Pass 
        /// <see cref="ScrollPatternIdentifiers.NoScroll" /> if the control cannot 
        /// be scrolled in this direction.
        /// </param>
        void Scroll(ScrollAmount horizontalAmount, ScrollAmount verticalAmount);

        /// <summary>
        /// Sets the horizontal and vertical scroll position as a percentage of the total content 
        /// area within the control.
        /// </summary>
        /// <param name="horizontalPercent">
        /// The horizontal position as a percentage of the content area's total range. Pass 
        /// <see cref="ScrollPatternIdentifiers.NoScroll" /> if the control cannot be scrolled in 
        /// this direction.
        /// </param>
        /// <param name="verticalPercent">
        /// The vertical position as a percentage of the content area's total range. Pass 
        /// <see cref="ScrollPatternIdentifiers.NoScroll" /> if the control cannot be scrolled in 
        /// this direction.
        /// </param>
        void SetScrollPercent(double horizontalPercent, double verticalPercent);

        /// <summary>
        /// Gets a value that indicates whether the control can scroll vertically.
        /// </summary>
        /// <returns>
        /// true if the control can scroll vertically; otherwise, false.
        /// </returns>
        bool VerticallyScrollable { get; }

        /// <summary>
        /// Gets the current vertical scroll position.
        /// </summary>
        /// <returns>
        /// The vertical scroll position as a percentage of the total content area within the control.
        /// </returns>
        double VerticalScrollPercent { get; }

        /// <summary>
        /// Gets the vertical view size.
        /// </summary>
        /// <returns>
        /// The vertical size of the viewable region as a percentage of the total content area within the control.
        /// </returns>
        double VerticalViewSize { get; }
    }
}
