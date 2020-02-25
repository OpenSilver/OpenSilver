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
    /// Converts instances of other types to and from instances of System.Windows.Media.Geometry.
    /// </summary>
    public sealed partial class GeometryConverter : TypeConverter
    {
        ///// <summary>
        ///// Initializes a new instance of the System.Windows.Media.Geometry class.
        ///// </summary>
        //public GeometryConverter();

        /// <summary>
        /// Indicates whether an object can be converted from a given type to an instance
        /// of a System.Windows.Media.Geometry.
        /// </summary>
        /// <param name="context">Context information required for conversion.</param>
        /// <param name="sourceType">The source System.Type that is being queried for conversion support.</param>
        /// <returns>
        /// true if object of the specified type can be converted to a System.Windows.Media.Geometry;
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
      
        /// <summary>
        /// Determines whether instances of System.Windows.Media.Geometry can be converted
        /// to the specified type.
        /// </summary>
        /// <param name="context">Context information required for conversion.</param>
        /// <param name="destinationType">
        /// The desired type this System.Windows.Media.Geometry is being evaluated to
        /// be converted to.
        /// </param>
        /// <returns>
        /// true if instances of System.Windows.Media.Geometry can be converted to destinationType;
        /// otherwise, false.
        /// </returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return false;
        }
      
        // Exceptions:
        //   System.NotSupportedException:
        //     Thrown if value is null or is not a valid type which can be converted to
        //     a System.Windows.Media.Geometry.
        /// <summary>
        /// Converts the specified object to a System.Windows.Media.Geometry.
        /// </summary>
        /// <param name="context">Context information required for conversion.</param>
        /// <param name="culture">Cultural information respected during conversion.</param>
        /// <param name="value">The object being converted.</param>
        /// <returns>The System.Windows.Media.Geometry created from converting value.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
                throw GetConvertFromException(value);

            if (value is string)
                return Geometry.INTERNAL_ConvertFromString((string)value);

            return base.ConvertFrom(context, culture, value);
        }
      
        // Exceptions:
        //   System.NotSupportedException:
        //     Thrown if value is null or is not a System.Windows.Media.Geometry, or if
        //     the destinationType cannot be converted into a System.Windows.Media.Geometry.
        /// <summary>
        /// Converts the specified System.Windows.Media.Geometry to the specified type.
        /// </summary>
        /// <param name="context">Context information required for conversion.</param>
        /// <param name="culture">Cultural information respected during conversion.</param>
        /// <param name="value">The System.Windows.Media.Geometry to convert.</param>
        /// <param name="destinationType">The type to convert the System.Windows.Media.Geometry to.</param>
        /// <returns>The object created from converting this System.Windows.Media.Geometry.</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            throw new NotImplementedException();
        }
    }
#endif
}