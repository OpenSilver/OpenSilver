

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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Runtime.Serialization
{
    /// <summary>
    /// Specifies that the type defines or implements a data contract and is serializable
    /// by a serializer, such as the DataContractSerializer.
    /// To make their type serializable, type authors must define a data contract
    /// for their type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum, Inherited = false, AllowMultiple = false)]
    public sealed partial class DataContract2Attribute : Attribute
    {
        public DataContract2Attribute() { }

        public DataContract2Attribute(string Name = null, string Namespace = "", bool IsReference = false)
        {
            this.Name = Name;
            this.Namespace = Namespace;
            this.IsReference = IsReference;
        }


        // Returns:
        //     true to keep object reference data using standard XML; otherwise, false.
        //     The default is false.
        /// <summary>
        /// Gets or sets a value that indicates whether to preserve object reference
        /// data.
        /// </summary>
        public bool IsReference { get; private set; } //todo: implement this.

        // Returns:
        //     The local name of a data contract. The default is the name of the class that
        //     the attribute is applied to.
        /// <summary>
        /// Gets or sets the name of the data contract for the type.
        /// </summary>
        public string Name { get; private set; } //todo: set the default value.

        /// <summary>
        /// Gets or sets the namespace for the data contract for the type.
        /// </summary>
        public string Namespace { get; private set; }
    }
}
