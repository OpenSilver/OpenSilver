
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

namespace System.Windows.Markup
{
    /// <summary>
    /// Defines a contract for how names of elements should be accessed within a
    /// particular XAML namescope, and how to enforce uniqueness of names within
    /// that XAML namescope.
    /// </summary>
    public interface INameScope
    {
        /// <summary>
        /// Returns an object that has the provided identifying name.
        /// </summary>
        /// <param name="name">The name identifier for the object being requested.</param>
        /// <returns>The object, if found. Returns null if no object of that name was found.</returns>
        object FindName(string name);

        /// <summary>
        /// Registers the provided name into the current XAML namescope.
        /// </summary>
        /// <param name="name">The name to register.</param>
        /// <param name="scopedElement">The specific element that the provided name refers to.</param>
        void RegisterName(string name, object scopedElement);

        /// <summary>
        /// Unregisters the provided name from the current XAML namescope.
        /// </summary>
        /// <param name="name">The name to unregister.</param>
        void UnregisterName(string name);
    }
}
