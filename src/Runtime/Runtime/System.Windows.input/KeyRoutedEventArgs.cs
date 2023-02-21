

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


using CSHTML5;
using CSHTML5.Internal;
#if !MIGRATION
using Windows.System;
using System;
#endif

#if MIGRATION
namespace System.Windows.Input
#else
namespace Windows.UI.Xaml.Input
#endif
{
    /// <summary>
    /// Provides data for the KeyUp and KeyDown routed events, as well as related
    /// attached and Preview events.
    /// </summary>
#if MIGRATION
    public sealed partial class KeyEventArgs : RoutedEventArgs
#else
    public sealed partial class KeyRoutedEventArgs : RoutedEventArgs
#endif
    {
        internal override void InvokeHandler(Delegate handler, object target)
        {
            ((KeyEventHandler)handler)(target, this);
        }

#if MIGRATION
        Key _key;
#else
        VirtualKey _key;
#endif
        int _platformKeyCode;

        /// <summary>
        /// Gets or sets a value that marks the routed event as handled. A true value
        /// for Handled prevents most handlers along the event route from handling the
        /// same event again.
        /// </summary>
        public bool Handled
        {
            get => HandledImpl;
            set => HandledImpl = value;
        }

        internal bool Cancellable { get; set; } = true;

        internal bool PreventDefault => Handled && Cancellable;

        // Returns:
        //     A system value that indicates the code for the key referenced by the event.
        /// <summary>
        /// Gets the keyboard key associated with the event.
        /// </summary>
#if MIGRATION
        public Key Key
#else
        public VirtualKey Key
#endif
        {
            get { return _key; }
            internal set { _key = value; }
        }

        /// <summary>
        /// Gets an integer value that represents the key that is pressed or released
        /// (depending on which event is raised). This value is browser-specific.
        /// </summary>
        public int PlatformKeyCode
        {
            get { return _platformKeyCode; }
            internal set { _platformKeyCode = value; }
        }

#if MIGRATION
        ModifierKeys _keyModifiers;
#else
        VirtualKeyModifiers _keyModifiers;
#endif
        /// <summary>
        /// Gets a value that indicates which key modifiers were active at the time that
        /// the pointer event was initiated.
        /// </summary>
#if MIGRATION
        public ModifierKeys KeyModifiers
#else
        public VirtualKeyModifiers KeyModifiers
#endif
        {
            get { return _keyModifiers; }
            internal set { _keyModifiers = value; }
        }

        internal void AddKeyModifiersAndUpdateDocumentValue(object jsEventArg)
        {
#if MIGRATION
            ModifierKeys keyModifiers = ModifierKeys.None;
#else
            VirtualKeyModifiers keyModifiers = VirtualKeyModifiers.None;
#endif
            string sEventArg = INTERNAL_InteropImplementation.GetVariableStringForJS(jsEventArg);
            if (OpenSilver.Interop.ExecuteJavaScriptBoolean($"{sEventArg}.shiftKey || false"))
            {
#if MIGRATION
                keyModifiers = keyModifiers | ModifierKeys.Shift;
#else
                keyModifiers = keyModifiers | VirtualKeyModifiers.Shift;
#endif
            }
            if (OpenSilver.Interop.ExecuteJavaScriptBoolean($"{sEventArg}.altKey || false"))
            {
#if MIGRATION
                keyModifiers = keyModifiers | ModifierKeys.Alt;
#else
                keyModifiers = keyModifiers | VirtualKeyModifiers.Menu;
#endif
            }
            if (OpenSilver.Interop.ExecuteJavaScriptBoolean($"{sEventArg}.ctrlKey || false"))
            {
#if MIGRATION
                keyModifiers = keyModifiers | ModifierKeys.Control;
#else
                keyModifiers = keyModifiers | VirtualKeyModifiers.Control;
#endif
            }
            KeyModifiers = keyModifiers;

            //Refreshing the value for key modifiers in the html document to ensure the value is correct when sending the event (For cases where the users use Keyboard.Modifiers inside the event's handler).
            //Note: this is mandatory because we have no way to be sure the events document.onkeyup and document.onkeydown are thrown before the one that made us arrive here.
            OpenSilver.Interop.ExecuteJavaScriptVoid($"document.refreshKeyModifiers({sEventArg});");
        }
    }
}
