﻿using System.ComponentModel;
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
        /// <see langword="true" /> if <paramref name="destinationType" /> is of type <see cref="T:System.String" />; otherwise, <see langword="false" />.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        // Exceptions:
        //   System.ArgumentNullException:
        //     source is null.
        //
        //   System.NotSupportedException:
        //     source is not null and is not a valid type which can be converted to a System.Windows.Media.FontFamily.
        /// <summary>
        /// Converts the specified object to a System.Windows.Media.FontFamily.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The object being converted.</param>
        /// <returns>The System.Windows.Media.FontFamily created from converting source.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            else if (value.GetType() != typeof(string))
            {
                throw GetConvertFromException(value);
            }

            return FontFamily.INTERNAL_ConvertFromString((string)value);
        }

        /// <summary>Attempts to convert a <see cref="T:System.Windows.Media.DoubleCollection" /> to a specified type. </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The <see cref="T:System.Windows.Media.DoubleCollection" /> to convert.</param>
        /// <param name="destinationType">The type to convert this <see cref="T:System.Windows.Media.DoubleCollection" /> to.</param>
        /// <returns>The object created from converting this <see cref="T:System.Windows.Media.DoubleCollection" />.</returns>
        /// <exception cref="T:System.ArgumentException">Occurs if <paramref name="value" /> or <paramref name="destinationType" /> is not a valid type for conversion.</exception>
        /// <exception cref="T:System.ArgumentNullException">Occurs if <paramref name="value" /> or <paramref name="destinationType" /> is <see langword="null" />.</exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (null == value)
            {
                throw new ArgumentNullException("value");
            }

            FontFamily fontFamily = value as FontFamily;
            if (fontFamily == null)
            {
                throw new ArgumentException($"Expected type {nameof(FontFamily)}.");
            }

            if (null == destinationType)
            {
                throw new ArgumentNullException("destinationType");
            }

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

                    CultureInfo parentCulture = null;
                    if (culture != null)
                    {
                        if (culture.Equals(CultureInfo.InvariantCulture))
                        {
                            culture = null;
                        }
                        else
                        {
                            parentCulture = culture.Parent;
                            if (parentCulture != null &&
                                (parentCulture.Equals(CultureInfo.InvariantCulture) || parentCulture == culture))
                            {
                                parentCulture = null;
                            }
                        }
                    }

                    throw new NotImplementedException();

                    return name;
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
