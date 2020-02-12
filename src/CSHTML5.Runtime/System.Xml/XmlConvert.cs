
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

namespace System.Xml
{
    // Summary:
    //     Encodes and decodes XML names and provides methods for converting between
    //     common language runtime types and XML Schema definition language (XSD) types.
    //     When converting data types the values returned are locale independent.
#if !BRIDGE
    [JSIL.Meta.JSStubOnly]
#endif
    public partial class XmlConvert
    {
        /// <summary>
        /// Initializes a new instance of the System.Xml.XmlConvert class.
        /// </summary>
        public XmlConvert()
        {
            //throw new NotImplementedException();
        }

        //// Summary:
        ////     Decodes a name. This method does the reverse of the System.Xml.XmlConvert.EncodeName(System.String)
        ////     and System.Xml.XmlConvert.EncodeLocalName(System.String) methods.
        ////
        //// Parameters:
        ////   name:
        ////     The name to be transformed.
        ////
        //// Returns:
        ////     The decoded name.
        //public static string DecodeName(string name);
        ////
        //// Summary:
        ////     Converts the name to a valid XML local name.
        ////
        //// Parameters:
        ////   name:
        ////     The name to be encoded.
        ////
        //// Returns:
        ////     The encoded name.
        //public static string EncodeLocalName(string name);
        ////
        //// Summary:
        ////     Converts the name to a valid XML name.
        ////
        //// Parameters:
        ////   name:
        ////     A name to be translated.
        ////
        //// Returns:
        ////     Returns the name with any invalid characters replaced by an escape string.
        //public static string EncodeName(string name);
        ////
        //// Summary:
        ////     Verifies the name is valid according to the XML specification.
        ////
        //// Parameters:
        ////   name:
        ////     The name to be encoded.
        ////
        //// Returns:
        ////     The encoded name.
        //public static string EncodeNmToken(string name);
        ////
        //// Summary:
        ////     Checks whether the passed-in character is a valid non-colon character type.
        ////
        //// Parameters:
        ////   ch:
        ////     The character to verify as a non-colon character.
        ////
        //// Returns:
        ////     Returns true if the character is a valid non-colon character type; otherwise,
        ////     false.
        //public static bool IsNCNameChar(char ch);
        ////
        //// Summary:
        ////     Returns the passed-in character instance if the character in the argument
        ////     is a valid public id character, otherwise null.
        ////
        //// Parameters:
        ////   ch:
        ////     System.Char object to validate.
        ////
        //// Returns:
        ////     Returns the passed-in character if the character is a valid public id character,
        ////     otherwise null.
        //public static bool IsPublicIdChar(char ch);
        ////
        //// Summary:
        ////     Checks if the passed-in character is a valid Start Name Character type.
        ////
        //// Parameters:
        ////   ch:
        ////     The character to validate.
        ////
        //// Returns:
        ////     true if the character is a valid Start Name Character type; otherwise, false.
        //public static bool IsStartNCNameChar(char ch);
        ////
        //// Summary:
        ////     Checks if the passed-in character is a valid XML whitespace character.
        ////
        //// Parameters:
        ////   ch:
        ////     The character to validate.
        ////
        //// Returns:
        ////     true if the passed in character is a valid XML whitespace character; otherwise
        ////     false.
        //public static bool IsWhitespaceChar(char ch);
        ////
        //// Summary:
        ////     Checks if the passed-in character is a valid XML character.
        ////
        //// Parameters:
        ////   ch:
        ////     The character to validate.
        ////
        //// Returns:
        ////     true if the passed in character is a valid XML character; otherwise false.
        //public static bool IsXmlChar(char ch);
        ////
        //// Summary:
        ////     Checks if the passed-in surrogate pair of characters is a valid XML character.
        ////
        //// Parameters:
        ////   lowChar:
        ////     The surrogate character to validate.
        ////
        ////   highChar:
        ////     The surrogate character to validate.
        ////
        //// Returns:
        ////     true if the passed in surrogate pair of characters is a valid XML character;
        ////     otherwise false.
        //public static bool IsXmlSurrogatePair(char lowChar, char highChar);
        
        // Exceptions:
        //   System.ArgumentNullException:
        //     s is null.
        //
        //   System.FormatException:
        //     s does not represent a Boolean value.
        /// <summary>
        /// Converts the System.String to a System.Boolean equivalent.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <returns>A Boolean value, that is, true or false.</returns>
        public static bool ToBoolean(string s)
        {
            if (s.ToLower() == "true")
                return true;
            return false;
        }
        
