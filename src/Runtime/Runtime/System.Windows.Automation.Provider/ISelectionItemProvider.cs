
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
    /// Exposes methods and properties to support access by a UI automation client to 
    /// individual, selectable child controls of containers that implement 
    /// <see cref="ISelectionProvider" />.
    /// </summary>
    public interface ISelectionItemProvider
    {
        /// <summary>
        /// Adds the current element to the collection of selected items.
        /// </summary>
        void AddToSelection();

        /// <summary>
        /// Gets a value that indicates whether an item is selected.
        /// </summary>
        /// <returns>
        /// true if the element is selected; otherwise, false.
        /// </returns>
        bool IsSelected { get; }

        /// <summary>
        /// Removes the current element from the collection of selected items.
        /// </summary>
        void RemoveFromSelection();

        /// <summary>
        /// Clears any existing selection and then selects the current element.
        /// </summary>
        void Select();

        /// <summary>
        /// Gets the UI automation provider that implements <see cref="ISelectionProvider" /> 
        /// and acts as the container for the calling object.
        /// </summary>
        /// <returns>
        /// The UI automation provider.
        /// </returns>
        IRawElementProviderSimple SelectionContainer { get; }
    }
}
