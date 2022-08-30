
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
namespace System.Windows.Automation.Provider
#else
namespace Windows.UI.Xaml.Automation.Provider
#endif
{
    /// <summary>
    /// Exposes methods and properties to support access by a UI Automation client to 
    /// controls that act as containers for a collection of individual, selectable child 
    /// items. The children of this control must implement <see cref="ISelectionItemProvider" />.
    /// </summary>
    public interface ISelectionProvider
    {
        /// <summary>
        /// Gets a value that indicates whether the UI automation provider allows more than 
        /// one child element to be selected concurrently.
        /// </summary>
        /// <returns>
        /// true if multiple selection is allowed; otherwise, false.
        /// </returns>
        bool CanSelectMultiple { get; }

        /// <summary>
        /// Retrieves a UI automation provider for each child element that is selected.
        /// </summary>
        /// <returns>
        /// A generic list of UI automation providers.
        /// </returns>
        IRawElementProviderSimple[] GetSelection();

        /// <summary>
        /// Gets a value that indicates whether the UI automation provider requires at 
        /// least one child element to be selected.
        /// </summary>
        /// <returns>
        /// true if selection is required; otherwise, false.
        /// </returns>
        bool IsSelectionRequired { get; }
    }
}
