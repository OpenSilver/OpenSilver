
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
namespace System.Windows
#else
namespace Windows.UI.Text
#endif
{
#if FOR_DESIGN_TIME
    /// <summary>
    /// Converts instances of System.Windows.FontWeight to and from other data types.
    /// </summary>
    public sealed partial class FontWeightConverter : TypeConverter
    {
        /// <summary>
        /// Returns a value that indicates whether this converter can convert an object
        /// of the given type to an instance of System.Windows.FontWeight.
        /// </summary>
        /// <param name="context">Context information of a type.</param>
        /// <param name="sourceType">The type of the source that is being evaluated for conversion.</param>
        /// <returns>
        /// true if the converter can convert the provided type to an instance of System.Windows.FontWeight;
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
        /// Determines whether an instance of System.Windows.FontWeight can be converted
        /// to a different type.
        /// </summary>
        /// <param name="context">Context information of a type.</param>
        /// <param name="destinationType">
        /// The desired type that that this instance of System.Windows.FontWeight is
        /// being evaluated for conversion to.
        /// </param>
        /// <returns>
        /// true if the converter can convert this instance of System.Windows.FontWeight;
        /// otherwise, false.
        /// </returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return false;
        }
      
        // Exceptions:
        //   System.NotSupportedException:
        //     value is null or is not a valid type for conversion.
        /// <summary>
        /// Attempts to convert a specified object to an instance of System.Windows.FontWeight.
        /// </summary>
        /// <param name="context">Context information of a type.</param>
        /// <param name="culture">System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The object being converted.</param>
        /// <returns>The instance of System.Windows.FontWeight created from the converted value.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
                throw GetConvertFromException(value);

            if (value is string)
                return FontWeight.INTERNAL_ConvertFromString((string)value);

            return base.ConvertFrom(context, culture, value);
        }
        
        // Exceptions:
        //   System.NotSupportedException:
        //     value is null-or-value i is not an instance of System.Windows.FontWeight-or-destinationType
        //     is not a valid destination type.
        /// <summary>
        /// Attempts to convert an instance of System.Windows.FontWeight to a specified
        /// type.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The instance of System.Windows.FontWeight to convert.</param>
        /// <param name="destinationType">The type this instance of System.Windows.FontWeight is converted to.</param>
        /// <returns>The object created from the converted instance of System.Windows.FontWeight.</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            throw new NotImplementedException();
        }
    }
#endif
}