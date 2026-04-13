
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
using OpenSilver.Internal.Controls;
using System.Windows.Controls;
using System.Windows.Media;
using EVENTS = System.Windows.Input.InputManager.EVENTS;

namespace System.Windows.Input;

/// <summary>
/// Abstract class that represents a keyboard device.
/// </summary>
public abstract class KeyboardDevice : InputDevice
{
    private readonly InputManager _inputManager;
    private UIElement _focus;
    private UIElement _restoreFocus;

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyboardDevice"/> class.
    /// </summary>
    /// <param name="inputManager">
    /// The input manager associated with this <see cref="KeyboardDevice"/>.
    /// </param>
    internal KeyboardDevice(InputManager inputManager)
    {
        ArgumentNullException.ThrowIfNull(inputManager);
        _inputManager = inputManager;
    }

    /// <summary>
    /// Gets the specified <see cref="IInputElement"/> that input from this device is sent to.
    /// </summary>
    /// <returns>
    /// The element that receives input.
    /// </returns>
    public override IInputElement Target => FocusedElement;

    /// <summary>
    /// Gets the element that has keyboard focus.
    /// </summary>
    /// <returns>
    /// The element with keyboard focus.
    /// </returns>
    public IInputElement FocusedElement => _focus;

    /// <summary>
    /// Gets the set of <see cref="ModifierKeys"/> which are currently pressed.
    /// </summary>
    /// <returns>
    /// The set of modifier keys.
    /// </returns>
    public ModifierKeys Modifiers => GetModifiers();

    /// <summary>
    /// Clears focus.
    /// </summary>
    public void ClearFocus() => Focus(null, false, false, false);

    /// <summary>
    /// Sets keyboard focus on the specified <see cref="IInputElement"/>.
    /// </summary>
    /// <param name="element">
    /// The element to move focus to.
    /// </param>
    /// <returns>
    /// The element that has keyboard focus.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// element is not a <see cref="UIElement"/>.
    /// </exception>
    public IInputElement Focus(IInputElement element)
    {
        UIElement oFocus = element switch
        {
            UIElement uie => uie,
            null => null,
            _ => throw new InvalidOperationException(string.Format(Strings.Invalid_IInputElement, element.GetType())),
        };

        bool forceToNullIfFailed = false;

        if (oFocus is null && _inputManager.ActiveWindow is not null)
        {
            oFocus = _inputManager.ActiveWindow;
            forceToNullIfFailed = true;
        }

        Focus(oFocus, true, true, forceToNullIfFailed);

        return _focus;
    }

    private void Focus(UIElement focus, bool askOld, bool askNew, bool forceToNullIfFailed)
    {
        if (_focus == focus)
        {
            return;
        }

        bool isValid = true;

        if (focus is not null)
        {
            isValid = Keyboard.IsFocusable(focus);
            if (!isValid && forceToNullIfFailed)
            {
                focus = null;
                isValid = true;
            }
        }

        if (isValid)
        {
            TryChangeFocus(focus, askOld, askNew, forceToNullIfFailed);
        }
    }

    private void TryChangeFocus(UIElement newFocus, bool askOld, bool askNew, bool forceToNullIfFailed)
    {
        int timestamp = Environment.TickCount;

        UIElement oldFocus = _focus;
        bool changeFocus = true;

        if (askOld && oldFocus is not null)
        {
            var previewLostFocus = new KeyboardFocusChangedEventArgs(this, timestamp, oldFocus, newFocus)
            {
                RoutedEvent = Keyboard.PreviewLostKeyboardFocusEvent,
                Source = oldFocus,
            };

            oldFocus.RaiseTrustedEvent(previewLostFocus);

            if (previewLostFocus.Handled)
            {
                changeFocus = false;
            }
        }

        if (askNew && changeFocus && newFocus is not null)
        {
            var previewGotFocus = new KeyboardFocusChangedEventArgs(this, timestamp, oldFocus, newFocus)
            {
                RoutedEvent = Keyboard.PreviewGotKeyboardFocusEvent,
                Source = newFocus,
            };

            newFocus.RaiseTrustedEvent(previewGotFocus);

            if (previewGotFocus.Handled)
            {
                changeFocus = false;
            }
        }

        if (changeFocus)
        {
            if (newFocus is not null)
            {
                var acquireFocus = new KeyboardInputProviderAcquireFocusEventArgs(this, timestamp, changeFocus)
                {
                    RoutedEvent = Keyboard.PreviewKeyboardInputProviderAcquireFocusEvent,
                    Source = newFocus,
                };

                newFocus.RaiseTrustedEvent(acquireFocus);
            }

            changeFocus = MoveFocus(newFocus, _focus);

            if (newFocus is not null)
            {
                var acquireFocus = new KeyboardInputProviderAcquireFocusEventArgs(this, timestamp, changeFocus)
                {
                    RoutedEvent = Keyboard.KeyboardInputProviderAcquireFocusEvent,
                    Source = newFocus,
                };

                newFocus.RaiseTrustedEvent(acquireFocus);
            }
        }

        if (!changeFocus && forceToNullIfFailed && oldFocus == _focus)
        {
            newFocus = null;
            changeFocus = true;
        }

        if (changeFocus)
        {
            ChangeFocus(newFocus, timestamp);
        }
    }

