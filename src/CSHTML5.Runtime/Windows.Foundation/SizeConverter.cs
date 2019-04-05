
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
namespace Windows.Foundation
#endif
{
#if FOR_DESIGN_TIME
    /// <summary>
    /// Converts instances of other types to and from instances of System.Windows.Size.
    /// </summary>
    public sealed class SizeConverter : TypeConverter
    {
        /// <summary>
        /// Determines whether the type converter can create an instance of System.Windows.Size
        /// from a specified type.
        /// </summary>
        /// <param name="context">The context information of a type.</param>
        /// <param name="sourceType">The source type that the type converter is evaluating for conversion.</param>
        /// <returns>
        /// true if the type converter can create an instance of System.Windows.Size
        /// from the specified type; otherwise, false.
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
        /// Determines whether the type converter can convert an instance of System.Windows.Size
        /// to a different type.
        /// </summary>
        /// <param name="context">The context information of a type.</param>
        /// <param name="destinationType">
        /// The type for which the type converter is evaluating this instance of System.Windows.Size
        /// for conversion.
        /// </param>
        /// <returns>
        /// true if the type converter can convert this instance of System.Windows.Size
        /// to the destinationType; otherwise, false.
        /// </returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return false;
        }

        // Exceptions:
        //   System.ArgumentNullException:
        //     The source object is a null reference (Nothing in Visual Basic).
        //
        //   System.ArgumentException:
        //     The example object is not a null reference and is not a valid type that can
        //     be converted to a System.Windows.Size.
        /// <summary>
        /// Attempts to create an instance of System.Windows.Size from a specified
        /// object.
        /// </summary>
        /// <param name="context">The context information for a type.</param>
        /// <param name="culture">The System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The sourceSystem.Object being converted.</param>
        /// <returns>An instance of System.Windows.Size created from the converted source.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
                throw GetConvertFromException(value);

            if (value is string)
                return Size.INTERNAL_ConvertFromString((string)value);

            return base.ConvertFrom(context, culture, value);
        }

        // Exceptions:
        //   System.ArgumentNullException:
        //     The value object is not a null reference (Nothing) and is not a Brush, or
        //     the destinationType is not one of the valid types for conversion.
        //
        //   System.ArgumentException:
        //     The value object is a null reference.
        /// <summary>
        /// Attempts to convert an instance of System.Windows.Size to a specified
        /// type.
        /// </summary>
        /// <param name="context">The context information of a type.</param>
        /// <param name="culture">The System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The instance of System.Windows.Size to convert.</param>
        /// <param name="destinationType">The type that this instance of System.Windows.Size is converted to.</param>
        /// <returns>
        /// The type that is created when the type converter converts an instance of
        /// System.Windows.Size.
        /// </returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            throw new NotImplementedException();
        }
    }
#endif
}
