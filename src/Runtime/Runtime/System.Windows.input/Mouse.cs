
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

using OpenSilver.Internal;

namespace System.Windows.Input;

/// <summary>
/// Represents the mouse device to a specific thread.
/// </summary>
public static class Mouse
{
    /// <summary>
    /// Identifies the Mouse.MouseDown attached event.
    /// </summary>
    public static readonly RoutedEvent MouseDownEvent =
        EventManager.RegisterRoutedEvent(
            "MouseDown",
            RoutingStrategy.Bubble,
            typeof(MouseButtonEventHandler),
            typeof(Mouse));

    /// <summary>
    /// Adds a handler for the Mouse.MouseDown attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler.
    /// </param>
    public static void AddMouseDownHandler(DependencyObject element, MouseButtonEventHandler handler)
        => AddHandler(element, MouseDownEvent, handler);

    /// <summary>
    /// Removes a handler for the System.Windows.Input.Mouse.MouseDown attached event.
    /// </summary>
    /// <param name="element">
    /// The System.Windows.UIElement or System.Windows.ContentElement that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler.
    /// </param>
    public static void RemoveMouseDownHandler(DependencyObject element, MouseButtonEventHandler handler)
        => RemoveHandler(element, MouseDownEvent, handler);

    /// <summary>
    /// Identifies the Mouse.MouseUp attached event.
    /// </summary>
    public static readonly RoutedEvent MouseUpEvent =
        EventManager.RegisterRoutedEvent(
            "MouseUp",
            RoutingStrategy.Bubble,
            typeof(MouseButtonEventHandler),
            typeof(Mouse));

    /// <summary>
    /// Adds a handler for the Mouse.MouseUp attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler.
    /// </param>
    public static void AddMouseUpHandler(DependencyObject element, MouseButtonEventHandler handler)
        => AddHandler(element, MouseUpEvent, handler);

    /// <summary>
    /// Removes a handler for the Mouse.MouseUp attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler.
    /// </param>
    public static void RemoveMouseUpHandler(DependencyObject element, MouseButtonEventHandler handler)
        => RemoveHandler(element, MouseUpEvent, handler);

    /// <summary>
    /// Identifies the Mouse.MouseMove attached event.
    /// </summary>
    public static readonly RoutedEvent MouseMoveEvent =
        EventManager.RegisterRoutedEvent(
            "MouseMove",
            RoutingStrategy.Bubble,
            typeof(MouseEventHandler),
            typeof(Mouse));

    /// <summary>
    /// Adds a handler for the Mouse.MouseMove attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler.
    /// </param>
    public static void AddMouseMoveHandler(DependencyObject element, MouseEventHandler handler)
        => AddHandler(element, MouseMoveEvent, handler);

    /// <summary>
    /// Removes a handler for the Mouse.MouseMove attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler.
    /// </param>
    public static void RemoveMouseMoveHandler(DependencyObject element, MouseEventHandler handler)
        => RemoveHandler(element, MouseMoveEvent, handler);

    /// <summary>
    /// dentifies the Mouse.MouseEnter attached event.
    /// </summary>
    public static readonly RoutedEvent MouseEnterEvent =
        EventManager.RegisterRoutedEvent(
            "MouseEnter",
            RoutingStrategy.Direct,
            typeof(MouseEventHandler),
            typeof(Mouse));

    /// <summary>
    /// Adds a handler for the Mouse.MouseEnter attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler.
    /// </param>
    public static void AddMouseEnterHandler(DependencyObject element, MouseEventHandler handler)
        => AddHandler(element, MouseEnterEvent, handler);

    /// <summary>
    /// Removes a handler for the Mouse.MouseEnter attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler.
    /// </param>
    public static void RemoveMouseEnterHandler(DependencyObject element, MouseEventHandler handler)
        => RemoveHandler(element, MouseEnterEvent, handler);

    /// <summary>
    /// Identifies the Mouse.MouseLeave attached event.
    /// </summary>
    public static readonly RoutedEvent MouseLeaveEvent =
        EventManager.RegisterRoutedEvent(
            "MouseLeave",
            RoutingStrategy.Direct,
            typeof(MouseEventHandler),
            typeof(Mouse));