        // Exceptions:
        //   System.ArgumentNullException:
        //     s is null.
        //
        //   System.FormatException:
        //     s is not in the correct format.
        //
        //   System.OverflowException:
        //     s represents a number less than System.Byte.MinValue or greater than System.Byte.MaxValue.
        /// <summary>
        /// Converts the System.String to a System.Byte equivalent.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <returns>A Byte equivalent of the string.</returns>
        public static byte ToByte(string s)
        {
            return (byte)Parse(s);
        }
        ////
        //// Summary:
        ////     Converts the System.String to a System.Char equivalent.
        ////
        //// Parameters:
        ////   s:
        ////     The string containing a single character to convert.
        ////
        //// Returns:
        ////     A Char representing the single character.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The value of the s parameter is null.
        ////
        ////   System.FormatException:
        ////     The s parameter contains more than one character.
        //public static char ToChar(string s);
        ////
        //// Summary:
        ////     Converts the System.String to a System.DateTime equivalent.
        ////
        //// Parameters:
        ////   s:
        ////     The string to convert.
        ////
        //// Returns:
        ////     A DateTime equivalent of the string.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     s is null.
        ////
        ////   System.FormatException:
        ////     s is an empty string or is not in the correct format.
        //[Obsolete("Use XmlConvert.ToDateTime() that takes in XmlDateTimeSerializationMode")]
        //public static DateTime ToDateTime(string s);
        ////
        //// Summary:
        ////     Converts the System.String to a System.DateTime equivalent.
        ////
        //// Parameters:
        ////   s:
        ////     The string to convert.
        ////
        ////   format:
        ////     The format structure to apply to the converted DateTime. Valid formats include
        ////     "yyyy-MM-ddTHH:mm:sszzzzzz" and its subsets. The string is validated against
        ////     this format.
        ////
        //// Returns:
        ////     A DateTime equivalent of the string.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     s is null.
        ////
        ////   System.FormatException:
        ////     s or format is String.Empty -or- s does not contain a date and time that
        ////     corresponds to format.
        //public static DateTime ToDateTime(string s, string format);
        ////
        //// Summary:
        ////     Converts the System.String to a System.DateTime equivalent.
        ////
        //// Parameters:
        ////   s:
        ////     The string to convert.
        ////
        ////   formats:
        ////     An array containing the format structures to apply to the converted DateTime.
        ////     Valid formats include "yyyy-MM-ddTHH:mm:sszzzzzz" and its subsets.
        ////
        //// Returns:
        ////     A DateTime equivalent of the string.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     s is null.
        ////
        ////   System.FormatException:
        ////     s or an element of formats is String.Empty -or- s does not contain a date
        ////     and time that corresponds to any of the elements of formats.
        //public static DateTime ToDateTime(string s, string[] formats);
        ////
        //// Summary:
        ////     Converts the System.String to a System.DateTime using the System.Xml.XmlDateTimeSerializationMode
        ////     specified
        ////
        //// Parameters:
        ////   s:
        ////     The System.String value to convert.
        ////
        ////   dateTimeOption:
        ////     One of the System.Xml.XmlDateTimeSerializationMode values that specify whether
        ////     the date should be converted to local time or preserved as Coordinated Universal
        ////     Time (UTC), if it is a UTC date.
        ////
        //// Returns:
        ////     A System.DateTime equivalent of the System.String.
        ////
        //// Exceptions:
        ////   System.NullReferenceException:
        ////     s is null.
        ////
        ////   System.ArgumentNullException:
        ////     The dateTimeOption value is null.
        ////
        ////   System.FormatException:
        ////     s is an empty string or is not in a valid format.
        //public static DateTime ToDateTime(string s, XmlDateTimeSerializationMode dateTimeOption);
        ////
        //// Summary:
        ////     Converts the supplied System.String to a System.DateTimeOffset equivalent.
        ////
        //// Parameters:
        ////   s:
        ////     The string to convert.Note   The string must conform to a subset of the W3C
        ////     Recommendation for the XML dateTime type. For more information see http://www.w3.org/TR/xmlschema-2/#dateTime.
        ////
        //// Returns:
        ////     The System.DateTimeOffset equivalent of the supplied string.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     s is null.
        ////
        ////   System.ArgumentOutOfRangeException:
        ////     The argument passed to this method is outside the range of allowable values.
        ////     For information about allowable values, see System.DateTimeOffset.
        ////
        ////   System.FormatException:
        ////     The argument passed to this method does not conform to a subset of the W3C
        ////     Recommendations for the XML dateTime type. For more information see http://www.w3.org/TR/xmlschema-2/#dateTime.
        //public static DateTimeOffset ToDateTimeOffset(string s);
        ////
        //// Summary:
        ////     Converts the supplied System.String to a System.DateTimeOffset equivalent.
        ////
        //// Parameters:
        ////   s:
        ////     The string to convert.
        ////
        ////   format:
        ////     The format from which s is converted. The format parameter can be any subset
        ////     of the W3C Recommendation for the XML dateTime type. (For more information
        ////     see http://www.w3.org/TR/xmlschema-2/#dateTime.) The string s is validated
        ////     against this format.
        ////
        //// Returns:
        ////     The System.DateTimeOffset equivalent of the supplied string.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     s is null.
        ////
        ////   System.FormatException:
        ////     s or format is an empty string or is not in the specified format.
        //public static DateTimeOffset ToDateTimeOffset(string s, string format);
        ////
        //// Summary:
        ////     Converts the supplied System.String to a System.DateTimeOffset equivalent.
        ////
        //// Parameters:
        ////   s:
        ////     The string to convert.
        ////
        ////   formats:
        ////     An array of formats from which s can be converted. Each format in formats
        ////     can be any subset of the W3C Recommendation for the XML dateTime type. (For
        ////     more information see http://www.w3.org/TR/xmlschema-2/#dateTime.) The string
        ////     s is validated against one of these formats.
        ////
        //// Returns:
        ////     The System.DateTimeOffset equivalent of the supplied string.
        //public static DateTimeOffset ToDateTimeOffset(string s, string[] formats);


       
        // Exceptions:
        //   System.ArgumentNullException:
        //     s is null.
        //
        //   System.FormatException:
        //     s is not in the correct format.
        //
        //   System.OverflowException:
        //     s represents a number less than System.Decimal.MinValue or greater than System.Decimal.MaxValue.
        /// <summary>
        /// Converts the System.String to a System.Decimal equivalent.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <returns>A Decimal equivalent of the string.</returns>
        public static decimal ToDecimal(string s)
        {
            return (decimal)Parse(s);
        }
       
