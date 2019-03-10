
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
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
    /// Specifies that the type defines or implements a data contract and is serializable
    /// by a serializer, such as the DataContractSerializer.
    /// To make their type serializable, type authors must define a data contract
    /// for their type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum, Inherited = false, AllowMultiple = false)]
    public sealed class DataContract2Attribute : Attribute
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
