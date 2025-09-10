
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
/// Represents the mouse device to a specific thread.
/// </summary>
public static class Mouse
{
    /// <summary>
    /// Identifies the Mouse.PreviewMouseDown attached event.
    /// </summary>
    public static readonly RoutedEvent PreviewMouseDownEvent =
        EventManager.RegisterCoreEvent(
            "PreviewMouseDown",
            RoutingStrategy.Tunnel,
            typeof(MouseButtonEventHandler),
            typeof(Mouse));

    /// <summary>
    /// Adds a handler for the Mouse.PreviewMouseDown attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler.
    /// </param>
    public static void AddPreviewMouseDownHandler(DependencyObject element, MouseButtonEventHandler handler)
        => UIElement.AddHandler(element, PreviewMouseDownEvent, handler);

    /// <summary>
    /// Removes a handler for the Mouse.PreviewMouseDown attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler.
    /// </param>
    public static void RemovePreviewMouseDownHandler(DependencyObject element, MouseButtonEventHandler handler)
        => UIElement.RemoveHandler(element, PreviewMouseDownEvent, handler);

    /// <summary>
    /// Identifies the Mouse.MouseDown attached event.
    /// </summary>
    public static readonly RoutedEvent MouseDownEvent =
        EventManager.RegisterCoreEvent(
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
        => UIElement.AddHandler(element, MouseDownEvent, handler);

    /// <summary>
    /// Removes a handler for the Mouse.MouseDown attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler.
    /// </param>
    public static void RemoveMouseDownHandler(DependencyObject element, MouseButtonEventHandler handler)
        => UIElement.RemoveHandler(element, MouseDownEvent, handler);

    /// <summary>
    /// Identifies the Mouse.PreviewMouseUp attached event.
    /// </summary>
    public static readonly RoutedEvent PreviewMouseUpEvent =
        EventManager.RegisterCoreEvent(
            "PreviewMouseUp",
            RoutingStrategy.Tunnel,
            typeof(MouseButtonEventHandler),
            typeof(Mouse));

    /// <summary>
    /// Adds a handler for the Mouse.PreviewMouseUp attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler.
    /// </param>
    public static void AddPreviewMouseUpHandler(DependencyObject element, MouseButtonEventHandler handler)
        => UIElement.AddHandler(element, PreviewMouseUpEvent, handler);

    /// <summary>
    /// Removes a handler for the Mouse.PreviewMouseUp attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler.
    /// </param>
    public static void RemovePreviewMouseUpHandler(DependencyObject element, MouseButtonEventHandler handler)
        => UIElement.RemoveHandler(element, PreviewMouseUpEvent, handler);

    /// <summary>
    /// Identifies the Mouse.MouseUp attached event.
    /// </summary>
    public static readonly RoutedEvent MouseUpEvent =
        EventManager.RegisterCoreEvent(
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
        => UIElement.AddHandler(element, MouseUpEvent, handler);

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
        => UIElement.RemoveHandler(element, MouseUpEvent, handler);

    /// <summary>
    /// Identifies the Mouse.PreviewMouseMove attached event.
    /// </summary>
    public static readonly RoutedEvent PreviewMouseMoveEvent =
        EventManager.RegisterCoreEvent(
            "PreviewMouseMove",
            RoutingStrategy.Tunnel,
            typeof(MouseEventHandler),
            typeof(Mouse));

    /// <summary>
    /// Adds a handler for the Mouse.PreviewMouseMove attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler.
    /// </param>
    public static void AddPreviewMouseMoveHandler(DependencyObject element, MouseEventHandler handler)
        => UIElement.AddHandler(element, PreviewMouseMoveEvent, handler);

    /// <summary>
    /// Removes a handler for the Mouse.PreviewMouseMove attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler.
    /// </param>
    public static void RemovePreviewMouseMoveHandler(DependencyObject element, MouseEventHandler handler)
        => UIElement.RemoveHandler(element, PreviewMouseMoveEvent, handler);

    /// <summary>
    /// Identifies the Mouse.MouseMove attached event.
    /// </summary>
    public static readonly RoutedEvent MouseMoveEvent =
        EventManager.RegisterCoreEvent(
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
        => UIElement.AddHandler(element, MouseMoveEvent, handler);

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
        => UIElement.RemoveHandler(element, MouseMoveEvent, handler);

    /// <summary>
    /// dentifies the Mouse.MouseEnter attached event.
    /// </summary>
    public static readonly RoutedEvent MouseEnterEvent =
        EventManager.RegisterCoreEvent(
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
        => UIElement.AddHandler(element, MouseEnterEvent, handler);

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
        => UIElement.RemoveHandler(element, MouseEnterEvent, handler);

    /// <summary>
    /// Identifies the Mouse.MouseLeave attached event.
    /// </summary>
    public static readonly RoutedEvent MouseLeaveEvent =
        EventManager.RegisterCoreEvent(
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
        => UIElement.AddHandler(element, MouseLeaveEvent, handler);

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
        => UIElement.RemoveHandler(element, MouseLeaveEvent, handler);

    /// <summary>
    /// Identifies the Mouse.GotMouseCapture attached event.
    /// </summary>
    public static readonly RoutedEvent GotMouseCaptureEvent =
        EventManager.RegisterCoreEvent(
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
        => UIElement.AddHandler(element, GotMouseCaptureEvent, handler);

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
        => UIElement.RemoveHandler(element, GotMouseCaptureEvent, handler);

    /// <summary>
    /// Identifies the Mouse.LostMouseCapture attached event.
    /// </summary>
    public static readonly RoutedEvent LostMouseCaptureEvent =
        EventManager.RegisterCoreEvent(
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
        => UIElement.AddHandler(element, LostMouseCaptureEvent, handler);

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
        => UIElement.RemoveHandler(element, LostMouseCaptureEvent, handler);

    /// <summary>
    /// Identifies the Mouse.PreviewMouseWheel attached event.
    /// </summary>
    public static readonly RoutedEvent PreviewMouseWheelEvent =
        EventManager.RegisterCoreEvent(
            "PreviewMouseWheel",
            RoutingStrategy.Tunnel,
            typeof(MouseWheelEventHandler),
            typeof(Mouse));

    /// <summary>
    /// Adds a handler for the Mouse.PreviewMouseWheel attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler.
    /// </param>
    public static void AddPreviewMouseWheelHandler(DependencyObject element, MouseWheelEventHandler handler)
        => UIElement.AddHandler(element, PreviewMouseWheelEvent, handler);

    /// <summary>
    /// Removes a handler for the Mouse.PreviewMouseWheel attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler.
    /// </param>
    public static void RemovePreviewMouseWheelHandler(DependencyObject element, MouseWheelEventHandler handler)
        => UIElement.RemoveHandler(element, PreviewMouseWheelEvent, handler);

    /// <summary>
    /// Identifies the Mouse.MouseWheel attached event.
    /// </summary>
    public static readonly RoutedEvent MouseWheelEvent =
        EventManager.RegisterCoreEvent(
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
        => UIElement.AddHandler(element, MouseWheelEvent, handler);

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
        => UIElement.RemoveHandler(element, MouseWheelEvent, handler);
}
