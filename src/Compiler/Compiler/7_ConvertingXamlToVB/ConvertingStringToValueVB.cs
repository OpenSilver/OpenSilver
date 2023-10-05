
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
using System.Text.RegularExpressions;

namespace OpenSilver.Compiler
{
    internal class ConvertingStringToValueVB : ConvertingStringToValue
    {
        public override string ConvertFromInvariantString(string type, string source)
        {
            string value = source.Trim();

            if (IsNullableType(type, out string underlyingType))
            {
                if (value == "null")
                {
                    return "Nothing";
                }
            }

            string result;

            switch (underlyingType)
            {
                case "Global.System.SByte":
                case "Global.System.UInt16":
                case "Global.System.UInt32":
                case "Global.System.UInt64":
                    // Note: for numeric types, removing the quotation marks is sufficient
                    // (+ potential additional letter to tell the actual type because casts
                    // from int to double for example causes an exception).
                    result = value;
                    break;

                case "Global.System.Decimal":
                    result = PrepareStringForDecimal(value);
                    break;

                case "Global.System.Char":
                    result = PrepareStringForChar(value);
                    break;

                case "Global.System.Object":
                    result = PrepareStringForString(source);
                    break;

                default:
                    // return after escaping (note: we use value and not stringValue
                    // because it can be a string that starts or ends with spaces)
                    result = CoreTypesHelperVB.ConvertFromInvariantStringHelper(
                        source,
                        underlyingType
                    );
                    break;
            }

            return result;
        }

        internal override string PrepareStringForChar(string source)
        {
            if (source != null && source.Length == 1)
            {
                return $"\"{source}\"c";
            }

            return "Chr(0)";
        }

        internal override bool IsNullableType(string type, out string underlyingType)
        {
            if (type.StartsWith("Global.System.Nullable<"))
            {
                // skips "Global.System.Nullable<" and then remove 
                // the trailing '>' at the end
                underlyingType = type.Substring(23, type.Length - 24);
                return true;
            }

            underlyingType = type;
            return false;
        }

        internal override string GetQuotedVerbatimString(string s)
        {
            return "\"" + Regex.Unescape(s.Replace("\"", "\"\"")) + "\"";
        }
    }

}
