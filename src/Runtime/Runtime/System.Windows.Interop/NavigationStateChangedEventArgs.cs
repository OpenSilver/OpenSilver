
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

namespace System.Windows.Interop
{
    /// <summary>
    /// Provides data for the <see cref="Host.NavigationStateChanged"/>
    /// event.
    /// </summary>
    public class NavigationStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationStateChangedEventArgs"/>
        /// class.
        /// </summary>
        /// <param name="previousNavigationState">
        /// The URI fragment that represents the previous navigation state.
        /// </param>
        /// <param name="newNavigationState">
        /// The URI fragment that represents the new navigation state.
        /// </param>
        public NavigationStateChangedEventArgs(string previousNavigationState, string newNavigationState)
        {
            NewNavigationState = newNavigationState;
            PreviousNavigationState = previousNavigationState;
        }

        /// <summary>
        /// Gets the URI fragment that represents the new navigation state.
        /// </summary>
        public string NewNavigationState { get; }

        /// <summary>
        /// Gets the URI fragment that represents the previous navigation state.
        /// </summary>
        public string PreviousNavigationState { get; }
    }
}