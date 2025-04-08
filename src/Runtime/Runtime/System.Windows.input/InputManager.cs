
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

using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using CSHTML5.Internal;

namespace System.Windows.Input;

internal sealed class InputManager
{
    // This must remain synchronyzed with the EVENTS enum defined in cshtml5.js.
    // Make sure to change both files if you update this !
    private enum EVENTS
    {
        POINTER_MOVE = 0,
        POINTER_LEFT_DOWN = 1,
        POINTER_LEFT_UP = 2,
        POINTER_RIGHT_DOWN = 3,
        POINTER_RIGHT_UP = 4,
        POINTER_MIDDLE_DOWN = 5,
        POINTER_MIDDLE_UP = 6,
        POINTER_ENTER = 7,
        POINTER_LEAVE = 8,
        WHEEL = 9,
        KEYDOWN = 10,
        KEYUP = 11,
        KEYPRESS = 12,
        FOCUS_MANAGED = 13,
        FOCUS_UNMANAGED = 14,
        WINDOW_FOCUS = 15,
        WINDOW_BLUR = 16,
    }

    private struct PointerCallbackParameters
    {
        public bool IsTouchEvent;
        public double PageX;
        public double PageY;
        public ModifierKeys KeyModifiers;
        public object UIEventArg;
    }

    private sealed class FocusQueue
    {
        private List<FocusRequest> _queue = new();

        public bool IsEmpty => _queue.Count == 0;

        public void AddRequest(FocusRequest request) => _queue.Add(request);

        public FocusRequest PeekLast()
        {
            if (!TryPeekLast(out FocusRequest request))
            {
                throw new InvalidOperationException("Queue is empty.");
            }

            return request;
        }

        public bool TryPeekLast(out FocusRequest request)
        {
            if (IsEmpty)
            {
                request = default;
                return false;
            }

            request = _queue[_queue.Count - 1];
            return true;
        }

        public void ProcessQueue()
        {
            foreach (FocusRequest r in Interlocked.Exchange(ref _queue, new()))
            {
                RoutedEvent routedEvent = r.Type switch
                {
                    FocusRequestType.LostFocus => UIElement.LostFocusEvent,
                    FocusRequestType.GotFocus => UIElement.GotFocusEvent,
                    _ => null,
                };

                if (routedEvent is null) continue;

                UIElement target = r.Target;
                RaiseUserInitiatedEvent(target, new RoutedEventArgs(routedEvent, target));
            }
        }
    }

    private readonly struct FocusRequest
    {
        public FocusRequest(UIElement target, FocusRequestType type)
        {
            Debug.Assert(target is not null);
            Target = target;
            Type = type;
        }

        public UIElement Target { get; }

        public FocusRequestType Type { get; }
    }

    private enum FocusRequestType { GotFocus, LostFocus }

    private readonly JavaScriptCallback _handler;
    private readonly JavaScriptCallback _pointerHandler;
    private readonly FocusQueue _focusQueue = new();

    private const int _doubleClickDeltaTime = 400;
    private const int _doubleClickDeltaX = 5;
    private const int _doubleClickDeltaY = 5;
    private Point _lastClick = new Point();
    private MouseButton _lastButton;
    private int _clickCount;
    private int _lastClickTime;
    private bool _mouseLeftDown;

    private InputManager()
    {
        if (Current is null)
        {
            _handler = JavaScriptCallback.Create(ProcessInput);
            _pointerHandler = JavaScriptCallback.Create(ProcessPointerInput);
            string sHandler = OpenSilver.Interop.GetVariableStringForJS(_handler);
            string sPointerHandler = OpenSilver.Interop.GetVariableStringForJS(_pointerHandler);
            OpenSilver.Interop.ExecuteJavaScriptVoid($"document.createInputManager({sHandler}, {sPointerHandler})");
        }
    }

    /// <summary>
    /// Return the input manager associated with the current context.
    /// </summary>
    public static InputManager Current { get; } = new InputManager();

    internal void RegisterRoot(INTERNAL_HtmlDomElementReference element)
    {
        string sElement = OpenSilver.Interop.GetVariableStringForJS(element);
        OpenSilver.Interop.ExecuteJavaScriptVoid($"document.inputManager.registerRoot({sElement})");
    }

