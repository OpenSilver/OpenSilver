#if WORKINPROGRESS

#if MIGRATION
namespace System.Windows.Documents
#else
namespace Windows.UI.Xaml.Documents
#endif
{
    /// <summary>Provides access to a rich set of OpenType typography properties. This class cannot be inherited.</summary>
    public static class Typography
    {
        public static readonly DependencyProperty CapitalsProperty = new DependencyProperty();

        public static FontCapitals GetCapitals(DependencyObject element) => (FontCapitals)Typography.GetTypographyValue(element, Typography.CapitalsProperty);

        public static void SetCapitals(DependencyObject element, FontCapitals value) => Typography.SetTypographyValue(element, Typography.CapitalsProperty, (object)value);

        private static object GetTypographyValue(DependencyObject element, DependencyProperty property) => element != null ? element.GetValue(property) : throw new ArgumentNullException(nameof(element));

        private static void SetTypographyValue(DependencyObject element, DependencyProperty property, object value)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            element.SetValue(property, value);
        }
    }
}

#endif
