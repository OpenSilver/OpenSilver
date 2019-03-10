
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
        public MemberInformation(MemberInfo memberInfo, bool emitDefaultValue, bool isRequired, string name, int order, bool hasXmlElementAttribute, bool hasXmlAttributeAttribute)
        {
            this.MemberInfo = memberInfo;
            this.EmitDefaultValue = emitDefaultValue;
            this.IsRequired = isRequired;
            this.Name = name;
            this.Order = order;
            this.HasXmlElementAttribute = hasXmlElementAttribute;
            this.HasXmlAttributeAttribute = hasXmlAttributeAttribute; // Note: repetition of the word "Attribute" is intended.
        }

        #region Initialized in the constructor

        public readonly MemberInfo MemberInfo;
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
