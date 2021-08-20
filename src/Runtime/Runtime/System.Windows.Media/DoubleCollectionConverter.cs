

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
    /// Used to convert a System.Windows.Media.DoubleCollection object to or from another object
    /// type.
    /// </summary>
    public sealed partial class DoubleCollectionConverter : TypeConverter
    {
        /// <summary>
        /// Determines whether this class can convert an object of a given type to a
        /// System.Windows.Media.DoubleCollection object.
        /// </summary>
        /// <param name="context">The conversion context.</param>
        /// <param name="sourceType">The type from which to convert.</param>
        /// <returns>
        /// Returns true if conversion is possible (object is string type); otherwise,
        /// false.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }
        
        /// <summary>
        /// Determines whether this class can convert an object of a given type to the
        /// specified destination type.
        /// </summary>
        /// <param name="context">The conversion context.</param>
        /// <param name="destinationType">The destination type.</param>
        /// <returns>Returns true if conversion is possible; otherwise, false.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return false;
        }
        
        // Exceptions:
        //   System.NotSupportedException:
        //     value is NULL or cannot be converted to a System.Windows.Media.DoubleCollection.
        /// <summary>
        /// Converts from an object of a given type to a System.Windows.Media.DoubleCollection object.
        /// </summary>
        /// <param name="context">The conversion context.</param>
        /// <param name="culture">The culture information that applies to the conversion.</param>
        /// <param name="value">The object to convert.</param>
        /// <returns>
        /// Returns a new System.Windows.Media.DoubleCollection object if successful; otherwise,
        /// NULL.
        /// </returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
                throw GetConvertFromException(value);

            if (value is string)
                return DoubleCollection.INTERNAL_ConvertFromString((string)value);

            return base.ConvertFrom(context, culture, value);
        }
   
        // Exceptions:
        //   System.NotSupportedException:
        //     value is NULL or it is not a System.Windows.Media.DoubleCollection-or-destinationType
        //     is not a valid destination type.
        /// <summary>
        /// Converts a System.Windows.Media.DoubleCollection object to a specified type, using the
        /// specified context and culture information.
        /// </summary>
        /// <param name="context">The conversion context.</param>
        /// <param name="culture">The current culture information.</param>
        /// <param name="value">The System.Windows.Media.DoubleCollection to convert.</param>
        /// <param name="destinationType">The destination type that the value object is converted to.</param>
        /// <returns>An object that represents the converted value.</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            throw new NotImplementedException();
        }
    }
#endif
}