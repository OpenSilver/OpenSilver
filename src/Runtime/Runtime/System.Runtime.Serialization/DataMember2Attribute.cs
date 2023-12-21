
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

namespace System.Runtime.Serialization
{
    /// <summary>
    /// When applied to the member of a type, specifies that the member is part of
    /// a data contract and is serializable by the DataContractSerializer.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete(Helper.ObsoleteMemberMessage + " Use System.Runtime.Serialization.DataMemberAttribute instead.")]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class DataMember2Attribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the DataMemberAttribute
        /// class.
        /// </summary>
        public DataMember2Attribute()
        {
            EmitDefaultValue = true; //default value
        }

        public DataMember2Attribute(bool EmitDefaultValue = true, bool IsRequired = false, string Name = null, int Order = -1)
        {
            this.EmitDefaultValue = EmitDefaultValue;
            this.IsRequired = IsRequired;
            this.Name = Name;
            this.Order = Order;
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

        int _order = -1; //default value
        /// <summary>
        /// Gets or sets the order of serialization and deserialization of a member.
        /// </summary>
        public int Order {
            get { return _order; }
            set
            {
                if(value < 0)
                {
                    throw new InvalidDataContractException("Order cannot be negative.");
                }
                _order = value;
            }
        }
        //About the Note below: cf https://theburningmonk.com/2010/08/wcf-be-ware-of-the-field-ordering-when-using-datacontractserializer/
        //Note: the order is Inherited properties then properties with no order defined then properties in the Order set. The order set only affects the type it is defined in, so an order in a parent and in an inheriting class do not affect each other.
        //      result below:
        //          -> start with the Parent class, then move down the tree towards the class of the instance
        //              -> for each "level" of class, start with the properties with no Order set (Order = -1); and then put the properties with Order set in the defined order.
        //                  Note: the last line can be simply read as "follow the Order property" since the user cannot set a negative order so any property with a set Order will come after any property without it.
    }
}
