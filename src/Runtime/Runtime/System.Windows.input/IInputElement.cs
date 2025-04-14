
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

using System.Windows.Input;

namespace System.Windows;

/// <summary>
/// Establishes the common events and also the event-related properties and methods for basic input processing 
/// by Windows Presentation Foundation (WPF) elements.
/// </summary>
public interface IInputElement
{
    /// <summary>
    /// Gets a value that indicates whether the mouse pointer is located over this element (including visual 
    /// children elements that are inside its bounds).
    /// </summary>
    /// <returns>
    /// true if the mouse pointer is over the element or its child elements; otherwise, false.
    /// </returns>
    bool IsMouseOver { get; }

    /// <summary>
    /// Gets or sets a value that indicates whether focus can be set to this element.
    /// </summary>
    /// <returns>
    /// true if the element can have focus set to it; otherwise, false.
    /// </returns>
    bool Focusable { get; set; }

    /// <summary>
    /// Gets a value that indicates whether this element is enabled in the user interface (UI).
    /// </summary>
    /// <returns>
    /// true if the element is enabled; otherwise, false.
    /// </returns>
    bool IsEnabled { get; }

    /// <summary>
    /// Gets a value that indicates whether the mouse is captured to this element.
    /// </summary>
    /// <returns>
    /// true if the element has mouse capture; otherwise, false.
    /// </returns>
    bool IsMouseCaptured { get; }

    /// <summary>
    /// Occurs when the right mouse button is released while the mouse pointer is over the element.
    /// </summary>
    event MouseButtonEventHandler MouseRightButtonUp;

    /// <summary>
    /// Occurs when this element gets text in a device-independent manner.
    /// </summary>
    event TextCompositionEventHandler TextInput;

    /// <summary>
    /// Occurs when the element captures the mouse.
    /// </summary>
    event MouseEventHandler GotMouseCapture;

    /// <summary>
    /// Occurs when a key is pressed while the keyboard is focused on this element.
    /// </summary>
    event KeyEventHandler KeyDown;

    /// <summary>
    /// Occurs when a key is released while the keyboard is focused on this element.
    /// </summary>
    event KeyEventHandler KeyUp;

    /// <summary>
    /// Occurs when this element loses mouse capture.
    /// </summary>
    event MouseEventHandler LostMouseCapture;

    /// <summary>
    /// Occurs when the mouse pointer enters the bounds of this element.
    /// </summary>
    event MouseEventHandler MouseEnter;

    /// <summary>
    /// Occurs when the left mouse button is pressed while the mouse pointer is over the element.
    /// </summary>
    event MouseButtonEventHandler MouseLeftButtonDown;

    /// <summary>
    /// Occurs when the left mouse button is released while the mouse pointer is over the element.
    /// </summary>
    event MouseButtonEventHandler MouseLeftButtonUp;

    /// <summary>
    /// Occurs when the mouse pointer moves while the mouse pointer is over the element.
    /// </summary>
    event MouseEventHandler MouseMove;

    /// <summary>
    /// Occurs when the right mouse button is pressed while the mouse pointer is over the element.
    /// </summary>
    event MouseButtonEventHandler MouseRightButtonDown;

    /// <summary>
    /// Occurs when the mouse wheel moves while the mouse pointer is over this element.
    /// </summary>
    event MouseWheelEventHandler MouseWheel;

    /// <summary>
    /// Occurs when the mouse pointer leaves the bounds of this element.
    /// </summary>
    event MouseEventHandler MouseLeave;

    /// <summary>
    /// Adds a routed event handler for a specific routed event to an element.
    /// </summary>
    /// <param name="routedEvent">
    /// The identifier for the routed event that is being handled.
    /// </param>
    /// <param name="handler">
    /// A reference to the handler implementation.
    /// </param>
    void AddHandler(RoutedEvent routedEvent, Delegate handler);