        // Exceptions:
        //   System.ArgumentNullException:
        //     s is null.
        //
        //   System.FormatException:
        //     s is not in the correct format.
        //
        //   System.OverflowException:
        //     s represents a number less than System.Double.MinValue or greater than System.Double.MaxValue.
        /// <summary>
        /// Converts the System.String to a System.Double equivalent.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <returns>A Double equivalent of the string.</returns>
        public static double ToDouble(string s)
        {
            return Parse(s);
        }
       
        /// <summary>
        /// Converts the System.String to a System.Guid equivalent.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <returns>A Guid equivalent of the string.</returns>
        public static Guid ToGuid(string s)
        {
            Guid guid;

            if (Guid.TryParse(s, out guid))
                return guid;
            return Guid.NewGuid();
        }
        
        // Exceptions:
        //   System.ArgumentNullException:
        //     s is null.
        //
        //   System.FormatException:
        //     s is not in the correct format.
        //
        //   System.OverflowException:
        //     s represents a number less than System.Int16.MinValue or greater than System.Int16.MaxValue.
        /// <summary>
        /// Converts the System.String to a System.Int16 equivalent.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <returns>An Int16 equivalent of the string.</returns>
        public static short ToInt16(string s)
        {
            return (short)Parse(s);
        }
       
        // Exceptions:
        //   System.ArgumentNullException:
        //     s is null.
        //
        //   System.FormatException:
        //     s is not in the correct format.
        //
        //   System.OverflowException:
        //     s represents a number less than System.Int32.MinValue or greater than System.Int32.MaxValue.
        /// <summary>
        /// Converts the System.String to a System.Int32 equivalent.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <returns>An Int32 equivalent of the string.</returns>
        public static int ToInt32(string s)
        {
            return (int)Parse(s);
        }
        