    internal ModifierKeys GetKeyboardModifiers()
    {
        return (ModifierKeys)OpenSilver.Interop.ExecuteJavaScriptInt32("document.inputManager.getModifiers()", false);
    }

    internal bool CaptureMouse(UIElement uie)
    {
        Debug.Assert(uie is not null);

        if (Pointer.Captured is null && _mouseLeftDown)
        {
            Pointer.Captured = uie;

            string sDiv = OpenSilver.Interop.GetVariableStringForJS(uie.OuterDiv);
            OpenSilver.Interop.ExecuteJavaScriptVoid($"document.inputManager.capturePointer({sDiv})");

            RaiseUserInitiatedEvent(uie, new MouseEventArgs
            {
                RoutedEvent = Mouse.GotMouseCaptureEvent,
                Source = uie,
            });

            return true;
        }

        return Pointer.Captured == uie;
    }

    internal void ReleaseMouseCapture(UIElement uie)
    {
        if (Pointer.Captured == uie)
        {
            Pointer.Captured = null;
            OpenSilver.Interop.ExecuteJavaScriptVoid($"document.inputManager.releasePointerCapture()");

            RaiseUserInitiatedEvent(uie, new MouseEventArgs
            {
                RoutedEvent = Mouse.LostMouseCaptureEvent,
                Source = uie,
            });
        }
    }

    internal bool SetFocus(UIElement uie)
    {
        DependencyObject focusScope = FocusManager.GetFocusScope(uie);
        UIElement focused = (UIElement)FocusManager.GetFocusedElement(focusScope);
        if (focused == uie)
        {
            return true;
        }

        if (uie.GetFocusTarget() is INTERNAL_HtmlDomElementReference target)
        {
            if (SetFocusNative(target))
            {
                KeyboardNavigation.UpdateFocusedElement(uie, focusScope);

                if (focused is not null)
                {
                    _focusQueue.AddRequest(new FocusRequest(focused, FocusRequestType.LostFocus));
                }
                _focusQueue.AddRequest(new FocusRequest(uie, FocusRequestType.GotFocus));
                return true;

            }
            ClearTabIndex(uie);
        }

        return false;
    }

    internal static bool SetFocusNative(INTERNAL_HtmlDomElementReference domElement)
    {
        string sDiv = OpenSilver.Interop.GetVariableStringForJS(domElement);
        return OpenSilver.Interop.ExecuteJavaScriptBoolean($"document.inputManager.focus({sDiv})");
    }

    internal static void ClearTabIndex(UIElement uie)
    {
        if (uie.GetFocusTarget() is INTERNAL_HtmlDomElementReference domElement)
        {
            switch (uie)
            {
                case TextBox or PasswordBox:
                    INTERNAL_HtmlDomManager.SetDomElementAttribute(domElement, "tabindex", "-1");
                    break;

                default:
                    INTERNAL_HtmlDomManager.RemoveAttribute(domElement, "tabindex");
                    break;
            }
        }
    }

    internal void OnElementRemoved(UIElement uie)
    {
        RaiseMouseLeave(uie);
        ResetFocus(uie);
        ReleaseMouseCapture(uie);

        static void RaiseMouseLeave(UIElement uie)
        {
            if (uie.IsMouseOver)
            {
                uie.ClearValue(UIElement.IsMouseOverPropertyKey);

                RaiseUserInitiatedEvent(uie, new MouseEventArgs
                {
                    RoutedEvent = Mouse.MouseLeaveEvent,
                    Source = uie,
                });
            }
        }

        void ResetFocus(UIElement uie)
        {
            DependencyObject focusScope = FocusManager.GetFocusScope(uie);
            UIElement focused = (UIElement)FocusManager.GetFocusedElement(focusScope);
            if (focused == uie)
            {
                KeyboardNavigation.UpdateFocusedElement(null, focusScope);

                bool processQueue = _focusQueue.IsEmpty;
                _focusQueue.AddRequest(new FocusRequest(focused, FocusRequestType.LostFocus));

                if (processQueue)
                {
                    _focusQueue.ProcessQueue();
                }
            }
        }

        static void ReleaseMouseCapture(UIElement uie)
        {
            // We make sure an element that is detached cannot have the cursor 
            // captured, which causes bugs.
            // For example in a DataGrid, if we had a column with two focusable 
            // elements in its edition mode, clicking one then the other one 
            // would leave the edition mode and detach the elements but the second 
            // element that was clicked would still have captured the pointer 
            // events, preventing the user to click on anything until the capture 
            // is released (if it does ever happen).
            if (Pointer.Captured == uie)
            {
                uie.ReleaseMouseCapture();
            }
        }
    }

