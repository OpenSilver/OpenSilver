using System;
using System.Collections.Generic;
using System.Text;

namespace System.Reflection.Emit
{
	[OpenSilver.NotImplemented]
    public partial class ModuleBuilder : Module
    {
		[OpenSilver.NotImplemented]
        public TypeBuilder DefineType(string name, TypeAttributes attr, Type parent)
        {
            return new TypeBuilder();
        }
    }
}

