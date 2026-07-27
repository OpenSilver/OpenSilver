
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
using OpenSilver.Internal;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;

namespace System.Windows.Input;

/// <summary>
/// Provides utility methods related to element focus, without the need to handle focus-related events.
/// </summary>
public static class FocusManager
{
    /// <summary>
    /// Identifies the <b>FocusManager.GotFocus</b> attached event.
    /// </summary>
    public static readonly RoutedEvent GotFocusEvent =
        EventManager.RegisterCoreEvent(
            "GotFocus",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(FocusManager));

    /// <summary>
    /// Adds a handler for the <b>FocusManager.GotFocus</b> attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler to be added.
    /// </param>
    public static void AddGotFocusHandler(DependencyObject element, RoutedEventHandler handler)
        => UIElement.AddHandler(element, GotFocusEvent, handler);

    /// <summary>
    /// Removes a handler for the <b>FocusManager.GotFocus</b> attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler to be removed.
    /// </param>
    public static void RemoveGotFocusHandler(DependencyObject element, RoutedEventHandler handler)
        => UIElement.RemoveHandler(element, GotFocusEvent, handler);

    /// <summary>
    /// Identifies the <b>FocusManager.LostFocus</b> attached event.
    /// </summary>
    public static readonly RoutedEvent LostFocusEvent =
        EventManager.RegisterCoreEvent(
            "LostFocus",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(FocusManager));

    /// <summary>
    /// Adds a handler for the <b>FocusManager.LostFocus</b> attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler to be added.
    /// </param>
    public static void AddLostFocusHandler(DependencyObject element, RoutedEventHandler handler)
        => UIElement.AddHandler(element, LostFocusEvent, handler);

    /// <summary>
    /// Removes a handler for the <b>FocusManager.LostFocus</b> attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler to be removed.
    /// </param>
    public static void RemoveLostFocusHandler(DependencyObject element, RoutedEventHandler handler)
        => UIElement.RemoveHandler(element, LostFocusEvent, handler);

    /// <summary>
    /// The DependencyProperty for the FocusedElement property.
    /// </summary>
    private static readonly DependencyProperty FocusedElementProperty =
        DependencyProperty.RegisterAttached(
            "FocusedElement",
            typeof(IInputElement),
            typeof(FocusManager),
            new PropertyMetadata(null, OnFocusedElementChanged));

    /// <summary>
    /// Queries the Silverlight focus system to determine which object has focus.
    /// </summary>
    /// <returns>
    /// The object that currently has focus. Typically, this is a <see cref="Control" /> class.
    /// </returns>
    public static object GetFocusedElement()
    {
        if (Window.Current is Window window && GetIsFocusScope(window))
        {
            return GetFocusedElement(window);
        }
        return null;
    }

    /// <summary>
    /// Gets the element with logical focus within the specified focus scope.
    /// </summary>
    /// <param name="element">
    /// An element that is a focus scope.
    /// </param>
    /// <returns>
    /// The element in the specified focus scope with logical focus.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    public static IInputElement GetFocusedElement(DependencyObject element) => GetFocusedElement(element, false);

    internal static IInputElement GetFocusedElement(DependencyObject element, bool validate)
    {
        ArgumentNullException.ThrowIfNull(element);

        DependencyObject focusedElement = (DependencyObject)element.GetValue(FocusedElementProperty);

        if (validate && focusedElement is not null)
        {
            DependencyObject focusScope = element;

            if (GetWindowSource(focusScope) != GetWindowSource(focusedElement))
            {
                SetFocusedElement(focusScope, null);
                focusedElement = null;
            }
        }

        return (IInputElement)focusedElement;
    }

    /// <summary>
    /// Sets logical focus on the specified element.
    /// </summary>
    /// <param name="scope">
    /// The focus scope in which to make the specified element the <b>FocusManager.FocusedElement</b>.
    /// </param>
    /// <param name="value">
    /// The element to give logical focus to.
    /// </param>
    public static void SetFocusedElement(DependencyObject scope, IInputElement value)
    {
        ArgumentNullException.ThrowIfNull(scope);
        scope.SetValueInternal(FocusedElementProperty, value);
    }

    private static void OnFocusedElementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        IInputElement newFocusedElement = (IInputElement)e.NewValue;
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

    /// <summary>
    /// Identifies the <b>FocusManager.IsFocusScope</b> attached property.
    /// </summary>
    internal static readonly DependencyProperty IsFocusScopeProperty =
        DependencyProperty.RegisterAttached(
            "IsFocusScope",
            typeof(bool),
            typeof(FocusManager),
            new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Determines whether the specified <see cref="DependencyObject"/> is a focus scope.
    /// </summary>
    /// <param name="element">
    /// The element from which to read the attached property.
    /// </param>
    /// <returns>
    /// true if <b>FocusManager.IsFocusScope</b> is set to true on the specified element; otherwise, false.
    /// </returns>
    internal static bool GetIsFocusScope(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return (bool)element.GetValue(IsFocusScopeProperty);
    }

    /// <summary>
    /// Sets the specified <see cref="DependencyObject"/> as a focus scope.
    /// </summary>
    /// <param name="element">
    /// The element to make a focus scope.
    /// </param>
    /// <param name="value">
    /// true if element is a focus scope; otherwise, false.
    /// </param>
    internal static void SetIsFocusScope(DependencyObject element, bool value)
    {
        ArgumentNullException.ThrowIfNull(element);
        element.SetValueInternal(IsFocusScopeProperty, value);
    }

    /// <summary>
    /// Determines the closest ancestor of the specified element that has <b>FocusManager.IsFocusScope</b> set to true.
    /// </summary>
    /// <param name="element">
    /// The element to get the closest focus scope for.
    /// </param>
    /// <returns>
    /// The focus scope for the specified element.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    public static DependencyObject GetFocusScope(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        if ((bool)element.GetValue(IsFocusScopeProperty))
        {
            return element;
        }

        if (LogicalTreeHelper.GetParent(element) is DependencyObject logicalParent)
        {
            return GetFocusScope(logicalParent);
        }

        if (VisualTreeHelper.GetParent(element) is DependencyObject visualParent)
        {
            return GetFocusScope(visualParent);
        }

        // If visual and logical parent is null - then the element is implicit focus scope
        return element;
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