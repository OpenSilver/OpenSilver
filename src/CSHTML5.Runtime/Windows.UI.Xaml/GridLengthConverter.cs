
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
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
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
#if FOR_DESIGN_TIME
    /// <summary>
    /// Converts instances of other types to and from System.Windows.GridLength instances.
    /// </summary>
    public sealed class GridLengthConverter : TypeConverter
    {
        /// <summary>
        /// Determines whether a class can be converted from a given type to an instance
        /// of System.Windows.GridLength.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="sourceType">The type of the source that is being evaluated for conversion.</param>
        /// <returns>
        /// true if the converter can convert from the specified type to an instance
        /// of System.Windows.GridLength; otherwise, false.
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
        /// Determines whether an instance of System.Windows.GridLength can be converted
        /// to a different type.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="destinationType">
        /// The desired type that this instance of System.Windows.GridLength is being
        /// evaluated for conversion.
        /// </param>
        /// <returns>
        /// true if the converter can convert this instance of System.Windows.GridLength
        /// to the specified type; otherwise, false.
        /// </returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return false;
        }

        // Exceptions:
        //   System.ArgumentNullException:
        //     source object is null.
        //
        //   System.ArgumentException:
        //     source object is not null and is not a valid type that can be converted to
        //     a System.Windows.GridLength.
        /// <summary>
        /// Attempts to convert a specified object to an instance of System.Windows.GridLength.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Cultural specific information that should be respected during conversion.</param>
        /// <param name="value">The object being converted.</param>
        /// <returns>
        /// The instance of System.Windows.GridLength that is created from the converted
        /// source.
        /// </returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
                throw GetConvertFromException(value);

            if (value is string)
                return GridLength.INTERNAL_ConvertFromString((string)value);

            return base.ConvertFrom(context, culture, value);
        }

        // Exceptions:
        //   System.ArgumentNullException:
        //     destinationType is not one of the valid types for conversion.
        //
        //   System.ArgumentException:
        //     value is null.
        /// <summary>
        /// Attempts to convert an instance of System.Windows.GridLength to a specified
        /// type.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Cultural specific information that should be respected during conversion.</param>
        /// <param name="value">The instance of System.Windows.GridLength to convert.</param>
        /// <param name="destinationType">The type that this instance of System.Windows.GridLength is converted to.</param>
        /// <returns>The object that is created from the converted instance of System.Windows.GridLength.</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            throw new NotImplementedException();
        }
    }
#endif
}