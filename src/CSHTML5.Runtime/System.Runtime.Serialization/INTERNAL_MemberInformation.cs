

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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace System.Runtime.Serialization
{
    internal partial class MemberInformation
    {
        public MemberInformation(MemberInfo memberInfo, MemberInfo memberInfoBase,  bool emitDefaultValue, bool isRequired, string name, int order, bool hasXmlElementAttribute, bool hasXmlAttributeAttribute)
        {
            this.MemberInfo = memberInfo;
            this.MemberInfoBase = memberInfoBase;
            this.EmitDefaultValue = emitDefaultValue;
            this.IsRequired = isRequired;
            this.Name = name;
            this.Order = order;
            this.HasXmlElementAttribute = hasXmlElementAttribute;
            this.HasXmlAttributeAttribute = hasXmlAttributeAttribute; // Note: repetition of the word "Attribute" is intended.
        }

        #region Initialized in the constructor

        public readonly MemberInfo MemberInfo;

        /// <summary>
        /// If the member is "override", this property contains the corresponding "virtual" or "abstract" declaration in the base classes.
        /// </summary>
        public readonly MemberInfo MemberInfoBase;

        public readonly bool EmitDefaultValue;
        public readonly bool IsRequired;
        public readonly string Name;
        public readonly int Order;
        public readonly bool HasXmlElementAttribute;
        public readonly bool HasXmlAttributeAttribute; // Note: repetition of the word "Attribute" is intended.

        #endregion

        public Type MemberType;

    }
}
