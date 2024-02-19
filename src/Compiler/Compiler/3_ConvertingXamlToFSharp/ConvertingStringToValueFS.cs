
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

namespace OpenSilver.Compiler
{
    internal class ConvertingStringToValueFS : ConvertingStringToValue
    {
        public override string ConvertFromInvariantString(string type, string source)
        {
            string value = source.Trim();

            if (IsNullableType(type, out string underlyingType))
            {
                if (value == "null")
                {
                    return "null";
                }
            }

            return CoreTypesHelperFS.ConvertFromInvariantStringHelper(source, underlyingType);
        }

        private static bool IsNullableType(string type, out string underlyingType)
        {
            if (type.StartsWith("global.System.Nullable<"))
            {
                // skips "System.Nullable<" and then remove 
                // the trailing '>' at the end
                // underlyingType = type.Substring(16, type.Length - 17);
                underlyingType = type; // In F#, without System.Nullable it occurs error
                return true;
            }

            underlyingType = type;
            return false;
        }
    }
}
