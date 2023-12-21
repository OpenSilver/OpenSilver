
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

using OpenSilver.Internal;
using System.ComponentModel;

namespace System.Runtime.Serialization
{
    /// <summary>
    /// When applied to the member of a type, specifies that the member is part of
    /// a data contract and is serializable by the DataContractSerializer.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete(Helper.ObsoleteMemberMessage + " Use System.Runtime.Serialization.CollectionDataContractAttribute instead.")]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public sealed class CollectionDataContract2Attribute : Attribute
    {
        public CollectionDataContract2Attribute() { }

        public CollectionDataContract2Attribute(bool IsReference = false, string ItemName = null, string KeyName = null, string Name = null, string Namespace = null, string ValueName = null)
        {
            this.IsReference = IsReference;
            this.ItemName = ItemName;
            this.KeyName = KeyName;
            this.Name = Name;
            this.Namespace = Namespace;
            this.ValueName = ValueName;
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to preserve object reference data.
        /// </summary>
        public bool IsReference { get; set; }

        /// <summary>
        /// Gets or sets a custom name for a collection element.
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// Gets or sets the custom name for a dictionary key name.
        /// </summary>
        public string KeyName { get; set; }

        /// <summary>
        /// Gets or sets the data contract name for the collection type.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the namespace for the data contract.
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Gets or sets the custom name for a dictionary value name.
        /// </summary>
        public string ValueName { get; set; }
    }
}
