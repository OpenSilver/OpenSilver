
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

using System.ComponentModel;

namespace System.Windows.Input;

/// <summary>
/// Represents the mouse device to a specific thread.
/// </summary>
public static class Mouse
{
    /// <summary>
    /// Represents the number of units the mouse wheel is rotated to scroll one line.
    /// </summary>
    public const int MouseWheelDeltaForOneLine = 120;

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

    /// <summary>
    /// Gets the primary mouse device.
    /// </summary>
    /// <returns>
    /// The device.
    /// </returns>
    public static MouseDevice PrimaryDevice => InputManager.Current.PrimaryMouseDevice;

    /// <summary>
    /// Gets the element that has captured the mouse.
    /// </summary>
    /// <returns>
    /// The element captured by the mouse.
    /// </returns>
    public static IInputElement Captured => PrimaryDevice.Captured;

    /// <summary>
    /// Gets the element the mouse pointer is directly over.
    /// </summary>
    /// <returns>
    /// The element the mouse pointer is over.
    /// </returns>
    public static IInputElement DirectlyOver => PrimaryDevice.DirectlyOver;

    /// <summary>
    /// Gets the state of the left button of the mouse.
    /// </summary>
    /// <returns>
    /// The state of the left mouse button.
    /// </returns>
    public static MouseButtonState LeftButton => PrimaryDevice.LeftButton;

    /// <summary>
    /// Gets the state of the middle button of the mouse.
    /// </summary>
    /// <returns>
    /// The state of the middle mouse button.
    /// </returns>
    public static MouseButtonState MiddleButton => PrimaryDevice.MiddleButton;

    /// <summary>
    /// Gets the state of the right button.
    /// </summary>
    /// <returns>
    /// The state of the right mouse button.
    /// </returns>
    public static MouseButtonState RightButton => PrimaryDevice.RightButton;

    /// <summary>
    /// Captures mouse input to the specified element.
    /// </summary>
    /// <param name="element">
    /// The element to capture the mouse.
    /// </param>
    /// <returns>
    /// true if the element was able to capture the mouse; otherwise, false.
    /// </returns>
    public static bool Capture(UIElement element) => PrimaryDevice.Capture(element);

    /// <summary>
    /// Captures mouse input to the specified element.
    /// </summary>
    /// <param name="element">
    /// The element to capture the mouse.
    /// </param>
    /// <returns>
    /// true if the element was able to capture the mouse; otherwise, false.
    /// </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool Capture(IInputElement element) => PrimaryDevice.Capture(element);

    /// <summary>
    /// Gets the position of the mouse relative to a specified element.
    /// </summary>
    /// <param name="relativeTo">
    /// The coordinate space in which to calculate the position of the mouse.
    /// </param>
    /// <returns>
    /// The position of the mouse relative to the parameter relativeTo.
    /// </returns>
    public static Point GetPosition(UIElement relativeTo) => PrimaryDevice.GetPosition(relativeTo);

    /// <summary>
    /// Gets the position of the mouse relative to a specified element.
    /// </summary>
    /// <param name="relativeTo">
    /// The coordinate space in which to calculate the position of the mouse.
    /// </param>
    /// <returns>
    /// The position of the mouse relative to the parameter relativeTo.
    /// </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Point GetPosition(IInputElement relativeTo) => PrimaryDevice.GetPosition(relativeTo);
}
