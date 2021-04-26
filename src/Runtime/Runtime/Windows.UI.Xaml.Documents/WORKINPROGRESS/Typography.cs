#if WORKINPROGRESS

using System;

#if MIGRATION
namespace System.Windows.Documents
#else
namespace Windows.UI.Xaml.Documents
#endif
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
    }
}

#endif
