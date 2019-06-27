
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace System.Runtime.Serialization
{
    internal class MemberInformation
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
