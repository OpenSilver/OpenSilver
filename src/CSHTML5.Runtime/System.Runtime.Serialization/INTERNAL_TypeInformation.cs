

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
    internal partial class TypeInformation
    {
        public TypeInformation(Type type, string name, string namespaceName, string itemName, string keyName, string valueName, SerializationType isAttributeSuchAsDataContractPresent)
        {
            this.Type = type;
            this.Name = name;
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