    private void ChangeFocus(UIElement focus, int timestamp)
    {
        if (focus != _focus)
        {
            UIElement oldFocus = _focus;
            _focus = focus;

            // Invalidate the IsKeyboardFocused properties.
            oldFocus?.SetValueInternal(UIElement.IsKeyboardFocusedPropertyKey, false);
            _focus?.SetValueInternal(UIElement.IsKeyboardFocusedPropertyKey, true);

            // Send the LostKeyboardFocus and GotKeyboardFocus events.
            if (oldFocus is not null)
            {
                var lostFocus = new KeyboardFocusChangedEventArgs(this, timestamp, oldFocus, focus)
                {
                    RoutedEvent = Keyboard.LostKeyboardFocusEvent,
                    Source = oldFocus,
                };

                oldFocus.RaiseTrustedEvent(lostFocus);
            }

            if (_focus is not null)
            {
                var gotFocus = new KeyboardFocusChangedEventArgs(this, timestamp, oldFocus, _focus)
                {
                    RoutedEvent = Keyboard.GotKeyboardFocusEvent,
                    Source = _focus,
                };

                _focus.RaiseTrustedEvent(gotFocus);
            }

            using (_inputManager.DisableProcessing())
            {
                if (oldFocus is not null)
                {
                    _inputManager.PushInput(new RoutedEventArgs(UIElement.LostFocusEvent, oldFocus));
                }

                if (_focus is not null)
                {
                    _inputManager.PushInput(new RoutedEventArgs(UIElement.GotFocusEvent, _focus));
                }
            }
        }
    }

    internal void ReevaluateFocus()
    {
        if (_focus is null || Keyboard.IsFocusable(_focus))
        {
            return;
        }

        Focus(_inputManager.ActiveWindow, false, true, true);
    }

    internal abstract ModifierKeys GetModifiers();

    internal abstract bool MoveFocus(UIElement newFocus, UIElement oldFocus);

    internal void ProcessInput(UIElement uie, EVENTS eventType, object jsEventArg)
    {
        if (uie is null)
        {
            ProcessUnmappedInput(eventType, jsEventArg);
        }
        else
        {
            DispatchEvent(uie, eventType, jsEventArg);
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

            case EVENTS.FOCUS_IN:
                OnFocusIn(uie, jsEventArg);
                break;
        }
    }

    private void ProcessUnmappedInput(EVENTS eventType, object jsEventArg)
    {
        switch (eventType)
        {
            case EVENTS.FOCUS_OUT:
                OnFocusOut();
                break;

            case EVENTS.WINDOW_FOCUS:
                OnWindowFocus(jsEventArg);
                break;

            case EVENTS.WINDOW_BLUR:
                OnWindowBlur(jsEventArg);
                break;
        }
    }

    private void ProcessOnKeyDown(UIElement uie, object jsEventArg)
    {
        uint nativeKeyCode = OpenSilver.Interop.ExecuteJavaScriptUInt32(
            $"{OpenSilver.Interop.GetVariableStringForJS(jsEventArg)}.keyCode", false);

        if (nativeKeyCode > int.MaxValue)
        {
            return;
        }

        UIElement source = GetEventSource(uie);

        int keyCode = VirtualKeysHelpers.FixKeyCodeForSilverlight((int)nativeKeyCode);
        Key key = VirtualKeysHelpers.GetKeyFromKeyCode(keyCode);
        ModifierKeys modifiers = Keyboard.Modifiers;
        int timestamp = Environment.TickCount;

        ToolTipService.OnKeyDown(key);

        var previewKeyDown = new KeyEventArgs(this, timestamp, key)
        {
            RoutedEvent = Keyboard.PreviewKeyDownEvent,
            Source = source,
            PlatformKeyCode = keyCode,
            KeyModifiers = modifiers,
            UIEventArg = jsEventArg,
        };

        source.RaiseTrustedEvent(previewKeyDown);

        if (previewKeyDown.Handled)
        {
            previewKeyDown.PreventDefault();
            return;
        }

        var keyDown = new KeyEventArgs(this, timestamp, key)
        {
            RoutedEvent = Keyboard.KeyDownEvent,
            Source = source,
            PlatformKeyCode = keyCode,
            KeyModifiers = modifiers,
            UIEventArg = jsEventArg,
        };

        source.RaiseTrustedEvent(keyDown);

        KeyboardNavigation.Current.ProcessInput(keyDown);

        if (keyDown.Handled)
        {
            keyDown.PreventDefault();
        }
    }

