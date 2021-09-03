using System;
using System.Collections.Generic;
using System.Text;

#if MIGRATION
namespace System.Windows.Automation.Provider
#else
namespace Windows.UI.Xaml.Automation.Provider
#endif
{
	public partial interface IScrollProvider
	{
		//
		// Summary:
		//     Gets a value that indicates whether the control can scroll horizontally.
		//
		// Returns:
		//     true if the control can scroll horizontally; otherwise, false.
		bool HorizontallyScrollable
		{
			get;
		}

		//
		// Summary:
		//     Gets the current horizontal scroll position.
		//
		// Returns:
		//     The horizontal scroll position as a percentage of the total content area within
		//     the control.
		double HorizontalScrollPercent
		{
			get;
		}

		//
		// Summary:
		//     Gets the current horizontal view size.
		//
		// Returns:
		//     The horizontal size of the viewable region as a percentage of the total content
		//     area within the control.
		double HorizontalViewSize
		{
			get;
		}

		//
		// Summary:
		//     Gets a value that indicates whether the control can scroll vertically.
		//
		// Returns:
		//     true if the control can scroll vertically; otherwise, false.
		bool VerticallyScrollable
		{
			get;
		}

		//
		// Summary:
		//     Gets the current vertical scroll position.
		//
		// Returns:
		//     The vertical scroll position as a percentage of the total content area within
		//     the control.
		double VerticalScrollPercent
		{
			get;
		}

		//
		// Summary:
		//     Gets the vertical view size.
		//
		// Returns:
		//     The vertical size of the viewable region as a percentage of the total content
		//     area within the control.
		double VerticalViewSize
		{
			get;
		}

		//
		// Summary:
		//     Scrolls the visible region of the content area horizontally, vertically, or both.
		//
		// Parameters:
		//   horizontalAmount:
		//     The horizontal increment that is specific to the control. Pass System.Windows.Automation.ScrollPatternIdentifiers.NoScroll
		//     if the control cannot be scrolled in this direction.
		//
		//   verticalAmount:
		//     The vertical increment that is specific to the control. Pass System.Windows.Automation.ScrollPatternIdentifiers.NoScroll
		//     if the control cannot be scrolled in this direction.
		void Scroll(ScrollAmount horizontalAmount, ScrollAmount verticalAmount);
		//
		// Summary:
		//     Sets the horizontal and vertical scroll position as a percentage of the total
		//     content area within the control.
		//
		// Parameters:
		//   horizontalPercent:
		//     The horizontal position as a percentage of the content area's total range. Pass
		//     System.Windows.Automation.ScrollPatternIdentifiers.NoScroll if the control cannot
		//     be scrolled in this direction.
		//
		//   verticalPercent:
		//     The vertical position as a percentage of the content area's total range. Pass
		//     System.Windows.Automation.ScrollPatternIdentifiers.NoScroll if the control cannot
		//     be scrolled in this direction.
		void SetScrollPercent(double horizontalPercent, double verticalPercent);
	}
}
