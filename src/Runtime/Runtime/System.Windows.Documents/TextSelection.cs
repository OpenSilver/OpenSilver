
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
using System.Windows.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

#if MIGRATION
namespace System.Windows.Documents
#else
namespace Windows.UI.Xaml.Documents
#endif
{
    /// <summary>
    /// Encapsulates the selection state for the <see cref="RichTextBox"/> control.
    /// </summary>
    public sealed class TextSelection
    {
        private readonly RichTextBox _richTextBox;
        private int _start;
        private int _length;

        internal TextSelection(RichTextBox rtb)
        {
            _richTextBox = rtb ?? throw new ArgumentNullException(nameof(rtb));
        }

        /// <summary>
        /// Gets a <see cref="TextPointer"/> that represents the end of the current selection.
        /// </summary>
        /// <returns>
        /// A <see cref="TextPointer"/> that represents the end of the current selection.
        /// </returns>
        [OpenSilver.NotImplemented]
        public TextPointer End { get; }

        /// <summary>
        /// Gets a <see cref="TextPointer"/> that represents the beginning of the current selection.
        /// </summary>
        /// <returns>
        /// A <see cref="TextPointer"/> that represents the beginning of the current selection.
        /// </returns>
        [OpenSilver.NotImplemented]
        public TextPointer Start { get; }

        /// <summary>
        /// Gets or sets the plain text contents of the current selection.
        /// </summary>
        /// <returns>
        /// A string that contains the plain text contents of the current selection.
        /// </returns>
        public string Text
        {
            get => _richTextBox.View?.GetText(_start, _length) ?? string.Empty;
            set => _richTextBox.View?.SetText(_start, _length, value);
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
            get => _richTextBox.View?.GetContents(_start, _length);
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
            => _richTextBox.View?.SetPropertyValue(formattingProperty, value, _start, _length);

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
            => _richTextBox.View?.GetPropertyValue(formattingProperty, _start, _length);
        
        /// <summary>
        /// Inserts or replaces the content at the current selection as a <see cref="TextElement"/>.
        /// </summary>
        /// <param name="element">
        /// The <see cref="TextElement"/> to be inserted.
        /// </param>
        public void Insert(TextElement element)
        {
            //TODO: support other TextElements
            if (element is Run run)
            {
                _richTextBox.View?.SetText(_start, _length, run.Text);
            }
        }
        
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
        /// <exception cref="ArgumentException">
        /// Position specifies a position from a different <see cref="RichTextBox"/>
        /// associated with the current position.
        /// </exception>
        [OpenSilver.NotImplemented]
        public void Select(TextPointer anchorPosition, TextPointer movingPosition)
        {
        }

        internal void UpdateSelection(int start, int length)
        {
            _start = start;
            _length = length;
        }
    }
}
