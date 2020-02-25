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

namespace System.Windows.Input
{
#if FOR_DESIGN_TIME

    /// <summary>
    /// Converts a System.Windows.Input.Cursor object to and from other types.
    /// </summary>
    public partial class CursorConverter : TypeConverter
    {
        /// <summary>
        /// Determines whether an object of the specified type can be converted to an
        /// instance of System.Windows.Input.Cursor, using the specified context.
        /// </summary>
        /// <param name="context">
        /// A format context that provides information about the environment from which
        /// this converter is being invoked.
        /// </param>
        /// <param name="sourceType">The type being evaluated for conversion.</param>
        /// <returns>true if sourceType is of type System.String; otherwise, false.</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }
    
        /// <summary>
        /// Determines whether an instance of System.Windows.Input.Cursor can be converted
        /// to the specified type, using the specified context.
        /// </summary>
        /// <param name="context">
        /// A format context that provides information about the environment from which
        /// this converter is being invoked.
        /// </param>
        /// <param name="destinationType">The type being evaluated for conversion.</param>
        /// <returns>true if destinationType is of type System.String; otherwise, false.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return false;
        }
        
        // Exceptions:
        //   System.NotSupportedException:
        //     value cannot be converted
        /// <summary>
        /// Attempts to convert the specified object to a System.Windows.Input.Cursor,
        /// using the specified context.
        /// </summary>
        /// <param name="context">
        /// A format context that provides information about the environment from which
        /// this converter is being invoked.
        /// </param>
        /// <param name="culture">Culture specific information.</param>
        /// <param name="value">The object to convert.</param>
        /// <returns>The converted object, or null if value is an empty string.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
                throw GetConvertFromException(value);

            if (value is string)
                return Cursor.INTERNAL_ConvertFromString((string)value);


            return base.ConvertFrom(context, culture, value);
        }
   
        // Exceptions:
        //   System.ArgumentNullException:
        //     destinationType is null.
        //
        //   System.NotSupportedException:
        //     source cannot be converted.
        /// <summary>
        /// Attempts to convert a System.Windows.Input.Cursor to the specified type,
        /// using the specified context.
        /// </summary>
        /// <param name="context">
        /// A format context that provides information about the environment from which
        /// this converter is being invoked.
        /// </param>
        /// <param name="culture">Culture specific information.</param>
        /// <param name="value">The object to convert.</param>
        /// <param name="destinationType">The type to convert the object to.</param>
        /// <returns>The converted object, or an empty string if value is null.</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            throw new NotImplementedException();
        }
        
        //todo: (?) the following

        ///// <summary>
        ///// Gets a collection of standard cursor values, using the specified context.
        ///// </summary>
        ///// <param name="context">
        ///// A format context that provides information about the environment from which
        ///// this converter is being invoked.
        ///// </param>
        ///// <returns>A collection that holds a standard set of valid values.</returns>
        //public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context);
       
        ///// <summary>
        ///// Determines whether this object supports a standard set of values that can
        ///// be picked from a list, using the specified context.
        ///// </summary>
        ///// <param name="context">
        ///// A format context that provides information about the environment from which
        ///// this converter is being invoked.
        ///// </param>
        ///// <returns>Always returns true.</returns>
        //public override bool GetStandardValuesSupported(ITypeDescriptorContext context);
    }
#endif
}
