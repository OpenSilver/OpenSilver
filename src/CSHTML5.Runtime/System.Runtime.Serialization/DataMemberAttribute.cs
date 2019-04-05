
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
#if !BRIDGE
    /// <summary>
    /// When applied to the member of a type, specifies that the member is part of
    /// a data contract and is serializable by the DataContractSerializer.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class DataMemberAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the DataMemberAttribute
        /// class.
        /// </summary>
        public DataMemberAttribute()
        {
            EmitDefaultValue = true; //default value
        }

        /// <summary>
        /// Gets or sets a value that specifies whether to serialize the default value
        /// for a field or property being serialized.
        /// </summary>
        public bool EmitDefaultValue { get; set; }

        // Exceptions:
        //   System.Runtime.Serialization.SerializationException:
        //     the member is not present.
        /// <summary>
        /// Gets or sets a value that instructs the serialization engine that the member
        /// must be present when reading or deserializing.
        /// </summary>
        public bool IsRequired { get; set; } //todo: implement this (it apparently throws an Exception when the member is not present). I think the Exception is not thrown by this but when trying to deserialize the member and this is set at true but the member is not there.

        // Returns:
        //     The name of the data member. The default is the name of the target that the
        //     attribute is applied to.
        /// <summary>
        /// Gets or sets a data member name.
        /// </summary>
        public string Name { get; set; } //todo: set the default value

        /// <summary>
        /// Gets or sets the order of serialization and deserialization of a member.
        /// </summary>
        public int Order { get; set; }
    }
#endif
}
