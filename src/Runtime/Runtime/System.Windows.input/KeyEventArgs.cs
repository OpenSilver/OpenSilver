
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

using System.ComponentModel;
using CSHTML5;

namespace System.Windows.Input
{
    /// <summary>
    /// Provides data for the KeyUp and KeyDown routed events, as well as related
    /// attached and Preview events.
    /// </summary>
    public sealed class KeyEventArgs : RoutedEventArgs
    {
        internal override void InvokeHandler(Delegate handler, object target)
        {
            ((KeyEventHandler)handler)(target, this);
        }

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

        /// <summary>
        /// Gets or sets a value that determines if the routed event will call <i>preventDefault()</i>
        /// if <see cref="Handled"/> is set to true. The default is true.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool Cancellable { get; set; } = true;

        internal void PreventDefault()
        {
            if (UIEventArg != null)
            {
                OpenSilver.Interop.ExecuteJavaScriptVoid(
                    $"{INTERNAL_InteropImplementation.GetVariableStringForJS(UIEventArg)}.preventDefault();");
            }
        }

        // Returns:
        //     A system value that indicates the code for the key referenced by the event.
        /// <summary>
        /// Gets the keyboard key associated with the event.
        /// </summary>
        public Key Key
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets an integer value that represents the key that is pressed or released
        /// (depending on which event is raised). This value is browser-specific.
        /// </summary>
        public int PlatformKeyCode
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets a value that indicates which key modifiers were active at the time that
        /// the pointer event was initiated.
        /// </summary>
        public ModifierKeys KeyModifiers
        {
            get;
            internal set;
        }
    }
}
