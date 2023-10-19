
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

using System.Windows.Automation.Text;

namespace System.Windows.Automation.Provider
{
    /// <summary>
    /// Exposes methods and properties to support UI automation client access to a span 
    /// of continuous text in a text container that implements <see cref="ITextProvider" />.
    /// </summary>
    public interface ITextRangeProvider
    {
        /// <summary>
        /// Returns a new <see cref="ITextRangeProvider" /> that is identical to the original 
        /// <see cref="ITextRangeProvider" /> and that inherits all the properties of the original.
        /// </summary>
        /// <returns>
        /// The new text range. This method never returns null.
        /// </returns>
        ITextRangeProvider Clone();

        /// <summary>
        /// Returns a value that indicates whether the span (the <see cref="TextPatternRangeEndpoint.Start" /> 
        /// endpoint through the <see cref="TextPatternRangeEndpoint.End" /> endpoint) of a text range is the 
        /// same as another text range.
        /// </summary>
        /// <param name="range">
        /// A text range to compare.
        /// </param>
        /// <returns>
        /// true if the span of both text ranges is identical; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The range to compare does not come from the same text provider.
        /// </exception>
        bool Compare(ITextRangeProvider range);

        /// <summary>
        /// Returns a value that specifies whether two text ranges have identical endpoints.
        /// </summary>
        /// <returns>
        /// Returns a negative value if the caller's endpoint occurs earlier in the text than the target 
        /// endpoint. Returns zero if the caller's endpoint is at the same location as the target endpoint.
        /// Returns a positive value if the caller's endpoint occurs later in the text than the target endpoint.
        /// </returns>
        /// <param name="endpoint">
        /// The <see cref="TextPatternRangeEndpoint.Start" /> or <see cref="TextPatternRangeEndpoint.End" /> 
        /// endpoint of the caller.
        /// </param>
        /// <param name="targetRange">
        /// The target range for comparison.
        /// </param>
        /// <param name="targetEndpoint">
        /// The <see cref="TextPatternRangeEndpoint.Start" /> or <see cref="TextPatternRangeEndpoint.End" /> 
        /// endpoint of the target.
        /// </param>
        /// <exception cref="ArgumentException">
        /// The <paramref name="targetRange" /> is from a different text provider.
        /// </exception>
        int CompareEndpoints(TextPatternRangeEndpoint endpoint, ITextRangeProvider targetRange, TextPatternRangeEndpoint targetEndpoint);

        /// <summary>
        /// Expands the text range to the specified text unit.
        /// </summary>
        /// <param name="unit">
        /// The textual unit.
        /// </param>
        void ExpandToEnclosingUnit(TextUnit unit);

        /// <summary>
        /// Returns a text range subset that has the specified attribute ID and attribute value.
        /// </summary>
        /// <param name="attributeId">
        /// The attribute ID to search for.
        /// </param>
        /// <param name="value">
        /// The attribute value to search for. This value must match the specified attribute type.
        /// </param>
        /// <param name="backward">
        /// true if the last occurring text range should be returned instead of the first; otherwise, false.
        /// </param>
        /// <returns>
        /// A text range that has a matching attribute ID and attribute value; otherwise, null.
        /// </returns>
        ITextRangeProvider FindAttribute(int attributeId, object value, bool backward);

        /// <summary>
        /// Returns a text range subset that contains the specified text.
        /// </summary>
        /// <param name="text">
        /// The text string to search for.
        /// </param>
        /// <param name="backward">
        /// true to return the last occurring text range instead of the first; otherwise, false.
        /// </param>
        /// <param name="ignoreCase">
        /// true to ignore case; otherwise, false.
        /// </param>
        /// <returns>
        /// A text range that matches the specified text; otherwise, null.
        /// </returns>
        ITextRangeProvider FindText(string text, bool backward, bool ignoreCase);

        /// <summary>
        /// Retrieves the value of the specified attribute ID across the text range.
        /// </summary>
        /// <param name="attributeId">
        /// The text attribute ID.
        /// </param>
        /// <returns>
        /// Retrieves an object that represents the value of the specified attribute. See Remarks.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The specified attribute ID is not valid.
        /// </exception>
        object GetAttributeValue(int attributeId);

        /// <summary>
        /// Retrieves a collection of bounding rectangles for each fully or partially visible line 
        /// of text in a text range.
        /// </summary>
        /// <returns>
        /// An array of bounding rectangles for each full or partial line of text in a text range.
        /// See Remarks.
        /// </returns>
        /// <remarks>
        /// This method returns an empty array for a degenerate range.
        /// This method returns an empty array for a text range that has screen coordinates that place 
        /// it completely off-screen, are scrolled out of view, or are obscured by an overlapping window.
        /// </remarks>
        double[] GetBoundingRectangles();

