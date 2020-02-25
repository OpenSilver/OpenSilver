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
    /// Converts instances of the System.String type to and from System.Windows.Media.FontFamily
    /// instances.
    /// </summary>
    public sealed partial class FontFamilyConverter : TypeConverter
    {
        /// <summary>
        /// Determines whether a class can be converted from a given type to an instance
        /// of System.Windows.Media.FontFamily.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="sourceType">The type of the source that is being evaluated for conversion.</param>
        /// <returns>
        /// true if the converter can convert from the specified type to an instance
        /// of System.Windows.Media.FontFamily; otherwise, false.
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
        /// Determines whether an instance of System.Windows.Media.FontFamily can be
        /// converted to a different type.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="destinationType">
        /// The desired type that this instance of System.Windows.Media.FontFamily is
        /// being evaluated for conversion.
        /// </param>
        /// <returns>
        /// true if the converter can convert this instance of System.Windows.Media.FontFamily
        /// to the specified type; otherwise, false.
        /// </returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return false;
        }

        // Exceptions:
        //   System.ArgumentNullException:
        //     o is null.
        //
        //   System.ArgumentException:
        //     o is not null and is not a valid type that can be converted to a System.Windows.Media.FontFamily.
        /// <summary>
        /// Attempts to convert a specified object to an instance of System.Windows.Media.FontFamily.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Cultural-specific information that should be respected during conversion.</param>
        /// <param name="value">The object being converted.</param>
        /// <returns>
        /// The instance of System.Windows.Media.FontFamily that is created from the
        /// converted o parameter.
        /// </returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
                throw GetConvertFromException(value);

            if (value is string)
                return FontFamily.INTERNAL_ConvertFromString((string)value);

            return base.ConvertFrom(context, culture, value);
        }

        // Exceptions:
        //   System.ArgumentException:
        //     Occurs if value or destinationType is not a valid type for conversion.
        //
        //   System.ArgumentNullException:
        //     Occurs if value or destinationType is null.
        /// <summary>
        /// Attempts to convert a specified object to an instance of System.Windows.Media.FontFamily.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Cultural-specific information that should be respected during conversion.</param>
        /// <param name="value">The object being converted.</param>
        /// <param name="destinationType">
        /// The type that this instance of System.Windows.Media.FontFamily is converted
        /// to.
        /// </param>
        /// <returns>The object that is created from the converted instance of System.Windows.Media.FontFamily.</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            throw new NotImplementedException();
        }
    }
#endif
}