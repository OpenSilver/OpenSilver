
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

using System.Diagnostics;

namespace System.Windows
{
    /// <summary>
    /// Refers to the density of a typeface, in terms of the lightness or heaviness of
    /// the strokes.
    /// </summary>
    public struct FontWeight : IFormattable
    {
        private readonly int _weight;

        internal FontWeight(int weight)
        {
            Debug.Assert(1 <= weight && weight <= 999);

            // We want the default zero value of new FontWeight() to correspond to FontWeights.Normal.
            // Therefore, the _weight value is shifted by 400 relative to the OpenType weight value.
            _weight = weight - 400;
        }

        /// <summary>
        /// Determines whether the current <see cref="FontWeight"/> object is equal to a
        /// specified object.
        /// </summary>
        /// <param name="obj">
        /// The object to compare for equality.
        /// </param>
        /// <returns>
        /// true if the values are equal; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
            => obj is FontWeight fontWeight && this == fontWeight;

        /// <summary>
        /// Compares this <see cref="FontWeight"/> structure to another <see cref="FontWeight"/>
        /// structure for equality.
        /// </summary>
        /// <param name="fontWeight">
        /// An instance of <see cref="FontWeight"/> to compare for equality.
        /// </param>
        /// <returns>
        /// true if the two instances of <see cref="FontWeight"/> are equal; otherwise, false.
        /// </returns>
        public bool Equals(FontWeight fontWeight) => this == fontWeight;

        /// <summary>
        /// Retrieves the hash code for this object.
        /// </summary>
        /// <returns>
        /// An integer hash value.
        /// </returns>
        public override int GetHashCode() => RealWeight;

        /// <summary>
        /// Returns a text string that represents the value of the <see cref="FontWeight"/> object.
        /// </summary>
        /// <returns>
        /// A string that represents the value of the <see cref="FontWeight"/> object, such
        /// as "Light" or "Normal".
        /// </returns>
        public override string ToString() => ConvertToString(null, null);

        string IFormattable.ToString(string format, IFormatProvider formatProvider)
            => ConvertToString(format, formatProvider);

        /// <summary>
        /// Compares two font weight values and returns an indication of their relative values.
        /// </summary>
        /// <param name="left">
        /// First object to compare.
        /// </param>
        /// <param name="right">
        /// Second object to compare.
        /// </param>
        /// <returns>
        /// A 32-bit signed integer indicating the lexical relationship between the two comparands.
        /// When the return value is less than zero this means that left is less than right.
        /// When the return value is zero this means that left is equal to right.
        /// When the return value is greater than zero this means that left is greater than right.
        /// </returns>
        public static int Compare(FontWeight left, FontWeight right) => left._weight - right._weight;
        
        /// <summary>
        /// Compares two instances of <see cref="FontWeight"/> for equality.
        /// </summary>
        /// <param name="left">
        /// The first instance of <see cref="FontWeight"/> to compare.
        /// </param>
        /// <param name="right">
        /// The second instance of <see cref="FontWeight"/> to compare.
        /// </param>
        /// <returns>
        /// true if the values of <see cref="FontWeight"/> are equal; otherwise, false.
        /// </returns>
        public static bool operator ==(FontWeight left, FontWeight right) => Compare(left, right) == 0;

        /// <summary>
        /// Evaluates two instances of <see cref="FontWeight"/> to determine inequality.
        /// </summary>
        /// <param name="left">
        /// The first instance of <see cref="FontWeight"/> to compare.
        /// </param>
        /// <param name="right">
        /// The second instance of <see cref="FontWeight"/> to compare.
        /// </param>
        /// <returns>
        /// false if values of left are equal to right; otherwise, true.
        /// </returns>
        public static bool operator !=(FontWeight left, FontWeight right) => !(left == right);

        public static bool operator >(FontWeight left, FontWeight right) => Compare(left, right) > 0;

        public static bool operator <(FontWeight left, FontWeight right) => Compare(left, right) < 0;

        public static bool operator >=(FontWeight left, FontWeight right) => Compare(left, right) >= 0;

        public static bool operator <=(FontWeight left, FontWeight right) => Compare(left, right) <= 0;
        
        /// <summary>
        /// Creates a new FontWeight object that corresponds to the OpenType usWeightClass value.
        /// </summary>
        /// <param name="weightValue">
        /// An integer value between 1 and 999 that corresponds to the usWeightClass definition in 
        /// the OpenType specification.
        /// </param>
        /// <returns>
        /// A new FontWeight object that corresponds to the weightValue parameter.
        /// </returns>
        internal static FontWeight FromOpenTypeWeight(int weightValue)
        {
            if (weightValue < 1 || weightValue > 999)
            {
                throw new ArgumentOutOfRangeException(nameof(weightValue), "The parameter value must be between '1' and '999'.");
            }

            return new FontWeight(weightValue);
        }

        /// <summary>
        /// Obtains OpenType usWeightClass value that corresponds to the FontWeight object.
        /// </summary>
        /// <returns>
        /// An integer value between 1 and 999 that corresponds to the usWeightClass definition 
        /// in the OpenType specification.
        /// </returns>
        internal int ToOpenTypeWeight() => RealWeight;

        /// <summary>
        /// Creates a string representation of this object based on the format string 
        /// and IFormatProvider passed in.  
        /// If the provider is null, the CurrentCulture is used.
        /// See the documentation for IFormattable for more information.
        /// </summary>
        /// <returns>
        /// A string representation of this object.
        /// </returns>
        private string ConvertToString(string format, IFormatProvider provider)
        {
            if (!FontWeights.FontWeightToString(RealWeight, out string convertedValue))
            {
                // This can happen if _weight value is not a multiple of 100.
                return RealWeight.ToString(provider);
            }

            return convertedValue;
        }

        /// <summary>
        /// We want the default zero value of new FontWeight() to correspond to FontWeights.Normal.
        /// Therefore, _weight value is shifted by 400 relative to the OpenType weight value.
        /// </summary>
        private int RealWeight => _weight + 400;
    }
}