
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

namespace System.Windows.Controls.Primitives
{
    /// <summary>
    /// Extends the <see cref="IItemContainerGenerator"/> interface to reuse the UI 
    /// content it generates. Classes that are responsible for generating user 
    /// interface (UI) content on behalf of a host implement this interface.
    /// </summary>
    public interface IRecyclingItemContainerGenerator : IItemContainerGenerator
	{
        /// <summary>
        /// Disassociates item containers from their data items and saves the containers
        /// so they can be reused later for other data items.
        /// </summary>
        /// <param name="position">
        /// The zero-based index of the first element to reuse. position must refer to a
        /// previously generated (realized) item.
        /// </param>
        /// <param name="count">
        /// The number of elements to reuse, starting at position.
        /// </param>
        void Recycle(GeneratorPosition position, int count);
	}
}
