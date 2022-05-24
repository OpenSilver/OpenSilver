
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

namespace OpenSilver.Internal.Xaml
{
    /// <summary>
    /// Provides markup compile and tools support for named XAML elements and for attaching
    /// event handlers to them.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public interface IComponentConnector
    {
        /// <summary>
        /// Attaches events and names to compiled content.
        /// </summary>
        /// <param name="connectionId">
        /// An identifier token to distinguish calls.
        /// </param>
        /// <param name="target">
        /// The target to connect events and names to.
        /// </param>
        void Connect(int connectionId, object target);
    }
}
