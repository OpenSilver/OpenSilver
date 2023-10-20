namespace System.Windows.Documents
{
    /// <summary>
    /// Provides access to a rich set of OpenType typography properties. 
    /// This class cannot be inherited.
    /// </summary>
    [OpenSilver.NotImplemented]
    public static class Typography
    {
        /// <summary>
        /// Identifies the Typography.Capitals dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty CapitalsProperty =
            DependencyProperty.RegisterAttached(
                "Capitals",
                typeof(FontCapitals),
                typeof(Typography),
                new PropertyMetadata(FontCapitals.Normal));

        /// <summary>
        /// Returns the value of the Typography.Capitals attached
        /// property for a specified dependency object.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static FontCapitals GetCapitals(DependencyObject element)
            => (FontCapitals)GetTypographyValue(element, Typography.CapitalsProperty);

        /// <summary>
        /// Sets the value of the Typography.Capitals attached property
        /// for a specified dependency object.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static void SetCapitals(DependencyObject element, FontCapitals value) 
            => Typography.SetTypographyValue(element, Typography.CapitalsProperty, (object)value);

        private static object GetTypographyValue(DependencyObject element, DependencyProperty property)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            return element.GetValue(property);
        }

        private static void SetTypographyValue(DependencyObject element, DependencyProperty property, object value)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            element.SetValue(property, value);
        }

        public static readonly DependencyProperty AnnotationAlternatesProperty = DependencyProperty.RegisterAttached(
       "AnnotationAlternates",
       typeof(int),
       typeof(Typography),
       new PropertyMetadata(0));

        public static readonly DependencyProperty EastAsianExpertFormsProperty = DependencyProperty.RegisterAttached(
            "EastAsianExpertForms",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(false));

        public static readonly DependencyProperty EastAsianLanguageProperty = DependencyProperty.RegisterAttached(
            "EastAsianLanguage",
            typeof(FontEastAsianLanguage),
            typeof(Typography),
            new PropertyMetadata(FontEastAsianLanguage.Normal));

        public static readonly DependencyProperty EastAsianWidthsProperty = DependencyProperty.RegisterAttached(
            "EastAsianWidths",
            typeof(FontEastAsianWidths),
            typeof(Typography),
            new PropertyMetadata(FontEastAsianWidths.Normal));

        public static readonly DependencyProperty StandardLigaturesProperty = DependencyProperty.RegisterAttached(
            "StandardLigatures",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(false));

        public static readonly DependencyProperty ContextualLigaturesProperty = DependencyProperty.RegisterAttached(
            "ContextualLigatures",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(false));

        public static readonly DependencyProperty DiscretionaryLigaturesProperty = DependencyProperty.RegisterAttached(
            "DiscretionaryLigatures",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(false));

        public static readonly DependencyProperty HistoricalLigaturesProperty = DependencyProperty.RegisterAttached(
            "HistoricalLigatures",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(false));

        public static readonly DependencyProperty StandardSwashesProperty = DependencyProperty.RegisterAttached(
            "StandardSwashes",
            typeof(int),
            typeof(Typography),
            new PropertyMetadata(0));

        public static readonly DependencyProperty ContextualSwashesProperty = DependencyProperty.RegisterAttached(
            "ContextualSwashes",
            typeof(int),
            typeof(Typography),
            new PropertyMetadata(0));

        public static readonly DependencyProperty ContextualAlternatesProperty = DependencyProperty.RegisterAttached(
            "ContextualAlternates",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(false));

        public static readonly DependencyProperty StylisticAlternatesProperty = DependencyProperty.RegisterAttached(
            "StylisticAlternates",
            typeof(int),
            typeof(Typography),
            new PropertyMetadata(0));

        public static readonly DependencyProperty StylisticSet1Property = DependencyProperty.RegisterAttached(
            "StylisticSet1",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(false));

        public static readonly DependencyProperty StylisticSet2Property = DependencyProperty.RegisterAttached(
            "StylisticSet2",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(false));

        public static readonly DependencyProperty StylisticSet3Property = DependencyProperty.RegisterAttached(
            "StylisticSet3",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(false));

        public static readonly DependencyProperty StylisticSet4Property = DependencyProperty.RegisterAttached(
            "StylisticSet4",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(false));

        public static readonly DependencyProperty StylisticSet5Property = DependencyProperty.RegisterAttached(
            "StylisticSet5",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(false));

        public static readonly DependencyProperty StylisticSet6Property = DependencyProperty.RegisterAttached(
            "StylisticSet6",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(false));

        public static readonly DependencyProperty StylisticSet7Property = DependencyProperty.RegisterAttached(
            "StylisticSet7",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(false));

        public static readonly DependencyProperty StylisticSet8Property = DependencyProperty.RegisterAttached(
            "StylisticSet8",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(false));

        public static readonly DependencyProperty StylisticSet9Property = DependencyProperty.RegisterAttached(
            "StylisticSet9",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(false));

        public static readonly DependencyProperty StylisticSet10Property = DependencyProperty.RegisterAttached(
            "StylisticSet10",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(false));

        public static readonly DependencyProperty StylisticSet11Property = DependencyProperty.RegisterAttached(
            "StylisticSet11",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(false));

        public static readonly DependencyProperty StylisticSet12Property = DependencyProperty.RegisterAttached(
            "StylisticSet12",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(false));

        public static readonly DependencyProperty StylisticSet13Property = DependencyProperty.RegisterAttached(
            "StylisticSet13",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(false));

        public static readonly DependencyProperty StylisticSet14Property = DependencyProperty.RegisterAttached(
            "StylisticSet14",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(false));

        public static readonly DependencyProperty StylisticSet15Property = DependencyProperty.RegisterAttached(
            "StylisticSet15",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(false));

        public static readonly DependencyProperty StylisticSet16Property = DependencyProperty.RegisterAttached(
            "StylisticSet16",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(false));

        public static readonly DependencyProperty StylisticSet17Property = DependencyProperty.RegisterAttached(
            "StylisticSet17",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(false));

        public static readonly DependencyProperty StylisticSet18Property = DependencyProperty.RegisterAttached(
            "StylisticSet18",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(false));

        public static readonly DependencyProperty StylisticSet19Property = DependencyProperty.RegisterAttached(
            "StylisticSet19",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(false));

        public static readonly DependencyProperty StylisticSet20Property = DependencyProperty.RegisterAttached(
            "StylisticSet20",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(false));

        public static readonly DependencyProperty CapitalSpacingProperty = DependencyProperty.RegisterAttached(
            "CapitalSpacing",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(false));

        public static readonly DependencyProperty KerningProperty = DependencyProperty.RegisterAttached(
            "Kerning",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(false));

        public static readonly DependencyProperty CaseSensitiveFormsProperty = DependencyProperty.RegisterAttached(
            "CaseSensitiveForms",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(false));

        public static readonly DependencyProperty HistoricalFormsProperty = DependencyProperty.RegisterAttached(
            "HistoricalForms",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(false));

        public static readonly DependencyProperty FractionProperty = DependencyProperty.RegisterAttached(
            "Fraction",
            typeof(FontFraction),
            typeof(Typography),
            new PropertyMetadata(FontFraction.Normal));

        public static readonly DependencyProperty NumeralStyleProperty = DependencyProperty.RegisterAttached(
            "NumeralStyle",
            typeof(FontNumeralStyle),
            typeof(Typography),
            new PropertyMetadata(FontNumeralStyle.Normal));

        public static readonly DependencyProperty NumeralAlignmentProperty = DependencyProperty.RegisterAttached(
            "NumeralAlignment",
            typeof(FontNumeralAlignment),
            typeof(Typography),
            new PropertyMetadata(FontNumeralAlignment.Normal));

        public static readonly DependencyProperty SlashedZeroProperty = DependencyProperty.RegisterAttached(
            "SlashedZero",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(false));

        public static readonly DependencyProperty MathematicalGreekProperty = DependencyProperty.RegisterAttached(
            "MathematicalGreek",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(false));

        public static readonly DependencyProperty VariantsProperty = DependencyProperty.RegisterAttached(
            "Variants",
            typeof(FontVariants),
            typeof(Typography),
            new PropertyMetadata(FontVariants.Normal));

        /// <summary>
        /// Returns the value of the Typography.Capitals attached
        /// property for a specified dependency object.
        /// </summary>
        public static int GetAnnotationAlternates(DependencyObject element)
        {
            return (int)GetTypographyValue(element, AnnotationAlternatesProperty);
        }

        public static void SetAnnotationAlternates(DependencyObject element, int value)
        {
            SetTypographyValue(element, AnnotationAlternatesProperty, value);
        }

        public static bool GetEastAsianExpertForms(DependencyObject element)
        {
            return (bool)GetTypographyValue(element, EastAsianExpertFormsProperty);
        }

        public static void SetEastAsianExpertForms(DependencyObject element, bool value)
        {
            SetTypographyValue(element, EastAsianExpertFormsProperty, value);
        }

        public static FontEastAsianLanguage GetEastAsianLanguage(DependencyObject element)
        {
            return (FontEastAsianLanguage)GetTypographyValue(element, EastAsianLanguageProperty);
        }

        public static void SetEastAsianLanguage(DependencyObject element, FontEastAsianLanguage value)
        {
            SetTypographyValue(element, EastAsianLanguageProperty, value);
        }

        public static FontEastAsianWidths GetEastAsianWidths(DependencyObject element)
        {
            return (FontEastAsianWidths)GetTypographyValue(element, EastAsianWidthsProperty);
        }

        public static void SetEastAsianWidths(DependencyObject element, FontEastAsianWidths value)
        {
            SetTypographyValue(element, EastAsianWidthsProperty, value);
        }

        public static bool GetStandardLigatures(DependencyObject element)
        {
            return (bool)GetTypographyValue(element, StandardLigaturesProperty);
        }

        public static void SetStandardLigatures(DependencyObject element, bool value)
        {
            SetTypographyValue(element, StandardLigaturesProperty, value);
        }

        public static bool GetContextualLigatures(DependencyObject element)
        {
            return (bool)GetTypographyValue(element, ContextualLigaturesProperty);
        }

        public static void SetContextualLigatures(DependencyObject element, bool value)
        {
            SetTypographyValue(element, ContextualLigaturesProperty, value);
        }

        public static bool GetDiscretionaryLigatures(DependencyObject element)
        {
            return (bool)GetTypographyValue(element, DiscretionaryLigaturesProperty);
        }

        public static void SetDiscretionaryLigatures(DependencyObject element, bool value)
        {
            SetTypographyValue(element, DiscretionaryLigaturesProperty, value);
        }

        public static bool GetHistoricalLigatures(DependencyObject element)
        {
            return (bool)GetTypographyValue(element, HistoricalLigaturesProperty);
        }

        public static void SetHistoricalLigatures(DependencyObject element, bool value)
        {
            SetTypographyValue(element, HistoricalLigaturesProperty, value);
        }

        public static int GetStandardSwashes(DependencyObject element)
        {
            return (int)GetTypographyValue(element, StandardSwashesProperty);
        }

        public static void SetStandardSwashes(DependencyObject element, int value)
        {
            SetTypographyValue(element, StandardSwashesProperty, value);
        }

        public static int GetContextualSwashes(DependencyObject element)
        {
            return (int)GetTypographyValue(element, ContextualSwashesProperty);
        }

        public static void SetContextualSwashes(DependencyObject element, int value)
        {
            SetTypographyValue(element, ContextualSwashesProperty, value);
        }

        public static bool GetContextualAlternates(DependencyObject element)
        {
            return (bool)GetTypographyValue(element, ContextualAlternatesProperty);
        }

        public static void SetContextualAlternates(DependencyObject element, bool value)
        {
            SetTypographyValue(element, ContextualAlternatesProperty, value);
        }

        public static int GetStylisticAlternates(DependencyObject element)
        {
            return (int)GetTypographyValue(element, StylisticAlternatesProperty);
        }

        public static void SetStylisticAlternates(DependencyObject element, int value)
        {
            SetTypographyValue(element, StylisticAlternatesProperty, value);
        }

        public static bool GetStylisticSet1(DependencyObject element)
        {
            return (bool)GetTypographyValue(element, StylisticSet1Property);
        }

        public static void SetStylisticSet1(DependencyObject element, bool value)
        {
            SetTypographyValue(element, StylisticSet1Property, value);
        }

        public static bool GetStylisticSet2(DependencyObject element)
        {
            return (bool)GetTypographyValue(element, StylisticSet2Property);
        }

        public static void SetStylisticSet2(DependencyObject element, bool value)
        {
            SetTypographyValue(element, StylisticSet2Property, value);
        }

        public static bool GetStylisticSet3(DependencyObject element)
        {
            return (bool)GetTypographyValue(element, StylisticSet1Property);
        }

        public static void SetStylisticSet3(DependencyObject element, bool value)
        {
            SetTypographyValue(element, StylisticSet1Property, value);
        }

        public static bool GetStylisticSet4(DependencyObject element)
        {
            return (bool)GetTypographyValue(element, StylisticSet4Property);
        }

        public static void SetStylisticSet4(DependencyObject element, bool value)
        {
            SetTypographyValue(element, StylisticSet4Property, value);
        }

        public static bool GetStylisticSet5(DependencyObject element)
        {
            return (bool)GetTypographyValue(element, StylisticSet5Property);
        }

        public static void SetStylisticSet5(DependencyObject element, bool value)
        {
            SetTypographyValue(element, StylisticSet5Property, value);
        }

        public static bool GetStylisticSet6(DependencyObject element)
        {
            return (bool)GetTypographyValue(element, StylisticSet6Property);
        }

        public static void SetStylisticSet6(DependencyObject element, bool value)
        {
            SetTypographyValue(element, StylisticSet6Property, value);
        }

        public static bool GetStylisticSet7(DependencyObject element)
        {
            return (bool)GetTypographyValue(element, StylisticSet7Property);
        }

        public static void SetStylisticSet7(DependencyObject element, bool value)
        {
            SetTypographyValue(element, StylisticSet7Property, value);
        }

        public static bool GetStylisticSet8(DependencyObject element)
        {
            return (bool)GetTypographyValue(element, StylisticSet8Property);
        }

        public static void SetStylisticSet8(DependencyObject element, bool value)
        {
            SetTypographyValue(element, StylisticSet8Property, value);
        }

        public static bool GetStylisticSet9(DependencyObject element)
        {
            return (bool)GetTypographyValue(element, StylisticSet9Property);
        }

        public static void SetStylisticSet9(DependencyObject element, bool value)
        {
            SetTypographyValue(element, StylisticSet9Property, value);
        }

        public static bool GetStylisticSet10(DependencyObject element)
        {
            return (bool)GetTypographyValue(element, StylisticSet10Property);
        }

        public static void SetStylisticSet10(DependencyObject element, bool value)
        {
            SetTypographyValue(element, StylisticSet10Property, value);
        }

        public static bool GetStylisticSet11(DependencyObject element)
        {
            return (bool)GetTypographyValue(element, StylisticSet11Property);
        }

        public static void SetStylisticSet11(DependencyObject element, bool value)
        {
            SetTypographyValue(element, StylisticSet11Property, value);
        }

        public static bool GetStylisticSet12(DependencyObject element)
        {
            return (bool)GetTypographyValue(element, StylisticSet12Property);
        }

        public static void SetStylisticSet12(DependencyObject element, bool value)
        {
            SetTypographyValue(element, StylisticSet12Property, value);
        }

        public static bool GetStylisticSet13(DependencyObject element)
        {
            return (bool)GetTypographyValue(element, StylisticSet13Property);
        }

        public static void SetStylisticSet13(DependencyObject element, bool value)
        {
            SetTypographyValue(element, StylisticSet13Property, value);
        }

        public static bool GetStylisticSet14(DependencyObject element)
        {
            return (bool)GetTypographyValue(element, StylisticSet14Property);
        }

        public static void SetStylisticSet14(DependencyObject element, bool value)
        {
            SetTypographyValue(element, StylisticSet14Property, value);
        }

        public static bool GetStylisticSet15(DependencyObject element)
        {
            return (bool)GetTypographyValue(element, StylisticSet15Property);
        }

        public static void SetStylisticSet15(DependencyObject element, bool value)
        {
            SetTypographyValue(element, StylisticSet15Property, value);
        }

        public static bool GetStylisticSet16(DependencyObject element)
        {
            return (bool)GetTypographyValue(element, StylisticSet16Property);
        }

        public static void SetStylisticSet16(DependencyObject element, bool value)
        {
            SetTypographyValue(element, StylisticSet16Property, value);
        }

        public static bool GetStylisticSet17(DependencyObject element)
        {
            return (bool)GetTypographyValue(element, StylisticSet17Property);
        }

        public static void SetStylisticSet17(DependencyObject element, bool value)
        {
            SetTypographyValue(element, StylisticSet17Property, value);
        }

        public static bool GetStylisticSet18(DependencyObject element)
        {
            return (bool)GetTypographyValue(element, StylisticSet18Property);
        }

        public static void SetStylisticSet18(DependencyObject element, bool value)
        {
            SetTypographyValue(element, StylisticSet18Property, value);
        }

        public static bool GetStylisticSet19(DependencyObject element)
        {
            return (bool)GetTypographyValue(element, StylisticSet19Property);
        }

        public static void SetStylisticSet19(DependencyObject element, bool value)
        {
            SetTypographyValue(element, StylisticSet19Property, value);
        }

        public static bool GetStylisticSet20(DependencyObject element)
        {
            return (bool)GetTypographyValue(element, StylisticSet20Property);
        }

        public static void SetStylisticSet20(DependencyObject element, bool value)
        {
            SetTypographyValue(element, StylisticSet20Property, value);
        }

        public static bool GetCapitalSpacing(DependencyObject element)
        {
            return (bool)GetTypographyValue(element, CapitalSpacingProperty);
        }

        public static void SetCapitalSpacing(DependencyObject element, bool value)
        {
            SetTypographyValue(element, CapitalSpacingProperty, value);
        }

        public static bool GetKerning(DependencyObject element)
        {
            return (bool)GetTypographyValue(element, KerningProperty);
        }

        public static void SetKerning(DependencyObject element, bool value)
        {
            SetTypographyValue(element, KerningProperty, value);
        }

        public static bool GetCaseSensitiveForms(DependencyObject element)
        {
            return (bool)GetTypographyValue(element, CaseSensitiveFormsProperty);
        }

        public static void SetCaseSensitiveForms(DependencyObject element, bool value)
        {
            SetTypographyValue(element, CaseSensitiveFormsProperty, value);
        }

        public static bool GetHistoricalForms(DependencyObject element)
        {
            return (bool)GetTypographyValue(element, HistoricalFormsProperty);
        }

        public static void SetHistoricalForms(DependencyObject element, bool value)
        {
            SetTypographyValue(element, HistoricalFormsProperty, value);
        }

        public static FontFraction GetFraction(DependencyObject element)
        {
            return (FontFraction)GetTypographyValue(element, FractionProperty);
        }

        public static void SetFraction(DependencyObject element, FontFraction value)
        {
            SetTypographyValue(element, FractionProperty, value);
        }

        public static FontNumeralStyle GetNumeralStyle(DependencyObject element)
        {
            return (FontNumeralStyle)GetTypographyValue(element, NumeralStyleProperty);
        }

        public static void SetNumeralStyle(DependencyObject element, FontNumeralStyle value)
        {
            SetTypographyValue(element, NumeralStyleProperty, value);
        }

        public static FontNumeralAlignment GetNumeralAlignment(DependencyObject element)
        {
            return (FontNumeralAlignment)GetTypographyValue(element, NumeralAlignmentProperty);
        }

        public static void SetNumeralAlignment(DependencyObject element, FontNumeralAlignment value)
        {
            SetTypographyValue(element, NumeralAlignmentProperty, value);
        }

        public static bool GetSlashedZero(DependencyObject element)
        {
            return (bool)GetTypographyValue(element, SlashedZeroProperty);
        }

        public static void SetSlashedZero(DependencyObject element, bool value)
        {
            SetTypographyValue(element, SlashedZeroProperty, value);
        }

        public static bool GetMathematicalGreek(DependencyObject element)
        {
            return (bool)GetTypographyValue(element, MathematicalGreekProperty);
        }

        public static void SetMathematicalGreek(DependencyObject element, bool value)
        {
            SetTypographyValue(element, MathematicalGreekProperty, value);
        }

        public static void SetVariants(DependencyObject element, FontVariants value)
        {
            SetTypographyValue(element, VariantsProperty, value);
        }

        public static FontVariants GetVariants(DependencyObject element)
        {
            return (FontVariants)GetTypographyValue(element, VariantsProperty);
        }
    }
}
