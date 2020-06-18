
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



#if WCF_STACK || BRIDGE || CSHTML5BLAZOR

using System.Net.Security;

namespace System.ServiceModel
{
    //[AttributeUsage(ServiceModelAttributeTargets.OperationContract, AllowMultiple = true, Inherited = false)]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    public sealed partial class FaultContractAttribute : Attribute
    {
        string action;
        string name;
        string ns;
        Type type;
        ProtectionLevel protectionLevel = ProtectionLevel.None;
        bool hasProtectionLevel = false;

        public FaultContractAttribute(Type detailType)
        {
            if (detailType == null)
                throw new ArgumentNullException("detailType");

            this.type = detailType;
        }

        public Type DetailType
        {
            get { return this.type; }
        }

        public string Action
        {
            get { return this.action; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                this.action = value;
            }
        }

        public string Name
        {
            get { return this.name; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                if (value == string.Empty)
                    throw new ArgumentOutOfRangeException("value");
                this.name = value;
            }
        }

        public string Namespace
        {
            get { return this.ns; }
            set
            {
                //if (!string.IsNullOrEmpty(value))
                //    NamingHelper.CheckUriProperty(value, "Namespace");
                this.ns = value;
            }
        }

        internal const string ProtectionLevelPropertyName = "ProtectionLevel";
        public ProtectionLevel ProtectionLevel
        {
            get
            {
                return this.protectionLevel;
            }
            set
            {
                //if (!ProtectionLevelHelper.IsDefined(value))
                //    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                this.protectionLevel = value;
                this.hasProtectionLevel = true;
            }
        }

        public bool HasProtectionLevel
        {
            get { return this.hasProtectionLevel; }
        }
    }
}

#endif