
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
using OpenSilver.Internal.Controls.Primitives;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using EVENTS = System.Windows.Input.InputManager.EVENTS;
using PointerCallbackParameters = System.Windows.Input.InputManager.PointerCallbackParameters;

namespace System.Windows.Input;

/// <summary>
/// Represents a mouse device.
/// </summary>
public abstract class MouseDevice : InputDevice
{
    private const int _doubleClickDeltaTime = 400;
    private const int _doubleClickDeltaX = 5;
    private const int _doubleClickDeltaY = 5;

    private readonly InputManager _inputManager;
    private UIElement _mouseOver;
    private UIElement _mouseCapture;
    private Point _lastClick;
    private MouseButton _lastButton;
    private int _clickCount;
    private int _lastClickTime;
    private Point _clientPosition;
    private Cursor _overrideCursor;

    /// <summary>
    /// Initializes a new instance of the <see cref="MouseDevice"/> class.
    /// </summary>
    /// <param name="inputManager">
    /// The input manager associated with this <see cref="MouseDevice"/>.
    /// </param>
    internal MouseDevice(InputManager inputManager)
    {
        ArgumentNullException.ThrowIfNull(inputManager);
        _inputManager = inputManager;
    }

    /// <summary>
    /// Gets the <see cref="IInputElement"/> that the input from this mouse device is sent to.
    /// </summary>
    /// <returns>
    /// The element that receives the input.
    /// </returns>
    public override IInputElement Target => _mouseOver;

    /// <summary>
    /// Gets the element that the mouse pointer is directly over.
    /// </summary>
    /// <returns>
    /// The element the mouse pointer is over.
    /// </returns>
    public IInputElement DirectlyOver => _mouseOver;

    /// <summary>
    /// Gets the <see cref="IInputElement"/> that is captured by the mouse.
    /// </summary>
    /// <returns>
    /// The element which is captured by the mouse.
    /// </returns>
    public IInputElement Captured => _mouseCapture;

    /// <summary>
    /// Gets the state of the left mouse button of this mouse device.
    /// </summary>
    /// <returns>
    /// The state of the button.
    /// </returns>
    public MouseButtonState LeftButton => GetButtonState(MouseButton.Left);

    /// <summary>
    /// The state of the middle button of this mouse device.
    /// </summary>
    /// <returns>
    /// The state of the button.
    /// </returns>
    public MouseButtonState MiddleButton => GetButtonState(MouseButton.Middle);

    /// <summary>
    /// Gets the state of the right button of this mouse device.
    /// </summary>
    /// <returns>
    /// The state of the button.
    /// </returns>
    public MouseButtonState RightButton => GetButtonState(MouseButton.Right);

    /// <summary>
    /// Gets or sets the cursor for the entire application.
    /// </summary>
    /// <returns>
    /// The override cursor or null if <see cref="OverrideCursor"/> is not set.
    /// </returns>
    public Cursor OverrideCursor
    {
        get => _overrideCursor;
        set
        {
            _overrideCursor = value;
            UpdateCursorPrivate();
        }
    }

    /// <summary>
    /// Captures mouse events to the specified element.
    /// </summary>
    /// <param name="element">
    /// The element to capture the mouse.
    /// </param>
    /// <returns>
    /// true if the element was able to capture the mouse; otherwise, false.
    /// </returns>
    public bool Capture(UIElement element)
    {
        int timestamp = Environment.TickCount;

        if (_mouseCapture == element)
        {
            return true;
        }

        bool success = false;

        if (element is null || _mouseCapture is null)
        {
            success = SetCaptureNative(element);
        }

        if (success)
        {
            ChangeMouseCapture(element, timestamp);
        }

        return success;
    }

    /// <summary>
    /// Captures mouse events to the specified element.
    /// </summary>
    /// <param name="element">
    /// The element to capture the mouse.
    /// </param>
    /// <returns>
    /// true if the element was able to capture the mouse; otherwise, false.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// <paramref name="element"/> is not a <see cref="UIElement"/>.
    /// </exception>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool Capture(IInputElement element)
    {
        UIElement capture = element switch
        {
            UIElement uie => uie,
            null => null,
            _ => throw new InvalidOperationException(string.Format(Strings.Invalid_IInputElement, element.GetType())),
        };

        return Capture(capture);
    }

