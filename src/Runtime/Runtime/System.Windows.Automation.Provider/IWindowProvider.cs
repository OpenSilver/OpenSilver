
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

namespace System.Windows.Automation.Provider
{
    /// <summary>
    /// Exposes methods and properties to support access by a UI automation client to 
    /// controls that provide fundamental window-based functionality within a traditional 
    /// graphical user interface (GUI).
    /// </summary>
    public interface IWindowProvider
    {
        /// <summary>
        /// Closes the window.
        /// </summary>
        void Close();

        /// <summary>
        /// Gets a value that specifies whether the window is modal.
        /// </summary>
        /// <returns>
        /// true if the window is modal; otherwise, false.
        /// </returns>
        bool IsModal { get; }

        /// <summary>
        /// Gets a value that specifies whether the window is the topmost element in the 
        /// z-order of layout.
        /// </summary>
        /// <returns>
        /// true if the window is topmost; otherwise, false.
        /// </returns>
        bool IsTopmost { get; }

        /// <summary>
        /// Gets a value that specifies whether the window can be maximized.
        /// </summary>
        /// <returns>
        /// true if the window can be maximized; otherwise, false.
        /// </returns>
        bool Maximizable { get; }

        /// <summary>
        /// Gets a value that specifies whether the window can be minimized.
        /// </summary>
        /// <returns>
        /// true if the window can be minimized; otherwise, false.
        /// </returns>
        bool Minimizable { get; }

        /// <summary>
        /// Changes the visual state of the window (such as minimizing or maximizing it).
        /// </summary>
        /// <param name="state">
        /// The visual state of the window to change to, as a value of the enumeration.
        /// </param>
        void SetVisualState(WindowVisualState state);

        /// <summary>
        /// Blocks the calling code for the specified time or until the associated process 
        /// enters an idle state, whichever completes first.
        /// </summary>
        /// <param name="milliseconds">
        /// The amount of time, in milliseconds, to wait for the associated process to become 
        /// idle.
        /// </param>
        /// <returns>
        /// true if the window has entered the idle state; false if the timeout occurred.
        /// </returns>
        bool WaitForInputIdle(int milliseconds);

        /// <summary>
        /// Gets the interaction state of the window.
        /// </summary>
        /// <returns>
        /// The interaction state of the control, as a value of the enumeration.
        /// </returns>
        WindowInteractionState InteractionState { get; }

        /// <summary>
        /// Gets the visual state of the window.
        /// </summary>
        /// <returns>
        /// The visual state of the window, as a value of the enumeration.
        /// </returns>
        WindowVisualState VisualState { get; }
    }
}
