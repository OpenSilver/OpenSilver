
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

using OpenSilver.Internal;
using System.Windows.Automation.Peers;
using System.Windows.Documents;

namespace System.Windows.Controls
{
    public sealed partial class RichTextBlock : FrameworkElement
    {
        /// <summary>
        /// Identifies the <see cref="FontStretch"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty FontStretchProperty =
            TextElement.FontStretchProperty.AddOwner(typeof(RichTextBlock));

        /// <summary>
        /// Gets or sets the degree to which a font is condensed or expanded on the screen.
        /// </summary>
        /// <returns>
        /// One of the values that specifies the degree to which a font is condensed or expanded
        /// on the screen. The default is <see cref="FontStretches.Normal"/>.
        /// </returns>
        [OpenSilver.NotImplemented]
        public FontStretch FontStretch
        {
            get => (FontStretch)GetValue(FontStretchProperty);
            set => SetValueInternal(FontStretchProperty, value);
        }

        private static readonly DependencyPropertyKey HasOverflowContentPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(HasOverflowContent),
                typeof(bool),
                typeof(RichTextBlock),
                new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="HasOverflowContent"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty HasOverflowContentProperty = HasOverflowContentPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets or sets a value that indicates whether the <see cref="RichTextBlock"/>
        /// has overflow content.
        /// </summary>
        /// <returns>
        /// true if <see cref="RichTextBlock"/> has overflow content; false otherwise.
        /// </returns>
        [OpenSilver.NotImplemented]
        public bool HasOverflowContent
        {
            get => (bool)GetValue(HasOverflowContentProperty);
            private set => SetValueInternal(HasOverflowContentPropertyKey, value);
        }

        /// <summary>
        /// Identifies the <see cref="OverflowContentTarget"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty OverflowContentTargetProperty =
            DependencyProperty.Register(
                nameof(OverflowContentTarget),
                typeof(RichTextBlockOverflow),
                typeof(RichTextBlock),
                new PropertyMetadata((object)null));

        /// <summary>
        /// Gets or sets the <see cref="RichTextBlockOverflow"/> that will consume
        /// the overflow content of this <see cref="RichTextBlock"/>.
        /// </summary>
        [OpenSilver.NotImplemented]
        public RichTextBlockOverflow OverflowContentTarget
        {
            get => (RichTextBlockOverflow)GetValue(OverflowContentTargetProperty);
            set => SetValueInternal(OverflowContentTargetProperty, value);
        }

        private static readonly DependencyPropertyKey SelectedTextPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(SelectedText),
                typeof(string),
                typeof(RichTextBlock),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// Identifies the <see cref="SelectedText"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty SelectedTextProperty = SelectedTextPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the plain text of the current selection in <see cref="RichTextBlock"/>.
        /// </summary>
        /// <returns>
        /// the plain text of the current selection in <see cref="RichTextBlock"/>.
        /// </returns>
        [OpenSilver.NotImplemented]
        public string SelectedText
        {
            get => (string)GetValue(SelectedTextProperty);
            private set => SetValueInternal(SelectedTextPropertyKey, value);
        }

        /// <summary>
        /// Gets a <see cref="TextPointer"/> that indicates the end of content
        /// in the <see cref="RichTextBlock"/>.
        /// </summary>
        /// <returns>
        /// Returns <see cref="TextPointer"/>.
        /// </returns>
        [OpenSilver.NotImplemented]
        public TextPointer ContentEnd { get; }

        /// <summary>
        /// Gets a <see cref="TextPointer"/> that indicates the start of content
        /// in the <see cref="RichTextBlock"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="TextPointer"/> that indicates the start of content in
        /// the <see cref="RichTextBlock"/>.
        /// </returns>
        [OpenSilver.NotImplemented]
        public TextPointer ContentStart { get; }

        /// <summary>
        /// Gets a <see cref="TextPointer"/> that indicates the start of the selection
        /// in the <see cref="RichTextBlock"/> or a chain of linked containers.
        /// </summary>
        /// <returns>
        /// A <see cref="TextPointer"/> that indicates the start of the selection
        /// in the <see cref="RichTextBlock"/> or a chain of linked containers.
        /// </returns>
        [OpenSilver.NotImplemented]
        public TextPointer SelectionEnd { get; }

        /// <summary>
        /// Gets a <see cref="TextPointer"/> that indicates the start of the selection
        /// in a <see cref="RichTextBlock"/> or a chain of linked containers.
        /// </summary>
        /// <returns>
        /// A <see cref="TextPointer"/> that indicates the start of the selection
        /// in a <see cref="RichTextBlock"/> or a chain of linked containers.
        /// </returns>
        [OpenSilver.NotImplemented]
        public TextPointer SelectionStart { get; }

        /// <summary>
        /// Occurs when the text selection has changed.
        /// </summary>
        [OpenSilver.NotImplemented]
        public event RoutedEventHandler SelectionChanged;

        /// <summary>
        /// Returns a <see cref="TextPointer"/> that indicates the closest insertion
        /// position for the specified point.
        /// </summary>
        /// <param name="point">
        /// A point in the coordinate space of the <see cref="RichTextBlock"/>
        /// for which the closest insertion position is retrieved.
        /// </param>
        /// <returns>
        /// A <see cref="TextPointer"/> that indicates the closest insertion position
        /// for the specified point.
        /// </returns>
        [OpenSilver.NotImplemented]
        public TextPointer GetPositionFromPoint(Point point) => null;

        /// <summary>
        /// Selects the content between two positions indicated by textpointer objects.
        /// </summary>
        /// <param name="start">
        /// The text pointer which marks the start position end of the updated selection.
        /// </param>
        /// <param name="end">
        /// The text pointer which marks the end position of the other end of the updated selection.
        /// </param>
        [OpenSilver.NotImplemented]
        public void Select(TextPointer start, TextPointer end) { }

        /// <summary>
        /// Selects the entire contents in the <see cref="RichTextBlock"/>.
        /// </summary>
        [OpenSilver.NotImplemented]
        public void SelectAll() { }

        [OpenSilver.NotImplemented]
        protected override AutomationPeer OnCreateAutomationPeer() => base.OnCreateAutomationPeer();
    }
}
