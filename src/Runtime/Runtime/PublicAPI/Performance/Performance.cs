using System;
using System.Collections.Generic;
#if MIGRATION
using System.Windows;
using System.Windows.Controls.Primitives;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
#endif

namespace CSHTML5
{
    public static class Performance
    {
        /// Gets a value indicating whether a control should be rendered before its
        /// children and whether the children should be rendered one at a time,
        /// rather than loading everything at once.
        public static bool GetEnableProgressiveLoading(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnableProgressiveLoadingProperty);
        }

        /// <summary>
        /// Sets a value indicating whether a control should be rendered before its
        /// children and whether the children should be rendered one at a time,
        /// rather than loading everything at once. The default value is false.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetEnableProgressiveLoading(DependencyObject obj, bool value)
        {
            obj.SetValue(EnableProgressiveLoadingProperty, value);
        }

        /// <summary>
        /// The backing store for the "EnableProgressiveLoading" attached property.
        /// </summary>
        public static readonly DependencyProperty EnableProgressiveLoadingProperty =
            DependencyProperty.RegisterAttached(
                name: "EnableProgressiveLoading",
                propertyType: typeof(bool),
                ownerType: typeof(Performance),
                typeMetadata: new PropertyMetadata(
                    defaultValue: false,
                    propertyChangedCallback: EnableProgressiveLoading_Changed));

        private static void EnableProgressiveLoading_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement && e.NewValue is bool)
            {
                ((UIElement)d).INTERNAL_EnableProgressiveLoading = (bool)e.NewValue;
            }
        }
    }
}