    private void ProcessInput(string id, int eventId, object jsEventArg)
    {
        if (INTERNAL_HtmlDomManager.GetElementById(id) is not UIElement uie)
        {
            ProcessUnmappedEvent((EVENTS)eventId, jsEventArg);
        }
        else
        {
            DispatchEvent(uie, (EVENTS)eventId, jsEventArg);
        }
    }

    private void ProcessPointerInput(string id, int eventId, object jsEventArg, bool isTouchEvent, double pageX, double pageY, int keyModifiers)
    {
        if (INTERNAL_HtmlDomManager.GetElementById(id) is not UIElement uie)
        {
            ProcessUnmappedEvent((EVENTS)eventId, jsEventArg);
        }
        else
        {
            DispatchEventPointerEvent(
                uie,
                (EVENTS)eventId,
                new PointerCallbackParameters
                {
                    IsTouchEvent = isTouchEvent,
                    PageX = pageX,
                    PageY = pageY,
                    KeyModifiers = (ModifierKeys)keyModifiers,
                    UIEventArg = jsEventArg,
                });
        }
    }

    private void DispatchEvent(UIElement uie, EVENTS eventType, object jsEventArg)
    {
        switch (eventType)
        {
            case EVENTS.KEYDOWN:
                ProcessOnKeyDown(uie, jsEventArg);
                break;

            case EVENTS.KEYUP:
                ProcessOnKeyUp(uie, jsEventArg);
                break;

            case EVENTS.KEYPRESS:
                ProcessOnKeyPress(uie, jsEventArg);
                break;

            case EVENTS.FOCUS_UNMANAGED:
                ProcessOnFocusUnmanaged(uie, jsEventArg);
                break;
        }
    }

    private void DispatchEventPointerEvent(UIElement uie, EVENTS eventType, PointerCallbackParameters parameters)
    {
        switch (eventType)
        {
            case EVENTS.POINTER_MOVE:
                ProcessOnMouseMove(uie, parameters);
                break;

            case EVENTS.POINTER_LEFT_DOWN:
                _mouseLeftDown = true;
                ProcessOnMouseLeftButtonDown(uie, parameters);
                break;

            case EVENTS.POINTER_LEFT_UP:
                _mouseLeftDown = false;
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

            case EVENTS.WHEEL:
                ProcessOnWheel(uie, parameters);
                break;
        }
    }

    private void ProcessUnmappedEvent(EVENTS eventType, object jsEventArg)
    {
        switch (eventType)
        {
            case EVENTS.POINTER_LEFT_DOWN:
                _mouseLeftDown = true;
                RefreshClickCount(MouseButton.Left, Environment.TickCount, new Point());
                break;

            case EVENTS.POINTER_RIGHT_DOWN:
                RefreshClickCount(MouseButton.Right, Environment.TickCount, new Point());
                break;

            case EVENTS.POINTER_MIDDLE_DOWN:
                RefreshClickCount(MouseButton.Middle, Environment.TickCount, new Point());
                break;

            case EVENTS.POINTER_LEFT_UP:
                _mouseLeftDown = false;
                ReleaseMouseCapture();
                break;

            case EVENTS.FOCUS_MANAGED:
                OnFocusManaged();
                break;

            case EVENTS.FOCUS_UNMANAGED:
                OnFocusUnmanaged();
                break;

            case EVENTS.WINDOW_FOCUS:
                OnWindowFocus(jsEventArg);
                break;

            case EVENTS.WINDOW_BLUR:
                OnWindowBlur(jsEventArg);
                break;
        }
    }

    private void ReleaseMouseCapture()
    {
        if (Pointer.Captured is UIElement uie)
        {
            ReleaseMouseCapture(uie);
        }
    }

    private void OnFocusManaged()
    {
        _focusQueue.ProcessQueue();
    }

