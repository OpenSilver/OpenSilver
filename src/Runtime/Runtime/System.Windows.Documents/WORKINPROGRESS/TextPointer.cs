namespace System.Windows.Documents
{
	//
	// Summary:
	//     Represents a position within a System.Windows.Controls.RichTextBox.
    [OpenSilver.NotImplemented]
	public class TextPointer
	{
		//
		// Summary:
		//     Gets a value that indicates whether the current position is an insertion.
		//
		// Returns:
		//     true if the current position is an insertion position; otherwise, false.
        [OpenSilver.NotImplemented]
		public bool IsAtInsertionPosition { get; }
		//
		// Summary:
		//     Gets the logical direction associated with the current position, which is used
		//     to disambiguate content associated with the current position.
		//
		// Returns:
		//     The System.Windows.Documents.LogicalDirection value that is associated with the
		//     current position.
        [OpenSilver.NotImplemented]
		public LogicalDirection LogicalDirection { get; }
		//
		// Summary:
		//     Gets the logical parent that contains the current position.
		//
		// Returns:
		//     The logical parent that contains the current position. Can return the System.Windows.Controls.RichTextBox
		//     when at the top of the content stack.
        [OpenSilver.NotImplemented]
		public DependencyObject Parent { get; }
		//
		// Summary:
		//     Gets the visual tree parent of the System.Windows.Documents.TextPointer object.
		//
		// Returns:
		//     The visual tree parent of the System.Windows.Documents.TextPointer object.
        [OpenSilver.NotImplemented]
		public FrameworkElement VisualParent { get; }

		//
		// Summary:
		//     Performs an ordinal comparison between the positions specified by the current
		//     System.Windows.Documents.TextPointer and a second specified System.Windows.Documents.TextPointer.
		//
		// Parameters:
		//   position:
		//     A System.Windows.Documents.TextPointer that specifies a position to compare to
		//     the current position.
		//
		// Returns:
		//     -1 if the current System.Windows.Documents.TextPointer precedes position; 0 if
		//     the locations are the same; +1 if the current System.Windows.Documents.TextPointer
		//     follows positions.
		//
		// Exceptions:
		//   T:System.ArgumentException:
		//     position specifies a position from a different System.Windows.Controls.RichTextBox
		//     associated with the current position.
        [OpenSilver.NotImplemented]
		public int CompareTo(TextPointer position)
		{
			return default(int);
		}
		
		//
		// Summary:
		//     Returns a bounding box for content that borders the current System.Windows.Documents.TextPointer
		//     in the specified logical direction.
		//
		// Parameters:
		//   direction:
		//     One of the System.Windows.Documents.LogicalDirection values that specify the
		//     logical direction in which to find a content bounding box.
		//
		// Returns:
		//     A System.Windows.Rect for content that borders the current System.Windows.Documents.TextPointer
		//     in the specified direction, or System.Windows.Rect.Empty if current and valid
		//     layout information is unavailable.
        [OpenSilver.NotImplemented]
		public Rect GetCharacterRect(LogicalDirection direction)
		{
			return default(Rect);
		}
		
		//
		// Summary:
		//     Returns a System.Windows.Documents.TextPointer to the next insertion position
		//     in the specified logical direction.
		//
		// Parameters:
		//   direction:
		//     One of the System.Windows.Documents.LogicalDirection values that specify the
		//     logical direction in which to search for the next insertion position.
		//
		// Returns:
		//     A System.Windows.Documents.TextPointer that identifies the next insertion position
		//     in the requested direction, or null if no next insertion position can be found.
        [OpenSilver.NotImplemented]
		public TextPointer GetNextInsertionPosition(LogicalDirection direction)
		{
			return default(TextPointer);
		}
		
		//
		// Summary:
		//     Returns a System.Windows.Documents.TextPointer to the position indicated by the
		//     specified offset, in symbols, from the beginning of the current System.Windows.Documents.TextPointer
		//     and in the specified direction.
		//
		// Parameters:
		//   offset:
		//     An offset, in symbols, for which to calculate and return the position. If the
		//     offset is negative, the returned System.Windows.Documents.TextPointer precedes
		//     the current System.Windows.Documents.TextPointer; otherwise, it follows.
		//
		//   direction:
		//     One of the System.Windows.Documents.LogicalDirection values that specifies the
		//     logical direction of the returned System.Windows.Documents.TextPointer.
		//
		// Returns:
		//     A System.Windows.Documents.TextPointer to the position indicated by the specified
		//     offset and in the direction specified by the direction parameter, or null if
		//     the offset extends past the end of the content.
        [OpenSilver.NotImplemented]
		public TextPointer GetPositionAtOffset(int offset, LogicalDirection direction)
		{
			return default(TextPointer);
		}
	}
}