        // Exceptions:
        //   System.ArgumentNullException:
        //     s is null.
        //
        //   System.FormatException:
        //     s is not in the correct format.
        //
        //   System.OverflowException:
        //     s represents a number less than System.Int64.MinValue or greater than System.Int64.MaxValue.
        /// <summary>
        /// Converts the System.String to a System.Int64 equivalent.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <returns>An Int64 equivalent of the string.</returns>
        public static long ToInt64(string s)
        {
            return (long)Parse(s);
        }
       
        // Exceptions:
        //   System.ArgumentNullException:
        //     s is null.
        //
        //   System.FormatException:
        //     s is not in the correct format.
        //
        //   System.OverflowException:
        //     s represents a number less than System.SByte.MinValue or greater than System.SByte.MaxValue.
        /// <summary>
        /// Converts the System.String to a System.SByte equivalent.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <returns>An SByte equivalent of the string.</returns>
        public static sbyte ToSByte(string s)
        {
            return (sbyte)Parse(s);
        }
       
        // Exceptions:
        //   System.ArgumentNullException:
        //     s is null.
        //
        //   System.FormatException:
        //     s is not in the correct format.
        //
        //   System.OverflowException:
        //     s represents a number less than System.Single.MinValue or greater than System.Single.MaxValue.
        /// <summary>
        /// Converts the System.String to a System.Single equivalent.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <returns>A Single equivalent of the string.</returns>
        public static float ToSingle(string s)
        {
            return (float)Parse(s);
        }
        
        /// <summary>
        /// Converts the System.Boolean to a System.String.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A string representation of the Boolean, that is, "true" or "false".</returns>
        public static string ToString(bool value)
        {
            if (value)
                return "true";
            return "false";
        }
      
        /// <summary>
        /// Converts the System.Byte to a System.String.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A string representation of the Byte.</returns>
        public static string ToString(byte value)
        {
            return value.ToString();
        }
       
        /// <summary>
        /// Converts the System.Char to a System.String.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A string representation of the Char.</returns>
        public static string ToString(char value)
        {
            return value.ToString();
        }
        ////
        //// Summary:
        ////     Converts the System.DateTime to a System.String.
        ////
        //// Parameters:
        ////   value:
        ////     The value to convert.
        ////
        //// Returns:
        ////     A string representation of the DateTime in the format yyyy-MM-ddTHH:mm:ss
        ////     where 'T' is a constant literal.
        //[Obsolete("Use XmlConvert.ToString() that takes in XmlDateTimeSerializationMode")]
        //public static string ToString(DateTime value);
        ////
        //// Summary:
        ////     Converts the supplied System.DateTimeOffset to a System.String.
        ////
        //// Parameters:
        ////   value:
        ////     The System.DateTimeOffset to be converted.
        ////
        //// Returns:
        ////     A System.String representation of the supplied System.DateTimeOffset.
        ////public static string ToString(DateTimeOffset value);
        ////
        //// Summary:
        ////     Converts the System.Decimal to a System.String.
        ////
        //// Parameters:
        ////   value:
        ////     The value to convert.
        ////
        //// Returns:
        ////     A string representation of the Decimal.
        //public static string ToString(decimal value) //note: this type doesn't exist in JSIL
        //{
        //    throw new NotImplementedException();
        //}
       
        /// <summary>
        /// Converts the System.Double to a System.String.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A string representation of the Double.</returns>
        public static string ToString(double value)
        {
            return value.ToString();
        }
       
        /// <summary>
        /// Converts the System.Single to a System.String.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A string representation of the Single.</returns>
        public static string ToString(float value) //--> single
        {
            return value.ToString();
        }
        
        /// <summary>
        /// Converts the System.Guid to a System.String.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A string representation of the Guid.</returns>
        public static string ToString(Guid value)
        {
            return value.ToString();
        }
        
        /// <summary>
        /// Converts the System.Int32 to a System.String.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A string representation of the Int32.</returns>
        public static string ToString(int value) //--> int32
        {
            return value.ToString();
        }
        
        /// <summary>
        /// Converts the System.Int64 to a System.String.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A string representation of the Int64.</returns>
        public static string ToString(long value) //--> int64
        {
            return value.ToString();
        }
        
        /// <summary>
        /// Converts the System.SByte to a System.String.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A string representation of the SByte.</returns>
        public static string ToString(sbyte value) //--> SByte
        {
            return value.ToString();
        }
      
