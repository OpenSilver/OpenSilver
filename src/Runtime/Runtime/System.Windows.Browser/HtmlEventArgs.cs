
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
using System.Security;

namespace System.Windows.Browser
{
    /// <summary>
    /// Provides event details to event handlers.
    /// </summary>
    [OpenSilver.NotImplemented]
    public class HtmlEventArgs : EventArgs
    {
        internal HtmlEventArgs() { }

        /// <summary>
        /// Gets the state of the ALT key when the event was raised.
        /// </summary>
        /// <returns>
        /// true if the ALT key was pressed when the event was raised; otherwise, false.
        /// </returns>
        public bool AltKey { get; }

        /// <summary>
        /// Gets the integer Unicode value of a key that was pressed.
        /// </summary>
        /// <returns>
        /// An integer Unicode value of the key that was pressed if the <see cref="EventType"/>
        /// property value is "keypress"; otherwise, 0 (zero).
        /// </returns>
        public int CharacterCode { get; }

        /// <summary>
        /// Gets the x-coordinate mouse position in pixels relative to the client area of
        /// the window.
        /// </summary>
        /// <returns>
        /// The x-coordinate mouse position in pixels if the Document Object Model (DOM)
        /// event includes position information; otherwise, 0 (zero).
        /// </returns>
        public int ClientX { get; }

        /// <summary>
        /// Gets the y-coordinate mouse position in pixels relative to the client area of
        /// the window.
        /// </summary>
        /// <returns>
        /// The y-coordinate mouse position in pixels if the Document Object Model (DOM)
        /// event includes position information; otherwise, 0 (zero).
        /// </returns>
        public int ClientY { get; }

        /// <summary>
        /// Gets the state of the CTRL key when the event was raised.
        /// </summary>
        /// <returns>
        /// true if the CTRL key was pressed when the event was raised; otherwise, false.
        /// </returns>
        public bool CtrlKey { get; }

        /// <summary>
        /// Gets the underlying HTML event object.
        /// </summary>
        /// <returns>
        /// An HTML event object.
        /// </returns>
        public ScriptObject EventObject { get; }

        /// <summary>
        /// Gets the name of the event raised by the browser.
        /// </summary>
        /// <returns>
        /// An event name.
        /// </returns>
        public string EventType { get; }

        /// <summary>
        /// Gets the integer Unicode value of a key that is associated with a keyboard event
        /// other than "keypress".
        /// </summary>
        /// <returns>
        /// The integer Unicode value of a key involved in a keyboard event, if the event
        /// is not "keypress"; otherwise, 0 (zero).
        /// </returns>
        public int KeyCode { get; }

        /// <summary>
        /// Gets the mouse buttons that were clicked at the time the event was raised.
        /// </summary>
        /// <returns>
        /// One of the enumeration values that indicates which mouse button was clicked.
        /// </returns>
        public MouseButtons MouseButton { get; }

        /// <summary>
        /// Gets the x-coordinate mouse position in pixels relative to the HTML object that
        /// raised the event.
        /// </summary>
        /// <returns>
        /// The x-coordinate mouse position in pixels if the Document Object Model (DOM)
        /// event includes position information; otherwise, 0 (zero).
        /// </returns>
        public int OffsetX { get; }

        /// <summary>
        /// Gets the y-coordinate mouse position in pixels relative to the HTML object that
        /// raised the event.
        /// </summary>
        /// <returns>
        /// The y-coordinate mouse position in pixels if the Document Object Model (DOM)
        /// event includes position information; otherwise, 0 (zero).
        /// </returns>
        public int OffsetY { get; }

        /// <summary>
        /// Gets the x-coordinate mouse position in pixels relative to the user's screen.
        /// </summary>
        /// <returns>
        /// The x-coordinate mouse position in pixels if the Document Object Model (DOM)
        /// event includes position information; otherwise, 0 (zero).
        /// </returns>
        public int ScreenX { get; }

        /// <summary>
        /// Gets the y-coordinate mouse position in pixels relative to the user's screen.
        /// </summary>
        /// <returns>
        /// The y-coordinate mouse position in pixels if the Document Object Model (DOM)
        /// event includes position information; otherwise, 0 (zero).
        /// </returns>
        public int ScreenY { get; }

        /// <summary>
        /// Gets the state of the SHIFT key when the event was raised.
        /// </summary>
        /// <returns>
        /// true if the SHIFT key was pressed when the event was raised; otherwise, false.
        /// </returns>
        public bool ShiftKey { get; }

        /// <summary>
        /// Gets a reference to the Document Object Model (DOM) element or object that raised
        /// the event.
        /// </summary>
        /// <returns>
        /// A reference to the DOM element or object that raised the event.
        /// </returns>
        public HtmlObject Source { get; }

        /// <summary>
        /// Specifies that the associated Document Object Model (DOM) element should not
        /// perform the default action for the current event.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// An unexpected error occurred.
        /// </exception>
        [SecuritySafeCritical]
        public void PreventDefault() { }

        /// <summary>
        /// Specifies that the event should not bubble up the Document Object Model (DOM)
        /// hierarchy.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// An unexpected error occurred.
        /// </exception>
        [SecuritySafeCritical]
        public void StopPropagation() { }
    }
}