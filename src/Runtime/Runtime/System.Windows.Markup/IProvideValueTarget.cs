
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
    /// Represents a service that reports situational object-property relationships
    /// for markup extension evaluation.
    /// </summary>
    public interface IProvideValueTarget
    {
        /// <summary>
        /// Gets the target object being reported.
        /// </summary>
        object TargetObject { get; }

        /// <summary>
        /// Gets an identifier for the target property being reported.
        /// </summary>
        object TargetProperty { get; }
    }
}