    /// <summary>
    /// Gets the position of the mouse relative to a specified element.
    /// </summary>
    /// <param name="relativeTo">
    /// The frame of reference in which to calculate the position of the mouse.
    /// </param>
    /// <returns>
    /// The position of the mouse relative to the parameter <paramref name="relativeTo"/>.
    /// </returns>
    public Point GetPosition(UIElement relativeTo) => GetPosition(GetClientPosition(), relativeTo);

    /// <summary>
    /// Gets the position of the mouse relative to a specified element.
    /// </summary>
    /// <param name="relativeTo">
    /// The frame of reference in which to calculate the position of the mouse.
    /// </param>
    /// <returns>
    /// The position of the mouse relative to the parameter <paramref name="relativeTo"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// <paramref name="relativeTo"/> is null or is not a <see cref="UIElement"/>.
    /// </exception>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Point GetPosition(IInputElement relativeTo) => GetPosition(GetClientPosition(), relativeTo);

    internal static Point GetPosition(Point origin, IInputElement relativeTo)
    {
        return relativeTo switch
        {
            UIElement uie => GetPosition(origin, uie),
            null => GetPosition(origin, null),
            _ => throw new InvalidOperationException(string.Format(Strings.Invalid_IInputElement, relativeTo.GetType())),
        };
    }

    internal static Point GetPosition(Point origin, UIElement relativeTo)
    {
        if (relativeTo is Popup popup)
        {
            relativeTo = popup.IsOpen ? popup.Child : null;
        }

        if (relativeTo is null)
        {
            return origin;
        }

        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(relativeTo))
        {
            Matrix m = relativeTo.InternalTransformToAncestor(null);
            if (m.HasInverse)
            {
                m.Invert();
            }

            return m.Transform(origin);
        }