        /// <summary>
        /// Returns the innermost element that encloses the text range.
        /// </summary>
        /// <returns>
        /// The enclosing control, typically the text provider that provides the text range. However, 
        /// if the text provider supports child text elements such as tables or hyperlinks, the 
        /// enclosing element can be a descendant of the text provider.
        /// </returns>
        IRawElementProviderSimple GetEnclosingElement();

        /// <summary>
        /// Retrieves the plain text of the range.
        /// </summary>
        /// <param name="maxLength">
        /// The maximum length of the string to return. Use -1 to specify an unlimited length.
        /// </param>
        /// <returns>
        /// The plain text of the text range, which might represent a portion of the full string 
        /// truncated at the specified <paramref name="maxLength" />.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="maxLength" /> is less than -1.
        /// </exception>
        string GetText(int maxLength);

        /// <summary>
        /// Moves the text range the specified number of text units.
        /// </summary>
        /// <param name="unit">
        /// The text unit boundary.
        /// </param>
        /// <param name="count">
        /// The number of text units to move.A positive value moves the text range forward; a 
        /// negative value moves the text range backward; and a value of 0 has no effect.
        /// </param>
        /// <returns>
        /// The number of units actually moved. This value can be less than the 
        /// <paramref name="count" /> requested if either of the new text range endpoints is 
        /// greater than or less than the <see cref="ITextProvider.DocumentRange" /> endpoints.
        /// </returns>
        int Move(TextUnit unit, int count);

        /// <summary>
        /// Moves one endpoint of the text range the specified number of text units within 
        /// the document range.
        /// </summary>
        /// <param name="endpoint">
        /// The endpoint to move.
        /// </param>
        /// <param name="unit">
        /// The textual unit for moving.
        /// </param>
        /// <param name="count">
        /// The number of units to move. A positive value moves the endpoint forward; a negative 
        /// value moves the endpoint backward; and a value of 0 has no effect.
        /// </param>
        /// <returns>
        /// The number of units actually moved. This value can be less than the <paramref name="count" /> 
        /// requested if moving the endpoint extends beyond the start or end of the document.
        /// </returns>
        int MoveEndpointByUnit(TextPatternRangeEndpoint endpoint, TextUnit unit, int count);

        /// <summary>
        /// Moves one endpoint of a text range to the specified endpoint of a second text range.
        /// </summary>
        /// <param name="endpoint">
        /// The endpoint to move.
        /// </param>
        /// <param name="targetRange">
        /// Another range from the same text provider.
        /// </param>
        /// <param name="targetEndpoint">
        /// An endpoint on the other range.
        /// </param>
        void MoveEndpointByRange(TextPatternRangeEndpoint endpoint, ITextRangeProvider targetRange, TextPatternRangeEndpoint targetEndpoint);

        /// <summary>
        /// Highlights text in the text control that corresponds to the 
        /// <see cref="TextPatternRangeEndpoint.Start" /> and <see cref="TextPatternRangeEndpoint.End" /> 
        /// endpoints of the text range.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Text selection is not supported by the text control.
        /// </exception>
        void Select();

        /// <summary>
        /// Adds to the collection of highlighted text in a text container that supports multiple 
        /// disjoint selections.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The text provider does not support multiple disjoint selections.
        /// </exception>
        void AddToSelection();

        /// <summary>
        /// From the collection of highlighted text in a text container that supports multiple disjoint 
        /// selections, removes a highlighted section of text that corresponds to the caller's 
        /// <see cref="TextPatternRangeEndpoint.Start" /> and <see cref="TextPatternRangeEndpoint.End" /> 
        /// endpoints.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The text provider does not support multiple disjoint.
        /// </exception>
        void RemoveFromSelection();

        /// <summary>
        /// Causes the text control to scroll vertically until the text range is visible in the viewport.
        /// </summary>
        /// <param name="alignToTop">
        /// true if the text control should be scrolled so that the text range is flush with the top of 
        /// the viewport; false if the text range is flush with the bottom of the viewport.
        /// </param>
        void ScrollIntoView(bool alignToTop);

        /// <summary>
        /// Retrieves a collection of all the embedded objects that exist within the text range.
        /// </summary>
        /// <returns>
        /// A collection of child objects that exist within the range. Child objects that overlap with 
        /// the text range but are not completely enclosed by it are also included in the collection. 
        /// Returns an empty collection if no child objects exist.
        /// </returns>
        IRawElementProviderSimple[] GetChildren();
    }
}