    private void ProcessOnKeyUp(UIElement uie, object jsEventArg)
    {
        uint nativeKeyCode = OpenSilver.Interop.ExecuteJavaScriptUInt32(
            $"{OpenSilver.Interop.GetVariableStringForJS(jsEventArg)}.keyCode", false);

        if (nativeKeyCode > int.MaxValue)
        {
            return;
        }

        UIElement source = GetEventSource(uie);

        int keyCode = VirtualKeysHelpers.FixKeyCodeForSilverlight((int)nativeKeyCode);
        Key key = VirtualKeysHelpers.GetKeyFromKeyCode(keyCode);
        ModifierKeys modifiers = Keyboard.Modifiers;
        int timestamp = Environment.TickCount;

        var previewKeyUp = new KeyEventArgs(this, timestamp, key)
        {
            RoutedEvent = Keyboard.PreviewKeyUpEvent,
            Source = source,
            PlatformKeyCode = keyCode,
            KeyModifiers = modifiers,
            UIEventArg = jsEventArg,
        };

        source.RaiseTrustedEvent(previewKeyUp);

        if (previewKeyUp.Handled)
        {
            return;
        }

        var keyUp = new KeyEventArgs(this, timestamp, key)
        {
            RoutedEvent = Keyboard.KeyUpEvent,
            Source = source,
            PlatformKeyCode = keyCode,
            KeyModifiers = modifiers,
            UIEventArg = jsEventArg,
        };

        source.RaiseTrustedEvent(keyUp);

        CommandManager.InvalidateRequerySuggested();
    }

    private void ProcessOnKeyPress(UIElement uie, object jsEventArg)
    {
        uint nativeKeyCode = OpenSilver.Interop.ExecuteJavaScriptUInt32(
            $"{OpenSilver.Interop.GetVariableStringForJS(jsEventArg)}.keyCode", false);

        if (nativeKeyCode > ushort.MaxValue)
        {
            return;
        }

        UIElement source = GetEventSource(uie);

        string text = ((char)nativeKeyCode).ToString();

        var textInputStartArgs = new TextCompositionEventArgs
        {
            RoutedEvent = UIElement.TextInputStartEvent,
            Source = source,
            Text = text,
            TextComposition = TextComposition.Empty,
            UIEventArg = jsEventArg,
        };

        source.RaiseTrustedEvent(textInputStartArgs);

        var textInputArgs = new TextCompositionEventArgs
        {
            RoutedEvent = UIElement.TextInputEvent,
            Source = source,
            Text = text,
            TextComposition = TextComposition.Empty,
            UIEventArg = jsEventArg,
        };

        source.RaiseTrustedEvent(textInputArgs);

        if (textInputArgs.Cancel)
        {
            textInputArgs.PreventDefault();
        }
    }

    private void OnFocusIn(UIElement uie, object jsEventArg)
    {
        UIElement focus = FindLogicalFocus(GetEventSource(uie));
        Focus(focus, false, false, true);

        static UIElement FindLogicalFocus(UIElement uie)
        {
            while (uie is not null && !KeyboardNavigation.Current.IsTabStop(uie))
            {
                uie = (UIElement)VisualTreeHelper.GetParent(uie);
            }

            return uie;
        }
    }

    private void OnFocusOut()
    {
        // Focus moved to a non managed element, clear focus
        ChangeFocus(null, Environment.TickCount);
    }

    private void OnWindowFocus(object jsEventArg)
    {
        if (_restoreFocus is UIElement restoreFocus)
        {
            _restoreFocus = null;
            Focus(restoreFocus);
        }
    }

    private void OnWindowBlur(object jsEventArg)
    {
        _restoreFocus = _focus;
        ChangeFocus(null, Environment.TickCount);
    }

    private static UIElement GetEventSource(UIElement uie) => uie is TextViewBase textViewBase ? textViewBase.Host : uie;
}
