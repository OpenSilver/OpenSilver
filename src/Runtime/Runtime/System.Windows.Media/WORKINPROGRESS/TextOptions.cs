namespace System.Windows.Media
{
    [OpenSilver.NotImplemented]
	public static class TextOptions
    {
        /// <summary> Text formatting mode Property </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty TextFormattingModeProperty = 
            DependencyProperty.RegisterAttached("TextFormattingMode", 
                                                typeof(TextFormattingMode), 
                                                typeof(TextOptions), 
                                                null);

        /// <summary> Text hinting property </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty TextHintingModeProperty =
            DependencyProperty.RegisterAttached("TextHintingMode",
                                                typeof(TextHintingMode),
                                                typeof(TextOptions),
                                                null);

        /// <summary> Text rendering Property </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty TextRenderingModeProperty =
            DependencyProperty.RegisterAttached("TextRenderingMode",
                                                typeof(TextRenderingMode),
                                                typeof(TextOptions),
                                                null);

        /// <summary>
        /// Gets the System.Windows.Media.TextFormattingMode for the specified <see cref="FrameworkElement"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// The element is null.
        /// </exception>
        [OpenSilver.NotImplemented]
        public static TextFormattingMode GetTextFormattingMode(FrameworkElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return (TextFormattingMode)element.GetValue(TextFormattingModeProperty);
        }

        /// <summary>
        /// Gets the System.Windows.Media.TextHintingMode for this <see cref="FrameworkElement"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// The element is null.
        /// </exception>
        [OpenSilver.NotImplemented]
        public static TextHintingMode GetTextHintingMode(FrameworkElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return (TextHintingMode)element.GetValue(TextHintingModeProperty);
        }

        /// <summary>
        /// Gets the System.Windows.Media.TextRenderingMode for the specified <see cref="FrameworkElement"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// The element is null.
        /// </exception>
        [OpenSilver.NotImplemented]
        public static TextRenderingMode GetTextRenderingMode(FrameworkElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return (TextRenderingMode)element.GetValue(TextRenderingModeProperty);
        }

        /// <summary>
        /// Sets the <see cref="TextFormattingMode"/> for the specified <see cref="FrameworkElement"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// The element is null.
        /// </exception>
        [OpenSilver.NotImplemented]
        public static void SetTextFormattingMode(FrameworkElement element, TextFormattingMode value)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            element.SetValueInternal(TextFormattingModeProperty, value);
        }

        /// <summary>
        /// Sets the <see cref="TextHintingMode"/> for this <see cref="FrameworkElement"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// The element is null.
        /// </exception>
        [OpenSilver.NotImplemented]
        public static void SetTextHintingMode(FrameworkElement element, TextHintingMode value)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            element.SetValueInternal(TextHintingModeProperty, value);
        }

        /// <summary>
        /// Sets the <see cref="TextRenderingMode"/> for this <see cref="FrameworkElement"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// The element is null.
        /// </exception>
        [OpenSilver.NotImplemented]
        public static void SetTextRenderingMode(FrameworkElement element, TextRenderingMode value)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            element.SetValueInternal(TextRenderingModeProperty, value);
        }
    }
}
