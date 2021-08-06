

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


using System;
using System.ComponentModel;
using System.Globalization;

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Converts a <see cref="T:System.Windows.Media.FontFamily" /> object to and from other types.
    /// </summary>
    public class FontFamilyConverter : TypeConverter
    {
        /// <summary>
        /// Determines whether an object of the specified type can be converted to an instance of <see cref="T:System.Windows.Media.FontFamily" />.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="sourceType">The type being evaluated for conversion.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="sourceType" /> is of type <see cref="T:System.String" />; otherwise, <see langword="false" />.</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        /// <summary>
        /// Determines whether an instance of <see cref="T:System.Windows.Media.FontFamily" /> can be converted to the specified type.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="destinationType">The type being evaluated for conversion.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="destinationType" /> is of type <see cref="T:System.String" /> or <see cref="T:System.Windows.Media.FontFamily" />;
        /// otherwise, <see langword="false" />.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (context != null)
                {
                    // When serializing to XAML we want to write the FontFamily as an attribute if and
                    // only if it's a named font family.
                    var fontFamily = context.Instance as FontFamily;

                    // Suppress PRESharp warning that fontFamily can be null; apparently PRESharp
                    // doesn't understand short circuit evaluation of operator &&.
                    return fontFamily != null && fontFamily.Source != null && fontFamily.Source.Length != 0;
                }
                else
                {
                    // Some clients call typeConverter.CanConvertTo(typeof(string)), in which case we
                    // don't have the FontFamily instance to convert. Most font families are named, and
                    // we can always give some kind of name, so return true.
                    return true;
                }
            }
            else if (destinationType == typeof(FontFamily))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        /// <summary>Attempts to convert a specified object to an instance of <see cref="T:System.Windows.Media.FontFamily" />.</summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The object being converted.</param>
        /// <returns>The instance of <see cref="T:System.Windows.Media.FontFamily" /> that is created from the converted <paramref name="value" /> parameter.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="value" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="value" /> is not <see langword="null" /> and is not a valid type that can be converted to a <see cref="T:System.Windows.Media.FontFamily" />.</exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            object result;

            if (value is null)
            {
                throw GetConvertFromException(value);
            }
            else if (value is string)
            {
                result = new FontFamily(value.ToString());
            }
            else
            {
                result = base.ConvertFrom(context, culture, value);
            }

            return result;
        }

        /// <summary>Attempts to convert a <see cref="T:System.Windows.Media.FontFamily" /> to a specified type. </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The <see cref="T:System.Windows.Media.FontFamily" /> to convert.</param>
        /// <param name="destinationType">The type to convert this <see cref="T:System.Windows.Media.FontFamily" /> to.</param>
        /// <returns>The object created from converting this <see cref="T:System.Windows.Media.FontFamily" />.</returns>
        /// <exception cref="T:System.ArgumentException">Occurs if <paramref name="value" /> or <paramref name="destinationType" /> is not a valid type for conversion.</exception>
        /// <exception cref="T:System.ArgumentNullException">Occurs if <paramref name="value" /> or <paramref name="destinationType" /> is <see langword="null" />.</exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            else if (destinationType is null)
            {
                throw new ArgumentNullException(nameof(destinationType));
            }
            else if (value is FontFamily fontFamily)
            {
                if (destinationType == typeof(string))
                {
                    if (fontFamily.Source != null)
                    {
                        // Usual case: it's a named font family.
                        return fontFamily.Source;
                    }
                    else
                    {
                        // If client calls typeConverter.CanConvertTo(typeof(string)) then we'll return
                        // true always, even though we don't have access to the FontFamily instance; so
                        // we need to be able to return some kind of family name even if Source==null.
                        string name = null;

                        throw new NotImplementedException();
                    }
                }
            }
            else
            {
                throw new ArgumentException($"Expected type {nameof(FontFamily)}.");
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
