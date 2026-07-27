
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
using System.Collections.Concurrent;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Threading;

namespace System.Windows.Input;

/// <summary>
/// Manages all the input systems in Windows Presentation Foundation (WPF).
/// </summary>
public sealed class InputManager : DispatcherObject
{
    // This must remain synchronyzed with the EVENTS enum defined in cshtml5.js.
    // Make sure to change both files if you update this !
    internal enum EVENTS
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
        POINTER_OVER = 9,
        POINTER_CAPTURE_LOST = 10,
        WHEEL = 11,
        KEYDOWN = 12,
        KEYUP = 13,
        KEYPRESS = 14,
        FOCUS_IN = 15,
        FOCUS_OUT = 16,
        WINDOW_FOCUS = 17,
        WINDOW_BLUR = 18,
    }

    internal readonly struct PointerCallbackParameters(bool isTouchEvent, double pageX, double pageY, ModifierKeys modifiers, object uiEventArg)
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
    private readonly BrowserKeyboardDevice _primaryKeyboardDevice;
    private readonly BrowserMouseDevice _primaryMouseDevice;

    private InputManager()
    {
        _primaryKeyboardDevice = new BrowserKeyboardDevice(this);
        _primaryMouseDevice = new BrowserMouseDevice(this);
    }

    /// <summary>
    /// Gets the <see cref="InputManager"/> associated with the current thread.
    /// </summary>
    /// <returns>
    /// The input manager.
    /// </returns>
    public static InputManager Current { get; } = new InputManager();

    /// <summary>
    /// Gets the primary keyboard device.
    /// </summary>
    /// <returns>
    /// The keyboard device.
    /// </returns>
    public KeyboardDevice PrimaryKeyboardDevice => _primaryKeyboardDevice;

    /// <summary>
    /// Gets the primary mouse device.
    /// </summary>
    /// <returns>
    /// The mouse device.
    /// </returns>
    public MouseDevice PrimaryMouseDevice => _primaryMouseDevice;

    /// <summary>
    /// Gets a value that represents the input device associated with the most recent input event.
    /// </summary>
    /// <returns>
    /// The input device.
    /// </returns>
    public InputDevice MostRecentInputDevice { get; private set; }

    internal Window ActiveWindow { get; private set; }

    internal void RegisterRoot(HtmlElementReference element)
    {
        OpenSilver.Interop.ExecuteJavaScriptVoid($"osjs.inputManager.registerRoot('{element.Uid}')");
    }

    internal static bool SetFocusNative(HtmlElementReference element)
    {
        return OpenSilver.Interop.ExecuteJavaScriptBoolean($"osjs.inputManager.focus('{element.Uid}')");
    }

    internal static void ClearFocusNative()
    {
        OpenSilver.Interop.ExecuteJavaScriptVoid("osjs.inputManager.clearFocus()");
    }

    internal IDisposable DisableProcessing() => _eventQueue.DisableProcessing();

    internal void PushInput(RoutedEventArgs e) => _eventQueue.AddEvent(e);

    internal void OnElementRemoved(UIElement uie)
    {
        using (DisableProcessing())
        {
            RaiseMouseLeave(uie);
            ResetFocus(uie);
            ReleaseMouseCapture(uie);
        }

        void RaiseMouseLeave(UIElement uie)
        {
            if (uie.IsMouseOver)
            {
                int timestamp = Environment.TickCount;

                uie.ClearValue(UIElement.IsMouseOverPropertyKey);

                PushInput(new MouseEventArgs(_primaryMouseDevice, timestamp)
                {
                    RoutedEvent = Mouse.MouseLeaveEvent,
                    Source = uie,
                });
            }
        }

        void ResetFocus(UIElement uie)
        {
            if (uie.IsKeyboardFocused)
            {
                _primaryKeyboardDevice.ReevaluateFocus();
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
            if (uie.IsMouseCaptured)
            {
                uie.ReleaseMouseCapture();
            }
        }
    }

    internal static void ProcessInputNative(string id, int eventId, object jsEventArg) => Current.ProcessInput(id, eventId, jsEventArg);

    private void ProcessInput(string id, int eventId, object jsEventArg)
    {
        using (DisableProcessing())
        {
            UIElement uie = INTERNAL_HtmlDomManager.GetElementById(id);
            EVENTS eventType = (EVENTS)eventId;

            if (uie is not null)
            {
                ActiveWindow = uie.ParentWindow;
            }

            UpdateMostRecentDevice(eventType);

            _primaryKeyboardDevice.ProcessInput(uie, eventType, jsEventArg);
        }
    }

    internal static void ProcessPointerInputNative(string id, int eventId, object jsEventArg, bool isTouchEvent, double pageX, double pageY, int keyModifiers)
        => Current.ProcessPointerInput(id, eventId, jsEventArg, isTouchEvent, pageX, pageY, keyModifiers);

    private void ProcessPointerInput(string id, int eventId, object jsEventArg, bool isTouchEvent, double pageX, double pageY, int keyModifiers)
    {
        using (DisableProcessing())
        {
            UIElement uie = INTERNAL_HtmlDomManager.GetElementById(id);
            EVENTS eventType = (EVENTS)eventId;

            if (uie is not null)
            {
                ActiveWindow = uie.ParentWindow;
            }

            UpdateMostRecentDevice(eventType);

            _primaryMouseDevice.ProcessInput(
                uie,
                eventType,
                new PointerCallbackParameters(isTouchEvent, pageX, pageY, (ModifierKeys)keyModifiers, jsEventArg));
        }
    }

    private void UpdateMostRecentDevice(EVENTS eventType)
    {
        if (eventType is EVENTS.KEYDOWN or EVENTS.KEYUP)
        {
            MostRecentInputDevice = _primaryKeyboardDevice;
        }
        else if (eventType is EVENTS.WHEEL or
                 EVENTS.POINTER_LEFT_DOWN or EVENTS.POINTER_LEFT_UP or
                 EVENTS.POINTER_RIGHT_DOWN or EVENTS.POINTER_RIGHT_UP or
                 EVENTS.POINTER_MIDDLE_DOWN or EVENTS.POINTER_MIDDLE_UP)
        {
            MostRecentInputDevice = _primaryMouseDevice;
        }
    }

    private sealed class BrowserKeyboardDevice : KeyboardDevice
    {
        public BrowserKeyboardDevice(InputManager inputManager)
            : base(inputManager)
        {
        }

        internal override ModifierKeys GetModifiers() =>
            (ModifierKeys)OpenSilver.Interop.ExecuteJavaScriptInt32("osjs.inputManager.getModifiers()", false);

        internal override bool MoveFocus(UIElement newFocus, UIElement oldFocus)
        {
            if (newFocus is null)
            {
                ClearFocusNative();
                ClearTabIndex(oldFocus);
                return true;
            }

            bool focusAcquired = false;

            if (newFocus.GetFocusTarget() is { IsConnected: true } element)
            {
                focusAcquired = SetFocusNative(element);

                if (focusAcquired)
                {
                    ClearTabIndex(oldFocus);
                }
                else
                {
                    ClearTabIndex(newFocus);
                }
            }

            return focusAcquired;
        }

        private static void ClearTabIndex(UIElement uie)
        {
            if (uie is null) return;

            if (uie.GetFocusTarget() is { IsConnected: true } element)
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
    }

    private sealed class BrowserMouseDevice : MouseDevice
    {
        private Cursor _cursor = Cursors.Arrow;
        private bool _forceCursor = false;

        public BrowserMouseDevice(InputManager inputManager)
           : base(inputManager)
        {
        }

        internal override bool SetCursorNative(Cursor cursor, bool forceCursor)
        {
            if (_cursor == cursor && _forceCursor == forceCursor)
            {
                return true;
            }

            bool success = OpenSilver.Interop.ExecuteJavaScriptBoolean(
                $"osjs.inputManager.setCursor('{cursor?.ToHtmlString() ?? string.Empty}', {(forceCursor ? "true" : "false")})");

            if (success)
            {
                _cursor = cursor;
                _forceCursor = forceCursor;
            }

            return success;
        }

        internal override bool SetCaptureNative(UIElement capture)
        {
            if (capture is null)
            {
                OpenSilver.Interop.ExecuteJavaScriptVoid("osjs.inputManager.releasePointerCapture()");
                return true;
            }

            if (capture.OuterDiv is { IsConnected: true } outerDiv)
            {
                return OpenSilver.Interop.ExecuteJavaScriptBoolean($"osjs.inputManager.capturePointer('{outerDiv.Uid}')");
            }

            return false;
        }

        internal override MouseButtonState GetButtonStateFromSystem(MouseButton mouseButton)
        {
            return (MouseButtonState)OpenSilver.Interop.ExecuteJavaScriptInt32(
                $"osjs.inputManager.getPointerButtonState({ToPointerButton(mouseButton)})");
        }

        private static string ToPointerButton(MouseButton mouseButton)
        {
            return mouseButton switch
            {
                MouseButton.Left => "1",
                MouseButton.Right => "2",
                MouseButton.Middle => "4",
                _ => throw new InvalidOperationException(),
            };
        }
    }
}
