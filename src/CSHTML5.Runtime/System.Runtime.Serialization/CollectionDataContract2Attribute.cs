
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Runtime.Serialization
{
    /// <summary>
    /// When applied to the member of a type, specifies that the member is part of
    /// a data contract and is serializable by the DataContractSerializer.
    /// </summary>
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
