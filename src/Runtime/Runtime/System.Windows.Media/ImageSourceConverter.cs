
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
using System.Windows.Media.Imaging;

namespace System.Windows.Media
{
    /// <summary>
    /// Converts a <see cref="ImageSource"/> to and from other data types.
    /// </summary>
    public sealed class ImageSourceConverter : TypeConverter
    {
        /// <summary>
        /// Determines whether the converter can convert an object of the given type to an
        /// instance of <see cref="ImageSource"/>.
        /// </summary>
        /// <param name="context">
        /// Type context information used to evaluate conversion.
        /// </param>
        /// <param name="sourceType">
        /// The type of the source that is being evaluated for conversion.
        /// </param>
        /// <returns>
        /// true if the converter can convert the provided type to an instance of <see cref="ImageSource"/>;
        /// otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return (sourceType == typeof(Uri) || sourceType == typeof(string));
        }

        // Note: we override this method to emulate the behavior of the base.CanConvertTo() from
        // Silverlight, which always returns false.

        /// <summary>
        /// Always returns false.
        /// </summary>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return false;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string source)
            {
                UriKind uriKind;
                if (source.Contains(":/"))
                {
                    uriKind = UriKind.Absolute;
                }
                else
                {
                    uriKind = UriKind.Relative;
                }

                return new BitmapImage(new Uri(source, uriKind));
            }
            else if (value is Uri uri)
            {
                return new BitmapImage(uri);
            }

            throw GetConvertFromException(value);
        }

        // Note: we override this method to emulate the behavior of the base.ConvertTo() from
        // Silverlight, which always throws a NotImplementedException.

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// Always throws.
        /// </exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            throw new NotImplementedException($"'{typeof(ImageSourceConverter)}' does not implement '{nameof(ConvertTo)}'.");
        }
    }
}
