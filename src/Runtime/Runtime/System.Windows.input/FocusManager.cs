
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
using System.Diagnostics;

#if MIGRATION
using System.Windows.Media;
#else
using Windows.UI.Xaml.Media;
#endif

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

        internal static bool HasFocus(UIElement uie, bool useLogicalTree = false)
        {
            Debug.Assert(uie != null);

            UIElement focused = GetFocusedElement() as UIElement;
            while (focused != null)
            {
                if (focused == uie)
                {
                    return true;
                }

                UIElement parent = VisualTreeHelper.GetParent(focused) as UIElement;
                if (parent == null && useLogicalTree)
                {
                    parent = (focused as FrameworkElement)?.Parent as UIElement;
                }

                focused = parent;
            }

            return false;
        }
    }
}