
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



using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace System.Runtime.Serialization
{
    internal static class DataContractSerializer_ValueTypesHandler
    {
        public static Dictionary<Type, string> TypesToNames;
        public static Dictionary<string, Type> NamesToTypes;

        static DataContractSerializer_ValueTypesHandler()
        {
            FillDictionaries();
        }

        public static void EnsureInitialized()
        {
            if (TypesToNames == null)
            {
                FillDictionaries();
            }
        }

        internal static object ConvertStringToValueType(string str, Type resultType)
        {
            // List of possible value types: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/value-types-table

            if (resultType == typeof(string))
            {
                return str;
            }
            else if (resultType == typeof(char) || resultType == typeof(char?))
            {
                return Convert.ToChar(int.Parse(str));
            }
            else if (resultType == typeof(bool) || resultType == typeof(bool?))
            {
                return bool.Parse(str);
            }
            else if (resultType == typeof(short) || resultType == typeof(short?)) //aka Int16
            {
                return short.Parse(str);
            }
            else if (resultType == typeof(int) || resultType == typeof(int?)) //aka Int32
            {
                return int.Parse(str);
            }
            else if (resultType == typeof(long) || resultType == typeof(long?)) //aka Int64
            {
                return long.Parse(str);
            }
            else if (resultType == typeof(ushort) || resultType == typeof(ushort?)) //aka UInt16
            {
                return ushort.Parse(str);
            }
            else if (resultType == typeof(uint) || resultType == typeof(uint?)) //aka UInt32
            {
                return uint.Parse(str);
            }
            else if (resultType == typeof(ulong) || resultType == typeof(ulong?)) //aka UInt64
            {
                return ulong.Parse(str);
            }
            else if (resultType == typeof(decimal) || resultType == typeof(decimal?))
            {
                return decimal.Parse(str);
            }
            else if (resultType == typeof(float) || resultType == typeof(float?)) //aka Single
            {
                return float.Parse(str);
            }
            else if (resultType == typeof(double) || resultType == typeof(double?))
            {
#if SILVERLIGHT
                return Double.Parse(str, CultureInfo.InvariantCulture);
#else
                return Double.Parse(str);
#endif
            }
            else if (resultType == typeof(DateTime) || resultType == typeof(DateTime?))
            {
                return DateTime.Parse(str);
            }
            else if (resultType == typeof(Guid) || resultType == typeof(Guid?))
            {
                return Guid.Parse(str);
            }
            else if (resultType == typeof(byte) || resultType == typeof(byte?))
            {
                return byte.Parse(str);
            }
            else if (resultType == typeof(sbyte) || resultType == typeof(sbyte?))
            {
                return sbyte.Parse(str);
            }
            else if (resultType.IsEnum)
            {
                var enumValue = Enum.Parse(resultType, str, true);
                return enumValue;
            }

            throw new Exception("Unable to create the type '" + resultType.ToString() + "' from the string '" + str + "'.");
        }

        private static void FillDictionaries()
        {
            //we fill the dictionaries that help with serializing/deserializing default types:
            TypesToNames = new Dictionary<Type, string>();
            NamesToTypes = new Dictionary<string, Type>();

            // Reference: https://docs.microsoft.com/en-us/dotnet/framework/wcf/feature-details/data-contract-schema-reference

            //bool type:
            TypesToNames.Add(typeof(bool), "boolean");
            NamesToTypes.Add("boolean", typeof(bool));

            //char type:
            TypesToNames.Add(typeof(char), "char");
            NamesToTypes.Add("char", typeof(char));

            //string type:
            TypesToNames.Add(typeof(string), "string");
            NamesToTypes.Add("string", typeof(string));

            //byte
            TypesToNames.Add(typeof(byte), "byte");
            NamesToTypes.Add("byte", typeof(byte));

            //short, aka Int16
            TypesToNames.Add(typeof(short), "short");
            NamesToTypes.Add("short", typeof(short));

            //int, aka Int32
            TypesToNames.Add(typeof(int), "int");
            NamesToTypes.Add("int", typeof(int));

            //long, aka Int64
            TypesToNames.Add(typeof(long), "long");
            NamesToTypes.Add("long", typeof(long));

            //UInt16
            TypesToNames.Add(typeof(UInt16), "unsignedShort");
            NamesToTypes.Add("unsignedShort", typeof(UInt16));

            //UInt32
            TypesToNames.Add(typeof(UInt32), "unsignedInt");
            NamesToTypes.Add("unsignedInt", typeof(UInt32));

            //UInt64
            TypesToNames.Add(typeof(UInt64), "unsignedLong");
            NamesToTypes.Add("unsignedLong", typeof(UInt64));

            //decimal
            TypesToNames.Add(typeof(decimal), "decimal");
            NamesToTypes.Add("decimal", typeof(decimal));

            //double
            TypesToNames.Add(typeof(double), "double");
            NamesToTypes.Add("double", typeof(double));

            //float, aka Single
            TypesToNames.Add(typeof(float), "float");
            NamesToTypes.Add("float", typeof(float));

            //DateTime
            TypesToNames.Add(typeof(DateTime), "dateTime");
            NamesToTypes.Add("dateTime", typeof(DateTime));

            //Guid
            TypesToNames.Add(typeof(Guid), "guid");
            NamesToTypes.Add("guid", typeof(Guid));

            //todo: add other missing value types (cf. https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/value-types-table
            // and https://docs.microsoft.com/en-us/dotnet/framework/wcf/feature-details/data-contract-schema-reference )
        }
    }
}
