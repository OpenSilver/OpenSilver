

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
    /// Converts instances of other types to and from an instance of Windows.Foundation.Point.
    /// </summary>
    public sealed partial class PointConverter : TypeConverter
    {
        /// <summary>
        /// Determines whether an object can be converted from a given type to an instance
        /// of a Windows.Foundation.Point.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="sourceType">The type of the source that is being evaluated for conversion.</param>
        /// <returns>
        /// true if the type can be converted to a Windows.Foundation.Point; otherwise,
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
        /// Determines whether an instance of a Windows.Foundation.Point can be converted
        /// to a different type.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="destinationType">The desired type this Windows.Foundation.Point is being evaluated for conversion.</param>
        /// <returns>
        /// true if this Windows.Foundation.Point can be converted to destinationType;
        /// otherwise, false.
        /// </returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return false;
        }

        /// <summary>
        /// Attempts to convert the specified object to a Windows.Foundation.Point.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Cultural information to respect during conversion.</param>
        /// <param name="value">The object being converted.</param>
        /// <returns>The Windows.Foundation.Point created from converting value.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
                throw GetConvertFromException(value);

            if (value is string)
                return Point.INTERNAL_ConvertFromString((string)value);

            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Attempts to convert a Windows.Foundation.Point to a specified type.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The Windows.Foundation.Point to convert.</param>
        /// <param name="destinationType">The type to convert this Windows.Foundation.Point to.</param>
        /// <returns>The object created from converting this Windows.Foundation.Point.</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            throw new NotImplementedException();
        }
    }
#endif
}