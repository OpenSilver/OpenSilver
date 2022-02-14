
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
using System.Diagnostics;
using System.Globalization;

namespace OpenSilver.Internal
{
    internal static class ObjectExtensions
    {
        internal static string ToInvariantString(this Object obj)
        {
            Debug.Assert(obj != null);
            return obj is IFormattable formattable ? formattable.ToInvariantString() : obj.ToString();
        }

        internal static string ToInvariantString(this double d)
        {
            return d.ToString(CultureInfo.InvariantCulture);
        }

        internal static string ToInvariantString(this double d, string format)
        {
            return d.ToString(format, CultureInfo.InvariantCulture);
        }

        internal static string ToInvariantString(this IFormattable formattable)
        {
            Debug.Assert(formattable != null);
            return FormatInvariantString(formattable, null);
        }

        internal static string ToInvariantString(this IFormattable formattable, string format)
        {
            Debug.Assert(formattable != null);
            return FormatInvariantString(formattable, format);
        }

        private static string FormatInvariantString(IFormattable formattable, string format)
        {
            return formattable.ToString(format, CultureInfo.InvariantCulture);
        }
    }
}
