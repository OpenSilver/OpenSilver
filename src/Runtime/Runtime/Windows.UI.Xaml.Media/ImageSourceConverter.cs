

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
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
#if FOR_DESIGN_TIME
    /// <summary>
    /// Converts a System.Windows.Media.ImageSource to and from other data types.
    /// </summary>
    public sealed partial class ImageSourceConverter : TypeConverter
    {
        /// <summary>
        /// Determines whether the converter can convert an object of the given type
        /// to an instance of System.Windows.Media.ImageSource.</summary>
        /// <param name="context">Type context information used to evaluate conversion.</param>
        /// <param name="sourceType">The type of the source that is being evaluated for conversion.</param>
        /// <returns>
        /// true if the converter can convert the provided type to an instance of System.Windows.Media.ImageSource;
        /// otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }
        
        // Exceptions:
        //   System.ArgumentException:
        //     context instance is not an System.Windows.Media.ImageSource.
        /// <summary>
        /// Determines whether an instance of System.Windows.Media.ImageSource can be
        /// converted to a different type.
        /// </summary>
        /// <param name="context">Type context information used to evaluate conversion.</param>
        /// <param name="destinationType">The desired type to evaluate the conversion to.</param>
        /// <returns>
        /// true if the converter can convert this instance of System.Windows.Media.ImageSource;
        /// otherwise, false.
        /// </returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return false;
        }
      
        // Exceptions:
        //   System.NotSupportedException:
        //     value is null or is an invalid type.
        /// <summary>
        /// Attempts to convert a specified object to an instance of System.Windows.Media.ImageSource.
        /// </summary>
        /// <param name="context">Type context information used for conversion.</param>
        /// <param name="culture">Cultural information that is respected during conversion.</param>
        /// <param name="value">The object being converted.</param>
        /// <returns>A new instance of System.Windows.Media.ImageSource.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
                throw GetConvertFromException(value);

            if (value is string)
                return ImageSource.INTERNAL_ConvertFromString((string)value);

            return base.ConvertFrom(context, culture, value);
        }
      
        // Exceptions:
        //   System.NotSupportedException:
        //     value is null or is not a valid type.-or-context instance cannot serialize
        //     to a string.
        /// <summary>
        /// Attempts to convert an instance of System.Windows.Media.ImageSource to a
        /// specified type.
        /// </summary>
        /// <param name="context">Context information used for conversion.</param>
        /// <param name="culture">Cultural information that is respected during conversion.</param>
        /// <param name="value">System.Windows.Media.ImageSource to convert.</param>
        /// <param name="destinationType">Type being evaluated for conversion.</param>
        /// <returns>A new instance of the destinationType.</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            throw new NotImplementedException();
        }
    }
#endif
}