        return new Point(0.0, 0.0);
    }

    /// <summary>
    /// Forces the mouse cursor to update.
    /// </summary>
    public void UpdateCursor() => UpdateCursorPrivate();

    /// <summary>
    /// Sets the mouse pointer to the specified <see cref="Cursor"/>.
    /// </summary>
    /// <param name="cursor">
    /// The cursor to set the mouse pointer to.
    /// </param>
    /// <returns>
    /// true if the mouse cursor is set; otherwise, false.
    /// </returns>
    public bool SetCursor(Cursor cursor) => SetCursor(cursor, true);

    private bool SetCursor(Cursor cursor, bool forceCursor)
    {
        // Override the cursor if one is set.
        if (_overrideCursor is not null)
        {
            cursor = _overrideCursor;
            forceCursor = true;
        }

        cursor ??= Cursors.None;

        return SetCursorNative(cursor, forceCursor);
    }

    /// <summary>
    /// Gets the state of the specified mouse button.
    /// </summary>
    /// <param name="mouseButton">
    /// The button which is being queried.
    /// </param>
    /// <returns>
    /// The state of the button.
    /// </returns>
    protected MouseButtonState GetButtonState(MouseButton mouseButton) => GetButtonStateFromSystem(mouseButton);

    /// <summary>
    /// Calculates the position of the mouse pointer, in client coordinates.
    /// </summary>
    /// <returns>
    /// The position of the mouse pointer, in client coordinates.
    /// </returns>
    protected Point GetClientPosition() => _clientPosition;

    internal abstract bool SetCursorNative(Cursor cursor, bool forceCursor);

    internal abstract bool SetCaptureNative(UIElement capture);

    internal abstract MouseButtonState GetButtonStateFromSystem(MouseButton mouseButton);

    private void ChangeMouseCapture(UIElement mouseCapture, int timestamp)
    {
        if (mouseCapture != _mouseCapture)
        {
            UIElement oldMouseCapture = _mouseCapture;
            _mouseCapture = mouseCapture;

            oldMouseCapture?.SetValueInternal(UIElement.IsMouseCapturedPropertyKey, false);
            _mouseCapture?.SetValueInternal(UIElement.IsMouseCapturedPropertyKey, true);

            using (_inputManager.DisableProcessing())
            {
                if (oldMouseCapture is not null)
                {
                    var lostMouseCapture = new MouseEventArgs(this, timestamp)
                    {
                        RoutedEvent = Mouse.LostMouseCaptureEvent,
                        Source = oldMouseCapture,
                    };

                    _inputManager.PushInput(lostMouseCapture);
                }

                if (_mouseCapture is not null)
                {
                    var gotMouseCapture = new MouseEventArgs(this, timestamp)
                    {
                        RoutedEvent = Mouse.GotMouseCaptureEvent,
                        Source = _mouseCapture,
                    };

                    _inputManager.PushInput(gotMouseCapture);
                }
            }
        }
    }

    private void ChangeMouseOver(UIElement mouseOver)
    {
        if (_mouseOver != mouseOver)
        {
            UIElement oldMouseOver = _mouseOver;
            _mouseOver = mouseOver;

            oldMouseOver?.SetValueInternal(UIElement.IsMouseDirectlyOverPropertyKey, false);
            _mouseOver?.SetValueInternal(UIElement.IsMouseDirectlyOverPropertyKey, true);
        }
    }

    private void UpdateCursorPrivate() => UpdateCursorPrivate(false, Keyboard.Modifiers, _clientPosition.X, _clientPosition.Y);

    private void UpdateCursorPrivate(bool isTouchEvent, ModifierKeys modifiers, double x, double y)
    {
        if (_mouseOver is null)
        {
            SetCursor(Cursors.Arrow, false);
            return;
        }

        var queryCursor = new QueryCursorEventArgs(this, Environment.TickCount, isTouchEvent, modifiers, x, y)
        {
            Cursor = Cursors.Arrow,
            RoutedEvent = Mouse.QueryCursorEvent,
        };

        _mouseOver?.RaiseTrustedEvent(queryCursor);

        SetCursor(queryCursor.Cursor, false);
    }

    internal void ProcessInput(UIElement uie, EVENTS eventType, PointerCallbackParameters parameters)
    {
        _clientPosition = new Point(parameters.PageX, parameters.PageY);

        if (uie is null)
        {
            ProcessUnmappedEvent(eventType);
        }
        else
        {
            DispatchEvent(uie, eventType, parameters);
        }
    }

    private void DispatchEvent(UIElement uie, EVENTS eventType, PointerCallbackParameters parameters)
    {
        switch (eventType)
        {
            case EVENTS.POINTER_MOVE:
                ProcessOnMouseMove(uie, parameters);
                break;

            case EVENTS.POINTER_LEFT_DOWN:
                ProcessOnMouseLeftButtonDown(uie, parameters);
                break;

            case EVENTS.POINTER_LEFT_UP:
                ProcessOnMouseLeftButtonUp(uie, parameters);
                break;

            case EVENTS.POINTER_RIGHT_DOWN:
                ProcessOnMouseRightButtonDown(uie, parameters);
                break;

            case EVENTS.POINTER_RIGHT_UP:
                ProcessOnMouseRightButtonUp(uie, parameters);
                break;

            case EVENTS.POINTER_MIDDLE_DOWN:
                ProcessOnMouseMiddleButtonDown(uie, parameters);
                break;

            case EVENTS.POINTER_MIDDLE_UP:
                ProcessOnMouseMiddleButtonUp(uie, parameters);
                break;

            case EVENTS.POINTER_ENTER:
                ProcessOnMouseEnter(uie, parameters);
                break;

            case EVENTS.POINTER_LEAVE:
                ProcessOnMouseLeave(uie, parameters);
                break;

            case EVENTS.POINTER_OVER:
                ProcessOnPointerOver(uie, parameters);
                break;

            case EVENTS.WHEEL:
                ProcessOnWheel(uie, parameters);
                break;
        }
    }

    private void ProcessUnmappedEvent(EVENTS eventType)
    {
        switch (eventType)
        {
            case EVENTS.POINTER_LEFT_DOWN:
                RefreshClickCount(MouseButton.Left, Environment.TickCount, new Point());
                PopupService.HandleMouseButton();
                break;

            case EVENTS.POINTER_RIGHT_DOWN:
                RefreshClickCount(MouseButton.Right, Environment.TickCount, new Point());
                PopupService.HandleMouseButton();
                break;

            case EVENTS.POINTER_MIDDLE_DOWN:
                RefreshClickCount(MouseButton.Middle, Environment.TickCount, new Point());
                PopupService.HandleMouseButton();
                break;

            case EVENTS.POINTER_OVER:
                ProcessOnPointerOut();
                break;

            case EVENTS.POINTER_CAPTURE_LOST:
                Capture(null);
                break;
        }
    }

    private void ProcessOnMouseMove(UIElement uie, PointerCallbackParameters parameters)
    {
        UpdateCursorPrivate(parameters.IsTouchEvent, parameters.KeyModifiers, parameters.PageX, parameters.PageY);

        int timestamp = Environment.TickCount;

        var previewMove = new MouseEventArgs(this, timestamp, parameters.IsTouchEvent, parameters.KeyModifiers, parameters.PageX, parameters.PageY)
        {
            RoutedEvent = Mouse.PreviewMouseMoveEvent,
            Source = uie,
            UIEventArg = parameters.UIEventArg,
        };

        uie.RaiseTrustedEvent(previewMove);

        if (previewMove.Handled)
        {
            return;
        }

        var move = new MouseEventArgs(this, timestamp, parameters.IsTouchEvent, parameters.KeyModifiers, parameters.PageX, parameters.PageY)
        {
            RoutedEvent = Mouse.MouseMoveEvent,
            Source = uie,
            UIEventArg = parameters.UIEventArg,
        };

        uie.RaiseTrustedEvent(move);
    }

    private void ProcessOnMouseLeftButtonDown(UIElement uie, PointerCallbackParameters parameters)
    {
        ProcessMouseDownEvent(
            uie,
            parameters,
            MouseButton.Left,
            refreshClickCount: true,
            closeToolTips: true);

        UpdateCursorPrivate(parameters.IsTouchEvent, parameters.KeyModifiers, parameters.PageX, parameters.PageY);
    }

    private void ProcessOnMouseLeftButtonUp(UIElement uie, PointerCallbackParameters parameters)
    {
        ProcessMouseUpEvent(uie, parameters, MouseButton.Left);
        ProcessOnTapped(uie, parameters);
        UpdateCursorPrivate(parameters.IsTouchEvent, parameters.KeyModifiers, parameters.PageX, parameters.PageY);
    }

    private void ProcessOnMouseRightButtonDown(UIElement uie, PointerCallbackParameters parameters)
    {
        bool handled = ProcessMouseDownEvent(
            uie,
            parameters,
            MouseButton.Right,
            refreshClickCount: true,
            closeToolTips: true);

        if (handled)
        {
            OpenSilver.Interop.ExecuteJavaScriptVoid("osjs.inputManager.suppressContextMenu(true)");
        }

        UpdateCursorPrivate(parameters.IsTouchEvent, parameters.KeyModifiers, parameters.PageX, parameters.PageY);
    }

    private void ProcessOnMouseRightButtonUp(UIElement uie, PointerCallbackParameters parameters)
    {
        ProcessMouseUpEvent(uie, parameters, MouseButton.Right);
        UpdateCursorPrivate(parameters.IsTouchEvent, parameters.KeyModifiers, parameters.PageX, parameters.PageY);
    }

    private void ProcessOnMouseMiddleButtonDown(UIElement uie, PointerCallbackParameters parameters)
    {
        ProcessMouseDownEvent(
            uie,
            parameters,
            MouseButton.Middle,
            refreshClickCount: true,
            closeToolTips: false);

        UpdateCursorPrivate(parameters.IsTouchEvent, parameters.KeyModifiers, parameters.PageX, parameters.PageY);
    }

    private void ProcessOnMouseMiddleButtonUp(UIElement uie, PointerCallbackParameters parameters)
    {
        ProcessMouseUpEvent(uie, parameters, MouseButton.Middle);
        UpdateCursorPrivate(parameters.IsTouchEvent, parameters.KeyModifiers, parameters.PageX, parameters.PageY);
    }

    private void ProcessOnWheel(UIElement uie, PointerCallbackParameters parameters)
    {
        int timestamp = Environment.TickCount;

        int sign = OpenSilver.Interop.ExecuteJavaScriptDouble(
            $"{OpenSilver.Interop.GetVariableStringForJS(parameters.UIEventArg)}.deltaY", false) > 0 ? -1 : 1;
        int delta = sign * Mouse.MouseWheelDeltaForOneLine;

        var previewWheel = new MouseWheelEventArgs(this,
            timestamp,
            delta,
            parameters.IsTouchEvent,
            parameters.KeyModifiers,
            parameters.PageX,
            parameters.PageY)
        {
            RoutedEvent = Mouse.PreviewMouseWheelEvent,
            Source = uie,
            UIEventArg = parameters.UIEventArg,
        };

        uie.RaiseTrustedEvent(previewWheel);

        if (previewWheel.Handled)
        {
            previewWheel.PreventDefault();
            return;
        }

        var wheel = new MouseWheelEventArgs(this,
            timestamp,
            delta,
            parameters.IsTouchEvent,
            parameters.KeyModifiers,
            parameters.PageX,
            parameters.PageY)
        {
            RoutedEvent = Mouse.MouseWheelEvent,
            Source = uie,
            UIEventArg = parameters.UIEventArg,
        };

        uie.RaiseTrustedEvent(wheel);

        if (wheel.Handled)
        {
            wheel.PreventDefault();
        }
    }

    private void ProcessOnMouseEnter(UIElement uie, PointerCallbackParameters parameters)
    {
        int timestamp = Environment.TickCount;

        uie.SetValueInternal(UIElement.IsMouseOverPropertyKey, true);

        var mouseEnter = new MouseEventArgs(this,
            timestamp,
            parameters.IsTouchEvent,
            parameters.KeyModifiers,
            parameters.PageX,
            parameters.PageY)
        {
            RoutedEvent = Mouse.MouseEnterEvent,
            Source = uie,
            UIEventArg = parameters.UIEventArg,
        };

        uie.RaiseTrustedEvent(mouseEnter);
    }

    private void ProcessOnMouseLeave(UIElement uie, PointerCallbackParameters parameters)
    {
        int timestamp = Environment.TickCount;

        uie.ClearValue(UIElement.IsMouseOverPropertyKey);

        var mouseLeave = new MouseEventArgs(this,
            timestamp,
            parameters.IsTouchEvent,
            parameters.KeyModifiers,
            parameters.PageX,
            parameters.PageY)
        {
            RoutedEvent = Mouse.MouseLeaveEvent,
            Source = uie,
            UIEventArg = parameters.UIEventArg,
        };

        uie.RaiseTrustedEvent(mouseLeave);
    }

    private void ProcessOnPointerOver(UIElement uie, PointerCallbackParameters parameters) => ChangeMouseOver(uie);

    private void ProcessOnPointerOut() => ChangeMouseOver(null);

    private bool ProcessMouseDownEvent(
        UIElement uie,
        PointerCallbackParameters parameters,
        MouseButton button,
        bool refreshClickCount,
        bool closeToolTips)
    {
        int timestamp = Environment.TickCount;

        if (closeToolTips)
        {
            ToolTipService.OnMouseButtonDown();
        }

        var previewMouseDown = new MouseButtonEventArgs(this,
            timestamp,
            button,
            MouseButtonState.Pressed,
            parameters.IsTouchEvent,
            parameters.KeyModifiers,
            parameters.PageX,
            parameters.PageY)
        {
            RoutedEvent = Mouse.PreviewMouseDownEvent,
            Source = uie,
            UIEventArg = parameters.UIEventArg,
        };

        if (refreshClickCount)
        {
            previewMouseDown.ClickCount = RefreshClickCount(button, Environment.TickCount, new Point(parameters.PageX, parameters.PageY));
        }

        uie.RaiseTrustedEvent(previewMouseDown);

        if (previewMouseDown.Handled)
        {
            return true;
        }

        var mouseDown = new MouseButtonEventArgs(this,
            timestamp,
            button,
            MouseButtonState.Pressed,
            parameters.IsTouchEvent,
            parameters.KeyModifiers,
            parameters.PageX,
            parameters.PageY)
        {
            RoutedEvent = Mouse.MouseDownEvent,
            Source = uie,
            UIEventArg = parameters.UIEventArg,
            ClickCount = previewMouseDown.ClickCount,
        };

        uie.RaiseTrustedEvent(mouseDown);

        return mouseDown.Handled;
    }

    private void ProcessMouseUpEvent(UIElement uie, PointerCallbackParameters parameters, MouseButton button)
    {
        int timestamp = Environment.TickCount;

        var previewMouseUp = new MouseButtonEventArgs(this,
            timestamp,
            button,
            MouseButtonState.Released,
            parameters.IsTouchEvent,
            parameters.KeyModifiers,
            parameters.PageX,
            parameters.PageY)
        {
            RoutedEvent = Mouse.PreviewMouseUpEvent,
            Source = uie,
            UIEventArg = parameters.UIEventArg,
        };

        uie.RaiseTrustedEvent(previewMouseUp);

        if (previewMouseUp.Handled)
        {
            return;
        }

        var mouseUp = new MouseButtonEventArgs(this,
            timestamp,
            button,
            MouseButtonState.Released,
            parameters.IsTouchEvent,
            parameters.KeyModifiers,
            parameters.PageX,
            parameters.PageY)
        {
            RoutedEvent = Mouse.MouseUpEvent,
            Source = uie,
            UIEventArg = parameters.UIEventArg,
        };

        uie.RaiseTrustedEvent(mouseUp);

        CommandManager.InvalidateRequerySuggested();
    }

    private void ProcessOnTapped(UIElement uie, PointerCallbackParameters parameters)
    {
        var e = new TappedRoutedEventArgs(this,
            Environment.TickCount,
            parameters.IsTouchEvent,
            parameters.KeyModifiers,
            parameters.PageX,
            parameters.PageY)
        {
            RoutedEvent = UIElement.TappedEvent,
            Source = uie,
            UIEventArg = parameters.UIEventArg,
        };

        uie.RaiseTrustedEvent(e);
    }

    private int RefreshClickCount(MouseButton button, int timeStamp, Point ptClient)
    {
        _clickCount = CalculateClickCount(button, timeStamp, ptClient);

        if (_clickCount == 1)
        {
            // we need to reset out data, since this is the start of the click count process...
            _lastButton = button;
        }

        _lastClick = ptClient;
        _lastClickTime = timeStamp;

        return _clickCount;
    }

    private int CalculateClickCount(MouseButton button, int timeStamp, Point downPt)
    {
        if (timeStamp - _lastClickTime < _doubleClickDeltaTime // How long since the last click?
              && _lastButton == button // Is this the same mouse button as the last click?
              && IsSameSpot(downPt)) // Is the delta coordinates of this click close enough to the last click?
        {
            return _clickCount + 1;
        }
        else
        {
            return 1;
        }
    }

    private bool IsSameSpot(Point newPosition)
    {
        // Is the delta coordinates of this click close enough to the last click?
        return (Math.Abs(newPosition.X - _lastClick.X) < _doubleClickDeltaX) &&
               (Math.Abs(newPosition.Y - _lastClick.Y) < _doubleClickDeltaY);
    }
}
