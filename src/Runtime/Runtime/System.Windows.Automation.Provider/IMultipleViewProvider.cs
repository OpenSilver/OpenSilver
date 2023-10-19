
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
    /// Exposes methods and properties to support UI automation client access to controls 
    /// that provide, and are able to switch between, multiple representations of the same 
    /// set of information or child controls.
    /// </summary>
    public interface IMultipleViewProvider
    {
        /// <summary>
        /// Gets the current control-specific view.
        /// </summary>
        /// <returns>
        /// The value for the current view of the UI automation element.
        /// </returns>
        int CurrentView { get; }

        /// <summary>
        /// Retrieves a collection of control-specific view identifiers.
        /// </summary>
        /// <returns>
        /// A collection of values that identifies the views available for a UI 
        /// automation element.
        /// </returns>
        int[] GetSupportedViews();

        /// <summary>
        /// Retrieves the name of a control-specific view.
        /// </summary>
        /// <param name="viewId">
        /// The view identifier.
        /// </param>
        /// <returns>
        /// A localized name for the view.
        /// </returns>
        string GetViewName(int viewId);

        /// <summary>
        /// Sets the current control-specific view.
        /// </summary>
        /// <param name="viewId">
        /// A view identifier.
        /// </param>
        void SetCurrentView(int viewId);
    }
}
