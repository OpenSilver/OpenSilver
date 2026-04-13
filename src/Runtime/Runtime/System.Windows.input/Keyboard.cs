
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

namespace System.Windows.Input;

/// <summary>
/// Represents the keyboard device.
/// </summary>
public static class Keyboard
{
    /// <summary>
    /// Identifies the Keyboard.PreviewKeyDown attached event.
    /// </summary>
    public static readonly RoutedEvent PreviewKeyDownEvent =
        EventManager.RegisterCoreEvent(
            "PreviewKeyDown",
            RoutingStrategy.Tunnel,
            typeof(KeyEventHandler),
            typeof(Keyboard));

    /// <summary>
    /// Adds a handler for the Keyboard.PreviewKeyDown attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler to be added.
    /// </param>
    public static void AddPreviewKeyDownHandler(DependencyObject element, KeyEventHandler handler)
        => UIElement.AddHandler(element, PreviewKeyDownEvent, handler);

    /// <summary>
    /// Removes a handler for the Keyboard.PreviewKeyDown attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler to be removed.
    /// </param>
    public static void RemovePreviewKeyDownHandler(DependencyObject element, KeyEventHandler handler)
        => UIElement.RemoveHandler(element, PreviewKeyDownEvent, handler);

    /// <summary>
    /// Identifies the Keyboard.KeyDown attached event.
    /// </summary>
    public static readonly RoutedEvent KeyDownEvent =
        EventManager.RegisterCoreEvent(
            "KeyDown",
            RoutingStrategy.Bubble,
            typeof(KeyEventHandler),
            typeof(Keyboard));

    /// <summary>
    /// Adds a handler for the Keyboard.KeyDown attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler to be added.
    /// </param>
    public static void AddKeyDownHandler(DependencyObject element, KeyEventHandler handler)
        => UIElement.AddHandler(element, KeyDownEvent, handler);

    /// <summary>
    /// Removes a handler for the Keyboard.KeyDown attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler to be removed.
    /// </param>
    public static void RemoveKeyDownHandler(DependencyObject element, KeyEventHandler handler)
        => UIElement.RemoveHandler(element, KeyDownEvent, handler);

    /// <summary>
    /// Identifies the Keyboard.PreviewKeyUp attached event.
    /// </summary>
    public static readonly RoutedEvent PreviewKeyUpEvent =
        EventManager.RegisterCoreEvent(
            "PreviewKeyUp",
            RoutingStrategy.Tunnel,
            typeof(KeyEventHandler),
            typeof(Keyboard));

    /// <summary>
    /// Adds a handler for the Keyboard.PreviewKeyUp attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler to be added.
    /// </param>
    public static void AddPreviewKeyUpHandler(DependencyObject element, KeyEventHandler handler)
        => UIElement.AddHandler(element, PreviewKeyUpEvent, handler);

    /// <summary>
    /// Removes a handler for the Keyboard.PreviewKeyUp attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler to be removed.
    /// </param>
    public static void RemovePreviewKeyUpHandler(DependencyObject element, KeyEventHandler handler)
        => UIElement.RemoveHandler(element, PreviewKeyUpEvent, handler);

    /// <summary>
    /// Identifies the Keyboard.KeyUp attached event.
    /// </summary>
    public static readonly RoutedEvent KeyUpEvent =
        EventManager.RegisterCoreEvent(
            "KeyUp",
            RoutingStrategy.Bubble,
            typeof(KeyEventHandler),
            typeof(Keyboard));

    /// <summary>
    /// Adds a handler for the Keyboard.KeyUp attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler to be added.
    /// </param>
    public static void AddKeyUpHandler(DependencyObject element, KeyEventHandler handler)
        => UIElement.AddHandler(element, KeyUpEvent, handler);

    /// <summary>
    /// Removes a handler for the Keyboard.KeyUp attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler to be removed.
    /// </param>
    public static void RemoveKeyUpHandler(DependencyObject element, KeyEventHandler handler)
        => UIElement.RemoveHandler(element, KeyUpEvent, handler);