        /// <summary>
        /// Converts the System.Int16 to a System.String.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A string representation of the Int16.</returns>
        public static string ToString(short value) //--> int16
        {
            return value.ToString();
        }
        ////
        //// Summary:
        ////     Converts the System.TimeSpan to a System.String.
        ////
        //// Parameters:
        ////   value:
        ////     The value to convert.
        ////
        //// Returns:
        ////     A string representation of the TimeSpan.
        //public static string ToString(TimeSpan value);
        
        /// <summary>
        /// Converts the System.UInt32 to a System.String.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A string representation of the UInt32.</returns>
        public static string ToString(uint value) //--> UInt32
        {
            return value.ToString();
        }
        
        /// <summary>
        /// Converts the System.UInt64 to a System.String.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A string representation of the UInt64.</returns>
        public static string ToString(ulong value) //--> UInt64
        {
            return value.ToString();
        }
       
        /// <summary>
        /// Converts the System.UInt16 to a System.String.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A string representation of the UInt16.</returns>
        public static string ToString(ushort value) //--> UInt16
        {
            return value.ToString();
        }
        ////
        //// Summary:
        ////     Converts the System.DateTime to a System.String.
        ////
        //// Parameters:
        ////   value:
        ////     The value to convert.
        ////
        ////   format:
        ////     The format structure that defines how to display the converted string. Valid
        ////     formats include "yyyy-MM-ddTHH:mm:sszzzzzz" and its subsets.
        ////
        //// Returns:
        ////     A string representation of the DateTime in the specified format.
        //public static string ToString(DateTime value, string format);
        ////
        //// Summary:
        ////     Converts the System.DateTime to a System.String using the System.Xml.XmlDateTimeSerializationMode
        ////     specified.
        ////
        //// Parameters:
        ////   value:
        ////     The System.DateTime value to convert.
        ////
        ////   dateTimeOption:
        ////     One of the System.Xml.XmlDateTimeSerializationMode values that specify how
        ////     to treat the System.DateTime value.
        ////
        //// Returns:
        ////     A System.String equivalent of the System.DateTime.
        ////
        //// Exceptions:
        ////   System.ArgumentException:
        ////     The dateTimeOption value is not valid.
        ////
        ////   System.ArgumentNullException:
        ////     The value or dateTimeOption value is null.
        //public static string ToString(DateTime value, XmlDateTimeSerializationMode dateTimeOption);
        ////
        //// Summary:
        ////     Converts the supplied System.DateTimeOffset to a System.String in the specified
        ////     format.
        ////
        //// Parameters:
        ////   value:
        ////     The System.DateTimeOffset to be converted.
        ////
        ////   format:
        ////     The format to which s is converted. The format parameter can be any subset
        ////     of the W3C Recommendation for the XML dateTime type. (For more information
        ////     see http://www.w3.org/TR/xmlschema-2/#dateTime.)
        ////
        //// Returns:
        ////     A System.String representation in the specified format of the supplied System.DateTimeOffset.
        //public static string ToString(DateTimeOffset value, string format);
        ////
        //// Summary:
        ////     Converts the System.String to a System.TimeSpan equivalent.
        ////
        //// Parameters:
        ////   s:
        ////     The string to convert. The string format must conform to the W3C XML Schema
        ////     Part 2: Datatypes recommendation for duration.
        ////
        //// Returns:
        ////     A TimeSpan equivalent of the string.
        ////
        //// Exceptions:
        ////   System.FormatException:
        ////     s is not in correct format to represent a TimeSpan value.
        //public static TimeSpan ToTimeSpan(string s);
       
        // Exceptions:
        //   System.ArgumentNullException:
        //     s is null.
        //
        //   System.FormatException:
        //     s is not in the correct format.
        //
        //   System.OverflowException:
        //     s represents a number less than System.UInt16.MinValue or greater than System.UInt16.MaxValue.
        /// <summary>
        /// Converts the System.String to a System.UInt16 equivalent.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <returns>A UInt16 equivalent of the string.</returns>
        public static ushort ToUInt16(string s)
        {
            return (ushort)Parse(s);
        }
        
