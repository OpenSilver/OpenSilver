
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
    internal class TypeInformation
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
