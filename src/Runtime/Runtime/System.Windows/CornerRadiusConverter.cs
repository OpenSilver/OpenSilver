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
    /// Converts instances of other types to and from a System.Windows.CornerRadius.
    /// </summary>
    public sealed partial class CornerRadiusConverter : TypeConverter
    {
        /// <summary>
        /// Indicates whether an object can be converted from a given type to a System.Windows.CornerRadius.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="sourceType">The source System.Type that is being queried for conversion support.</param>
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
        /// Determines whether System.Windows.CornerRadius values can be converted to
        /// the specified type.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="destinationType">
        /// The desired type this System.Windows.CornerRadius is being evaluated to be
        /// converted to.
        /// </param>
        /// <returns>true if destinationType is of type System.String; otherwise, false.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return false;
        }

        
        // Exceptions:
        //   System.ArgumentNullException:
        //     source is null.
        //
        //   System.ArgumentException:
        //     source is not null and is not a valid type which can be converted to a System.Windows.CornerRadius.
        /// <summary>
        /// Converts the specified object to a System.Windows.CornerRadius.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The object being converted.</param>
        /// <returns>The System.Windows.CornerRadius created from converting source.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
                throw GetConvertFromException(value);

            if (value is string)
                return CornerRadius.INTERNAL_ConvertFromString((string)value);

            return base.ConvertFrom(context, culture, value);
        }

       
        // Exceptions:
        //   System.ArgumentNullException:
        //     value is null.
        //
        //   System.ArgumentException:
        //     value is not null and is not a System.Windows.Media.Brush, or if destinationType
        //     is not one of the valid destination types.
        /// <summary>
        /// Converts the specified System.Windows.CornerRadius to the specified type.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The System.Windows.CornerRadius to convert.</param>
        /// <param name="destinationType">The type to convert the System.Windows.CornerRadius to.</param>
        /// <returns>The object created from converting this System.Windows.CornerRadius (a string).</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            throw new NotImplementedException();
        }
    }
#endif
}
