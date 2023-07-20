
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
using System.Windows.Automation.Peers;
using System.Windows.Documents;
#else
using Windows.Foundation;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Automation.Peers;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    public sealed partial class RichTextBlock : FrameworkElement
    {
        /// <summary>
        /// Identifies the <see cref="CharacterSpacing"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty CharacterSpacingProperty =
            DependencyProperty.Register(
                nameof(CharacterSpacing),
                typeof(int),
                typeof(RichTextBlock),
                new PropertyMetadata(0));

        /// <summary>
        /// Gets or sets the distance between characters of text in the control measured
        /// in 1000ths of the font size.
        /// </summary>
        /// <returns>
        /// The distance between characters of text in the control measured in 1000ths of
        /// the font size. The default is 0.
        /// </returns>
        [OpenSilver.NotImplemented]
        public int CharacterSpacing
        {
            get => (int)GetValue(CharacterSpacingProperty);
            set => SetValue(CharacterSpacingProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="FontStretch"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty FontStretchProperty =
            DependencyProperty.Register(
                nameof(FontStretch),
                typeof(FontStretch),
                typeof(RichTextBlock),
                new PropertyMetadata(FontStretches.Normal));

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
            set => SetValue(FontStretchProperty, value);
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
            private set => SetValue(HasOverflowContentPropertyKey, value);
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
            set => SetValue(OverflowContentTargetProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="LineHeight"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty LineHeightProperty =
            DependencyProperty.Register(
                nameof(LineHeight),
                typeof(double),
                typeof(RichTextBlock),
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        ///  Gets or sets the height of each line of content.
        /// </summary>
        /// <returns>
        /// The height of each line in pixels. A value of 0 indicates that the line height
        /// is determined automatically from the current font characteristics. The default
        /// is 0.
        /// </returns>
        [OpenSilver.NotImplemented]
        public double LineHeight
        {
            get => (double)GetValue(LineHeightProperty);
            set => SetValue(LineHeightProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="LineStackingStrategy"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty LineStackingStrategyProperty =
            DependencyProperty.Register(
                nameof(LineStackingStrategy),
                typeof(LineStackingStrategy),
                typeof(RichTextBlock),
                new PropertyMetadata(LineStackingStrategy.MaxHeight));

        /// <summary>
        /// Gets or sets a value that indicates how a line box is determined for each line
        /// of text in the <see cref="RichTextBlock"/>.
        /// </summary>
        /// <returns>
        /// A value that indicates how a line box is determined for each line of text in
        /// the <see cref="RichTextBlock"/>. The default is <see cref="LineStackingStrategy.MaxHeight"/>.
        /// </returns>
        [OpenSilver.NotImplemented]
        public LineStackingStrategy LineStackingStrategy
        {
            get => (LineStackingStrategy)GetValue(LineStackingStrategyProperty);
            set => SetValue(LineStackingStrategyProperty, value);
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
            private set => SetValue(SelectedTextPropertyKey, value);
        }

        /// <summary>
        /// Gets a value that represents the offset in pixels from the top of the content
        /// to the baseline of the first paragraph. The baseline of the paragraph is the
        /// baseline of the first line in it.
        /// </summary>
        /// <returns>
        /// The computed baseline for the first paragraph, or 0 if the <see cref="RichTextBlock"/>
        /// is empty.
        /// </returns>
        [OpenSilver.NotImplemented]
        public double BaselineOffset { get; }

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
