
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

namespace System.Windows.Data
{
    /// <summary>
    /// Describes how the data propagates in a binding.
    /// </summary>
    public enum BindingMode
    {
        /// <summary>
        /// Updates the target property when the binding is created. Changes to the source object can also propagate to the target.
        /// </summary>
        OneWay = 1,
        /// <summary>
        /// Updates the target property when the binding is created.
        /// </summary>
        OneTime = 2,
        /// <summary>
        /// Updates either the target or the source object when either changes. When the binding is created, the target property is updated from the source.
        /// </summary>
        TwoWay = 3,

    }
}