
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

namespace OpenSilver.Compiler
{
    internal class ConvertingStringToValueCS : ConvertingStringToValue
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
            
            string result;
            
            switch (underlyingType)
            {
                case "global::System.SByte":
                case "global::System.UInt16":
                case "global::System.UInt32":
                case "global::System.UInt64":
                    // Note: for numeric types, removing the quotation marks is sufficient
                    // (+ potential additional letter to tell the actual type because casts
                    // from int to double for example causes an exception).
                    result = value;
                    break;

                case "global::System.Decimal":
                    result = PrepareStringForDecimal(value);
                    break;

                case "global::System.Char":
                    result = PrepareStringForChar(value);
                    break;

                case "global::System.Object":
                    result = PrepareStringForString(source);
                    break;

                default:
                    // return after escaping (note: we use value and not stringValue
                    // because it can be a string that starts or ends with spaces)
                    result = CoreTypesHelper.ConvertFromInvariantStringHelper(
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
                return $"'{source}'";
            }

            return "'\\0'";
        }


        internal override bool IsNullableType(string type, out string underlyingType)
        {
            if (type.StartsWith("global::System.Nullable<"))
            {
                // skips "global::System.Nullable<" and then remove 
                // the trailing '>' at the end
                underlyingType = type.Substring(24, type.Length - 25);
                return true;
            }

            underlyingType = type;
            return false;
        }

        internal override string GetQuotedVerbatimString(string s)
        {
            return "@\"" + s.Replace("\"", "\"\"") + "\"";
        }
    }

}
