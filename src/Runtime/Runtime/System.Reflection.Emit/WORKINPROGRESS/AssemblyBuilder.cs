using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace System.Reflection.Emit
{
    [OpenSilver.NotImplemented]
    public sealed partial class AssemblyBuilder : Assembly
    {
		[OpenSilver.NotImplemented]
        public ModuleBuilder DefineDynamicModule(string name)
        {
            return new ModuleBuilder();
        }

		[OpenSilver.NotImplemented]
        public ModuleBuilder DefineDynamicModule(string name, bool emitSymbolInfo)
        {
            return new ModuleBuilder();
        }

    }
}

