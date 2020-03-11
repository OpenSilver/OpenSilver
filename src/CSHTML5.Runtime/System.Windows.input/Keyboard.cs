

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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if !MIGRATION
using Windows.System;
#endif

namespace System.Windows.Input
{
    /// <summary>
    /// Represents the keyboard device.
    /// </summary>
    public static class Keyboard
    {
        /// <summary>
        /// Gets the set of System.Windows.Input.ModifierKeys that are currently pressed.
        /// </summary>
#if MIGRATION
        public static ModifierKeys Modifiers
#else
        public static VirtualKeyModifiers Modifiers
#endif
        {
            get
            {
                int value = Convert.ToInt32(CSHTML5.Interop.ExecuteJavaScript("document.modifiersPressed"));
#if MIGRATION
                return (ModifierKeys)value;
#else
                return (VirtualKeyModifiers)value;
#endif
            }
        }
       
        #region Not implemented stuff

        //// Summary:
        ////     Identifies the System.Windows.Input.Keyboard.GotKeyboardFocus attached event.
        ////
        //// Returns:
        ////     The identifier for the System.Windows.Input.Keyboard.GotKeyboardFocus attached
        ////     event.
        //public static readonly RoutedEvent GotKeyboardFocusEvent;
        ////
        //// Summary:
        ////     Identifies the System.Windows.Input.Keyboard.KeyboardInputProviderAcquireFocus attached
        ////     event.
        ////
        //// Returns:
        ////     The identifier for the System.Windows.Input.Keyboard.KeyboardInputProviderAcquireFocus attached
        ////     event.
        //public static readonly RoutedEvent KeyboardInputProviderAcquireFocusEvent;
        ////
        //// Summary:
        ////     Identifies the System.Windows.Input.Keyboard.KeyDown attached event.
        ////
        //// Returns:
        ////     The identifier for the System.Windows.Input.Keyboard.KeyDown attached event.
        //public static readonly RoutedEvent KeyDownEvent;
        ////
        //// Summary:
        ////     Identifies the System.Windows.Input.Keyboard.KeyUp attached event.
        ////
        //// Returns:
        ////     The identifier for the System.Windows.Input.Keyboard.KeyUp attached event.
        //public static readonly RoutedEvent KeyUpEvent;
        ////
        //// Summary:
        ////     Identifies the System.Windows.Input.Keyboard.LostKeyboardFocus attached event.
        ////
        //// Returns:
        ////     The identifier for the System.Windows.Input.Keyboard.LostKeyboardFocus attached
        ////     event.
        //public static readonly RoutedEvent LostKeyboardFocusEvent;
        ////
        //// Summary:
        ////     Identifies the System.Windows.Input.Keyboard.PreviewGotKeyboardFocus attached
        ////     event.
        ////
        //// Returns:
        ////     The identifier for the System.Windows.Input.Keyboard.PreviewGotKeyboardFocus attached
        ////     event.
        //public static readonly RoutedEvent PreviewGotKeyboardFocusEvent;
        ////
        //// Summary:
        ////     Identifies the System.Windows.Input.Keyboard.PreviewKeyboardInputProviderAcquireFocus attached
        ////     event.
        ////
        //// Returns:
        ////     The identifier for the System.Windows.Input.Keyboard.PreviewKeyboardInputProviderAcquireFocus attached
        ////     event.
        //public static readonly RoutedEvent PreviewKeyboardInputProviderAcquireFocusEvent;
        ////
        //// Summary:
        ////     Identifies the System.Windows.Input.Keyboard.PreviewKeyDown attached event.
        ////
        //// Returns:
        ////     The identifier for the System.Windows.Input.Keyboard.PreviewKeyDown attached
        ////     event.
        //public static readonly RoutedEvent PreviewKeyDownEvent;
        ////
        //// Summary:
        ////     Identifies the System.Windows.Input.Keyboard.PreviewKeyUp attached event.
        ////
        //// Returns:
        ////     The identifier for the System.Windows.Input.Keyboard.PreviewKeyUp attached
        ////     event.
        //public static readonly RoutedEvent PreviewKeyUpEvent;
        ////
        //// Summary:
        ////     Identifies the System.Windows.Input.Keyboard.PreviewLostKeyboardFocus attached
        ////     event.
        ////
        //// Returns:
        ////     The identifier for the System.Windows.Input.Keyboard.PreviewLostKeyboardFocus
        ////      attached event.
        //public static readonly RoutedEvent PreviewLostKeyboardFocusEvent;

