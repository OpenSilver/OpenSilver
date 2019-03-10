
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
//
//===============================================================================


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
    public sealed class KeyEventArgs : RoutedEventArgs
#else
    public sealed class KeyRoutedEventArgs : RoutedEventArgs
#endif
    {
        bool _handled = false;
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
            get { return _handled; }
            set { _handled = value; }
        }

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
            if (Convert.ToBoolean(Interop.ExecuteJavaScript("$0.shiftKey || false", jsEventArg))) //Note: we use "||" because the value "shiftKey" may be null or undefined. For more information on "||", read: https://stackoverflow.com/questions/476436/is-there-a-null-coalescing-operator-in-javascript
            {
#if MIGRATION
                keyModifiers = keyModifiers | ModifierKeys.Shift;
#else
                keyModifiers = keyModifiers | VirtualKeyModifiers.Shift;
#endif
            }
            if (Convert.ToBoolean(Interop.ExecuteJavaScript("$0.altKey || false", jsEventArg)))
            {
#if MIGRATION
                keyModifiers = keyModifiers | ModifierKeys.Alt;
#else
                keyModifiers = keyModifiers | VirtualKeyModifiers.Menu;
#endif
            }
            if (Convert.ToBoolean(Interop.ExecuteJavaScript("$0.ctrlKey || false", jsEventArg)))
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
            Interop.ExecuteJavaScript(@"document.refreshKeyModifiers($0);", jsEventArg);

            /*
            if (!CSHTML5.Interop.IsRunningInTheSimulator)
            {
                CSHTML5.Interop.ExecuteJavaScript(@"document.refreshKeyModifiers($0);", jsEventArg);
            }
            else
            {
                dynamic document = INTERNAL_Simulator.HtmlDocument;
                document.Invoke("refreshKeyModifiers", jsEventArg);
            }
             */
        }
    }
}
