
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

namespace OpenSilver.Compiler
{
    internal abstract class SystemTypesHelper
    {
        public abstract bool IsSupportedSystemType(string typeFullName, string assemblyIfAny);

        public abstract string GetFullTypeName(string namespaceName, string typeName, string assemblyIfAny);

        public abstract string ConvertFromInvariantString(string source, string typeFullName);

        public abstract string GetDefaultValue(string namespaceName, string typeName, string assemblyIfAny);

        internal abstract string ConvertToDouble(string source);

        internal abstract string ConvertToSingle(string source);

        internal abstract string ConvertToTimeSpan(string source);

        internal abstract string ConvertToString(string source);

        internal abstract string ConvertToBoolean(string source);

        internal abstract string ConvertToByte(string source);

        internal abstract string ConvertToInt16(string source);

        internal abstract string ConvertToInt32(string source);

        internal abstract string ConvertToInt64(string source);

        internal abstract bool IsMscorlibOrNull(string assemblyName);

        internal abstract string GetKey(string namespaceName, string typeName);

        internal abstract string Escape(string s);
    }
}