        //// Summary:
        ////     Gets or sets the behavior of Windows Presentation Foundation (WPF) when restoring
        ////     focus.
        ////
        //// Returns:
        ////     An enumeration value that specifies the behavior of WPF when restoring focus.
        ////     The default in System.Windows.Input.RestoreFocusMode.Auto.
        //public static RestoreFocusMode DefaultRestoreFocusMode { get; set; }
        ////
        //// Summary:
        ////     Gets the element that has keyboard focus.
        ////
        //// Returns:
        ////     The focused element.
        //public static IInputElement FocusedElement { get; }
        
        ////
        //// Summary:
        ////     Gets the primary keyboard input device.
        ////
        //// Returns:
        ////     The device.
        //public static KeyboardDevice PrimaryDevice { get; }

        //// Summary:
        ////     Adds a handler for the System.Windows.Input.Keyboard.GotKeyboardFocus attached
        ////     event.
        ////
        //// Parameters:
        ////   element:
        ////     The System.Windows.UIElement or System.Windows.ContentElement that listens
        ////     to this event.
        ////
        ////   handler:
        ////     The event handler to be added.
        //public static void AddGotKeyboardFocusHandler(DependencyObject element, KeyboardFocusChangedEventHandler handler);
        ////
        //// Summary:
        ////     Adds a handler for the System.Windows.Input.Keyboard.KeyboardInputProviderAcquireFocus attached
        ////     event.
        ////
        //// Parameters:
        ////   element:
        ////     The System.Windows.UIElement or System.Windows.ContentElement that listens
        ////     to this event.
        ////
        ////   handler:
        ////     The event handler to be added.
        //public static void AddKeyboardInputProviderAcquireFocusHandler(DependencyObject element, KeyboardInputProviderAcquireFocusEventHandler handler);
        ////
        //// Summary:
        ////     Adds a handler for the System.Windows.Input.Keyboard.KeyDown attached event.
        ////
        //// Parameters:
        ////   element:
        ////     The System.Windows.UIElement or System.Windows.ContentElement that listens
        ////     to this event.
        ////
        ////   handler:
        ////     The event handler to be added.
        //public static void AddKeyDownHandler(DependencyObject element, KeyEventHandler handler);
        ////
        //// Summary:
        ////     Adds a handler for the System.Windows.Input.Keyboard.KeyUp attached event.
        ////
        //// Parameters:
        ////   element:
        ////     The System.Windows.UIElement or System.Windows.ContentElement that listens
        ////     to this event.
        ////
        ////   handler:
        ////     The event handler to be added.
        //public static void AddKeyUpHandler(DependencyObject element, KeyEventHandler handler);
        ////
        //// Summary:
        ////     Adds a handler for the System.Windows.Input.Keyboard.LostKeyboardFocus attached
        ////     event.
        ////
        //// Parameters:
        ////   element:
        ////     The System.Windows.UIElement or System.Windows.ContentElement that listens
        ////     to this event.
        ////
        ////   handler:
        ////     The event handler to be added.
        //public static void AddLostKeyboardFocusHandler(DependencyObject element, KeyboardFocusChangedEventHandler handler);
        ////
        //// Summary:
        ////     Adds a handler for the System.Windows.Input.Keyboard.PreviewGotKeyboardFocus attached
        ////     event.
        ////
        //// Parameters:
        ////   element:
        ////     The System.Windows.UIElement or System.Windows.ContentElement that listens
        ////     to this event.
        ////
        ////   handler:
        ////     The event handler to be added.
        //public static void AddPreviewGotKeyboardFocusHandler(DependencyObject element, KeyboardFocusChangedEventHandler handler);
        ////
        //// Summary:
        ////     Adds a handler for the System.Windows.Input.Keyboard.PreviewKeyboardInputProviderAcquireFocus attached
        ////     event.
        ////
        //// Parameters:
        ////   element:
        ////     The System.Windows.UIElement or System.Windows.ContentElement that listens
        ////     to this event.
        ////
        ////   handler:
        ////     The event handler to be added.
        //public static void AddPreviewKeyboardInputProviderAcquireFocusHandler(DependencyObject element, KeyboardInputProviderAcquireFocusEventHandler handler);
        ////
        //// Summary:
        ////     Adds a handler for the System.Windows.Input.Keyboard.PreviewKeyDown attached
        ////     event.
        ////
        //// Parameters:
        ////   element:
        ////     The System.Windows.UIElement or System.Windows.ContentElement that listens
        ////     to this event.
        ////
        ////   handler:
        ////     The event handler to be added.
        //public static void AddPreviewKeyDownHandler(DependencyObject element, KeyEventHandler handler);
        ////
        //// Summary:
        ////     Adds a handler for the System.Windows.Input.Keyboard.PreviewKeyUp attached
        ////     event.
        ////
        //// Parameters:
        ////   element:
        ////     The System.Windows.UIElement or System.Windows.ContentElement that listens
        ////     to this event.
        ////
        ////   handler:
        ////     The event handler to be added.
        //public static void AddPreviewKeyUpHandler(DependencyObject element, KeyEventHandler handler);
        ////
        //// Summary:
        ////     Adds a handler for the System.Windows.Input.Keyboard.PreviewLostKeyboardFocus attached
        ////     event.
        ////
        //// Parameters:
        ////   element:
        ////     The System.Windows.UIElement or System.Windows.ContentElement that listens
        ////     to this event.
        ////
        ////   handler:
        ////     The event handler to be added.
        //public static void AddPreviewLostKeyboardFocusHandler(DependencyObject element, KeyboardFocusChangedEventHandler handler);
        ////
        //// Summary:
        ////     Clears focus.
        //public static void ClearFocus();
        ////
        //// Summary:
        ////     Sets keyboard focus on the specified element.
        ////
        //// Parameters:
        ////   element:
        ////     The element on which to set keyboard focus.
        ////
        //// Returns:
        ////     The element with keyboard focus.
        //public static IInputElement Focus(IInputElement element);
        ////
        //// Summary:
        ////     Gets the set of key states for the specified key.
        ////
        //// Parameters:
        ////   key:
        ////     The specified key.
        ////
        //// Returns:
        ////     A bitwise combination of the System.Windows.Input.KeyStates values.
        //public static KeyStates GetKeyStates(Key key);
        ////
        //// Summary:
        ////     Determines whether the specified key is pressed.
        ////
        //// Parameters:
        ////   key:
        ////     The specified key.
        ////
        //// Returns:
        ////     true if key is in the down state; otherwise, false.
        //public static bool IsKeyDown(Key key);
        ////
        //// Summary:
        ////     Determines whether the specified key is toggled.
        ////
        //// Parameters:
        ////   key:
        ////     The specified key.
        ////
        //// Returns:
        ////     true if key is in the toggled state; otherwise, false.
        //public static bool IsKeyToggled(Key key);
        ////
        //// Summary:
        ////     Determines whether the specified key is released.
        ////
        //// Parameters:
        ////   key:
        ////     The key to check.
        ////
        //// Returns:
        ////     true if key is in the up state; otherwise, false.
        //public static bool IsKeyUp(Key key);
        ////
        //// Summary:
        ////     Removes a handler for the System.Windows.Input.Keyboard.GotKeyboardFocus attached
        ////     event.
        ////
        //// Parameters:
        ////   element:
        ////     The System.Windows.UIElement or System.Windows.ContentElement that listens
        ////     to this event.
        ////
        ////   handler:
        ////     The event handler to be removed.
        //public static void RemoveGotKeyboardFocusHandler(DependencyObject element, KeyboardFocusChangedEventHandler handler);
        ////
        //// Summary:
        ////     Removes a handler for the System.Windows.Input.Keyboard.KeyboardInputProviderAcquireFocus attached
        ////     event.
        ////
        //// Parameters:
        ////   element:
        ////     The System.Windows.UIElement or System.Windows.ContentElement that listens
        ////     to this event.
        ////
        ////   handler:
        ////     The event handler to be removed.
        //public static void RemoveKeyboardInputProviderAcquireFocusHandler(DependencyObject element, KeyboardInputProviderAcquireFocusEventHandler handler);
        ////
        //// Summary:
        ////     Removes a handler for the System.Windows.Input.Keyboard.KeyDown attached
        ////     event.
        ////
        //// Parameters:
        ////   element:
        ////     The System.Windows.UIElement or System.Windows.ContentElement that listens
        ////     to this event.
        ////
        ////   handler:
        ////     The event handler to be removed.
        //public static void RemoveKeyDownHandler(DependencyObject element, KeyEventHandler handler);
        ////
        //// Summary:
        ////     Removes a handler for the System.Windows.Input.Keyboard.KeyUp attached event.
        ////
        //// Parameters:
        ////   element:
        ////     The System.Windows.UIElement or System.Windows.ContentElement that listens
        ////     to this event.
        ////
        ////   handler:
        ////     The event handler to be removed.
        //public static void RemoveKeyUpHandler(DependencyObject element, KeyEventHandler handler);
        ////
        //// Summary:
        ////     Removes a handler for the System.Windows.Input.Keyboard.LostKeyboardFocus attached
        ////     event.
        ////
        //// Parameters:
        ////   element:
        ////     The System.Windows.UIElement or System.Windows.ContentElement that listens
        ////     to this event.
        ////
        ////   handler:
        ////     The event handler to be removed.
        //public static void RemoveLostKeyboardFocusHandler(DependencyObject element, KeyboardFocusChangedEventHandler handler);
        ////
        //// Summary:
        ////     Removes a handler for the System.Windows.Input.Keyboard.PreviewGotKeyboardFocus attached
        ////     event.
        ////
        //// Parameters:
        ////   element:
        ////     The System.Windows.UIElement or System.Windows.ContentElement that listens
        ////     to this event.
        ////
        ////   handler:
        ////     The event handler to be removed.
        //public static void RemovePreviewGotKeyboardFocusHandler(DependencyObject element, KeyboardFocusChangedEventHandler handler);
        ////
        //// Summary:
        ////     Removes a handler for the System.Windows.Input.Keyboard.PreviewKeyboardInputProviderAcquireFocus attached
        ////     event.
        ////
        //// Parameters:
        ////   element:
        ////     The System.Windows.UIElement or System.Windows.ContentElement that listens
        ////     to this event.
        ////
        ////   handler:
        ////     The event handler to be removed.
        //public static void RemovePreviewKeyboardInputProviderAcquireFocusHandler(DependencyObject element, KeyboardInputProviderAcquireFocusEventHandler handler);
        ////
        //// Summary:
        ////     Removes a handler for the System.Windows.Input.Keyboard.PreviewKeyDown attached
        ////     event.
        ////
        //// Parameters:
        ////   element:
        ////     The System.Windows.UIElement or System.Windows.ContentElement that listens
        ////     to this event.
        ////
        ////   handler:
        ////     The event handler to be removed.
        //public static void RemovePreviewKeyDownHandler(DependencyObject element, KeyEventHandler handler);
        ////
        //// Summary:
        ////     Removes a handler for the System.Windows.Input.Keyboard.PreviewKeyUp attached
        ////     event.
        ////
        //// Parameters:
        ////   element:
        ////     The System.Windows.UIElement or System.Windows.ContentElement that listens
        ////     to this event.
        ////
        ////   handler:
        ////     The event handler to be removed.
        //public static void RemovePreviewKeyUpHandler(DependencyObject element, KeyEventHandler handler);
        ////
        //// Summary:
        ////     Removes a handler for the System.Windows.Input.Keyboard.PreviewLostKeyboardFocus attached
        ////     event.
        ////
        //// Parameters:
        ////   element:
        ////     The System.Windows.UIElement or System.Windows.ContentElement that listens
        ////     to this event.
        ////
        ////   handler:
        ////     The event handler to be removed.
        //public static void RemovePreviewLostKeyboardFocusHandler(DependencyObject element, KeyboardFocusChangedEventHandler handler);

        #endregion
    }
}
