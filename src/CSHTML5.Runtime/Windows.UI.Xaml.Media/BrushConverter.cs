﻿
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
    /// Used to convert a System.Windows.Media.Brush object to or from another object
    /// type.
    /// </summary>
    public sealed partial class BrushConverter : TypeConverter
    {
        /// <summary>
        /// Determines whether this class can convert an object of a given type to a
        /// System.Windows.Media.Brush object.
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
        //     value is NULL or cannot be converted to a System.Windows.Media.Brush.
        /// <summary>
        /// Converts from an object of a given type to a System.Windows.Media.Brush object.
        /// </summary>
        /// <param name="context">The conversion context.</param>
        /// <param name="culture">The culture information that applies to the conversion.</param>
        /// <param name="value">The object to convert.</param>
        /// <returns>
        /// Returns a new System.Windows.Media.Brush object if successful; otherwise,
        /// NULL.
        /// </returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
                throw GetConvertFromException(value);

            if (value is string)
                return Brush.INTERNAL_ConvertFromString((string)value);

            return base.ConvertFrom(context, culture, value);
        }
        //
        // Summary:
        //     
        //     
        //
        // Parameters:
        //   context:
        //     
        //
        //   culture:
        //     
        //
        //   value:
        //     
        //
        //   destinationType:
        //     
        //
        // Returns:
        //     
        //
        // Exceptions:
        //   System.NotSupportedException:
        //     value is NULL or it is not a System.Windows.Media.Brush-or-destinationType
        //     is not a valid destination type.
        /// <summary>
        /// Converts a System.Windows.Media.Brush object to a specified type, using the
        /// specified context and culture information.
        /// </summary>
        /// <param name="context">The conversion context.</param>
        /// <param name="culture">The current culture information.</param>
        /// <param name="value">The System.Windows.Media.Brush to convert.</param>
        /// <param name="destinationType">The destination type that the value object is converted to.</param>
        /// <returns>An object that represents the converted value.</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            throw new NotImplementedException();
        }
    }
#endif
}