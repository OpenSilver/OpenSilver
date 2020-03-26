

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
namespace Windows.Foundation
#endif
{
#if FOR_DESIGN_TIME
    /// <summary>
    /// Converts instances of other types to and from instances of System.Windows.Size.
    /// </summary>
    public sealed partial class SizeConverter : TypeConverter
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
