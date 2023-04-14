
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

using System;
using CSHTML5.Internal;

#if MIGRATION
using System.Windows.Controls;
#else
using System.Windows.Input;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using MouseEventArgs = Windows.UI.Xaml.Input.PointerRoutedEventArgs;
using MouseButtonEventArgs = Windows.UI.Xaml.Input.PointerRoutedEventArgs;
using MouseWheelEventArgs = Windows.UI.Xaml.Input.PointerRoutedEventArgs;
using KeyEventArgs = Windows.UI.Xaml.Input.KeyRoutedEventArgs;
using ModifierKeys = Windows.System.VirtualKeyModifiers;
using INTERNAL_VirtualKeysHelpers = Windows.System.INTERNAL_VirtualKeysHelpers;
#endif

#if MIGRATION
namespace System.Windows.Input;
#else
namespace Windows.UI.Xaml.Input;
#endif

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
        FOCUS = 10,
        BLUR = 11,
        KEYPRESS = 12,
        INPUT = 13,
        TOUCH_START = 14,
        TOUCH_END = 15,
        TOUCH_MOVE = 16,
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

    private readonly JavaScriptCallback _handler;

    private const int _doubleClickDeltaTime = 400;
    private const int _doubleClickDeltaX = 5;
    private const int _doubleClickDeltaY = 5;
    private Point _lastClick = new Point();
    private MouseButton _lastButton;
    private int _clickCount;
    private int _lastClickTime;
    private WeakReference<UIElement> _lastClickTarget;

    private InputManager()
    {
        _handler = JavaScriptCallback.Create(ProcessInput, true);

        if (Current == null)
        {
            string sRootElement = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(
                INTERNAL_HtmlDomManager.GetApplicationRootDomElement());
            string sHandler = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_handler);
            OpenSilver.Interop.ExecuteJavaScriptVoid($"document.createInputManager({sRootElement}, {sHandler});");
        }
    }

    /// <summary>
    /// Return the input manager associated with the current context.
    /// </summary>
    public static InputManager Current { get; } = new InputManager();

    internal ModifierKeys GetKeyboardModifiers()
    {
        return (ModifierKeys)OpenSilver.Interop.ExecuteJavaScriptInt32("document.inputManager.getModifiers();", false);
    }

    internal bool CaptureMouse(UIElement uie)
    {
        if (Pointer.INTERNAL_captured is null)
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
        if (Pointer.INTERNAL_captured is not null)
        {
            Pointer.INTERNAL_captured = null;
            OpenSilver.Interop.ExecuteJavaScriptVoid($"document.inputManager.releaseMouseCapture();");
            uie.OnLostMouseCapturedInternal(new MouseEventArgs());
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
                RefreshClickCount(null, MouseButton.Left, Environment.TickCount, new Point());
                break;

            case EVENTS.MOUSE_RIGHT_DOWN:
                RefreshClickCount(null, MouseButton.Right, Environment.TickCount, new Point());
                break;

            case EVENTS.FOCUS:
                OnFocus();
                break;

            case EVENTS.WINDOW_BLUR:
                OnWindowBlur(jsEventArg);
                break;
        }
    }

    private void OnFocus()
    {
        if (FocusManager.GetFocusedElement() is UIElement focusedElement)
        {
            // Focus moved back to the application (most likely to the opensilver-root div).
            // Reposition focus to the element that has logical focus.
            INTERNAL_HtmlDomManager.SetFocus(focusedElement);
        }
        else
        {
            DependencyObject rootVisual = Application.Current?.RootVisual;
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
                ProcessOnMouseLeftButtonDown(uie, jsEventArg);
                break;

            case EVENTS.MOUSE_LEFT_UP:
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

            case EVENTS.FOCUS:
                ProcessOnFocus(uie, jsEventArg);
                break;

            case EVENTS.KEYPRESS:
                ProcessOnKeyPress(uie, jsEventArg);
                break;

            case EVENTS.INPUT:
                ProcessOnInput(uie, jsEventArg);
                break;

            case EVENTS.TOUCH_END:
                ProcessOnTouchEndEvent(uie, jsEventArg);
                break;
        }
    }

    private void ProcessOnMouseMove(UIElement uie, object jsEventArg)
    {
        UIElement mouseTarget = uie.MouseTarget;
        if (mouseTarget is not null)
        {
#if MIGRATION
            RoutedEvent routedEvent = UIElement.MouseMoveEvent;
#else
            RoutedEvent routedEvent = UIElement.PointerMovedEvent;
#endif
            ProcessPointerEvent(mouseTarget, jsEventArg, routedEvent);
        }
    }

    private void ProcessOnMouseLeftButtonDown(UIElement uie, object jsEventArg)
    {
        UIElement mouseTarget = uie.MouseTarget;
        if (mouseTarget is not null)
        {
#if MIGRATION
            RoutedEvent routedEvent = UIElement.MouseLeftButtonDownEvent;
#else
            RoutedEvent routedEvent = UIElement.PointerPressedEvent;
#endif
            ProcessMouseButtonEvent(
                mouseTarget,
                jsEventArg,
                routedEvent,
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
#if MIGRATION
            RoutedEvent routedEvent = UIElement.MouseLeftButtonUpEvent;
#else
            RoutedEvent routedEvent = UIElement.PointerReleasedEvent;
#endif
            ProcessMouseButtonEvent(
                mouseTarget,
                jsEventArg,
                routedEvent,
                MouseButton.Left,
                Environment.TickCount,
                refreshClickCount: false,
                closeToolTips: false);

            ProcessOnTapped(mouseTarget, jsEventArg);

            mouseTarget.Dispatcher.BeginInvoke(() =>
            {
#if MIGRATION
                Pointer.INTERNAL_captured?.ReleaseMouseCapture();
#else
                Pointer.INTERNAL_captured?.ReleasePointerCapture();
#endif
            });
        }
    }

    private void ProcessOnMouseRightButtonDown(UIElement uie, object jsEventArg)
    {
#if MIGRATION
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
#endif
    }

    private void ProcessOnMouseRightButtonUp(UIElement uie, object jsEventArg)
    {
        UIElement mouseTarget = uie.MouseTarget;
        if (mouseTarget is not null)
        {
#if MIGRATION
            var e = new MouseButtonEventArgs()
            {
                RoutedEvent = UIElement.MouseRightButtonUpEvent,
                OriginalSource = mouseTarget,
                UIEventArg = jsEventArg,
            };
#else
            var e = new RightTappedRoutedEventArgs()
            {
                RoutedEvent = UIElement.RightTappedEvent,
                OriginalSource = mouseTarget,
                UIEventArg = jsEventArg,
            };
#endif

            e.FillEventArgs(mouseTarget, jsEventArg);
            mouseTarget.RaiseEvent(e);

            mouseTarget.Dispatcher.BeginInvoke(() =>
            {
#if MIGRATION
                Pointer.INTERNAL_captured?.ReleaseMouseCapture();
#else
                Pointer.INTERNAL_captured?.ReleasePointerCapture();
#endif
            });
        }
    }

    private void ProcessOnWheel(UIElement uie, object jsEventArg)
    {
        UIElement mouseTarget = uie.MouseTarget;
        if (mouseTarget is not null)
        {
#if MIGRATION
            RoutedEvent routedEvent = UIElement.MouseWheelEvent;
#else
            RoutedEvent routedEvent = UIElement.PointerWheelChangedEvent;
#endif
            var e = new MouseWheelEventArgs
            {
                RoutedEvent = routedEvent,
                OriginalSource = mouseTarget,
                UIEventArg = jsEventArg,
            };

            e.FillEventArgs(mouseTarget, jsEventArg);

#if MIGRATION
            // fill the Mouse Wheel delta:
            e.Delta = MouseWheelEventArgs.GetPointerWheelDelta(jsEventArg);
#endif

            mouseTarget.RaiseEvent(e);
        }
    }

    private void ProcessOnMouseEnter(UIElement uie, object jsEventArg)
    {
        UIElement mouseTarget = uie.MouseTarget;
        if (mouseTarget is not null)
        {
            mouseTarget.IsPointerOver = true;

#if MIGRATION
            RoutedEvent routedEvent = UIElement.MouseEnterEvent;
#else
            RoutedEvent routedEvent = UIElement.PointerEnteredEvent;
#endif
            ProcessPointerEvent(mouseTarget, jsEventArg, routedEvent);
        }
    }

    private void ProcessOnMouseLeave(UIElement uie, object jsEventArg)
    {
        UIElement mouseTarget = uie.MouseTarget;
        if (mouseTarget is not null)
        {
            mouseTarget.IsPointerOver = false;

#if MIGRATION
            RoutedEvent routedEvent = UIElement.MouseLeaveEvent;
#else
            RoutedEvent routedEvent = UIElement.PointerExitedEvent;
#endif
            ProcessPointerEvent(mouseTarget, jsEventArg, routedEvent);
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

#if MIGRATION
        keyCode = INTERNAL_VirtualKeysHelpers.FixKeyCodeForSilverlight(keyCode);
#endif
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

        if (e.Handled && e.Cancellable)
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

#if MIGRATION
        keyCode = INTERNAL_VirtualKeysHelpers.FixKeyCodeForSilverlight(keyCode);
#endif
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

    private void ProcessOnFocus(UIElement uie, object jsEventArg)
    {
        UIElement newFocus = uie.KeyboardTarget;

        newFocus?.RaiseEvent(new RoutedEventArgs
        {
            RoutedEvent = UIElement.GotFocusEvent,
            OriginalSource = newFocus,
        });
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

    private void ProcessOnInput(UIElement uie, object jsEventArg)
    {
        uie.KeyboardTarget?.OnTextInputInternal();
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
#if MIGRATION
        RoutedEvent routedEvent = UIElement.MouseLeftButtonUpEvent;
#else
        RoutedEvent routedEvent = UIElement.PointerReleasedEvent;
#endif

        var e = new MouseButtonEventArgs()
        {
            RoutedEvent = routedEvent,
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
