#if MIGRATION
using System.Windows.Automation.Text;
#else
using Windows.UI.Xaml.Automation.Text;
#endif

#if MIGRATION
namespace System.Windows.Automation.Provider
#else
namespace Windows.UI.Xaml.Automation.Provider
#endif
{
	//
	// Summary:
	//     Exposes methods and properties to support UI automation client access to a span
	//     of continuous text in a text container that implements System.Windows.Automation.Provider.ITextProvider.
	public partial interface ITextRangeProvider
	{
		//
		// Summary:
		//     Adds to the collection of highlighted text in a text container that supports
		//     multiple disjoint selections.
		//
		// Exceptions:
		//   T:System.InvalidOperationException:
		//     The text provider does not support multiple disjoint selections.
		void AddToSelection();
		//
		// Summary:
		//     Returns a new System.Windows.Automation.Provider.ITextRangeProvider that is identical
		//     to the original System.Windows.Automation.Provider.ITextRangeProvider and that
		//     inherits all the properties of the original.
		//
		// Returns:
		//     The new text range. This method never returns null.
		ITextRangeProvider Clone();
		//
		// Summary:
		//     Returns a value that indicates whether the span (the System.Windows.Automation.Text.TextPatternRangeEndpoint.Start
		//     endpoint through the System.Windows.Automation.Text.TextPatternRangeEndpoint.End
		//     endpoint) of a text range is the same as another text range.
		//
		// Parameters:
		//   range:
		//     A text range to compare.
		//
		// Returns:
		//     true if the span of both text ranges is identical; otherwise, false.
		//
		// Exceptions:
		//   T:System.ArgumentException:
		//     The range to compare does not come from the same text provider.
		bool Compare(ITextRangeProvider range);
		//
		// Summary:
		//     Returns a value that specifies whether two text ranges have identical endpoints.
		//
		// Parameters:
		//   endpoint:
		//     The System.Windows.Automation.Text.TextPatternRangeEndpoint.Start or System.Windows.Automation.Text.TextPatternRangeEndpoint.End
		//     endpoint of the caller.
		//
		//   targetRange:
		//     The target range for comparison.
		//
		//   targetEndpoint:
		//     The System.Windows.Automation.Text.TextPatternRangeEndpoint.Start or System.Windows.Automation.Text.TextPatternRangeEndpoint.End
		//     endpoint of the target.
		//
		// Returns:
		//     Returns a negative value if the caller's endpoint occurs earlier in the text
		//     than the target endpoint. Returns zero if the caller's endpoint is at the same
		//     location as the target endpoint. Returns a positive value if the caller's endpoint
		//     occurs later in the text than the target endpoint.
		//
		// Exceptions:
		//   T:System.ArgumentException:
		//     The targetRange is from a different text provider.
		int CompareEndpoints(TextPatternRangeEndpoint endpoint, ITextRangeProvider targetRange, TextPatternRangeEndpoint targetEndpoint);
		//
		// Summary:
		//     Expands the text range to the specified text unit.
		//
		// Parameters:
		//   unit:
		//     The textual unit.
		void ExpandToEnclosingUnit(TextUnit unit);
		//
		// Summary:
		//     Returns a text range subset that has the specified attribute ID and attribute
		//     value.
		//
		// Parameters:
		//   attributeId:
		//     The attribute ID to search for.
		//
		//   value:
		//     The attribute value to search for. This value must match the specified attribute
		//     type.
		//
		//   backward:
		//     true if the last occurring text range should be returned instead of the first;
		//     otherwise, false.
		//
		// Returns:
		//     A text range that has a matching attribute ID and attribute value; otherwise,
		//     null.
		ITextRangeProvider FindAttribute(int attributeId, object value, bool backward);
		//
		// Summary:
		//     Returns a text range subset that contains the specified text.
		//
		// Parameters:
		//   text:
		//     The text string to search for.
		//
		//   backward:
		//     true to return the last occurring text range instead of the first; otherwise,
		//     false.
		//
		//   ignoreCase:
		//     true to ignore case; otherwise, false.
		//
		// Returns:
		//     A text range that matches the specified text; otherwise, null.
		ITextRangeProvider FindText(string text, bool backward, bool ignoreCase);
		//
		// Summary:
		//     Retrieves the value of the specified attribute ID across the text range.
		//
		// Parameters:
		//   attributeId:
		//     The text attribute ID.
		//
		// Returns:
		//     Retrieves an object that represents the value of the specified attribute. See
		//     Remarks.
		//
		// Exceptions:
		//   T:System.ArgumentException:
		//     The specified attribute ID is not valid.
		object GetAttributeValue(int attributeId);
		//
		// Summary:
		//     Retrieves a collection of bounding rectangles for each fully or partially visible
		//     line of text in a text range.
		//
		// Returns:
		//     An array of bounding rectangles for each full or partial line of text in a text
		//     range. See Remarks.
		double[] GetBoundingRectangles();
		//
		// Summary:
		//     Retrieves a collection of all the embedded objects that exist within the text
		//     range.
		//
		// Returns:
		//     A collection of child objects that exist within the range. Child objects that
		//     overlap with the text range but are not completely enclosed by it are also included
		//     in the collection. Returns an empty collection if no child objects exist.
		IRawElementProviderSimple[] GetChildren();
		//
		// Summary:
		//     Returns the innermost element that encloses the text range.
		//
		// Returns:
		//     The enclosing control, typically the text provider that provides the text range.
		//     However, if the text provider supports child text elements such as tables or
		//     hyperlinks, the enclosing element can be a descendant of the text provider.
		IRawElementProviderSimple GetEnclosingElement();
		//
		// Summary:
		//     Retrieves the plain text of the range.
		//
		// Parameters:
		//   maxLength:
		//     The maximum length of the string to return. Use -1 to specify an unlimited length.
		//
		// Returns:
		//     The plain text of the text range, which might represent a portion of the full
		//     string truncated at the specified maxLength.
		//
		// Exceptions:
		//   T:System.ArgumentOutOfRangeException:
		//     maxLength is less than -1.
		string GetText(int maxLength);
		//
		// Summary:
		//     Moves the text range the specified number of text units.
		//
		// Parameters:
		//   unit:
		//     The text unit boundary.
		//
		//   count:
		//     The number of text units to move.A positive value moves the text range forward;
		//     a negative value moves the text range backward; and a value of 0 has no effect.
		//
		// Returns:
		//     The number of units actually moved. This value can be less than the count requested
		//     if either of the new text range endpoints is greater than or less than the System.Windows.Automation.Provider.ITextProvider.DocumentRange
		//     endpoints.
		int Move(TextUnit unit, int count);
		//
		// Summary:
		//     Moves one endpoint of a text range to the specified endpoint of a second text
		//     range.
		//
		// Parameters:
		//   endpoint:
		//     The endpoint to move.
		//
		//   targetRange:
		//     Another range from the same text provider.
		//
		//   targetEndpoint:
		//     An endpoint on the other range.
		void MoveEndpointByRange(TextPatternRangeEndpoint endpoint, ITextRangeProvider targetRange, TextPatternRangeEndpoint targetEndpoint);
		//
		// Summary:
		//     Moves one endpoint of the text range the specified number of text units within
		//     the document range.
		//
		// Parameters:
		//   endpoint:
		//     The endpoint to move.
		//
		//   unit:
		//     The textual unit for moving.
		//
		//   count:
		//     The number of units to move. A positive value moves the endpoint forward; a negative
		//     value moves the endpoint backward; and a value of 0 has no effect.
		//
		// Returns:
		//     The number of units actually moved. This value can be less than the count requested
		//     if moving the endpoint extends beyond the start or end of the document.
		int MoveEndpointByUnit(TextPatternRangeEndpoint endpoint, TextUnit unit, int count);
		//
		// Summary:
		//     From the collection of highlighted text in a text container that supports multiple
		//     disjoint selections, removes a highlighted section of text that corresponds to
		//     the caller's System.Windows.Automation.Text.TextPatternRangeEndpoint.Start and
		//     System.Windows.Automation.Text.TextPatternRangeEndpoint.End endpoints.
		//
		// Exceptions:
		//   T:System.InvalidOperationException:
		//     The text provider does not support multiple disjoint .
		void RemoveFromSelection();
		//
		// Summary:
		//     Causes the text control to scroll vertically until the text range is visible
		//     in the viewport.
		//
		// Parameters:
		//   alignToTop:
		//     true if the text control should be scrolled so that the text range is flush with
		//     the top of the viewport; false if the text range is flush with the bottom of
		//     the viewport.
		void ScrollIntoView(bool alignToTop);
		//
		// Summary:
		//     Highlights text in the text control that corresponds to the System.Windows.Automation.Text.TextPatternRangeEndpoint.Start
		//     and System.Windows.Automation.Text.TextPatternRangeEndpoint.End endpoints of
		//     the text range.
		//
		// Exceptions:
		//   T:System.InvalidOperationException:
		//     Text selection is not supported by the text control.
		void Select();
	}
}
