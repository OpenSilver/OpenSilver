using System;

#if MIGRATION
namespace System.Windows.Input
#else
namespace Windows.UI.Xaml.Input
#endif
{
    /// <summary>
    /// Provides utility methods related to element focus, without the need to handle focus-related events.
    /// </summary>
	public static class FocusManager
	{
        /// <summary>
        /// The DependencyProperty for the FocusedElement property.
        /// </summary>
        private static readonly DependencyProperty FocusedElementProperty =
            DependencyProperty.RegisterAttached(
                "FocusedElement",
                typeof(UIElement),
                typeof(FocusManager),
                new PropertyMetadata((object)null));

        /// <summary>
        /// Queries the Silverlight focus system to determine which object has focus.
        /// </summary>
        /// <returns>The object that currently has focus. Typically, this is a <see cref="Controls.Control" /> class.</returns>
		public static object GetFocusedElement()
        {
            return Window.Current?.GetValue(FocusedElementProperty);
        }

        /// <summary>
        /// Gets the element with focus within the specified focus scope.
        /// </summary>
        /// <returns>The element in the specified focus scope that has current focus.</returns>
        /// <param name="element">Declares the scope.</param>
        public static object GetFocusedElement(DependencyObject element)
        {
            return element is Window ? element.GetValue(FocusedElementProperty) : null;
        }

        /// <summary>
        /// Set FocusedElement property for element.
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="focusedElement"></param>
        internal static void SetFocusedElement(DependencyObject scope, UIElement focusedElement)
        {
            scope.SetValue(FocusedElementProperty, focusedElement);
        }
    }
}