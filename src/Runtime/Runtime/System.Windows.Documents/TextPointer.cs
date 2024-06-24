
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

using System.Diagnostics;
using System.Windows.Controls;

namespace System.Windows.Documents;

/// <summary>
/// Represents a position within a <see cref="RichTextBox"/>.
/// </summary>
public class TextPointer
{
    private readonly int _offset;

    internal TextPointer(FrameworkElement visualParent, int offset, LogicalDirection logicalDirection)
    {
        Debug.Assert(visualParent is not null);
        VisualParent = visualParent;
        LogicalDirection = logicalDirection;
        _offset = offset;
    }

    internal int Offset
    {
        get
        {
            return VisualParent switch
            {
                RichTextBox richTextBox => Math.Min(_offset, richTextBox.ContentEnd._offset),
                _ => _offset,
            };
        }
    }

    /// <summary>
    /// Gets a value that indicates whether the current position is an insertion.
    /// </summary>
    /// <returns>
    /// true if the current position is an insertion position; otherwise, false.
    /// </returns>
    public bool IsAtInsertionPosition => true;

    /// <summary>
    /// Gets the logical direction associated with the current position, which is used to 
    /// disambiguate content associated with the current position.
    /// </summary>
    /// <returns>
    /// The <see cref="Documents.LogicalDirection"/> value that is associated with the current position.
    /// </returns>
    public LogicalDirection LogicalDirection { get; }

    /// <summary>
    /// Gets the logical parent that contains the current position.
    /// </summary>
    /// <returns>
    /// The logical parent that contains the current position. Can return the <see cref="RichTextBox"/>
    /// when at the top of the content stack.
    /// </returns>
    [OpenSilver.NotImplemented]
    public DependencyObject Parent => VisualParent;

    /// <summary>
    /// Gets the visual tree parent of the <see cref="TextPointer"/> object.
    /// </summary>
    /// <returns>
    /// The visual tree parent of the <see cref="TextPointer"/> object.
    /// </returns>
    public FrameworkElement VisualParent { get; }

    /// <summary>
    /// Performs an ordinal comparison between the positions specified by the current
    /// <see cref="TextPointer"/> and a second specified <see cref="TextPointer"/>.
    /// </summary>
    /// <param name="position">
    /// A <see cref="TextPointer"/> that specifies a position to compare to the current 
    /// position.
    /// </param>
    /// <returns>
    /// -1 if the current <see cref="TextPointer"/> precedes position; 0 if the locations 
    /// are the same; +1 if the current <see cref="TextPointer"/> follows positions.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// position is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// position specifies a position from a different <see cref="RichTextBox"/> associated 
    /// with the current position.
    /// </exception>
    public int CompareTo(TextPointer position)
    {
        if (position is null)
        {
            throw new ArgumentNullException(nameof(position));
        }

        if (VisualParent != position.VisualParent)
        {
            throw new ArgumentException("position does not belong to the same visual parent.", nameof(position));
        }

        return Offset.CompareTo(position.Offset);
    }

    /// <summary>
    /// Returns a bounding box for content that borders the current <see cref="TextPointer"/>
    /// in the specified logical direction.
    /// </summary>
    /// <param name="direction">
    /// One of the <see cref="Documents.LogicalDirection"/> values that specify the logical direction 
    /// in which to find a content bounding box.
    /// </param>
    /// <returns>
    /// A <see cref="Rect"/> for content that borders the current <see cref="TextPointer"/>
    /// in the specified direction, or <see cref="Rect.Empty"/> if current and valid layout 
    /// information is unavailable.
    /// </returns>
    [OpenSilver.NotImplemented]
    public Rect GetCharacterRect(LogicalDirection direction)
    {
        return default(Rect);
    }

    /// <summary>
    /// Returns a <see cref="TextPointer"/> to the next insertion position in the 
    /// specified logical direction.
    /// </summary>
    /// <param name="direction">
    /// One of the <see cref="Documents.LogicalDirection"/> values that specify the logical 
    /// direction in which to search for the next insertion position.
    /// </param>
    /// <returns>
    /// A <see cref="TextPointer"/> that identifies the next insertion position in 
    /// the requested direction, or null if no next insertion position can be found.
    /// </returns>
    public TextPointer GetNextInsertionPosition(LogicalDirection direction) =>
        GetPositionAtOffset(direction == LogicalDirection.Forward ? 1 : -1, direction);

    /// <summary>
    /// Returns a <see cref="TextPointer"/> to the position indicated by the specified 
    /// offset, in symbols, from the beginning of the current <see cref="TextPointer"/>
    /// and in the specified direction.
    /// </summary>
    /// <param name="offset">
    /// An offset, in symbols, for which to calculate and return the position. If the
    /// offset is negative, the returned <see cref="TextPointer"/> precedes the current 
    /// <see cref="TextPointer"/>; otherwise, it follows.
    /// </param>
    /// <param name="direction">
    /// One of the <see cref="Documents.LogicalDirection"/> values that specifies the
    /// logical direction of the returned <see cref="TextPointer"/>.
    /// </param>
    /// <returns>
    /// A <see cref="TextPointer"/> to the position indicated by the specified offset 
    /// and in the direction specified by the direction parameter, or null if the 
    /// offset extends past the end of the content.
    /// </returns>
    public TextPointer GetPositionAtOffset(int offset, LogicalDirection direction)
    {
        int position = Offset + offset;
        if (position < 0 || position > GetContentLength())
        {
            return null;
        }

        return new TextPointer(VisualParent, position, direction);
    }

    private int GetContentLength() =>
        VisualParent switch
        {
            RichTextBox richTextBox => richTextBox.ContentEnd.Offset,
            _ => 0,
        };
}
