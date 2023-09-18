
/*===================================================================================
* 
*   Copyright (c) Userware (OpenSilver.net, CSHTML5.com)
*      
*   This file is part of both the OpenSilver Compiler (https://opensilver.net), which
*   is licensed under the MIT license (https://opensource.org/licenses/MIT), and the
*   CSHTML5 Compiler (http://cshtml5.com), which is dual-licensed (MIT + commercial).
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using OpenSilver.Compiler.Common;

namespace OpenSilver.Compiler
{
    internal interface ICoreTypesConverter
    {
        bool IsSupportedCoreType(string typeFullName, string assemblyName);

        string ConvertFromInvariantString(string source, string typeFullName);
    }

    internal abstract class CoreTypesConverterBase : ICoreTypesConverter
    {
        protected abstract Dictionary<string, Func<string, string>> SupportedCoreTypes { get; }

        public bool IsSupportedCoreType(string typeFullName, string assemblyName)
        {
            if (IsCoreAssemblyOrNull(assemblyName))
            {
                return SupportedCoreTypes.ContainsKey(typeFullName.ToLower());
            }

            return false;
        }

        public string ConvertFromInvariantString(string source, string typeFullName)
        {
            if (SupportedCoreTypes.TryGetValue(typeFullName.ToLower(), out var converter))
            {
                Debug.Assert(converter != null);
                return converter(source);
            }

            throw new InvalidOperationException(
                $"Cannot find a converter for type '{typeFullName}'"
            );
        }

        private static bool IsCoreAssemblyOrNull(string assemblyName)
        {
            if (assemblyName == null ||
                assemblyName.Equals(Constants.NAME_OF_CORE_ASSEMBLY_USING_BLAZOR, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }
    }
}