    /// <summary>
    /// Adds a handler for the Mouse.MouseLeave attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler.
    /// </param>
    public static void AddMouseLeaveHandler(DependencyObject element, MouseEventHandler handler)
        => AddHandler(element, MouseLeaveEvent, handler);

    /// <summary>
    /// Removes a handler for the Mouse.MouseLeave attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler.
    /// </param>
    public static void RemoveMouseLeaveHandler(DependencyObject element, MouseEventHandler handler)
        => RemoveHandler(element, MouseLeaveEvent, handler);

    /// <summary>
    /// Identifies the Mouse.GotMouseCapture attached event.
    /// </summary>
    public static readonly RoutedEvent GotMouseCaptureEvent =
        EventManager.RegisterRoutedEvent(
            "GotMouseCapture",
            RoutingStrategy.Bubble,
            typeof(MouseEventHandler),
            typeof(Mouse));

    /// <summary>
    /// Adds a handler for the Mouse.GotMouseCapture attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> or that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler.
    /// </param>
    public static void AddGotMouseCaptureHandler(DependencyObject element, MouseEventHandler handler)
        => AddHandler(element, GotMouseCaptureEvent, handler);

    /// <summary>
    /// Removes a handler for the Mouse.GotMouseCapture attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler.
    /// </param>
    public static void RemoveGotMouseCaptureHandler(DependencyObject element, MouseEventHandler handler)
        => RemoveHandler(element, GotMouseCaptureEvent, handler);

    /// <summary>
    /// Identifies the Mouse.LostMouseCapture attached event.
    /// </summary>
    public static readonly RoutedEvent LostMouseCaptureEvent =
        EventManager.RegisterRoutedEvent(
            "LostMouseCapture",
            RoutingStrategy.Bubble,
            typeof(MouseEventHandler),
            typeof(Mouse));

    /// <summary>
    /// Adds a handler for the Mouse.LostMouseCapture attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler.
    /// </param>
    public static void AddLostMouseCaptureHandler(DependencyObject element, MouseEventHandler handler)
        => AddHandler(element, LostMouseCaptureEvent, handler);

    /// <summary>
    /// Removes a handler for the Mouse.LostMouseCapture attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler.
    /// </param>
    public static void RemoveLostMouseCaptureHandler(DependencyObject element, MouseEventHandler handler)
        => RemoveHandler(element, LostMouseCaptureEvent, handler);

    /// <summary>
    /// Identifies the Mouse.MouseWheel attached event.
    /// </summary>
    public static readonly RoutedEvent MouseWheelEvent =
        EventManager.RegisterRoutedEvent(
            "MouseWheel",
            RoutingStrategy.Bubble,
            typeof(MouseWheelEventHandler),
            typeof(Mouse));

    /// <summary>
    /// Adds a handler for the Mouse.MouseWheel attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler.
    /// </param>
    public static void AddMouseWheelHandler(DependencyObject element, MouseWheelEventHandler handler)
        => AddHandler(element, MouseWheelEvent, handler);

    /// <summary>
    /// Removes a handler for the Mouse.MouseWheel attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler.
    /// </param>
    public static void RemoveMouseWheelHandler(DependencyObject element, MouseWheelEventHandler handler)
        => RemoveHandler(element, MouseWheelEvent, handler);

    private static void AddHandler(DependencyObject d, RoutedEvent routedEvent, Delegate handler)
    {
        switch (d)
        {
            case UIElement uiElement:
                uiElement.AddHandler(routedEvent, handler);
                break;

            case IUIElement iuiElement:
                iuiElement.AddHandler(routedEvent, handler, false);
                break;

            default:
                throw new ArgumentException(string.Format(Strings.Invalid_IInputElement, d.GetType()));
        }
    }

    private static void RemoveHandler(DependencyObject d, RoutedEvent routedEvent, Delegate handler)
    {
        switch (d)
        {
            case UIElement uiElement:
                uiElement.RemoveHandler(routedEvent, handler);
                break;

            case IUIElement iuiElement:
                iuiElement.RemoveHandler(routedEvent, handler);
                break;

            default:
                throw new ArgumentException(string.Format(Strings.Invalid_IInputElement, d.GetType()));
        }
    }
}
