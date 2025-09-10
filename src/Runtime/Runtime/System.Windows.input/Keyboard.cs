
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
    /// Gets the set of <see cref="ModifierKeys"/> that are currently pressed.
    /// </summary>
    public static ModifierKeys Modifiers => InputManager.Current.GetKeyboardModifiers();

    internal static bool IsFocusable(UIElement uie) => KeyboardNavigation.Current.IsTabStop(uie);

    internal static IInputElement FocusedElement => FocusManager.GetFocusedElement(Window.Current) as IInputElement;
}
