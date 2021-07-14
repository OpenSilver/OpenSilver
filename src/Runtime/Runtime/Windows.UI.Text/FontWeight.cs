

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


using System.ComponentModel;
using DotNetForHtml5.Core;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Text
#endif
{
    /// <summary>
    /// Refers to the density of a typeface, in terms of the lightness or heaviness
    /// of the strokes.
    /// </summary>
    [TypeConverter(typeof(FontWeightTypeConverter))]
    public partial struct FontWeight
    {
        /// <summary>
        /// The font weight expressed as a numeric value. See Remarks.
        /// </summary>
        public ushort Weight;

        static FontWeight()
        {
            TypeFromStringConverters.RegisterConverter(typeof(FontWeight), INTERNAL_ConvertFromString);
        }

        internal string INTERNAL_ToHtmlString()
        {
            return Weight.ToString();
        }

        internal static object INTERNAL_ConvertFromString(string fontCode)
        {
            try
            {
                // Check if the font is a named font:
                ushort result;
                if (!ushort.TryParse(fontCode, out result))
                {
                    FontWeights.INTERNAL_FontweightsEnum namedFont = (FontWeights.INTERNAL_FontweightsEnum)Enum.Parse(typeof(FontWeights.INTERNAL_FontweightsEnum), fontCode); // Note: "TryParse" does not seem to work in JSIL.
                    result = (ushort)namedFont;
                }
                return INTERNAL_ConvertFromUshort(result);
            }
            catch (Exception ex)
            {
                throw new Exception("Invalid font: " + fontCode, ex);
            }
        }

        internal static FontWeight INTERNAL_ConvertFromUshort(ushort fontWeightAsUshort)
        {
            return new FontWeight()
            {
                Weight = fontWeightAsUshort
            };
        }

        public override string ToString()
        {
            return Weight.ToString();
        }

        /// <summary>
        /// Compares two font weight values and returns an indication of their relative values.
        /// </summary>
        /// <param name="left">First object to compare.</param>
        /// <param name="right">Second object to compare.</param>
        /// <returns>A 32-bit signed integer indicating the lexical relationship between the two comparands.
        /// When the return value is less than zero this means that left is less than right.
        /// When the return value is zero this means that left is equal to right.
        /// When the return value is greater than zero this means that left is greater than right.
        /// </returns>
        public static int Compare(FontWeight left, FontWeight right)
        {
            return left.Weight - right.Weight;
        }

        public bool Equals(FontWeight fontWeight)
        {
            return this == fontWeight;
        }

        public override bool Equals(object o)
        {
            if (o is FontWeight)
            {
                var fw = (FontWeight)o;
                return this == fw;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.Weight;
        }

        public static bool operator ==(FontWeight left, FontWeight right)
        {
            return Compare(left, right) == 0;
        }

        public static bool operator !=(FontWeight left, FontWeight right)
        {
            return Compare(left, right) != 0;
        }

        public static bool operator >(FontWeight left, FontWeight right)
        {
            return Compare(left, right) > 0;
        }

        public static bool operator <(FontWeight left, FontWeight right)
        {
            return Compare(left, right) < 0;
        }

        public static bool operator >=(FontWeight left, FontWeight right)
        {
            return Compare(left, right) >= 0;
        }

        public static bool operator <=(FontWeight left, FontWeight right)
        {
            return Compare(left, right) <= 0;
        }
    }
}