#if !MIGRATION
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows.Automation.Provider
#else
namespace Windows.UI.Xaml.Automation.Provider
#endif
{
	//
	// Summary:
	//     Exposes methods and properties to support UI Automation client access to controls
	//     that contain text.
	public partial interface ITextProvider
	{
		//
		// Summary:
		//     Gets a text range that encloses the main text of a document.
		//
		// Returns:
		//     A text range that encloses the main text of a document.
		ITextRangeProvider DocumentRange
		{
			get;
		}

		//
		// Summary:
		//     Gets a value that specifies whether a text provider supports selection, and if
		//     it does, the type of selection that is supported.
		//
		// Returns:
		//     A value of System.Windows.Automation.SupportedTextSelection.
		SupportedTextSelection SupportedTextSelection
		{
			get;
		}

		//
		// Summary:
		//     Retrieves a collection of disjoint text ranges that are associated with the current
		//     text selection or selections.
		//
		// Returns:
		//     A collection of disjoint text ranges.
		//
		// Exceptions:
		//   T:System.InvalidOperationException:
		//     If the UI Automation provider does not support text selection.
		ITextRangeProvider[] GetSelection();
		//
		// Summary:
		//     Retrieves an array of disjoint text ranges from a text container. Each text range
		//     begins with the first partially visible line and ends with the last partially
		//     visible line.
		//
		// Returns:
		//     The collection of visible text ranges within a container or an empty array. This
		//     method never returns null.
		ITextRangeProvider[] GetVisibleRanges();
		//
		// Summary:
		//     Retrieves a text range that encloses a child element, such as an image, hyperlink,
		//     or other embedded object.
		//
		// Parameters:
		//   childElement:
		//     The enclosed object.
		//
		// Returns:
		//     A range that spans the child element.
		ITextRangeProvider RangeFromChild(IRawElementProviderSimple childElement);
		//
		// Summary:
		//     Returns the degenerate (empty) text range that is nearest to the specified screen
		//     coordinates.
		//
		// Parameters:
		//   screenLocation:
		//     The location, in screen coordinates.
		//
		// Returns:
		//     A degenerate range nearest the specified location. This method never returns
		//     null.
		ITextRangeProvider RangeFromPoint(Point screenLocation);
	}
}
