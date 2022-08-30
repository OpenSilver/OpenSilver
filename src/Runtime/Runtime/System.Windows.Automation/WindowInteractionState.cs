
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

#if MIGRATION
namespace System.Windows.Automation
#else
namespace Windows.UI.Xaml.Automation
#endif
{
    /// <summary>
    /// Defines values that specify the current state of the window for purposes of user or 
    /// programmatic interaction.
    /// </summary>
    public enum WindowInteractionState
    {
        /// <summary>
        /// The window is running. This does not guarantee that the window is responding or ready 
        /// for user interaction.
        /// </summary>
        Running,
        /// <summary>
        /// The window is closing.
        /// </summary>
        Closing,
        /// <summary>
        /// The window is ready for user interaction.
        /// </summary>
        ReadyForUserInteraction,
        /// <summary>
        /// The window is blocked by a modal window.
        /// </summary>
        BlockedByModalWindow,
        /// <summary>
        /// The window is not responding.
        /// </summary>
        NotResponding,
    }
}
