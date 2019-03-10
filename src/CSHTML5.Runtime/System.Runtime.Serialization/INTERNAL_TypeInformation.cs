
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
    internal class TypeInformation
    {
        public TypeInformation(Type type, string name, string namespaceName, string itemName, string keyName, string valueName, SerializationType isAttributeSuchAsDataContractPresent)
        {
            this.Type = type;
            this.Name = DataContractSerializer_Helpers.GetTypeNameSafeForSerialization(type);
            this.NamespaceName = namespaceName;
            this.ItemName = itemName;
            this.KeyName = keyName;
            this.ValueName = valueName;
            this.serializationType = isAttributeSuchAsDataContractPresent;
        }

        #region Initialized in the constructor

        public readonly Type Type;
        public readonly string Name;
        public readonly string NamespaceName;
        public readonly string ItemName;
        public readonly string KeyName;
        public readonly string ValueName;
        public readonly SerializationType serializationType;

        #endregion
    }
}
