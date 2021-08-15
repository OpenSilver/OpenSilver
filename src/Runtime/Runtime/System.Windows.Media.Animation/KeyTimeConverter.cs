﻿

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
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
#if FOR_DESIGN_TIME
    /// <summary>
    /// Converts instances of System.Windows.Media.Animation.KeyTime to and from
    /// other types.
    /// </summary>
    public partial class KeyTimeConverter : TypeConverter
    {
        /// <summary>
        /// Determines whether an object can be converted from a given type to an instance
        /// of a System.Windows.Media.Animation.KeyTime.
        /// </summary>
        /// <param name="typeDescriptorContext">Contextual information required for conversion.</param>
        /// <param name="type">Type being evaluated for conversion.</param>
        /// <returns>true if this type can be converted; otherwise, false.</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }
  
        /// <summary>
        /// Determines if a given type can be converted to an instance of System.Windows.Media.Animation.KeyTime.
        /// </summary>
        /// <param name="typeDescriptorContext">Contextual information required for conversion.</param>
        /// <param name="type">Type being evaluated for conversion.</param>
        /// <returns>true if this type can be converted; otherwise, false.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return false;
        }
      
        /// <summary>
        /// Attempts to convert a given object to an instance of System.Windows.Media.Animation.KeyTime.
        /// </summary>
        /// <param name="typeDescriptorContext">Context information required for conversion.</param>
        /// <param name="cultureInfo">Cultural information that is respected during conversion.</param>
        /// <param name="value">The object being converted to an instance of System.Windows.Media.Animation.KeyTime.</param>
        /// <returns>
        /// A new instance of System.Windows.Media.Animation.KeyTime, based on the supplied
        /// value.
        /// </returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
                throw GetConvertFromException(value);

            if (value is string)
                return KeyTime.INTERNAL_ConvertFromString((string)value);

            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Attempts to convert an instance of System.Windows.Media.Animation.KeyTime
        /// to another type.
        /// </summary>
        /// <param name="typeDescriptorContext">Context information required for conversion.</param>
        /// <param name="cultureInfo">Cultural information that is respected during conversion.</param>
        /// <param name="value">System.Windows.Media.Animation.KeyTime value to convert from.</param>
        /// <param name="destinationType">Type being evaluated for conversion.</param>
        /// <returns>A new object, based on value.</returns>
        public override object ConvertTo(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object value, Type destinationType)
        {
            throw new NotImplementedException();
        }
    }
#endif
}