        // Exceptions:
        //   System.ArgumentNullException:
        //     s is null.
        //
        //   System.FormatException:
        //     s is not in the correct format.
        //
        //   System.OverflowException:
        //     s represents a number less than System.UInt32.MinValue or greater than System.UInt32.MaxValue.
        /// <summary>
        /// Converts the System.String to a System.UInt32 equivalent.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <returns>A UInt32 equivalent of the string.</returns>
        public static uint ToUInt32(string s)
        {
            return (uint)Parse(s);
        }
       
        // Exceptions:
        //   System.ArgumentNullException:
        //     s is null.
        //
        //   System.FormatException:
        //     s is not in the correct format.
        //
        //   System.OverflowException:
        //     s represents a number less than System.UInt64.MinValue or greater than System.UInt64.MaxValue.
        /// <summary>
        /// Converts the System.String to a System.UInt64 equivalent.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <returns>A UInt64 equivalent of the string.</returns>
        public static ulong ToUInt64(string s)
        {
            return (ulong)Parse(s);
        }
        ////
        //// Summary:
        ////     Verifies that the name is a valid name according to the W3C Extended Markup
        ////     Language recommendation.
        ////
        //// Parameters:
        ////   name:
        ////     The name to verify.
        ////
        //// Returns:
        ////     The name, if it is a valid XML name.
        ////
        //// Exceptions:
        ////   System.Xml.XmlException:
        ////     name is not a valid XML name.
        ////
        ////   System.ArgumentNullException:
        ////     name is null or String.Empty.
        //public static string VerifyName(string name);
        ////
        //// Summary:
        ////     Verifies that the name is a valid NCName according to the W3C Extended Markup
        ////     Language recommendation. An NCName is a name that cannot contain a colon.
        ////
        //// Parameters:
        ////   name:
        ////     The name to verify.
        ////
        //// Returns:
        ////     The name, if it is a valid NCName.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     name is null or String.Empty.
        ////
        ////   System.Xml.XmlException:
        ////     name is not a valid non-colon name.
        //public static string VerifyNCName(string name);
        ////
        //// Summary:
        ////     Verifies that the string is a valid NMTOKEN according to the W3C XML Schema
        ////     Part2: Datatypes recommendation
        ////
        //// Parameters:
        ////   name:
        ////     The string you wish to verify.
        ////
        //// Returns:
        ////     The name token, if it is a valid NMTOKEN.
        ////
        //// Exceptions:
        ////   System.Xml.XmlException:
        ////     The string is not a valid name token.
        ////
        ////   System.ArgumentNullException:
        ////     name is null.
        //public static string VerifyNMTOKEN(string name);
        ////
        //// Summary:
        ////     Returns the passed in string instance if all the characters in the string
        ////     argument are valid public id characters.
        ////
        //// Parameters:
        ////   publicId:
        ////     System.String that contains the id to validate.
        ////
        //// Returns:
        ////     Returns the passed-in string if all the characters in the argument are valid
        ////     public id characters.
        //public static string VerifyPublicId(string publicId);
        ////
        //// Summary:
        ////     Verifies that the string is a valid token according to the W3C XML Schema
        ////     Part2: Datatypes recommendation.
        ////
        //// Parameters:
        ////   token:
        ////     The string value you wish to verify.
        ////
        //// Returns:
        ////     The token, if it is a valid token.
        ////
        //// Exceptions:
        ////   System.Xml.XmlException:
        ////     The string value is not a valid token.
        //public static string VerifyTOKEN(string token);
        ////
        //// Summary:
        ////     Returns the passed-in string instance if all the characters in the string
        ////     argument are valid whitespace characters.
        ////
        //// Parameters:
        ////   content:
        ////     System.String to verify.
        ////
        //// Returns:
        ////     Returns the passed-in string instance if all the characters in the string
        ////     argument are valid whitespace characters, otherwise null.
        //public static string VerifyWhitespace(string content);
        ////
        //// Summary:
        ////     Returns the passed-in string if all the characters and surrogate pair characters
        ////     in the string argument are valid XML characters, otherwise null.
        ////
        //// Parameters:
        ////   content:
        ////     System.String that contains characters to verify.
        ////
        //// Returns:
        ////     Returns the passed-in string if all the characters and surrogate-pair characters
        ////     in the string argument are valid XML characters, otherwise null.
        //public static string VerifyXmlChars(string content);


        private static double Parse(string s)
        {
            double number;

            if(double.TryParse(s, out number))
            {
                return number;
            }
            return 0;
        }

    }
}