    private void OnFocusUnmanaged()
    {
        if (FocusManager.GetFocusedElement() is UIElement focusedElement)
        {
            // Focus moved back to the application (most likely to the opensilver-root div).
            // Reposition focus to the element that has logical focus.
            SetFocus(focusedElement);
        }
        else
        {
            if (Window.Current?.Content is DependencyObject rootVisual)
            {
                KeyboardNavigation.Current.Navigate(
                    rootVisual,
                    new TraversalRequest(
                        ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift) ?
                        FocusNavigationDirection.Last :
                        FocusNavigationDirection.First));
            }
        }
    }

    private void OnWindowFocus(object jsEventArg)
    {
        // The window received focus, re-focus element with logical focus if any.
        if (FocusManager.GetFocusedElement() is UIElement focusedElement)
        {
            RaiseUserInitiatedEvent(focusedElement, new RoutedEventArgs(UIElement.GotFocusEvent, focusedElement)
            {
                UIEventArg = jsEventArg,
            });
        }
    }

    private void OnWindowBlur(object jsEventArg)
    {
        if (FocusManager.GetFocusedElement() is UIElement focusedElement)
        {
            RaiseUserInitiatedEvent(focusedElement, new RoutedEventArgs(UIElement.LostFocusEvent, focusedElement)
            {
                UIEventArg = jsEventArg,
            });
        }
    }

    private void ProcessOnMouseMove(UIElement uie, PointerCallbackParameters parameters)
    {
        if (uie.MouseTarget is UIElement mouseTarget)
        {
            ProcessPointerEvent(mouseTarget, Mouse.MouseMoveEvent, parameters);
        }
    }

    private void ProcessOnMouseLeftButtonDown(UIElement uie, PointerCallbackParameters parameters)
    {
        if (uie.MouseTarget is UIElement mouseTarget)
        {
            ProcessMouseButtonEvent(
                mouseTarget,
                Mouse.MouseDownEvent,
                parameters,
                MouseButton.Left,
                Environment.TickCount,
                refreshClickCount: true,
                closeToolTips: true);
        }
    }

    private void ProcessOnMouseLeftButtonUp(UIElement uie, PointerCallbackParameters parameters)
    {
        if (uie.MouseTarget is UIElement mouseTarget)
        {
            ProcessMouseButtonEvent(
                mouseTarget,
                Mouse.MouseUpEvent,
                parameters,
                MouseButton.Left,
                Environment.TickCount,
                refreshClickCount: false,
                closeToolTips: false);

            ProcessOnTapped(mouseTarget, parameters);
        }

        ReleaseMouseCapture();
    }

    private void ProcessOnMouseRightButtonDown(UIElement uie, PointerCallbackParameters parameters)
    {
        if (uie.MouseTarget is UIElement mouseTarget)
        {
            bool handled = ProcessMouseButtonEvent(
                mouseTarget,
                Mouse.MouseDownEvent,
                parameters,
                MouseButton.Right,
                Environment.TickCount,
                refreshClickCount: true,
                closeToolTips: true);

            if (handled)
            {
                OpenSilver.Interop.ExecuteJavaScriptVoid("document.inputManager.suppressContextMenu(true)");
            }
        }
    }

    private void ProcessOnMouseRightButtonUp(UIElement uie, PointerCallbackParameters parameters)
    {
        if (uie.MouseTarget is UIElement mouseTarget)
        {
            var e = new MouseButtonEventArgs(MouseButton.Right, parameters.IsTouchEvent, parameters.KeyModifiers, parameters.PageX, parameters.PageY)
            {
                RoutedEvent = Mouse.MouseUpEvent,
                Source = mouseTarget,
                UIEventArg = parameters.UIEventArg,
            };

            RaiseUserInitiatedEvent(mouseTarget, e);
        }

        ReleaseMouseCapture();
    }

    private void ProcessOnMouseMiddleButtonDown(UIElement uie, PointerCallbackParameters parameters)
    {
        if (uie.MouseTarget is UIElement mouseTarget)
        {
            ProcessMouseButtonEvent(
                mouseTarget,
                Mouse.MouseDownEvent,
                parameters,
                MouseButton.Middle,
                Environment.TickCount,
                refreshClickCount: true,
                closeToolTips: false);
        }
    }

    private void ProcessOnMouseMiddleButtonUp(UIElement uie, PointerCallbackParameters parameters)
    {
        if (uie.MouseTarget is UIElement mouseTarget)
        {
            ProcessMouseButtonEvent(
                mouseTarget,
                Mouse.MouseUpEvent,
                parameters,
                MouseButton.Middle,
                Environment.TickCount,
                refreshClickCount: false,
                closeToolTips: false);
        }
    }

    private void ProcessOnWheel(UIElement uie, PointerCallbackParameters parameters)
    {
        if (uie.MouseTarget is UIElement mouseTarget)
        {
            int delta = OpenSilver.Interop.ExecuteJavaScriptDouble(
                $"{OpenSilver.Interop.GetVariableStringForJS(parameters.UIEventArg)}.deltaY", false) > 0 ? -120 : 120;

            var e = new MouseWheelEventArgs(parameters.IsTouchEvent, parameters.KeyModifiers, parameters.PageX, parameters.PageY, delta)
            {
                RoutedEvent = Mouse.MouseWheelEvent,
                Source = mouseTarget,
                UIEventArg = parameters.UIEventArg,
            };

            RaiseUserInitiatedEvent(mouseTarget, e);

            if (e.Handled)
            {
                e.PreventDefault();
            }
        }
    }

    private void ProcessOnMouseEnter(UIElement uie, PointerCallbackParameters parameters)
    {
        if (uie.MouseTarget is UIElement mouseTarget)
        {
            mouseTarget.SetValueInternal(UIElement.IsMouseOverPropertyKey, true);

            ProcessPointerEvent(mouseTarget, Mouse.MouseEnterEvent, parameters);
        }
    }

    private void ProcessOnMouseLeave(UIElement uie, PointerCallbackParameters parameters)
    {
        if (uie.MouseTarget is UIElement mouseTarget)
        {
            mouseTarget.ClearValue(UIElement.IsMouseOverPropertyKey);

            ProcessPointerEvent(mouseTarget, Mouse.MouseLeaveEvent, parameters);
        }
    }

    private void ProcessOnKeyDown(UIElement uie, object jsEventArg)
    {
        if (uie.KeyboardTarget is not UIElement keyboardTarget)
        {
            return;
        }

        uint nativeKeyCode = OpenSilver.Interop.ExecuteJavaScriptUInt32(
            $"{OpenSilver.Interop.GetVariableStringForJS(jsEventArg)}.keyCode", false);

        if (nativeKeyCode > int.MaxValue)
        {
            return;
        }

        int keyCode = VirtualKeysHelpers.FixKeyCodeForSilverlight((int)nativeKeyCode);

        var e = new KeyEventArgs()
        {
            RoutedEvent = UIElement.KeyDownEvent,
            Source = keyboardTarget,
            UIEventArg = jsEventArg,
            PlatformKeyCode = keyCode,
            Key = VirtualKeysHelpers.GetKeyFromKeyCode(keyCode),
            KeyModifiers = Keyboard.Modifiers,
        };

        ToolTipService.OnKeyDown(e);

        RaiseUserInitiatedEvent(keyboardTarget, e);

        KeyboardNavigation.Current.ProcessInput(e);

        if (e.Handled)
        {
            e.PreventDefault();
        }
    }

    private void ProcessOnKeyUp(UIElement uie, object jsEventArg)
    {
        if (uie.KeyboardTarget is not UIElement keyboardTarget)
        {
            return;
        }

        uint nativeKeyCode = OpenSilver.Interop.ExecuteJavaScriptUInt32(
            $"{OpenSilver.Interop.GetVariableStringForJS(jsEventArg)}.keyCode", false);

        if (nativeKeyCode > int.MaxValue)
        {
            return;
        }

        int keyCode = VirtualKeysHelpers.FixKeyCodeForSilverlight((int)nativeKeyCode);

        var e = new KeyEventArgs()
        {
            RoutedEvent = UIElement.KeyUpEvent,
            Source = keyboardTarget,
            UIEventArg = jsEventArg,
            PlatformKeyCode = keyCode,
            Key = VirtualKeysHelpers.GetKeyFromKeyCode(keyCode),
            KeyModifiers = Keyboard.Modifiers,
        };

        RaiseUserInitiatedEvent(keyboardTarget, e);
    }

    private void ProcessOnFocusUnmanaged(UIElement uie, object jsEventArg)
    {
        DependencyObject focusScope = FocusManager.GetFocusScope(uie);
        UIElement oldFocus = (UIElement)FocusManager.GetFocusedElement(focusScope);
        UIElement newFocus = FindLogicalFocus(uie.KeyboardTarget);

        if (newFocus == oldFocus)
        {
            return;
        }

        KeyboardNavigation.UpdateFocusedElement(newFocus, focusScope);

        bool processQueue = _focusQueue.IsEmpty;

        if (oldFocus is not null)
        {
            _focusQueue.AddRequest(new FocusRequest(oldFocus, FocusRequestType.LostFocus));
        }

        if (newFocus is not null)
        {
            _focusQueue.AddRequest(new FocusRequest(newFocus, FocusRequestType.GotFocus));
        }

        if (processQueue)
        {
            _focusQueue.ProcessQueue();
        }

        static UIElement FindLogicalFocus(UIElement uie)
        {
            while (uie is not null && !KeyboardNavigation.Current.IsTabStop(uie))
            {
                uie = (UIElement)VisualTreeHelper.GetParent(uie);
            }

            return uie;
        }
    }

    private void ProcessOnKeyPress(UIElement uie, object jsEventArg)
    {
        if (uie.KeyboardTarget is not UIElement keyboardTarget)
        {
            return;
        }

        uint nativeKeyCode = OpenSilver.Interop.ExecuteJavaScriptUInt32(
            $"{OpenSilver.Interop.GetVariableStringForJS(jsEventArg)}.keyCode", false);

        if (nativeKeyCode > ushort.MaxValue)
        {
            return;
        }

        string text = ((char)nativeKeyCode).ToString();

        var textInputStartArgs = new TextCompositionEventArgs
        {
            RoutedEvent = UIElement.TextInputStartEvent,
            Source = keyboardTarget,
            Text = text,
            TextComposition = TextComposition.Empty,
            UIEventArg = jsEventArg,
        };

        RaiseUserInitiatedEvent(keyboardTarget, textInputStartArgs);

        var textInputArgs = new TextCompositionEventArgs
        {
            RoutedEvent = UIElement.TextInputEvent,
            Source = keyboardTarget,
            Text = text,
            TextComposition = TextComposition.Empty,
            UIEventArg = jsEventArg,
        };

        RaiseUserInitiatedEvent(keyboardTarget, textInputArgs);

        if (textInputArgs.Cancel)
        {
            textInputArgs.PreventDefault();
        }
    }

    private void ProcessPointerEvent(UIElement uie, RoutedEvent routedEvent, PointerCallbackParameters parameters)
    {
        var e = new MouseEventArgs(parameters.IsTouchEvent, parameters.KeyModifiers, parameters.PageX, parameters.PageY)
        {
            RoutedEvent = routedEvent,
            Source = uie,
            UIEventArg = parameters.UIEventArg,
        };

        RaiseUserInitiatedEvent(uie, e);
    }

    private bool ProcessMouseButtonEvent(
        UIElement uie,
        RoutedEvent routedEvent,
        PointerCallbackParameters parameters,
        MouseButton button,
        int timeStamp,
        bool refreshClickCount,
        bool closeToolTips)
    {
        var e = new MouseButtonEventArgs(button, parameters.IsTouchEvent, parameters.KeyModifiers, parameters.PageX, parameters.PageY)
        {
            RoutedEvent = routedEvent,
            Source = uie,
            UIEventArg = parameters.UIEventArg,
        };

        if (refreshClickCount)
        {
            e.ClickCount = RefreshClickCount(button, timeStamp, e.GetPosition(null));
        }

        if (closeToolTips)
        {
            ToolTipService.OnMouseButtonDown(e);
        }

        RaiseUserInitiatedEvent(uie, e);

        return e.Handled;
    }

    private void ProcessOnTapped(UIElement uie, PointerCallbackParameters parameters)
    {
        var e = new TappedRoutedEventArgs(parameters.IsTouchEvent, parameters.KeyModifiers, parameters.PageX, parameters.PageY)
        {
            RoutedEvent = UIElement.TappedEvent,
            Source = uie,
            UIEventArg = parameters.UIEventArg,
        };

        RaiseUserInitiatedEvent(uie, e);
    }

    private static void RaiseUserInitiatedEvent(UIElement uie, RoutedEventArgs e)
    {
        e.MarkAsUserInitiated();

        try
        {
            uie.RaiseEvent(e);
        }
        finally
        {
            e.ClearUserInitiated();
        }
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
