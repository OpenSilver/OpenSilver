
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

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Describes the degree to which a font has been stretched, compared to the normal
    /// aspect ratio of that font.
    /// </summary>
    public struct FontStretch : IFormattable
    {
        private readonly int _stretch;

        internal FontStretch(int stretch)
        {
            Debug.Assert(1 <= stretch && stretch <= 9);

            // We want the default zero value of new FontStretch() to correspond to FontStretches.Normal.
            // Therefore, the _stretch value is shifted by 5 relative to the OpenType stretch value.
            _stretch = stretch - 5;
        }

        /// <summary>
        /// Compares an object with the current <see cref="FontStretch"/> object.
        /// </summary>
        /// <param name="obj">
        /// The instance of the object to compare for equality.
        /// </param>
        /// <returns>
        /// true if two instances are equal; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
            => obj is FontStretch fontStretch && this == fontStretch;

        /// <summary>
        /// Retrieves the hash code for this object.
        /// </summary>
        /// <returns>
        /// An integer hash value.
        /// </returns>
        public override int GetHashCode() => RealStretch;

        /// <summary>
        /// Creates a <see cref="string"/> representation of the current <see cref="FontStretch"/>
        /// object.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> value representation of the object.
        /// </returns>
        public override string ToString() => ConvertToString(null, null);

        string IFormattable.ToString(string format, IFormatProvider formatProvider)
            => ConvertToString(format, formatProvider);

        /// <summary>
        /// Compares two instances of <see cref="FontStretch"/> for equality.
        /// </summary>
        /// <param name="left">
        /// First instance of <see cref="FontStretch"/> to compare.
        /// </param>
        /// <param name="right">
        /// Second instance of <see cref="FontStretch"/> to compare.
        /// </param>
        /// <returns>
        /// true when the specified <see cref="FontStretch"/> objects are equal; otherwise,
        /// false.
        /// </returns>
        public static bool operator ==(FontStretch left, FontStretch right) => left._stretch == right._stretch;

        /// <summary>
        /// Evaluates two instances of <see cref="FontStretch"/> to determine inequality.
        /// </summary>
        /// <param name="left">
        /// The first instance of <see cref="FontStretch"/> to compare.
        /// </param>
        /// <param name="right">
        /// The second instance of <see cref="FontStretch"/> to compare.
        /// </param>
        /// <returns>
        /// false if left is equal to right; otherwise, true.
        /// </returns>
        public static bool operator !=(FontStretch left, FontStretch right) => !(left == right);

        /// <summary>
        /// Creates a new FontStretch object that corresponds to the OpenType usWidthClass value.
        /// </summary>
        /// <param name="stretchValue">An integer value between 1 and 9 that corresponds
        /// to the usWidthClass definition in the OpenType specification.</param>
        /// <returns>A new FontStretch object that corresponds to the stretchValue parameter.</returns>
        // Important note: when changing this method signature please make sure to update FontStretchConverter accordingly.
        internal static FontStretch FromOpenTypeStretch(int stretchValue)
        {
            if (stretchValue < 1 || stretchValue > 9)
            {
                throw new ArgumentOutOfRangeException(nameof(stretchValue), "The parameter value must be between '1' and '9'.");
            }

            return new FontStretch(stretchValue);
        }

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
            FontStretches.FontStretchToString(RealStretch, out string convertedValue);
            return convertedValue;
        }

        /// <summary>
        /// We want the default zero value of new FontStretch() to correspond to <see cref="FontStretches.Normal"/>.
        /// Therefore, _stretch value is shifted by 5 relative to the OpenType stretch value.
        /// </summary>
        private int RealStretch => _stretch + 5;
    }
}