    /// <summary>
    /// Identifies the Keyboard.PreviewGotKeyboardFocus attached event.
    /// </summary>
    public static readonly RoutedEvent PreviewGotKeyboardFocusEvent =
        EventManager.RegisterRoutedEvent(
            "PreviewGotKeyboardFocus",
            RoutingStrategy.Tunnel,
            typeof(KeyboardFocusChangedEventHandler),
            typeof(Keyboard));

    /// <summary>
    /// Adds a handler for the Keyboard.PreviewGotKeyboardFocus attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler to be added.
    /// </param>
    public static void AddPreviewGotKeyboardFocusHandler(DependencyObject element, KeyboardFocusChangedEventHandler handler)
        => UIElement.AddHandler(element, PreviewGotKeyboardFocusEvent, handler);

    /// <summary>
    /// Removes a handler for the Keyboard.PreviewGotKeyboardFocus attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler to be removed.
    /// </param>
    public static void RemovePreviewGotKeyboardFocusHandler(DependencyObject element, KeyboardFocusChangedEventHandler handler)
        => UIElement.RemoveHandler(element, PreviewGotKeyboardFocusEvent, handler);

    /// <summary>
    /// Identifies the Keyboard.GotKeyboardFocus attached event.
    /// </summary>
    public static readonly RoutedEvent GotKeyboardFocusEvent =
        EventManager.RegisterRoutedEvent(
            "GotKeyboardFocus",
            RoutingStrategy.Bubble,
            typeof(KeyboardFocusChangedEventHandler),
            typeof(Keyboard));

    /// <summary>
    /// Adds a handler for the Keyboard.GotKeyboardFocus attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler to be added.
    /// </param>
    public static void AddGotKeyboardFocusHandler(DependencyObject element, KeyboardFocusChangedEventHandler handler)
        => UIElement.AddHandler(element, GotKeyboardFocusEvent, handler);

    /// <summary>
    /// Removes a handler for the Keyboard.GotKeyboardFocus attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler to be removed.
    /// </param>
    public static void RemoveGotKeyboardFocusHandler(DependencyObject element, KeyboardFocusChangedEventHandler handler)
        => UIElement.RemoveHandler(element, GotKeyboardFocusEvent, handler);

    /// <summary>
    /// Identifies the Keyboard.PreviewLostKeyboardFocus attached event.
    /// </summary>
    public static readonly RoutedEvent PreviewLostKeyboardFocusEvent =
        EventManager.RegisterRoutedEvent(
            "PreviewLostKeyboardFocus",
            RoutingStrategy.Tunnel,
            typeof(KeyboardFocusChangedEventHandler),
            typeof(Keyboard));

    /// <summary>
    /// Adds a handler for the Keyboard.PreviewLostKeyboardFocus attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler to be added.
    /// </param>
    public static void AddPreviewLostKeyboardFocusHandler(DependencyObject element, KeyboardFocusChangedEventHandler handler)
        => UIElement.AddHandler(element, PreviewLostKeyboardFocusEvent, handler);

    /// <summary>
    /// Removes a handler for the Keyboard.PreviewLostKeyboardFocus attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler to be removed.
    /// </param>
    public static void RemovePreviewLostKeyboardFocusHandler(DependencyObject element, KeyboardFocusChangedEventHandler handler)
        => UIElement.RemoveHandler(element, PreviewLostKeyboardFocusEvent, handler);

    /// <summary>
    /// Identifies the Keyboard.LostKeyboardFocus attached event.
    /// </summary>
    public static readonly RoutedEvent LostKeyboardFocusEvent =
        EventManager.RegisterRoutedEvent(
            "LostKeyboardFocus",
            RoutingStrategy.Bubble,
            typeof(KeyboardFocusChangedEventHandler),
            typeof(Keyboard));

    /// <summary>
    /// Adds a handler for the Keyboard.LostKeyboardFocus attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler to be added.
    /// </param>
    public static void AddLostKeyboardFocusHandler(DependencyObject element, KeyboardFocusChangedEventHandler handler)
        => UIElement.AddHandler(element, LostKeyboardFocusEvent, handler);

    /// <summary>
    /// Removes a handler for the Keyboard.LostKeyboardFocus attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler to be removed.
    /// </param>
    public static void RemoveLostKeyboardFocusHandler(DependencyObject element, KeyboardFocusChangedEventHandler handler)
        => UIElement.RemoveHandler(element, LostKeyboardFocusEvent, handler);

