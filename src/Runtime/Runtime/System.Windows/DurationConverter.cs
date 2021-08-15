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
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
#if FOR_DESIGN_TIME
    /// <summary>
    /// Converts instances of System.Windows.Duration to and from other type representations.
    /// </summary>
    public partial class DurationConverter : TypeConverter
    {
        /// <summary>
        /// Determines if conversion from a given type to an instance of System.Windows.Duration
        /// is possible.</summary>
        /// <param name="td">Context information used for conversion.</param>
        /// <param name="t">Type being evaluated for conversion.</param>
        /// <returns>true if t is of type System.String; otherwise, false.</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }
    
        /// <summary>
        /// Determines if conversion to a specified type is possible.
        /// </summary>
        /// <param name="context">Context information used for conversion.</param>
        /// <param name="destinationType">Type being evaluated for conversion.</param>
        /// <returns> if destinationType is of type System.String; otherwise, false.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return false;
        }
     
        /// <summary>
        /// Converts a given string value to an instance of System.Windows.Duration.
        /// </summary>
        /// <param name="td">Context information used for conversion.</param>
        /// <param name="cultureInfo">Cultural information that is respected during conversion.</param>
        /// <param name="value">String value to convert to an instance of System.Windows.Duration.</param>
        /// <returns>A new instance of System.Windows.Duration.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
                throw GetConvertFromException(value);

            if (value is string)
                return Duration.INTERNAL_ConvertFromString((string)value);

            return base.ConvertFrom(context, culture, value);
        }
       
        /// <summary>
        /// Converts an instance of System.Windows.Duration to another type.
        /// </summary>
        /// <param name="context">Context information used for conversion.</param>
        /// <param name="cultureInfo">Cultural information that is respected during conversion.</param>
        /// <param name="value">Duration value to convert from.</param>
        /// <param name="destinationType">Type being evaluated for conversion.</param>
        /// <returns>A new instance of the destinationType.</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo cultureInfo, object value, Type destinationType)
        {
            throw new NotImplementedException();
        }
    }
#endif
}