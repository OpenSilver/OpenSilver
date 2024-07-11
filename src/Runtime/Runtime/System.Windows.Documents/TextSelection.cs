
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
using OpenSilver.Internal.Controls;

namespace System.Windows.Documents
{
    /// <summary>
    /// Encapsulates the selection state for the <see cref="RichTextBox"/> control.
    /// </summary>
    public sealed class TextSelection
    {
        private readonly RichTextBox _richTextBox;

        internal TextSelection(RichTextBox rtb)
        {
            Debug.Assert(rtb is not null);
            _richTextBox = rtb;
            Update(0, 0);
        }

        /// <summary>
        /// Gets a <see cref="TextPointer"/> that represents the end of the current selection.
        /// </summary>
        /// <returns>
        /// A <see cref="TextPointer"/> that represents the end of the current selection.
        /// </returns>
        public TextPointer End { get; private set; }

        /// <summary>
        /// Gets a <see cref="TextPointer"/> that represents the beginning of the current selection.
        /// </summary>
        /// <returns>
        /// A <see cref="TextPointer"/> that represents the beginning of the current selection.
        /// </returns>
        public TextPointer Start { get; private set; }

        private int Length => End.Offset - Start.Offset;

        /// <summary>
        /// Gets or sets the plain text contents of the current selection.
        /// </summary>
        /// <returns>
        /// A string that contains the plain text contents of the current selection.
        /// </returns>
        public string Text
        {
            get => _richTextBox.View?.GetSelectedText() ?? string.Empty;
            set => _richTextBox.View?.SetSelectedText(value);
        }

        /// <summary>
        /// Gets or sets the XAML representation of the current selection.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> that is a XAML representation of the current selection. This
        /// XAML representation is the same XAML that is applied to the clipboard for a copy
        /// operation.
        /// </returns>
        public string Xaml
        {
            get => _richTextBox.View?.GetXaml(Start.Offset, Length) ?? string.Empty;
            set
            {
                //TODO: implement
            }
        }

        /// <summary>
        /// Applies the specified formatting property and value to the current selection.
        /// </summary>
        /// <param name="formattingProperty">
        /// A formatting property to apply.
        /// </param>
        /// <param name="value">
        /// The value for the formatting property.
        /// </param>
        public void ApplyPropertyValue(DependencyProperty formattingProperty, object value)
            => _richTextBox.View?.Format(formattingProperty, value);

        /// <summary>
        /// Gets the value of the specified formatting property on the current selection.
        /// </summary>
        /// <param name="formattingProperty">
        /// A formatting property to get the value of on the current selection.
        /// </param>
        /// <returns>
        /// An object that indicates the value of the specified formatting property on the
        /// current selection.
        /// </returns>
        public object GetPropertyValue(DependencyProperty formattingProperty)
        {
            if (_richTextBox.View is RichTextBoxView view)
            {
                return view.GetFormat(formattingProperty);
            }

            return GetRichTextBoxFormattingProperty(formattingProperty);
        }

        private object GetRichTextBoxFormattingProperty(DependencyProperty formattingProperty)
        {
            if (formattingProperty == TextElement.FontFamilyProperty ||
                formattingProperty == TextElement.FontWeightProperty ||
                formattingProperty == TextElement.FontStyleProperty ||
                formattingProperty == TextElement.FontSizeProperty ||
                formattingProperty == TextElement.ForegroundProperty ||
                formattingProperty == TextElement.CharacterSpacingProperty ||
                formattingProperty == TextElement.FontStretchProperty ||
                formattingProperty == Inline.TextDecorationsProperty ||
                formattingProperty == Block.TextAlignmentProperty ||
                formattingProperty == Block.LineHeightProperty)
            {
                return _richTextBox.GetValue(formattingProperty);
            }
            return DependencyProperty.UnsetValue;
        }

        /// <summary>
        /// Inserts or replaces the content at the current selection as a <see cref="TextElement"/>.
        /// </summary>
        /// <param name="element">
        /// The <see cref="TextElement"/> to be inserted.
        /// </param>
        public void Insert(TextElement element)
            => _richTextBox.View?.UpdateContentsFromTextElement(element, Start.Offset, Length);

        /// <summary>
        /// Updates the current selection, taking two <see cref="TextPointer"/>
        /// positions to indicate the updated selection.
        /// </summary>
        /// <param name="anchorPosition">
        /// A fixed anchor position that marks one end of the updated selection.
        /// </param>
        /// <param name="movingPosition">
        /// A movable position that marks the other end of the updated selection.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// anchorPosition or movingPosition is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Position specifies a position from a different <see cref="RichTextBox"/>
        /// associated with the current position.
        /// </exception>
        public void Select(TextPointer anchorPosition, TextPointer movingPosition)
        {
            if (anchorPosition is null)
            {
                throw new ArgumentNullException(nameof(anchorPosition));
            }
            if (movingPosition is null)
            {
                throw new ArgumentNullException(nameof(movingPosition));
            }
            if (anchorPosition.VisualParent != _richTextBox || movingPosition.VisualParent != _richTextBox)
            {
                throw new ArgumentException("TextPointer is not in the TextTree associated with this object.");
            }

            if (_richTextBox.View is RichTextBoxView view)
            {
                int start = Math.Min(anchorPosition.Offset, movingPosition.Offset);
                int length = Math.Abs(anchorPosition.Offset - movingPosition.Offset);

                view.Select(start, length);
            }
        }

        internal void Update(int start, int length)
        {
            Start = new TextPointer(_richTextBox, start, LogicalDirection.Backward);
            End = new TextPointer(_richTextBox, start + length, LogicalDirection.Forward);
        }
    }
}
