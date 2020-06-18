#if WORKINPROGRESS
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace System
{
    public static partial class AppDomainExtensions
    {
        public static AssemblyBuilder DefineDynamicAssembly(this AppDomain appDomain, AssemblyName name, AssemblyBuilderAccess access)
        {
            return null;
        }
    }
}

#endif