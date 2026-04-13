
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

using CSHTML5.Internal;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;

namespace System.Windows.Input
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
                new PropertyMetadata(null, OnFocusedElementChanged));

        /// <summary>
        /// Queries the Silverlight focus system to determine which object has focus.
        /// </summary>
        /// <returns>The object that currently has focus. Typically, this is a <see cref="Control" /> class.</returns>
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

        internal static UIElement GetFocusedElement(DependencyObject element, bool validate)
        {
            ArgumentNullException.ThrowIfNull(element);

            UIElement focusedElement = (UIElement)element.GetValue(FocusedElementProperty);

            if (validate && focusedElement is not null)
            {
                DependencyObject focusScope = element;

                if (GetWindowSource(focusScope) != GetWindowSource(focusedElement))
                {
                    SetFocusedElement(focusScope, null);
                    focusedElement = null;
                }
            }

            return focusedElement;
        }

        /// <summary>
        /// Set FocusedElement property for element.
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="newFocus"></param>
        internal static void SetFocusedElement(DependencyObject scope, UIElement newFocus)
        {
            scope.SetValueInternal(FocusedElementProperty, newFocus);
        }

        private static void OnFocusedElementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement newFocusedElement = (UIElement)e.NewValue;
            DependencyObject oldVisual = (DependencyObject)e.OldValue;
            DependencyObject newVisual = (DependencyObject)e.NewValue;

            oldVisual?.ClearValue(UIElement.IsFocusedPropertyKey);

            if (newVisual is not null)
            {
                // set IsFocused on the element.  The element may redirect Keyboard focus
                // in response to this, so detect whether this happens.
                DependencyObject oldFocus = Keyboard.FocusedElement as DependencyObject;
                newVisual.SetValueInternal(UIElement.IsFocusedPropertyKey, true);
                DependencyObject newFocus = Keyboard.FocusedElement as DependencyObject;

                // set the Keyboard focus to the new element, provided that
                //  a) the element didn't already set Keyboard focus
                //  b) Keyboard focus is not already on the new element
                //  c) the new element is within the same focus scope as the current
                //      holder (if any) of Keyboard focus
                if (oldFocus == newFocus && newVisual != newFocus &&
                    (newFocus is null || GetRoot(newVisual) == GetRoot(newFocus)))
                {
                    Keyboard.Focus(newFocusedElement);
                }
            }
        }

        internal static DependencyObject GetFocusScope(UIElement uie)
        {
            return uie?.ParentWindow ?? Window.Current;
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

        private static Window GetWindowSource(DependencyObject dependencyObject)
        {
            if (dependencyObject is UIElement uie && INTERNAL_VisualTreeManager.IsElementInVisualTree(uie))
            {
                return Window.GetWindow(uie);
            }

            return null;
        }

        private static DependencyObject GetRoot(DependencyObject element)
        {
            if (element is null)
            {
                return null;
            }

            DependencyObject parent = null;
            DependencyObject dependencyObject = element;

            while (dependencyObject is not null)
            {
                parent = dependencyObject;
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }

            return parent;
        }
    }
}