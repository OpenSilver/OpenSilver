
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


#if !OPENSILVER
#if WCF_STACK || BRIDGE || CSHTML5BLAZOR

namespace System.ServiceModel
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Interface, Inherited = true, AllowMultiple = true)]
    public sealed partial class ServiceKnownTypeAttribute : Attribute
    {
        Type declaringType;
        string methodName;
        Type type;

        private ServiceKnownTypeAttribute()
        {
            // Disallow default constructor
        }

        public ServiceKnownTypeAttribute(Type type)
        {
            this.type = type;
        }

        public ServiceKnownTypeAttribute(string methodName)
        {
            this.methodName = methodName;
        }

        public ServiceKnownTypeAttribute(string methodName, Type declaringType)
        {
            this.methodName = methodName;
            this.declaringType = declaringType;
        }

        public Type DeclaringType
        {
            get { return declaringType; }
        }

        public string MethodName
        {
            get { return methodName; }
        }

        public Type Type
        {
            get { return type; }
        }
    }
}

#endif
#endif