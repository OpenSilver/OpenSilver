#if WORKINPROGRESS

using System;

#if MIGRATION
namespace System.Windows.Documents
#else
namespace Windows.UI.Xaml.Documents
#endif
{
    /// <summary>Provides access to a rich set of OpenType typography properties. This class cannot be inherited.</summary>
    [OpenSilver.NotImplemented]
    public static class Typography
    {
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty CapitalsProperty = new DependencyProperty();

        [OpenSilver.NotImplemented]
        public static FontCapitals GetCapitals(DependencyObject element) => (FontCapitals)Typography.GetTypographyValue(element, Typography.CapitalsProperty);

        [OpenSilver.NotImplemented]
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
