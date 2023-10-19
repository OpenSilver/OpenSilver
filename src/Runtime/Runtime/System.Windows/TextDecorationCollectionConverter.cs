
/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using System.ComponentModel;
using System.Globalization;

namespace System.Windows
{
    /// <summary>
    /// TypeConverter for TextDecorationCollection 
    /// </summary>   
    internal sealed class TextDecorationCollectionConverter : TypeConverter
    {
        /// <summary>
        /// CanConvertFrom
        /// </summary>
        /// <param name="context"> ITypeDescriptorContext </param>
        /// <param name="sourceType">Type to convert to </param>
        /// <returns> true if it can convert from sourceType to TextDecorations, false otherwise </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        /// <summary>
        /// CanConvertTo method
        /// </summary>
        /// <param name="context"> ITypeDescriptorContext </param>
        /// <param name="destinationType"> Type to convert to </param>
        /// <returns> false will always be returned because TextDecorations cannot be converted to any other type. </returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        /// <summary>
        /// ConvertFrom
        /// </summary>
        /// <param name="context"> ITypeDescriptorContext </param>
        /// <param name="culture"> CultureInfo </param>        
        /// <param name="value"> The input object to be converted to TextDecorations </param>
        /// <returns> the converted value of the input object </returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string source)
            {
                return FromString(source);
            }

            throw GetConvertFromException(value);
        }

        /// <summary>
        /// ConvertTo
        /// </summary>
        /// <param name="context"> ITypeDescriptorContext </param>
        /// <param name="culture"> CultureInfo </param>        
        /// <param name="value"> the object to be converted to another type </param>
        /// <param name="destinationType"> The destination type of the conversion </param>
        /// <returns> null will always be returned because TextDecorations cannot be converted to any other type. </returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException(nameof(destinationType));
            }

            if (destinationType == typeof(string))
            {
                if (value is TextDecorationCollection tdc)
                {
                    return tdc.ToString();
                }
            }

            throw GetConvertToException(value, destinationType);
        }

        internal static TextDecorationCollection FromString(string source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            switch (source.Trim().ToLower())
            {
                case "underline":
                    return TextDecorations.Underline;
                case "strikethrough":
                    return TextDecorations.Strikethrough;
                case "overline":
                    return TextDecorations.OverLine;
                //case "baseline":
                //    return TextDecorations.Baseline;
                case "none":
                    return null;

                default:
                    throw new FormatException($"Failed to create a '{typeof(TextDecorationCollection)}' from '{source}'");
            }
        }
    }
}
