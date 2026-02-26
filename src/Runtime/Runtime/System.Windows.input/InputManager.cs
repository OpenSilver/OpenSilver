
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
using OpenSilver;
using OpenSilver.Internal.Controls.Primitives;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media;

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
        FOCUS_UNMANAGED = 13,
        WINDOW_FOCUS = 14,
        WINDOW_BLUR = 15,
    }

    private readonly struct PointerCallbackParameters(bool isTouchEvent, double pageX, double pageY, ModifierKeys modifiers, object uiEventArg)
    {
        public readonly bool IsTouchEvent = isTouchEvent;
        public readonly double PageX = pageX;
        public readonly double PageY = pageY;
        public readonly ModifierKeys KeyModifiers = modifiers;
        public readonly object UIEventArg = uiEventArg;
    }

    private sealed class EventQueue
    {
        private readonly ConcurrentQueue<RoutedEventArgs> _queue = [];
        private readonly EventQueueProcessingDisabled _processingDisabled;
        private int _disableProcessingRequests;

        public EventQueue()
        {
            _processingDisabled = new(this);
        }

        public void AddEvent(RoutedEventArgs args) => _queue.Enqueue(args);

        public IDisposable DisableProcessing()
        {
            Interlocked.Increment(ref _disableProcessingRequests);
            return _processingDisabled;
        }

        private void EnableProcessing()
        {
            if (Interlocked.Decrement(ref _disableProcessingRequests) == 0)
            {
                ProcessQueue();
            }
        }

        private void ProcessQueue()
        {
            if (_queue.IsEmpty)
            {
                return;
            }

            while (_queue.TryDequeue(out RoutedEventArgs args))
            {
                UIElement target = (UIElement)args.Source;
                target.RaiseTrustedEvent(args);

                if (args.RoutedEvent == UIElement.LostFocusEvent ||
                    args.RoutedEvent == UIElement.GotFocusEvent)
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        private sealed class EventQueueProcessingDisabled : IDisposable
        {
            private readonly EventQueue _eventQueue;

            public EventQueueProcessingDisabled(EventQueue eventQueue)
            {
                _eventQueue = eventQueue;
            }

            public void Dispose() => _eventQueue.EnableProcessing();
        }
    }

    private readonly EventQueue _eventQueue = new();

    private const int _doubleClickDeltaTime = 400;
    private const int _doubleClickDeltaX = 5;
    private const int _doubleClickDeltaY = 5;
    private Point _lastClick = new Point();
    private MouseButton _lastButton;
    private int _clickCount;
    private int _lastClickTime;
    private bool _mouseLeftDown;

    private InputManager() { }

    /// <summary>
    /// Return the input manager associated with the current context.
    /// </summary>
    public static InputManager Current { get; } = new InputManager();

    internal void RegisterRoot(HtmlElementReference element)
    {
        OpenSilver.Interop.ExecuteJavaScriptVoid($"osjs.inputManager.registerRoot('{element.Uid}')");
    }

    internal ModifierKeys GetKeyboardModifiers()
    {
        return (ModifierKeys)OpenSilver.Interop.ExecuteJavaScriptInt32("osjs.inputManager.getModifiers()", false);
    }

    internal bool CaptureMouse(UIElement uie)
    {
        Debug.Assert(uie is not null);

        if (Pointer.Captured is null && _mouseLeftDown && uie.OuterDiv is { IsConnected: true } outerDiv)
        {
            Pointer.Captured = uie;

            OpenSilver.Interop.ExecuteJavaScriptVoid($"osjs.inputManager.capturePointer('{outerDiv.Uid}')");

            using (_eventQueue.DisableProcessing())
            {
                _eventQueue.AddEvent(new MouseEventArgs
                {
                    RoutedEvent = Mouse.GotMouseCaptureEvent,
                    Source = uie,
                });
            }

            return true;
        }

        return Pointer.Captured == uie;
    }

    internal void ReleaseMouseCapture(UIElement uie)
    {
        if (Pointer.Captured == uie)
        {
            Pointer.Captured = null;
            OpenSilver.Interop.ExecuteJavaScriptVoid($"osjs.inputManager.releasePointerCapture()");

            using (_eventQueue.DisableProcessing())
            {
                _eventQueue.AddEvent(new MouseEventArgs
                {
                    RoutedEvent = Mouse.LostMouseCaptureEvent,
                    Source = uie,
                });
            }
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

        HtmlElementReference target = uie.GetFocusTarget();
        if (target.IsConnected)
        {
            if (SetFocusNative(target))
            {
                KeyboardNavigation.UpdateFocusedElement(uie, focusScope);

                using (_eventQueue.DisableProcessing())
                {
                    if (focused is not null)
                    {
                        _eventQueue.AddEvent(new RoutedEventArgs(UIElement.LostFocusEvent, focused));
                    }

                    _eventQueue.AddEvent(new RoutedEventArgs(UIElement.GotFocusEvent, uie));
                }

                return true;

            }
            ClearTabIndex(uie);
        }

        return false;
    }

    internal static bool SetFocusNative(HtmlElementReference element)
    {
        return OpenSilver.Interop.ExecuteJavaScriptBoolean($"osjs.inputManager.focus('{element.Uid}')");
    }

    internal static void ClearTabIndex(UIElement uie)
    {
        HtmlElementReference element = uie.GetFocusTarget();
        if (element.IsConnected)
        {
            switch (uie)
            {
                case TextBox or PasswordBox:
                    element.SetAttribute("tabindex", "-1");
                    break;

                default:
                    element.RemoveAttribute("tabindex");
                    break;
            }
        }
    }

    internal void OnElementRemoved(UIElement uie)
    {
        using (_eventQueue.DisableProcessing())
        {
            RaiseMouseLeave(uie);
            ResetFocus(uie);
            ReleaseMouseCapture(uie);
        }

        void RaiseMouseLeave(UIElement uie)
        {
            if (uie.IsMouseOver)
            {
                uie.ClearValue(UIElement.IsMouseOverPropertyKey);

                _eventQueue.AddEvent(new MouseEventArgs
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

                _eventQueue.AddEvent(new RoutedEventArgs(UIElement.LostFocusEvent, focused));
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

    internal static void ProcessInputNative(string id, int eventId, object jsEventArg) => Current.ProcessInput(id, eventId, jsEventArg);

    private void ProcessInput(string id, int eventId, object jsEventArg)
    {
        using (_eventQueue.DisableProcessing())
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
    }

    internal static void ProcessPointerInputNative(string id, int eventId, object jsEventArg, bool isTouchEvent, double pageX, double pageY, int keyModifiers)
        => Current.ProcessPointerInput(id, eventId, jsEventArg, isTouchEvent, pageX, pageY, keyModifiers);

    private void ProcessPointerInput(string id, int eventId, object jsEventArg, bool isTouchEvent, double pageX, double pageY, int keyModifiers)
    {
        using (_eventQueue.DisableProcessing())
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
                    new PointerCallbackParameters(isTouchEvent, pageX, pageY, (ModifierKeys)keyModifiers, jsEventArg));
            }
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

            case EVENTS.POINTER_LEFT_UP:
                _mouseLeftDown = false;
                ReleaseMouseCapture();
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
            focusedElement.RaiseTrustedEvent(new RoutedEventArgs(UIElement.GotFocusEvent, focusedElement)
            {
                UIEventArg = jsEventArg,
            });
        }
    }

    private void OnWindowBlur(object jsEventArg)
    {
        if (FocusManager.GetFocusedElement() is UIElement focusedElement)
        {
            _eventQueue.AddEvent(new RoutedEventArgs(UIElement.LostFocusEvent, focusedElement)
            {
                UIEventArg = jsEventArg,
            });
        }
    }

    private void ProcessOnMouseMove(UIElement uie, PointerCallbackParameters parameters)
    {
        if (uie.MouseTarget is not UIElement mouseTarget)
        {
            return;
        }

        var previewMove = new MouseEventArgs(parameters.IsTouchEvent, parameters.KeyModifiers, parameters.PageX, parameters.PageY)
        {
            RoutedEvent = Mouse.PreviewMouseMoveEvent,
            Source = mouseTarget,
            UIEventArg = parameters.UIEventArg,
        };

        mouseTarget.RaiseTrustedEvent(previewMove);

        if (previewMove.Handled)
        {
            return;
        }

        var move = new MouseEventArgs(parameters.IsTouchEvent, parameters.KeyModifiers, parameters.PageX, parameters.PageY)
        {
            RoutedEvent = Mouse.MouseMoveEvent,
            Source = mouseTarget,
            UIEventArg = parameters.UIEventArg,
        };

        mouseTarget.RaiseTrustedEvent(move);
    }

    private void ProcessOnMouseLeftButtonDown(UIElement uie, PointerCallbackParameters parameters)
    {
        if (uie.MouseTarget is UIElement mouseTarget)
        {
            ProcessMouseDownEvent(
                mouseTarget,
                parameters,
                MouseButton.Left,
                refreshClickCount: true,
                closeToolTips: true);
        }
    }

    private void ProcessOnMouseLeftButtonUp(UIElement uie, PointerCallbackParameters parameters)
    {
        if (uie.MouseTarget is UIElement mouseTarget)
        {
            ProcessMouseUpEvent(mouseTarget, parameters, MouseButton.Left);

            ProcessOnTapped(mouseTarget, parameters);
        }

        ReleaseMouseCapture();
    }

    private void ProcessOnMouseRightButtonDown(UIElement uie, PointerCallbackParameters parameters)
    {
        if (uie.MouseTarget is UIElement mouseTarget)
        {
            bool handled = ProcessMouseDownEvent(
                mouseTarget,
                parameters,
                MouseButton.Right,
                refreshClickCount: true,
                closeToolTips: true);

            if (handled)
            {
                OpenSilver.Interop.ExecuteJavaScriptVoid("osjs.inputManager.suppressContextMenu(true)");
            }
        }
    }

    private void ProcessOnMouseRightButtonUp(UIElement uie, PointerCallbackParameters parameters)
    {
        if (uie.MouseTarget is UIElement mouseTarget)
        {
            ProcessMouseUpEvent(mouseTarget, parameters, MouseButton.Right);
        }

        ReleaseMouseCapture();
    }

    private void ProcessOnMouseMiddleButtonDown(UIElement uie, PointerCallbackParameters parameters)
    {
        if (uie.MouseTarget is UIElement mouseTarget)
        {
            ProcessMouseDownEvent(
                mouseTarget,
                parameters,
                MouseButton.Middle,
                refreshClickCount: true,
                closeToolTips: false);
        }
    }

    private void ProcessOnMouseMiddleButtonUp(UIElement uie, PointerCallbackParameters parameters)
    {
        if (uie.MouseTarget is UIElement mouseTarget)
        {
            ProcessMouseUpEvent(mouseTarget, parameters, MouseButton.Middle);
        }
    }

    private void ProcessOnWheel(UIElement uie, PointerCallbackParameters parameters)
    {
        if (uie.MouseTarget is UIElement mouseTarget)
        {
            int delta = OpenSilver.Interop.ExecuteJavaScriptDouble(
                $"{OpenSilver.Interop.GetVariableStringForJS(parameters.UIEventArg)}.deltaY", false) > 0 ? -120 : 120;

            var previewWheel = new MouseWheelEventArgs(parameters.IsTouchEvent, parameters.KeyModifiers, parameters.PageX, parameters.PageY, delta)
            {
                RoutedEvent = Mouse.PreviewMouseWheelEvent,
                Source = mouseTarget,
                UIEventArg = parameters.UIEventArg,
            };

            mouseTarget.RaiseTrustedEvent(previewWheel);

            if (previewWheel.Handled)
            {
                previewWheel.PreventDefault();
                return;
            }

            var wheel = new MouseWheelEventArgs(parameters.IsTouchEvent, parameters.KeyModifiers, parameters.PageX, parameters.PageY, delta)
            {
                RoutedEvent = Mouse.MouseWheelEvent,
                Source = mouseTarget,
                UIEventArg = parameters.UIEventArg,
            };

            mouseTarget.RaiseTrustedEvent(wheel);

            if (wheel.Handled)
            {
                wheel.PreventDefault();
            }
        }
    }

    private void ProcessOnMouseEnter(UIElement uie, PointerCallbackParameters parameters)
    {
        if (uie.MouseTarget is not UIElement mouseTarget)
        {
            return;
        }

        mouseTarget.SetValueInternal(UIElement.IsMouseOverPropertyKey, true);

        var mouseEnter = new MouseEventArgs(parameters.IsTouchEvent, parameters.KeyModifiers, parameters.PageX, parameters.PageY)
        {
            RoutedEvent = Mouse.MouseEnterEvent,
            Source = mouseTarget,
            UIEventArg = parameters.UIEventArg,
        };

        mouseTarget.RaiseTrustedEvent(mouseEnter);
    }

    private void ProcessOnMouseLeave(UIElement uie, PointerCallbackParameters parameters)
    {
        if (uie.MouseTarget is not UIElement mouseTarget)
        {
            return;
        }

        mouseTarget.ClearValue(UIElement.IsMouseOverPropertyKey);

        var mouseLeave = new MouseEventArgs(parameters.IsTouchEvent, parameters.KeyModifiers, parameters.PageX, parameters.PageY)
        {
            RoutedEvent = Mouse.MouseLeaveEvent,
            Source = mouseTarget,
            UIEventArg = parameters.UIEventArg,
        };

        mouseTarget.RaiseTrustedEvent(mouseLeave);
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
        Key key = VirtualKeysHelpers.GetKeyFromKeyCode(keyCode);
        ModifierKeys modifiers = Keyboard.Modifiers;

        ToolTipService.OnKeyDown(key);

        var previewKeyDown = new KeyEventArgs
        {
            RoutedEvent = Keyboard.PreviewKeyDownEvent,
            Source = keyboardTarget,
            UIEventArg = jsEventArg,
            PlatformKeyCode = keyCode,
            Key = key,
            KeyModifiers = modifiers,
        };

        keyboardTarget.RaiseTrustedEvent(previewKeyDown);

        if (previewKeyDown.Handled)
        {
            previewKeyDown.PreventDefault();
            return;
        }

        var keyDown = new KeyEventArgs
        {
            RoutedEvent = Keyboard.KeyDownEvent,
            Source = keyboardTarget,
            UIEventArg = jsEventArg,
            PlatformKeyCode = keyCode,
            Key = key,
            KeyModifiers = modifiers,
        };

        keyboardTarget.RaiseTrustedEvent(keyDown);

        KeyboardNavigation.Current.ProcessInput(keyDown);

        if (keyDown.Handled)
        {
            keyDown.PreventDefault();
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
        Key key = VirtualKeysHelpers.GetKeyFromKeyCode(keyCode);
        ModifierKeys modifiers = Keyboard.Modifiers;

        var previewKeyUp = new KeyEventArgs
        {
            RoutedEvent = Keyboard.PreviewKeyUpEvent,
            Source = keyboardTarget,
            UIEventArg = jsEventArg,
            PlatformKeyCode = keyCode,
            Key = key,
            KeyModifiers = modifiers,
        };

        keyboardTarget.RaiseTrustedEvent(previewKeyUp);

        if (previewKeyUp.Handled)
        {
            return;
        }

        var keyUp = new KeyEventArgs
        {
            RoutedEvent = Keyboard.KeyUpEvent,
            Source = keyboardTarget,
            UIEventArg = jsEventArg,
            PlatformKeyCode = keyCode,
            Key = key,
            KeyModifiers = modifiers,
        };

        keyboardTarget.RaiseTrustedEvent(keyUp);

        CommandManager.InvalidateRequerySuggested();
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

        using (_eventQueue.DisableProcessing())
        {
            if (oldFocus is not null)
            {
                _eventQueue.AddEvent(new RoutedEventArgs(UIElement.LostFocusEvent, oldFocus));
            }

            if (newFocus is not null)
            {
                _eventQueue.AddEvent(new RoutedEventArgs(UIElement.GotFocusEvent, newFocus));
            }
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

        keyboardTarget.RaiseTrustedEvent(textInputStartArgs);

        var textInputArgs = new TextCompositionEventArgs
        {
            RoutedEvent = UIElement.TextInputEvent,
            Source = keyboardTarget,
            Text = text,
            TextComposition = TextComposition.Empty,
            UIEventArg = jsEventArg,
        };

        keyboardTarget.RaiseTrustedEvent(textInputArgs);

        if (textInputArgs.Cancel)
        {
            textInputArgs.PreventDefault();
        }
    }

    private bool ProcessMouseDownEvent(
        UIElement uie,
        PointerCallbackParameters parameters,
        MouseButton button,
        bool refreshClickCount,
        bool closeToolTips)
    {
        if (closeToolTips)
        {
            ToolTipService.OnMouseButtonDown();
        }

        var previewMouseDown = new MouseButtonEventArgs(button,
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
            previewMouseDown.ClickCount = RefreshClickCount(button, Environment.TickCount, previewMouseDown.GetPosition(null));
        }

        uie.RaiseTrustedEvent(previewMouseDown);

        if (previewMouseDown.Handled)
        {
            return true;
        }

        var mouseDown = new MouseButtonEventArgs(button,
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
        var previewMouseUp = new MouseButtonEventArgs(button,
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

        var mouseUp = new MouseButtonEventArgs(button,
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
        var e = new TappedRoutedEventArgs(parameters.IsTouchEvent, parameters.KeyModifiers, parameters.PageX, parameters.PageY)
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
