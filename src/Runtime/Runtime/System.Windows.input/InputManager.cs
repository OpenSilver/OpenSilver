
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
        MOUSE_MOVE = 0,
        MOUSE_LEFT_DOWN = 1,
        MOUSE_LEFT_UP = 2,
        MOUSE_RIGHT_DOWN = 3,
        MOUSE_RIGHT_UP = 4,
        MOUSE_ENTER = 5,
        MOUSE_LEAVE = 6,
        WHEEL = 7,
        KEYDOWN = 8,
        KEYUP = 9,
        KEYPRESS = 10,
        TOUCH_START = 11,
        TOUCH_END = 12,
        TOUCH_MOVE = 13,
        FOCUS_MANAGED = 14,
        FOCUS_UNMANAGED = 15,
        WINDOW_FOCUS = 16,
        WINDOW_BLUR = 17,
    }

    private enum MouseButton
    {
        /// <summary>
        /// The left mouse button.
        /// </summary>
        Left,

        /// <summary>
        /// The middle mouse button.
        /// </summary>
        Middle,

        /// <summary>
        /// The right mouse button.
        /// </summary>
        Right,
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
                target.RaiseEvent(new RoutedEventArgs
                {
                    RoutedEvent = routedEvent,
                    OriginalSource = target,
                });
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
    private readonly FocusQueue _focusQueue = new();

    private const int _doubleClickDeltaTime = 400;
    private const int _doubleClickDeltaX = 5;
    private const int _doubleClickDeltaY = 5;
    private Point _lastClick = new Point();
    private MouseButton _lastButton;
    private int _clickCount;
    private int _lastClickTime;
    private WeakReference<UIElement> _lastClickTarget;
    private bool _mouseLeftDown;

    private InputManager()
    {
        _handler = JavaScriptCallback.Create(ProcessInput, true);

        if (Current == null)
        {
            string sHandler = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_handler);
            OpenSilver.Interop.ExecuteJavaScriptVoid($"document.createInputManager({sHandler});");
        }
    }

    /// <summary>
    /// Return the input manager associated with the current context.
    /// </summary>
    public static InputManager Current { get; } = new InputManager();

    internal void RegisterRoot(object element)
    {
        string sElement = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(element);
        OpenSilver.Interop.ExecuteJavaScriptVoid($"document.inputManager.registerRoot({sElement});");
    }

    internal ModifierKeys GetKeyboardModifiers()
    {
        return (ModifierKeys)OpenSilver.Interop.ExecuteJavaScriptInt32("document.inputManager.getModifiers();", false);
    }

    internal bool CaptureMouse(UIElement uie)
    {
        if (Pointer.INTERNAL_captured is null && _mouseLeftDown)
        {
            Pointer.INTERNAL_captured = uie;

            string sDiv = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(uie.INTERNAL_OuterDomElement);
            OpenSilver.Interop.ExecuteJavaScriptVoid($"document.inputManager.captureMouse({sDiv});");

            return true;
        }

        return Pointer.INTERNAL_captured == uie;
    }

    internal void ReleaseMouseCapture(UIElement uie)
    {
        if (Pointer.INTERNAL_captured == uie)
        {
            Pointer.INTERNAL_captured = null;
            OpenSilver.Interop.ExecuteJavaScriptVoid($"document.inputManager.releaseMouseCapture();");

            uie.RaiseEvent(new MouseEventArgs
            {
                RoutedEvent = UIElement.LostMouseCaptureEvent,
                OriginalSource = uie,
            });
        }
    }

    internal void AddEventListeners(UIElement uie, bool isFocusable)
    {
        if (uie.INTERNAL_OuterDomElement is INTERNAL_HtmlDomElementReference domRef)
        {
            OpenSilver.Interop.ExecuteJavaScriptFastAsync(
                $"document.inputManager.addListeners('{domRef.UniqueIdentifier}', {(isFocusable ? "true" : "false")});");
        }
        else
        {
            string sOuter = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(uie.INTERNAL_OuterDomElement);
            OpenSilver.Interop.ExecuteJavaScriptFastAsync(
                $"document.inputManager.addListeners({sOuter}, {(isFocusable ? "true" : "false")});");
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

        if (uie.GetFocusTarget() is object target)
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

    internal static bool SetFocusNative(object domElement)
    {
        string sDiv = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(domElement);
        return OpenSilver.Interop.ExecuteJavaScriptBoolean($"document.inputManager.focus({sDiv});");
    }

    internal static void ClearTabIndex(UIElement uie)
    {
        if (uie.GetFocusTarget() is { } domElement)
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
            if (uie.IsPointerOver)
            {
                uie.IsPointerOver = false;

                uie.RaiseEvent(new MouseEventArgs
                {
                    RoutedEvent = UIElement.MouseLeaveEvent,
                    OriginalSource = uie,
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
            if (Pointer.INTERNAL_captured == uie)
            {
                uie.ReleaseMouseCapture();
            }
        }
    }

    private void ProcessInput(string id, int eventId, object jsEventArg)
    {
        UIElement uie = INTERNAL_HtmlDomManager.GetElementById(id);
        if (uie is null)
        {
            ProcessEvent((EVENTS)eventId, jsEventArg);
        }
        else
        {
            DispatchEvent(uie, (EVENTS)eventId, jsEventArg);
        }
    }

    private void ProcessEvent(EVENTS eventType, object jsEventArg)
    {
        switch (eventType)
        {
            case EVENTS.MOUSE_LEFT_DOWN:
                _mouseLeftDown = true;
                RefreshClickCount(null, MouseButton.Left, Environment.TickCount, new Point());
                break;

            case EVENTS.MOUSE_RIGHT_DOWN:
                RefreshClickCount(null, MouseButton.Right, Environment.TickCount, new Point());
                break;

            case EVENTS.MOUSE_LEFT_UP:
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
        if (Pointer.INTERNAL_captured is UIElement uie)
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
            DependencyObject rootVisual = Window.Current?.Content;
            if (rootVisual is not null)
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
            focusedElement.RaiseEvent(new RoutedEventArgs
            {
                RoutedEvent = UIElement.GotFocusEvent,
                OriginalSource = focusedElement,
                UIEventArg = jsEventArg,
            });
        }
    }

    private void OnWindowBlur(object jsEventArg)
    {
        if (FocusManager.GetFocusedElement() is UIElement focusedElement)
        {
            focusedElement.RaiseEvent(new RoutedEventArgs
            {
                RoutedEvent = UIElement.LostFocusEvent,
                OriginalSource = focusedElement,
                UIEventArg = jsEventArg,
            });
        }
    }

    private void DispatchEvent(UIElement uie, EVENTS eventType, object jsEventArg)
    {
        switch (eventType)
        {
            case EVENTS.MOUSE_MOVE:
            case EVENTS.TOUCH_MOVE:
                ProcessOnMouseMove(uie, jsEventArg);
                break;

            case EVENTS.MOUSE_LEFT_DOWN:
            case EVENTS.TOUCH_START:
                _mouseLeftDown = true;
                ProcessOnMouseLeftButtonDown(uie, jsEventArg);
                break;

            case EVENTS.MOUSE_LEFT_UP:
                _mouseLeftDown = false;
                ProcessOnMouseLeftButtonUp(uie, jsEventArg);
                break;

            case EVENTS.MOUSE_RIGHT_DOWN:
                ProcessOnMouseRightButtonDown(uie, jsEventArg);
                break;

            case EVENTS.MOUSE_RIGHT_UP:
                ProcessOnMouseRightButtonUp(uie, jsEventArg);
                break;

            case EVENTS.MOUSE_ENTER:
                ProcessOnMouseEnter(uie, jsEventArg);
                break;

            case EVENTS.MOUSE_LEAVE:
                ProcessOnMouseLeave(uie, jsEventArg);
                break;

            case EVENTS.WHEEL:
                ProcessOnWheel(uie, jsEventArg);
                break;

            case EVENTS.KEYDOWN:
                ProcessOnKeyDown(uie, jsEventArg);
                break;

            case EVENTS.KEYUP:
                ProcessOnKeyUp(uie, jsEventArg);
                break;

            case EVENTS.KEYPRESS:
                ProcessOnKeyPress(uie, jsEventArg);
                break;

            case EVENTS.TOUCH_END:
                ProcessOnTouchEndEvent(uie, jsEventArg);
                break;

            case EVENTS.FOCUS_UNMANAGED:
                ProcessOnFocusUnmanaged(uie, jsEventArg);
                break;
        }
    }

    private void ProcessOnMouseMove(UIElement uie, object jsEventArg)
    {
        UIElement mouseTarget = uie.MouseTarget;
        if (mouseTarget is not null)
        {
            ProcessPointerEvent(mouseTarget, jsEventArg, UIElement.MouseMoveEvent);
        }
    }

    private void ProcessOnMouseLeftButtonDown(UIElement uie, object jsEventArg)
    {
        UIElement mouseTarget = uie.MouseTarget;
        if (mouseTarget is not null)
        {
            ProcessMouseButtonEvent(
                mouseTarget,
                jsEventArg,
                UIElement.MouseLeftButtonDownEvent,
                MouseButton.Left,
                Environment.TickCount,
                refreshClickCount: true,
                closeToolTips: true);
        }
    }

    private void ProcessOnMouseLeftButtonUp(UIElement uie, object jsEventArg)
    {
        UIElement mouseTarget = uie.MouseTarget;
        if (mouseTarget is not null)
        {
            ProcessMouseButtonEvent(
                mouseTarget,
                jsEventArg,
                UIElement.MouseLeftButtonUpEvent,
                MouseButton.Left,
                Environment.TickCount,
                refreshClickCount: false,
                closeToolTips: false);

            ProcessOnTapped(mouseTarget, jsEventArg);
        }

        ReleaseMouseCapture();
    }

    private void ProcessOnMouseRightButtonDown(UIElement uie, object jsEventArg)
    {
        UIElement mouseTarget = uie.MouseTarget;
        if (mouseTarget is not null)
        {
            bool handled = ProcessMouseButtonEvent(
                mouseTarget,
                jsEventArg,
                UIElement.MouseRightButtonDownEvent,
                MouseButton.Right,
                Environment.TickCount,
                refreshClickCount: true,
                closeToolTips: true);

            if (handled)
            {
                OpenSilver.Interop.ExecuteJavaScriptVoid("document.inputManager.suppressContextMenu(true);");
            }
        }
    }

    private void ProcessOnMouseRightButtonUp(UIElement uie, object jsEventArg)
    {
        UIElement mouseTarget = uie.MouseTarget;
        if (mouseTarget is not null)
        {
            var e = new MouseButtonEventArgs()
            {
                RoutedEvent = UIElement.MouseRightButtonUpEvent,
                OriginalSource = mouseTarget,
                UIEventArg = jsEventArg,
            };

            e.FillEventArgs(mouseTarget, jsEventArg);
            mouseTarget.RaiseEvent(e);
        }

        ReleaseMouseCapture();
    }

    private void ProcessOnWheel(UIElement uie, object jsEventArg)
    {
        UIElement mouseTarget = uie.MouseTarget;
        if (mouseTarget is not null)
        {
            var e = new MouseWheelEventArgs
            {
                RoutedEvent = UIElement.MouseWheelEvent,
                OriginalSource = mouseTarget,
                UIEventArg = jsEventArg,
            };

            e.FillEventArgs(mouseTarget, jsEventArg);

            // fill the Mouse Wheel delta:
            e.Delta = MouseWheelEventArgs.GetPointerWheelDelta(jsEventArg);

            mouseTarget.RaiseEvent(e);

            if (e.Handled)
            {
                e.PreventDefault();
            }
        }
    }

    private void ProcessOnMouseEnter(UIElement uie, object jsEventArg)
    {
        UIElement mouseTarget = uie.MouseTarget;
        if (mouseTarget is not null)
        {
            mouseTarget.IsPointerOver = true;

            ProcessPointerEvent(mouseTarget, jsEventArg, UIElement.MouseEnterEvent);
        }
    }

    private void ProcessOnMouseLeave(UIElement uie, object jsEventArg)
    {
        UIElement mouseTarget = uie.MouseTarget;
        if (mouseTarget is not null)
        {
            mouseTarget.IsPointerOver = false;

            ProcessPointerEvent(mouseTarget, jsEventArg, UIElement.MouseLeaveEvent);
        }
    }

    private void ProcessOnKeyDown(UIElement uie, object jsEventArg)
    {
        UIElement keyboardTarget = uie.KeyboardTarget;
        if (keyboardTarget is null || !int.TryParse(OpenSilver.Interop.ExecuteJavaScriptString(
            $"{CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(jsEventArg)}.keyCode"), out int keyCode))
        {
            return;
        }

        keyCode = INTERNAL_VirtualKeysHelpers.FixKeyCodeForSilverlight(keyCode);
        var e = new KeyEventArgs()
        {
            RoutedEvent = UIElement.KeyDownEvent,
            OriginalSource = keyboardTarget,
            UIEventArg = jsEventArg,
            PlatformKeyCode = keyCode,
            Key = INTERNAL_VirtualKeysHelpers.GetKeyFromKeyCode(keyCode),
            KeyModifiers = Keyboard.Modifiers,
        };

        ToolTipService.OnKeyDown(e);

        keyboardTarget.RaiseEvent(e);

        KeyboardNavigation.Current.ProcessInput(e);

        if (e.Handled)
        {
            e.PreventDefault();
        }
    }

    private void ProcessOnKeyUp(UIElement uie, object jsEventArg)
    {
        UIElement keyboardTarget = uie.KeyboardTarget;

        if (keyboardTarget is null || !int.TryParse(OpenSilver.Interop.ExecuteJavaScriptString(
            $"{CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(jsEventArg)}.keyCode"), out int keyCode))
        {
            return;
        }

        keyCode = INTERNAL_VirtualKeysHelpers.FixKeyCodeForSilverlight(keyCode);
        var e = new KeyEventArgs()
        {
            RoutedEvent = UIElement.KeyUpEvent,
            OriginalSource = keyboardTarget,
            UIEventArg = jsEventArg,
            PlatformKeyCode = keyCode,
            Key = INTERNAL_VirtualKeysHelpers.GetKeyFromKeyCode(keyCode),
            KeyModifiers = Keyboard.Modifiers,
        };

        keyboardTarget.RaiseEvent(e);
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
        UIElement keyboardTarget = uie.KeyboardTarget;
        if (keyboardTarget is null || !int.TryParse(OpenSilver.Interop.ExecuteJavaScriptString(
            $"{CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(jsEventArg)}.keyCode"), out int keyCode))
        {
            return;
        }

        var e = new TextCompositionEventArgs
        {
            RoutedEvent = UIElement.TextInputEvent,
            OriginalSource = keyboardTarget,
            Text = ((char)keyCode).ToString(),
            TextComposition = new TextComposition(string.Empty),
            UIEventArg = jsEventArg,
        };

        keyboardTarget.RaiseEvent(e);

        if (e.PreventDefault)
        {
            OpenSilver.Interop.ExecuteJavaScriptVoid(
                $"{CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(jsEventArg)}.preventDefault()");
        }
    }

    private void ProcessPointerEvent(UIElement uie, object jsEventArg, RoutedEvent routedEvent)
    {
        var e = new MouseEventArgs()
        {
            RoutedEvent = routedEvent,
            OriginalSource = uie,
            UIEventArg = jsEventArg,
        };

        e.FillEventArgs(uie, jsEventArg);
        uie.RaiseEvent(e);
    }

    private void ProcessOnTouchEndEvent(
        UIElement uie,
        object jsEventArg)
    {
        var e = new MouseButtonEventArgs()
        {
            RoutedEvent = UIElement.MouseLeftButtonUpEvent,
            OriginalSource = uie,
            UIEventArg = jsEventArg,
        };

        e.FillEventArgs(uie, jsEventArg);
        
        uie.RaiseEvent(e);
    }

    private bool ProcessMouseButtonEvent(
        UIElement uie,
        object jsEventArg,
        RoutedEvent routedEvent,
        MouseButton button,
        int timeStamp,
        bool refreshClickCount,
        bool closeToolTips)
    {
        var e = new MouseButtonEventArgs()
        {
            RoutedEvent = routedEvent,
            OriginalSource = uie,
            UIEventArg = jsEventArg,
        };

        e.FillEventArgs(uie, jsEventArg);

        if (refreshClickCount)
        {
            e.ClickCount = RefreshClickCount(uie, button, timeStamp, e.GetPosition(null));
        }

        if (closeToolTips)
        {
            ToolTipService.OnMouseButtonDown(e);
        }

        uie.RaiseEvent(e);

        return e.Handled;
    }

    private void ProcessOnTapped(UIElement uie, object jsEventArg)
    {
        var e = new TappedRoutedEventArgs
        {
            RoutedEvent = UIElement.TappedEvent,
            OriginalSource = uie,
            UIEventArg = jsEventArg,
        };

        e.FillEventArgs(uie, jsEventArg);
        uie.RaiseEvent(e);
    }

    private int RefreshClickCount(UIElement target, MouseButton button, int timeStamp, Point ptClient)
    {
        _clickCount = CalculateClickCount(target, button, timeStamp, ptClient);
        
        if (_clickCount == 1)
        {
            // we need to reset out data, since this is the start of the click count process...
            _lastButton = button;            
            _lastClickTarget ??= new WeakReference<UIElement>(null);
            _lastClickTarget.SetTarget(target);
        }

        _lastClick = ptClient;
        _lastClickTime = timeStamp;

        return _clickCount;
    }

    private int CalculateClickCount(UIElement uie, MouseButton button, int timeStamp, Point downPt)
    {
        if (timeStamp - _lastClickTime < _doubleClickDeltaTime // How long since the last click?
              && _lastButton == button // Is this the same mouse button as the last click?
              && IsSameTarget(uie) // Is it the same element as the last click?
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

    private bool IsSameTarget(UIElement target)
    {
        return target != null &&
            _lastClickTarget != null &&
            _lastClickTarget.TryGetTarget(out UIElement uie) &&
            target == uie;
    }
}