    /// <summary>
    /// Identifies the Keyboard.PreviewKeyboardInputProviderAcquireFocus attached event.
    /// </summary>
    public static readonly RoutedEvent PreviewKeyboardInputProviderAcquireFocusEvent =
        EventManager.RegisterRoutedEvent(
            "PreviewKeyboardInputProviderAcquireFocus",
            RoutingStrategy.Tunnel,
            typeof(KeyboardInputProviderAcquireFocusEventHandler),
            typeof(Keyboard));

    /// <summary>
    /// Adds a handler for the Keyboard.PreviewKeyboardInputProviderAcquireFocus attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler to be added.
    /// </param>
    public static void AddPreviewKeyboardInputProviderAcquireFocusHandler(DependencyObject element, KeyboardInputProviderAcquireFocusEventHandler handler)
        => UIElement.AddHandler(element, PreviewKeyboardInputProviderAcquireFocusEvent, handler);

    /// <summary>
    /// Removes a handler for the Keyboard.PreviewKeyboardInputProviderAcquireFocus attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler to be removed.
    /// </param>
    public static void RemovePreviewKeyboardInputProviderAcquireFocusHandler(DependencyObject element, KeyboardInputProviderAcquireFocusEventHandler handler)
        => UIElement.RemoveHandler(element, PreviewKeyboardInputProviderAcquireFocusEvent, handler);

    /// <summary>
    /// Identifies the Keyboard.KeyboardInputProviderAcquireFocus attached event.
    /// </summary>
    public static readonly RoutedEvent KeyboardInputProviderAcquireFocusEvent =
        EventManager.RegisterRoutedEvent(
            "KeyboardInputProviderAcquireFocus",
            RoutingStrategy.Bubble,
            typeof(KeyboardInputProviderAcquireFocusEventHandler),
            typeof(Keyboard));

    /// <summary>
    /// Adds a handler for the Keyboard.KeyboardInputProviderAcquireFocus attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler to be added.
    /// </param>
    public static void AddKeyboardInputProviderAcquireFocusHandler(DependencyObject element, KeyboardInputProviderAcquireFocusEventHandler handler)
        => UIElement.AddHandler(element, KeyboardInputProviderAcquireFocusEvent, handler);

    /// <summary>
    /// Removes a handler for the Keyboard.KeyboardInputProviderAcquireFocus attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler to be removed.
    /// </param>
    public static void RemoveKeyboardInputProviderAcquireFocusHandler(DependencyObject element, KeyboardInputProviderAcquireFocusEventHandler handler)
        => UIElement.RemoveHandler(element, KeyboardInputProviderAcquireFocusEvent, handler);

    /// <summary>
    /// Gets the primary keyboard input device.
    /// </summary>
    /// <returns>
    /// The device.
    /// </returns>
    public static KeyboardDevice PrimaryDevice => InputManager.Current.PrimaryKeyboardDevice;

    /// <summary>
    /// Gets the element that has keyboard focus.
    /// </summary>
    /// <returns>
    /// The focused element.
    /// </returns>
    public static IInputElement FocusedElement => PrimaryDevice.FocusedElement;

    /// <summary>
    /// Gets the set of <see cref="ModifierKeys"/> that are currently pressed.
    /// </summary>
    /// <returns>
    /// A bitwise combination of the <see cref="ModifierKeys"/> values.
    /// </returns>
    public static ModifierKeys Modifiers => PrimaryDevice.Modifiers;

    /// <summary>
    /// Sets keyboard focus on the specified element.
    /// </summary>
    /// <param name="element">
    /// The element on which to set keyboard focus.
    /// </param>
    /// <returns>
    /// The element with keyboard focus.
    /// </returns>
    public static IInputElement Focus(IInputElement element) => PrimaryDevice.Focus(element);

    /// <summary>
    /// Clears focus.
    /// </summary>
    public static void ClearFocus() => PrimaryDevice.ClearFocus();

    internal static bool IsFocusable(DependencyObject uie) => KeyboardNavigation.Current.IsTabStop(uie);
}
