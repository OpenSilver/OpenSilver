
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
using OpenSilver.Internal;

namespace System
{
    /// <summary>
    /// Indicates that a class can be serialized. This class cannot be inherited.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete(Helper.ObsoleteMemberMessage + " Use System.SerializableAttribute instead.")]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Delegate, Inherited = false)]
    public sealed class Serializable2Attribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the System.SerializableAttribute class.
        /// </summary>
        public Serializable2Attribute() { }
    }
}