    /// <summary>
    /// Attempts to force capture of the mouse to this element.
    /// </summary>
    /// <returns>
    /// true if the mouse is successfully captured; otherwise, false.
    /// </returns>
    bool CaptureMouse();

    /// <summary>
    /// Attempts to focus the keyboard on this element.
    /// </summary>
    /// <returns>
    /// true if keyboard focus is moved to this element or already was on this element; otherwise, false.
    /// </returns>
    bool Focus();

    /// <summary>
    /// Raises the routed event that is specified by the <see cref="RoutedEventArgs.RoutedEvent"/> property within 
    /// the provided <see cref="RoutedEventArgs"/>.
    /// </summary>
    /// <param name="e">
    /// An instance of the <see cref="RoutedEventArgs"/> class that contains the identifier for the event to raise.
    /// </param>
    void RaiseEvent(RoutedEventArgs e);

    /// <summary>
    /// Releases the mouse capture, if this element holds the capture.
    /// </summary>
    void ReleaseMouseCapture();

    /// <summary>
    /// Removes all instances of the specified routed event handler from this element.
    /// </summary>
    /// <param name="routedEvent">
    /// Identifier of the routed event for which the handler is attached.
    /// </param>
    /// <param name="handler">
    /// The specific handler implementation to remove from this element's event handler collection.
    /// </param>
    void RemoveHandler(RoutedEvent routedEvent, Delegate handler);

    // bool IsKeyboardFocusWithin { get; }
    // bool IsStylusOver { get; }
    // bool IsStylusDirectlyOver { get; }
    // bool IsStylusCaptured { get; }
    // bool IsKeyboardFocused { get; }
    // bool IsMouseDirectlyOver { get; }

    // event StylusEventHandler PreviewStylusMove;
    // event StylusEventHandler PreviewStylusInRange;
    // event StylusEventHandler PreviewStylusInAirMove;
    // event StylusDownEventHandler PreviewStylusDown;
    // event StylusButtonEventHandler PreviewStylusButtonUp;
    // event StylusButtonEventHandler PreviewStylusButtonDown;
    // event MouseWheelEventHandler PreviewMouseWheel;
    // event MouseButtonEventHandler PreviewMouseRightButtonUp;
    // event MouseButtonEventHandler PreviewMouseRightButtonDown;
    // event MouseEventHandler PreviewMouseMove;
    // event StylusEventHandler PreviewStylusOutOfRange;
    // event StylusSystemGestureEventHandler PreviewStylusSystemGesture;
    // event StylusDownEventHandler StylusDown;
    // event TextCompositionEventHandler PreviewTextInput;
    // event StylusButtonEventHandler StylusButtonDown;
    // event StylusButtonEventHandler StylusButtonUp;
    // event MouseButtonEventHandler PreviewMouseLeftButtonUp;
    // event StylusEventHandler StylusEnter;
    // event StylusEventHandler StylusInAirMove;
    // event StylusEventHandler StylusInRange;
    // event StylusEventHandler StylusLeave;
    // event StylusEventHandler StylusMove;
    // event StylusEventHandler StylusOutOfRange;
    // event StylusSystemGestureEventHandler StylusSystemGesture;
    // event StylusEventHandler PreviewStylusUp;
    // event MouseButtonEventHandler PreviewMouseLeftButtonDown;
    // event KeyEventHandler PreviewKeyUp;
    // event KeyboardFocusChangedEventHandler GotKeyboardFocus;
    // event StylusEventHandler GotStylusCapture;
    // event KeyboardFocusChangedEventHandler PreviewLostKeyboardFocus;
    // event StylusEventHandler LostStylusCapture;
    // event KeyboardFocusChangedEventHandler LostKeyboardFocus;
    // event StylusEventHandler StylusUp;
    // event KeyboardFocusChangedEventHandler PreviewGotKeyboardFocus;
    // event KeyEventHandler PreviewKeyDown;

    // bool CaptureStylus();
    // void ReleaseStylusCapture();
}
